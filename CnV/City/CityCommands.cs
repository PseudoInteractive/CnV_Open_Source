﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static CnV.City;
using System.Text.Json;
using Microsoft.UI.Xaml;

namespace CnV;
using static CnV.View;


public static partial class CityUI
{
	
	public static async void ReturnAtBatch(SpotId cid)
	{
		(var at, var okay) = await Views.DateTimePicker.ShowAsync("Return By:");
		if(!okay)
			return; // aborted
		using var work = new WorkScope("Return At..");

		var cids = Spot.GetSelectedForContextMenu(cid);
		foreach(var _cid in cids)
		{
			var __cid = _cid;
			await Raiding.ReturnAt(__cid, at);
		}
		Note.Show($"End {cids.Count} raids at {at.Format()} ");

	}
	
	public static async void ShowCity(int cityId, bool lazyMove, bool select = true, bool scrollToInUI = true)
	{
		try
		{
			//				ShellPage.SetViewModeWorld();

			// if (City.IsMine(cityId))
			// don't change build
			if(cityId != City.build )
			{
				ShellPage.SetViewModeRegion();

			}
			{
				CityUI.SetFocus(cityId, scrollToInUI, select, true, lazyMove);
			}

			// if (CnVServer.IsWorldView())
			//	cityId.BringCidIntoWorldView(lazyMove, false);

			CnVServer.FetchCity(cityId);
			//             if( City.IsMine(cityId)  )
			//                 Raiding.UpdateTSHome();



		}
		catch(Exception e)
		{
			LogEx(e);
		}


	}

}
/// <summary>
/// These operations use the City class the Client project, which works with shared source projects.
/// As opposed to class libraries, as City is a shared Type.
/// </summary>

public partial class City
{
	public string toolTip
	{
		get {
			var city = this;
			if(isShrine)
				return "Shrine\n(To be named)";
			else if(isPortal)
				return "Moon Gate";
			else if(isDungeon)
				return Cavern.Get(cid).ToString();
													//if (spot is City city)
													{
														using var psb = new PooledStringBuilder();
														var sb = psb.s;
														//	var notes = city.remarks.IsNullOrEmpty() ? "" : city.remarks.Substring(0, city.remarks.Length.Min(40)) + "\n";
														sb.Append(player.shortName)
															.AppendLinePre(city.nameAndRemarks)
														.AppendFormat("\npts:{0:N0}",city.points);
														if(player.allianceId!= 0)
															sb.AppendLinePre(Alliance.IdToName(player.allianceId));
														

														//if(city.senatorInfo.Length != 0)
														{
															city.GetSenatorInfo(sb);
														}
														if(city.incoming.Any(i=>i.isAttack))
														{
															sb.AppendFormat("\n{0} incoming attacks",city.incoming.Count(i=>i.isAttack));

															var incAttacks = 0;
															var incTs = 0u;
															foreach(var i in city.incoming)
															{
																if(i.isAttack)
																{
																	++incAttacks;
																	incTs += i.ts;
																	if(incAttacks<=3)
																	{
																		sb.AppendLinePre(i.troops.Format('\n')); // only show first two
																	}
																	else if(incAttacks ==4)
																	{
																		sb.AppendLinePre("..");
																	}
																}
															}
															if(incTs > 0)
																sb.AppendFormat("\n({0} total TS)",incTs);

															if(city.claim != 0)
																sb.AppendFormat("\n{0}% Claim",city.claim);

															sb.AppendFormat("\n{0} total def",city.tsDefMax);

															sb.Append(city.GetDefString('\n'));

														}
													{
														var outgoingStatus = city.outgoingStatus;
														if(!outgoingStatus.IsNone())
														{
															if(outgoingStatus.IsSeiging())
																sb.Append("\nSieging");
															else if(outgoingStatus.IsSending())
																sb.Append("\nAttack Sent");
															else
																sb.Append("\nAttack Scheduled");
														}
													}
													{
														var reinf = city.reinforcementsIn;
														if(reinf.Any())
														{
															sb.AppendFormat("\n{0} def",city.tsDefMax);
															int counter = 0;
															foreach(var i in reinf)
															{
																sb.Append(i.troops.Format(header: $"\nFrom {City.GetOrAddCity(i.sourceCid).nameAndRemarks}:",separator: '\n'));
																if(++counter >= 4)
																{
																	sb.AppendLinePre("...");
																	break;
																}
															}

														}
													}
														if(!city.notes.IsNullOrEmpty())
															sb.AppendLinePre(city.notes.AsSpan().Wrap(20));
														if(city.hasAcademy.GetValueOrDefault())
															sb.AppendLinePre("Has Academy");
														if(NearRes.IsVisible())
														{
															sb.AppendLinePre($"Carts:{AUtil.FormaRatio((city.cartsHome, city.carts))}");
															if(city.ships > 0)
																sb.AppendLinePre($"Ships:{AUtil.FormatRatio(city.shipsHome,city.ships)}");
															var res = city.sampleResources;
																sb.AppendLinePre($"Wood:{res[0].Format()}, Stone:{ res[1].Format()}");
																sb.AppendLinePre($"Iron:{res[2].Format()}, Food:{ res[3].Format()}");
														}
														sb.Append($"\n{c.y / 100}{c.x / 100} ({c.x}:{c.y})");

														sb.AppendLinePre(city.GetTroopsString("\n"));

														return sb.ToString();

													}

		}
	}
	internal void Show(object sender,RoutedEventArgs e)
	{
		CityUI.ShowCity(cid,false);
	}

	public void ReturnAt(object sender, RoutedEventArgs e)
	{
		ShowReturnAt(true);
	}
	public async Task ShowReturnAt( bool wantDialog)
	{
		AppS.MessageBox("Not yet supported");
		return;



		//if (!IsBuild(cid))
		{
			var cid = this.cid;
			await AppS.DispatchOnUIThreadExclusive(cid, async () =>
			{

				try
				{
					DateTimeOffset? time = null;
					var ogaStr = await CnVServer.ExecuteScriptAsync("getOGA()");
					using var jsDoc = JsonDocument.Parse(ogaStr.Replace("\\\"", "\"").Trim('"'));
					foreach(var i in jsDoc.RootElement.EnumerateArray())
					{
						try
						{
							var type = i[0].GetAsInt();
							if(type == 5) // raid
								continue;
							Trace(type);
							var timing = i[6].GetAsString();
							var id = timing.IndexOf("Departs:");
							if(id == -1)
								continue;
							timing = timing.Substring(id + 9);
							var t = CnVServer.serverTime;
							;
							var today = timing.StartsWith("Today");
							var tomorrow = timing.StartsWith("Tomorrow");
							if(today || tomorrow)
							{
								timing = today ? timing.Substring(6) : timing.Substring(9);
								var hr = int.Parse(timing.Substring(0, 2));
								var min = int.Parse(timing.Substring(3, 2));
								var sec = int.Parse(timing.Substring(6, 2));
								t = new DateTimeOffset(t.Year, t.Month, t.Day, hr, min, sec, TimeSpan.Zero);
								if(tomorrow)
									t += TimeSpan.FromDays(1);
							}
							else
							{
								t = timing.ParseDateTime(true);
							}

							t -= TimeSpan.FromHours(Settings.returnRaidsBias);
							if(time == null || time > t)
								time = t;
						}
						catch(Exception ex)
						{
							LogEx(ex);
						}
					}


					if(wantDialog)
					{
						(time, var okay) = await DateTimePicker.ShowAsync("Return By:", time);
						if(!okay)
							return; // aborted
					}

					if(time != null)
					{
						await Raiding.ReturnAt(cid, time.Value);
						Note.Show($"{City.Get(cid).nameMarkdown} end raids at {time.Value.Format()}");
					}
					else
					{
						Note.Show($"{City.Get(cid).nameMarkdown} no scheduled outgoing");
					}
				}
				catch(Exception ex)
				{
					LogEx(ex);
				}
			});

		}
	}

	
}