using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Azure;
using Azure.Data.Tables;
using Azure.Data.Tables.Models;

namespace COTG.Services
{
	class Tables
	{
		const string accountName = "avatab";
		public async Task QueryTablesAsync()
		{
			string storageUri = $"https://{accountName}.table.core.windows.net/";
			string storageAccountKey = "zLeBJT4uQpdbEFgcQHvlGNVX8xJctrL/qvByyZd+ujPLEf55awn9mGMVCW7UIHSJCnQ3xe3gbch88+eTjAqZng==";
			string tableName = "sharestring";

			var serviceClient = new TableServiceClient(
				new Uri(storageUri),
				new TableSharedKeyCredential(accountName, storageAccountKey) );

			var table = await serviceClient.CreateTableAsync(tableName);

			#region Snippet:TablesSample3QueryTablesAsync
			// Use the <see cref="TableServiceClient"> to query the service. Passing in OData filter strings is optional.
			AsyncPageable<TableItem> queryTableResults = serviceClient.GetTablesAsync(filter: $"TableName eq '{tableName}'");

			Console.WriteLine("The following are the names of the tables in the query result:");
			// Iterate the <see cref="Pageable"> in order to access individual queried tables.
			await foreach (TableItem table in queryTableResults)
			{
				Console.WriteLine(table.TableName);
			}
			#endregion

			await serviceClient.DeleteTableAsync(tableName);
		}
	}
}
}
