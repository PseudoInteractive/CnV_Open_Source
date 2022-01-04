using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static CnV.Spot;
using static CnV.City;
namespace CnV;


public static partial class CityUI
{
	public static void UpdateFocusText()
	{
		ShellPage.instance.focus.Content = Spot.GetOrAdd(focus).nameAndRemarks;
		ShellPage.instance.coords.Text   = focus.CidToString();
	}

	public static void SetFocus(this Spot me,bool scrollIntoView, bool select = true, bool bringIntoWorldView = true, bool lazyMove = true)
	{
		SetFocus(me.cid, scrollIntoView, select, bringIntoWorldView, lazyMove);
	}



	public static void SetFocus(int cid, bool scrollintoView, bool select = true, bool bringIntoView = true, bool lazyMove = true)
	{
		var changed = cid != focus;
		var spot    = Spot.GetOrAdd(cid);
		if(select)
			spot.SelectMe(false, AppS.keyModifiers, scrollintoView);
		if(changed)
		{
			focus = cid;
			AppS.QueueOnUIThread(UpdateFocusText);
		}
		if(bringIntoView)
			cid.BringCidIntoWorldView(lazyMove);
	}
	public static async Task<bool> DoClick(this Spot me)
	{
		var cid = me.cid;
		if(City.CanVisit(cid))
		{
			var wasBuild = City.IsBuild(cid);

			if(!await JSClient.CitySwitch(cid, false, true, false))
				return false;

			if(wasBuild)
			{
				View.SetViewMode(View.viewMode.GetNext());
			}
		}
		else
		{
			CityUI.ShowCity(cid, false, true, false);
		}

		return true;
	}
	public static  async void SelectInWorldView(this City me, bool lazyMove)
	{
		var cid = me.cid;
		if(!me.isBuild)
		{
			if(!await JSClient.CitySwitch(cid, lazyMove)) // keep current view, switch to city 
				return;
		}
		if(!View.IsWorldView())
			View.SetViewMode(ViewMode.world); // toggle between city/region view

		NavStack.Push(cid);

	}
}

