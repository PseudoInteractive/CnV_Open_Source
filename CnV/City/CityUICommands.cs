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
	public async void Visit() 
		{
		if(isBuild) {
			ProcessCoordClick(cid,AppS.keyModifiers.ClickMods() | ClickModifiers.noFlyout);
		}
		else {
			if(Player.IsSubOrMe(cid.CidToPid()) || (await AppS.DoYesNoBox("Sub",$"Switch to sub {cid.AsCity().player.name}?")==1)) {
				await CnVClient.CitySwitch(cid); // keep current view, switch to city
												 //	View.SetViewMode(ShellPage.viewMode.GetNextUnowned());// toggle between city/region view
			}
		}
	}



	[ACommand("settings","mana","Set trade settings")]
	public void TradeSettings() { TradeSettingsDialog.ShowInstance(this); }

}