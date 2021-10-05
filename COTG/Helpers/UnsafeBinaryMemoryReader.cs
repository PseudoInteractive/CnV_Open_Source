using Microsoft.Toolkit.HighPerformance.Buffers;

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

using static COTG.Debug;
namespace COTG.BinaryMemory
{
	/// <summary>
	/// An UNSAFE binary memory reader. This class can be used to read binary data from a pointer.
	/// </summary>
	/// <remarks>Use this class only if you are sure that you won't read over the memory border.</remarks>
	public unsafe ref struct Reader 
    {
		MemoryHandle memoryHandle;

		private byte* data;
		int position; // read point
		int end; // end point (not currently used)
		
        public Reader(BinaryData mem)
        {
			var memory = mem.ToMemory();
			memoryHandle = memory.Pin();
			end = memory.Length;
			data = (byte*)memoryHandle.Pointer;
			position=0;
			//data = mem.DangerousGetArray().Array;
        }
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public  ref readonly byte ReadByte()
		{
			Assert(position < end);
			return ref data[position++];
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly ref byte PeekByte()  => ref data[position];

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe ref T Read<T>() where T : unmanaged
		{
			var sz = Marshal.SizeOf<T>();
			Assert(position+sz <= end);
			var _r = position;
			position += sz;

			return ref *(T*)&data[_r];
		}

		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly unsafe ref readonly T Peek<T>(int byteOffset) where T : unmanaged
		{
			var sz = Marshal.SizeOf<T>();
			Assert(position+sz <= end);
			
			return ref *(T*)&data[byteOffset];
			
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly public unsafe  T  *  PeekPointer<T>(int byteOffset=0) where T: unmanaged
 		{
			Assert(position+byteOffset <= end);
			return (T*)&data[position+byteOffset];
		}

		/// <summary>
		/// Reads a string encoded in UTF-8 with 7 bit encoded length prefix.
		/// </summary>
		/// <returns>The string.</returns>
		/// <remarks>Returns null if the string is empty.</remarks>
		public unsafe string ReadString(string valueIfEmpty = null)
        {
            int length = 0;
            int shift = 0;
            string result;

            while ((PeekByte() & 0x80) == 0x80 && shift <= 28)
            {
                length |= (ReadByte() & 0x7F) << shift;
                shift += 7;
            }

            if (shift > 28)
                throw new System.IO.InvalidDataException("Ambiguous length information.");

            length |= ReadByte() << shift;

            if (length == 0)
                return valueIfEmpty;

            result = Encoding.UTF8.GetString(PeekPointer<byte>(), length);

            position += length;

            return result;
        }


		/// <summary>
		/// Reads a string encoded in UTF-8 without leading length and without NUL-termination.
		/// </summary>
		/// <returns>The string.</returns>
		///// <remarks>Returns null if the string is empty.</remarks>
		//public string ReadVanillaString(int bytes)
		//{
		//    if (bytes <= 0)
		//        return ;

		//    position += bytes;

		//    return Encoding.UTF8.GetString(position - bytes, bytes);
		//}

		///// <summary>
		///// Reads a string encoded in UTF-8 without leading length and without NUL-termination.
		///// </summary>
		///// <returns>The string.</returns>
		///// <remarks>Returns an empty string if the string is empty and not null.</remarks>
		//public string ReadVanillaStringNonNull(int bytes)
		//{
		//    return ReadVanillaString(bytes) ?? "";
		//}

		/// <summary>
		/// Cuts a sub reader at the current position with the specified length.
		/// </summary>
		/// <param name="size">The size of the cut.</param>
		///// <returns>The new reader.</returns>
		//public Reader Cut(int size)
		//{
		//    position += size;

		//    return new Reader(position - size);
		//}

		/// <summary>
		/// Jumps step bytes forward.
		/// </summary>
		/// <param name="step">The amount of bytes to jump.</param>
		/// <remarks>Beware: This method doesn't check for negative values.</remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Skip(int bytes)
        {
            position += bytes;
        }

   
        /// <summary>
        /// Reads a bool.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ReadBool()
        {
            return ReadByte() != 0x00;
        }

        
        /// <summary>
        /// Reads a signed byte.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public sbyte ReadSByte()
        {
            return Read<sbyte>();
        }

        /// <summary>
        /// Reads an unsigned short.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ushort ReadUInt16()
        {
         
            return Read<ushort>();
	    }

        /// <summary>
        /// Reads a short.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public short ReadInt16()
        {
			return Read<short>();
        }

        /// <summary>
        /// Reads a 3 byte integer.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ReadUInt24()
        {
            return ((int)ReadByte() << 16) + Read<ushort>();
        }

        /// <summary>
        /// Reads an unsigned int.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint ReadUInt32() => Read<uint>();

        /// <summary>
        /// Reads a int.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ReadInt32() => Read<int>();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public SmallTime ReadSmallTime() => Read<uint>(); // equivalent

		/// <summary>
		/// Reads an unsigned long.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong ReadUInt64() => Read<ulong>();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long ReadInt64()=>	Read<long>();

		/// <summary>
		/// Reads a 7 bit encoded number.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ulong Read7BitEncoded64()
        {
            ulong result = 0;
            int	shift = 0;

            for(;;)
            {
				var c = ReadByte();
				if( (c & 0x80) == 0x00)
				{
					 return result + ((ulong)(c) << shift);
				}
				result += (ulong) (c & 0x7F) << shift;
			   shift += 7;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint Read7BitEncoded()
		{
			uint result = 0;
			int shift = 0;

			for(;;)
			{
				var c = ReadByte();
				if((c & 0x80) == 0x00)
				{
					return result + ((uint)(c) << shift);
						}
						result += (uint)(c & 0x7F) << shift;
				shift += 7;
				}
		}
		


		/// <summary>
		/// Reads a char.
		///// </summary>
		//[MethodImpl(MethodImplOptions.AggressiveInlining)]
  //      public char ReadChar()
  //      {
		//	var c =Peek();
  //          if ((c & 0b10000000) == 0b00000000)
  //          {
  //              return (char)Read();
  //          }

  //          if ((c & 0b11100000) == 0b11000000)
  //          {
  //              position += 2;
		//		var c
  //              return (char)((*(position - 1) & 0b00111111) | ((*(position - 2) & 0b00011111) << 6));
  //          }

  //          if ((Peek() & 0b11110000) == 0b11100000)
  //          {
  //              position += 3;

  //              return (char)((*(position - 1) & 0b00111111) | ((*(position - 2) & 0b00111111) << 6) | ((*(position - 3) & 0b00001111) << 12));
  //          }

  //          throw new InvalidOperationException("Char in memory is too wide for char datatype.");
  //      }

        /// <summary>
        /// Reads a float.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float ReadSingle() => Read<float>();

        /// <summary>
        /// Reads a double.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double ReadDouble() => Read<double>();

        /// <summary>
        /// Reads a timespan.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TimeSpan ReadTimeSpan() => new TimeSpan(Read<long>());

        /// <summary>
        /// Reads a datetime.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DateTimeOffset ReadDateTime() => new DateTimeOffset(ReadInt64(),TimeSpan.Zero);

        /// <summary>
        /// Reads a decimal.
        /// </summary>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public decimal ReadDecimal()
        //{
        //    position += 16;

        //    return new decimal(new int[] { *(int*)(position - 16), *(int*)(position - 12), *(int*)(position - 8), *(int*)(position - 4) });
        //}

        /// <summary>
        /// Reads count bytes from the current position into data starting at offset.
        /// </summary>
        /// <param name="data">The byte array where data will be written to.</param>
        /// <param name="offset">The position in the byte array where those data will be written to.</param>
        /// <param name="count">The amount of bytes which will be written.</param>
        /// <remarks>BEWARE: This method is also NOT DOING input checks of the given parameters.</remarks>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public void ReadBytes(byte[] data, int offset, int count)
        //{
        //    fixed (byte* pData = data)
        //        Buffer.MemoryCopy(position, pData + offset, count, count);

        //    position += count;
        //}

		//[MethodImpl(MethodImplOptions.AggressiveInlining)]
		//public uint[] ReadUints()
		//{
		//	Assert(false);// optimize
		//	var count = Read7BitEncoded();
		//	var byteCount = count * sizeof(uint);
		//	uint[] rv = new uint[count];
		//	fixed (uint* pData = rv)
		//	{
		//		Buffer.MemoryCopy(position, pData, byteCount, byteCount);
		//	}

		//	position += byteCount;
		//	return rv;
		//}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint[] ReadPackedUints()
		{
			var count = Read7BitEncoded();
			var rv = new uint[count];
			for(int i=0;i<count;++i)
			{
				rv[i] = Read7BitEncoded();
			}
			return rv;
		}

		///// <summary>
		///// Peeks a string encoded in UTF-8 with 7 bit encoded length prefix.
		///// </summary>
		///// <returns>The string.</returns>
		///// <remarks>Returns null if the string is empty.</remarks>
		//public string PeekString()
  //      {
  //          if (Peek() == 0x00)
  //              return null;

  //          if ((Peek() & 0x80) == 0x00)
  //              return Encoding.UTF8.GetString(position + 1, Peek());

  //          if (((*(position + 1)) & 0x80) == 0x00)
  //              return Encoding.UTF8.GetString(position + 2, (Peek() & 0x7F) | ((*(position + 1) & 0x7F) << 7));

  //          if (((*(position + 2)) & 0x80) == 0x00)
  //              return Encoding.UTF8.GetString(position + 3, (Peek() & 0x7F) | ((*(position + 1) & 0x7F) << 7) | ((*(position + 2) & 0x7F) << 14));

  //          if (((*(position + 3)) & 0x80) == 0x00)
  //              return Encoding.UTF8.GetString(position + 4, (Peek() & 0x7F) | ((*(position + 1) & 0x7F) << 7) | ((*(position + 2) & 0x7F) << 14) | ((*(position + 3) & 0x7F) << 21));

  //          if (((*(position + 4)) & 0x80) == 0x00)
  //              return Encoding.UTF8.GetString(position + 5, (Peek() & 0x7F) | ((*(position + 1) & 0x7F) << 7) | ((*(position + 2) & 0x7F) << 14) | ((*(position + 3) & 0x7F) << 21) | ((*(position + 4) & 0x0F) << 28));

  //          return null;
  //      }

  //      /// <summary>
  //      /// Peeks a string encoded in UTF-8 with 7 bit encoded length prefix.
  //      /// </summary>
  //      /// <returns>The string.</returns>
  //      /// <remarks>Returns an empty string if the string is empty and not null.</remarks>
  //      public string PeekStringNonNull()
  //      {
  //          return PeekString() ?? "";
  //      }

  //      /// <summary>
  //      /// Peeks a string encoded in UTF-8 without leading length and without NUL-termination.
  //      /// </summary>
  //      /// <returns>The string.</returns>
  //      /// <remarks>Returns null if the string is empty.</remarks>
  //      public string PeekVanillaString(int bytes)
  //      {
  //          if (bytes <= 0)
  //              return null;

  //          return Encoding.UTF8.GetString(position, bytes);
  //      }

  //      /// <summary>
  //      /// Peeks a string encoded in UTF-8 without leading length and without NUL-termination.
  //      /// </summary>
  //      /// <returns>The string.</returns>
  //      /// <remarks>Returns an empty string if the string is empty and not null.</remarks>
  //      public string PeekVanillaStringNonNull(int bytes)
  //      {
  //          return ReadVanillaString(bytes) ?? "";
  //      }

  //      /// <summary>
  //      /// Peeks a boolean.
  //      /// </summary>
  //      [MethodImpl(MethodImplOptions.AggressiveInlining)]
  //      public bool PeekBoolean()
  //      {
  //          return Peek() != 0x00;
  //      }

  //      /// <summary>
  //      /// Peeks a byte.
  //      /// </summary>
  //      [MethodImpl(MethodImplOptions.AggressiveInlining)]
  //      public byte PeekByte()
  //      {
  //          return Peek();
  //      }

  //      /// <summary>
  //      /// Peeks a signed byte.
  //      /// </summary>
  //      [MethodImpl(MethodImplOptions.AggressiveInlining)]
  //      public sbyte PeekSByte()
  //      {
  //          return *(sbyte*)position;
  //      }

  //      /// <summary>
  //      /// Peeks an unsigned short.
  //      /// </summary>
  //      [MethodImpl(MethodImplOptions.AggressiveInlining)]
  //      public ushort PeekUInt16()
  //      {
  //          return *(ushort*)position;
  //      }

  //      /// <summary>
  //      /// Peeks a short.
  //      /// </summary>
  //      [MethodImpl(MethodImplOptions.AggressiveInlining)]
  //      public short PeekInt16()
  //      {
  //          return *(short*)position;
  //      }

  //      /// <summary>
  //      /// Peeks a 3 byte integer.
  //      /// </summary>
  //      [MethodImpl(MethodImplOptions.AggressiveInlining)]
  //      public int PeekUInt24()
  //      {
  //          return (Peek() << 16) | *(ushort*)(position + 1);
  //      }

  //      /// <summary>
  //      /// Peeks an unsigned int.
  //      /// </summary>
  //      [MethodImpl(MethodImplOptions.AggressiveInlining)]
  //      public uint PeekUInt32()
  //      {
  //          return *(uint*)position;
  //      }

  //      /// <summary>
  //      /// Peeks a int.
  //      /// </summary>
  //      [MethodImpl(MethodImplOptions.AggressiveInlining)]
  //      public int PeekInt32()
  //      {
  //          return *(int*)position;
  //      }

  //      /// <summary>
  //      /// Peeks an unsigned long.
  //      /// </summary>
  //      [MethodImpl(MethodImplOptions.AggressiveInlining)]
  //      public ulong PeekUInt64()
  //      {
  //          return *(ulong*)position;
  //      }

  //      /// <summary>
  //      /// Peeks a long.
  //      /// </summary>
  //      [MethodImpl(MethodImplOptions.AggressiveInlining)]
  //      public long PeekInt64()
  //      {
  //          return *(long*)position;
  //      }

  //      /// <summary>
  //      /// Peeks a 7 bit encoded number.
  //      /// </summary>
  //      public ulong Peek7BitEncoded()
  //      {
  //          ulong result = 0;
  //          int shift = 0;

  //          byte* position = this.position;

  //          do
  //          {
  //              result |= (ulong)(Peek() & 0x7F) << shift;
  //              shift += 7;
  //          } while ((Read() & 0x80) != 0x00);

  //          return result;
  //      }

  //      /// <summary>
  //      /// Peeks a char.
  //      /// </summary>
  //      [MethodImpl(MethodImplOptions.AggressiveInlining)]
  //      public char PeekChar()
  //      {
  //          if ((Peek() & 0b10000000) == 0b00000000)
  //              return (char)Peek();

  //          if ((Peek() & 0b11100000) == 0b11000000)
  //              return (char)((*(position + 1) & 0b00111111) | ((Peek() & 0b00011111) << 6));

  //          if ((Peek() & 0b11110000) == 0b11100000)
  //              return (char)((*(position + 2) & 0b00111111) | ((*(position + 1) & 0b00111111) << 6) | ((Peek() & 0b00001111) << 12));

  //          throw new InvalidOperationException("Char in memory is too wide for char datatype.");
  //      }

  //      /// <summary>
  //      /// Peeks a float.
  //      /// </summary>
  //      [MethodImpl(MethodImplOptions.AggressiveInlining)]
  //      public float PeekSingle()
  //      {
  //          return *(float*)position;
  //      }

  //      /// <summary>
  //      /// Peeks a double.
  //      /// </summary>
  //      [MethodImpl(MethodImplOptions.AggressiveInlining)]
  //      public double PeekDouble()
  //      {
  //          return *(double*)position;
  //      }

  //      /// <summary>
  //      /// Peeks a timespan.
  //      /// </summary>
  //      [MethodImpl(MethodImplOptions.AggressiveInlining)]
  //      public TimeSpan PeekTimeSpan()
  //      {
  //          return new TimeSpan(*(long*)position);
  //      }

  //      /// <summary>
  //      /// Peeks a datetime.
  //      /// </summary>
  //      [MethodImpl(MethodImplOptions.AggressiveInlining)]
  //      public DateTime PeekDateTime()
  //      {
  //          return new DateTime(*(long*)position);
  //      }

  //      /// <summary>
  //      /// Peeks a decimal.
  //      /// </summary>
  //      [MethodImpl(MethodImplOptions.AggressiveInlining)]
  //      public decimal PeekDecimal()
  //      {
  //          return new decimal(new int[] { *(int*)(position), *(int*)(position + 4), *(int*)(position + 8), *(int*)(position + 12) });
  //      }

  //      /// <summary>
  //      /// Reads count bytes from the current position into data starting at offset.
  //      /// </summary>
  //      /// <param name="data">The byte array where data will be written to.</param>
  //      /// <param name="offset">The position in the byte array where those data will be written to.</param>
  //      /// <param name="count">The amount of bytes which will be written.</param>
  //      /// <remarks>BEWARE: This method is also NOT DOING input checks of the given parameters.</remarks>
  //      [MethodImpl(MethodImplOptions.AggressiveInlining)]
  //      public void PeekBytes(byte[] data, int offset, int count)
  //      {
  //          fixed (byte* pData = data)
  //              Buffer.MemoryCopy(position, pData + offset, count, count);
  //      }
    }
}
