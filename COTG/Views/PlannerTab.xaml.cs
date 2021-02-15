using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using static COTG.Debug;
using static COTG.Game.City;
using COTG.Game;
using COTG.JSON;
using static COTG.Game.Enum;
using COTG.Draw;

namespace COTG.Views
{
	public sealed partial class PlannerTab : UserTab
	{
		public static PlannerTab instance;
		public static bool IsVisible() => instance.isVisible;

		public async override void VisibilityChanged(bool visible)
		{
			if (visible)
			{
				CityBuild._isPlanner = true;
				BuildingsChanged();
			}
			else
			{
				CityBuild._isPlanner = false;

			}
		}

		public PlannerTab()
		{
			Assert(instance == null);
			instance = this;

			this.InitializeComponent();
		}

		public static void UpdateStats()
		{
			if (statsDirty ==false || !IsVisible())
				return;
			statsDirty = false;
			// recruit speeds
			var city = City.GetBuild();
			var bds = buildingsCache;
			var rsInf = 0;
			for (int x = span0; x <= span1; ++x)
			{
				for (int y = span0; y <= span1; ++y)
				{
					var cc = (x, y);
					var spot = XYToId(cc);
					var bd = bds[spot];
					var bid = bd.bid;
					var bdef = BuildingDef.all[bid]; 
					if(bid == bidTrainingground)
					{
						rsInf += bdef.Ts[bd.bl];
						for(int dx=-1;dx<=1;++dx)
							for(int dy=-1;dy<=1;++dy)
							{
								var cc1 = (x + dx, y + dy);
								var bd1 = bds[XYToId(cc1)];
								if (bd1.bid == bidBarracks)
								{
									rsInf += bd1.def.Ts[bd1.bl];
								}

							}

					}
				}
			}
			if(rsInf != 0 )
			{
				rsInf += 100;
				var gain = 100.0/rsInf;
				instance.rsInf.Text = $"{rsInf}%";
				instance.rtVanq.Text = $"{TroopInfo.all[ttVanquisher].Ps*gain:0.00} s";
				instance.rtRanger.Text = $"{TroopInfo.all[ttRanger].Ps * gain:0.00} s";
				instance.rtTriari.Text = $"{TroopInfo.all[ttTriari].Ps * gain:0.00} s";
			}

		}
		static bool statsDirty;

		internal static void BuildingsChanged()
		{
			if (statsDirty == true || !IsVisible() )
				return;
			CityView.BuildingsOrQueueChanged();
			statsDirty = true;
			App.DispatchOnUIThreadLow(UpdateStats);
		}
	}
}
