using System;
using System.Threading.Tasks;
using System.Configuration;
using System.Collections.Generic;
using Azure.Cosmos;
using COTG.DB;
using COTG.Game;
using Windows.Web.Http;
using System.Threading;
using static COTG.Debug;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using System.Text.Json;

namespace COTG.Services
{

	public static class Cosmos
	{
		// The Azure Cosmos DB endpoint for running this sample.
		private static readonly string EndpointUri = "https://avatars.documents.azure.com:443/";

		// The primary key for the Azure Cosmos account.
		private static readonly string PrimaryKey = "58VB6zTdjvySN7UmmxjalK3dltKdvArFwAOsZ7b2aqQBtPFM0AEwPlUxMnovFVpObUou3QNIEIGjYgI42pNZYQ==";

		// The Cosmos client instance
		private static CosmosClient cosmosClient;

		// The database we will create
		private static CosmosDatabase database;

		// The container we will create.
		private static CosmosContainer container;

		// The name of the database and container we will create
		private static string databaseId => $"w{JSClient.world}";
		private static string containerId => $"i{111+Alliance.myId}";
        private static string blobContainerId => $"c{JSClient.world}";
        private static string blobName => $"b{311 + Alliance.myId}22";
        private static SemaphoreSlim semaphore;

        static Cosmos()
		{
            Assert(JSClient.world != 0);
			// Create a new instance of the Cosmos Client
			cosmosClient = new CosmosClient(EndpointUri, PrimaryKey, new CosmosClientOptions() { });
			database = cosmosClient.GetDatabase(databaseId);
            if (database != null)
            {
                container = database.GetContainer(containerId);
            }
            semaphore = new SemaphoreSlim(1, 1);
            //            await ScaleContainerAsync();
            //	await AddItemsToContainerAsync();
            //		await ReplaceFamilyItemAsync();
        }
        // </GetStartedDemoAsync>

        // <CreateDatabaseAsync>
        /// <summary>
        /// Create the database if it does not exist
        /// </summary>
        //private static async Task CreateDatabaseAsync()
        //{
        //	// Create a new database
        //	database = cosmosClient.GetDatabase(databaseId);
        //	container = database.GetContainer(containerId);
        //	Console.WriteLine("Created CosmosDatabase: {0}\n", database.Id);
        //	Console.WriteLine("Created CosmosContainer: {0}\n", container.Id);
        //}
        // </CreateDatabaseAsync>

        // <CreateContainerAsync>
        /// <summary>
        /// Create the container if it does not exist. 
        /// Specifiy "/LastName" as the partition key since we're storing family information, to ensure good distribution of requests and storage.
        /// </summary>
        /// <returns></returns>
        // </CreateContainerAsync>

        // <ScaleContainerAsync>
        /// <summary>
        /// Scale the throughput provisioned on an existing CosmosContainer.
        /// You can scale the throughput (RU/s) of your container up and down to meet the needs of the workload. Learn more: https://aka.ms/cosmos-request-units
        /// </summary>
        /// <returns></returns>
        //private static async Task ScaleContainerAsync()
        //{
        //	// Read the current throughput
        //	int? throughput = await container.ReadThroughputAsync();
        //	if (throughput.HasValue)
        //	{
        //		Console.WriteLine("Current provisioned throughput : {0}\n", throughput.Value);
        //		int newThroughput = throughput.Value + 100;
        //		// Update throughput
        //		await container.ReplaceThroughputAsync(newThroughput);
        //		Console.WriteLine("New provisioned throughput : {0}\n", newThroughput);
        //	}

        //}
        // </ScaleContainerAsync>
        public static Dictionary<int, COTG.DB.Spot> spots = new Dictionary<int,COTG.DB.Spot>();
       
            async static public Task GetSpotDB()
            {
                HttpClient client = null;
                try
                {
                    for (; ; )
                    {
                        if (JSClient.clientPool.TryTake(out client))
                            break;
                        await Task.Delay(128);
                    }
                var blobName = $"https://avag.blob.core.windows.net/c{JSClient.world}/b{311 + Alliance.myId}22";
                    var buff = await client.GetBufferAsync(new Uri(blobName));
                    JSClient.clientPool.Add(client);
                    client = null;
                    if (buff != null)
                    {
                        var temp = new byte[buff.Length];

                        using (var dataReader = Windows.Storage.Streams.DataReader.FromBuffer(buff))
                        {
                            dataReader.ReadBytes(temp);
                        }
                    // Log("Hello!");
                  //  var str = new Dictionary<int, COTG.DB.Spot>();

                    var str = JsonSerializer.Deserialize<Dictionary<string, COTG.DB.Spot>>(temp);
                    // Log("Helllo!");
                    spots = new Dictionary<int, DB.Spot>(str.Count);
                    foreach(var s in str)
                    {
                        var x = int.Parse(s.Key.Substring(0, 3));
                        var y = int.Parse(s.Key.Substring(3, 3));
                        spots[(x, y).WorldToCid()] = s.Value; 
                    }
                }
                    else
                    {
                        Log("Error!");
                    };
                }
                catch (Exception e)
                {
                    if (client != null)
                        JSClient.clientPool.Add(client);
                    client = null;
                    Log(e);
                }


            }        //static public string id0 = Spot.CoordsToString((220, 220));
            public static async Task SummarizeNotes()
        {
            if(!await semaphore.WaitAsync(30 * 1000) )
                return;
            var blobData = new Dictionary<string, COTG.DB.Spot>();
            try
            {

                await foreach (var spot in container.GetItemQueryIterator<COTG.DB.Spot>(
                    queryDefinition: null,
                    requestOptions: new QueryRequestOptions()
                    {

                    }))
                {
                    var s = string.Empty;
                    int counter = 0;
                    //foreach (var rec in spot.recb)
                    //{
                    //    if (counter != 0)
                    //        s = s+ '\n';
                    //    s = $"{s}{ SmallTime.ToDateTime(rec.t).FormatDefault() }:{ Game.Enum.reportStrings[rec.typ]}{rec.trp.Format(":",' ',',')}";
                    //    if (++counter >= 4)
                    //        break;
                    //}
                    blobData[spot.id] = spot;
                }
            } catch (Exception e)
            {
                Log(e);
            } finally
            {
                semaphore.Release();
            }
            var blobServiceClient = new BlobServiceClient("DefaultEndpointsProtocol=https;AccountName=avag;AccountKey=G545SQSDGDM6LSu3eanZ6wSbsiz2rt7/jrusjll4Hh7yS9rJaQTX7CSOLLdN2C7dX+Z+PCOWyXrDgGZX5YT1dw==;EndpointSuffix=core.windows.net");
            var containerClient = blobServiceClient.GetBlobContainerClient(blobContainerId);
            var blobClient = containerClient.GetBlockBlobClient(blobName);
            var temp = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(blobData);
            using (var ms = new System.IO.MemoryStream(temp))
            {
                await blobClient.UploadAsync(ms);
            }
        }
        public static int battleRecordsUpserted;

            public static async Task AddBattleRecord(Army army)
		{
            if (container == null || database == null)
                return;

            var success = await semaphore.WaitAsync(30 * 1000);
            if (!success)
                return;

            try
            {
                COTG.DB.Spot s0, s1;
                // Create a family object for the Andersen family
                var sourceId = COTG.DB.Spot.CoordsToString(army.sourceCid.CidToWorld());
                var targetId = COTG.DB.Spot.CoordsToString(army.targetCid.CidToWorld());
                var targetMine = Alliance.IsMine(army.targetAlliance);
                var sourceMine = Alliance.IsMine(army.sourceAlliance);
                if (!sourceMine && army.isAttack)
                {
                    try
                    {
                        // Read the item to see if it exists.  
                        ItemResponse<COTG.DB.Spot> source = await container.ReadItemAsync<COTG.DB.Spot>(sourceId, new PartitionKey(sourceId));
                        s0 = source.Value;
                    }
                    catch (CosmosException ex)
                    {
                        if (ex.Status != (int)HttpStatusCode.NotFound)
                            Log(ex);
                        s0 = new COTG.DB.Spot() { id = sourceId, own = army.sPid }; // todo:  set owner
                                                                              // Create an item in the container representing the Andersen family. Note we provide the value of the partition key for this item, which is "Andersen"
                                                                              //         ItemResponse<COTG.DB.Spot> source = await container.CreateItemAsync<COTG.DB.Spot>(s0, new PartitionKey(sourceId));

                    }
                    var recb = new RecordBattle() { rep = army.reportId, typ = army.type, trp = army.troops };
                    recb.SetTime(army.time);
                    if (s0.AddRecord(recb))
                    {
                        await container.UpsertItemAsync<COTG.DB.Spot>(s0, new PartitionKey(sourceId));
                        ++battleRecordsUpserted;
                    }
                }
                if (!targetMine && army.isAttack && army.sumDef.TS() > 0) // defense report
                {
                    try
                    {
                        // Read the item to see if it exists.  
                        ItemResponse<COTG.DB.Spot> source = await container.ReadItemAsync<COTG.DB.Spot>(targetId, new PartitionKey(targetId));
                        s1 = source.Value;
                    }
                    catch (CosmosException ex)
                    {
                        if (ex.Status != (int)HttpStatusCode.NotFound)
                           Log(ex);
                        //      if(ex.Status == (int)HttpStatusCode.NotFound)
                        s1 = new COTG.DB.Spot() { id = targetId, own = army.tPid }; // todo:  set owner

                    }
                    var recb = new RecordBattle() { rep = army.reportId, typ = Game.Enum.reportDefenseStationed, trp = army.sumDef };
                    recb.SetTime(army.time);
                    if (s1.AddRecord(recb))
                    {
                        await container.UpsertItemAsync<COTG.DB.Spot>(s1, new PartitionKey(targetId));
                        ++battleRecordsUpserted;

                    }
                }
                //	recb = new RecordBattle[]
                //	{
                //		new RecordBattle() { t = DateTime.UtcNow.Ticks/10000, typ = 1, rep = "1", trp = new TroopTypeCount[] { new TroopTypeCount() { t= 2, c = 4 } } },
                //	},

                //var id1 = Spot.CoordsToString((221, 220));
                //// Create a family object for the Wakefield family
                //Spot wakefieldFamily = new Spot
                //{
                //	id = id1,
                //	typ = Spot.typeCity,
                //	recb = new RecordBattle[]
                //	{
                //		new RecordBattle() { t = DateTime.UtcNow.Ticks/10000, typ = 1, rep = "1", trp = new TroopTypeCount[] { new TroopTypeCount() { t= 2, c = 4 } } },
                //	},
                //};
            }catch(Exception e)
            {
                Log(e);
            }
            finally
            {
               semaphore.Release();
            }
            //try
            //{
            //	// Read the item to see if it exists
            //	ItemResponse<Spot> wakefieldFamilyResponse = await container.ReadItemAsync<Spot>(wakefieldFamily.id, new PartitionKey(wakefieldFamily.id));
            //	Console.WriteLine("Item in database with id: {0} already exists\n", wakefieldFamilyResponse.Value.id);
            //}
            //catch (CosmosException ex) when (ex.Status == (int)HttpStatusCode.NotFound)
            //{
            //	// Create an item in the container representing the Wakefield family. Note we provide the value of the partition key for this item, which is "Wakefield"
            //	ItemResponse<Spot> wakefieldFamilyResponse = await container.CreateItemAsync<Spot>(wakefieldFamily, new PartitionKey(id1));

            //	// Note that after creating the item, we can access the body of the item with the Value property off the ItemResponse. We can also access the RequestCharge property to see the amount of RUs consumed on this request.
            //	Console.WriteLine("Created item in database with id: {0} Operation consumed {1} RUs.\n", wakefieldFamilyResponse.Value.id, 0);
            //}
        }
        //// </AddItemsToContainerAsync>

        //// <QueryItemsAsync>
        ///// <summary>
        ///// Run a query (using Azure Cosmos DB SQL syntax) against the container
        ///// Including the partition key value of lastName in the WHERE filter results in a more efficient query
        ///// </summary>
        ////private async Task QueryItemsAsync()
        ////{
        ////    var sqlQueryText = "SELECT * FROM c WHERE c.LastName = 'Andersen'";

        ////    Console.WriteLine("Running query: {0}\n", sqlQueryText);

        ////    QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
        ////    FeedIterator<Spot> queryResultSetIterator = container.GetItemQueryIterator<Spot>(queryDefinition);

        ////    List<Spot> families = new List<Spot>();

        ////    while (queryResultSetIterator.HasMoreResults)
        ////    {
        ////        FeedResponse<Spot> currentResultSet = await queryResultSetIterator.ReadNextAsync();
        ////        foreach (Spot family in currentResultSet)
        ////        {
        ////            families.Add(family);
        ////            Console.WriteLine("\tRead {0}\n", family);
        ////        }
        ////    }
        ////}
        //// </QueryItemsAsync>

        //// <ReplaceFamilyItemAsync>
        ///// <summary>
        ///// Replace an item in the container
        ///// </summary>
        //private static async Task ReplaceFamilyItemAsync()
        //{
        //	ItemResponse<Spot> wakefieldFamilyResponse = await container.ReadItemAsync<Spot>(id0, new PartitionKey(id0));
        //	var itemBody = wakefieldFamilyResponse.Value;

        //	// update registration status from false to true
        //	itemBody.recn = new[] { new RecordNote() { src = 1, n = "claimed" } };
        //	// update grade of child
        //	itemBody.own = 2;

        //	// replace the item with the updated content
        //	wakefieldFamilyResponse = await container.ReplaceItemAsync<Spot>(itemBody, id0, new PartitionKey(id0));
        //	Console.WriteLine("Updated Spot [{0},{1}].\n \tBody is now: {2}\n", itemBody.own, itemBody.id, wakefieldFamilyResponse.Value);
        //}
        //// </ReplaceFamilyItemAsync>

        //// <DeleteFamilyItemAsync>
        ///// <summary>
        ///// Delete an item in the container
        ///// </summary>
        //private static async Task DeleteFamilyItemAsync()
        //{
        //	var partitionKeyValue = "Wakefield";
        //	var familyId = "Wakefield.7";

        //	// Delete an item. Note we must provide the partition key value and id of the item to delete
        //	ItemResponse<Spot> wakefieldFamilyResponse = await container.DeleteItemAsync<Spot>(familyId, new PartitionKey(partitionKeyValue));
        //	Console.WriteLine("Deleted Spot [{0},{1}]\n", partitionKeyValue, familyId);
        //}
        // </DeleteFamilyItemAsync>

        // <DeleteDatabaseAndCleanupAsync>
        /// <summary>
        /// Delete the database and dispose of the Cosmos Client instance
        /// </summary>
  //      private static async Task DeleteDatabaseAndCleanupAsync()
		//{
		//	DatabaseResponse databaseResourceResponse = await database.DeleteAsync();
		//	// Also valid: await cosmosClient.Databases["FamilyDatabase"].DeleteAsync();

		//	Console.WriteLine("Deleted CosmosDatabase: {0}\n", databaseId);

		//	//Dispose of CosmosClient
		//	cosmosClient.Dispose();
		//}
		// </DeleteDatabaseAndCleanupAsync>
	}
}


