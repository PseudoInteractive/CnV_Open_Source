namespace CnV;

public partial class City
{
	internal List<(int x, int y)> FindOverlayBuildingsOfType(int bid, int max = 100)
	{
		List<(int x, int y)> rv = new();
		for(var cy = span0;cy <= span1;++cy)
		{
			for(var cx = span0;cx <= span1;++cx)
			{
				if(rv.Count > max)
					return rv;

				var c = (cx, cy);
				if(bid == this.GetLayoutBid(c))
				{
					rv.Add(c);

				}
			}
		}
		return rv;
	}

	public bool leaveMe => HasTag(Tags.LeaveMe);

	//public async Task<BuildInfo> GetBuildStage()
	//{
	//	if(leaveMe)
	//		return new BuildInfo(BuildStage.leave, 100);
	//	if (CityRename.IsNew(this))
	//		return new BuildInfo(BuildStage._new, 100);
	//	//await GetCity.Post(cid);
	//	var cabinLevel = await GetAutobuildCabinLevel();
	//	return GetBuildStage(GetBuildingCounts(cabinLevel));

	//}
	public bool NeedsSorcTower(BuildingCount bc)
	{
		return ((bc.sorcTowers == 0 || bc.sorcTowerLevel != 10) && tsTotal > SettingsPage.tsForSorcTower && HasOverlayBuildingOfType(bidSorcTower));
	}
	public bool NeedsCastle(BuildingCount bc)
	{
		return ((!bc.hasCastle) && tsTotal > SettingsPage.tsForCastle && HasOverlayBuildingOfType(bidCastle));
	}

	public async Task MoveStuffLocked()
	{
		Note.Show($"Move slots: {Player.moveSlots}");


		var initialMoveSlots = Player.moveSlots;
		var nextMoveConfirm  = initialMoveSlots - QueueTab.movesPerConfirm;

		var result = await AppS.DoYesNoBox("Move Stuff", "Whould you like to demo resources where buildings should go?", cancel: "Don't Move", no: "Move Stuff", yes: "Move+Demo");
		if (result == -1)
			return;
		var allowDemo = result == 1;

		for (int bad = 0; bad < 16; ++bad)
		{
			var hasChanges = false;
			for (int pass = 0; pass < 2; ++pass)
			{
				for (int r = 1; r <= City.citySpan; ++r)
				{
					for (var y = -r; y <= r; ++y)
					{
						for (var x = -r; x <= r; ++x)
						{
							if ((x == -r || x == r) || (y == -r || y == r))
							{
								var c  = (x, y);
								var id = XYToId(c);
								if (!IsBuildingSpot(id))
									continue;
									
								var bid = GetLayoutBid(id);
								if (bid == 0)
									continue;
								var bl   = postQueueBuildings[id];
								var pbid = bl.bid;
								if (pbid == bid)
									continue;
								if (pbid == 0 || (bl.isRes && allowDemo))
								{
									var spare = FindSpare(bid, false);
									if (spare != 0)
									{
										if (bl.isRes)
										{
											await Demolish(c, false);
											await Task.Delay(500);
											hasChanges = true;
										}
										if (spare > 0)
										{
											if (!await MoveBuilding(spare, id, false))
											{
												goto error;
											}
											hasChanges = true;
											if (Player.moveSlots < nextMoveConfirm)
											{
												nextMoveConfirm = (Player.moveSlots - QueueTab.movesPerConfirm).Max(3);
												if (await AppS.DoYesNoBox("Move Stuff", $"{initialMoveSlots - Player.moveSlots} moves so far, {Player.moveSlots} moves left, continue?") != 1)
													goto done;
											}
										}
									}

								}
								//	else if (pass == 1)
								//	{
								//		var spare = CityBuild.FindAnyFreeSpotForMove(id);
								//		if (!await MoveBuilding(id, spare, false)) 
								//		{

								//			goto error;
								//		}
								//		hasChanges = true;
								//		break;
								//}
							}


						}

					}
				}
				if (hasChanges)
					break;
			}
			if (!hasChanges)
				break;

		}

		done:

		Note.Show($"Final Move slots: {Player.moveSlots}");
		return;
		error:
		await AppS.DoYesNoBox("Move Stuff", "Something did not move right.  Maybe a race condition?  Maybe try again to continue", "Okay", null);

	}
	public string bStage
	{
		get
		{
			if (IsNew())
				return "New";
			if (buildings == Emptybuildings)
			{
				//GetCity.Post(cid);
				return "pending...";
			}
			if (wantSorcTower)
				return "WantSorc";
			if (wantCastle)
				return "WantCastle";

			return buildStage.AsString();
		}
	}

	//public BuildInfo GetBuildStageNoFetch()
	//{
	//	if (leaveMe)
	//		return new BuildInfo(BuildStage.leave, 100);
	//	if (CityRename.IsNew(this))
	//		return new BuildInfo(BuildStage._new, 100);
	//	if (buildings == Emptybuildings)
	//	{
	//		//GetCity.Post(cid);
	//		return new BuildInfo(BuildStage.pending, 100);
	//	}
	//	return GetBuildStage(GetBuildingCounts(GetAutobuildCabinLevelNoFetch()));
	//}

	//public static async Task<BuildInfo> GetBuildBuildStage(BuildingCount bc)
	//{
	//	var city = GetBuild();
	//	if (city.leaveMe)
	//		return new BuildInfo(BuildStage.leave, 100);
	//	if (CityRename.IsNew(city))
	//		return new BuildInfo(BuildStage._new, 100);
	////	await GetCity.Post(City.build);

	//	return city.GetBuildStage(bc);
	//}

	//public static async Task<BuildInfo> GetBuildBuildStage()
	//{
	//	var city = GetBuild();
	//	if (city.leaveMe)
	//		return new BuildInfo(BuildStage.leave, 100);

	//	if (CityRename.IsNew(city))
	//		return new BuildInfo(BuildStage._new, 100);
	//	await GetCity.Post(City.build);
	//	var cabinLevel = await city.GetAutobuildCabinLevel();
	//	return city.GetBuildStage(GetBuildingCountPostQueue(cabinLevel));

	//}

	public bool HasOverlayBuildingOfType(int bid)
	{
		if (!isLayoutCustom)
			return false;

		for (var id = 0; id < City.citySpotCount; ++id)
		{
			if (bid == GetLayoutBid(id))
				return true;

		}
		return false;
	}
	public (int x, int y) FindOverlayBuildingOfType( int bid)
	{
		if (!isLayoutCustom)
			return (0,0);
			
		for (var id = 0;id <City.citySpotCount; ++id)
		{
			if (bid == GetLayoutBid(id))
				return IdToXY(id);

		}
		return (0, 0);
	}
	public bool hasExtraBuildings => FindExtraBuilding() != -1;

	public int FindExtraBuilding()
	{
		if (!isLayoutCustom)
			return -1;
		Dictionary<short, short> counts = new();


		// first collect counts
		foreach(var id in buildingSpotsLandLocked)
		{
			
			var bid = (short)GetLayoutBid(id);
			if (bid != 0 && bid != bidTemple )
			{
				if (counts.TryGetValue(bid, out var c) == false)
				{
					c = 1;
					counts.TryAdd(bid, c);
				}
				else
				{
					++c;
					counts[bid] = c;
				}
			}

			var bl = postQueueBuildings[id];
			Assert(!bl.isTower);
			if (!(bl.isRes || bl.isEmpty || bl.isTemple || bl.isCabin || bl.isTower ))
			{
				bid = bl.bid;
				if (counts.TryGetValue(bid, out var c) == false)
				{
					c = -1;
					counts.TryAdd(bid, c);
				}
				else
				{
					--c;
					counts[bid] = c;
				}

			}
		}
		int rv        = -1;
		int bestLevel = int.MaxValue;
		for (var id = 1; id < City.citySpotCount - 1; ++id)
		{
			if (!IsBuildingSpot(id))
				continue;

			var oBid = (short)GetLayoutBid(id);
			var bl   = postQueueBuildings[id];
			if (!(bl.isRes || bl.isEmpty || bl.isTemple || bl.isCabin || bl.isTower || bl.isCastle))
			{
				var bid = (short)bl.bid;
				if (bid == oBid)
					continue;
				if (counts[bid] < 0)
				{
					Assert(bid != bidCastle);
					if (bl.bl < bestLevel)
					{
						bestLevel = bl.bl;
						rv        = id;
					}
				}


			}
		}
		return rv;
	}

	public bool hasCastleInLayout
	{
		get
		{
			if (!isLayoutCustom)
				return false;

			for (int i = 0; i < citySpotCount; ++i)
			{
				if (GetLayoutBid(i) == bidCastle)
					return true;
			}
			return false;
		}
	}
	public (bool hasCastle, bool hasSorcTower) hasCastleOrSorcTowerInLayout
	{
		get
		{
			bool hasCastle    = false;
			bool hasSorcTower = false;
			if (!isLayoutCustom)
				return (false, false);

			for (int i = 0; i < citySpotCount; ++i)
			{
				if (GetLayoutBid(i) == bidCastle)
					hasCastle = true;
				else if (GetLayoutBid(i) == bidSorcTower)
					hasSorcTower = true;
			}
			return (hasCastle, hasSorcTower);
		}
	}


	public void ClearRes()
	{
		App.DispatchOnUIThreadExclusive(cid,async () =>
											{
												await ClearResUI();
											});
	}
	public  async Task ClearResUI()
	{
		var asked = false;
		if(isLayoutCustom)
		{
			for (int r = 1; r <= City.citySpan; ++r)
			{
				for (var y = -r; y <= r; ++y)
				{
					for (var x = -r; x <= r; ++x)
					{
						if ((x == -r || x == r) || (y == -r || y == r))
						{
							var c = (x, y); // (int x, int y) c = RandCitySpot();
							if (!c.IsXYInCenter() && SettingsPage.clearOnlyCenterRes)
								continue;
							if (GetLayoutBid(c) == 0)
								continue;
								
							if (postQueueBuildings[City.XYToId(c)].isRes )
							{
								if(asked ==false)
								{
									asked = true;
									if(await AppS.DoYesNoBox("Clear Res?","Would you like to clear out the resources?") != 1)
									{
										return;
									}
								}
								await Demolish(City.XYToId(c), false);
							}
						}
					}
				}
			}
		}
		else
		{
			await JSClient.ClearCenter(cid);
		}
	}

	public bool IsLayoutComplete(City city)
	{
		try
		{
			if (!isLayoutCustom)
				return true;
			var bds = isBuild ? city.postQueueBuildings : buildings;
			for (var id = 1; id < City.citySpotCount; ++id)
			{
				if (!IsBuildingSpot(id))
					continue;

				var bid = GetLayoutBid(id);
				if (bid != 0)
				{
					var bl = bds[id];
					if (bl.bid != bid)
						return false;
				}

			}
		}
		catch(Exception ex)
		{
			LogEx(ex);
		}

		return true;
	}

	internal bool IsAllyOrNap()
	{
		return Alliance.IsAllyOrNap(allianceId);
	}

		
}