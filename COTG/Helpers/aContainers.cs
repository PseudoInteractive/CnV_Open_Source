using COTG.Game;
using COTG.Views;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using static COTG.Debug;

namespace COTG
{
	public class NotifyCollection<T> : List<T>, INotifyCollectionChanged,INotifyPropertyChanged
	{
		public event NotifyCollectionChangedEventHandler CollectionChanged;
		public void OnPropertyChanged(T city,string propertyName = "") => PropertyChanged?.Invoke(city,new PropertyChangedEventArgs(propertyName));
		public void OnPropertyChanged(string propertyName = "") => PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(propertyName));
		public event PropertyChangedEventHandler PropertyChanged;

		protected long lastHash;

		public static long GetHashCode(IEnumerable<T> v)
		{
			var hash = 0;
			foreach(var i in v)
			{
				hash = hash*13 + RuntimeHelpers.GetHashCode(v);
			}
			return hash;
		}


		public void NotifyReset(bool itemsChanged)
		{
			
			var newHash = GetHashCode(this);
			var hashChanged = newHash != lastHash;
			if( !hashChanged && !itemsChanged )
				return;
			lastHash= newHash;

			if(CollectionChanged != null)
			{
				App.DispatchOnUIThreadIdle(() =>
				{
					try
					{
						if(itemsChanged)
							OnPropertyChanged();
					//  Assert(App.IsOnUIThread());
						if( hashChanged)
						{
							CollectionChanged(this,new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
						}

					}
					catch(Exception __ex)
					{
						Debug.LogEx(__ex);
					}
				});
			}

		}

		public void NotifyInsert(int id,T added)
		{
			if(CollectionChanged != null)
			{
				App.DispatchOnUIThreadIdle(() =>
				{
					try
					{
						//  Assert(App.IsOnUIThread());
						try
						{
							CollectionChanged(this,new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,added,id));


						}
						catch(Exception __ex)
						{
							Debug.LogEx(__ex);
						}

					}
					catch(Exception __ex)
					{
						Debug.LogEx(__ex);
					}				
				});
			}
		}

		public void NotifyAdd(IList<T> added)
		{
			if(CollectionChanged != null)
			{
				App.DispatchOnUIThreadIdle( () =>
				{
					//  Assert(App.IsOnUIThread());
					try
					{
						CollectionChanged(this,new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,added as IList));


					}
					catch(Exception __ex)
					{
						Debug.LogEx(__ex);
					}				
				});
			}

		}
		public void NotifyRemoveAt(int id,T removed)
		{
			if(CollectionChanged != null )
			{
				var _count = Count;
				App.DispatchOnUIThreadIdle(() =>
				{
					try
					{
						if(id <= _count)
							CollectionChanged.Invoke(this,new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove,removed,id));
						else
						{
							NotifyReset();
						}
					}
					catch(Exception __ex)
					{
						Debug.LogEx(__ex);
					}
				});
			}
		}
		
		public void NotifyRemove(IList<T> removed)
		{
			if(CollectionChanged != null)
			{
				App.DispatchOnUIThreadIdle(() =>
				{
					try
					{
						CollectionChanged(this,new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove,removed as IList));


					}
					catch(Exception __ex)
					{
						Debug.LogEx(__ex);
					}			
				});
			}
		}
		public void Add(T item,bool notify)
		{
			var id = Count;
			base.Add(item);
			if(notify)
				NotifyInsert(id,item);
		}
		public  void Insert(int id,T item,bool notify)
		{
			base.Insert(id,item);
			if(notify && CollectionChanged != null)
			{
				NotifyInsert(id,item);
			}
		}
		public void RemoveAt(int id,bool notify)
		{
			var item = base[id];
			base.RemoveAt(id);
			if(notify)
				NotifyRemoveAt(id,item);
		}
		public void Remove(T i,bool notify)
		{
			var index = IndexOf(i);
			if(index >= 0)
			{
				RemoveAt(index);
				if(notify)
					NotifyRemoveAt(index,i);
			}
		}

		public void Set(IEnumerable<T> src,bool notify)
		{
			if(src.SequenceEqual(this))
				return;

			base.Clear();
			if(!src.IsNullOrEmpty() )
				base.AddRange(src);
			if(notify)
				NotifyReset();
		}
		public void Clear(bool notify)
		{
			Set(null,notify);
		}
		// do not use these
		private new void Clear() { }
		private new void Add(T a) { }
		private new void Insert(int a,T b) { }
		private new void Remove(T a) { }
		private new void RemoveAt(int a) { }
		private new void AddRange(IEnumerable<T>   a) { }
	}



	//public class ACollection<T> : ObservableCollection<T>
	//{
	//	public void NotifyReset() => OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
	//	public void NotifyPropertyChange() => OnPropertyChanged(new PropertyChangedEventArgs(string.Empty));
	//}
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
