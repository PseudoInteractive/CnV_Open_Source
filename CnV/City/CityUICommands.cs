using Microsoft.UI.Xaml.Controls;

namespace CnV;

partial class City
{

	public static ACommandCategory ACatCity = new(nameof(ACatCity),"City","zirconia","tbd",null);

	public static ACommandCategory ACatAttack = new(nameof(ACatAttack),"Attack","zirconia","tbd",ACatCity);
	public static ACommandCategory ACatDefense = new(nameof(ACatDefense),"Defense","zirconia","tbd",ACatCity);
	public static ACommandCategory ACatTrade = new(nameof(ACatTrade),"Trade","zirconia","tbd",ACatCity);
	public static ACommandCategory ACatSettings = new(nameof(ACatSettings),"Settings","zirconia","tbd",ACatCity);
	  

	internal bool CanVisitUI { get {
			return !isBuild && CanVisit(cid);

		} }
	[ACommand("city","zirconia","Visit city")]
	public async Task<bool?> Visit() 
	{
		if(!CanVisit(cid)) {
			CityUI.ProcessClick(cid,AppS.keyModifiers.ClickMods(isRight:true,noFlyout: true)) ;
			return true;
		}

		if(isBuild) {
			await ProcessCoordClick(cid,AppS.keyModifiers.ClickMods(isRight: true) );
		}
		else {
			if(Player.IsSubOrMe(cid.CidToPid()) || (await AppS.DoYesNoBox("Sub",$"Switch to sub {cid.AsCity().player.name}?")==1)) {
				return await CnVClient.CitySwitch(cid); // keep current view, switch to city
												 //	View.SetViewMode(ShellPage.viewMode.GetNextUnowned());// toggle between city/region view
			}
		}
		return false;
	}



	[ACommand("settings","mana","Set trade settings")]
	public Task<bool?> TradeSettings() 
	{
		TradeSettingsDialog.ShowInstance(this);
		return AUtil.completedTaskTrueN;
	}

	[ACommand("city","mana","Toggle sected")]
	public Task<bool?> Select(bool ? prior) {
		if(prior is null)
			return AUtil.TaskResult(  (bool?)false );
		isSelected = prior.Value;
		SpotTab.QueueSyncSelectionToUI();
		return AUtil.completedTaskFalseN;
	}

}