﻿using Azure.Storage.Blobs;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using COTG.Game;
using COTG.Helpers;
using System.IO;
using System.IO.Compression;
using COTG.Views;
using Azure.Storage.Blobs.Models;
using System.Buffers;
using COTG.BinaryMemory;
using static COTG.Debug;
namespace COTG.Services
{
	class Blobs
	{
		const string connectionString = "DefaultEndpointsProtocol=https;AccountName=avata;AccountKey=IWRPGlttorpK5DcHWin/GdA2VEcZKnHkr30lE0ZDvKLG0q1CjZONcAQYI2D26DENd7TIAxF8tPsE0mIk98BafA==;EndpointSuffix=core.windows.net";

		struct Stat
		{
			public string blob;
			public int id;

			public Stat(string blob, int id)
			{
				this.blob = blob;
				this.id = id;
			}
		}

		static string statsContainerName => $"r{JSClient.world}";
		static string statsTSContainerName => $"t{JSClient.world}{Alliance.MyId}";
		static string changesContainerName => $"c{JSClient.world}";

		const int timeBetweenSnapshots = 4 * 60;
		const int minTimeBetweenSnapshots = timeBetweenSnapshots - 15;
		const int maxTimeBetweenSnapshots = timeBetweenSnapshots + 15;

		static BlobContainerClient snapShotContainer;
		static BlobContainerClient tsSnapShotContainer;

		static async Task<BlobContainerClient> GetSnapshotContainer()
		{
			if (snapShotContainer == null)
			{
				snapShotContainer = new BlobContainerClient(connectionString, statsContainerName, GetClientOptions());
				await snapShotContainer.CreateIfNotExistsAsync();

			}
			return snapShotContainer;
		}
		static async Task<BlobContainerClient> GetTSSnapshotContainer()
		{
			if (tsSnapShotContainer == null)
			{
				tsSnapShotContainer = new BlobContainerClient(connectionString, statsTSContainerName, GetClientOptions());
				await tsSnapShotContainer.CreateIfNotExistsAsync();
			}
			return tsSnapShotContainer;
		}


		//const int timeBetweenSnapshots = 5;
		//const int minTimeBetweenSnapshots = timeBetweenSnapshots - 1;
		//const int maxTimeBetweenSnapshots = timeBetweenSnapshots + 1;
		public static async void ProcessStats()
		{
			for (; ; )
			{

				var container = await GetSnapshotContainer();

				var info = await container.GetPropertiesAsync();
				var lastWritten = info.Value.LastModified;// + TimeSpan.FromHours(12) + TimeSpan.FromMinutes(AMath.random.Next(60)-30);
				var currentT = DateTimeOffset.UtcNow;
				var dt = currentT - lastWritten;
				if (dt.TotalMinutes > minTimeBetweenSnapshots)
				{
					COTG.Debug.Trace("Snapshot");
					// take a snapshot
					var snap = await Snapshot.GetStats();
					using (var mem = new MemoryStream())
					{
						using (var deflate = new GZipStream(mem, CompressionLevel.Optimal, true))
						{
							using (var writer = new BinaryWriter(deflate, Encoding.UTF8, true))
							{
								writer.Write(snap.time);
								writer.Write(snap.playerStats.Count);
								foreach (var pss in snap.playerStats)
								{
									var ps = pss.Value;

									writer.Write(ps.pid);
									writer.Write(ps.reputation);
									writer.Write(ps.offensiveRep);
									writer.Write(ps.defensiveRep);
									writer.Write(ps.unitKills);
									writer.Write(ps.alliance);
									writer.Write(ps.raiding);
									writer.Write(ps.perContinent.Count);
									foreach (var c in ps.perContinent)
									{
										writer.Write(c.continent);
										writer.Write(c.cities);
										writer.Write(c.score);

									}
								}
								writer.Write(snap.allianceStats.Count);
								foreach (var pss in snap.allianceStats)
								{
									var ps = pss.Value;

									writer.Write(ps.aid);
									writer.Write(ps.reputation);
									writer.Write(ps.perContinent.Count);

									foreach (var c in ps.perContinent)
									{
										writer.Write(c.continent);
										writer.Write(c.cities);
										writer.Write(c.score);
										writer.Write(c.military);
									}
								}

							}
						}
						try
						{
							mem.Flush();
							mem.Seek(0, SeekOrigin.Begin);
							var str = snap.time.dateTime.FormatFileTime();
							await container.UploadBlobAsync(str, mem);
							await container.SetMetadataAsync(new Dictionary<string, string>() { { "last", str } });

						}
						catch (Exception ex)
						{
							Debug.LogEx(ex);
						}

					}
					lastWritten = currentT;
				}

				var nextSnapShot = lastWritten + TimeSpan.FromMinutes(AMath.random.Next(minTimeBetweenSnapshots, maxTimeBetweenSnapshots));

				await Task.Delay((nextSnapShot - currentT));
			}
		}

		public static async void ProcessTSStats()
		{
			for (; ; )
			{
				var container = await GetTSSnapshotContainer();
				
				var info = await container.GetPropertiesAsync();
				var lastWritten = info.Value.LastModified;// + TimeSpan.FromHours(12) + TimeSpan.FromMinutes(AMath.random.Next(60)-30);
				var currentT = DateTimeOffset.UtcNow;
				var dt = currentT - lastWritten;
				if (dt.TotalMinutes > minTimeBetweenSnapshots)
				{
					COTG.Debug.Trace("Snapshot");
					// take a snapshot
					var snap = await TSSnapshot.GetStats();
					using (var mem = new MemoryStream())
					{
						using (var deflate = new GZipStream(mem, CompressionLevel.Optimal, true))
						{
							using (var writer = new BinaryWriter(deflate, Encoding.UTF8, true))
							{
								writer.Write(snap.time);
								writer.Write(snap.continents.Count);
								foreach (var cont in snap.continents)
								{
									writer.Write(cont.continent);
									writer.Write(cont.players.Count);
									foreach (var p in cont.players)
									{
										writer.Write(p.pid);
										writer.Write(p.score);
										writer.Write(p.cities);
										writer.Write(p.tsTotal);
										writer.Write(p.tsOff);
										writer.Write(p.tsDef);
									}
								}
							}
						}
						try
						{
							mem.Flush();
							mem.Seek(0, SeekOrigin.Begin);
							var str = snap.time.dateTime.FormatFileTime();
							await container.UploadBlobAsync(str, mem);
							await container.SetMetadataAsync(new Dictionary<string, string>() { { "last", str } });

						}
						catch (Exception ex)
						{
							Debug.LogEx(ex);
						}

					}
					lastWritten = currentT;
				}

				var nextSnapShot = lastWritten + TimeSpan.FromMinutes(AMath.random.Next(minTimeBetweenSnapshots, maxTimeBetweenSnapshots));

				await Task.Delay((nextSnapShot - currentT));
			}
		}

		public static int snapShotBufferSize = 256 * 1024;
		/// <summary>
		///  Fetch alliance stats
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static async Task AllianceStats(DateTimeOffset t0, DateTimeOffset t1, int continent, int minCities)
		{
			BlobContainerClient container = await  GetSnapshotContainer();
			var snaps = new List<Snapshot>();
			await foreach(var bi in container.GetBlobsAsync())
			{
				var t = bi.Name.ParseFileTime();
				if (t >= t0 && t <= t1)
					snaps.Add(await LoadSnapshot(bi.Name));
			}
			if(snaps.IsNullOrEmpty())
			{
				Log("No snapshots");
				return;
			}
			snaps.Sort((a, b) => a.time.seconds.CompareTo(b.time.seconds));
			var sb = new StringBuilder();
			var aids = new List<int>();
			foreach(var alli in snaps.Last().allianceStats )
			{
				var cont = alli.Value.perContinent.FirstOrDefault(a => a.continent == continent);
				if (cont != null && cont.cities >= minCities)
					aids.Add(alli.Value.aid);
			}
			sb.Append("Time");
			foreach (var aid in aids)
			{
				var an = Alliance.IdToName(aid);
				sb.Append($"\t{an}_military\t{an}_score\t{an}_cities");
			}
			sb.Append("\n");
			foreach (var snap in snaps)
			{
				sb.Append(snap.time.ToString(AUtil.fullDateFormat));
				foreach (var aid in aids)
				{
					bool present = false;
					if( snap.allianceStats.TryGetValue(aid, out var alli))
					{ 
						var cont = alli.perContinent.FirstOrDefault(a => a.continent == continent);
						if(cont!=null)
						{
							present = true;
							sb.Append($"\t{cont.military}\t{cont.score}\t{cont.cities}");
						}
					}
					if(!present)
					{
						sb.Append($"\t0\t0\t0");
					}

				}
				sb.Append("\n");
			}

			App.CopyTextToClipboard(sb.ToString());
			Note.Show("Copied stats to clipboard (tsv) for sheets");

		}

		public static async Task PlayerStats(DateTimeOffset t0, DateTimeOffset t1, int continent, int minTS, bool scoreAndCities,bool allianceStats, int maxPlayers, bool tsTotal, bool tsDef, bool tsOff)
		{
			BlobContainerClient container = await GetTSSnapshotContainer();
			var snaps = new List<TSSnapshot>();
			await foreach (var bi in container.GetBlobsAsync())
			{
				var t = bi.Name.ParseFileTime();
				if (t >= t0 && t <= t1)
					snaps.Add(await LoadTSSnapshot(bi.Name));
			}
			if (snaps.IsNullOrEmpty())
			{
				Log("No snapshots");
				return;
			}
			snaps.Sort((a, b) => a.time.seconds.CompareTo(b.time.seconds));
			var sb = new StringBuilder();
			var pids = new List<int>();
			{
				var cont = snaps.Last().continents.FirstOrDefault(a => a.continent == continent);
				if(cont ==null)
				{
					Log("No snapshots");
					return;
				}
				foreach(var p in cont.players)
				{
					if (pids.Count >= maxPlayers)
						break;
					if(p.tsTotal >= minTS)
					{
						pids.Add(p.pid);
					}
				}

			}
			sb.Append("Time");
			if(allianceStats)
				sb.Append("\tAllianceTs\tAllianceTsOff\tAllianceTsDef\tOtherTS\tOtherTsOff\tOtherTsDef");
			foreach (var pid in pids)
			{
				var an =Player.IdToName(pid);
				if(scoreAndCities)
					sb.Append($"\t{an}_score\t{an}_cities");
				if (tsTotal)
					sb.Append($"\t{an}_ts");
				if (tsOff)
					sb.Append($"\t{an}_tsOff");
				if (tsDef)
					sb.Append($"\t{an}_tsDef");
			}
			sb.Append("\n");
			foreach (var snap in snaps)
			{
				var cont = snap.continents.FirstOrDefault(a => a.continent == continent);
				if (cont == null)
					continue;
				sb.Append(snap.time.ToString(AUtil.fullDateFormat));


				//alliance counts
				var allianceTs = 0;
				var allianceTsOff = 0;
				var allianceTsDef = 0;
				var otherTs = 0;
				var otherTsOff = 0;
				var otherTsDef = 0;
				{
					foreach (var p in cont.players)
					{
						if (Alliance.IsAlly(Player.Get(p.pid).alliance))
						{
							allianceTs += p.tsTotal;
							allianceTsDef += p.tsDef;
							allianceTsOff += p.tsOff;
						}
						else
						{
							otherTs += p.tsTotal;
							otherTsDef += p.tsDef;
							otherTsOff += p.tsOff;

						}
					}
				}
				if (allianceStats)
				{
					sb.Append($"\t{allianceTs}\t{allianceTsOff}\t{allianceTsDef}\t{otherTs}\t{otherTsOff}\t{otherTsDef}");
				}
				foreach (var pid in pids)
				{
					var p = cont.players.FirstOrDefault(a => a.pid == pid);
					if (scoreAndCities)
					{
						if (p == null)
							sb.Append("\t0\t0");
						else
							sb.Append($"\t{p.score}\t{p.cities}");
					}
					if (tsTotal)
					{
						sb.Append($"\t{ (p != null ? p.tsTotal : 0)}");
					}
					if (tsOff)
					{
						sb.Append($"\t{ (p != null ? p.tsOff : 0)}");
					}
					if (tsDef)
					{
						sb.Append($"\t{ (p != null ? p.tsDef : 0)}");
					}
				}
				sb.Append("\n");
			 }
			
			

			App.CopyTextToClipboard(sb.ToString());
			Note.Show("Copied stats to clipboard (tsv) for sheets");

		}


		public static async Task<Snapshot> LoadSnapshot(string str)
		{
			try
			{
				BlobContainerClient container = await GetSnapshotContainer(); 
				
				var res = await container.GetBlobClient(str).DownloadAsync();
				//if (res.GetRawResponse().Status != 200)
				//{
				//	return day;
				//}

				using var deflate = new GZipStream(res.Value.Content, CompressionMode.Decompress);
				byte[] readBuffer = ArrayPool<byte>.Shared.Rent(snapShotBufferSize);
				var readOffset = 0;
				for (; ; )
				{

					var readSize = deflate.Read(readBuffer, readOffset, snapShotBufferSize - readOffset);
					if (readSize < snapShotBufferSize)
					{
						break;
					}
					readOffset += readSize;
					var _readBuffer = readBuffer;
					snapShotBufferSize *= 2;
					readBuffer = ArrayPool<byte>.Shared.Rent(snapShotBufferSize);
					for (int i = 0; i < readOffset; ++i)
						readBuffer[i] = _readBuffer[i];
					ArrayPool<byte>.Shared.Return(_readBuffer);
				}
				var snap = new Snapshot();
				unsafe
				{
					fixed (byte* pData = readBuffer)
					{

						var r = new Reader(pData);
						snap.time = r.ReadInt32();
						var playerCount = r.ReadInt32();
						snap.playerStats = new();
						for(int i =0;i<playerCount;++i)
						{
							var ps = new PlayerStats();
							
							ps.pid = r.ReadInt32();
							snap.playerStats.Add(ps.pid, ps);

							ps.reputation = r.ReadInt32();
							ps.offensiveRep = r.ReadInt32();
							ps.defensiveRep = r.ReadInt32();
							ps.unitKills = r.ReadInt32();
							ps.alliance = r.ReadInt32();
							ps.raiding = r.ReadInt32();
							var continentCount = r.ReadInt32();
							for(int pc=0;pc<continentCount;++pc)
							{
								var perC = new ContinentPlayerStats();
								perC.continent = r.ReadInt32();
								perC.cities = r.ReadInt32();
								perC.score = r.ReadInt32();
							}

						}
						var allianceCount = r.ReadInt32();
						for(int ia=0;ia<allianceCount;++ia)
						{
							var al = new AllianceStats();
							al.aid = r.ReadInt32();
							al.reputation = r.ReadInt32();
							snap.allianceStats.Add(al.aid,al);
							var perContinentCount = r.ReadInt32();
							for(int i=0;i<perContinentCount;++i)
							{
								ContinentAllianceStats cas = new();
								al.perContinent.Add(cas);
								cas.continent = r.ReadInt32();
								cas.cities = r.ReadInt32();
								cas.score = r.ReadInt32();
								cas.military = r.ReadInt32();

							}

						}
					}

				}
				ArrayPool<byte>.Shared.Return(readBuffer);
				return snap;
			}
			catch (Azure.RequestFailedException r)
			{
				Log(r.ErrorCode);
				Assert(r.Status == 404);
				return null;
			}
			catch (Exception e)
			{
				return null;
			}
			return null;
		}

		public static async Task<TSSnapshot> LoadTSSnapshot(string str)
		{
			try
			{
				BlobContainerClient container = await GetTSSnapshotContainer(); 
				
				var res = await container.GetBlobClient(str).DownloadAsync();
				//if (res.GetRawResponse().Status != 200)
				//{
				//	return day;
				//}

				using var deflate = new GZipStream(res.Value.Content, CompressionMode.Decompress);
				byte[] readBuffer = ArrayPool<byte>.Shared.Rent(snapShotBufferSize);
				var readOffset = 0;
				for (; ; )
				{

					var readSize = deflate.Read(readBuffer, readOffset, snapShotBufferSize - readOffset);
					if (readSize < snapShotBufferSize)
					{
						break;
					}
					readOffset += readSize;
					var _readBuffer = readBuffer;
					snapShotBufferSize *= 2;
					readBuffer = ArrayPool<byte>.Shared.Rent(snapShotBufferSize);
					for (int i = 0; i < readOffset; ++i)
						readBuffer[i] = _readBuffer[i];
					ArrayPool<byte>.Shared.Return(_readBuffer);
				}
				var snap = new TSSnapshot();
				unsafe
				{
					fixed (byte* pData = readBuffer)
					{
						var r = new Reader(pData);
						snap.time = r.ReadInt32();
						var contCount = r.ReadInt32();
						for (int i = 0; i < contCount; ++i)
						{
							var cont = new TSContinentStats();
							snap.continents.Add(cont);
							cont.continent = r.ReadInt32();
							int players = r.ReadInt32();
							for(int pID=0; pID < players;++pID)
							{
								var p = new TSContinentPlayerStats();
								cont.players.Add(p);
								p.pid = r.ReadInt32();
								p.score = r.ReadInt32();
								p.cities = r.ReadInt32();
								p.tsTotal = r.ReadInt32();
								p.tsOff = r.ReadInt32();
								p.tsDef = r.ReadInt32();

							}
						}
					}

				}
				ArrayPool<byte>.Shared.Return(readBuffer);
				return snap;
			}
			catch (Azure.RequestFailedException r)
			{
				Log(r.ErrorCode);
				Assert(r.Status == 404);
				return null;
			}
			catch (Exception e)
			{
				return null;
			}
			return null;
		}

		//for (; ; )
		//{
		//	if (client == null)
		//		client = new BlobContainerClient(connectionString, JSClient.world.ToString());
		//	return client;

		//}
		//public static async Task<bool> SaveDayChanges(DayChanges changes)
		//{
		//	BlobContainerClient container = await GetChangesContainer();

		//	COTG.Debug.Trace("Snapshot");
		//	// take a snapshot
		//	using (var mem = new MemoryStream())
		//	{
		//		using (var deflate = new DeflateStream(mem, CompressionLevel.Optimal, true))
		//		{
		//			using (var writer = new BinaryWriter(deflate, Encoding.UTF8, true))
		//			{
		//				changes.Save(writer);


		//			}
		//			try
		//			{
		//				mem.Flush();
		//				mem.Seek(0, SeekOrigin.Begin);
		//				var str = changes.dateStr;
		//				var success = await container.GetBlobClient(str).UploadAsync(str, new BlobUploadOptions() { Conditions = new BlobRequestConditions() { IfMatch = changes.eTag } });
		//				if (success.GetRawResponse().Status != 200)
		//				{
		//					return false;
		//				}

		//				changes.eTag = success.Value.ETag;

		//				return true;
		//			}
		//			catch (Exception ex)
		//			{
		//				Debug.Log(ex);
		//			}

		//		}

		//	}
		//	return false;




		//}

		static BlobContainerClient changesContainer;
		public static BlobClientOptions GetClientOptions()
		{
			var rv = new BlobClientOptions();

#if DEBUG
			rv.Diagnostics.IsLoggingEnabled = true;

#else
		rv.Diagnostics.IsLoggingEnabled = false;
#endif
			return rv;
		}
		public static async Task<BlobContainerClient> GetChangesContainer()
		{
			if (changesContainer == null)
			{
				changesContainer = new BlobContainerClient(connectionString, changesContainerName, GetClientOptions());
				await changesContainer.CreateIfNotExistsAsync();
			}
			return changesContainer;
		}
	}
}
