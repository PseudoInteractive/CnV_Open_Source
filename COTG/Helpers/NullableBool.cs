using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace COTG
{
	readonly public struct NullableBool : IEquatable<NullableBool>
	{
		public const byte _null = 0;
		public const byte _true = 1;
		public const byte _false = 2;
		readonly byte value;
		public NullableBool(byte _v) { value = _v; }
		public NullableBool(bool _v) { value = _v?_true:_false; }
		public NullableBool(bool ?_v) { value = _v is null ? _null : _v.Value? _true : _false; }
		readonly public bool IsFalse => value == _false;
		readonly public bool IsTrueOrNull => value != _false;
		readonly public bool IsFalseNull => value != _true;
		readonly public bool ValueOrDefault => value == _true;

		public override bool Equals(object obj)
		{
			return obj is NullableBool @bool && Equals(@bool);
		}

		public bool Equals(NullableBool other)
		{
			return value == other.value;
		}

		public override int GetHashCode()
		{
			return (value);
		}

		public static bool operator ==(NullableBool left, NullableBool right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(NullableBool left, NullableBool right)
		{
			return !(left == right);
		}

		public static implicit operator bool?(NullableBool t) => t.value == _null ? null : t.value== _true ? true : false;
		 public static implicit operator bool (NullableBool t) => t.value == _true ? true : false;
		public static implicit operator NullableBool(bool? t) => new NullableBool(t);
		public static implicit operator NullableBool(bool t) => new NullableBool(t);
	}
}
