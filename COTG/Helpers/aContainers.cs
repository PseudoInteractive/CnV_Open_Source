using COTG.Game;
using COTG.Views;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.System;

using static COTG.Debug;
namespace COTG.Helpers
{

    // All updates should happen on the UI thread atomically and synchronously which will reduce the need for synchronization
    // we do not track individual properties
    public class DumbCollection<T> : List<T>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public DumbCollection(IList<T> collection)
        {
            Set(collection);
        }

        public DumbCollection()
        {
        }



        //public void NotifyChange(T changedItem)
        //{
        //    var dummy = PropertyChanged;
        //    PropertyChanged?.Invoke(changedItem, new PropertyChangedEventArgs(string.Empty));

        //    //            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, changedItem, changedItem));
        //}


        public void NotifyReset()
        {
          //  Assert(App.IsOnUIThread());
            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void Set(IEnumerable<T> src)
        {
			if (Count>0)
				base.Clear();
			else
			{

				if (src==null)
				{
					return;
				}
			}
			
			if (src != null)
			{
				base.AddRange(src);
			}
			//App.DispatchOnUIThreadSneaky(() =>
   //         {
   //             // catch for thread safety
               
                NotifyReset();
			//   });
			
        }
        public new void Clear()
        {
            Set(null);
        }

        public void OnPropertyChanged(T city, string propertyName = "") => PropertyChanged?.Invoke(city, new PropertyChangedEventArgs(propertyName));
        public event PropertyChangedEventHandler PropertyChanged;

        //       public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


        public new void Add(T item)
        {
            int id = Count;
            base.Add(item);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, id));
        }
        public new void Insert(int id, T item)
        {
            base.Insert(id, item);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, id));
        }
        public new void RemoveAt(int id)
        {
            var item = base[id];
            base.RemoveAt(id);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, id));
        }
        public new void Remove(T i)
        {
            var index = IndexOf(i);
            if (index >= 0)
            {
                RemoveAt(index);
            }
        }


    }

	// A List that allows manual reset notifications to be sent for large scale changes
	// does not support find grained changes, any changes should be promoted to a reset
	public class ResetableCollection<T> : List<T>, INotifyCollectionChanged
	{
		public event NotifyCollectionChangedEventHandler CollectionChanged;

		public void NotifyReset(T changed = default)
		{
			App.DispatchOnUIThreadSneakyLow(() =>
			{
				//  Assert(App.IsOnUIThread());
				if (CollectionChanged != null)
				{
					if (changed != null)
					{
						CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset, changed));
					}
					else
					{
						CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
					}
				}
		});

		}

		public void NotifyAdd( T added )
		{
			App.DispatchOnUIThreadSneakyLow(() =>
			{
				//  Assert(App.IsOnUIThread());
				if (CollectionChanged != null)
				{
					CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, added));
				}
			});
		}
		public void NotifyAdd(IList<T> added)
		{
			App.DispatchOnUIThreadSneakyLow(() =>
			{
				//  Assert(App.IsOnUIThread());
				if (CollectionChanged != null)
				{
					CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, added));
				}
			});
		}
		public void NotifyRemove(T removed)
		{
			App.DispatchOnUIThreadSneakyLow(() =>
			{
				//  Assert(App.IsOnUIThread());
				if (CollectionChanged != null)
				{
					CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removed));
				}
			});
		}
		public void NotifyRemove(IList<T> removed)
		{
			App.DispatchOnUIThreadSneakyLow(() =>
			{
				//  Assert(App.IsOnUIThread());
				if (CollectionChanged != null)
				{
					CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removed));
				}
			});
		}
		public void NotifyMove(T moved, int oldIndex, int newIndex)
		{
			App.DispatchOnUIThreadSneakyLow(() =>
			{
				//  Assert(App.IsOnUIThread());
				if (CollectionChanged != null)
				{
					CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move,moved,newIndex, oldIndex ));
				}
			});
		}
		public void Set( IEnumerable<T> src)
		{
			Clear();
			AddRange(src);
			NotifyReset();
		}

	}
	public static class DumbHelpers
    {
        public static void NotifyChange(this HashSet<City> items, params string[] memberName)
        {
            if (items.Count == 0)
                return;

            // defer the call, we don't need it right away
           App.DispatchOnUIThreadSneakyLow(() =>
           {
               try
               {

                   //       MainPage.instance.gridCitySource

                   foreach (var i in items)
                   {
                       if (memberName.IsNullOrEmpty())
                       {
						   i.OnPropertyChanged(string.Empty);
					   }
                       else
                       {
                           foreach (var m in memberName)
                           {
                               i.OnPropertyChanged(m);
                           }
                       }

                   }
               }
               catch (Exception e)
               {
                   LogEx(e);
               }
           });
        }


    }
}
