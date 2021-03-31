﻿using System;
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
	class Tables
	{
		const string accountName = "avatab";

		static TableServiceClient serviceClient;
		static TableClient tableClient;
		static bool TouchT()
		{
			if (tableClient != null)
				return true;

			string storageUri = $"https://{accountName}.table.core.windows.net/";
			string storageAccountKey = "zLeBJT4uQpdbEFgcQHvlGNVX8xJctrL/qvByyZd+ujPLEf55awn9mGMVCW7UIHSJCnQ3xe3gbch88+eTjAqZng==";
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
			try
			{

				var i = new ShareStringDB(part, key, s);
				var r = await tableClient.UpsertEntityAsync(i, TableUpdateMode.Replace);
				// todo
			}
			catch (Exception e)
			{
				Log(e);
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
				Log(ex);
				tableClient = null; // try recreating it.
			}
			return null;
		}
	}
}
