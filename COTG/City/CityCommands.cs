using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static CnV.Spot;
using static CnV.City;
using System.Text.Json;
using Microsoft.UI.Xaml;

namespace CnV;


public static partial class CityUI
{
	public static async Task ShowReturnAt(this City me,bool wantDialog)
	{




		//if (!IsBuild(cid))
		{
			var cid = me.cid;
			await AppS.DispatchOnUIThreadExclusive(cid, async () =>
			{

				try
				{
					DateTimeOffset? time = null;
					var ogaStr = await JSClient.ExecuteScriptAsync("getOGA()");
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
							var t = CnVServer.ServerTime();
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
	public static void ReturnAt(object sender, RoutedEventArgs e)
	{
		ShowReturnAt(true);
	}
	public static async void ShowCity(int cityId, bool lazyMove, bool select = true, bool scrollToInUI = true)
	{
		try
		{
			//				ShellPage.SetViewModeWorld();

			// if (City.IsMine(cityId))
			{
				Spot.SetFocus(cityId, scrollToInUI, select, true, lazyMove);
			}

			// if (JSClient.IsWorldView())
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