namespace CnV;

using Cysharp.Text;

public static partial class World
{
	public static void SetHeatmapDates(ServerTime t0, ServerTime t1)
	{
		Assert(t0.seconds != 0);
		Assert(t1.seconds != 0);
		ServerTime _t0 = t0;
		ServerTime _t1 = t1 + 1;

		//if ( _t0.Date() != t0.Date())
		//{
		//	//	Assert(false);
		//	_t0 = t0;
		//}
		if (World.heatMapT0 == _t0 &&
			World.heatMapT1 == _t1)
			return;

		World.heatMapT0 = _t0;
		World.heatMapT1 = _t1;



		if (World.changeMapInProgress)
		{
			Log($"Heat Busy: {changeMapRequested}, {t0}, {t1} ");

			World.changeMapRequested = true;
			return;
		}

		World.changeMapInProgress = true;

		Log($"Heat Change: {changeMapRequested}, {t0}, {t1} ");

		Task.Run(UpdateChangeMap);
	}

	public static async void UpdateChangeMap()
	{
		if(World.changeMapInProgress != true)
		{
			return;
		}
		try
		{
			using var _ = await HeatMap.mutex.LockAsync();

			//Log("Snapshots");
			var data = await HeatMap.GetSnapshot(World.heatMapT0);
			var data1 = await HeatMap.GetSnapshot(World.heatMapT1);
			//Log("Change string");

			var task = AppS.DispatchOnUIThreadTask(() =>
			{
				if(HeatTab.instance.isFocused)
				{
					if(rawPrior0 == null || (data == null || data1 == null))
					{
						HeatTab.instance.header.Text = "Please select a date range to see changes";
					}
					else
					{
						using var sb = ZString.CreateUtf8StringBuilder();
						sb.AppendFormat("-- {0} => {1} --\n", (heatMapT0).ToString(), (heatMapT1).ToString());
						{
							// find most recently open continent
							for(int c = World.continentCount - 1; --c >= 0;)
							{
								var cidDig = World.continentOpeningOrder[c];
								var cId = cidDig.ContinentToXY();
								var cityCount1 = World.GetContinentCityCount(data1, cId);
								if(cityCount1 <= 0)
									continue;

								if(cityCount1 >= continentCityThreshHold3)
									break;
								// all are open
								var cityCount0 = World.GetContinentCityCount(data, cId);
								var dt = heatMapT1 - heatMapT0;
								var dc = cityCount1 - cityCount0;
								if(cityCount0 == 0)
								{
									sb.AppendFormat("To see continent opening prediction select a later start date (after {0} opened)\n", cidDig);
								}
								else if(dc <= 0 || dt < 60 * 5)
								{
									sb.AppendFormat("Select an earlier start date to see opening prediction for {0}", World.continentOpeningOrder[c + 1]);
								}
								else
								{
									sb.AppendFormat("Predicted opening of {0} (current {1}, rate: {2} cities/day)\n", World.continentOpeningOrder[c + 1], cityCount1, dc * 60 * 60 * 24 / (dt));
									for(int j = 0; j < 4; ++j)
									{
										var target = j switch { 0 => continentCityThreshHold0, 1 => continentCityThreshHold1, 2 => continentCityThreshHold2, _ => continentCityThreshHold3 };
										if(cityCount1 < target)
										{
											var delta = target - cityCount1;
											var sec = ((float)dt * (float)(delta) / dc);
											sb.AppendFormat("{0} cities at {1}", target, heatMapT1 + sec.RoundToInt() );
											sb.AppendFormat(" ({0})\n", sec );
										}
									}
								}
								break;


							}

						}


						var c0 = CityCounts.GetcityCountsByAlliance(data1);
						var c1 = CityCounts.GetcityCountsByAlliance(data);

						foreach(var i in c0)
						{
							var v = i.Value;
							var v1 = c1.GetValueOrDefault(i.Key);
							if(v1 == null)
								v1 = new();
							if(v.total < 10 && v1.total < 10)
								continue;
							var d = v.Sub(v1);
							sb.AppendFormat("{0}: {1} ({2})", Alliance.IdToName(i.Key), v.total, d.total.FormatWithSign());
							if(v.castles > 0)
								sb.AppendFormat(", {0} ({1}) castles", v.castles, d.castles.FormatWithSign());
							if(v.temples > 0)
								sb.AppendFormat(", {0} ({1}) temples", v.temples, d.temples.FormatWithSign());
							if(v.big > 0)
								sb.AppendFormat(", {0} ({1}) big", v.big, d.big.FormatWithSign());
							sb.AppendLine();
						}

						sb.Append("\nChanges");
						var ch = new ChangeInfo().ComputeDeltas(data, data1);
						sb.Append(ch.ToString());
						{
							PlayerChangeTab.changes.Set(ch.players.Values.OrderByDescending(a => a.activity), true);
							var tab = PlayerChangeTab.instance;
							if(!tab.isFocused)
							{
								tab.ShowOrAdd(true, false);
							}
						}



						var str = sb.ToString();
						Log(str);
						HeatTab.instance.header.Text = str;
					}
				}
				Log("Change done");
				return Task.CompletedTask;
			});

			if(data == null || data1 == null)
			{
				ClearHeatmap();
				return;
			}


			Log("Change pixels");
			World.CreateChangePixels(data, data1);
			Log("Change pixels Done");
			await task;
			World.changeMapInProgress = false;
		}
		catch(Exception ex)
		{
			Log(ex);
		}
	}
}