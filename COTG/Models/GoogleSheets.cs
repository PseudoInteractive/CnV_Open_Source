using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CnV;

public class GoogleSheets
{

	//static void Main(string[] args)
	//{
	//	var config = BuildConfig();

	//	// Get the Google Spreadsheet Config Values
	//	var serviceAccount = config["GOOGLE_SERVICE_ACCOUNT"];
	//	var documentId = config["GOOGLE_SPREADSHEET_ID"];
	//	var jsonCredsPath = config["GOOGLE_JSON_CREDS_PATH"];

	//	// In this case the json creds file is stored locally, but you can store this however you want to (Azure Key Vault, HSM, etc)
	//	var jsonCredsContent = File.ReadAllText(jsonCredsPath);

	//	// Create a new SheetHelper class
	//	var sheetHelper = new SheetHelper(documentId,serviceAccount,"");
	//	sheetHelper.Init(jsonCredsContent);

	//	// Get all the rows for the first 2 columns in the spreadsheet
	//	var rows = sheetHelper.GetRows(new SheetRange("",1,1,2));

	//	// Write all the values from the result set
	//	foreach(var row in rows)
	//	{
	//		foreach(var col in row)
	//		{
	//			Console.Write($"{col}\t");
	//		}
	//		Console.Write("\n");
	//	}

	//	// export a csv file from the current spreadsheet and tab
	//	var exporter = new SheetExporter(sheetHelper);

	//	var filepath = @"output.csv";

	//	using(var stream = new FileStream(filepath,FileMode.Create))
	//	{
	//		var range = new SheetRange("",1,1,2);
	//		exporter.ExportAsCsv(range,stream);
	//	}
	//}




}