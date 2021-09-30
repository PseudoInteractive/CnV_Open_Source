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
using Microsoft.Toolkit.HighPerformance.Buffers;
using Nito.AsyncEx;
using COTG.Views;
using System.ComponentModel;
using static COTG.Game.World;
using Cysharp.Text;
using EnumsNET;

namespace COTG.Game
{
	public enum AzureLoadState : byte
	{
		none,
		loading,
		loaded,
		doesNotExist,
		failed,
	}
	public class HeatMapItem : INotifyPropertyChanged
	{
		public uint[] a;
	
		public SmallTime t;
		public string desc { get; set; }
		public HeatMapItem(SmallTime t)
		{
			this.t = t;
			desc = t.ToString("s");

		}

	public virtual event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		public void NotifyChange(string member = "")
		{
			App.DispatchOnUIThreadLow(() =>
			{
				OnPropertyChanged(member);

			});
		}

		public override bool Equals(object obj)
		{
			return obj is HeatMapItem item &&
				   t.Equals(item.t);
		}

		public override int GetHashCode()
		{
			return t.secondsI;
		}

		public static bool operator ==(HeatMapItem left, HeatMapItem right)
		{
			return EqualityComparer<HeatMapItem>.Default.Equals(left, right);
		}

		public static bool operator !=(HeatMapItem left, HeatMapItem right)
		{
			return !(left == right);
		}
	}

	public class HeatMapDay : HeatMapItem
	{
		// key is lastUpdated.Date
		public static ResetableCollection<HeatMapDay> days = new() ;
		public ResetableCollection<HeatMapDelta> deltas { get; set; } = new(); //  ResetableCollection<HeatMapDelta>.empty; // only valid in heatMapDay

		
		public HeatMapDay(SmallTime t) : base(t)
		{
			desc = dateStr + " ..click to load";
			deltas.Add(HeatMapDelta.pending);
		}


		public string dateStr => t.ToString("yyyy-MM-dd");
		public bool isLoaded => loadState == AzureLoadState.loaded;
		public bool isLoadedOrDoesNotExist => loadState switch { AzureLoadState.loaded or AzureLoadState.doesNotExist=>true,_ => false }; 
		public bool loadHasBeenCalled => loadState >= AzureLoadState.loading;
		public AzureLoadState loadState;
		public override string ToString() => desc;

		public Azure.ETag eTag; // server version, if our version is not equal to the version on azure we discard current work and fetch the latest

		public uint[] snapshot = Array.Empty<uint>(); // 600*600
		private int deltaCount => hasDeltas ? deltas.Count : 0;

		public bool hasDeltas
		{
			get
			{
				Assert(deltas.Count != 0);
				return deltas.Count != 1 || deltas[0] != HeatMapDelta.pending;
			}
		}
		public int UncompressedSizeEstimate()
		{
			var rv = 1024 + World.spanSquared * sizeof(uint);
			foreach (var i in deltas)
			{
				rv += 16 + i.changes.Length * sizeof(uint);
			}
			return rv;
		}

		public void UpdateDesc(bool updateNext, bool updateSnapshots)
		{
			var prior = GetEarlier();
			
			if (!isLoaded || prior == null || !prior.isLoaded)
			{
				desc = dateStr +  " ..click to load";
			}
			else
			{
				desc = dateStr + new ChangeInfo().ComputeDeltas(prior.snapshot, snapshot).ToString();
			}

			NotifyChange(nameof(desc));
			if(updateSnapshots && isLoaded)
				UpdateSnapshots();
			if (updateNext)
			{
				var next = GetLater();
				if (next != null)
					next.UpdateDesc(false,false);
			}
		}
		public void UpdateSnapshots()
		{
			Assert(isLoaded);
			if (!isLoaded)
				return;
			var snap0 = new uint[snapshot.Length];
			var snap1 = snapshot.ToArray();
			
			if (hasDeltas)
			{
				foreach (var delta in deltas)
				{
					MemoryExtensions.CopyTo(snap1, snap0.AsSpan() );
					HeatMap.ApplyDelta(snap1, delta.changes);
					delta.desc = delta.timeStr + new ChangeInfo().ComputeDeltas(snap1, snap0).ToString();
					delta.NotifyChange(nameof(delta.desc));
				}
			}
		}

		internal int Write(byte[] buffer)
		{
			unsafe
			{
				fixed (byte* pData = buffer)
				{
					var o = new Writer(pData);



					o.Write(t);
					o.WritePackedUints(snapshot);

					int dCount = deltaCount;
					o.Write7BitEncoded(dCount);
					
					Trace($"Deltas: {dCount}");
					for (int i = 0; i < dCount; ++i)
					{
						var d = deltas[i];
						Assert(d.t.Date() == this.t.Date());
						o.Write(d.t);
						o.WritePackedUints(d.changes);
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
				t = r.ReadSmallTime();
				if (snapshot.Length > 0)
				{
					// dispose it
				}
				snapshot = r.ReadPackedUints();
				var dCount = r.Read7BitEncoded();
				Assert(deltas.Count > 0);
				if (dCount > 0)
				{
					deltas.Clear();
					for (int i = 0; i < dCount; ++i)
					{
						var t = r.ReadSmallTime();
						var ds = r.ReadPackedUints();
						if (t.Date() == this.t.Date())
						{
							deltas.Add(new HeatMapDelta(t, ds));
						}
						else
						{
							Log($"Invalid Delta {t}");
						}

					}

					// data is corrupt?
					if(!deltas.Any())
					{
						deltas.Add(HeatMapDelta.pending);
					}
					deltas.NotifyReset();
				}
				//NotifyChange();
			}

		}

		//private void CleanDelta()
		//{
		//	if (deltas.Count == 1 && deltas[0].deltas == null)
		//	{
		//		deltas.Clear();
		//	}
		//}
		public bool AddSnapshot(SmallTime t, uint[] newSnap)
		{
			Trace($"Add Snapshot: {desc} [{t}] [{this.t}]");
			if (snapshot.Length == 0)
			{
				this.t = t;
				snapshot = newSnap;
				Assert(!hasDeltas);
				return true;
			}

			//			var isDeltaBogus = deltas.Count == 1 && deltas[0].deltas.Length==0;


			if (t >= this.t)
			{
				// simple operation, add to head
				var delta = HeatMap.ComputeDelta(snapshot, newSnap);
				// only create a snapshot of there is enought space
				if (delta.Length < HeatMap.minDeltasPerSnapshot * 2)
				{
				//	Trace("Ignoring small delta");
				
					return false;
				}
				if (!hasDeltas)
					deltas.Clear();

				deltas.Insert(0, new HeatMapDelta(this.t, delta));
				this.t = t;
				//var prior = snapshot;
				snapshot = newSnap;
				//World.ReturnWorldBuffer(prior);
				return true;
			}
			else
			{
				return false;
			}
		}
		public bool IsLoadingOrLoaded()
		{
			var prior = GetEarlier();
			return (prior == null || prior.loadHasBeenCalled) && loadHasBeenCalled;
		}

		public async Task Load()
		{
			var prior = GetEarlier();
			if( (prior==null || prior.loadHasBeenCalled)&&loadHasBeenCalled)
			{
				return;
			}

			using (var _ = await HeatMap.mutex.LockAsync())
			{
				var _t0 = loadHasBeenCalled ? null : LoadInternal();
				var _t1 = prior == null || prior.loadHasBeenCalled ? null : prior.LoadInternal();
				if (_t0 != null)
					await _t0;
				if (_t1 != null)
					await _t1;

				UpdateDesc(true, true);
			}

//			NotifyChange();
		}
		
		public async Task<HeatMapDay> LoadInternal()
		{
			try
			{
				while(loadState == AzureLoadState.loading)
				{
					await Task.Delay(500).ConfigureAwait(false);
				}
				//
				if (loadState == AzureLoadState.none)
					loadState = AzureLoadState.loading;
			
				var cont = await Blobs.GetChangesContainer();
				if (cont == null)
					return null;
				var str = dateStr;

				try
				{
					// If etag changes it should be reloaded 
					// This might get loaded more than once if called again before prior load has completed but thats not fatal
					var res = await  cont.GetBlobClient(str).DownloadAsync(default, isLoaded? new BlobRequestConditions() { IfNoneMatch = eTag } : null );
					var status = res.GetRawResponse().Status;
					if ( status >= 300)
					{
						Assert(status == 304);
						// No need to load
					}
					else
					{
						Assert(status == 200);

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
						Assert(snapshot.Length > 0);
						ArrayPool<byte>.Shared.Return(readBuffer);
						eTag = res.Value.Details.ETag;
						loadState = AzureLoadState.loaded;
					}
					return this;
				}
				catch (Azure.RequestFailedException r)
				{
					if ((r.Status == 404))
					{
						loadState = AzureLoadState.doesNotExist;
					}
					else
					{
						Assert(isLoaded);
						if (loadState == AzureLoadState.loading)
						{
							Assert(false);
							loadState = AzureLoadState.failed;
						}
						Log(r.ErrorCode); // already loaded, leave it
					}
					//Assert(r.Status == 404);
			//		return isLoaded() ? this : null;
				}
				//	catch (Azure.NoBodyResponse<Azure.Storage.Blobs.Models.BlobDownloadStreamingResult>.ResponseBodyNotFoundException)
				//	{
				//		Assert(isLoaded);
				//		if (loadState == AzureLoadState.loading)
				//			loadState = AzureLoadState.failed;

				//		// etag mismatch
				////		return this;
				//	}
				catch (Exception e)
				{
				//	Assert(isLoaded);
					if (loadState == AzureLoadState.loading)
					{
						Assert(false);
						loadState = AzureLoadState.failed;
					}
					else
					{
						loadState = AzureLoadState.failed;
					}
					LogEx(e,false);
		//			return this;
				}
			}
			catch (Exception ex)
			{
				loadState = AzureLoadState.failed;

				Debug.LogEx(ex);
			}
			return isLoadedOrDoesNotExist ? this : null; 
		}

		// stored in reverse order, 0 is most recent
		public HeatMapDay GetLater()
		{
			var id = HeatMapDay.days.IndexOf(this);
			if (id > 0)
				return HeatMapDay.days[id - 1];
			else
				return null;

		}
		public HeatMapDay GetEarlier()
		{
			var id = HeatMapDay.days.IndexOf(this);
			if (id >= 0 && id + 1 < HeatMapDay.days.Count)
				return HeatMapDay.days[id + 1];
			else
				return null;

		}

	}

	public class HeatMapDelta : HeatMapItem
	{
		internal static HeatMapDelta pending = new(0, Array.Empty<uint>());

		public uint[] changes;
		public string timeStr => t.ToString("HH':'mm':'ss");


		public HeatMapDelta(SmallTime t, uint[] deltas) : base(t)
		{
			this.changes = deltas;

			if(deltas.Length > 0)
			{
				desc = timeStr;
			}
			else
			{
				desc = " ..pending";
			}
		}
		public override string ToString()
		{
			return $"{t.ToString("s")}: {changes.Length / 2} changes";
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
		//public static async Task<bool> Upload(HeatMapDay day)
		//{
		//	using var _ = await mutex.LockAsync();
		//	return await UploadInternal(day);
		//}
		public static async Task<bool> UploadInternal(HeatMapDay day)
		{
			
			var buffer = RentBuffer(day.UncompressedSizeEstimate());
			//byte[] compressedBuffer = null;
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
					//	Log(success.GetRawResponse());
					var result = success.GetRawResponse().Status;
					if(result >= 300)
					{
						Assert(result == 304); // changed, don't overwrite overwrite please
					}
					day.loadState = AzureLoadState.loaded;
	//				Log(success.Value.ETag);
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
				//if (compressedBuffer != null)
				//	ArrayPool<byte>.Shared.Return(compressedBuffer);
				if (buffer != null)
					ArrayPool<byte>.Shared.Return(buffer);
			}


		}
		public static async Task<uint[]> GetSnapshot(SmallTime t)
		{
	//		using var _ = await mutex.LockAsync();
			var day = GetDay(t, false);
			if (day == null)
			{
				Assert(false);
				return null;
//				day = HeatMapDay.days.First();
			}
			await day.LoadInternal();

			Assert(day.isLoaded);
			var rv = day.snapshot.ToArray();
			if (day.hasDeltas)
			{
				foreach (var d in day.deltas)
				{
					if (d.t < t)
						break;
					ApplyDelta(rv, d.changes);

				}
			}
			return rv;
		
		}
		


		static HeatMapDay GetDay(SmallTime t, bool createIfNotExists)
		{

			var date0 = t.Date();
			HeatMapDay day = null;
			int i = 0;
			for (; i < HeatMapDay.days.Count; ++i)
			{
				var date1 = HeatMapDay.days[i].t.Date();
				if (date1 <= date0)
				{
					if (date1== date0)
					{
						day = HeatMapDay.days[i];
					}
					break;
				}
			}

			if (day == null && createIfNotExists)
			{
				day = new(t);
			
				//day.deltas.Add(new HeatMapDelta( t , Array.Empty<uint>() ) );
				HeatMapDay.days.Insert(i, day);
			//	HeatTab.DaysChanged();
			}
			return day;

		}
		public static async Task LoadList()
		{
			using var _ = await HeatMap.mutex.LockAsync();
			try
			{
				var cont = await Blobs.GetChangesContainer();
				if (cont == null)
					return;
				await foreach (var b in cont.GetBlobsAsync())
				{
					var split = b.Name.Split('-');
					if (split.Length != 3)
						continue;

					var t = new SmallTime(new DateTimeOffset(int.Parse(split[0]), int.Parse(split[1]), int.Parse(split[2]), 0, 0, 0, TimeSpan.Zero));
					GetDay(t, true);
				}
			}
			catch (Exception ex)
			{
				LogEx(ex);
			}

		}



		public static async Task<(HeatMapDay day, bool modified)> AddSnapshot(SmallTime t, uint[] isSnap, bool uploadIfChanged)
		{
			using var ___ = await mutex.LockAsync();
			var day =await (GetDay(t,true)).LoadInternal();
			if (day != null)
			{
				var mod = day.AddSnapshot(t, isSnap);
			//	day.UpdateDesc(true,true);
				

				if (mod && uploadIfChanged)
				{
					await UploadInternal(day);
				}
				return (day, mod);
			}
			return (null, false);
		}

		public static unsafe uint[] ComputeDelta(Span<uint> d0,Span<uint> d1)
		{
			var count = spanSquared;
			Assert(d1.Length == count);
			Assert(d0.Length == count);
		
			List<uint> rv = new();
			for(var i = 0;i < count;++i)
			{
				var c0 = d0[i];
				var c1 = d1[i];
				if(c0 == c1)
				{
					continue;
				}
				rv.Add((uint)i);
				rv.Add(c0 ^ c1);

			}
			return rv.ToArray();
		}

		

		public static void ApplyDelta(Span<uint> d,Span<uint> delta)
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
	public class CityCounts
	{
		public int total;
		public int castles;
		public int big;
		public int temples;
		public CityCounts Sub( CityCounts b)
		{
			return new CityCounts() { total = total - b.total, castles = castles - b.castles, big = big - b.big, temples = temples - b.temples };
		}
		public static Dictionary<int, CityCounts> GetcityCountsByAlliance(ReadOnlySpan<uint> t0)
		{
			Dictionary<int, CityCounts> rv = new();
			for (int i = 0; i < World.spanSquared; ++i)
			{
				if (!Spot.TestContinentFilterPacked(PackedIdToPackedContinent((uint)i)))
					continue;
				var v0 = t0[i];
				if (World.GetType(v0) != typeCity)
					continue;
				var player0 = World.GetPlayer(v0);
				var alliance0 = Alliance.FromPlayer(player0);
				if (!rv.TryGetValue(alliance0, out var c))
				{
					c = new();
					rv.Add(alliance0, c);

				}
				var isTemple0 = IsTemple(v0);
				var isCastle0 = IsCastle(v0);
				var isBig0 = IsBig(v0);

				++c.total;
				if (isTemple0)
					++c.temples;
				if (isCastle0)
					++c.castles;
				if (isBig0)
					++c.big;



			}
			return rv;
		}
	}

	public class ChangeInfo
	{
		public byte allianceCaptures;
		public byte allianceLosses;
		public byte otherCaptures;
		public byte otherLosses;
		public byte allianceNew;
		public byte otherNew;
		public byte flattened;
		public byte shrines;
		public byte allianceCastles;
		public byte otherCastles;
		public byte abandonedCastles;
		public byte abandonedCities;
		public byte decayed;
		public byte grew;
		public byte portalsOpened;
		public byte newTemples;
		public byte destroyedTemples;
		
		public Dictionary<int, PlayerChanges> players = new();


		public override string ToString()
		{
			using var sb = ZString.CreateUtf8StringBuilder();
			if (newTemples > 0)
				sb.AppendFormat(", +{0} Temples", newTemples);
			if (destroyedTemples > 0)
				sb.AppendFormat(", -{0} Destroyed Temples", destroyedTemples);
			if (shrines> 0)
				sb.AppendFormat(", +{0} Shrines", shrines);
			if (allianceCaptures > 0)
				sb.AppendFormat(", {0} Alliance Caps", allianceCaptures);
			if (allianceLosses > 0)
				sb.AppendFormat(", {0} Lost", allianceLosses);
			if (otherLosses > 0)
				sb.AppendFormat(", {0} Other Losses", otherLosses);
			if (otherCaptures > 0)
				sb.AppendFormat(", {0} Other Caps", otherCaptures);
			if (allianceNew > 0)
				sb.AppendFormat(", +{0} Alliance Cities", allianceNew);
			if (otherNew > 0)
				sb.AppendFormat(", +{0} Cities", otherNew);
			if(abandonedCastles > 0)
				sb.AppendFormat(", {0} Abandoned Castles", abandonedCastles);
			if (abandonedCities > 0)
				sb.AppendFormat(", {0} Abandoned", abandonedCities);
			if(allianceCastles > 0)
				sb.AppendFormat(", +{0} Alliance Castles", allianceCastles);
			if (otherCastles > 0)
				sb.AppendFormat(", +{0} Castles", otherCastles);
			if(grew > 0)
				sb.AppendFormat(", {0} Renovations", grew);
			if (flattened > 0)
				sb.AppendFormat(", {0} Downgrades", flattened);
			if (decayed > 0)
				sb.AppendFormat(", {0} Decays", decayed);

			return sb.ToString();
		}

		public static bool includeCastle = false;
		public static bool includeSettle = false;
		public static bool includeFlatten = false;

		public static string GetChangeDesc(uint v0, uint v1)
		{
			if (v0 == v1)
				return null; 
			var type0 = World.GetType(v0);
			var type1 = World.GetType(v1);
			if (type0 == typeShrine || type1 == typeShrine)
			{
				var d = World.GetData(v1);
				if (d != 255)
					return $"New { ((Faith)(d-1)).AsString() }";

			}
			else if (type0 == typePortal || type1 == typePortal)
			{
				if (World.GetData(v1) != 0)
					return "Portal Opened";
			}
			else
			{
				Assert(type0 == typeCity || type1 == typeCity);

				var player0 = World.GetPlayer(v0);
				var player1 = World.GetPlayer(v1);

				var alliance0 = Alliance.FromPlayer(player0);
				var alliance1 = Alliance.FromPlayer(player1);
				var isTemple0 = IsTemple(v0);
				var isTemple1 = IsTemple(v1);
				var isCastle0 = IsCastle(v0);
				var isCastle1 = IsCastle(v1);
				var isBig0 = IsBig(v0);
				var isBig1 = IsBig(v1);
				if (isTemple0 != isTemple1)
				{
					if (isTemple1)
						return $"{Player.IdToName(player1)} Became Temple";
					else
						return $"{Player.IdToName(player1)} No longer Temple :(";
				}
				if (player0 != player1)
				{
					// New city
					if (type0 == 0)
					{
						Assert(player1 != 0);
						if( isBig1)
							return $"{Player.IdToName(player1)} speed built";
						else if (isCastle1)
							return $"{Player.IdToName(player1)} fast castle";
						else if(includeSettle)
							return $"{Player.IdToName(player1)} settled city";

					}
					else
					{
						if (player1 == 0  )
						{
							if((isBig0 || isCastle0))
								return $"{Player.IdToName(player0)} abandoned";
						}
						else if (player0 == 0)
						{
							if( isBig1 || isCastle1)
							 return  $"{Player.IdToName(player1)} took lawless";
						}
						else if (isCastle0)
						{
							return $"{Player.IdToName(player0)} conquered by {Player.IdToName(player1)}";
						}
					}
				}
				if (isBig0 != isBig1 && isBig0)
				{
					if (isCastle0 && player0 != 0 && player1 != 0)
						return includeFlatten ? $"{Player.IdToName(player1)} was flattened" : null;
				}
				if (isCastle0 != isCastle1)
				{
					if (isCastle1)
					{
						return includeCastle ? $"{Player.IdToName(player1)} castled" : null;
					}
				}
			}
			return null;
		}

		public ChangeInfo ComputeDeltas(ReadOnlySpan<uint> t0, ReadOnlySpan<uint> t1 )
		{
			Assert(t0.Length == World.spanSquared);
			Assert(t1.Length == World.spanSquared);
			for (int i=0;i<World.spanSquared;++i)
			{
				if(!Spot.TestContinentFilterPacked(PackedIdToPackedContinent((uint)i)))
					continue;

				var v0 = t0[i];
				var v1 = t1[i];
				if (v0 == v1)
					continue;
				var type0 = World.GetType(v0);
				var type1 = World.GetType(v1);
				if (type0 == typeShrine || type1 == typeShrine)
				{
					if (World.GetData(v1) != 255)
						++shrines;

				}
				else if (type0 == typePortal || type1 == typePortal)
				{
					if (World.GetData(v1) != 0)
						++portalsOpened;
				}
				else if(type0 == typeCity || type1 == typeCity)
				{

					var player0 = World.GetPlayer(v0);
					var player1 = World.GetPlayer(v1);

					// pid0 is lawless
					if (!players.TryGetValue(player0,out var p0))
					{
						p0 = new PlayerChanges() { pid = player0 };
						if(Player.Get(player0).cities.Count > 1 )
							players.Add(player0,p0);
					}
					if (!players.TryGetValue(player1, out var p1))
					{
						p1 = new PlayerChanges() { pid = player1 };
						if (Player.Get(player1).cities.Count > 1)
							players.Add(player1,p1);
					}


					var alliance0 = Alliance.FromPlayer(player0);
					var alliance1 = Alliance.FromPlayer(player1);
					var isTemple0 = IsTemple(v0);
					var isTemple1 = IsTemple(v1);
					var isCastle0 = IsCastle(v0);
					var isCastle1 = IsCastle(v1);
					var isBig0 = IsBig(v0);
					var isBig1 = IsBig(v1);
					if(isTemple0 != isTemple1)
					{
						if (isTemple1)
						{
							++newTemples;
							++p1.templesMade;
						}
						else
						{
							++destroyedTemples;
							++p0.templesLost;
						}
					}
					if(isBig0 != isBig1 )
					{
						if (player0 != 0 && player1 != 0)
						{
							if(isBig0)
							{
								++flattened;
								++p0.flattened;
							}
							else
							{
								++grew;
								++p1.upgraded;
							}
						}
					}
					if(isCastle0!=isCastle1 )
					{
						if(isCastle1)
						{
							if (Alliance.IsMine(alliance1))
								++allianceCastles;
							else
								++otherCastles;
							++p1.castled;
						}
					}
					if (player0 != player1)
					{
						// New city
						if(type0 == 0)
						{
							if (Alliance.IsMine(alliance1))
								++allianceNew;
							else
								++otherNew;
							++p1.settled;
							if (isCastle1)
								++p1.castled;
						}
						else
						{
							if (player1 == 0)
							{
								if (isCastle0)
								{
									++abandonedCastles;
									++p0.castlesAbandoned;
								}
								else
								{
									++abandonedCities;
									++p1.abandonedCities;
								}
							}
							else if (player0 == 0)
							{
								// settled lawless
								if (Alliance.IsMine(alliance1))
									++allianceNew;
								else
									++otherNew;
								if (isCastle0)
								{
									++p1.castlesCappedLawless;

									if (Alliance.IsMine(alliance1))
									{
										++allianceCaptures;
									}
									else if (alliance0 != alliance1)
									{
										++otherCaptures;
									}
								}
								else
								{
									++p1.settled;
								}
							}
							else if (isCastle0)
							{
								var allianceChanged = (alliance0 != alliance1);
								if (allianceChanged)
								{
									++p0.castlesLostEnemy;
									++p1.castlesCappedEnemy;
									// Capture
									if (Alliance.IsMine(alliance0))
									{
										++allianceLosses;
									}
									else if (Alliance.IsMine(alliance1))
									{
										++allianceCaptures;
									}
									else if (alliance0 != alliance1)
									{
										++otherCaptures;
									}
								}
							}
							else
							{
								++p0.castlesLostAlly;
								++p1.castlesCappedAlly;

							}

						}
					
					}

					if (type0 != type1)
					{

						if (type0 == typeCity)
						{
							// decayed, ignore
							++decayed;

						}
					}
					else
					{

					}
				}
				else
				{
				//	Assert(type0 == typeCity || type1 == typeCity);
				}


			}
			return this;
		}
	}


}
