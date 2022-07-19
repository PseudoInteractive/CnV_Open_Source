using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static CnV.City;
namespace CnV;


public static partial class CityUI
{
	public static void UpdateFocusText()
	{
		PlayerStats.instance?.focus.SetCity(Spot.GetFocus(),false);
	}

	


	public static void ProcessClick(this City c,ClickModifiers clickMods) {

		ProcessClick(c.cid,clickMods);
	}
	static bool lastFocusViewToggle;
	public static void ProcessClick(int cid,ClickModifiers clickMods)
	{
		if(cid == Spot.cidNone) {
			Assert(false);
			return;
		}
		var wantFlyout = (!(clickMods.IsShiftOrControl()|clickMods.IsRight()|clickMods.HasFlag(ClickModifiers.noFlyout)));
		var spot = Spot.GetOrAdd(cid);
		bool focusChanged = false;
		if(clickMods.HasFlag(ClickModifiers.setFocus)) {
			focusChanged= cid != focus;

			if(focusChanged) {
			
					lastFocusViewToggle = false;
				focus = (WorldC)cid;
				AppS.QueueOnUIThread(UpdateFocusText);
			}
		}

		if(!wantFlyout | clickMods.HasFlag(ClickModifiers.bringIntoWorldView)) {
		
			var moved = cid.BringCidIntoWorldView(clickMods.HasFlag(ClickModifiers.bringIntoWorldViewCenter) );
			if(!moved && clickMods.HasFlag(ClickModifiers.autoToggleView)) {
				// Toggle
				switch(View.viewMode) {
				
					case ViewMode.world:
							lastFocusViewToggle = false;
							View.SetViewMode( ViewMode.region,cid,clickMods.HasFlag(ClickModifiers.bringIntoWorldViewCenter));
						break;
					case ViewMode.region:
							if(City.CanVisit(cid)) {
								View.SetViewMode(lastFocusViewToggle ?  ViewMode.world : ViewMode.city,cid,true);
							}
							else if(View.IsNearFocus(new(cid)))
								View.SetViewMode(ViewMode.world,cid,false);
							else
								View.EnsureInView(cid,true);

						break;
					default:  // view mode city
						lastFocusViewToggle  = true;
						View.SetViewMode(ViewMode.region ,cid,false);
						break;
				}

			}
		
		}
		//if(clickMods.HasFlag(ClickModifiers.bringIntoWorldView) && !clickMods.IsShiftOrControl()) {
		//	//var lazy = clickMods.HasFlag(ClickModifiers.bringIntoWorldViewLazy);
		//	cid.BringCidIntoWorldView( );
		//	//if(clickMods.IsRight()) {
		//	//	if( cid != City.build && CanVisit(cid) ) {
		//	//		cid.AsCity().Visit();
		//	//	}
		//	//}
		//}
		if(clickMods.HasFlag(ClickModifiers.addToMru))
			SpotTab.AddToGrid(spot);
		
		if(clickMods.HasFlag(ClickModifiers.select) | clickMods.IsShiftOrControl() )
			ProcessSelection(spot,clickMods);

		if(clickMods.HasFlag(ClickModifiers.scrollIntoUiView)) {
			CityUI.ScrollIntoView(cid);
		}
		NavStack.Push(cid);
		if( wantFlyout)
				cid.AsCity().ShowContextMenu();

	}

	//private static void ToggleView(int cid) {
	//	View.SetViewMode(View.viewMode.GetNext(cid),cid);
	//	View.TrySetViewTarget(TODO);
	//}

	
	//public static  async void SelectInWorldView(this City me, bool lazyMove)
	//{
	//	var cid = me.cid;
	//	if(!me.isBuild)
	//	{
	//		if(!await CnVClient.CitySwitch(cid, lazyMove)) // keep current view, switch to city 
	//			return;
	//	}
	//	if(!View.IsWorldView())
	//		View.SetViewMode(ViewMode.world); // toggle between city/region view

	//	NavStack.Push(cid);

	//}
}

