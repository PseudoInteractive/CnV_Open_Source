using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Azure;
using Azure.Data.Tables;
using Azure.Data.Tables.Models;

using static COTG.Debug;


namespace COTG.Services
{
	public class ShareStringDB : ITableEntity
	{
		public ShareStringDB()
		{
		}

		public ShareStringDB(string partitionKey, string rowKey, string s)
		{
			this.PartitionKey = partitionKey;
			this.RowKey = rowKey;
			this.s = s;

		}

		public string PartitionKey { get; set; }
		public string RowKey { get; set; }
		public DateTimeOffset? Timestamp { get; set; }
		public ETag ETag { get; set; }
		public string s { get; set; }


	}

	class Tables
	{
			const string accountName = "avata";
		// DefaultEndpointsProtocol=https;AccountName=avata;AccountKey=IWRPGlttorpK5DcHWin/GdA2VEcZKnHkr30lE0ZDvKLG0q1CjZONcAQYI2D26DENd7TIAxF8tPsE0mIk98BafA==;EndpointSuffix=core.windows.net
		static TableServiceClient serviceClient;
		static TableClient tableClient;
		static bool TouchT()
		{
			if (tableClient != null)
				return true;

			string storageUri = $"https://{accountName}.table.core.windows.net/";
			string storageAccountKey = "IWRPGlttorpK5DcHWin/GdA2VEcZKnHkr30lE0ZDvKLG0q1CjZONcAQYI2D26DENd7TIAxF8tPsE0mIk98BafA==";
			string tableName = "sharestring";

			serviceClient = new TableServiceClient(
				new Uri(storageUri),
				new TableSharedKeyCredential(accountName, storageAccountKey));

			tableClient = serviceClient.GetTableClient(tableName);
			return true;
		}


		public static async void ShareShareString(string part, string key, string s)
		{
			if (!TouchT())
				return;
			key = key.Replace('/', '+').Replace('\\', '+'); // these chars are illegal
			try
			{

				var i = new ShareStringDB(part, key, s);
				var r = await tableClient.UpsertEntityAsync(i, TableUpdateMode.Replace);
				// todo
			}
			catch (Exception e)
			{
				LogEx(e);
				tableClient = null; // try recreating it.
			}
		}
		public static async Task<List<ShareStringDB>> ReadShares(string part)
		{
			if (!TouchT())
				return null;
			try
			{

				var entities = tableClient.QueryAsync<ShareStringDB>();

				//			.Select(x => new CustomerEntity() { PartitionKey = x.PartitionKey, RowKey = x.RowKey, Email = x.Email });

				var rv = new List<ShareStringDB>();
				await foreach (var i in entities)
				{
					rv.Add(i);
				}
				return rv;
			}
			catch(Exception ex)
			{
				LogEx(ex);
				tableClient = null; // try recreating it.
			}
			return null;
		}
	}
}

