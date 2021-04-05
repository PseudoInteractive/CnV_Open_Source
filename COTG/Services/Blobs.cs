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

namespace COTG.Services
{
	class Blobs
	{
		const string connectionString = "DefaultEndpointsProtocol=https;AccountName=avata;AccountKey=IWRPGlttorpK5DcHWin/GdA2VEcZKnHkr30lE0ZDvKLG0q1CjZONcAQYI2D26DENd7TIAxF8tPsE0mIk98BafA==;EndpointSuffix=core.windows.net";
		public static BlobContainerClient client;

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
		static string containerName =>  $"s{JSClient.world}";

		const int timeBetweenSnapshots = 6*60;
		const int minTimeBetweenSnapshots = timeBetweenSnapshots - 30;
		const int maxTimeBetweenSnapshots = timeBetweenSnapshots + 30;
		//const int timeBetweenSnapshots = 5;
		//const int minTimeBetweenSnapshots = timeBetweenSnapshots - 1;
		//const int maxTimeBetweenSnapshots = timeBetweenSnapshots + 1;
		public static async void ProcessStats()
		{


			BlobContainerClient container = new BlobContainerClient(connectionString, containerName);
			await container.CreateIfNotExistsAsync();

			for (; ;)
			{

				var info = await container.GetPropertiesAsync();
				var lastWritten = info.Value.LastModified;// + TimeSpan.FromHours(12) + TimeSpan.FromMinutes(AMath.random.Next(60)-30);
				var currentT = DateTimeOffset.UtcNow;
				var dt = currentT - lastWritten;
				if (dt.TotalMinutes > minTimeBetweenSnapshots)
				{
					COTG.Debug.Trace("Snapshot");
					// take a snapshot
					var snap = await Snapshot.GetStats();
					snap.dateTime = currentT;
					using(var mem = new MemoryStream())
					{
						using (var deflate = new DeflateStream(mem, CompressionLevel.Optimal,true))
						{
							using (var writer = new BinaryWriter(deflate,Encoding.UTF8,true))
							{
								writer.Write(currentT.Ticks);
								writer.Write(snap.playerStats.Count);
								foreach(var pss in snap.playerStats)
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
									foreach(var c in ps.perContinent)
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
									foreach (var c in ps.perContinent)
									{
										writer.Write(c.continent);
										writer.Write(c.cities);
										writer.Write(c.score);
										writer.Write(c.military);
									}
								}

							}
							try 
							{
								mem.Seek(0, SeekOrigin.Begin);
								await container.UploadBlobAsync(currentT.ToString("o", System.Globalization.CultureInfo.InvariantCulture.DateTimeFormat), mem);
							
							}
							catch(Exception ex)
							{
								Debug.Log(ex);
							}
							
						}
						
					}


					lastWritten = currentT;
				}

				var nextSnapShot = lastWritten + TimeSpan.FromMinutes(AMath.random.Next(minTimeBetweenSnapshots, maxTimeBetweenSnapshots));


				await Task.Delay( (nextSnapShot - currentT) );



			}



		}

		

		//for (; ; )
		//{
		//	if (client == null)
		//		client = new BlobContainerClient(connectionString, JSClient.world.ToString());
		//	return client;

		//}

	}
}
