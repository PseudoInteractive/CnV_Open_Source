using Azure.Storage.Blobs.Models;

using COTG.BinaryMemory;
using COTG.Services;

using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using ZstdNet;

using static COTG.Debug;

namespace COTG.Game
{
/*	public class HeatMapDay
	{
		
		// key is lastUpdated.Date
		public static SortedList<SmallTime,HeatMapDay> cache = new();

		public SmallTime lastUpdate;
		public string dateStr => lastUpdate.ToString("yyyy-MM-dd");

		public Azure.ETag eTag; // server version, if our version is not equal to the version on azure we discard current work and fetch the latest

		public uint[] snapshot = Array.Empty<uint>(); // 600*600
		public List<HeatMapDelta> deltas = new();

		public int UncompressedSizeEstimate()
		{
			var rv = 1024 + World.worldDim * World.worldDim * sizeof(uint);
			foreach(var i in deltas)
			{
				rv += 16 + i.deltas.Length * sizeof(uint);
			}
			return rv;
		}

		internal void Write(ref Writer o)
		{
			o.Write(lastUpdate);
			o.WritePackedUints(snapshot);
			int dCount = deltas.Count;
			o.Write7BitEncoded(dCount);
			for (int i=0;i<dCount;++i)
			{
				var d = deltas[i];

				o.Write(d.t);
				o.WriteUints(d.deltas);
			}
		}
		internal unsafe void Read( byte[] data)
		{
			fixed (byte* pData = data)
			{
				var r = new Reader(pData);
				lastUpdate = r.ReadSmallTime();
				snapshot = r.ReadPackedUints();
				var dCount = r.Read7BitEncoded();
				
				for (int i = 0; i < dCount; ++i)
				{
					var t =  r.ReadSmallTime();
					var ds = r.ReadUints();
					deltas.Add( new HeatMapDelta(t,ds) );
				}
			}
		}
		public bool AddSnapshot(SmallTime t, uint [] snap)
		{
			if(snapshot.Length==0)
			{
				lastUpdate = t;
				snapshot = snap;
				Assert(deltas.Count == 0);
				return true;

			}
		
			if( t >= lastUpdate)
			{
				// simple operation, add to head
				var delta = HeatMap.ComputeDelta(snapshot, snap);
				// only create a snapshot of there is enought space
				if (delta.Length  < HeatMap.minDeltasPerSnapshot*2)
					return false;
				deltas.Add(new HeatMapDelta(lastUpdate, delta));
				lastUpdate = t;
				snapshot = snap;
				return true;
			}
			else
			{
				var temp = snapshot;
				int offset;
				for (offset=0;offset<deltas.Count;++offset)
				{
					if (deltas[offset].t <= t)
						break;
					HeatMap.ApplyDelta(temp, deltas[offset].deltas );
				}
				var newDelta = HeatMap.ComputeDelta(temp, snap);
				if (newDelta.Length < HeatMap.minDeltasPerSnapshot * 2)
				{
					return false;

				}
				if (offset + 1 < deltas.Count)
				{
					HeatMap.ApplyDelta(temp, deltas[offset].deltas);
					deltas[offset] = new HeatMapDelta(deltas[offset].t, HeatMap.ComputeDelta(temp,snap));
				}
				deltas.Insert(0, new HeatMapDelta(t, newDelta));
				return true;
			}



		}
	}
	public struct HeatMapDelta
	{
		public SmallTime t;
		public uint[] deltas;


		public HeatMapDelta(SmallTime t, uint[] deltas) 
		{
			this.t = t;
			this.deltas = deltas;
		}
		
	}*/

	public static class HeatMap
	{
		/*
		public const int minDeltasPerSnapshot = 24;
		static int readBufferSize = 128 * 1024;
		static BinaryData SaveToBuffer(HeatMapDay day)
		{
			var size = day.UncompressedSizeEstimate();
			unsafe
			{
				var buffer = ArrayPool<byte>.Shared.Rent(size);
				BinaryData data;

				try
				{

					fixed (byte* pData = buffer)
					{
						var o = new Writer(pData);
						day.Write(ref o);


						using (var compressor = new ZstdNet.Compressor())
						{
							data = new BinaryData(compressor.Wrap(new ReadOnlySpan<byte>(pData, (int)(o.Position - pData))));

						}
					}
				}
				finally
				{
					ArrayPool<byte>.Shared.Return(buffer);
				}

				return data;
			}
		}

		static async Task<bool> SaveAsync(HeatMapDay day)
		{
			try
			{
				var cont = await Blobs.GetChangesContainer();
				if (cont == null)
					return false;
				var data = SaveToBuffer(day);
				var str = day.dateStr;
				// What abougt etag == null?
				var success = await cont.GetBlobClient(str).UploadAsync(data, new BlobUploadOptions() { Conditions = new BlobRequestConditions() { IfMatch = day.eTag } });
				LogJson(success.GetRawResponse());
				if (success.GetRawResponse().Status != 400)
				{
					return false;
				}

				Log(success.Value.ETag);
				day.eTag = success.Value.ETag;

				return true;
			}
			catch (Exception ex)
			{
				Debug.Log(ex);
			}
			return false;
		}
		static async Task<HeatMapDay> LoadAsync(SmallTime t)
		{
			try
			{
				var date = t.Date();
				if (!HeatMapDay.cache.TryGetValue(date, out var day ))
				{
					day = new();
					day.lastUpdate = t;
					HeatMapDay.cache.TryAdd(date,day);
				}
				var cont = await Blobs.GetChangesContainer();
				if (cont == null)
					return day;
				var str = day.dateStr;

				var res = await cont.GetBlobClient(str).DownloadAsync(default,new BlobRequestConditions() { IfNoneMatch = day.eTag } );
				if (res.GetRawResponse().Status != 400)
				{
					return day;
				}
				byte [] data;
				using (var compressor = new ZstdNet.DecompressionStream(res.Value.Content))
				{
					Log(compressor.Length);
					for (; ; )
					{
						data = ArrayPool<byte>.Shared.Rent(readBufferSize);
						var i = await compressor.ReadAsync(data, 0, readBufferSize);
						if(i < readBufferSize)
						{
							break;
						}
						readBufferSize *= 2;
						ArrayPool<byte>.Shared.Return(data);
					}
				}
				day.Read(data);
				ArrayPool<byte>.Shared.Return(data);
				day.eTag = res.Value.Details.ETag;
	

				return day;
			}
			catch (Exception ex)
			{
				Debug.Log(ex);
			}
			return null;
		}
		public static void AddSnapshot(SmallTime t, uint [] snap)
		{
			var day = LoadAsync(t);
			
		}

		*/
		public static uint[] ComputeDelta(uint[] d0, uint[] d1)
		{
			var rv = new List<uint>();
			int count = d0.Length;
			Assert(d1.Length == count);
			for (int i = 0; i < count; ++i)
			{
				var c0 = d0[i];
				var c1 = d1[i];
				if (c0 == c1
					|| ((c0 & World.typeMask) == World.typeBoss)
					|| ((c0 & World.typeMask) == World.typeDungeon)
					|| ((c1 & World.typeMask) == World.typeBoss)
					|| ((c1 & World.typeMask) == World.typeDungeon))
				{
					continue;
				}
				rv.Add((uint)i);
				rv.Add(c0 ^ c1);

			}
			return rv.ToArray();
			/*
            var outCount = rv.Count;
            if (outCount <= 64)
                return null;
            var bytes = new byte[outCount*4];
            for (int i = 0; i < outCount; ++i)
            {
                CopyBytes(rv[i], bytes, i);
            }
            return bytes;*/

		}
		public static void ApplyDelta(uint[] d, uint[] delta)
		{
			int count = delta.Length / 2;
			for (int i = 0; i < count; ++i)
			{
				var off = delta[i * 2];
				var x = delta[i * 2 + 1];
				d[off] ^= x;

			}
		}
		
	}
}
