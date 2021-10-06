
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.System;

namespace COTG
{
	public class NotifyCollection<T> :IReadOnlyCollection<T>, IEnumerable, IEnumerable<T>, INotifyCollectionChanged,INotifyPropertyChanged where T : class
	{
		public List<T> c = new();
		T [] cache;
		public NotifyCollection()
		{

		}
		bool hasNotifications => (CollectionChanged is not null | PropertyChanged is not null);
		public event NotifyCollectionChangedEventHandler? CollectionChanged;
		public void OnPropertyChanged(T city,string propertyName = "") => PropertyChanged?.Invoke(city,new PropertyChangedEventArgs(propertyName));
		public void OnPropertyChanged(string propertyName = "") => PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(propertyName));
		public event PropertyChangedEventHandler? PropertyChanged;

		protected long lastDataHash;

		public int Count => c.Count;
		public T  this[int id] => c[id];

		public void ClearHash() => lastDataHash=-1L;

		public static long GetDataHash(IEnumerable<T> v)
		{
			var hash = 0;
			foreach(var i in v)
			{
				hash = hash*13 + RuntimeHelpers.GetHashCode(i);
			}
			return hash;
		}


		public void NotifyReset(bool itemsChanged=true)
		{
			
			var newHash = GetDataHash(c);
			var hashChanged = newHash != lastDataHash;
		//	if( !hashChanged && !itemsChanged )
		//		return;
			lastDataHash= newHash;
		//	if(hasNotifications)
			{
					Debounce.Q( hash: RuntimeHelpers.GetHashCode(this)*11,runOnUIThread:true,action: ()=> {
					try
					{
			//			if(itemsChanged)
							OnPropertyChanged();
					//  Assert(App.IsOnUIThread());
						if( hashChanged)
						{
							if(CollectionChanged is not null)
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
				var _count = Count;
					if(_count == Count)
					{
						NotifyChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,added,id),true);
					}
					else
					{
						NotifyReset();
					}

				
			}
		}

		public void NotifyAdd(IList<T> added)
		{
			if(CollectionChanged != null)
			{
				NotifyChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,added),true);
			}

		}
		public void NotifyRemoveAt(int id,T removed)
		{
				var _count = Count;
						if(_count == Count)
						{
							NotifyChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove,removed,id),true);
						}
						else
						{
							NotifyReset(true);				
						}
					
			

		}

		public void NotifyChange(NotifyCollectionChangedEventArgs args, bool sizeChanged)
		{
			if(hasNotifications)
			{
				App.DispatchOnUIThreadIdle(() =>
				{
					try
					{

						if(sizeChanged)
							OnPropertyChanged();

						CollectionChanged(this,args);


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
			NotifyChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove,removed as IList),true);
		}

		public void Remove(Predicate<T>  pred, bool notify)
		{
			int id = c.FindIndex(pred);
			if(id >= 0)
			{
				RemoveAt(id, notify);
			}
		}

		public void SortSmall(Comparison<T> cmp)
		{
			c.SortSmall(cmp);
			NotifyReset(true);

		}
		public void Add(T item,bool notify)
		{
			var id = Count;
			c.Add(item);
			if(notify)
			{
				NotifyInsert(id,item);
			}
		}
		public  void Insert(int id,T item,bool notify)
		{
			c.Insert(id,item);
			notify=true;
			if(notify && hasNotifications)
			{
				NotifyInsert(id,item);
			}
		}
		public void RemoveAt(int id,bool notify)
		{
			var item = c[id];
			c.RemoveAt(id);
			if(notify && hasNotifications)
				NotifyRemoveAt(id,item);
		}
		public void Remove(T i,bool notify)
		{
			var index = c.IndexOf(i);
			if(index >= 0)
			{
				RemoveAt(index,notify);
			}
		}

		public void Set(IEnumerable<T> src,bool notify, bool itemsChanged=true)
		{
		//	if(notify == )
		//	if(src.SequenceEqual(this))
		//		return;

			c.Clear();
			if(!src.IsNullOrEmpty() )
				c.AddRange(src);
			if(notify && hasNotifications)
				NotifyReset(itemsChanged);
		}

		public void Clear(bool notify)
		{
			Set(null,notify,true);
		}
		// do not use these
		

		public IEnumerator<T> GetEnumerator()
		{
			return c.GetEnumerator();
			cache = c.ToArray();
			return cache.AsEnumerable<T>().GetEnumerator();
		}
//		IEnumerator IEnumerable.GetEnumerator() => c.ToArray().GetEnumerator();
		//IEnumerator IEnumerable.GetEnumerator() => c.GetEnumerator();
		 IEnumerator IEnumerable.GetEnumerator()
		{
			return c.GetEnumerator();
			cache = c.ToArray();
			return cache.GetEnumerator();
		}
		
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
