﻿using COTG.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI;

namespace COTG
{
    public enum RefreshState
    {
        dirty,
        updating,
        clean,
    }
    public static class AUtil
    {
        public static Color WithAlpha(this Color c, byte alpha)
        {
            return Color.FromArgb(alpha, c.R, c.G, c.B);
        }

        public static void Nop<T0>(T0 t) { }
        public static DateTimeOffset dateTimeZero => new DateTimeOffset(1969, 1, 1, 0, 0, 0, TimeSpan.Zero);
        // Lists
        public static Color Lerp(this float t, Color c0, Color c1)
        {
            return Color.FromArgb(
                (byte)t.Lerp((float)c0.A, (float)c1.A).RoundToInt(),
                (byte)t.Lerp((float)c0.R, (float)c1.R).RoundToInt(),
                (byte)t.Lerp((float)c0.G, (float)c1.G).RoundToInt(),
                (byte)t.Lerp((float)c0.B, (float)c1.B).RoundToInt());
        }
        public static Color LerpGamma(this float t, Color c0, Color c1)
        {
           
            return Color.FromArgb(
                (byte)t.LerpSqrt((float)c0.A, (float)c1.A).RoundToInt(),
                (byte)t.LerpSqrt((float)c0.R, (float)c1.R).RoundToInt(),
                (byte)t.LerpSqrt((float)c0.G, (float)c1.G).RoundToInt(),
                (byte)t.LerpSqrt((float)c0.B, (float)c1.B).RoundToInt());
        }
        public static bool IsZero(this DateTimeOffset c) => c == dateTimeZero;
        public static bool AddIfAbsent<T>(this List<T> l, T a) where T: IEquatable<T>
        {
            foreach(var i in l)
            {
                if (i.Equals( a) )
                    return false;
            }
            l.Add(a);
            return true;
        }
        public static T[] ArrayAppend<T>(this T[] l, T a)
        {
            int lg = l.Length;
            var result = new T[lg + 1];
            for(int i=0;i<lg;++i)
            {
                result[i] = l[i];
            }
            result[lg] = a;
            return result;

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

        public void EnterWriteLock() => _lock.EnterWriteLock();
        public void ExitWriteLock() => _lock.ExitWriteLock();

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
