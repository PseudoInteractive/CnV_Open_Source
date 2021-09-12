using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AppCenter.Analytics;
using System.Threading.Tasks;

// Todo: batch these and use a background thread
namespace COTG
{
	

	public static class AAnalytics
	{
		public static bool initialized;
		// Todo: Batch these and dispatch in a background thread
		public static void Track(string e, IDictionary<string, string> properties = null)
		{
			if(initialized)
				Analytics.TrackEvent(e, properties);
		}
	}
}
