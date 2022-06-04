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
		ShellPage.instance.focus.Content = Spot.GetOrAdd(focus).nameAndRemarks;
		ShellPage.instance.coords.Text   = focus.ToString();
	}

	



	public static void SetFocus(int cid, bool scrollintoView, bool select = true, bool bringIntoView = true, bool lazyMove = true)
	{
		var changed = cid != focus;
		var spot    = Spot.GetOrAdd(cid);
		if(select)
			spot.SelectMe(false, AppS.keyModifiers, scrollintoView);
		if(changed)
		{
			focus = (WorldC)cid;
			AppS.QueueOnUIThread(UpdateFocusText);
		}
		if(bringIntoView)
			cid.BringCidIntoWorldView(lazyMove);
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

