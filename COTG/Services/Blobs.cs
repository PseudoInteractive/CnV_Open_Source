using Azure.Storage.Blobs;

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

		const int timeBetweenSnapshots = 1 * 60;
		const int minTimeBetweenSnapshots = timeBetweenSnapshots - 15;
		const int maxTimeBetweenSnapshots = timeBetweenSnapshots + 15;
		//const int timeBetweenSnapshots = 5;
		//const int minTimeBetweenSnapshots = timeBetweenSnapshots - 1;
		//const int maxTimeBetweenSnapshots = timeBetweenSnapshots + 1;
		public static async void ProcessStats()
		{

			for (; ; )
			{

				BlobContainerClient container = new BlobContainerClient(connectionString, statsContainerName, GetClientOptions());
				await container.CreateIfNotExistsAsync();

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
				BlobContainerClient container = new BlobContainerClient(connectionString, statsTSContainerName, GetClientOptions());
				await container.CreateIfNotExistsAsync();

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
		public async Task<Snapshot> LoadSnapshot(string str)
		{
			try
			{
				BlobContainerClient container = new BlobContainerClient(connectionString, statsContainerName, GetClientOptions());
				await container.CreateIfNotExistsAsync();

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

		public async Task<TSSnapshot> LoadTSSnapshot(string str)
		{
			try
			{
				BlobContainerClient container = new BlobContainerClient(connectionString, statsTSContainerName, GetClientOptions());
				await container.CreateIfNotExistsAsync();

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
