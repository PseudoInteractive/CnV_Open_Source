
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
using Windows.Foundation.Collections;
using Windows.Foundation;

using ChangeCollection = System.Collections.Immutable.ImmutableArray<(System.Collections.Specialized.NotifyCollectionChangedEventArgs change, COTG.NotifyCollectionBase c)>;
namespace COTG
{
	public abstract class NotifyCollectionBase: INotifyPropertyChanged, INotifyCollectionChanged 
	{
	public static AsyncReaderWriterLock _lock = new();
		public static long GetDataHash<T>(IEnumerable<T> c)
		{
			var hash = 0;
			foreach(var i in c)
			{
				hash = hash*13 + RuntimeHelpers.GetHashCode(i);
			}
			return hash;
		}
		public static ImmutableArray<NotifyCollectionBase> all = ImmutableArray<NotifyCollectionBase>.Empty;

		public bool hasNotifications => (CollectionChanged is not null | PropertyChanged is not null);



		public event NotifyCollectionChangedEventHandler? CollectionChanged;
		//	public void OnPropertyChanged(T city,string propertyName = "") => PropertyChanged?.Invoke(city,new PropertyChangedEventArgs(propertyName));

		public event PropertyChangedEventHandler? PropertyChanged;
	

		//public object _lock;

		public object[] GetObservors()
		{
			if(CollectionChanged is not null)
			{
				return CollectionChanged.GetInvocationList().Select(i => i.Target).ToArray();
			}
			return Array.Empty<object>();
		}

		public NotifyCollectionBase()
		{
			all = all.Add(this);
		}
		public void Dispose()
		{
			all = all.Remove(this);  // TODO Async
		}

		public static void ResetAll(bool clearHash)
		{
			foreach(var col in NotifyCollectionBase.all)
			{
				col.NotifyReset(true,clearHash);
			}
		}
		static ChangeCollection collectionChanges = ImmutableArray<(NotifyCollectionChangedEventArgs change, NotifyCollectionBase c)>.Empty;

		static Task ProcessChanges()
		{
			try
			{
				int before = collectionChanges.Length;
				var counter = collectionChanges.Length/8 + 2;
				do
				{
					if(!collectionChanges.Any())
						break;

						try
						{

							(var change, var i) = collectionChanges[0];
							collectionChanges = collectionChanges.RemoveAt(0);
						switch(change.Action)
						{
							case NotifyCollectionChangedAction.Add:
								Assert(change.NewItems is not null);
								Assert(change.NewStartingIndex >= 0 );
								Assert(change.OldStartingIndex==-1);
								Assert(change.OldItems == null);
								break;
							case NotifyCollectionChangedAction.Remove:
								Assert(change.NewItems is null);
								Assert(change.NewStartingIndex == 0);
								Assert(change.OldStartingIndex >=0 );
								Assert(change.OldItems is not null);
								break;
							case NotifyCollectionChangedAction.Reset:
								Assert(change.NewItems is null);
								Assert(change.NewStartingIndex == -1);
								Assert(change.OldStartingIndex==-1);
								Assert(change.OldItems == null);
								break;
							default:
								break;
						}
						i.PropertyChanged?.Invoke(i,new("Count"));
							i.CollectionChanged?.Invoke(i,change);
						} 
						catch(Exception ex)
						{
							LogEx(ex);
							//i.CollectionChanged?.Invoke(i,new(NotifyCollectionChangedAction.Reset));
							// that didn't work, try another
						}
				} while(--counter>0) ;

				var after = collectionChanges.Length;

			}
			catch(Exception __ex)
			{
				Debug.LogEx(__ex);
			}
			if(collectionChanges.Any())
				ChangesDebounce.Go();
			return Task.CompletedTask;
		}

		

		public void NotifyChange(NotifyCollectionChangedEventArgs args,bool sizeChanged)
		{


			if(!hasNotifications)
				return;


			Note.ShowQuiet($"CollectionChanges: {args.Action} {args.NewItems.CollectionToString()} {args.OldItems.CollectionToString()}");


			ImmutableInterlocked.Update(ref collectionChanges,(ch) =>
					   {
						   try
						   {

							   for(int counter = 0;counter< ch.Length;++counter)
							   {
								   var c = ch[counter].c;
								   if(c!=this)
									   continue;

								   var args0 = ch[counter].change;

								   // can't get worse
								   if(args0.Action == NotifyCollectionChangedAction.Replace)
									   return ch;

								   if(args0.Action == args.Action && args0.Action == NotifyCollectionChangedAction.Add)
								   {

									   if(args0.NewStartingIndex+args0.NewItems.Count == args.NewStartingIndex)
									   {
										   var list = new ArrayList(args0.NewItems);
										   list.AddRange(args.NewItems);
										   return ch.SetItem(counter,(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,list,args0.NewStartingIndex), this));
									   }
									   else if(args.NewStartingIndex+args.NewItems.Count == args0.NewStartingIndex)
									   {
										   var list = new ArrayList(args.NewItems);
										   list.AddRange(args0.NewItems);
										   return ch.SetItem(counter,(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,list,args.NewStartingIndex), this));
									   }
								   }
								   // unknown or incompatible change
								   return ch.SetItem(counter,(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset), this));

							   }

							   return ch.Add((args, this));
						   }
						   catch(Exception __ex)
						   {
							   Debug.LogEx(__ex);
							   return ch;
						   }
					   } );
						if(collectionChanges.Any())
						{
							ChangesDebounce.Go();
						}
			

				
			
		}

		public abstract long GetCurrentHashData();

		public void NotifyResetWithHash(long newHash, bool itemsChanged = true,bool skipHashCheck = false)
		{

			var hashChanged = newHash != lastDataHash;
			if(!hashChanged && !skipHashCheck)
				return;
			lastDataHash= newHash;
			if(hasNotifications)
			{
				NotifyChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset),itemsChanged);
			}

		}


		public void NotifyReset(bool itemsChanged = true,bool skipHashCheck = false)
		{
			NotifyResetWithHash(GetCurrentHashData(),itemsChanged,skipHashCheck);
		}


		protected long lastDataHash;
		static Debounce ChangesDebounce = new(ProcessChanges) { runOnUiThread = true,debounceDelay=300,throttleDelay=1000 };
		public void ClearHash() => lastDataHash=-1L;

		//public virtual long GetDataHash()
		//{
		////	Assert(false);
		//	return  RuntimeHelpers.GetHashCode(this);
		//}

		public bool MoveCurrentTo(object item) => throw new NotImplementedException();
		public bool MoveCurrentToPosition(int index) => throw new NotImplementedException();
		public bool MoveCurrentToFirst() => throw new NotImplementedException();
		public bool MoveCurrentToLast() => throw new NotImplementedException();
		public bool MoveCurrentToNext() => throw new NotImplementedException();
		public bool MoveCurrentToPrevious() => throw new NotImplementedException();
		public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count) => throw new NotImplementedException();
		public int IndexOf(object item) => throw new NotImplementedException();
		public void Insert(int index,object item) => throw new NotImplementedException();
		public void RemoveAt(int index) => throw new NotImplementedException();
	//	public void Add(object item) => throw new NotImplementedException();
		public void Clear() => throw new NotImplementedException();
		public bool Contains(object item) => throw new NotImplementedException();
		public void CopyTo(object[] array,int arrayIndex) => throw new NotImplementedException();
		public bool Remove(object item) => throw new NotImplementedException();
		public IEnumerator<object> GetEnumerator() => throw new NotImplementedException();
	//	IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();
	}


	public class NotifyCollection< T> :NotifyCollectionBase, ICollection<T>, IEnumerable<T>, IEnumerable, IList<T>, IReadOnlyCollection<T>, IReadOnlyList<T>, ICollection, IList where T : class
	{
		
		public ImmutableArray<T> c = ImmutableArray<T>.Empty;


		public int Count => ((ICollection<T>)c).Count;

		public bool IsReadOnly => ((ICollection<T>)c).IsReadOnly;

		public bool IsSynchronized => ((ICollection)c).IsSynchronized;

		public object SyncRoot => ((ICollection)c).SyncRoot;

		public bool IsFixedSize => ((IList)c).IsFixedSize;

		object IList.this[int index] { get => ((IList)c)[index]; set => ((IList)c)[index]=value; }
		public T this[int index] { get => ((IList<T>)c)[index]; set => ((IList<T>)c)[index]=value; }



		//	public bool IsReadOnly => true;

		//		T IList<T>.this[int index] { get => c[index]; set=> Assert(false); }

		//	T IList<T>.this[int index] { get => ((IList<T>)c)[index]; set => ((IList<T>)c)[index]=value; }

		//	public T this[int id]  { get => c.IList<T>[id]; set=> Assert(false); }

		//public ref readonly T itemRef(int id) => ref  c.ItemRef(id);

		public new void NotifyReset(bool itemsChanged = true,bool skipHashCheck = false)
		{
			base.NotifyResetWithHash(GetDataHash(c),itemsChanged,skipHashCheck);
		}

		public static long GetDataHash(IEnumerable<T> c)
		{
			var hash = 0;
			foreach(var i in c)
			{
				hash = hash*13 + RuntimeHelpers.GetHashCode(i);
			}
			return hash;
		}
		public override long GetCurrentHashData()
		{
			return GetDataHash(c);
		}


		public void NotifyRemoveAt(int id,T removed)
		{
			if(hasNotifications)
			{
				Assert(id >= 0);
				Assert(id <= Count);
				var _count = Count;
				if(_count == Count)
				{
					lastDataHash = GetCurrentHashData();
					NotifyChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove,removed,id),true);
				}
				else
				{
					NotifyReset(true);
				}
			}
		}
		public void NotifyInsert(int id,T added)
		{
			if(hasNotifications)
			{
				var _count = Count;
				if(_count == Count)
				{
					Assert(id >=0);
					Assert(id <= Count);
					lastDataHash = GetCurrentHashData();

					NotifyChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,added,id),true);
				}
				else
				{
					NotifyReset();
				}


			}
		}


		public void Remove(Predicate< T>  pred, bool notify)
		{
			int id = c.FindIndex(pred);
			if(id >= 0)
			{
				RemoveAt(id, notify);
			}
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
			
			if(notify )
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
		public bool Remove(T id, bool notify)
		{
			var index = c.IndexOf(id);
			if(index >= 0)
			{
				RemoveAt(index,notify);
				return true;
			}
			return false;
		}


		public void Set(IEnumerable<T> src,bool notify=true, bool itemsChanged=true)
		{
			if( src == null)
			{
				if(c.Any())
				{
					c= c.Clear();
					NotifyReset();
				}

			}
			else
			{
				if(itemsChanged)
					ItemContentChanged();

				// no change
				var newHash = GetDataHash(src);
				if(newHash == lastDataHash)
					return;
				var prior = c;
				c= src.ToImmutableArray();
				
				if(notify )
				{
					// check for trivial simplifications
					if(prior.Length == c.Length + 1 )
					{
						// todo
					}
					NotifyReset(itemsChanged);
				}

			}
				
		}

		public void Clear(bool notify)
		{
			Set(null,notify,true);
		}
		// do not use these

	
//		IEnumerator IEnumerable.GetEnumerator() => c.ToArray().GetEnumerator();
		//IEnumerator IEnumerable.GetEnumerator() => c.GetEnumerator();
//		 IEnumerator IEnumerable.GetEnumerator()
	

		//public int Add(T item) => Assert(false); 
		public bool Contains(T item) => c.Contains(item);
		public void CopyTo(T[] array,int arrayIndex) => c.CopyTo(array,arrayIndex);

		public void CopyTo(Array array,int index) => ((ICollection)c).CopyTo(array,index);
		//public bool Contains(T value) => ((IList<T>)c).Contains(value);
		//public int IndexOf(T value) => ((IList<T>)c).IndexOf(value);
		//public void Insert(int index,T value) => Insert(index,value,true);
		// void Remove(T value) => ((IList<T>)c).Remove(value);
		public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)c).GetEnumerator();
		public void RemoveAt(int index) => Assert(false);
		IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)c).GetEnumerator();
		 int IList.Add(object value) => ((IList)c).Add(value);
		 bool IList.Contains(object value) => ((IList)c).Contains(value);
		 int IList.IndexOf(object value) => ((IList)c).IndexOf(value);
		void IList.Insert(int index,object value) => Assert(false);
		void IList.Remove(object value) => Assert(false);
		public void Clear() => Assert(false);
		public bool Remove(T item) { Assert(false); return false; }

		public int IndexOf(T item) => c.IndexOf(item);
		public void Insert(int index,T item) => Assert(false);
		internal void ItemContentChanged()
		{
			foreach(var i in c)
			{
				if(i is IANotifyPropertyChanged changed)
					changed.IOnPropertyChanged();
			}
		}


		//	public void SyncWith( IEnumerable<T> from )
		//	{
		//		int iter = Count;
		//		while(--iter >= 0)
		//		{
		//			var b = to[iter];
		//			if(!from.Any(a => EqualityComparer<T>.Default.Equals(a,b)))
		//				to.RemoveAt(iter);
		//		}
		//		foreach(var b in from)
		//		{
		//			if(!to.Any(a => EqualityComparer<T>.Default.Equals(a,b)))
		//				to.Add(b);
		//		}
		//	}
		public void  SortSmall<TKey>(Func<T,TKey> cmp) 
		{
			Set(c.OrderBy(cmp));

		}
		public void SortSmallReverse<TKey>(Func<T,TKey> cmp)
		{
			Set(c.OrderByDescending(cmp));

		}
		public void SortSmall(Comparison<T> comparer) { Assert(false); }
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
