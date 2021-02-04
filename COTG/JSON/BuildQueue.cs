using COTG.Game;
using COTG.Helpers;
using COTG.Services;
using COTG.Views;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static COTG.Debug;

namespace COTG.JSON
{
	//	public struct BuildQueueItem
	//	{
	//		public long ds;
	//		public long de;
	//		public long btime;
	//		public long bidHash; // building id hash, generated for each building in the commnad queue
	//		public int btype; // This is the "proto" member of buildingDef associated with brep
	//		public int bspot; // location
	//		public int brep; // bid type
	//		public byte slvl;
	//		public byte elvl;
	//		public byte pa; // pa=1 is queued normally, pa=0 is not paid

	//		public bool isRes => BuildingDef.IsRes(brep);
	//		public bool isDemo => elvl == 0;
	//		public bool isBuild => slvl == 0;

	//		/*	"bq": [
	//            {
	//                "bid": 83072020,
	//                "btype": 455,
	//                "bspot": 220,
	//                "slvl": 8,
	//                "elvl": 9,
	//                "ds": 1610764495448,
	//                "de": 1610770669591,
	//                "brep": 455,
	//                "btime": 6174143,
	//                "pa": 1

	//}*/
	//	}

	public struct BuildQueueItem
	{
		public byte slvl;
		public byte elvl;
		public ushort bid; // building id
		public ushort bspot; // xy

		public BuildQueueItem(byte slvl, byte elvl, ushort bid, ushort bspot)
		{
			this.slvl = slvl;
			this.elvl = elvl;
			this.bid = bid;
			this.bspot = bspot;
		}

		//public bool isBuild => slvl == 0;

		public bool isRes => BuildingDef.IsRes(bid);
		public bool isDemo => elvl == 0;


		// Moves from the pending queue to the active queue;
		public void Apply(int cid)
		{
			JSClient.view.InvokeScriptAsync("buildex", new[] { bid.ToString(), bspot.ToString(), slvl.ToString(), elvl.ToString(), cid.ToString() });
			if(cid == City.build)
				City.buildQueue.Add(this);
		}
	}
	public class CityBuildQueue
	{
		public int cid;
		public ConcurrentQueue<BuildQueueItem> queue = new ConcurrentQueue<BuildQueueItem>();
		public static ConcurrentDictionary<int, CityBuildQueue> all = new ConcurrentDictionary<int, CityBuildQueue>();

		public async void Process()
		{
			for (; ; )
			{
				var delay = 2000;
				if (cid == City.build)
				{
					// process pending queue first if appropriate
					while (City.wantBuildCommands && queue.TryDequeue(out var rv))
					{
						rv.Apply(cid);
					}
				}
				else
				{
					const int desBuildQueueLenght = 12;
					// todo
					await GetCity.Post(cid, (jse, city) =>
					 {
						 if (jse.TryGetProperty("bq", out var bq))
						 {
							 int count = bq.GetArrayLength();
							 // todo: sort dependencies
							 var js = bq[count - 1];
							 var e = js.GetAsInt64("de");
							 var t = JSClient.AsJSTime(e);
							 var st = JSClient.ServerTime();
							 Log(t);
							 Log(st);
							 delay = (t - st).TotalMilliseconds.RoundToInt() - 2000; // recover after 2 seconds
							 if (delay < 1000)
								 delay = 1000;
							 Log(delay);
							 // no progress :( wait a minute
							 if(count >= desBuildQueueLength)
							 {
								 if (delay < 60 * 1000)
									 delay = 60 * 1000;
							 }

							 for (;count< desBuildQueueLenght;++count)
							 {
								 if( queue.TryDequeue(out var rv))
								 {
									 rv.Apply(cid);
								 }
								 else
								 {
									 break;
								 }

							 }
							


						 }
						 else
						 {
							 int counter = 0;
							 while (++counter <= desBuildQueueLenght && queue.TryDequeue(out var rv))
							 {
								 rv.Apply(cid);
							 }
							 delay = 5000;
						 }

					 });
				}

				if (!queue.Any())
				{
					all.TryRemove(cid, out var dummy);
					return;
				}
				await Task.Delay(delay); // todo estimate this
			}

		}

		public static CityBuildQueue Get(int cid)
		{
			if (all.TryGetValue(cid, out var rv))
				return (rv);
			rv = new CityBuildQueue() { cid = cid };
			if (all.TryAdd(cid, rv))
				return (rv);
			else
				return (all[cid]); // someone else added
		}




	}
	public static class BuildQueueHelper
	{
		public static void Enqueue(this int cid, byte slvl, byte elvl, ushort bid, ushort spot)
		{
			var pending = CityBuildQueue.Get(cid);

			var op = new BuildQueueItem(slvl, elvl, bid, spot);
			pending.queue.Enqueue(op);
			if (pending.queue.Count == 1)
			{
				pending.Process();
			}
		}
		public static bool TryGetQueue(out IEnumerator<BuildQueueItem> rv)
		{
			rv = null;
			if (!CityBuildQueue.all.TryGetValue(City.build, out var q))
				return false;
			rv = q.queue.GetEnumerator();
			return true;
		}
		public static void ClearQueue()
		{
			if (!CityBuildQueue.all.TryGetValue(City.build, out var q))
				q.queue.Clear();
		}

	}
}
