
using Microsoft.UI.Xaml.Data;

using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.WinUI.UI;

namespace COTG
{
	public class NotifyCollection< T> :AdvancedCollectionView<T>, INotifyCollectionChanged,INotifyPropertyChanged where T : class
	{
		bool hasNotifications =>true;
	//	public event NotifyCollectionChangedEventHandler? CollectionChanged;
		public void OnPropertyChanged(T city,string propertyName = "") => PropertyChanged?.Invoke(city,new PropertyChangedEventArgs(propertyName));
		public void OnPropertyChanged(string propertyName = "") => PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(propertyName));
		public object _lock;
		protected long lastDataHash;

		//		public int Count => c.Length;
		public NotifyCollection()
		{
			_lock = new();
			BindingOperations..
		}
		//	public bool IsReadOnly => true;

		//		T IList<T>.this[int index] { get => c[index]; set=> Assert(false); }

		//	T IList<T>.this[int index] { get => ((IList<T>)c)[index]; set => ((IList<T>)c)[index]=value; }

		//	public T this[int id]  { get => c.IList<T>[id]; set=> Assert(false); }

		//public ref readonly T itemRef(int id) => ref  c.ItemRef(id);
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
	//		if( !hashChanged && !itemsChanged )
	//			return;
			lastDataHash= newHash;
			if(hasNotifications)
			{
					Debounce.Q( hash: RuntimeHelpers.GetHashCode(this)*113,runOnUIThread:true,action: ()=> {
					try
					{
						if(itemsChanged)
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
					NotifyReset();
//					NotifyChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,added,id),true);
					}
					else
					{
						NotifyReset();
					}

				
			}
		}

		//public void NotifyAdd(IList<T> added)
		//{
		//	if(CollectionChanged != null)
		//	{
		//		NotifyChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,added),true);
		//	}

		//}
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
			int id = FindIndex(pred);
			if(id >= 0)
			{
				base.RemoveItem(id);
			}
		}

		public void SortSmall(Comparison<T> cmp)
		{
			Sort.SortSmall(this, cmp);
			NotifyReset(true);

		}
		public void Add(T item,bool notify)
		{
	//		if(notify)
				base.Add(item);
//			Insert(Count,item,notify);
			
		}
		public  void Insert(int id,T item,bool notify)
		{
			base.InsertItem(id,item);
			
			if(notify && hasNotifications)
			{
				NotifyInsert(id,item);
			}
		}
		public void RemoveAt(int id,bool notify)
		{
			var item = c[id];
			base.RemoveAt(id);
			if(notify && hasNotifications)
				NotifyRemoveAt(id,item);
		}
		public void Remove(T i,bool notify)
		{
			var index = base.IndexOf(i);
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

			base.Clear();
			if(!src.IsNullOrEmpty() )
				base.AddRange(src);
		
			if(notify || itemsChanged)
				NotifyReset(itemsChanged);
		}

		public void Clear(bool notify)
		{
			Set(null,notify,true);
		}
		// do not use these

		 record struct EnumeratorProxy :IEnumerator< T>  
		{
			 NotifyCollection<T> c;
			public EnumeratorProxy(NotifyCollection<T> c) { this .c =c; e = c.c.GetEnumerator();}
			ImmutableArray<T>.Enumerator e;
			public T Current => e.Current;
			object IEnumerator.Current => e.Current;


			public void Dispose(){ }
			public bool MoveNext() => e.MoveNext();
			public void Reset() => e = c.c.GetEnumerator();
		}
		public IEnumerator<T> GetEnumerator()
		{
			return new EnumeratorProxy(this);
		}
//		IEnumerator IEnumerable.GetEnumerator() => c.ToArray().GetEnumerator();
		//IEnumerator IEnumerable.GetEnumerator() => c.GetEnumerator();
		 IEnumerator IEnumerable.GetEnumerator()
		{
			return  new c.IList<T>.EnumeratorProxy(this);
		}

		public int IndexOf(T item) => c.IndexOf(item);
		public void Insert(int index,T item) => Assert(false);
		public void RemoveAt(int index) => Assert(false);
		public void Add(T item) => Assert(false); 
		public void Clear() => Assert(false); 
		public bool Contains(T item) => c.Contains(item);
		public void CopyTo(T[] array,int arrayIndex) => c.CopyTo(array,arrayIndex);
		public bool Remove(T item) { Assert(false); return false; } 
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
