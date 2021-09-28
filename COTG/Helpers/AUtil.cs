using Cysharp.Text;

using Microsoft.Toolkit.HighPerformance.Buffers;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
//using NetFabric.Hyperlinq; 
using Color = Microsoft.Xna.Framework.Color;

namespace COTG
{
	public enum RefreshState
	{
		dirty,
		updating,
		clean,
	}

	public static partial class AUtil
	{
		
		public static object AsObject(this object a) => a;
		
		public const string emptyJson = "{}";
		public const string defaultTimeSpanFormat = "h':'mm':'ss";
		public const string defaultTimeFormat = "HH':'mm':'ss";
		public const string defaultDateFormat = "MM/dd HH':'mm':'ss";

		public const string preciseDateTimeFormat = "yyyy-MM-dd HH':'mm':'ss.fff";
		// public const string preciseDateTimeFormat = "MM/dd HH':'mm':'ss.fff";

		public const string fullDateFormat = "yyyy/MM/dd HH':'mm':'ss";
		public const string raidDateTimeFormat = "MM/dd/yyyy HH':'mm':'ss";
		public const string spreadSheetDateTimeFormat = "yyyy/MM/dd HH':'mm':'ss";

		public static MemoryOwner<T> AsMemoryOwner<T>(this Span<T> me)
		{
			var lg = me.Length;
			var i = MemoryOwner<T>.Allocate(lg);
			me.CopyTo(i.Span);
			return i;
		}
		public static string FirstCharToUpper(this string s)
		{
			// Check for empty string.  
			if (string.IsNullOrEmpty(s) || !(s[0] >= 'a' && s[0] <= 'z'))
			{
				return s;
			}
			// Return char and concat substring.  
			return (char)(s[0] + ('A'-'a')) + s.Substring(1);
		}
		public static T PickRandom<T>(this T[] array) => array[AMath.random.Next(array.Length)];
		// public static MemoryOwner<T> AsMemoryOwner<T>(this SpanOwner<T> me) =>
		// AsMemoryOwner<T>((ReadOnlySpan<T>)me.Span); public static MemoryOwner<T>
		// AsMemoryOwner<T>(this Span<T> me) => AsMemoryOwner<T>((ReadOnlySpan<T>)me);
		public static MemoryOwner<T> Clone<T>(this MemoryOwner<T> me)
		{
			var lg = me.Length;
			var i = MemoryOwner<T>.Allocate(lg);
			var sp0 = me.Span;
			var sp1 = i.Span;
			for (int index = 0; index < lg; ++index)
			{
				sp1[index] = sp0[index];
			}
			return i;
		}

		// this will be false for lamda functions
		public static int As01(this bool b) => b ? 1 : 0;

		public static bool IsEqual(this Delegate a, Delegate b)
		{
			// ADDED THIS -------------- remove delegate overhead
			while (a.Target is Delegate)
			{
				a = a.Target as Delegate;
			}

			while (b.Target is Delegate)
			{
				b = b.Target as Delegate;
			}

			// standard equality
			if (a == b)
			{
				return true;
			}

			// null
			if (a == null || b == null)
			{
				return false;
			}

			// compiled method body
			if (a.Target != b.Target)
			{
				return false;
			}

			return a.Method == b.Method;
		}

		public static DateTimeOffset ToServerTime(this DateTimeOffset t) => t.ToUniversalTime() + JSClient.gameTOffset;

		public static DateTimeOffset FromServerTime(this DateTimeOffset t) => t.ToUniversalTime() - JSClient.gameTOffset;


		public static unsafe void UnsafeCopy<T>(in T[] source, in T[] target) where T : unmanaged
		{
			COTG.Debug.Assert(source.Length == target.Length);
			Buffer.BlockCopy(source, 0, target, 0, source.Length * sizeof(T));
		}

		public static void Swap<T>(ref T a, ref T b)
		{
			T temp = a;
			a = b;
			b = temp;
		}

		public static bool ContainsAny(this string s, string[] tags)
		{
			foreach (var a in tags)
			{
				if (s.Contains(a))
				{
					return true;
				}
			}
			return false;
		}

		public static string FormatDurationFromSeconds(double f)
		{
			if (Math.Abs(f) < 60)
			{
				return $"{f:0.00}s";
			}
			else
			{
				var prefix = string.Empty;
				var s = f.RoundToInt();
				if (s < 0)
				{
					s = -s;
					prefix = "-";
				}
				if (s < 60 * 60)
				{
					var min = (s / 60);
					var sec = s - min * 60;
					return $"{prefix}{min}m {sec}s";
				}
				else
				{
					var hour = (s / (60 * 60));
					s -= hour * 60 * 60;
					var min = (s) / 60;
					s -= min * 60;
					var sec = s;
					return $"{prefix}{hour}h {min}m {sec}s";
				}
			}
		}

		internal static void Clear<T>(out T[] resDest)
		{
			resDest = Array.Empty<T>();
		}

		public static string fileTimeFormat = "yyyy_MM_dd_HH_mm_ss";
		public static string fileTimeFormatMinute = "yyyy_MM_dd_HH_mm";
		public static string fileTimeFormatLegacy = "yyyy_MM_dd_H_mm_ss";
		public static string FormatFileTime(this DateTimeOffset m) => m.ToString(fileTimeFormat, CultureInfo.InvariantCulture);
		public static string FormatFileTimeToMinute(this DateTimeOffset m) => m.ToString(fileTimeFormatMinute, CultureInfo.InvariantCulture);
		public static DateTimeOffset ParseFileTime(this string s)
		{
			if (DateTimeOffset.TryParseExact(s, fileTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var rv))
				return rv;
			if (DateTimeOffset.TryParseExact(s, fileTimeFormatLegacy, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out rv))
				return rv;
			Debug.Assert(false);
			return DateTimeOffset.MinValue;

		}

		public static string Format(this TimeSpan t)
		{
			if (t <= TimeSpan.FromMinutes(1))
				return t.ToString("ss's'", CultureInfo.InvariantCulture);
			if (t <= TimeSpan.FromHours(1))
				return t.ToString("mm'm 'ss's'", CultureInfo.InvariantCulture);
			if ( t <= TimeSpan.FromDays(1))
				return t.ToString("hh'hr 'mm'm 'ss's'", CultureInfo.InvariantCulture);
			return t.ToString("dd'd 'hh'hr 'mm'm 'ss's'", CultureInfo.InvariantCulture);
		}
		public static string Format(this int i)
		{
			return i > 10_000_000 ? i.DivideRound(1_000_000).ToString("N0") + "m" : i > 10_000 ? i.DivideRound(1000).ToString("N0") + "k": i.ToString("N0");
		}
		public static string FormatWithSign(this int d)
		{
			return d >= 0 ? ("+" + Format(d)) : Format(d);
		}

		public static string Format(this (int i, int max) k) => $"{Format(k.i)}/{Format(k.max)}";
		public static string Format(this int i, int max) => $"{Format(i)}/{Format(max)}";
		public static string Format(this (ushort i, ushort max) k ) => Format( ((int)k.i,(int)k.max));
		public static string Format(this ushort i, ushort max) => Format(((int)i, (int)max));


		public static string Format(this ushort i) => Format((int)i);
		public static string Format(this short i) => Format((int)i);

		public static string FormatTimePrecise(this DateTimeOffset m) => m.ToString(preciseDateTimeFormat, CultureInfo.InvariantCulture);


		
		public static string Format(this DateTimeOffset m)
		{
			var serverNow = JSClient.ServerTime();
			if (serverNow.Day == m.Day && serverNow.Month == m.Month)
			{
				return m.ToString(defaultTimeFormat, CultureInfo.InvariantCulture);
			}
			else
			{
				return m.ToString(defaultDateFormat, CultureInfo.InvariantCulture);
			}
		}

		public static string FormatDateForFileName(this DateTimeOffset m)=>	m.ToString("yyyy_MM_dd", CultureInfo.InvariantCulture);
	
		public static string FormatFull(this DateTimeOffset m) => m.ToString(fullDateFormat, CultureInfo.InvariantCulture);

		public static TimeSpan localTimeOffset = TimeZoneInfo.Local.BaseUtcOffset;

		public static Color HalfSaturation(this Color c)
		{
			return new Color((byte)(128 + c.R / 2), (byte)(128 + c.G / 2), (byte)(128 + c.B / 2), c.A);
		}

		public static Color ThreeQuarterSaturation(this Color c)
		{
			return new Color((byte)(64 + c.R * 3 / 4), (byte)(64 + c.G * 3 / 4), (byte)(64 + c.B * 3 / 4), c.A);
		}

		public static void Nop<T0>(T0 t)
		{
		}

		public static DateTimeOffset dateTimeZero => new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);

		// Lists
		public static Color Lerp(this float t, Color c0, Color c1)
		{
			return new Color(

				(byte)t.Lerp(c0.R, c1.R).RoundToInt(),
				(byte)t.Lerp(c0.G, c1.G).RoundToInt(),
				(byte)t.Lerp(c0.B, c1.B).RoundToInt(),
				 (byte)t.Lerp(c0.A, c1.A).RoundToInt());
		}

		public static Color LerpGamma(this float t, Color c0, Color c1)
		{
			return new Color(

				(byte)t.LerpSqrt(c0.R, c1.R).RoundToInt(),
				(byte)t.LerpSqrt(c0.G, c1.G).RoundToInt(),
				(byte)t.LerpSqrt(c0.B, c1.B).RoundToInt(),
				 (byte)t.LerpSqrt(c0.A, c1.A).RoundToInt());
		}

		public static bool IsZero(this DateTimeOffset c) => c == dateTimeZero;

		public static bool AddIfAbsent<T>(this List<T> l, T a) where T : IEquatable<T>
		{
			foreach (var i in l)
			{
				if (i.Equals(a))
				{
					return false;
				}
			}
			l.Add(a);
			return true;
		}

		public static T[] ArrayAppend<T>(this T[] l, T a)
		{
			if (l == null || l.Length == 0)
			{
				return new T[1] { a };
			}

			int lg = l.Length;
			var result = new T[lg + 1];
			for (int i = 0; i < lg; ++i)
			{
				result[i] = l[i];
			}
			result[lg] = a;
			return result;
		}

		public static T[] ArrayAppendIfAbsent<T>(this T[] l, T a) where T : IEquatable<T>
		{
			if (l == null || l.Length == 0)
			{
				return new T[1] { a };
			}

			int lg = l.Length;
			for (int i = 0; i < lg; ++i)
			{
				if (a.Equals(l[i]))
				{
					return l;
				}
			}
			var result = new T[lg + 1];
			for (int i = 0; i < lg; ++i)
			{
				result[i] = l[i];
			}
			result[lg] = a;
			return result;
		}
		public static void AddIfAbsent<T>(ref T[] l,T a) where T : IEquatable<T>
		{
			l = ArrayAppendIfAbsent(l,a);
		}

		public static T[] ArrayRemoveDuplicates<T>(this T[] l) where T : IEquatable<T>
		{
			int lg = l.Length;
			int removed = 0;
			for (int b = lg; --b > 0;)
			{
				for (int a = 0; a < b; ++a)
				{
					if (l[a].Equals(l[b]))
					{
						++removed;
						--lg;
						if (b != lg)
						{
							l[b] = l[lg];
						}
						break;
					}
				}
			}
			if (removed > 0)
			{
				var result = new T[lg];
				for (int i = 0; i < lg; ++i)
				{
					result[i] = l[i];
				}
				return result;
			}
			else
			{
				return l;
			}
		}

		//public static int IndexOf<T>(this T[] me, T val) where T : IEqualityComparable<T>
		//{
		//	int lg = me.Length;
		//	for (int i = 0; i < lg; ++i)
		//		if (Equals(me[i], val))
		//			return i;
		//	return -1;
		//}

		public static T[] ArrayRemove<T>(this T[] l, int index)
		{
			if (l == null || l.Length <= 0)
			{
				return Array.Empty<T>();
			}

			int lg = l.Length;

			var result = new T[lg - 1];
			var put = 0;
			for (int i = 0; i < lg; ++i)
			{
				if (i != index)
				{
					result[put++] = l[i];
				}
			}
			return result;
		}
//		public static T[] RemoveAll<T>(this T[] l, Func<T, bool> pred) => l.Where(pred).ToArray();

		public static T[] ArrayClone<T>(this T[] l)
		{
			if (l == null || l.Length <= 0)
			{
				return Array.Empty<T>();
			}

			int lg = l.Length;

			var result = new T[lg];
			for (int i = 0; i < lg; ++i)
			{
				result[i] = l[i];
			}
			return result;
		}

		public static TValue GetOrAdd<TKey, TValue>(this SortedList<TKey, TValue> l, TKey key, Func<TKey, TValue> factory)
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

		public static (int x, int y) DecomposeXY(this int xy, int columns)
		{
			var y = xy / columns;
			var x = xy - y * columns;
			return (x, y);
		}

		

		public static System.Numerics.Vector2 ToVector(this (int x, int y) a)
		{
			return new(a.x, a.y);
		}


		public static int DecodeCid(int offset, string s)
		{
			try
			{
				if (s.Length <= 1)
				{
					return 0;
				}

				var s0s = offset;
				while (!char.IsDigit(s[s0s]))
				{
					++s0s;
				}
				var s0e = s0s + 1;
				int counter = 0;
				while (char.IsDigit(s[s0e]))
				{
					++s0e;
					if (++counter > 3)
					{
						return -1;
					}
				}
				var s1s = s0e + 1; // skip :
				COTG.Debug.Assert(char.IsDigit(s[s1s]));
				var s1e = s1s + 1;
				counter = 0;
				while (s1e < s.Length && char.IsDigit(s[s1e]))
				{
					++s1e;
					if (++counter > 3)
					{
						return -1;
					}
				}

				var x = int.Parse(s.Substring(s0s, s0e - s0s));
				var y = int.Parse(s.Substring(s1s, s1e - s1s));
				return x + y * 65536;
			}
			catch (Exception e)
			{
				COTG.Debug.LogEx(e);
				return -1;
			}
		}

		public static Regex coordsRegex = new Regex(@":*\b\d{2,3}:\d{2,3}\b:*", RegexOptions.CultureInvariant | RegexOptions.Compiled);
		public static Regex coordsRegex2 = new Regex(@"[^:]?\b(\d{2,3}:\d{2,3})\b[^:]?", RegexOptions.CultureInvariant | RegexOptions.Compiled);
		public static Regex coordsRegexAny = new Regex(@"(?:[^\d]|^)?(\d{2,3}:\d{2,3})(?:[^\d]|$)", RegexOptions.CultureInvariant | RegexOptions.Compiled);
		static Regex regexEndNumber = new Regex(@"([0-9|A-F]+)$", RegexOptions.CultureInvariant | RegexOptions.Compiled);
		public static string IncrementVersion(this string s)
		{
			var i = regexEndNumber.Match(s);
			if (i.Success && i.Groups.Count > 1)
			{
				var g = i.Groups[1];
				if (int.TryParse(g.Value, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo, out var v))
				{
					return i.Result(@"$`" + (v + 1).ToString("X"));
				}
			}
			return s+(AMath.random.Next(0x10)).ToString("X");
		}


		public static string ReplaceInvalidFileNameChars(string fileName, char newValue = '_' )
		{
			var invalid = System.IO.Path.GetInvalidFileNameChars();

			char[] chars = fileName.ToCharArray();
			for (int i = 0; i < chars.Length; i++)
			{
				char c = chars[i];
				if (invalid.Contains(c))
					chars[i] = newValue;
			}

			return new string(chars);
		}

	}

	//public ref struct SemaLockRef
	//{
	//	public static async Task<SemaLockRef> Go(SemaphoreSlim _sema)
	//	{
	//		await _sema.WaitAsync();
	//		return new SemaLockRef(_sema);
	//	}
	//	private readonly SemaphoreSlim mySema;
	//	private SemaLockRef(SemaphoreSlim _sema) {  mySema = _sema; }
	//	public void Dispose()
	//	{
	//		if (mySema != null)
	//			mySema.Release();
	//	}
	//}
	public struct SemaLock : IDisposable
	{
		public static async Task<SemaLock> Go(SemaphoreSlim _sema)
		{
			await _sema.WaitAsync();
			return new SemaLock(_sema );
		}
		private readonly SemaphoreSlim mySema;

		private SemaLock(SemaphoreSlim mySema)
		{
			this.mySema = mySema;
		}

		public void Dispose()
		{
			if (mySema != null)
				mySema.Release();
		}
	}

	public class ConcurrentHashSet<T> : IDisposable
	{
		public readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
		public readonly HashSet<T> _hashSet = new HashSet<T>();

		public void EnterReadLock() => _lock.EnterReadLock();

		public void ExitReadlLock() => _lock.ExitReadLock();

		public void EnterWriteLock() => _lock.EnterWriteLock();

		public void ExitWriteLock() => _lock.ExitWriteLock();

		public T[] ToArray()
		{
			EnterReadLock();
			T[] rv = null;
			try
			{
				rv = _hashSet.ToArray();
			}
			catch
			{
				rv = Array.Empty<T>();
			}
			finally
			{
				ExitReadlLock();
			}
			return rv;
		}

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
				if (_lock.IsWriteLockHeld)
				{
					_lock.ExitWriteLock();
				}
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
				if (_lock.IsWriteLockHeld)
				{
					_lock.ExitWriteLock();
				}
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
				if (_lock.IsReadLockHeld)
				{
					_lock.ExitReadLock();
				}
			}
		}

		public bool ContainsAlreadyLocked(T item)
		{
			return _hashSet.Contains(item);
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
				if (_lock.IsWriteLockHeld)
				{
					_lock.ExitWriteLock();
				}
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
					if (_lock.IsReadLockHeld)
					{
						_lock.ExitReadLock();
					}
				}
			}
		}

		#endregion Implementation of ICollection<T> ...ish

		#region Dispose

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_lock != null)
				{
					_lock.Dispose();
				}
			}
		}

		~ConcurrentHashSet()
		{
			Dispose(false);
		}

		#endregion Dispose
	}
	//public class GridEnumerator : IEnumerable<(int x,int y)>
	//{
	//	public int radius;
	//	public static IEnumerable<BuildQueueItem> IterateQueue()
	//	{
	//		foreach (var i in buildQueue)
	//			yield return i;

	//		if (CityBuildQueue.all.TryGetValue(City.build, out var q))
	//		{
	//			var count = q.queue.count;
	//			var data = q.queue.v;

	//			for (int i = 0; i < count; ++i)
	//			{
	//				//semi safe
	//				yield return data[i];
	//			}
	//		}
	//	}
	//}
}
