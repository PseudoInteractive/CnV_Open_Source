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
//using ImpromptuNinjas.ZStd;

using static COTG.Debug;
using COTG.Helpers;
using System.IO.Compression;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.HighPerformance.Buffers;
using Nito.AsyncEx;
using COTG.Views;
using System.ComponentModel;
using JM.LinqFaster;

namespace COTG.Game
{
	public interface IHeatMapItem
	{
		public ResetableCollection<HeatMapDelta> children { get; } // only valid in heatMapDay
		public string label { get; }
		public bool hasUnrealizedChildren { get; }

	}

	public class HeatMapDay : IHeatMapItem, INotifyPropertyChanged
	{
		// key is lastUpdated.Date
		public static ResetableCollection<HeatMapDay> days = new();

		public SmallTime lastUpdate;
		public string dateStr => lastUpdate.ToString("yyyy-MM-dd");
		public string desc => dateStr + (isInitialized ? $" - {deltas.Count + 1} snaps" : "...");
		public bool isInitialized => snapshot.Length > 0;


		public Azure.ETag eTag; // server version, if our version is not equal to the version on azure we discard current work and fetch the latest

		public MemoryOwner<uint> snapshot = MemoryOwner<uint>.Empty; // 600*600
		public ResetableCollection<HeatMapDelta> deltas { get; set; } = new();
		ResetableCollection<HeatMapDelta> IHeatMapItem.children => deltas;
		string IHeatMapItem.label => desc;
		bool IHeatMapItem.hasUnrealizedChildren => !isInitialized ;


		public int UncompressedSizeEstimate()
		{
			var rv = 1024 + World.spanSquared * sizeof(uint);
			foreach (var i in deltas)
			{
				rv += 16 + i.deltas.Length * sizeof(uint);
			}
			return rv;
		}
		public override string ToString()
		{
			return desc;
		}

		public virtual event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		public void NotifyChange(string member = "")
		{
			App.DispatchOnUIThreadSneakyLow(() =>
			{
				OnPropertyChanged(member);
				Debug.Log("NotifyChange");

			});
		}

		internal int Write(byte[] buffer)
		{
			unsafe
			{
				fixed (byte* pData = buffer)
				{
					var o = new Writer(pData);



					o.Write(lastUpdate);
					o.WritePackedUints(snapshot.Span);
					var a = snapshot.Span.CountNonZero();
					Assert(a != 0);

					int dCount = deltas.Count;
					o.Write7BitEncoded(dCount);
					Assert(dCount <= 16);
					Trace($"Deltas: {dCount}");
					for (int i = 0; i < dCount; ++i)
					{
						var d = deltas[i];

						o.Write(d.t);
						Assert(d.deltas.Span.CountZero() == 0);
						o.WritePackedUints(d.deltas.Span);
					}
					return (int)(o.Position - pData);
				}
			}

		}
		internal unsafe void Read(byte[] data)
		{
			fixed (byte* pData = data)
			{
				var r = new Reader(pData);
				lastUpdate = r.ReadSmallTime();
				if (snapshot.Length > 0)
				{
					// dispose it
				}
				snapshot = r.ReadPackedUints();
				var a = snapshot.Span.CountNonZero();
				Assert(a != 0);
				var dCount = r.Read7BitEncoded();
				deltas.Clear();
				for (int i = 0; i < dCount; ++i)
				{
					var t = r.ReadSmallTime();
					var ds = r.ReadPackedUints();
					Assert(ds.Span.CountZero() == 0);

					deltas.Add(new HeatMapDelta(t, ds));

				}
				deltas.NotifyReset();
				NotifyChange();
			}

		}

		//private void CleanDelta()
		//{
		//	if (deltas.Count == 1 && deltas[0].deltas == null)
		//	{
		//		deltas.Clear();
		//	}
		//}
		public bool AddSnapshot(SmallTime t, MemoryOwner<uint> newSnap)
		{
			Trace($"Add Snapshot: {desc} [{t}] [{lastUpdate}]");
			if (!isInitialized)
			{
				lastUpdate = t;
				snapshot = newSnap;
				Assert(snapshot.Span.CountNonZero() != 0);
				Assert(deltas.Count == 0);
				return true;
			}

			//			var isDeltaBogus = deltas.Count == 1 && deltas[0].deltas.Length==0;


			if (t >= lastUpdate)
			{
				// simple operation, add to head
				var delta = HeatMap.ComputeDelta(snapshot.Span, newSnap.Span);
				// only create a snapshot of there is enought space
				if (delta.Length < HeatMap.minDeltasPerSnapshot * 2)
				{
					Trace("Ignoring small delta");
					World.ReturnWorldBuffer(newSnap);
					return false;
				}
				
				deltas.Insert(0, new HeatMapDelta(lastUpdate, delta));
				lastUpdate = t;
				//var prior = snapshot;
				snapshot = newSnap;
				Assert(snapshot.Span.CountNonZero() != 0);
				//World.ReturnWorldBuffer(prior);
				return true;
			}
			else
			{

				using var temp = snapshot.AsMemoryOwner();
				var tSpan = temp.Span;
				var snapshotSpan = snapshot.Span;


				int offset;
				for (offset = 0; offset < deltas.Count; ++offset)
				{
					if (deltas[offset].t <= t)
						break;
					HeatMap.ApplyDelta(tSpan, deltas[offset].deltas.Span);
				}
				var newDelta = HeatMap.ComputeDelta(tSpan, newSnap.Span);
				if (newDelta.Length < HeatMap.minDeltasPerSnapshot * 2)
				{
					Trace($"Ignoring small delta {newDelta.Length}");
					newSnap.Dispose();
					return false;
				}
				if (offset < deltas.Count)
				{
					HeatMap.ApplyDelta(tSpan, deltas[offset].deltas.Span);
					var newDelta1 = HeatMap.ComputeDelta(tSpan, newSnap.Span);
				
					if (newDelta1.Length < HeatMap.minDeltasPerSnapshot * 2)
					{
						Trace($"Ignoring small delta {deltas[offset].deltas.Length} => {newDelta1.Length}");
						newSnap.Dispose();
						newDelta1.Dispose();
						return false;
					}
					Trace($"ChangeDelta {deltas[offset].deltas.Length} => {newDelta1.Length}" );
					deltas[offset].deltas = newDelta1;/// HeatMap.ComputeDelta(temp,newSnap);
				}

				newSnap.Dispose();
				deltas.Insert(offset, new HeatMapDelta(t, newDelta));
				for (int i = 0; i < deltas.Count - 1; ++i)
				{
					Assert(deltas[i].t >= deltas[i + 1].t);
				}
				return true;
			}
		}
		public async Task<HeatMapDay> Load()
		{
			using var _ = await HeatMap.mutex.LockAsync();
			return await LoadInternal();
		}
		public async Task<HeatMapDay> LoadInternal()
		{
			try
			{

				var cont = await Blobs.GetChangesContainer();
				if (cont == null)
					return this;
				var str = dateStr;

				try
				{
					var res = await cont.GetBlobClient(str).DownloadAsync(default, new BlobRequestConditions() { IfNoneMatch = eTag });
					//if (res.GetRawResponse().Status != 200)
					//{
					//	return day;
					//}

					using var deflate = new GZipStream(res.Value.Content, CompressionMode.Decompress);
					byte[] readBuffer = ArrayPool<byte>.Shared.Rent(HeatMap.bufferSize);
					var readOffset = 0;
					for (; ; )
					{

						var readSize = deflate.Read(readBuffer, readOffset, HeatMap.bufferSize - readOffset);
						if (readSize < HeatMap.bufferSize)
						{
							break;
						}
						readOffset += readSize;
						var _readBuffer = readBuffer;
						HeatMap.bufferSize *= 2;
						readBuffer = ArrayPool<byte>.Shared.Rent(HeatMap.bufferSize);
						for (int i = 0; i < readOffset; ++i)
							readBuffer[i] = _readBuffer[i];
						ArrayPool<byte>.Shared.Return(_readBuffer);
					}

					Read(readBuffer);
					ArrayPool<byte>.Shared.Return(readBuffer);
					eTag = res.Value.Details.ETag;
					return this;
				}
				catch (Azure.RequestFailedException r)
				{
					Log(r.ErrorCode);
					Assert(r.Status == 404);
					return this;
				}
				catch (Exception e)
				{
					return this;
				}
			}
			catch (Exception ex)
			{
				Debug.LogEx(ex);
			}
			return null;
		}
	}

	public class HeatMapDelta : IHeatMapItem
	{
		public SmallTime t;
		public MemoryOwner<uint> deltas;
		public string timeStr => $"{t.ToString("HH':'mm':'ss")} - {deltas.Length / 2} changes";

		ResetableCollection<HeatMapDelta> IHeatMapItem.children => null;
		string IHeatMapItem.label => timeStr;
		bool IHeatMapItem.hasUnrealizedChildren => false;

		public HeatMapDelta(SmallTime t, MemoryOwner<uint> deltas)
		{
			this.t = t;
			this.deltas = deltas;

			Trace($"Delta: {this}");
			if(deltas.Length > 200)
			{
				int q = 0;
			}

		}
		public override string ToString()
		{
			return timeStr;
		}
	}

	public static class HeatMap
	{
		public static AsyncLock mutex = new();
		public const int minDeltasPerSnapshot = 32;
		public static int bufferSize = 1024 * 1024;

		static byte[] RentBuffer(int minSize)
		{
			if (minSize > bufferSize)
				bufferSize = minSize;
			return ArrayPool<byte>.Shared.Rent(bufferSize);
		}
		public static async Task<bool> Upload(HeatMapDay day)
		{
			using var _ = await mutex.LockAsync();
			return await UploadInternal(day);
		}
		public static async Task<bool> UploadInternal(HeatMapDay day)
		{
			
			var buffer = RentBuffer(day.UncompressedSizeEstimate());
			byte[] compressedBuffer = null;
			//	BinaryData data;
			try
			{
				int bufferSize = day.Write(buffer);


				using (var mem = new MemoryStream())
				{
					using (var deflate = new GZipStream(mem, CompressionLevel.Optimal, true))
					{
						deflate.Write(buffer, 0, bufferSize);
						deflate.Flush();
					}
					mem.Flush();
					mem.Seek(0, SeekOrigin.Begin);


					var cont = await Blobs.GetChangesContainer();
					if (cont == null)
						return false;
					var str = day.dateStr;
					// What abougt etag == null?
					var success = await cont.GetBlobClient(str).UploadAsync(mem, new BlobUploadOptions() { Conditions = new BlobRequestConditions() { IfMatch = day.eTag } });
					Log(success.GetRawResponse());
					//if (success.GetRawResponse().Status != 200)
					//{
					//	return false;
					//}

					Log(success.Value.ETag);
					day.eTag = success.Value.ETag;
				}
				return true;

			}
			catch (Exception ex)
			{
				LogEx(ex);
				return false;
			}
			finally
			{
				if (compressedBuffer != null)
					ArrayPool<byte>.Shared.Return(compressedBuffer);
				if (buffer != null)
					ArrayPool<byte>.Shared.Return(buffer);
			}


		}
		static HeatMapDay GetDay(SmallTime t)
		{

			var date0 = t.Date();
			HeatMapDay day = null;
			int i = 0;
			for (; i < HeatMapDay.days.Count; ++i)
			{
				if (HeatMapDay.days[i].lastUpdate >= date0)
				{
					if (HeatMapDay.days[i].lastUpdate.Date() == date0)
					{
						day = HeatMapDay.days[i];
					}
					break;
				}
			}

			if (day == null)
			{
				day = new();
				day.lastUpdate = t;
				//day.deltas.Add(new HeatMapDelta( t , Array.Empty<uint>() ) );
				HeatMapDay.days.Insert(i, day);
				HeatTab.DaysChanged();
			}
			return day;

		}
		public static async Task LoadList()
		{
			using var _ = await HeatMap.mutex.LockAsync();

			var cont = await Blobs.GetChangesContainer();
			if (cont == null)
				return;
			await foreach (var b in cont.GetBlobsAsync())
			{
				var split = b.Name.Split('-');
				if (split.Length != 3)
					continue;

				var t = new SmallTime(new DateTimeOffset(int.Parse(split[0]), int.Parse(split[1]), int.Parse(split[2]), 0, 0, 1, TimeSpan.Zero));
				GetDay(t);
			}

		}



		public static async Task<(HeatMapDay day, bool modified)> AddSnapshot(SmallTime t, uint[] isSnap, bool uploadIfChanged)
		{
			using var _ = await mutex.LockAsync();
			var day =await (GetDay(t)).LoadInternal();
			if (day != null)
			{
				var mod = day.AddSnapshot(t, World.SwizzleForCompression(isSnap));
				
				if (mod && uploadIfChanged)
				{
					await UploadInternal(day);
				}
				return (day, mod);
			}
			return (null, false);
		}

		public static MemoryOwner<uint> ComputeDelta(Span<uint> d0, Span<uint> d1)
		{
			var rv = new DArray<uint>(World.spanSquared);
			int count = d0.Length;
			Assert(d1.Length == count);
			for (int i = 0; i < count; ++i)
			{
				var c0 = d0[i];
				var c1 = d1[i];
				if (c0 == c1)
				{
					continue;
				}
				rv.Add((uint)i);
				rv.Add(c0 ^ c1);

			}
			var m = MemoryOwner<uint>.Allocate(rv.count);
			for (int i = 0; i < rv.count; ++i)
				m.Span[i] = rv[i];
			return m;
		}

		public static void ApplyDelta(Span<uint> d, ReadOnlySpan<uint> delta)
		{

			var count = delta.Length / 2;
			for (var i = 0; i < count; ++i)
			{
				var off = delta[i * 2];
				var x = delta[i * 2 + 1];
				d[(int)off] ^= x;

			}
		}

	}
}
