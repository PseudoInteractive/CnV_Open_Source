using COTG.Draw;
using COTG.Game;
using COTG.Helpers;
using COTG.Services;
using COTG.Views;

using System;
using System.Buffers;
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
using static COTG.JSON.BuildQueue;

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
		public static MemoryPool<BuildQueueItem> pool = MemoryPool<BuildQueueItem>.Shared;
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
		public BuildingDef def => BuildingDef.all[bid];
		public bool isRes => BuildingDef.IsRes(bid);
		public bool isDemo => elvl == 0;

		public bool isBuild => slvl == 0 && elvl != 0;


		// Moves from the pending queue to the active queue;
		public Windows.Foundation.IAsyncOperation<string> Apply(int cid)
		{
			if (cid == City.build)
				City.buildQueue.Add(this);
			return JSClient.view.InvokeScriptAsync("buildex", new[] { bid.ToString(), bspot.ToString(), slvl.ToString(), elvl.ToString(), cid.ToString() });
			
		}
		public void ApplyOnUIThread(int cid)
		{
			if (cid == City.build)
				City.buildQueue.Add(this);
			
			JSClient.JSInvoke("buildex", new[] { bid.ToString(), bspot.ToString(), slvl.ToString(), elvl.ToString(), cid.ToString() });
			CityView.BuildingsOrQueueChanged();
		}
	}
	public class CityBuildQueue
	{
		public int cid;
		public ConcurrentQueue<BuildQueueItem> queue = new ConcurrentQueue<BuildQueueItem>();
		public static ConcurrentDictionary<int, CityBuildQueue> all = new ConcurrentDictionary<int, CityBuildQueue>();
		public bool TryDequeue(out BuildQueueItem BuildQueueItem)
		{
			if(queue.TryDequeue(out BuildQueueItem))
			{
				BuildTab.RemoveOp(BuildQueueItem, cid);
				return true;
			}
			return false;
		}
		public bool TryPeek(out BuildQueueItem BuildQueueItem)
		{
			if (queue.TryPeek(out BuildQueueItem))
			{
				return true;
			}
			return false;
		}
		public async void Process(int initialDelay=0)
		{
			if(initialDelay > 0 )
			{
				await Task.Delay(initialDelay);
			}
			// Ordering:
			// too few buildings - advance townhall or a demo
			// castle:  process demo first
			// repace a building:  Wait for demo to complete first
			// towers must wait for walls
			// Temple:  must demo first

			var city = City.GetOrAdd(cid);
		
			for (; ; )
			{
				int ququeCount = 0;
				int delay;

				if (cid == City.build)
				{
					// every two seconds commands will only be queud on changes
					delay = 2000;
					// process pending queue first if appropriate
					ququeCount= City.buildQueue.count;
				}
				else
				{
					delay = 5000;

					// todo
					await GetCity.Post(cid, (jse, city) =>
					 {
						
						 if (jse.TryGetProperty("bq", out var bq))
						 {
							 ququeCount = bq.GetArrayLength();
							 if (ququeCount > 0)
							 {
								 // todo: sort dependencies
								 var js = bq[ququeCount - 1];

								 var e = js.GetAsInt64("de");
								 var t = JSClient.AsJSTime(e);
								 var st = JSClient.ServerTime();
								 delay = (t - st).TotalMilliseconds.RoundToInt() - 2000; // recover after 2 seconds
								 if (delay < 5000)
									 delay = 5000; // never wait less than 5 seconds
								 // no progress :( wait two minutes
								 if (ququeCount >= City.safeBuildQueueLength)
								 {
									 if (delay < 2 * 60 * 1000)
										 delay = 2 * 60 * 1000;
								 }
								 //foreach(var cmd in bq.EnumerateArray())
								 //{
									// cotgQ.Add(new BuildQueueItem());
									// cotgQ.Add(new BuildQueueItem(
									//	 cmd.GetAsByte("slvl"),
									//	 cmd.GetAsByte("elvl"),
									//	 cmd.GetAsUShort("brep"),
									//	 cmd.GetAsUShort("bspot")
									//	 ));

								 //}
							 }
						 }
						

					 });
				}
				int commandsToQueue = (City.safeBuildQueueLength - ququeCount).Min(queue.Count);
				if(commandsToQueue > 0)
				{
					var ops = new BuildQueueItem[commandsToQueue];
					int put = 0;
	//				if(BuildTab.IsVisible)
	//					BuildTab.cvsGroups.View
					do
					{
						if (TryPeek(out var i))
						{
							// do we have to wait on this on?
							// - towers before wall
							if( i.isBuild )
							{
								if (i.def.isTower)
								{
									if (city.buildings[City.bspotWall].bl == 0)
									{
										// wait for wall to build first
										break;

									}
								}
								else if(i.def.bid == City.bidCastle)
								{
									var bd = city.CountBuildingWithoutQueue();
									// cannot queue if there is no room, even if there is a demo in progress
									if (bd.count >= bd.max)
									{
										break;
									}
									
								}
								// is there a building in the way?
								// wait for the demo
								if((!city.buildings[i.bspot].isEmpty) && i.bspot != City.bspotWall)
								{
									break;
								}
							}
							// - castles before free space
							// - build before demo

						}


						if (!TryDequeue(out var rv))
						{
							break;
						}
						ops[put++] = rv;

					} while (--commandsToQueue > 0);
					App.DispatchOnUIThreadSneakyLow(async () =>
					{
						for (int i = 0; i < put; ++i)
						{
							var _i = i;
							await ops[_i].Apply(cid);
						}
						CityView.BuildingsOrQueueChanged();
						SaveNeeded();
					}

					);
					

				}


				if (!queue.Any())
				{
					all.TryRemove(cid, out var dummy);
					SaveNeeded();
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



		public  BuildQueueItem Enqueue(BuildQueueItem op)
		{
			CityView.animationOffsets[op.bspot] = AGame.animationT*CityView.animationRate; // start animation
			queue.Enqueue(op);
			CityView.BuildingsOrQueueChanged();
			BuildTab.AddOp(op, cid);
			return op;
		}


	}

	public static class BuildQueue
	{
		static ThreadPoolTimer saveTimer;
		public static byte buildActionCounter; // needs to be saved to disc
		public static void SaveNeeded() => buildActionCounter = 3;


		public static bool initialized => saveTimer != null;
		public static async void Enqueue(this int cid, byte slvl, byte elvl, ushort bid, ushort spot)
		{
			Assert(initialized);
			var op = new BuildQueueItem(slvl, elvl, bid, spot);
			if (bid == City.bidTemple)
			{
				Assert(cid == City.build);
				op.ApplyOnUIThread(cid);
				return;
			}
			if (bid == City.bidCastle)
			{
				Assert(cid == City.build);
				var dialog = new ContentDialog()
				{
					Title = $"Castle {City.GetOrAddCity(cid).nameAndRemarks}?",
					Content = "Building a Castle allows you to plunder, scout, assault and siege other cities, but it also makes your own city vulnerable to attacks from other players. Building a Castle will exponentially increase the maximum army size of your city, and is an irreversible action.",
					PrimaryButtonText = "Yes",
					SecondaryButtonText = "Cancel"
				};
				if (await dialog.ShowAsync2().ConfigureAwait(true) != ContentDialogResult.Primary)
				{
					return;
				}
			}
			var pending = CityBuildQueue.Get(cid);
			pending.Enqueue(op);
			SaveNeeded();
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
				BuildTab.Clear(City.build);
				SaveNeeded();
			}
		}
		public static void SaveIfNeeded()
		{
			if (!initialized)
				return;
			SaveTimerGo(true);
		}
		static internal void ShutDown(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
		{
			if (!initialized)
				return;
			saveTimer.Cancel();
			SaveTimerGo(true); // flush pending saves if any
		}
		static internal void SaveTimer_Tick(ThreadPoolTimer timer)
		{
			SaveTimerGo(false);
		}
		static internal void SaveTimerGo(bool force)
		{
			if (buildActionCounter == 0)
			{
				return;
			}
			if (force)
				buildActionCounter = 0;
			else
				--buildActionCounter;
			if (buildActionCounter > 0)
				return;
			
				try
				{
					if (CityBuildQueue.all.Any())
					{
						var str = Serialize();
						Log($"SaveQueue: {str}");
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
					SaveNeeded(); // something went wrong, try again later
				}
			
		}

		static public string Serialize()
		{
			StringBuilder sb = new StringBuilder("{");
			bool isFirst = true;
			foreach (var city in CityBuildQueue.all.Values)
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
				Log($"{city.cid} {city.queue.Count}");
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
		public static async void Initialize()
		{
			if (initialized)
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
				foreach (var jsCity in js.RootElement.EnumerateObject())
				{
					var cid = int.Parse(jsCity.Name);
					var cq = CityBuildQueue.Get(cid);
					foreach (var d in jsCity.Value.EnumerateArray())
					{
						var op = new BuildQueueItem(d[2].GetByte(), d[3].GetByte(), d[1].GetUInt16(), d[0].GetUInt16());
						cq.Enqueue(op);
					}
					Log($"{cid} {cq.queue.Count}");
					cq.Process(AMath.random.Next(1024, 3 * 1024)); // wait 1 - 3 seconds.

				}
			}

			saveTimer = ThreadPoolTimer.CreatePeriodicTimer(SaveTimer_Tick, TimeSpan.FromSeconds(120));

		}
	}
}
