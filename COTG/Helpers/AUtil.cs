using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace COTG
{
    public static class AUtil
    {
        // Lists
        public static void AddIfAbsent<T>(this List<T> l, T a) where T: IEquatable<T>
        {
            foreach(var i in l)
            {
                if (i.Equals( a) )
                    return;
            }
            l.Add(a);
        }
        public static TValue GetOrAdd<TKey,TValue>(this SortedList<TKey,TValue> l,TKey key, Func<TKey, TValue> factory )
        {
            if (!l.TryGetValue(key, out var value))
            {
                value = factory(key);
                l.Add(key, value);
            }
            return value;
        }
        public static TValue GetOrAdd<TKey, TValue>(this SortedList<TKey, TValue> l, TKey key) where TValue : new()
        {
            if (!l.TryGetValue(key, out var value))
            {
                value = new TValue();
                l.Add(key, value);
            }
            return value;
        }
    }
    public class ConcurrentHashSet<T> : IDisposable
    {
        public readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        public readonly HashSet<T> _hashSet = new HashSet<T>();

        public void EnterReadLock() => _lock.EnterReadLock();
        public void ExitReadlLock() => _lock.ExitReadLock();

        #region Implementation of ICollection<T> ...ish
        public bool Add(T item)
        {
            _lock.EnterWriteLock();
            try
            {
                return _hashSet.Add(item);
            }
            finally
            {
                if (_lock.IsWriteLockHeld) _lock.ExitWriteLock();
            }
        }

        public void Clear()
        {
            _lock.EnterWriteLock();
            try
            {
                _hashSet.Clear();
            }
            finally
            {
                if (_lock.IsWriteLockHeld) _lock.ExitWriteLock();
            }
        }

        public bool Contains(T item)
        {
            _lock.EnterReadLock();
            try
            {
                return _hashSet.Contains(item);
            }
            finally
            {
                if (_lock.IsReadLockHeld) _lock.ExitReadLock();
            }
        }

        public bool Remove(T item)
        {
            _lock.EnterWriteLock();
            try
            {
                return _hashSet.Remove(item);
            }
            finally
            {
                if (_lock.IsWriteLockHeld) _lock.ExitWriteLock();
            }
        }

        public int Count
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    return _hashSet.Count;
                }
                finally
                {
                    if (_lock.IsReadLockHeld) _lock.ExitReadLock();
                }
            }
        }
        #endregion

        #region Dispose
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                if (_lock != null)
                    _lock.Dispose();
        }
        ~ConcurrentHashSet()
        {
            Dispose(false);
        }
        #endregion
    }
}
