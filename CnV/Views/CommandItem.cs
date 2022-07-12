using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
//using CommunityToolkit.WinUI.UI;
//using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
//using Expander = CommunityToolkit.WinUI.UI.Controls.cer;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.
using static CnV.CnVEventUseArtifacts;

namespace CnV;

public sealed partial class CityStats
{

	private void CommandListViewItemClick(object _sender,ItemClickEventArgs e) {

		var sender = _sender as UIElement;
		Assert(sender is not null);
		var army = e.ClickedItem as CommandItem;
		Assert(army is not null);
		if(!army.army.isSubOrMe)
			return;
		var flyout = new MenuFlyout();
		army.AddToFlyout(flyout);
		//			flyout.ShowContext(sender,args);
		flyout.ShowAt(sender,new());
	}
	private void CommandItemsClick(object sender,RoutedEventArgs e) {

		var b = sender as Button;
		var flyout = Flyout.GetAttachedFlyout(b) as MenuFlyout ??  new MenuFlyout();
		flyout.Items.Clear();
		var c = city;

		if(c.outgoing.Any(i => i.canReturn)) {
			flyout.AddItem("Return All",Symbol.Delete,() => {
				foreach(var i in c.outgoing) {
					if(!i.canReturn) {
						AppS.MessageBox("One or more armies cannot be returned automatically (too late for outgoing attack?)");
						return;
					}
				}
				new CnVEventReturnTroops(c.c,CnVEventReturnTroops.outgoingAll).EnqueueAsap();
			});
		}

		var anyRaid = c.outgoing.FirstOrDefault(i => i.isRaid && (i.isRepeating || (!i.isReturn)));
		if(anyRaid is not null) {
			flyout.AddItem("Return all raids at..",() => {
				SendTroops.ShowInstance(prior: anyRaid,timing: TimingSetting.arrival,returnAllRaids: true);
			});
			if(c.outgoing.Any(o => o.isAttackOrDefense && !o.departed)) {

				flyout.AddItem("Return raids in time for scheduled attacks or defense",c.ReturnRaidsForScheduled);

			}

			flyout.AddItem("Return all raids immediately",Symbol.Refresh,() => {
				new CnVEventReturnTroops(c.c,CnVEventReturnTroops.outgoingRaidsFast).EnqueueAsap();
			});


			if(c.outgoing.Any(i => i.isRaid && (i.isRepeating))) {
				flyout.AddItem("Return all raids when complete",Symbol.Refresh,() => {
					new CnVEventReturnTroops(c.c,CnVEventReturnTroops.outgoingRaidsSlow).EnqueueAsap();
				});
			}
			if(c.outgoing.Any(i => i.isRaid && (!i.isRepeating))) {
				flyout.AddItem("Resume all raids",Symbol.Play,() => {
					new CnVEventReturnTroops(c.c,CnVEventReturnTroops.outgoingRaidsResume,default,useWings: false,resume: true,returnSlow: true).EnqueueAsap();
				});
			}



		}

		Flyout.SetAttachedFlyout(b,flyout);
		Flyout.ShowAttachedFlyout(b);
	}
}
public class CommandItem:INotifyPropertyChanged
{
	internal Army army;
	internal bool isOutgoing => army.sourceCid == City.build;
	//	public string sourceCoords=> army.sourceCity.nameAndRemarksAndPlayer;
	//	public string targetCoords=> army.targetCity.nameAndRemarksAndPlayer;
	public string info => $"{army.NextStopTimeString(' ')} {army.splitsS}{(army.isReturn^isOutgoing ? "" : "")} {(isOutgoing ? army.targetCity : army.sourceCity)}";

	//internal void SourceClick(object sender,RoutedEventArgs e)
	//{
	//	CityUI.ShowCity(army.sourceCid,false);
	//}
	internal void TargetClick(object sender,RoutedEventArgs e) {
		CityUI.ShowCity(isOutgoing ? army.targetCid : army.sourceCid);
	}


	internal string toolTip => army.WorldToolTip().tip;

	public CommandItem(Army army) {
		this.army = army;
		//UpdateAction();


	}

	public BitmapImage action =>
			  ImageHelper.Get(
								army.isRaid ? (
												army.isRepeating ? "UI/Icons/icon_cmmnds_raid_loop.png" :
												"UI/Icons/icon_cmmnds_raid_once.png") :
								army.isReturn ? "Region/UI/icon_player_own_troops_ret.png" :
								army.isSettle ? "Region/UI/icon_player_own_settlement.png" :
								army.isDefense ? "Region/UI/icon_player_own_support_inc.png" :
								army.isSiege&&army.shareInfo ? "Region/UI/icon_player_alliance_siege.png" :
								"Region/UI/icon_player_own_attack.png");

	internal void MenuOpening(object? sender,object  args) {
		var a = sender as MenuFlyout;
		var b = a.Target as Button;
		var commandItem = b.DataContext as CommandItem;
		if(!commandItem.army.isSubOrMe)
			return;
		a.Items.Clear();
		commandItem.AddToFlyout(a);
	}
	public void ContextRequested(UIElement sender,ContextRequestedEventArgs args) {
		if(!army.isSubOrMe)
			return;
		args.Handled    = true;
		var flyout = new MenuFlyout();
		AddToFlyout(flyout);
		flyout.ShowContext(sender,args);
	}

	internal void AddToFlyout(MenuFlyout flyout) {

		if(!army.isSubOrMe) {
			Assert(false);
			return;
		}
		if((army.isDefense&&!army.isReturn) || army.isSieging || army.isRaid) {
			flyout.AddItem("Return at..",glyph: (char)0xF738,command: () => SendTroops.ShowInstance(prior: army,timing: TimingSetting.arrival));
		}

		if(army.isRaid) {
			if(army.isRepeating) {
				flyout.AddItem(army.isReturn ? "Stop repeating once returned" : "Abort raid and return",Symbol.Undo,() => {
					CnVEventReturnTroops.TryReturn(army,default,false,isResume: false,isSlow: false);
				});
				if(!army.isReturn) {
					flyout.AddItem("Stop repeating raid",glyph: (char)0xE1CC,() => {
						CnVEventReturnTroops.TryReturn(army,default,false,isResume: false,isSlow: true);
					});
				}
			}
			else {
				if(!army.isReturn) {
					flyout.AddItem("Return immediately",Symbol.Undo,() => {
						CnVEventReturnTroops.TryReturn(army,default,false,isResume: false,isSlow: false);
					});

				}
				flyout.AddItem("Resume raiding",Symbol.Redo,() => {
					CnVEventReturnTroops.TryReturn(army,default,false,isResume: true,isSlow: false);
				});

			}

		}
		else if(!army.isReturn) {
			flyout.AddItem(!army.departed ? "Cancel" : army.isDefense ? "Return defense.." : army.isSieging ? "Return Siege.." : "Return"
				,Symbol.Undo,() => {
					if((army.isDefense && army.departed) || army.isSieging) {
						SendTroops.ShowInstance(prior: army);
					}
					else {
						CnVEventReturnTroops.TryReturn(army);
					}
				});
		}
		if(army.canUseWings)
			flyout.AddItem("Use Horns",Symbol.Forward,SpeedupDefense);
		if(army.canUseFanfare)
			flyout.AddItem("Use Quadriga",Symbol.Forward,UseFanfare);


	}


	private void UseFanfare() {
		try {
			var art = Artifact.GetUniversal(Artifact.ArtifactType.magical_fanfare);
			if(art is null) {
				Assert(false);
				return;
			}
			var artifact = art.id;


			var city = army.sourceCity;
			var id = city.outgoing.IndexOf(army);
			Assert(id != -1);
			if(id >= 0) {
				var toUse = 1;
				var needed = toUse- Player.active.ArtifactCount(artifact);
				if(!Artifact.Get(artifact).IsOkayToUse(toUse))
					return;

				// do we need to return first
				Assert(army.isRaid);
				// return troops first

				{
					SocketClient.DeferSendStart();

					try {
						if(needed > 0) {
							new CnVEventPurchaseArtifacts((ushort)artifact,(ushort)needed,Player.active.id).EnqueueAsap();
						}
						// If the army has not returned, we do a return with wings and pay later with a dummy call to useArtifacts
						// If it has started returning, we use the artifact



						(new CnVEventUseArtifacts(city.c) { artifactId = (ushort)artifact,count = (ushort)toUse,aux=id }).EnqueueAsap();

					}
					catch(Exception _ex) {
						LogEx(_ex);

					}
					finally {
						SocketClient.DeferSendEnd();
					}
				}
			}
		}
		catch(Exception _ex) {
			LogEx(_ex);

		}
	}

	private void SpeedupDefense() {
		try {
			var art = Artifact.GetForPlayerRank(Artifact.ArtifactType.Horn);
			if(art is null) {
				Assert(false);
				return;
			}
			var artifact = art.id;


			var city = army.sourceCity;
			var id = city.outgoing.IndexOf(army);
			Assert(id != -1);
			if(id >= 0) {
				var toUse = (int)army.splits;
				var needed = toUse- Player.active.ArtifactCount(artifact);
				if(!Artifact.Get(artifact).IsOkayToUse(toUse))
					return;

				// do we need to return first
				Assert(army.isDefense);
				// return troops first

				{
					SocketClient.DeferSendStart();

					try {
						if(needed > 0) {
							new CnVEventPurchaseArtifacts((ushort)artifact,(ushort)needed,Player.active.id).EnqueueAsap();
						}
						// If the army has not returned, we do a return with wings and pay later with a dummy call to useArtifacts
						// If it has started returning, we use the artifact
						if(army.arrived) {
							CnVEventReturnTroops.TryReturn(army,default,true);
						}


							(new CnVEventUseArtifacts(city.c) { artifactId = (ushort)artifact,count = (ushort)toUse,aux=id,flags = army.arrived.Switch(Flags.none,Flags.noEffect) | AppS.isTest.Switch(Flags.none,Flags.dontRemoveArtifacts|Flags.dontRemoveKarma) }).EnqueueAsap();

					}
					catch(Exception _ex) {
						LogEx(_ex);

					}
					finally {
						SocketClient.DeferSendEnd();
					}
				}
			}
		}
		catch(Exception _ex) {
			LogEx(_ex);

		}
	}

	public void ReinContextRequested(UIElement sender,ContextRequestedEventArgs args) {
		args.Handled    = true;
		var flyout = new MenuFlyout();


		flyout.AddItem("Return",Symbol.Undo,() => {
			SendTroops.ShowInstance(prior: army);
			//				CnVEventReturnTroops.TryReturn(army);
		});
		//	if( army.canUseWings)
		//			flyout.AddItem("Speedup",Symbol.Forward,SpeedupDefense);

		flyout.ShowContext(sender,args);
	}


	public void OnPropertyChanged(string members = null) => PropertyChanged?.Invoke(this,new(members));

	public event PropertyChangedEventHandler? PropertyChanged;
}


