﻿using COTG.Game;
using COTG.Views;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.System;

using static COTG.Debug;
namespace COTG
{

    // All updates should happen on the UI thread atomically and synchronously which will reduce the need for synchronization
    // we do not track individual properties
    public class DumbCollection<T> : List<T>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;
		public void OnPropertyChanged(T city, string propertyName = "") => PropertyChanged?.Invoke(city, new PropertyChangedEventArgs(propertyName));
		public void OnPropertyChanged( string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		public event PropertyChangedEventHandler PropertyChanged;


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
			if (CollectionChanged != null)
			{
				App.QueueOnUIThread(() =>{

				   //  Assert(App.IsOnUIThread());
				try
				{
					CollectionChanged(this,new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));


				}
				catch(Exception __ex)
				{
					Debug.LogEx(__ex);
				} }			);
			}
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
			//App.DispatchOnUIThreadLow(() =>
   //         {
   //             // catch for thread safety
               
                NotifyReset();
			//   });
			
        }
        public new void Clear()
        {
            Set(null);
        }


		//       public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		//public new Task AddAsync(T item)
		//{
		//	int id = Count;
		//	base.Add(item);
		//	if (CollectionChanged != null)
		//		return App.DispatchOnUIThreadTask(() => CollectionChanged.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, id)));
		//	else
		//		return Task.CompletedTask;
		//}

		public new void Add(T item)
        {
            base.Add(item);
			if(CollectionChanged!=null)
	            App.DispatchOnUIThreadLow( ()=>
				{
					try
					{
						var id = IndexOf(item);
						if(id != -1)
							CollectionChanged.Invoke(this,new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,item,id));
						else
							CollectionChanged.Invoke(this,new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

					}
					catch(Exception __ex)
					{
						Debug.LogEx(__ex);
					}
				});
        }
        public new void Insert(int id, T item)
        {
            base.Insert(id, item);
			if (CollectionChanged != null)
				App.DispatchOnUIThreadLow(() =>
				{
					try
					{
						var id = IndexOf(item);
						if(id != -1)
							CollectionChanged.Invoke(this,new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,item,id));
						else
							CollectionChanged.Invoke(this,new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

					}
					catch(Exception __ex)
					{
						Debug.LogEx(__ex);
					}
				});
        }
        public new void RemoveAt(int id)
        {
            var item = base[id];
            base.RemoveAt(id);
			var count = Count;
			if (CollectionChanged != null)
				App.DispatchOnUIThreadLow(() =>
				{
					try
					{
						if(id <= Count)
							CollectionChanged.Invoke(this,new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove,item,id));
						else
							CollectionChanged.Invoke(this,new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

					}
					catch(Exception __ex)
					{
						Debug.LogEx(__ex);
					}
				});
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
	public class ResetableCollection<T> : List<T>, INotifyCollectionChanged, INotifyPropertyChanged
	{
		public event NotifyCollectionChangedEventHandler CollectionChanged;
		public void OnPropertyChanged(T city, string propertyName = "") => PropertyChanged?.Invoke(city, new PropertyChangedEventArgs(propertyName));
		public void OnPropertyChanged(string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		public event PropertyChangedEventHandler PropertyChanged;


		public static ResetableCollection<T> empty = new();

		public void NotifyReset(T[] changed = null)
		{
			App.DispatchOnUIThreadIdle(() =>
			{
				OnPropertyChanged();
				//  Assert(App.IsOnUIThread());
				if (CollectionChanged != null)
				{
					if (changed != null)
					{
						CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset, changed as IList ));
					}
					else
					{
						CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
					}
				}
		});

		}
		public Task NotifyResetAsync(T[] changed = null)
		{
			return App.DispatchOnUIThreadTask(  () =>
			{
				OnPropertyChanged();
				//  Assert(App.IsOnUIThread());
				if (CollectionChanged != null)
				{
					if (changed != null)
					{
						CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset, changed as IList));
					}
					else
					{
						CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
					}
				}
				return Task.CompletedTask;
			});

		}
		public void NotifyItemsChanged()
		{
			App.DispatchOnUIThreadIdle(() =>
			{
				foreach(var i in this)
					OnPropertyChanged(i);
			});

		}
		public void NotifyAdd( T added )
		{
			App.DispatchOnUIThreadLow(() =>
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
			App.DispatchOnUIThreadLow(() =>
			{
				//  Assert(App.IsOnUIThread());
				if (CollectionChanged != null)
				{
					CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, added as IList));
				}
			});
			
		}
		public void NotifyRemove(T removed)
		{
			App.DispatchOnUIThreadLow(() =>
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
			App.DispatchOnUIThreadLow(() =>
			{
				//  Assert(App.IsOnUIThread());
				if (CollectionChanged != null)
				{
					CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removed as IList));
				}
			});
		}
		//public void NotifyMove(T moved, int oldIndex, int newIndex)
		//{
		//	App.DispatchOnUIThreadSneakyLow(() =>
		//	{
		//		//  Assert(App.IsOnUIThread());
		//		if (CollectionChanged != null)
		//		{
		//			CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move,moved,newIndex, oldIndex ));
		//		}
		//	});
		//}
		public void Set( IEnumerable<T> src)
		{
			Clear();
			base.AddRange(src);
			NotifyReset();
		}
		public new void AddRange(IEnumerable<T> src)
		{
			Assert(false);
			base.AddRange(src);
			if (src is IList<T> _src)
			{
				NotifyAdd(_src);
			}
			else
			{
				Assert(false);
				NotifyReset();
			}
		}

	}
	public class ACollection<T> : ObservableCollection<T>
	{
		public void NotifyReset() => OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		public void NotifyPropertyChange() => OnPropertyChanged(new PropertyChangedEventArgs(string.Empty));
	}
	public static class DumbHelpers
    {
        public static void NotifyChange(this HashSet<City> items, params string[] memberName)
        {
            if (items.Count == 0)
                return;

            // defer the call, we don't need it right away
           App.DispatchOnUIThreadIdle(() =>
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
		public static void SyncList<T>(this IEnumerable<T> from, IList<T> to)
		{
			int iter = to.Count;
			while (--iter >= 0)
			{
				var b = to[iter];
				if (!from.Any(a => EqualityComparer<T>.Default.Equals(a, b)))
					to.RemoveAt(iter);
			}
			foreach (var b in from)
			{
				if (!to.Any(a => EqualityComparer<T>.Default.Equals(a,b)))
					to.Add(b);
			}
		}
		public static void SyncList<T>(this HashSet<T> from, IList<T> to)
		{
			int iter = to.Count;
			while (--iter >= 0)
			{
				var b = to[iter];
				if (!from.Contains(b))
					to.RemoveAt(iter);
			}
			foreach (var b in from)
			{
				if (!to.Any(a => EqualityComparer<T>.Default.Equals(a, b)))
					to.Add(b);
			}
		}
		public static void SyncList<T0, T1>(this IEnumerable<T0> from, IList<T1> to, Func<T0, T1, bool> equals, Func<T0, T1> convert)
		{
			int iter = to.Count;
			while (--iter >= 0)
			{
				var b = to[iter];
				if (!from.Any(a => equals(a, b)))
					to.RemoveAt(iter);
			}
			foreach (var b in from)
			{
				if (!to.Any(a => equals(b, a)))
					to.Add(convert(b));
			}
		}

	}
}
