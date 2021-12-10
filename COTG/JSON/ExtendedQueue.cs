using CnV;

using CnV.Draw;
using CnV.Services;

using Cysharp.Text;

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

using Windows.Storage;

using static CnV.BuildQueue;
using static CnV.Game.City;

using ContentDialog = Microsoft.UI.Xaml.Controls.ContentDialog;
using ContentDialogResult = Microsoft.UI.Xaml.Controls.ContentDialogResult;

namespace CnV;

using System;
using System.IO;
using System.Linq;
using Draw;
using Game;
using Nito.AsyncEx;
using Services;

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
	move,
}

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public readonly struct BuildQueueItem:IEquatable<BuildQueueItem>
{
	//	public static MemoryPool<BuildQueueItem> pool = MemoryPool<BuildQueueItem>.Shared;
	public readonly short bid; // building id
	public readonly short bspot; // xy
	public readonly ushort buildTime; // delta time in seconds
	public readonly byte slvl;
	public readonly byte elvlAndPA; // 0xf is level mask, ox10 is pa mask
	public readonly byte elvl => (byte)(elvlAndPA & 0xF);
	public readonly bool pa => (elvlAndPA & 0x10) != 0;


	public BuildQueueItem(byte slvl,byte elvl,short bid,short bspot,ushort buildTime = 0,bool pa = false)
	{
		Assert(elvl < 11);

		if(pa)
			elvl |= 0x10;
		this.slvl = slvl;
		this.elvlAndPA = elvl;
		this.bid = bid;
		this.bspot = bspot;
		this.buildTime = (ushort)(((int)buildTime).Max(1));
	}

	public void Deconstruct(out byte slvl,out byte elvl,out short bspot,out ushort buildTime,out bool pa)
	{
		slvl = this.slvl;
		elvl = this.elvl;
		bspot = this.bspot;
		buildTime = this.buildTime;
		pa = this.pa;

	}

	//public bool isBuild => slvl == 0;
	public readonly BuildingDef def => BuildingDef.all[bid];
	public readonly bool isRes => BuildingDef.IsBidRes(bid);
	public readonly bool isDemo => elvl == 0 && (slvl != 0 || BuildingDef.IsBidRes(bid) ); // red demo is 0=>0
	public readonly bool isNop => slvl == 255; // special token for noop
	public readonly string buildingName => def.Bn;
	public readonly int proto => def.Proto;
	public readonly bool isMovePart0 => (slvl == elvl)&&(!BuildingDef.IsBidRes(bid));  // move from is 0,0, swap is both

	public readonly bool isBuild
	{
		get {
			if(slvl == 0 && elvl != 0)
			{
				Assert(bid != 0);
				return true;
			}
			return false;



		}
	}
	internal readonly bool isValid => bid != City.bidTownHall || slvl != 0;

	internal readonly bool isBuilding => !isRes;

	public readonly string bn => def.Bn;
	public readonly bool isUpgrade => slvl < elvl && slvl != 0;
	public readonly bool isDowngrade => slvl > elvl && elvl != 0;
	public readonly BuildOp op =>
		isMovePart0 ? BuildOp.move:
		isDemo ? BuildOp.demo :
		isBuild ? BuildOp.build :
		isNop ? BuildOp.nop :
		isDowngrade ? BuildOp.downgrade :
				BuildOp.upgrade;

	// Moves from the pending queue to the active queue;
	//public Windows.Foundation.IAsyncOperation<string> Apply(int cid)
	//{
	//	if (cid == City.build)
	//		City.buildQueue.Add(this);
	//	Assert(App.IsOnUIThread());
	//	return JSClient.view.ExecuteScriptAsync("buildex", new[] { bid.ToString(), bspot.ToString(), slvl.ToString(), elvl.ToString(), cid.ToString() });

	//}
	//public void ApplyOnUIThread(int cid)
	//{
	//	if (cid == City.build)
	//		City.buildQueue.Add(this);

	//	JSClient.JSInvoke("buildex", new[] { bid.ToString(), bspot.ToString(), slvl.ToString(), elvl.ToString(), cid.ToString() });
	//	if (cid == City.build)
	//		CityView.BuildingsOrQueueChanged();
	//}

	public readonly override bool Equals(object obj)
	{
		return base.Equals(obj);
	}

	public readonly bool Equals(BuildQueueItem other)
	{
		return (bid == other.bid) &
			(bspot == other.bspot) &
			(buildTime==other.buildTime)&
			(slvl == other.slvl) &
			(elvlAndPA == other.elvlAndPA);
	}



	public readonly override int GetHashCode()
	{
		return (int)slvl + (int)elvl * 10 + bspot * 128 + (int)bid * 51200;
	}

	public readonly override string ToString()
	{
		return $"{slvl}=>{elvl} {buildingName} <{City.IdToXY(bspot).bspotToString()}>({bspot})";
	}

	public static bool operator ==(BuildQueueItem left,BuildQueueItem right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(BuildQueueItem left,BuildQueueItem right)
	{
		return !(left == right);
	}

	public readonly Building Apply(Building b)
	{
		byte id;
		if(isDemo)
		{
			id = 0;
		}
		else if(isBuild || isMovePart0)
		{
			id = BuildingDef.BidToId(bid); // building id changes
		}

		else
		{
			// upgrade or downgrade
			id = b.id;  // same building
		}
		return new Building(id,elvl);
	}

	private string GetDebuggerDisplay()
	{
		return ToString();
	}
}
public class ExtendedQueue:IDisposable
{
	//public static DispatcherQueueController _queueController;
	//public static DispatcherQueue _queue;
	//public static DispatcherQueueTimer _timer;

	public float nextTick;

	// Start the Timer

	public bool isRunning;
	public int cid;
	public DArray<BuildQueueItem> queue = new(128); // extra large - this is the max commands we can queue before realloc which would mess up the threading
	public CancellationTokenSource cancellationTokenSource;
	public static ConcurrentDictionary<int,ExtendedQueue> all = new ConcurrentDictionary<int,ExtendedQueue>();
	void RemoveAt(int id)
	{
		queue.RemoveAt(id);
	}
	public AsyncLock queueLock = new();
	//		public static SemaphoreSlim queueLock = new SemaphoreSlim(1);
	public static SemaphoreSlim processLock = new SemaphoreSlim(1);
	// todo
	//public static void Initialize()
	//{
	//	_queueController = DispatcherQueueController.CreateOnDedicatedThread();
	//	_queue = _queueController.DispatcherQueue;
	//	_timer = _queue.CreateTimer();
	//	_timer.Interval = TimeSpan.FromSeconds(0.5f);

	//	// The tick handler will be invoked repeatedly after every 5
	//	// seconds on the dedicated thread.
	//	_timer.Tick += Tick;

	//}

	//private static void Tick(DispatcherQueueTimer sender, object args)
	//{
	//}
	public static City cityQueueInUse; // only one city is processing at once
	public static Debounce pollDebounce = new(RequstPoll) { debounceDelay=50,throttleDelay=1500,runOnUiThread=true };

	static Task RequstPoll()
	{
		JSClient.coreWebView.PostWebMessageAsString("{\"poll\":100}");
		return Task.CompletedTask;
	}
	static string pollResult;

	public async void Process(int initialDelay = 1000)
	{
		if(isRunning)
			return;
		isRunning = true;
		if(initialDelay > 0)
		{
			await Task.Delay(initialDelay).ConfigureAwait(false);
		}
		// Ordering:
		// too few buildings - advance townhall or a demo
		// castle:  process demo first
		// repace a building:  Wait for demo to complete first
		// towers must wait for walls
		// Temple:  must demo first

		var city = City.GetOrAdd(cid);

		for(;;)
		{
			int delay = 5000;
			//var pollPaused = false;
			//if (queue.count > 0)


			await processLock.WaitAsync().ConfigureAwait(false);
			var cotgQ = city.buildQueue;
			var cotgQLInitial = cotgQ.Length;
			var queueLInitial = queue.Length;
			try
			{

				//				Post.Get("/overview/mconv.php",$"a={cid}");
				for(int i = 0;i<3;++i)
				{
					var gotBQ = await city.DoPoll();
					if(gotBQ)
						break;
					Log($"{city.nameMarkdown} failed to get BQ {i} bq:{city.buildQueue.Length} eq:{queue.Length}");
					//Trace(pollResult);
					await Task.Delay(500);
				}
				//	var queueValid = false;
				// what if someone modifies this while in use?
				cityQueueInUse = city;


				Log($"{city.nameMarkdown} got BQ {cotgQ.Length}");
				if(cid == City.build)
				{
					// every 3 seconds commands will only be queud on changes
					delay = 2000;
					// process pending queue first if appropriate
					//	ququeCount= City.buildQueue.count;
					//	await City.buildQUpdated;
					//pollDebounce.Go();
					//if (City.buildQInSync)
					//{
					//	queueValid = true;
					////	Log(cotgQ.ToString());
					//}
					//var lg = cotgQ.Length;
					//if(lg > 0)
					//{
					//	var bt = 0;
					//	// Wait for the first only
					//	for(int i = 0;i < lg;++i)
					//		bt += cotgQ[i].buildTime;
					//	bt -= 8;
					//	delay = (bt * 1000).Clamp(5 * 1000,15 * 60 * 1000);
					//	Log("Delay " + delay / 1000.0f);
					//}

				}
				else if(city.isMine)
				{
					//var gotBQ = false;
					//var gotBuildings = false;
					delay = 10 * 1000; // 10s default
									   //var lg = cotgQ.Length;
									   //if (lg > 0)
									   //{
									   //	var anyPa=false;
									   //	{ 
									   //		var bt = 0;
									   //		// skip the first
									   //		for (int i = 0; i < lg; ++i)
									   //		{
									   //			bt += cotgQ[i].buildTime;
									   //			anyPa |=cotgQ[i].pa;
									   //		}
									   //		bt =  bt*3/4 - 4; // wait for 3/4 of the time minus 8 seconds
									   //		delay = (bt * 1000).Clamp(3 * 1000, 30 * 60 * 1000);
									   //		Trace("Delay " + delay / 1000.0f);
									   //		if(anyPa==false)
									   //		{ 
									   //			delay = delay.Max(10*1000);
									   //			Trace("nothing is paid");
									   //		}
									   //	}
									   //}

					//pollPaused = true;
					// First try to get it from poll2, then if that fails, try GC 
					//for (int i = 0; i < 3; ++i)
					//{
					//	//	await Post.Send("/overview/mconv.php", $"a={cid}");
					//	//	await Post.Send("/overview/bqSt.php", $"cid={cid}");
					//	JSClient.extCityHack = null;
					//	await JSClient.JSInvokeTask("extpoll", new[] { cid.ToString() });
					//	int timeout = 0;
					//	while(JSClient.extCityHack==null)
					//	{
					//		await Task.Delay(50);
					//		if (++timeout > 20)
					//			break;
					//	}

					//	if (JSClient.extCityHack == null)
					//		continue;

					//	if( JSClient.extCityHack.RootElement.TryGetProperty("ext", out var ext) )
					//	{
					//		gotBQ |= GetBQInfo(ref delay, ref cotgQLength, ref cotgQ, ref ext);
					//		gotBuildings |= city.TryGetBuildings(ext);
					//	}
					//	JSClient.extCityHack = null;
					//	if (gotBQ && (city.buildingsLoaded) )
					//		break;
					//}
					//if (!gotBQ || !city.buildingsLoaded)
					//{
					//	Trace("Failed to get poll or missing buildings?");
					//	await GetCity.Post(cid, (jsCity, city) =>
					//	 {
					//		 queueValid = GetBQInfo(ref delay, ref cotgQLength, ref cotgQ, ref jsCity);
					//	 });
					//}
					//else
					//{
					//	queueValid = true;
					//}
				}
				else
				{
					// City lost?
					all.TryRemove(cid,out _);

					Dispose();
					return;
				}
				if(city.buildingsLoaded)
				{

					//	var spaceInQueue = City.safeBuildQueueLength - cotgQ.Length;
					// this is either 1..+ can send commands normally
					//                0 can send unblocking commands
					//               -1  unblock un progress
					var isBlocked = (cotgQ.Length == safeBuildQueueLength) && !cotgQ.Any(a => a.pa);
					//	int commandsToQueue = isBlocked ? 1 : spaceInQueue.Min(queue.count);
					var maxBuildCommands = isBlocked ? buildQMax : safeBuildQueueLength;
				//	if(isBlocked)
				//		Trace($"Blocked! {city.nameMarkdown} ");
					// queue.count goes through here if the player cancels the queue so that this can be deleted
					if(maxBuildCommands > cotgQ.Length || queue.count == 0)
					{


						int offset = 0;
						//using var _lock = await this.queueLock.LockAsync();  // lock before modify

						try
						{

							// up to 16
							//		commandsToQueue = (City.safeBuildQueueLength + 1 - cotgQ.Length).Min(queue.count);
							//var endPoint = cotgQ.Length + commandsToQueue;
							bool anyBuildsPending = false;


							while(offset < queue.count && cotgQ.Length < maxBuildCommands)
							{
								var i = queue.v[offset];

								if(isBlocked && (i.isBuild || i.isUpgrade))
								{
									++offset;
									continue;
								}

								// do we have to wait on this on?
								// - towers before wall
								if (i.isMovePart0)
								{
									Assert(offset+1 < queue.count);
									var iNext = queue.v[offset+1];
									
									var bPrior0 = city.GetBuildingPostQueue(i.bspot,cotgQ.span,queue.Slice(0,offset));
									var bPrior1 = city.GetBuildingPostQueue(iNext.bspot,cotgQ.span,queue.Slice(0,offset));

									if ( bPrior0.bid != iNext.bid || bPrior1.bid != i.bid)
									{
										Trace("Move invalid");
										goto removeOp;
									}

									if(!city.IsValidForMove(i.bspot,iNext.bid,false))
										goto doItLater;
									if(!city.IsValidForMove(iNext.bspot,i.bid,false))
										goto doItLater;

									if (iNext.elvl == 0 )
									{
									
										// move
										if(!await DoMove(cid,iNext.bspot,i.bspot))
											goto doItLater;
									}
									else
									{
										if(Player.moveSlots <= 3)
											goto doItLater;
										var scratch = city.FindAnyFreeSpotForMove(i.bspot);
										if (scratch == 0)
										{
											goto doItLater;
										}

										Assert( city.IsValidForMove(scratch,0,false) );
									
										if(!await DoMove(cid,iNext.bspot,scratch))
											goto removeOp;
										if(!await DoMove(cid,i.bspot,iNext.bspot))
											goto removeOp;
										if(!await DoMove(cid,scratch,i.bspot))
											goto removeOp;

									}
									removeOp:
									RemoveAt(offset);
									RemoveAt(offset);
									continue;
									doItLater:
								//	Assert(false);
									offset += 2;
//									RemoveAt(offset);
//									RemoveAt(offset);
									continue;

								}
								else if(i.isBuild)
								{
									anyBuildsPending = true;
									if(i.def.isTower)
									{
										// Todo:  check res
										if(city.buildings[City.bspotWall].bl == 0)
										{
											var bw = city.GetBuildingPostQueue(bspotWall,cotgQ.span,queue.Slice(0,offset));
											// wait for wall to build first
											// is there a wall queued?
											var wallPending = bw.bl > 0;
											if(!wallPending)
											{
												Log($"Invalid tower (missing wall)");

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
									else if(i.def.isWall)
									{
										// todo:  check res
									}
									
									else
									{



										// is there a building in the way?
										// wait for the demo
										var bdPrior = city.GetBuildingPostQueue(i.bspot,cotgQ.span,queue.Slice(0,offset));
										if(!bdPrior.isEmpty)
										{
											//  trying to build on top of a spot to be built
											Trace($"Invalid build on building: {city.nameMarkdown} {i.ToString()} <=> {city.buildings[i.bspot].bid} {city.buildings[i.bspot].name},{city.buildings[i.bspot].bl}");

											// cancel this order
											
										}
										else if(!city.buildings[i.bspot].isEmpty)
										{

											// leave it in the queue
											// wait for space
											++offset;

											continue;
										}
										if(i.def.bid == City.bidCastle)
										{
											//	Log($"Castle  {city.nameMarkdown}");
											// TODO: check res
											// cannot queue if there is no room, even if there is a demo in progress
											var counts = city.CountBuildingsWithoutQueue();
											if(counts.count >= counts.max  || city.buildings[City.bspotTownHall].bl < 8)
											{
												Log("Waiting for space or stone or level for castle");
												// leave it in the queue
												++offset;
												continue;
											}
											if(city.WillHaveBuilding(bidCastle,false))
											{
												Trace("Already has a castle");
												// leave it in the queue
												RemoveAt(offset);
												continue;

											}
										}
									}
									// is building
								}
								else if(i.isUpgrade)
								{
									var prior = city.GetBuildingPostQueue(i.bspot,cotgQ.span,queue.Slice(0,offset));
									// is build not yet queued?
									if(i.elvl > 10)
									{
										Log($"Invalid ugrade to level {i.elvl}");
										RemoveAt(offset);
										continue;
									}
									else if(prior.bid != i.bid)
									{

										Log($"Invalid ugrade {prior.bid} => {i.bid}");
										// invalid command, discard it
										RemoveAt(offset);
										continue;

									}
									else
									{
										// already upgraded?
										if(prior.bl > i.slvl)
										{
											// invalid command, discard it
											RemoveAt(offset);
											Log($"Invalid upgrade {prior.bid} => {i.bid} {prior.bl} to {i.slvl}");

											continue;
										}
									}
								}
								else if(i.isDemo)
								{
									var prior = city.GetBuildingPostQueue(i.bspot,cotgQ.span,queue.Slice(0,offset));
									if(prior.bid != i.bid)
									{
										// invalid command, discard it
										Log($"Invalid demo  {prior.bid} => {i.bid}");
										RemoveAt(offset);
										continue;
									}
									// Todo checkr es
									// if there are any modifications keep it in the queue until they are done
									var isBeingModified = cotgQ.Any(a => a.bspot == i.bspot);
									if(isBeingModified)
									{
										// wait for later to destroy
										++offset;
										continue;
									}
								}
								else if(i.isDowngrade)
								{
									if(anyBuildsPending && i.bspot == City.bspotTownHall)
									{
										// Don't down grade townhall until building finished
										++offset;
										continue;

									}
									var prior = city.GetBuildingPostQueue(i.bspot,cotgQ.span,queue.Slice(0,offset));
									if(prior.bid != i.bid)
									{

										Trace($"Invalid downgrade {prior.bid} => {i.bid}");
										// invalid command, discard it
										RemoveAt(offset);
										continue;

									}
									else
									{
										// already upgraded?
										if(prior.bl != i.slvl)
										{
											// invalid command, discard it
											RemoveAt(offset);
											Trace($"Invalid downgrade {prior.bid} => {i.bid}");

											continue;
										}
									}

								}

								// issue this command
								//									if (cid == City.build)
								//										City.buildQueue.Add(i);
								if(i.bid==bidCastle)
								{

									// casle failed.
									var a = await Post.SendForText("/includes/nCb.php",$"type={bidCastle}&spot={i.bspot}&cid={cid}&bt={CnVServer.ServerTimeMs()}").ConfigureAwait(false);
									// failed to build castle, keep it in the queue

									if(a==null || a.Length <= 4)
									{
										// error!
										++offset;
										continue;
									}

								}
								else
								{
									var de = (CnVServer.ServerTimeSeconds()+1 + cotgQ.Sum(q => q.buildTime));

									var ok = await IssueCommand(i,cid,de*1000).ConfigureAwait(false);
									if(!ok)
									{
										Assert(false);
										++offset;
										continue;

									}
								}


								RemoveAt(offset);
								//	cotgQ.Add(i);

								//	--commandsToQueue;
								//	if (validCount > 0 && cotgQ.Length >= City.safeBuildQueueLength)
								//		break;
							}







							if(!queue.Any())
							{
								//	if (!queue.Any())
								{
									Log($"Queue Complete {city} ex:{queueLInitial} cotg:{cotgQLInitial}");
									var okay= all.TryRemove(cid,out _);
									Assert(okay);
									Dispose();
									SaveNeeded();
									return;
								}
							}
							else
							{
								//	Trace("Queue invalid");
							}



						}
						catch(Exception ex)
						{
							LogEx(ex);
						}
						finally
						{
							//	queueLock.Release();
						}

					}
					else
					{
						Log($"No space {city} Q:{cotgQ.Length} XQ:{queue.Length} isBlocked:{isBlocked}");
					}

					//	await Post.Get("/overview/mconv.php",$"a={cid}",onlyHeaders: true).ConfigureAwait(false); ;

					//if(cid != City.build)
					//{
					//	//			Post.Get("/includes/bqSt.php", $"cid={cid}",onlyHeaders:true).ContinueWith( (_) => Post.Get("/includes/poll2.php",$"cid={cid}&ai=0&ss={JSClient.ppss}",onlyHeaders:true) );
					//	await Post.Get("/includes/bqSt.php", $"cid={cid}",onlyHeaders:true).ContinueWith( (_) => Post.Get("/includes/poll2.php",$"cid={cid}&ai=0&ss={JSClient.ppss}",onlyHeaders:true) );

					//}

				} // buildings loaded
				else
				{
					Log($"{city} buildings not loaded");
				}
			}
			catch(Exception _exception)
			{
				Debug.LogEx(_exception);
			}
			finally
			{
				//if(pollPaused)
				//  await JSClient.JSInvokeTask("resumepoll", null);
				//if (cotgQ != null && cotgQ != City.buildQueue)
				//	cotgQ.Dispose();
				cityQueueInUse = null;
				processLock.Release();
			}


			//				if(delay > 60*1000 )
			//		if(city.cid!= City.build)
			//			Trace($"iterate: Q {queue.count} delay {delay} city {city.nameMarkdown}");
			try
			{
				//	Trace( $"Delay:{cid==City.build} {delay/1000.0f} {city.nameMarkdown} bq:{city.buildQueue.count} xq:{queue.count}");

				cancellationTokenSource = new();
				//delay -= 300;
				//if(delay > 1000)
				if(cid != City.build)
				{
					await Task.Delay(1200).ConfigureAwait(false);
					Debounce.Q(uniqueNumber: cid*113,debounceT: 200,throttleT: 4000,action: async () =>
					{
						await city.DoPoll().ConfigureAwait(false);

					});
					// todo:  Read city data
					//var cotgQ = city.buildQueue;
					var anyPa = false;
					{
						var bt = 0;
						// skip the first
						for(int i = 0;i < cotgQ.Length;++i)
						{
							bt += cotgQ[i].buildTime;
							anyPa |=cotgQ[i].pa;
						}
						bt =  bt/2 - 4; // wait for 3/4 of the time minus 8 seconds
						delay = (bt * 1000).Clamp(3 * 1000,30 * 60 * 1000);
						Log($"Delay {city} q:{cotgQ.count} ext:{queue.count} dt:{delay / 1000.0f}");
						if(anyPa==false)
						{
							delay = delay.Max(10*1000);
							Log($"- nothing is paid {city}  q:{cotgQ.count} ext:{queue.count} dt:{delay / 1000.0f}");
						}
					}


					if(cotgQ.Length != cotgQLInitial || queue.Length != queueLInitial)
					{
						if(delay < 1 * 20 * 1000)
						{
							//											Log($"Short delay: {city} {delay/1000f} {queue.count}");
							delay = 1 * 20 * 1000;
						}

						Log($"Nothing changed, {city} q:{cotgQ.count} ext:{queue.count} dt:{delay / 1000.0f}");

					}


					//			Post.Get("/includes/bqSt.php", $"cid={cid}",onlyHeaders:true).ContinueWith( (_) => Post.Get("/includes/poll2.php",$"cid={cid}&ai=0&ss={JSClient.ppss}",onlyHeaders:true) );

					//	delay -= 1000;
				}
				else
				{
					if(cotgQ.Length != cotgQLInitial || queue.Length != queueLInitial)
					{
						city.BuildingsOrQueueChanged();
						//		if(delay > 2000)
						//			delay = 2000;
						//AppS.DispatchOnUIThreadIdle(() => JSClient.coreWebView.PostWebMessageAsString("{\"poll\":200}"));
						pollDebounce.Go();
					}
					else
					{
						Log($"Nothing to do {cid==City.build} {city.nameMarkdown}.. {queue.count} dt: {delay/1000}");
						// nothing queued
						// no progress :( wait a minute
						if(delay < 1 * 10 * 1000)
						{
							//												Log($"Short delay: {city} {delay/1000f} {queue.count}");
							delay = 1 * 10 * 1000;
						}
					}
					Log($"Build Q: {city} q:{cotgQ.count} ext:{queue.count} dt:{delay / 1000.0f}");
				}
				if(delay > 0)
					await Task.Delay(delay,cancellationTokenSource.Token).ConfigureAwait(false);

			}
			catch(TaskCanceledException tex)
			{
			}
			catch(Exception ex)
			{
				LogEx(ex);
			}
			finally
			{
				var c = cancellationTokenSource;
				cancellationTokenSource = null;
				if(c!= null)
				{
					c.Dispose();
				}
			}
		}
	}



	//private static bool GetBQInfo(ref int delay, City city)
	//{
	//	var lg = city.buildQueue.Length;
	//	if (lg > 0)
	//	{
	//		delay = delay.Max(CnVServer.ServerTimeOffsetMs(city.buildQueue[ (lg-2).Max(0) ].de ).Min(15*60*1000) ); 
	//	}
	//	try
	//	{
	//		if (jsCity.TryGetProperty("bq", out var bq))
	//		{
	//			if (bq.ValueKind == JsonValueKind.Array )
	//			{
	//				result = true;

	//				//{
	//				//	var delays = new float[cotgQLength*2];
	//				//	var put = 0;
	//				//	foreach (var cmd in bq.EnumerateArray())
	//				//	{
	//				//		delays[put++] = CnVServer.ServerTimeOffset(cmd.GetAsInt64("ds"));
	//				//		delays[put++] = CnVServer.ServerTimeOffset(cmd.GetAsInt64("de"));
	//				//	}
	//				//	ShellPage.debugTip = delays.ToArrayString();
	//				//	Log(delays.ToArrayString());
	//				//}
	//				if (cotgQLength > 0)
	//				{
	//					if (delay > 15 * 60 * 1000) /// never more than 15 minutes please
	//					{
	//						//	Assert(false);

	//						delay = 15 * 60 * 1000;
	//					}

	//				//	if ((City.safeBuildQueueLength > cotgQLength))
	//					{
	//						cotgQ = new DArray<BuildQueueItem>(128);
	//						foreach (var cmd in bq.EnumerateArrayOrObject())
	//						{
	//							cotgQ.Add(new BuildQueueItem(
	//										 cmd.GetAsByte("slvl"),
	//										 cmd.GetAsByte("elvl"),
	//										 cmd.GetAsUShort("brep"),
	//										 cmd.GetAsUShort("bspot")
	//										 ));


	//						}
	//					}
	//				}
	//			}
	//			else if (bq.ValueKind == JsonValueKind.Object)
	//			{ 
	//			}
	//			else
	//			{
	//				Assert(false);

	//			}
	//		}

	//	}
	//	catch (Exception _exception)
	//	{
	//		COTG.Debug.LogEx(_exception);
	//	}
	//	return result;

	//}

	public static async Task UnblockQueue(int cid)
	{
		//await queueLock.WaitAsync().ConfigureAwait(false);
		//try
		{
			if(all.TryGetValue(cid,out var rv))
			{
				var cancelToken = rv.cancellationTokenSource;
				if(cancelToken!=null)
					rv.cancellationTokenSource.Cancel();

			}
		}
		//finally
		{
			//	queueLock.Release();
		}


	}
	public static IEnumerable<BuildQueueItem> TryGetBuildQueue(int cid)
	{
		if(all.TryGetValue(cid,out var rv))
		{
			return rv.queue;
		}
		return Array.Empty<BuildQueueItem>();
	}
	public static ExtendedQueue Get(int cid)
	{
		//	await queueLock.WaitAsync().ConfigureAwait(false);
		return (
			all.GetOrAdd(cid,(_cid) => new ExtendedQueue() { cid = _cid }));
	}



	public void Enqueue(BuildQueueItem op)
	{
		//if (cid == City.build && !SettingsPage.extendedBuild)
		//{
		//	// build immediately
		//	var sb = ZString.CreateUtf8StringBuilder();


		//	sb.Append($"{{\"{cid}\":[");
		//	var qFirst = true;
		//	Serialize(ref sb, op, ref qFirst);
		//	sb.Append("]}");


		//	JSClient.JSInvoke($"buildex(\"{sb.ToString()}\")");
		//	sb.Dispose();

		//	City.Get(cid).BuildingsOrQueueChanged();
		//	return;
		//}

		if(!op.isValid)
		{
			Note.Show("Please don't build a second town hall");
			return;
		}
		CityView.animationOffsets[op.bspot] = AGame.animationT;// * CityView.animationRate; // start animation
															   //if (op.bid == 0)
															   //{
															   //	Trace($"Bad queueop {op}");
															   //	return;
															   //}
		queue.Add(op);
		City.Get(cid).BuildingsOrQueueChanged();
	}

	public void Dispose()
	{
		var _queue = queue;
		queue = null;
		if(_queue != null)
		{
			_queue.Clear();
		}
	}

	public static Span<BuildQueueItem> GetExtendedBuildQueue(int cid)
	{
		if(all.TryGetValue(cid,out var rv))
		{
			return rv.queue.span;
		}
		return Span<BuildQueueItem>.Empty;
	}
}
//public class CityBuildQueueLock : IDisposable
//{
//	public ExtendedQueue cityBuildQueue;

//	public CityBuildQueueLock(ExtendedQueue cityBuildQueue)
//	{
//		this.cityBuildQueue = cityBuildQueue;
//	}

//	public void Dispose()
//	{
//		if (cityBuildQueue != null)
//		{
//			cityBuildQueue=null;
//			ExtendedQueue.queueLock.Release();
//		}
//	}
//}
public static class BuildQueue
{
	static System.Timers.Timer saveTimer;
	public static byte buildActionCounter; // needs to be saved to disc

	public static bool needSave;
	//	public AsyncLock queueLock = new(1,1);
	public static void SaveNeeded()
	{
		needSave = true;
		buildActionCounter = 3;
	} //		public static Utf8ValueStringBuilder commandBuilder = ZString.CreateUtf8StringBuilder();

	public static StorageFolder folder => ApplicationData.Current.LocalFolder;
	//static string fileName => $"buildQueue{JSClient.world}_{Player.myName}.json";

	public static bool initialized => saveTimer != null;
	public static async Task Enqueue(this int cid,BuildQueueItem b)
	{
		try
		{
			var a = b;
			if(a.elvl > a.slvl)
				Assert(a.elvl == a.slvl + 1);
			if(a.elvl < a.slvl && a.elvl != 0)
			{
				if(a.elvl != a.slvl - 1)
				{
					Note.Show($"Bad downgrade? {a.slvl} => {a.elvl}");
					Assert(false);
					return;
				}
			}
			Assert(initialized);
			var op = new BuildQueueItem(a.slvl,a.elvl,a.bid,a.bspot);
			

			if(a.bid == City.bidTemple)
			{
				if(a.slvl == 0)
				{
					Assert(cid == City.build);
					await JSClient.JSInvokeTask($"buildTemple({a.bspot.ToString()})");
				}
				else
				{
					Log("Invalid temple op");

				}
				cid.AsCity().BuildingsOrQueueChanged();
				return;
			}
			if(a.bid == City.bidCastle && a.isBuild)
			{
				Assert(cid == City.build);
				var result = await AppS.DispatchOnUIThreadTask(async () =>
			   {

				   var dialog = new ContentDialog()
				   {
					   Title = $"Castle {City.GetOrAddCity(cid).nameMarkdown}?",
					   Content = "Building a Castle allows you to plunder, scout, assault and siege other cities, but it also makes your own city vulnerable to attacks from other players. Building a Castle will exponentially increase the maximum army size of your city, and is an irreversible action.",
					   PrimaryButtonText = "Yes",
					   SecondaryButtonText = "Cancel"
				   };
				   return await dialog.ShowAsync2();
			   });

				if(result != ContentDialogResult.Primary)
				{
					return;
				}

			}
			var cityBuildQueue = ExtendedQueue.Get(cid);
			{
				if(cityBuildQueue.queue.CanGrow())
				{
					cityBuildQueue.Enqueue(op);
					City.Get(cid).BuildingsOrQueueChanged();
					SaveNeeded();
					// if it is waiting, wake it up
					if(cityBuildQueue.cancellationTokenSource != null)
						cityBuildQueue.cancellationTokenSource.Cancel();  // unblock on queue

					// wake it up
					cityBuildQueue.Process(300);

					//if (a.bid == City.bidCastle && a.slvl == 0)
					//{
					//	await Task.Delay(200);
					//}
				}
				else
				{
					Assert(false);
				}
			}

		}
		catch(Exception __ex)
		{
			Debug.LogEx(__ex);
		}


	}



	public static async Task ClearQueue(int cid = -1)
	{
		if(cid == -1)
			cid = City.build;
		if(ExtendedQueue.all.TryGetValue(cid,out var q))
		{
			using(var _lock = await q.queueLock.LockAsync())
			{
				q.queue.ClearKeepBuffer();
			}
			SaveNeeded();
			City.Get(cid).BuildingsOrQueueChanged();
		}
	}
	//public static async Task SaveIfNeeded()
	//{
	//	if (!initialized)
	//		return;
	//	await SaveTimerGo(true, true);
	//}

	// Where should this go?
	static internal void ShutDown()
	{
		if(!initialized)
			return;
		var _saveTimer = saveTimer;
		saveTimer = null;
		_saveTimer.Stop();

		SaveAll(true,false).Wait();

	}

	private static string fileName => $"buildQueue{JSClient.world}_{Player.myName}.json";
	static internal async Task SaveAll(bool force, bool async)
	{

		if(!initialized)
			return;
		if (!force && !BuildQueue.needSave)
			return;
		try
		{
			Log("SaveQ");
			// must lock them all
			if(ExtendedQueue.all.Any())
			{
				IDisposable[] locks = null;
				try
				{
				if (async)
				{
					await ExtendedQueue.processLock.LockAsync();

						var all = ExtendedQueue.all.ToArray();
						;


						locks = new IDisposable[all.Length];


						for (int i = 0; i < all.Length; ++i)
						{

							Log($"SaveQWait {i}");
							locks[i] = await all[i].Value.queueLock.LockAsync();

						}
					
					
				}


				Log("SaveQWait1");

					var str = Serialize();

						LocalFiles.Write(fileName,str);
						BuildQueue.needSave = false;
				}
				catch(Exception ex)
				{
					LogEx(ex);
					SaveNeeded();
				}
				finally
				{
					Log("SaveQDoneA");
					if (async)
					{
						ExtendedQueue.processLock.Release();
						;

						Log("SaveQDone0");
						if (locks != null)
						{
							foreach (var i in locks)
							{
								if (i != null)
									i.Dispose();
							}
						}

						Log("SaveQDone1");
					}
				}
				//Log($"SaveQueue: {str}");


			}

		}
		catch(Exception __ex)
		{
			Debug.LogEx(__ex);
		}
	}

	

	static internal void SaveTimer_Tick(object sender,System.Timers.ElapsedEventArgs e)
	{
		SaveTimerGo(false,false);
	}
	static internal async Task SaveTimerGo(bool force,bool awaitContext)
	{

		//if (force)
		buildActionCounter = 0;
		//	else
		//	--buildActionCounter;
		//	if (buildActionCounter > 0)
		//	return;

		try
		{
			Post.Get("overview/bcounc.php",onlyHeaders: true);
			//if(buildActionCounter == 0)
			//{
			//	return;
			//}
			await SaveAll(false,true);
		}
		catch(Exception ex)
		{
			LogEx(ex);
			SaveNeeded(); // something went wrong, try again later
		}

	}


	//private static async void SaveBuildQueue(string str)
	//{
	//	if (JSClient.isSub)
	//		return;

	//	var file = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
	//	if (file)
	//	{
	//		await FileIO.WriteTextAsync(file, str);
	//	}
	//}
	private static async Task<string> LoadBuildQueue()
	{
		if(JSClient.isSub)
			return string.Empty;

		try
		{
			var file = await folder.CreateFileAsync(fileName,CreationCollisionOption.OpenIfExists);
			if(file != null)
			{
				return await FileIO.ReadTextAsync(file);
			}
		}
		catch(Exception e)
		{
			LogEx(e);
		}
		return string.Empty;
	}

	/*
			{"10813767":[[318,451,0,0],[318,445,0,1],[319,451,0,0],[319,445,0,1],[320,445,0,1],[321,451,0,0],[342,445,0,1],[363,454,0,0],[363,445,0,1],[359,452,0,0],[359,445,0,1],[380,452,0,0],[405,452,0,0],[405,445,0,1]],"16646480":[[165,449,0,1],[142,449,0,1],[143,449,0,1],[144,449,0,1],[123,449,0,1],[101,449,0,1],[102,449,0,1],[103,449,0,1],[79,449,0,1],[80,449,0,1],[59,449,0,1],[60,449,0,1],[38,449,0,1],[82,449,0,1]],"16515413":[[359,445,0,1],[402,445,0,1],[403,445,0,1],[404,451,0,0],[404,445,0,1],[405,445,0,1],[384,445,0,1]],"10813771":[[100,445,0,1],[92,446,9,0],[121,445,0,1],[93,446,9,0],[142,445,0,1],[36,453,0,0],[60,452,0,0],[123,452,0,0],[98,451,0,0],[106,446,9,0],[36,445,0,1],[107,446,9,0],[98,445,0,1],[108,446,9,0],[60,445,0,1],[109,446,9,0],[123,445,0,1],[37,445,0,1],[110,446,9,0],[38,445,0,1],[127,446,9,0],[58,445,0,1],[128,446,9,0],[79,445,0,1],[129,446,9,0],[99,445,0,1],[130,446,9,0],[101,445,0,1],[131,446,9,0],[81,445,0,1],[148,446,9,0],[102,445,0,1],[149,446,9,0],[144,445,0,1],[150,446,9,0],[164,445,0,1],[170,446,9,0],[163,445,0,1],[171,446,9,0],[35,445,0,1],[172,446,9,0],[56,445,0,1],[192,446,9,0],[77,445,0,1],[193,446,9,0],[119,445,0,1],[244,445,0,1],[117,547,0,1]],"10944843":[[38,445,0,1],[55,446,10,0],[35,445,0,1],[56,446,10,0],[37,445,0,1],[64,446,10,0],[56,445,0,1],[66,446,10,0],[57,483,0,1],[68,446,10,0],[58,445,0,1],[77,446,10,0],[77,445,0,1],[78,453,0,0],[69,446,10,0],[78,483,0,1],[70,446,10,0],[79,445,0,1],[71,446,10,0],[80,483,0,1],[75,446,10,0],[60,445,0,1],[81,454,0,0],[85,446,10,0],[81,445,0,1],[87,446,10,0],[98,445,0,1],[89,446,10,0],[99,445,0,1],[90,446,10,0],[100,445,0,1],[95,446,10,0],[101,445,0,1],[102,454,0,0],[96,446,10,0],[102,445,0,1],[106,446,10,0],[119,445,0,1],[107,446,10,0],[120,483,0,1],[109,446,10,0],[121,445,0,1],[110,446,10,0],[122,483,0,1],[128,446,10,0],[123,445,0,1],[142,451,0,0],[129,446,10,0],[142,445,0,1],[143,451,0,0],[130,446,10,0],[143,483,0,1],[131,446,10,0],[144,445,0,1],[163,451,0,0],[150,446,10,0],[163,445,0,1],[151,446,10,0],[164,445,0,1],[169,446,10,0],[256,449,6,0],[235,449,6,0]]}
	*/

	static public void Serialize(ref Utf8ValueStringBuilder sb,in BuildQueueItem q,ref bool qFirst)
	{
		if(qFirst)
		{
			qFirst = false;
		}
		else
		{
			sb.Append(',');
		}
		sb.Append($"[{q.bspot},{q.bid},{q.slvl},{q.elvl}]");
	}


	static public async Task<bool> IssueCommand(BuildQueueItem q,int cid,long t0)
	{
		var json = $"{{\"bt\":{(q.isBuild ? 0 : q.isDemo ? 3 : q.isUpgrade ? 1 : 2)},\"pa\":{0},\"slvl\":{q.slvl},\"elvl\":{q.elvl},\"bid\":\"{AMath.RandomDigits(10)}\",\"brep\":{q.bid},\"ds\":{t0},\"de\":{t0+1000},\"btype\":{q.proto},\"bspot\":{q.bspot}}}";
		var encoded = Aes.Encode(json,$"X2U11s33S{World.CidToPlayerOrMe(cid)}ccJx1e2");
		//var encoded = Aes.Encode(json, $"XTR977sW{World.CidToPlayer(cid)}sss2x2");
		var args = $"cid={cid}&a=" + HttpUtility.UrlEncode(encoded,Encoding.UTF8);
		var response = await Post.SendForJson("includes/nBuu.php",args);
		if (response == null)
		{
			Assert(false);
			return false;
		}

		City.Get(cid).LoadCityData(response.RootElement);

		return true; ;

		//if(process)
		//{
		//	await new BuildEx(json,cid).Post(false);

		//}
		//else
		//{
		//	return new BuildEx(json,cid).Send(false);
		//}
		//			Assert(rv==true);
		//if(qFirst)
		//{
		//	qFirst = false;
		//}
		//else
		//{
		//	sb.Append(',');
		//}
		//sb.Append($"[{q.bspot},{q.bid},{q.slvl},{q.elvl}]");
	}


	static public string Serialize()
	{
		var sb = ZString.CreateUtf8StringBuilder();
		sb.Append("{");
		bool isFirst = true;

		foreach(var city in ExtendedQueue.all.Values)
		{
			if(isFirst)
			{
				isFirst = false;
			}
			else
			{
				sb.Append(',');
			}
			sb.Append($"\"{city.cid}\":[");
			//	Log($"{city.cid} {city.queue.count}");
			var qFirst = true;
			foreach(var q in city.queue)
			{
				Serialize(ref sb,q,ref qFirst);
			}
			sb.Append(']');
		}
		sb.Append('}');
		var rv = sb.ToString();
		//	Log(rv);
		return rv;

	}
	public static async void Initialize()
	{

		while(!World.initialized)
		{
			await Task.Delay(500);
		}

		if(initialized)
		{
			Assert(false);
			return;
		}
		try
		{
			var data = await LoadBuildQueue();
			if(!data.IsNullOrEmpty())
			{
				//	await CityBuildQueue.throttle.WaitAsync();
				// Todo:  Ensure no building occurs yet
				// Load from JSon, too bad we can't do UTF8
				try
				{

					var js = JsonDocument.Parse(data);
					foreach(var jsCity in js.RootElement.EnumerateObject())
					{
						var cid = int.Parse(jsCity.Name);
						if(!cid.AsCity().isMine)
							continue;
						var cq = ExtendedQueue.Get(cid);
						{
							if(cq!=null)
							{
								foreach(var d in jsCity.Value.EnumerateArray())
								{
									if(cq.queue.CanGrow())
									{

										var op = new BuildQueueItem(d[2].GetByte(),d[3].GetByte(),d[1].GetInt16(),d[0].GetInt16());
										cq.Enqueue(op);
									}
									else
									{
										Assert(false);
									}
								}
								Log($"{cid} {cq.queue.count}");
								cq.Process(AMath.random.Next(3 * 1024,6 * 1024)); // wait 1 - 3 seconds.
							}
							else
							{

							}
						}
					}
				}
				finally
				{
					//	CityBuildQueue.throttle.Release();
				}

			}
		}
		catch(Exception __ex)
		{
			Debug.LogEx(__ex);

		}

		saveTimer = new System.Timers.Timer(3*60*1000);
		saveTimer.Elapsed+= SaveTimer_Tick;
		saveTimer.AutoReset = true;
		saveTimer.Start();
		saveTimer.Enabled=true;
	}


}
public static class CityHelpers
{
	public static Span<BuildQueueItem> GetExtendedBuildQueue(this City city) => ExtendedQueue.GetExtendedBuildQueue(city.cid);

}


