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
	public static void ProcessClick(int cid,ClickModifiers clickMods)
	{
		if(cid == Spot.cidNone) {
			Assert(false);
			return;
		}
		var changed = cid != focus;
		var spot    = Spot.GetOrAdd(cid);
		if(changed)
		{
			focus = (WorldC)cid;
			AppS.QueueOnUIThread(UpdateFocusText);
		}
		if(clickMods.HasFlag(ClickModifiers.bringIntoWorldView) && !clickMods.IsShiftOrControl()) {
			cid.BringCidIntoWorldView(clickMods.HasFlag(ClickModifiers.bringIntoWorldViewLazy),clickMods.IsRight());
			if(clickMods.IsRight()) {
				if( cid != City.build && CanVisit(cid) ) {
					cid.AsCity().Visit();
				}
			}
		}
		SpotTab.AddToGrid(spot);
		
		if(clickMods.HasFlag(ClickModifiers.select) | clickMods.IsShiftOrControl() )
			ProcessSelection(spot,clickMods);

		if(clickMods.HasFlag(ClickModifiers.scrollIntoUiView)) {
			CityUI.ScrollIntoView(cid);
		}
		NavStack.Push(cid);
		if( !(clickMods.IsShiftOrControl()|clickMods.IsRight()|clickMods.HasFlag(ClickModifiers.noFlyout) ))
				cid.AsCity().ShowContextMenu();

	}

	public static bool IsNearFocus(this WorldC cid) {
		var thresh = 0.5f;
		// only move if moving more than about 1 city span
		return (System.Numerics.Vector2.Distance(View.viewTargetW2,cid.v) <= thresh);

	}
	public static bool IsOnScreen(this WorldC cid) {
		return View.cullWCF.Contains( cid.v);
	}
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

