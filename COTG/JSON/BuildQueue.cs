using COTG.Game;
using COTG.Helpers;
using COTG.Services;
using COTG.Views;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Windows.System.Threading;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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

	public readonly struct BuildQueueItem
	{
		public readonly byte slvl;
		public readonly byte elvl;
		public readonly ushort bid; // building id
		public readonly ushort bspot; // xy

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
		public static bool saveNeeded; // needs to be saved to disc
		static ThreadPoolTimer saveTimer;
		public static bool initialized => saveTimer != null;
		public async void Process(int initialDelay=0)
		{
			if(initialDelay > 0 )
			{
				await Task.Delay(initialDelay);
			}
			for (; ; )
			{
				int delay;
				if (cid == City.build)
				{
					// every two seconds commands will only be queud on changes
					delay = 2000;
					// process pending queue first if appropriate
					while (City.wantBuildCommands && queue.TryDequeue(out var rv))
					{
						rv.Apply(cid);
						saveNeeded = true;
					}
				}
				else
				{
					delay = 5000;

					// todo
					await GetCity.Post(cid, (jse, city) =>
					 {
						 int count = 0;
						 if (jse.TryGetProperty("bq", out var bq))
						 {
							 count = bq.GetArrayLength();
							 // todo: sort dependencies
							 var js = bq[count - 1];
							 var e = js.GetAsInt64("de");
							 var t = JSClient.AsJSTime(e);
							 var st = JSClient.ServerTime();
							 Log(t);
							 Log(st);
							 delay = (t - st).TotalMilliseconds.RoundToInt() - 2000; // recover after 2 seconds
							 if (delay < 5000)
								 delay = 5000; // never wait less than 5 seconds
							 Log(delay);
							 // no progress :( wait two minutes
							 if (count >= City.safeBuildQueueLength)
							 {
								 if (delay < 2 * 60 * 1000)
									 delay = 2 * 60 * 1000;
							 }
						 }
						 for (; count < City.safeBuildQueueLength; ++count)
						 {
							 if (queue.TryDequeue(out var rv))
							 {
								 rv.Apply(cid);
								 saveNeeded = true;
							 }
							 else
							 {
								 break;
							 }

						 }

					 });
				}

				if (!queue.Any())
				{
					all.TryRemove(cid, out var dummy);
					saveNeeded = true;
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
		static public string Serialize()
		{
			StringBuilder sb = new StringBuilder("{");
			bool isFirst = true;
			foreach (var city in all.Values)
			{
				if (isFirst)
				{
					isFirst = false;
				}
				else
				{
					sb.Append(',');
				}
				sb.Append($"\"{city.cid}\":[");
				var qFirst = true;
				foreach (var q in city.queue)
				{
					if (qFirst)
					{
						qFirst = false;
					}
					else
					{
						sb.Append(',');
					}
					sb.Append($"[{q.bspot},{q.bid},{q.slvl},{q.elvl}]");
				}
				sb.Append(']');
			}
			sb.Append('}');
			return sb.ToString();

		}
		public static async void Init()
		{
			if(initialized)
			{
				Assert(false);
				return;
			}
			var data = await Cosmos.LoadBuildQueue();
			if (!data.IsNullOrEmpty())
			{
				// Todo:  Ensure no building occurs yet
				// Load from JSon, too bad we can't do UTF8
				var js = JsonDocument.Parse(data);
				foreach(var jsCity in js.RootElement.EnumerateObject())
				{
					var cid = int.Parse(jsCity.Name);
					var cq = Get(cid);
					foreach(var d in jsCity.Value.EnumerateArray())
					{
						var op = new BuildQueueItem(d[2].GetByte(),d[3].GetByte(), d[1].GetUInt16(), d[0].GetUInt16() );
						cq.queue.Enqueue(op);
					}
					cq.Process(AMath.random.Next(1024, 3 * 1024)); // wait 1 - 3 seconds.

				}
			}
			// Todo: load
			saveTimer = ThreadPoolTimer.CreatePeriodicTimer(Dummy.instance.SaveTimer_Tick, TimeSpan.FromMinutes(1));

			App.DispatchOnUIThreadSneaky(() => Window.Current.Closed += Dummy.instance.ShutDown);
		}

		

		class Dummy
		{
			internal static Dummy instance = new Dummy();
			internal void ShutDown(object sender, Windows.UI.Core.CoreWindowEventArgs e)
			{
				if (!initialized)
					return;
				saveTimer.Cancel();
				SaveTimer_Tick(null); // flush pending saves if any
			}
			internal void SaveTimer_Tick(ThreadPoolTimer timer)
			{
				if (saveNeeded == false)
					return;
				saveNeeded = false;
				try
				{
					if (all.Any())
					{
						var str = Serialize();
						Cosmos.SaveBuildQueue(str);
					}
					else
					{
						Cosmos.ClearBuildQueue();
					}
				}
				catch (Exception ex)
				{
					Log(ex);
					saveNeeded = true; // something went wrong, try again later
				}
			}
		}
	
	}

	public static class BuildQueueHelper
	{
		public static async void Enqueue(this int cid, byte slvl, byte elvl, ushort bid, ushort spot)
		{
			Assert(CityBuildQueue.initialized);
			var op = new BuildQueueItem(slvl, elvl, bid, spot);
			if(bid == City.bidTemple)
			{
				Assert(cid == City.build);
				op.Apply(cid);
				return;
			}
			if(bid == City.bidCastle)
			{
				Assert(cid == City.build);
				var dialog = new ContentDialog()
				{
					Title = $"Castle {City.GetOrAddCity(cid).nameAndRemarks}?",
					Content = "Building a Castle allows you to plunder, scout, assault and siege other cities, but it also makes your own city vulnerable to attacks from other players. Building a Castle will exponentially increase the maximum army size of your city, and is an irreversible action.",
					PrimaryButtonText = "Yes",
					SecondaryButtonText = "Cancel"
				};
				if (await dialog.ShowAsync2().ConfigureAwait(true) == ContentDialogResult.Primary)
				{
					Assert(cid == City.build);
					op.Apply(cid);
				}
				return;
			}
			var pending = CityBuildQueue.Get(cid);
			CityBuildQueue.saveNeeded = true;
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
			if (CityBuildQueue.all.TryGetValue(City.build, out var q))
			{
				q.queue.Clear();
				CityBuildQueue.saveNeeded = true;
			}
		}

	}
}
