//using System;
using System.Threading.Tasks;
//using System.Collections.Generic;

//using COTG.DB;
using CnV.Game;
using System.Threading;
//using static COTG.Debug;
//using Microsoft.Azure.Cosmos;
//using System.Net;
//using Microsoft.Azure.Cosmos.Linq;
//using Azure.Data.Tables;
//using System.Linq;
//using Azure;

namespace CnV.Services
{
//	public class ShareStringDB : ITableEntity
//	{
//		public ShareStringDB()
//		{
//		}

//		public ShareStringDB(string partitionKey, string rowKey,string s) 
//		{
//			this.PartitionKey = partitionKey;
//			this.RowKey = rowKey;
//			this.s = s;
		
//		}

//		public string PartitionKey { get; set; }
//		public string  RowKey { get; set; }
//		public DateTimeOffset? Timestamp { get; set; }
//		public ETag ETag { get; set; }
//		public string s { get; set; }

	
//	}

//	public class TableBuildQueue : ITableEntity
//	{
//		public TableBuildQueue()
//		{
//		}

//		public TableBuildQueue(string partitionKey, string rowKey, string s)
//		{
//			this.PartitionKey = partitionKey;
//			this.RowKey = rowKey;
//			this.s = s;
//		}

//		public string PartitionKey { get; set; }
//		public string RowKey { get; set; }
//		public DateTimeOffset? Timestamp { get; set; }
//		public ETag ETag { get; set; }
//		public string s { get; set; }
//	}

	public static class Cosmos
    {
//        // The Azure Cosmos DB endpoint for running this sample.
//        private static readonly string EndpointUri = "https://avatars.documents.azure.com:443/";
//		private static readonly string EndpointUriT = "https://avadata.table.cosmos.azure.com:443/";

//		private const string avatabConnect = "DefaultEndpointsProtocol=https;AccountName=avatab;AccountKey=zLeBJT4uQpdbEFgcQHvlGNVX8xJctrL/qvByyZd+ujPLEf55awn9mGMVCW7UIHSJCnQ3xe3gbch88+eTjAqZng==;EndpointSuffix=core.windows.net";

//		// The primary key for the Azure Cosmos account.
//		private static readonly string PrimaryKey = "58VB6zTdjvySN7UmmxjalK3dltKdvArFwAOsZ7b2aqQBtPFM0AEwPlUxMnovFVpObUou3QNIEIGjYgI42pNZYQ==";
//		private static readonly string PrimaryKeyT = "NCTG9rOtJjJbiPNiRSPup3hRCNea4igaaYQRKWZN3v9u4qP2bk1DNLFA5WXM6Ct05tCaaZWw8wqdeFlj4gUVpw==";
//		private static readonly string connectionStringT = @"DefaultEndpointsProtocol=https;AccountName=avadata;AccountKey=NCTG9rOtJjJbiPNiRSPup3hRCNea4igaaYQRKWZN3v9u4qP2bk1DNLFA5WXM6Ct05tCaaZWw8wqdeFlj4gUVpw==;TableEndpoint=https://avadata.table.cosmos.azure.com:443/;";

//		// The Cosmos client instance
//		private static CosmosClient cosmosClient;
//		private static CosmosClient cosmosClientT; // tables

//		// The database we will create
//		private static Database database;

//        // The container we will create.
// //       private static Container container;
//     //   private static Container ordersContainer;
//		private static Container presenceContainer;
//		//		private static Container sessionContainer;
//	//	static CosmosClientOptions clientOptions = new() { ConnectionMode = ConnectionMode.Direct ,LimitToEndpoint=true,EnableContentResponseOnWrite=false };

//		static string worldPostfix => CnVServer.world == 21 ? "": ('_' + CnVServer.world.ToString());

//		// The name of the database and container we will create
//		// We are currently putting them all into w21 to save on databases
//		private static string databaseId => $"w21";
//        private static string containerId => $"i{111+(Alliance.myId==131||Alliance.myId==132 ? 22 : Alliance.myId) }{worldPostfix}";
//		private static string presenceContainerId => $"p282{worldPostfix}";
//	//	private static string sessionContainerId => $"s{151 + (Alliance.myId == 131 || Alliance.myId == 132 ? 131 : Alliance.myId) }";
//		private static string ordersContainerId => $"seenOrders{worldPostfix}";
////        private static string blobContainerId => $"c{CnVServer.world}";
// //       private static string blobName => $"b{311 + Alliance.myId}22";
//     	const int concurrentRequestCount = 1;
//        private static SemaphoreSlim throttle = new SemaphoreSlim(concurrentRequestCount);


//		//	static CloudStorageAccount storageAccount ;

//		// Create a table client for interacting with the table service
//		static TableClient tableClient;// = storageAccount.CreateCloudTableClient(new TableClientConfiguration());
//		static string tableName => "a";

//	// Create a table client for interacting with the table service 


//		private static SemaphoreSlim throttleT = new SemaphoreSlim(concurrentRequestCount);

//		static async Task<Container> GetContainer( string id, string parition = "/id")
//		{
//			var props = new ContainerProperties(id, parition);
//			props.IndexingPolicy = new IndexingPolicy()
//			{
//				Automatic = false,
			
//				IndexingMode= IndexingMode.None,
				
				


//			};
		
//			var c = await database.CreateContainerIfNotExistsAsync(props);
//			return c.Container;
//		}
//		static async ValueTask<bool> TouchT()
//		{
//			// 
//			if (tableClient != null)
//				return true;


//			await throttleT.WaitAsync();
//			try
//			{
//				if (tableClient != null)
//					return true;

//				tableClient = new TableClient(connectionStringT, tableName);
//				return tableClient != null;
//			}
//			catch (Exception e)
//			{
//				Log(e);
//				return false;
//			}
//			finally
//			{
//				throttleT.Release();
//			}

//			//            await ScaleContainerAsync();
//			//	await AddItemsToContainerAsync();
//		}





//		#region BuildQ

//		//static bool buildQueueExists;
//		//static string buildQueuePartition => $"{CnVServer.world}_{Player.myName}";
//		//const string buildQueueKey = "buildq";
//		//public static async Task<string> LoadBuildQueue()
//		//{

//		//	if (!await TouchT())
//		//		return String.Empty;

//		//	var part = buildQueuePartition;
//		//	var key = buildQueueKey;

//		//	await throttleT.WaitAsync();
//		//	try
//		//	{
//		//		var r = await tableClient.GetEntityAsync<TableBuildQueue>(part, key);
//		//		if (r != null)
//		//		{
//		//			buildQueueExists = true;
//		//			return r.Value.s;
//		//		}
//		//	}
//		//	catch (Azure.RequestFailedException e)
//		//	{
//		//		if( e.Status == 404)
//		//		{
//		//			// not found, don't log it, this is common
//		//		}
//		//		else
//		//		{
//		//			Log(e);

//		//		}

//		//	}
//		//	catch (Exception e)
//		//	{
//		//		Log(e);
//		//	}
//		//	finally
//		//	{
//		//		throttleT.Release();
//		//	}
//		//	return string.Empty;
//		//}


//		//public static async void SaveBuildQueue(string data)
//		//{

//		//	//if (!await TouchT())
//		//	//	return;

//		//	//var part = buildQueuePartition;
//		//	//var key = buildQueueKey;

//		//	//await throttleT.WaitAsync();
//		//	//try
//		//	//{

//		//	//	var i = new TableBuildQueue(part, key,data);
//		//	//	var r = await tableClient.UpsertEntityAsync(i, TableUpdateMode.Replace);
//		//	//	buildQueueExists = true;
//		//	//}
//		//	//catch (Exception e)
//		//	//{
//		//	//	Log(e);
//		//	//}
//		//	//finally
//		//	//{
//		//	//	throttleT.Release();
//		//	//}
//		//}
//		//public static async void ClearBuildQueue()
//		//{
//		//	//if (!buildQueueExists)
//		//	//	return;
//		//	//if (!await TouchT())
//		//	//	return;
//		//	//var part = buildQueuePartition;
//		//	//var key = buildQueueKey;

//		//	//await throttleT.WaitAsync();
//		//	//try
//		//	//{
//		//	//	var r = await tableClient.DeleteEntityAsync(part, key);
//		//	//	buildQueueExists = false;
//		//	//}
//		//	//catch (Exception e)
//		//	//{
//		//	//	Log(e);  // ignore if it does not exist
//		//	//}
//		//	//finally
//		//	//{
//		//	//	throttleT.Release();
//		//	//}
//		//}
//		#endregion

//	//	static async ValueTask<bool> Touch()
//	//	{
//	//		Assert(CnVServer.world != 0);
//	//		if (cosmosClient != null)
//	//			return database != null;
		
//	//		while(Alliance.diplomacyFetched==false)
//	//		{
//	//			await Task.Delay(300);
//	//		}

//	//		await throttle.WaitAsync();
//	//		try
//	//		{
//	//			if (cosmosClient != null)
//	//				return true;


//	//			// Create a new instance of the Cosmos Client
//	//			//	clientOptions.Diagnostics.IsDistributedTracingEnabled = false;
//	//			//		clientOptions.Diagnostics.IsLoggingContentEnabled = false;
//	//			//		clientOptions.Diagnostics.IsTelemetryEnabled = false;
//	//			//		clientOptions.Diagnostics.IsLoggingEnabled = false;

//	//				var _cosmosClient = new CosmosClient(EndpointUri, PrimaryKey);
//	//				database = _cosmosClient.GetDatabase(databaseId);
//	//				if (database != null)
//	//				{
//	//				//	container = await GetContainer(containerId);
//	//				//	ordersContainer = await GetContainer(ordersContainerId);
//	////					sessionContainer = await GetContainer(sessionContainerId);
//	//					presenceContainer = await GetContainer(presenceContainerId, "/p");
//	//				}
//	//				// write back 
//	//				cosmosClient = _cosmosClient;
//	//				return database != null;
//	//		}
//	//		catch(Exception e)
//	//		{
//	//			Log(e);
//	//			return false;
//	//		}
//	//		finally
//	//		{
//	//			throttle.Release();
//	//		}

// //           //            await ScaleContainerAsync();
// //           //	await AddItemsToContainerAsync();
// //       }
		
//		public static Task PublishPlayerInfo(int pid,int cid, string token, string cookie)
//		{
//			return Task.CompletedTask;
//			//// don't send for subs
//			//if (CnVServer.ppss != 0)
//			//	return;
//			//if (!await Touch())
//			//	return;

//			//var lastSeen = ServerTime.Now();
//			//await throttle.WaitAsync();
//			//try
//			//{
//			//	var pp = new PlayerPresenceDB() { id = pid.ToString(),cid=cid,t=lastSeen, ck = cookie, tk = token };
//			//	await presenceContainer.UpsertItemAsync<PlayerPresenceDB>(pp, new PartitionKey(false), itemRequesDefault);
			
//			//}
//			//catch(Exception ex)
//			//{
//			//	Log(ex);
//			//}
//			//finally
//			//{
//			//	throttle.Release();
//			//}
//		}

//		public static async Task< List<PlayerPresenceDB> > GetPlayersInfo()
//		{
	
//			List<PlayerPresenceDB> rv = new List<PlayerPresenceDB>();
//			return rv;
///*
//			if (!await Touch())
//				return rv;
//			await throttle.WaitAsync();
//			try
//			{
//				using (FeedIterator<PlayerPresenceDB> feedIterator = presenceContainer.GetItemLinqQueryable<PlayerPresenceDB>().ToFeedIterator())

//				{
//					while (feedIterator.HasMoreResults)
//					{
//						foreach (var item in await feedIterator.ReadNextAsync())
//						{
//							rv.Add(item);
//						}
//					}
//				}
//				return rv;
//			}
//			catch(Exception ex)
//			{
//				Log(ex);
//				return rv;
//			}
//			finally
//			{
//				throttle.Release();
//			}*/

//		}

	
//		public class Order
//        {
//            public string id { get; set; }
//        }

//        public static int battleRecordsUpserted;
////        static ItemRequestOptions itemRequestOptions = new ItemRequestOptions() { ConsistencyLevel=ConsistencyLevel.Eventual, EnableContentResponseOnWrite =false}
//   //     public static async Task AddBattleRecord(Army army)
//   //     {
//			//if (!await Touch())
//			//	return;


//   //         await throttle.WaitAsync();

//   //         try
//   //         {
//   //             //   semaphore.EnterWriteLock();
//   //             COTG.DB.SpotDB s0, s1;
//   //             // Create a family object for the Andersen family
//   //             var sourceId = COTG.DB.SpotDB.CoordsToString(army.sourceCid.CidToWorld());
//   //             var targetId = COTG.DB.SpotDB.CoordsToString(army.targetCid.CidToWorld());
//   //             var targetMine = Alliance.IsMine(army.targetAlliance);
//   //             var sourceMine = Alliance.IsMine(army.sourceAlliance);
//   //             if (!sourceMine && army.isAttack)
//   //             {
//   //                 try
//   //                 {
//   //                     // Read the item to see if it exists.  
//   //                     ItemResponse<COTG.DB.SpotDB> source = await container.ReadItemAsync<COTG.DB.SpotDB>(sourceId, new PartitionKey(sourceId), itemRequesDefault);
//   //                     s0 = source.Resource;
//   //                 }
//   //                 catch (CosmosException ex)
//   //                 {
//   //                     if (ex.StatusCode != HttpStatusCode.NotFound)
//   //                         return;
//   //                     s0 = new COTG.DB.SpotDB() { id = sourceId, own = army.sPid }; // todo:  set owner
//   //                                                                                 // Create an item in the container representing the Andersen family. Note we provide the value of the partition key for this item, which is "Andersen"
//   //                                                                                 //         ItemResponse<COTG.DB.Spot> source = await container.CreateItemAsync<COTG.DB.Spot>(s0, new PartitionKey(sourceId));

//   //                 }
//   //                 var recb = new RecordBattle() { rep = army.reportId, typ = army.type, trp = army.troops };
//   //                 recb.SetTime(army.time);
//   //                 if (s0.AddRecord(recb))
//   //                 {
//   //                     try
//   //                     {
//   //                         await container.UpsertItemAsync<COTG.DB.SpotDB>(s0, new PartitionKey(sourceId), itemRequesDefault);

//   //                     }
//   //                     catch (Exception ex)
//   //                     {
//   //                         Log(ex);
//   //                     }

//   //                     ++battleRecordsUpserted;
//   //                 }
//   //             }
//   //             if (!targetMine && army.isAttack && army.sumDef.TS() > 0) // defense report
//   //             {
//   //                 try
//   //                 {
//   //                     // Read the item to see if it exists.  
//   //                     ItemResponse<COTG.DB.SpotDB> source = await container.ReadItemAsync<COTG.DB.SpotDB>(targetId, new PartitionKey(targetId), itemRequesDefault);
//   //                     s1 = source.Resource;
//   //                 }
//   //                 catch (CosmosException ex)
//   //                 {

//   //                     if (ex.StatusCode != HttpStatusCode.NotFound)
//   //                         return;
//   //                     //      if(ex.Status == (int)HttpStatusCode.NotFound)
//   //                     s1 = new COTG.DB.SpotDB() { id = targetId, own = army.tPid }; // todo:  set owner

//   //                 }
//   //                 var recb = new RecordBattle() { rep = army.reportId, typ = Game.Enum.reportDefenseStationed, trp = army.sumDef };
//   //                 recb.SetTime(army.time);
//   //                 if (s1.AddRecord(recb))
//   //                 {
//   //                     try
//   //                     {
//   //                         await container.UpsertItemAsync<COTG.DB.SpotDB>(s1, new PartitionKey(targetId), itemRequesDefault);

//   //                     }
//   //                     catch (Exception ex)
//   //                     {
//   //                         Log(ex);

//   //                     }

//   //                     ++battleRecordsUpserted;

//   //                 }
//   //             }
//   //             //	recb = new RecordBattle[]
//   //             //	{
//   //             //		new RecordBattle() { t = DateTime.UtcNow.Ticks/10000, typ = 1, rep = "1", trp = new TroopTypeCount[] { new TroopTypeCount() { t= 2, c = 4 } } },
//   //             //	},

//   //             //var id1 = Spot.CoordsToString((221, 220));
//   //             //// Create a family object for the Wakefield family
//   //             //Spot wakefieldFamily = new Spot
//   //             //{
//   //             //	id = id1,
//   //             //	typ = Spot.typeCity,
//   //             //	recb = new RecordBattle[]
//   //             //	{
//   //             //		new RecordBattle() { t = DateTime.UtcNow.Ticks/10000, typ = 1, rep = "1", trp = new TroopTypeCount[] { new TroopTypeCount() { t= 2, c = 4 } } },
//   //             //	},
//   //             //};
//   //         }
//   //         catch (Exception e)
//   //         {
//   //             Log(e);
//   //         }
//   //         finally
//   //         {
//   //             throttle.Release();


//   //         }
//   //         //finally
//   //         //{
//   //         //   semaphore.ExitWriteLock();
//   //         //}
//   //         //try
//   //         //{
//   //         //	// Read the item to see if it exists
//   //         //	ItemResponse<Spot> wakefieldFamilyResponse = await container.ReadItemAsync<Spot>(wakefieldFamily.id, new PartitionKey(wakefieldFamily.id));
//   //         //	Console.WriteLine("Item in database with id: {0} already exists\n", wakefieldFamilyResponse.Value.id);
//   //         //}
//   //         //catch (CosmosException ex) when (ex.Status == (int)HttpStatusCode.NotFound)
//   //         //{
//   //         //	// Create an item in the container representing the Wakefield family. Note we provide the value of the partition key for this item, which is "Wakefield"
//   //         //	ItemResponse<Spot> wakefieldFamilyResponse = await container.CreateItemAsync<Spot>(wakefieldFamily, new PartitionKey(id1));

//   //         //	// Note that after creating the item, we can access the body of the item with the Value property off the ItemResponse. We can also access the RequestCharge property to see the amount of RUs consumed on this request.
//   //         //	Console.WriteLine("Created item in database with id: {0} Operation consumed {1} RUs.\n", wakefieldFamilyResponse.Value.id, 0);
//   //         //}
//   //     }
//        //// </AddItemsToContainerAsync>

//        //// <QueryItemsAsync>
//        ///// <summary>
//        ///// Run a query (using Azure Cosmos DB SQL syntax) against the container
//        ///// Including the partition key value of lastName in the WHERE filter results in a more efficient query
//        ///// </summary>
//        ////private async Task QueryItemsAsync()
//        ////{
//        ////    var sqlQueryText = "SELECT * FROM c WHERE c.LastName = 'Andersen'";

//        ////    Console.WriteLine("Running query: {0}\n", sqlQueryText);

//        ////    QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
//        ////    FeedIterator<Spot> queryResultSetIterator = container.GetItemQueryIterator<Spot>(queryDefinition);

//        ////    List<Spot> families = new List<Spot>();

//        ////    while (queryResultSetIterator.HasMoreResults)
//        ////    {
//        ////        FeedResponse<Spot> currentResultSet = await queryResultSetIterator.ReadNextAsync();
//        ////        foreach (Spot family in currentResultSet)
//        ////        {
//        ////            families.Add(family);
//        ////            Console.WriteLine("\tRead {0}\n", family);
//        ////        }
//        ////    }
//        ////}
//        //// </QueryItemsAsync>

//        //// <ReplaceFamilyItemAsync>
//        ///// <summary>
//        ///// Replace an item in the container
//        ///// </summary>
//        //private static async Task ReplaceFamilyItemAsync()
//        //{
//        //	ItemResponse<Spot> wakefieldFamilyResponse = await container.ReadItemAsync<Spot>(id0, new PartitionKey(id0));
//        //	var itemBody = wakefieldFamilyResponse.Value;

//        //	// update registration status from false to true
//        //	itemBody.recn = new[] { new RecordNote() { src = 1, n = "claimed" } };
//        //	// update grade of child
//        //	itemBody.own = 2;

//        //	// replace the item with the updated content
//        //	wakefieldFamilyResponse = await container.ReplaceItemAsync<Spot>(itemBody, id0, new PartitionKey(id0));
//        //	Console.WriteLine("Updated Spot [{0},{1}].\n \tBody is now: {2}\n", itemBody.own, itemBody.id, wakefieldFamilyResponse.Value);
//        //}
//        //// </ReplaceFamilyItemAsync>

//        //// <DeleteFamilyItemAsync>
//        ///// <summary>
//        ///// Delete an item in the container
//        ///// </summary>
//        //private static async Task DeleteFamilyItemAsync()
//        //{
//        //	var partitionKeyValue = "Wakefield";
//        //	var familyId = "Wakefield.7";

//        //	// Delete an item. Note we must provide the partition key value and id of the item to delete
//        //	ItemResponse<Spot> wakefieldFamilyResponse = await container.DeleteItemAsync<Spot>(familyId, new PartitionKey(partitionKeyValue));
//        //	Console.WriteLine("Deleted Spot [{0},{1}]\n", partitionKeyValue, familyId);
//        //}
//        // </DeleteFamilyItemAsync>

//        // <DeleteDatabaseAndCleanupAsync>
//        /// <summary>
//        /// Delete the database and dispose of the Cosmos Client instance
//        /// </summary>
//        //      private static async Task DeleteDatabaseAndCleanupAsync()
//        //{
//        //	DatabaseResponse databaseResourceResponse = await database.DeleteAsync();
//        //	// Also valid: await cosmosClient.Databases["FamilyDatabase"].DeleteAsync();

//        //	Console.WriteLine("Deleted CosmosDatabase: {0}\n", databaseId);

//        //	//Dispose of CosmosClient
//        //	cosmosClient.Dispose();
//        //}
//        // </DeleteDatabaseAndCleanupAsync>
    }
}


