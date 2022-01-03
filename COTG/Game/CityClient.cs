using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CnV
{
	using Windows.System;
	using Microsoft.UI.Input;
	using Microsoft.UI.Xaml;

	public static class CityClient
	{
		//public static async void ProcessClick(this City me,string column, PointerPoint pt, UIElement uie, VirtualKeyModifiers modifiers)
		//{
		//	var cid = me.cid;
		//	modifiers.UpdateKeyModifiers();
		//	//    Note.Show($"{this} {column} {pt.Position}");

		//	if(pt.Properties.IsLeftButtonPressed && !(modifiers.IsShiftOrControl())) // ignore selection style clicks
		//	{


		//		// If we are already selected and we get clicked, there will be no selection chagne to raids are not scanned automatically
		//		//	var wantRaidingScan = (City.CanVisit(cid) && MainPage.IsVisible());
		//		var wantRaidScan = me.isFocus;
		//		//                var needCityData = 
		//		var wantSelect = true;
		//		var wantClick = false;
		//		switch(column)
		//		{
		//			case nameof(me.nameAndRemarks):
		//				// first click selects
		//				// second acts as coord click
		//				if(IsSelected(cid))
		//				{
		//					ProcessCoordClick(cid, false, modifiers, false);
		//					wantRaidScan = false;
		//				}
		//				break;
		//			case nameof(City.bStage):
		//				DoTheStuff();
		//				break;
		//			case nameof(xy):
		//				ProcessCoordClick(cid, false, modifiers, false);
		//				wantRaidScan = false;
		//				break;
		//			case nameof(icon):
		//				if(!await DoClick()) return;
		//				wantSelect = false;
		//				wantRaidScan = false;
		//				break;
		//			case "+":
		//				{
		//					ShowDungeons();
		//					wantRaidScan = false;
		//					wantSelect = false;
		//					break;
		//				}
		//			case nameof(City.tsTotal):
		//				if(City.CanVisit(cid) && MainPage.IsVisible())
		//				{
		//					Raiding.UpdateTS(true, true);
		//				}

		//				wantRaidScan = false;
		//				break;
		//			case nameof(tsHome):
		//			case nameof(tsRaid):
		//				if(City.CanVisit(cid) && MainPage.IsVisible())
		//				{
		//					Raiding.UpdateTS(true, true);
		//				}
		//				wantRaidScan = false;
		//				break;
		//			case nameof(City.raidReturn):
		//				if(City.CanVisit(cid) && MainPage.IsVisible())
		//				{
		//					Raiding.ReturnFast(cid, true);
		//				}
		//				wantRaidScan = false;
		//				break;
		//			case nameof(pinned):
		//				var newSetting = !pinned;

		//				SetPinned(newSetting);

		//				return;
		//			case nameof(City.AutoWalls):
		//				(this as City).AutoWalls = !(this as City).autoWalls;
		//				return;
		//			case nameof(City.AutoTowers):
		//				(this as City).AutoTowers = !(this as City).autoTowers;
		//				return;
		//			case nameof(City.raidCarry):
		//				if(City.CanVisit(cid) && MainPage.IsVisible())
		//				{
		//					Raiding.ReturnSlow(cid, true);
		//				}
		//				wantRaidScan = false;
		//				break;
		//			default:
		//				wantRaidScan = true;
		//				break;
		//		}


		//		//if (MainPage.IsVisible() && isMine && wantRaidScan)
		//		//{
		//		//	//                MainPage.SetRaidCity(cid,true);
		//		//	ScanDungeons.Post(cid, true, false);
		//		//}
		//		if(wantSelect)
		//			SetFocus(false, true, true, false);
		//		NavStack.Push(cid);

		//	}
		//	else if(pt.Properties.IsRightButtonPressed)
		//	{
		//		if(!modifiers.IsShiftOrControl())
		//			SetFocus(false, true, true);
		//		ShowContextMenu(uie, pt.Position);

		//	}
		//	//else if (pt.Properties.IsMiddleButtonPressed)
		//	//{
		//	//	var text = ToTsv();
		//	//	Note.Show($"Copied to clipboard: {text}");
		//	//	AppS.CopyTextToClipboard(text);
		//	//	SetFocus(false);
		//	//}
		//	else
		//	{
		//		// if shift or conntrol is pressed normal processing takes place
		//	}
		//	SpotTab.TouchSpot(cid, modifiers);
		//}
	}
}
