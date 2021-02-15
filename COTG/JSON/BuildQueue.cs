﻿using COTG.Draw;
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
using System.Web;

using Windows.System.Threading;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Toolkit.Extensions;
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
	public enum BuildOp
	{
		nop,
		build,
		demo,
		upgrade,
		downgrade,
	}
	public readonly struct BuildQueueItem : IEquatable<BuildQueueItem>
	{
	//	public static MemoryPool<BuildQueueItem> pool = MemoryPool<BuildQueueItem>.Shared;
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
		public bool isNop => slvl == 255; // special token for noop
		public string buildingName => def.Bn;
		public bool isBuild => slvl == 0 && elvl != 0;
		public static BuildQueueItem nop = new BuildQueueItem(255, 255, 0, 0);
		public bool isUpgrade => slvl < elvl && slvl !=0;
		public BuildOp op => 
			isDemo ? BuildOp.demo : 
			isBuild ? BuildOp.build : 
			isNop ? BuildOp.nop :
			slvl > elvl ? BuildOp.downgrade :
					BuildOp.upgrade;

		// Moves from the pending queue to the active queue;
		//public Windows.Foundation.IAsyncOperation<string> Apply(int cid)
		//{
		//	if (cid == City.build)
		//		City.buildQueue.Add(this);
		//	Assert(App.IsOnUIThread());
		//	return JSClient.view.InvokeScriptAsync("buildex", new[] { bid.ToString(), bspot.ToString(), slvl.ToString(), elvl.ToString(), cid.ToString() });
			
		//}
		public void ApplyOnUIThread(int cid)
		{
			if (cid == City.build)
				City.buildQueue.Add(this);
			
			JSClient.JSInvoke("buildex", new[] { bid.ToString(), bspot.ToString(), slvl.ToString(), elvl.ToString(), cid.ToString() });
			if (cid == City.build)
				CityView.BuildingsOrQueueChanged();
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		public bool Equals(BuildQueueItem other)
		{
			return slvl == other.slvl &&
				   elvl == other.elvl &&
				   bid == other.bid &&
				   bspot == other.bspot;
		}

		public override int GetHashCode()
		{
			return (int)slvl + (int)elvl*10+bspot*128+(int)bid*51200;
		}

		public override string ToString()
		{
			return $"{slvl}=>{elvl} {buildingName} <{City.IdToXY(bspot).bspotToString()}>({bspot})";
		}

		public static bool operator ==(BuildQueueItem left, BuildQueueItem right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(BuildQueueItem left, BuildQueueItem right)
		{
			return !(left == right);
		}
	}
	public class CityBuildQueue : IDisposable
	{
		public bool isRunning;
		public int cid;
		public DArray<BuildQueueItem> queue = new(128); // extra large
		
		public static ConcurrentDictionary<int, CityBuildQueue> all = new ConcurrentDictionary<int, CityBuildQueue>();

		void RemoveAt(int id)
		{
			BuildTab.RemoveOp(queue[id], cid);
			queue.RemoveAt(id);
		}
	     private static SemaphoreSlim throttle = new SemaphoreSlim(1);

		public async void Process(int initialDelay=0)
		{
			if (isRunning)
				return;
			isRunning = true;
			if (initialDelay > 0 )
			{
				await Task.Delay(initialDelay);
			}
			Log("Queue Starting!");
			// Ordering:
			// too few buildings - advance townhall or a demo
			// castle:  process demo first
			// repace a building:  Wait for demo to complete first
			// towers must wait for walls
			// Temple:  must demo first

			var city = City.GetOrAdd(cid);
		
			for (; ; )
			{
				int delay = 3000;
				var pollPaused = false;
				//if (queue.count > 0)
				{
					
					int cotgQLength = 0;
					DArray<BuildQueueItem> cotgQ = null;
					await throttle.WaitAsync();

					try
					{


						if (cid == City.build)
						{
							// every two seconds commands will only be queud on changes
							delay = 4000;
							// process pending queue first if appropriate
							//	ququeCount= City.buildQueue.count;
							cotgQ = City.buildQueue;
							cotgQLength = cotgQ.count;
						}
						else
						{
							var gotBQ = false;
							delay = 6000;
							pollPaused = true;
							// First try to get it from poll2, then if that fails, try GC 
							for (int i = 0; i < 3; ++i)
							{
								//	await Post.Send("/overview/mconv.php", $"a={cid}");
								//	await Post.Send("/overview/bqSt.php", $"cid={cid}");
								JSClient.extCityHack = null;
								await JSClient.JSInvokeTask("extpoll", new[] { cid.ToString() });
								int timeout = 0;
								while(JSClient.extCityHack==null)
								{
									await Task.Delay(50);
									if (++timeout > 20)
										break;
								}

								if (JSClient.extCityHack == null)
									continue;
								
								if( JSClient.extCityHack.RootElement.TryGetProperty("ext", out var ext) )
								{
									gotBQ = GetBQInfo(ref delay, ref cotgQLength, ref cotgQ, ref ext);
								}
								JSClient.extCityHack = null;
								if (gotBQ)
									break;
							}
							if (!gotBQ)
							{
								Trace("Failed to get poll?");
								await GetCity.Post(cid, (jsCity, city) =>
								 {
									 GetBQInfo(ref delay, ref cotgQLength, ref cotgQ, ref jsCity);
								 });
							}
						}
						int commandsToQueue = (City.safeBuildQueueLength - cotgQLength).Min(queue.count);
						if (commandsToQueue > 0)
						{
							StringBuilder sb = new StringBuilder();
							
							sb.Append($"{{\"{cid}\":[");
							var qFirst = true;

							int offset = 0;
							
							while(offset<queue.count && commandsToQueue > 0 )
							{
								var i = queue.v[offset];

								// do we have to wait on this on?
								// - towers before wall
								if (i.isBuild)
								{
									if (i.def.isTower)
									{
										if (city.buildings[City.bspotWall].bl == 0)
										{
											// wait for wall to build first
											// is there a wall queued?
											var wallPending = (cotgQ != null && cotgQ.Any((a) => a.bid == City.bidWall && a.slvl == 0));
											if (!wallPending)
											{
												for (int j = 0; j < offset; ++j)
												{
													if (queue.v[j].bid == City.bidWall && queue.v[j].slvl == 0)
													{
														wallPending = true;
														break;
													}
												}
											}
											if (!wallPending)
											{

												// cancel this order
												RemoveAt(offset);
											}
											else
											{
												// leave it in the queue
												++offset;
											}
											continue;
										}
									}
									else if (i.def.bid == City.bidCastle)
									{
										// TODO: check res
										var bd = city.CountBuildingWithoutQueue();
										// cannot queue if there is no room, even if there is a demo in progress
										if (bd.count >= bd.max)
										{
											Log("Waiting for space for castle");
											// leave it in the queue
											++offset;
											continue;
										}

									}
									// is there a building in the way?
									// wait for the demo
									if (i.bspot != City.bspotWall)
									{
										if (!city.buildings[i.bspot].isEmpty)
										{
											var demoPending = (cotgQ != null) && cotgQ.Any(a => a.isDemo && a.bspot == i.bspot);
											if (!demoPending)
											{
												// cancel this order
												RemoveAt(offset);

											}
											else
											{
												// leave it in the queue
												++offset;
											}
											continue;
										}
									}
								}
								else if (i.isUpgrade)
								{
									var prior = city.GetBuildingPostQueue(i.bspot, cotgQ);
									// is build not yet queued?
									if (prior.bid != i.bid)
									{
										City.GetPostQueue(ref prior, i.bspot, queue.v, offset); // is the build queued?
										if (prior.bid != i.bid)
										{
											// invalid command, discard it
											RemoveAt(offset);
											continue;
										}
										else
										{
											// leave it in the queue
											++offset;
											continue;
										}
									}
									else
									{
										// already upgraded?
										if (prior.bl > i.slvl)
										{
											// invalid command, discard it
											RemoveAt(offset);
											Trace("Invlid u");

											continue;
										}
									}
								}
								else if (i.isDemo)
								{
									var prior = city.GetBuildingPostQueue(i.bspot, cotgQ);
									if (prior.bid != i.bid)
									{
										// invalid command, discard it
										Trace("Invlid demo");
										RemoveAt(offset);
										continue;
									}
									// if there are any modifications keep it in the queue until they are done
									var isBeingModified = cotgQ != null && cotgQ.Any(a => a.bspot == i.bspot);
									if(isBeingModified)
									{
										// wait for later to destroy
										++offset;
										continue;
									}

								}
							
								// issue this command
								RemoveAt(offset);
								if (cid == City.build)
									City.buildQueue.Add(i);
								Serialize(ref sb, i, ref qFirst);
								--commandsToQueue;

							}

							if (!qFirst)
							{
								sb.Append("]}");


								JSClient.JSInvoke("buildex", new[] { sb.ToString() });

								if (cid == City.build)
									CityView.BuildingsOrQueueChanged();

								SaveNeeded();
							}
							else
							{
								Trace($"Nothing to do.. {queue.count}");
								// nothing queued
								// no progress :( wait a minute
								if (cid == City.build)
								{

								}
								else
								{
									if (delay < 2 * 30 * 1000)
										delay = 2 * 30 * 1000;
								}
							}

						}

						if (!queue.Any())
						{
							all.TryRemove(cid, out _);
							BuildTab.Clear(cid);
							Log("Queue Done!");
							SaveNeeded();
							Dispose();
							return;
						}
						Log($"Next in {delay / 1000.0f} {City.GetOrAdd(cid).nameAndRemarks}");
					}
					catch (Exception _exception)
					{
						COTG.Debug.Log(_exception);
					}
					finally
					{
						if(pollPaused)
						 JSClient.JSInvoke("resumepoll", null);

						throttle.Release();
						if (cotgQ != null && cotgQ != City.buildQueue)
							cotgQ.Dispose();
					}

				}
				Trace($"iterate: Q {queue.count} delay {delay} city {city.nameAndRemarks}");
				await Task.Delay(delay); // todo estimate this
			}

		}

		private static bool GetBQInfo(ref int delay, ref int cotgQLength, ref DArray<BuildQueueItem> cotgQ, ref JsonElement jsCity)
		{
			var result = false;
			try
			{
				if (jsCity.TryGetProperty("bq", out var bq))
				{
					if (bq.ValueKind == JsonValueKind.Array)
					{
						result = true;
						cotgQLength = bq.GetArrayLength();

						{
							var delays = new float[cotgQLength];
							var put = 0;
							foreach (var cmd in bq.EnumerateArray())
							{
								delays[put++] = JSClient.ServerTimeOffset(cmd.GetAsInt64("de"));
							}
							ShellPage.debugTip = delays.ToArrayString();
						}


						if (cotgQLength > 0 && (City.safeBuildQueueLength > cotgQLength))
						{
							delay = delay.Max(JSClient.ServerTimeOffset(bq[cotgQLength - 1].GetAsInt64("de"))); // recover after 2 seconds
							if(delay > 32*1000)
							{
							//	Assert(false);
	
								delay = 32000;
							}
							cotgQ = new DArray<BuildQueueItem>(128);
							foreach (var cmd in bq.EnumerateArray())
							{
								cotgQ.Add(new BuildQueueItem(
											 cmd.GetAsByte("slvl"),
											 cmd.GetAsByte("elvl"),
											 cmd.GetAsUShort("brep"),
											 cmd.GetAsUShort("bspot")
											 ));


							}
						}
					}
				}

			}
			catch (Exception _exception)
			{
				COTG.Debug.Log(_exception);
			}
			return result;

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



		public  void Enqueue(BuildQueueItem op)
		{
			CityView.animationOffsets[op.bspot] = AGame.animationT*CityView.animationRate; // start animation
			queue.Add(op);
			if(cid==City.build)
				CityView.BuildingsOrQueueChanged();
			BuildTab.AddOp(op, cid);
		}

		public void Dispose()
		{
			if(queue!=null)
			{
				queue.Dispose();
				queue = null;
			}
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
			if (bid == City.bidTemple && slvl == 0)
			{
				Assert(cid == City.build);
				JSClient.JSInvoke("buildTemple", new[] {spot.ToString()});
				return;
			}
			if (bid == City.bidCastle && slvl == 0)
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
			
			pending.Process();
			
		}

		public static void CancelBuildOp(int cid,in  BuildQueueItem i)
		{
			if (!CityBuildQueue.all.TryGetValue(cid, out var cq))
				return;
			var q = cq.queue;
			for(int iter=0;iter<q.count;++iter)
			{
				if (q[iter] == i)
				{
					q.RemoveAt(iter);
					return;
				}
			}
			BuildTab.RemoveOp(i, cid);
			// if its empty it will be removed next iteration
			CityView.BuildingsOrQueueChanged();
		}

		
		public static void ClearQueue(int cid = -1)
		{
			if (cid == -1)
				cid = City.build;
			if (CityBuildQueue.all.TryGetValue(cid, out var q))
			{
				q.queue.Clear();
				BuildTab.Clear(cid);
				SaveNeeded();
				CityView.BuildingsOrQueueChanged();
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

		/*
				{"10813767":[[318,451,0,0],[318,445,0,1],[319,451,0,0],[319,445,0,1],[320,445,0,1],[321,451,0,0],[342,445,0,1],[363,454,0,0],[363,445,0,1],[359,452,0,0],[359,445,0,1],[380,452,0,0],[405,452,0,0],[405,445,0,1]],"16646480":[[165,449,0,1],[142,449,0,1],[143,449,0,1],[144,449,0,1],[123,449,0,1],[101,449,0,1],[102,449,0,1],[103,449,0,1],[79,449,0,1],[80,449,0,1],[59,449,0,1],[60,449,0,1],[38,449,0,1],[82,449,0,1]],"16515413":[[359,445,0,1],[402,445,0,1],[403,445,0,1],[404,451,0,0],[404,445,0,1],[405,445,0,1],[384,445,0,1]],"10813771":[[100,445,0,1],[92,446,9,0],[121,445,0,1],[93,446,9,0],[142,445,0,1],[36,453,0,0],[60,452,0,0],[123,452,0,0],[98,451,0,0],[106,446,9,0],[36,445,0,1],[107,446,9,0],[98,445,0,1],[108,446,9,0],[60,445,0,1],[109,446,9,0],[123,445,0,1],[37,445,0,1],[110,446,9,0],[38,445,0,1],[127,446,9,0],[58,445,0,1],[128,446,9,0],[79,445,0,1],[129,446,9,0],[99,445,0,1],[130,446,9,0],[101,445,0,1],[131,446,9,0],[81,445,0,1],[148,446,9,0],[102,445,0,1],[149,446,9,0],[144,445,0,1],[150,446,9,0],[164,445,0,1],[170,446,9,0],[163,445,0,1],[171,446,9,0],[35,445,0,1],[172,446,9,0],[56,445,0,1],[192,446,9,0],[77,445,0,1],[193,446,9,0],[119,445,0,1],[244,445,0,1],[117,547,0,1]],"10944843":[[38,445,0,1],[55,446,10,0],[35,445,0,1],[56,446,10,0],[37,445,0,1],[64,446,10,0],[56,445,0,1],[66,446,10,0],[57,483,0,1],[68,446,10,0],[58,445,0,1],[77,446,10,0],[77,445,0,1],[78,453,0,0],[69,446,10,0],[78,483,0,1],[70,446,10,0],[79,445,0,1],[71,446,10,0],[80,483,0,1],[75,446,10,0],[60,445,0,1],[81,454,0,0],[85,446,10,0],[81,445,0,1],[87,446,10,0],[98,445,0,1],[89,446,10,0],[99,445,0,1],[90,446,10,0],[100,445,0,1],[95,446,10,0],[101,445,0,1],[102,454,0,0],[96,446,10,0],[102,445,0,1],[106,446,10,0],[119,445,0,1],[107,446,10,0],[120,483,0,1],[109,446,10,0],[121,445,0,1],[110,446,10,0],[122,483,0,1],[128,446,10,0],[123,445,0,1],[142,451,0,0],[129,446,10,0],[142,445,0,1],[143,451,0,0],[130,446,10,0],[143,483,0,1],[131,446,10,0],[144,445,0,1],[163,451,0,0],[150,446,10,0],[163,445,0,1],[151,446,10,0],[164,445,0,1],[169,446,10,0],[256,449,6,0],[235,449,6,0]]}
		*/

		static public void Serialize(ref StringBuilder sb, in BuildQueueItem q, ref bool qFirst)
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
				Log($"{city.cid} {city.queue.count}");
				var qFirst = true;
				foreach (var q in city.queue)
				{
					Serialize(ref sb, q, ref qFirst);
				}
				sb.Append(']');
			}
			sb.Append('}');
			var rv = sb.ToString();
			Log(rv);
			return rv;

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
					Log($"{cid} {cq.queue.count}");
					cq.Process(AMath.random.Next(1024, 3 * 1024)); // wait 1 - 3 seconds.

				}
			}

			saveTimer = ThreadPoolTimer.CreatePeriodicTimer(SaveTimer_Tick, TimeSpan.FromSeconds(120));

		}
	}
}
