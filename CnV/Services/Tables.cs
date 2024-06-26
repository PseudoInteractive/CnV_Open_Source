﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Azure;
using Azure.Data.Tables;
using Azure.Data.Tables.Models;
using System.Globalization;
using static CnV.Debug;
using CnV.Game;
using System.Runtime.InteropServices;
using System.Web;

namespace CnV
{

	using CnVChat;
	using Game;

	
	//public class PlayerGlobal : ITableEntity
	//{
	//	public PlayerGlobal()
	//	{
	//	}

	//	public PlayerGlobal(string partitionKey, string rowKey, string s)
	//	{
	//		this.PartitionKey = partitionKey;
	//		this.RowKey = rowKey;
	//		this.s = s;

	//	}

	//	public string PartitionKey { get; set; }
	//	public string RowKey { get; set; }
	//	public DateTimeOffset? Timestamp { get; set; }
	//	public ETag ETag { get; set; }
	//	public string s { get; set; }
	//}

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
	
	//public class DiscordAllianceDB : ITableEntity
	//{
	//	public string PartitionKey { get; set; }
	//	public string RowKey { get; set; }
	//	public DateTimeOffset? Timestamp { get; set; }
	//	public ETag ETag { get; set; }
	//	public Int64 ChatID { get; set; }


	//}


	//public class IncomingDB : ITableEntity
	//{
	//	public IncomingDB() 
	//	{ 
	//	}

	//	public IncomingDB(DateTimeOffset date,long orderId)
	//	{
	//		this.PartitionKey = date.ToString("yyyy_MM_dd", CultureInfo.InvariantCulture );
	//		this.RowKey = orderId.ToString();
	//	}

	//	public string PartitionKey { get; set; }
	//	public string RowKey { get; set; }
	//	public DateTimeOffset? Timestamp { get; set; }
	//	public ETag ETag { get; set; }
	//}

	static class Tables
	{
	//	const string accountNameCnV = "cnv";
	//	const string storageAccountKeyCnV = "EWd320nJYCPBsIJIb53HTQvdauQLpX0zyzXhkmhzNaZLSkXJhhfzeQa6bliSfUWAWyRjlTsvPVtGxSzsfq+Rqw==";
	//	static string tableNameDiscord => $"Discord{CnVServer.world}";
	//	const string storageUriCnV = "https://" + accountNameCnV + ".table.core.windows.net/";
	//	static bool tableClientCnVInitialized;
	//	static TypedTable<DiscordAllianceDB> tableDiscord;

		const string accountName = "avata";
		const string storageUri = "https://" + accountName + ".table.core.windows.net/";
		const string storageAccountKey = "IWRPGlttorpK5DcHWin/GdA2VEcZKnHkr30lE0ZDvKLG0q1CjZONcAQYI2D26DENd7TIAxF8tPsE0mIk98BafA==";
		
		const string tableName = "sharestring";

//		static string incTableName => $"inc{CnVServer.worldId}";
		// DefaultEndpointsProtocol=https;AccountName=avata;AccountKey=IWRPGlttorpK5DcHWin/GdA2VEcZKnHkr30lE0ZDvKLG0q1CjZONcAQYI2D26DENd7TIAxF8tPsE0mIk98BafA==;EndpointSuffix=core.windows.net
		static ATableService          serviceClient = new(
														new Uri(storageUri),
														new TableSharedKeyCredential(accountName, storageAccountKey));
		static TypedTable<ShareStringDB>? TableShareStrings;



		static bool TouchT()
		{
			if (TableShareStrings != null)
				return true;

			TableShareStrings = serviceClient.Table<ShareStringDB>(tableName);
			return true;
		}
		//static TypedTable<IncomingDB> TouchIncT()
		//{
		//	return incTableClient = GetServiceClient().Table<IncomingDB>(incTableName);
		//}

		//static public async Task<TypedTable<DiscordAllianceDB> > TouchCnV()
		//{
		//	if (!tableClientCnVInitialized)
		//	{
		//		tableClientCnVInitialized=true;
		//		for (int counter = 0; counter < 32; ++counter)
		//		{
		//			if (Alliance.alliancesFetched)
		//			{
		//				tableDiscord = new ATableService(
		//					new Uri(storageUriCnV),
		//					new TableSharedKeyCredential(accountNameCnV, storageAccountKeyCnV)).Table<DiscordAllianceDB>(tableNameDiscord);
		//				break;
		//			}

		//			await Task.Delay(1000).ConfigureAwait(false);
		//		}
		//	}
		//	return tableDiscord;
		//}

		//static public async Task<ulong> GetDiscordChatId()
		//{
		//	try
		//	{
		//		var tableClient = await TouchCnV().ConfigureAwait(false);
		//		if (tableClient == null)
		//			return 0;

		//		var entity = await tableClient.GetAsync($"{Alliance.my.name} ChatInfo", "IDs").ConfigureAwait(false);
		//		if (entity is null)
		//			return 0;

		//		return (ulong)entity.ChatID;
		//	}
		//	catch (Exception ex)
		//	{
		//		Log("Alliance has no chat id");
		//		return 0;
		//	}
		//}
		//public static ulong Hash64(this string a)
		//{
		//	return a.XxHash();
		//}
		//static public async Task<bool> TryAddChatMessage(string message)
		//{
		//	try
		//	{
		//		var tableClient = await TouchCnV().ConfigureAwait(false);
		//		if (tableClient == null)
		//			return false;
		//		var hash = message.Hash64().ToString();
		//		return await tableClient.AddAsync(new () { PartitionKey = $"{Alliance.my.name} ChatInfo", RowKey = hash }).ConfigureAwait(false);
		//	}
		//	catch (Exception ex)
		//	{
		//		return false;
		//	}
		//}


		public static async void ShareShareString(string part, string key, string s)
		{
			if (!TouchT())
				return;
			key = key.Replace('/', '+').Replace('\\', '+'); // these chars are illegal
			try
			{

				var i = new ShareStringDB(part, key, s);
				var r = await TableShareStrings.UpsertAsync(i, TableUpdateMode.Replace).ConfigureAwait(false);
				// todo
			}
			catch (Exception e)
			{
				LogEx(e);
				serviceClient = null;
				TableShareStrings = null; // try recreating it.
			}
		}
		public static async Task<List<ShareStringDB>> ReadShares(string part)
		{
			if (!TouchT())
				return null;
			try
			{

				var entities = TableShareStrings.QueryAsync().ConfigureAwait(false);

				//			.Select(x => new CustomerEntity() { PartitionKey = x.PartitionKey, RowKey = x.RowKey, Email = x.Email });

				var rv = new List<ShareStringDB>();
				await foreach (var i in entities.ConfigureAwait(false))
				{
					rv.Add(i);
				}
				return rv;
			}
			catch(Exception ex)
			{
				LogEx(ex);
				TableShareStrings = null; // try recreating it.
				serviceClient = null;
			}
			return null;
		}

		//static ConcurrentHashSet<long> ordersSeen = new ConcurrentHashSet<long>();
		//		/// returns true of the order was inserted, false if it already existed
		//		static ItemRequestOptions itemRequesDefault = new ItemRequestOptions() { EnableContentResponseOnWrite = false };

		//		/// 
		//public static async Task<bool> TryAddOrder(DateTimeOffset date, long orderId)
		//{
		//	//Log($"Order {date} {orderId}");
		//	if (!ordersSeen.Add(orderId))
		//		return false;

		//	try
		//	{

		//		var tab = TouchIncT();
		//		if (tab == null)
		//			return false;

		//		var ent = new IncomingDB(date, orderId);
		//		var i = await tab.AddAsync(ent).ConfigureAwait(false);
		//		//	Log($"Created entry {orderId}");

		//		return i;
				
		//		//return false;
		//	}
		//	catch(Exception ex)
		//	{
		//		//LogEx(ex);

		//		return false;
		//	}
		//	//	////		Assert(used.Add(orderId) == true);
		//	// //           if (!await Touch() )
		//	// //               return false;
		//	// //           var order = new Order() { id = orderId.ToString() };
		//	//	//		await throttle.WaitAsync();
		//	//	//		try
		//	//	//		{
		//	//	//			//var result = await ordersContainer.CreateItemAsync(order, new PartitionKey(order.id));
		//	//	//			var result = await ordersContainer.CreateItemAsync(order, new PartitionKey(order.id), itemRequesDefault);

		//	// //           }
		//	//	//		catch (CosmosException ex)
		//	//	//		{
		//	//	//			if (ex.StatusCode == HttpStatusCode.Conflict)
		//	//	//				return false;
		//	//	//			Log(ex);

		//	//	//		}
		//	//	//		catch (Exception ex)
		//	// //           {

		//	//	//			Log(ex);
		//	//	//			return false;
		//	// //           }

		//	//	//		finally
		//	//	//		{
		//	//	//			throttle.Release();
		//	//	//		}
		//	// //           return true;
		//}
	}
}

