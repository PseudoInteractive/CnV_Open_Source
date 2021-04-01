using Azure.Storage.Blobs;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COTG.Services
{
	class Blobs
	{
		const string connectionString = "DefaultEndpointsProtocol=https;AccountName=statsc;AccountKey=kRV2GyMSenMv0vk+TfEjquZG9VW6eFlC+bnz5icg8cOXWSq84HxxWIcwtgb4ND2x1i93pgO7DSeiZ22V09gwdg==;EndpointSuffix=core.windows.net";
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
		static Stat[] stats = new[] { new Stat("score", 0) };
		const string containerName = "c";

		public static BlobContainerClient Touch()
		{
			if(client == null)
				client = new BlobContainerClient(connectionString, JSClient.world.ToString());
			return client;
		}
		public static async void Process()
		{
			
		
				BlobContainerClient container = new BlobContainerClient(connectionString, containerName);
				var info = await container.CreateIfNotExistsAsync();
				var t = info.Value.LastModified + TimeSpan.FromHours(12) + TimeSpan.FromMinutes(AMath.random.Next(60)-30);
				var dt = t - DateTimeOffset.UtcNow;
				if(dt.TotalHours > 0)
				{
					await Task.Delay(dt);
				}

			foreach (var i in stats)
			{

				var _i = i;
				var info2 = await container.GetPropertiesAsync();
				if (info2.Value.LastModified + TimeSpan.FromHours(11.5f) > DateTimeOffset.UtcNow)
					continue;
				var js = await Post.SendForJson("includes/gR.php", $"a={_i.id}&b=56");


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
