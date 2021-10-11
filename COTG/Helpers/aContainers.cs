
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
using System.Collections.Concurrent;
using System.Linq;
using System;

namespace COTG
{
	public class NotifyCollection< T> :ICollection<T>, IEnumerable<T>, IEnumerable, IList<T>, IReadOnlyCollection<T>, IReadOnlyList<T>, ICollection, IList, INotifyPropertyChanged, INotifyCollectionChanged where T : class
	{
		public ImmutableArray<T> c = ImmutableArray<T>.Empty;

	//	static ImmutableArray<NotifyCollection<T>> propertyChanges = ImmutableArray<NotifyCollection<T>>.Empty;
		static ImmutableArray<(NotifyCollectionChangedEventArgs change, NotifyCollection<T> c)> collectionChanges = ImmutableArray<(NotifyCollectionChangedEventArgs change, NotifyCollection<T> c)>.Empty;


		

		static Task ProcessChanges()
		{
			try
			{
				var counter = maxCollectionChanges/8 + 2;
				do
				{
					(var i,var change) = collectionChanges.FirstOrDefault();
					if(i == default)
						break;
					collectionChanges = collectionChanges.RemoveAt(0);
				
					i.PropertyChanged?.Invoke(i,new(i,"Count") );
					i.CollectionChanged?.Invoke(i,change);
				} while(--counter>0);

			}
			catch(Exception __ex)
			{
				Debug.LogEx(__ex);
			}
			if(collectionChanges.Any())
				ChangesDebounce.Go();
			return Task.CompletedTask;

		}
		static Debounce ChangesDebounce = new(ProcessChanges) { runOnUiThead = true, debounceDelay=300,throttleDelay=1000}; 
		

		bool hasNotifications => (CollectionChanged is not null | PropertyChanged is not null);
		public event NotifyCollectionChangedEventHandler? CollectionChanged;
		//	public void OnPropertyChanged(T city,string propertyName = "") => PropertyChanged?.Invoke(city,new PropertyChangedEventArgs(propertyName));
		
		public event PropertyChangedEventHandler? PropertyChanged;
		//public void OnCollce(string propertyName = "") => PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(propertyName));
		

		public int Count => ((ICollection<T>)c).Count;

		public bool IsReadOnly => ((ICollection<T>)c).IsReadOnly;

		public bool IsSynchronized => ((ICollection)c).IsSynchronized;

		public object SyncRoot => ((ICollection)c).SyncRoot;

		public bool IsFixedSize => ((IList)c).IsFixedSize;

		object IList.this[int index] { get => ((IList)c)[index]; set => ((IList)c)[index]=value; }
		public T this[int index] { get => ((IList<T>)c)[index]; set => ((IList<T>)c)[index]=value; }

	
	//	public event NotifyCollectionChangedEventHandler? CollectionChanged;
			public object _lock;
		protected long lastDataHash;

		//		public int Count => c.Length;
		public NotifyCollection()
		{
			_lock = new();
			
		}
		//	public bool IsReadOnly => true;

		//		T IList<T>.this[int index] { get => c[index]; set=> Assert(false); }

		//	T IList<T>.this[int index] { get => ((IList<T>)c)[index]; set => ((IList<T>)c)[index]=value; }

		//	public T this[int id]  { get => c.IList<T>[id]; set=> Assert(false); }

		//public ref readonly T itemRef(int id) => ref  c.ItemRef(id);
		public void ClearHash() => lastDataHash=-1L;

		public long GetDataHash()
		{
			var hash = 0;
			foreach(var i in c)
			{
				hash = hash*13 + RuntimeHelpers.GetHashCode(i);
			}
			return hash;
		}


		public void NotifyReset(bool itemsChanged=true)
		{
			
			var newHash = GetDataHash();
			var hashChanged = newHash != lastDataHash;
			if( !hashChanged  )
				return;
			lastDataHash= newHash;
			if(hasNotifications)
			{
				NotifyChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset),itemsChanged);
			}

		}

		public void NotifyInsert(int id,T added)
		{
			if(CollectionChanged != null)
			{
				var _count = Count;
					if(_count == Count)
					{
	//				NotifyReset();
					NotifyChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,added,id),true);
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
				if(CollectionChanged!=null)
				{
				
					try
					{

						
						int counter=0;
						foreach(var i in collectionChanges)
						{
							if( i.c==this)
							{ switch(i.change.Action)
							{
							case NotifyCollectionChangedAction.Add:
							case NotifyCollectionChangedAction.Remove:
							case NotifyCollectionChangedAction.Replace:
							case NotifyCollectionChangedAction.Move:
										collectionChanges=collectionChanges.SetItem(counter,( new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset), this));
								goto dontAdd;
									
							case NotifyCollectionChangedAction.Reset:
								goto dontAdd;
									
								default:
								break;
							}
								++counter;
							}
							break;
						}
						collectionChanges=collectionChanges.Add(((args), this));
						dontAdd:

						ChangesDebounce.Go();

					}
					catch(Exception __ex)
					{
						Debug.LogEx(__ex);
					}
				
				}
			}
		}
		public void NotifyRemove(IList<T> removed)
		{
			NotifyChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove,removed as IList),true);
		}

		public void Remove(Predicate< T>  pred, bool notify)
		{
			int id = c.FindIndex(pred);
			if(id >= 0)
			{
				RemoveAt(id, notify);
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
				Insert(Count,item,notify);
			
		}

		public void Add(T i) => Assert(false);
		public  void Insert(int id,T item,bool notify)
		{
			c=c.Insert(id,item);
			
			if(notify && hasNotifications)
			{
				NotifyInsert(id,item);
			}
		}
		public void RemoveAt(int id,bool notify)
		{
			var item = c[id];
			c=c.RemoveAt(id);
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

			c=c.Clear();
			if(!src.IsNullOrEmpty() )
				c=c.AddRange(src);
		
			if(notify || itemsChanged)
				NotifyReset(itemsChanged);
		}

		public void Clear(bool notify)
		{
			Set(null,notify,true);
		}
		// do not use these

	
//		IEnumerator IEnumerable.GetEnumerator() => c.ToArray().GetEnumerator();
		//IEnumerator IEnumerable.GetEnumerator() => c.GetEnumerator();
//		 IEnumerator IEnumerable.GetEnumerator()
	

		public int IndexOf(T item) => c.IndexOf(item);
		public void Insert(int index,T item) => Assert(false);
		public void RemoveAt(int index) => Assert(false);
		//public int Add(T item) => Assert(false); 
		public void Clear() => Assert(false); 
		public bool Contains(T item) => c.Contains(item);
		public void CopyTo(T[] array,int arrayIndex) => c.CopyTo(array,arrayIndex);
		public bool Remove(T item) { Assert(false); return false; }

		public void CopyTo(Array array,int index) => ((ICollection)c).CopyTo(array,index);
		//public bool Contains(T value) => ((IList<T>)c).Contains(value);
		//public int IndexOf(T value) => ((IList<T>)c).IndexOf(value);
		//public void Insert(int index,T value) => Insert(index,value,true);
		// void Remove(T value) => ((IList<T>)c).Remove(value);
		public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)c).GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)c).GetEnumerator();
		 int IList.Add(object value) => ((IList)c).Add(value);
		 bool IList.Contains(object value) => ((IList)c).Contains(value);
		 int IList.IndexOf(object value) => ((IList)c).IndexOf(value);
		 void IList.Insert(int index,object value) => Assert(false);
		 void IList.Remove(object value) => Assert(false);
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
               try
               {

                   //       MainPage.instance.gridCitySource

                   foreach (var i in items)
                   {
					   i.OnPropertyChanged(string.Empty);
                   }
               }
               catch (Exception e)
               {
                   LogEx(e);
               }
          
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
