using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using static CnV.View;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI;
using Humanizer;
//using CommunityToolkit.WinUI.UI;
//using CommunityToolkit.WinUI.UI.Controls;
using static CnV.City;
using System.Collections.ObjectModel;
using Microsoft.UI.Xaml.Media.Imaging;
using System.Collections.Specialized;
using Windows.ApplicationModel.DataTransfer;
//using Expander = CommunityToolkit.WinUI.UI.Controls.cer;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.
using static CnV.CityStats;
using EnumsNET;
using static CnV.BuildQueueItem;

namespace CnV
{
	public sealed partial class CityStats:UserControl, INotifyPropertyChanged {
		public static CityStats instance;
		public CityStats() {
			this.InitializeComponent();
			//buildQueue.CollectionChanged+=BuildQueue_CollectionChanged;
		}
#if false
		static TTip _BuildQueue_1 = new TTip("TT_Buildings_1",() =>
		
			City.GetBuild().buildQueue.Length > 0
		 );
		static TTip _BuildQueue_2 = new TTip("TT_Buildings_2");
#endif

		public event PropertyChangedEventHandler? PropertyChanged;
		public void OnPropertyChanged(string? member = null) {
			if(this.PropertyChanged is not null)
				AppS.QueueOnUIThread(() => PropertyChanged?.Invoke(this,new(member)));
		}
		public static void Changed(string? member = null) {
			if(instance is not null)
				instance.OnPropertyChanged(member);
		}
		private void CityBox_SelectionChanged(object sender,SelectionChangedEventArgs e) {
			var sel = cityBox.SelectedItem as City;
			if(sel != null && sel.cid != City.build) {
				City.ProcessCoordClick(sel.cid,false,AppS.keyModifiers);
			}
		}


		private void CitySubmitted(ComboBox sender,ComboBoxTextSubmittedEventArgs args) {
			var text = args.Text.ToLower();

			Log($"Summitted {text}");

			var items = City.gridCitySource;
			foreach(var it in items) {
				// its good
				if(it.nameAndRemarks == text) {
					return;
				}
			}


			foreach(var it in items) {
				if(it.nameAndRemarks.ToLower().StartsWith(text)) {
					sender.Text = it.nameAndRemarks;
					sender.SelectedItem = it;
					args.Handled = true;
					return;
				}
			}
			// try contains
			foreach(var it in items) {
				if(it.nameAndRemarks.ToLower().Contains(text)) {
					Log($"Contains {it}");

					sender.Text         = it.nameAndRemarks;
					sender.SelectedItem = it;
					args.Handled = true;
					return;
				}
			}
			// todo!
		}
		//		private static void BuildQueue_CollectionChanged(object? sender,NotifyCollectionChangedEventArgs e)
		//		{
		//			// invalidate

		//			instance?.buildCityChangeDebounce.Go();
		////			displayQueue = BuildQueueType.Empty;
		//			//Log(e.Action);
		//			//LogJson(e);
		//			// This is coming from the UI to us
		//			// Validate that the queue is still valid

		//			//switch(e.Action)
		//			//{
		//			//	case NotifyCollectionChangedAction.Move:
		//			//		{

		//			//			var city = instance.city;
		//			//			if(city.AttempMove(e.OldStartingIndex,e.NewStartingIndex))
		//			//			{
		//			//			}
		//			//			NotifyBuildQueueChange();
		//			//			break;
		//			//		}
		//			//	case NotifyCollectionChangedAction.Remove:
		//			//		{
		//			//			return;
		//			//			var city = instance.city;
		//			//			if(city.AttempRemove(e.OldStartingIndex))
		//			//			{
		//			//			}
		//			//			NotifyBuildQueueChange();
		//			//			break;
		//			//		}
		//			//	case NotifyCollectionChangedAction.Add:
		//			//		{

		//			//			return;
		//			//			var city = instance.city;
		//			//			if(city.AttemptAdd(e.NewStartingIndex, e.NewItems.Cast<BuildItem>().Select( a => a.op ).ToArray() ))
		//			//			{
		//			//			}
		//			//			NotifyBuildQueueChange();
		//			//			break;
		//			//		}
		//			//	default:
		//			//		Assert(false);
		//			//		break;
		//			//}
		//		}

		public City city => City.GetBuild();
		//internal void NotifyBuildCityChange()
		//{
		//	var prior = city;
		//	prior.PropertyChanged -= City_PropertyChanged;
		//	city = City.GetBuild();
		//	city.PropertyChanged += City_PropertyChanged;
		//	Invalidate();

		//}


		internal static BuildQueueType lastSynchronizedQueue = BuildQueueType.Empty;
		internal static RecruitQueueType lastSynchronizedRecruitQueue = RecruitQueueType.Empty;
		static void BuildCityChanged() {
			UpdateBuildQueue();
			UpdateCommandItems();
			UpdateRecruitQueue();
			UpdateTradeItems();

			ClearLastDisplayed();
			Changed();
			// Close misc dialogs




		}
		public string recruitTooltip { get {
				string rv = "Recruit Queue, click to recruit.";
				if(city.stats.rsInf>0)
					rv += $"\nInfantry: {city.stats.rsInf}";
				if(city.stats.rsCav>0)
					rv += $"\nCavalry: {city.stats.rsCav}";
				if(city.stats.rsMagic>0)
					rv += $"\nMagic: {city.stats.rsMagic}";
				if(city.stats.rsArt>0)
					rv += $"\nArt: {city.stats.rsArt}";
				if(city.stats.rsBlessed>0)
					rv += $"\nBlessed: {city.stats.rsBlessed}";
				if(city.stats.rsNavy>0)
					rv += $"\nNavy: {city.stats.rsNavy}";
				if(city.stats.rsGuard>0)
					rv += $"\nGuard: {city.stats.rsGuard}";
				return rv;
			}
		}
		static void UpdateBuildQueue() {
			// Don't notify whole we are doing stuff
			try {
				//	Trace("Update build Queue");
				//	instance.buildQueue.CollectionChanged -= BuildQueue_CollectionChanged;
				//var firstVisible = instance.buildQueueListView.vis

				var city = instance.city;
				var displayQueue = city.buildQueue;
				lastSynchronizedQueue = displayQueue;
				//int lg = displayQueue.Length;
				var bq = instance.buildQueue;
				//for(int i = bq.Count;--i>= 0;)
				//{
				//	var op = bq[i].op;
				//	if(!displayQueue.Any(a => a == op))
				//	{
				//		bq.RemoveAt(i);
				//	}
				//}
				var anyRemoved = SyncLists(displayQueue,bq,(rt,city) => new BuildItem(rt,city),(a,b) => a == b.op);
				var lg = displayQueue.Length;
				if(lg == bq.Count()) {
					var status = city.GetBuildQueueStatuses();
					if(status.Length == lg) {
						for(int i = 0;i<lg;++i) {
							if(bq[i].status != status[i]) {
								bq[i].status = status[i];
								bq[i].OnPropertyChanged();
							}
						}
					}
				}
				// keep first in view
				//if(anyRemoved && bq.Any() )
				//{
				//	instance.buildQueueListView.ScrollIntoView(bq.First());
				//}
			}
			catch(Exception ex) {
				LogEx(ex);
			}
			// restore callback
			//	instance.buildQueue.CollectionChanged += BuildQueue_CollectionChanged;

		}
		static void UpdateRecruitQueue() {
			// Don't notify whole we are doing stuff
			try {

				//var firstVisible = instance.buildQueueListView.vis
				var city = instance.city;
				var displayQueue = city.recruitQueue;
				lastSynchronizedRecruitQueue = displayQueue;
				int lg = displayQueue.Length;
				var bq = instance.recruitQueue;
				//for(int i = bq.Count;--i>= 0;)
				//{
				//	var op = bq[i].op;
				//	if(!displayQueue.Any(a => a == op))
				//	{
				//		bq.RemoveAt(i);
				//	}
				//}

				// Add or update
				bool anyRemoved = SyncLists(displayQueue,bq,(rt,city) => new RecruitItem(rt,city),(a,b) => a == b.op);

				Assert(bq.Count == lg);
				for(int i = 0;i<lg;++i) {
					Assert(bq[i].op == displayQueue[i]);
				}
				// keep first in view
				//if(anyRemoved && bq.Any())
				//{
				//	instance.RecruitQueueListView.ScrollIntoView(bq.First());
				//}
			}
			catch(Exception ex) {
				LogEx(ex);
			}
			// restore callback

		}

		private static bool SyncLists<TR, TX>(IList<TR> rtQ,ObservableCollection<TX> xQ,Func<TR,City,TX> factory,Func<TR,TX,bool> equals) {
			var city = instance.city;
			var lg = rtQ.Count;
			var anyRemoved = false;

			// first remove
			//{
			//__restartRemove:
			//	int i = 0;
			//	foreach(var x in xQ) {
			//		if(!rtQ.Any(a => equals(a,x))) {
			//			xQ.RemoveAt(i);
			//			anyRemoved=true;
			//			goto __restartRemove;
			//		}
			//		++i;
			//	}
			//}

			for(int i = 0;i<lg;++i) {
				var op = rtQ[i];
				int cur = -1;
				for(int j = i;j<xQ.Count;++j) {
					if(equals(op,xQ[j])) {
						cur = j;
						break;
					}
				}
				if(cur == -1) {
					var bi = factory(op,city);
					xQ.Insert(i,bi);
				}
				else {
					if(cur != i) {
						xQ.Move(cur,i);

					}

				}
			}

			// removed unused, these are all at the end now
			for(int r = xQ.Count;--r >= lg;)
			{
				xQ.RemoveAt(r);
				anyRemoved=true;
			}
			
			Assert(xQ.Count == rtQ.Count);
			Assert(xQ.Count == lg);
			//for(int i = 0;i<lg;++i) {
			//	Assert(equals(rtQ[i],xQ[i]));
			//}
			return anyRemoved;
		}

		static void UpdateCommandItems() {
			// Don't notify whole we are doing stuff
			try {

				//var firstVisible = instance.buildQueueListView.vis

				var city = instance.city;
				{
					var displayQueue = city.outgoing.OrderBy(a => a.arrival).ToArray();
					int lg = displayQueue.Length;
					var bq = instance.commandItems;

					var anyRemoved = SyncLists(displayQueue,bq,(rt,city) => new CommandItem(rt),(a,b) => a == b.army);
					if(commandsDirty) {
						foreach(var i in bq) {
							//i.UpdateAction();
							i.OnPropertyChanged();
						}
						commandsDirty=false;
					}
				}
				{
					var displayQueue = city.incoming.Where(a => a.isDefense).OrderBy(a => a.arrival).ToArray();
					int lg = displayQueue.Length;
					var bq = instance.reinforcementItems;

					var anyRemoved = SyncLists(displayQueue,bq,(rt,city) => new CommandItem(rt),(a,b) => a == b.army);
				}
				{
					var displayQueue = city.incoming.Where(a => a.isAttack).OrderBy(a => a.arrival).ToArray();
					int lg = displayQueue.Length;
					var bq = instance.incomingItems;

					var anyRemoved = SyncLists(displayQueue,bq,(rt,city) => new CommandItem(rt),(a,b) => a == b.army);
				}
				// keep first in view
				//if(anyRemoved && bq.Any() )
				//{
				//	instance.CommandsListView.ScrollIntoView(bq.First());
				//}
			}
			catch(Exception ex) {
				LogEx(ex);
			}
			// restore callback

		}
		static void UpdateTradeItems() {
			// Don't notify while we are doing stuff
			try {

				//var firstVisible = instance.buildQueueListView.vis

				var city = instance.city;
				var displayQueue = city.tradesOut.Concat(city.tradesIn).OrderBy(a => a.isReturning ? a.returnTime : a.arrival).ToArray();
				int lg = displayQueue.Length;
				var bq = instance.tradeItems;

				var anyRemoved = SyncLists(displayQueue,bq,(rt,city) => new TradeItem(rt),(a,b) => a == b.trade);




			}
			catch(Exception ex) {
				LogEx(ex);
			}
			// restore callback

		}
		//internal DebounceA buildCityChangeDebounce = new(BuildCityChanged) { runOnUiThread=true,debounceDelay=100 };
		internal Visibility palaceVisible => city.blessData.isValid.Switch(Visibility.Collapsed,Visibility.Visible);
		internal string palaceVirtue => city.blessData.virtue.EnumName();
		internal string palaceInfo { get{
				using var sb = new PooledStringBuilder();
				var city = this.city;
				var bd = city.blessData;
				if(city.isBlessed) {
					sb.s.Append("Blessed until ").Append(bd.blessedUntil.Format());
				}
				if(bd.level > 0) {
					sb.s.Append("\nPalace level ").Append(bd.level);
				}
				sb.s.Append("\nStored:\n").Append(bd.stored.Format(" "));
				var incoming = city.incomingDonations;
				if(incoming.isNonZero) {
					sb.s.Append("\nIncoming:\n").Append(incoming.Format(" "))
						.Append("\nWill have:\n").Append( (bd.stored+incoming).Format(" ") );
				
				}
				if(bd.level < 10) {

					sb.s.Append("\nNeed:\n").Append(bd.needed.Format(" "));
				}

				if(!bd.notes.IsNullOrEmpty())
					sb.s.AppendLinePre(bd.notes);
				if(bd.priority != PalacePriority.NA )
					sb.s.Append("\nPriority: ").Append(bd.priority.EnumTitle());
				if(bd.damage > 0)
					sb.s.Append("\nDamage: ").Append(bd.damage.Format());
				


				return sb.s.ToString();
			} }

		internal string TroopsHomeS => city?.troopsHere.Format(separator: ',');
		internal string TroopsOwnedS => city?.troopsOwned.Format(separator: ',');
		internal string IncomingReinforcements => city?.incomingReinforcements.Format(separator: ',');

		internal Visibility TroopsHomeVisible => city?.troopsOwned != new TroopTypeCounts(city.troopsHere) ? Visibility.Visible : Visibility.Collapsed;

		internal Visibility incomingVisible => city.incoming.Any(a => a.isAttack) ? Visibility.Visible : Visibility.Collapsed;

		internal Visibility IncomingReinforcementsVisible => city.allReinforcements.Any()==true ? Visibility.Visible : Visibility.Collapsed;

		public string commandsTitle => $"Commands {city.activeCommands}/{city.commandSlots}";
		public string tradesTitle {
		get{
			var rv = $"Trades {tradeItems.Count} {CnV.Resources.cartGlyph}{city.cartsHome}/{city.carts}";
			if(city.ships >0 ) 
					rv += $"{CnV.Resources.shipGlyph}{city.shipsHome}/{city.ships}";
				return rv;
} }
public string troopsTitle => $"Troops {city?.tsTotal}/{city?.stats.maxTs}";

		public string recruitTitle => city?.amuletTime > 0 ? $"Recruit +{city.amuletTime}" : "Recruit";
		//public SolidColorBrush ResourceForeground(int resId) => new SolidColorBrush(Windows.UI.Color.FromArgb(255,(byte)(31+64*resId),128,128) );
		//public string ResourceStr(int resId) => $"{city?.resources[resId]:N0}";

		//public event PropertyChangedEventHandler PropertyChanged;

		// invalidate the cache
		private static City lastDisplayed;
		static void  ClearLastDisplayed()
		{
			if(lastDisplayed is not null)
			{
				lastDisplayed.PropertyChanged -= City_PropertyChanged;
				lastDisplayed=null;
			}
		}
		public static void CityBuildingsChange(City _city)
		{
			if(_city == lastDisplayed)
				ClearLastDisplayed();
			if(_city.isBuild)
			{
				Invalidate();
			}
		}
		public static void CityRecruitChange(City _city)
		{
			if(_city == lastDisplayed)
				ClearLastDisplayed();
			if(_city.isBuild)
			{
				Invalidate();
			}
		}
		//public void OnPropertyChanged() =>
		//		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
		

		// called from Sim
		public void UpdateUI()
		{
			try
			{
				
				//	var i = CityStats.instance;
				
				//if(City.GetBuild().IsInvalid())
				//	return;
				// building counts

		

				AppS.QueueOnUIThread(() =>
				{
					

					try
					{
						var city = City.GetBuild();
						var hasBeenDisplayed = lastDisplayed == city;
						if(!hasBeenDisplayed)
						{
							lastDisplayed = city;
							city.PropertyChanged+=City_PropertyChanged;
							Changed();
						}
						var bdd = !hasBeenDisplayed ? GetBuildingCounts(city) : default;
						var t = Sim.simTime;
#if DEBUG
						ShellPage.instance.timeDisplay.Text = $"{t.FormatWithYear()}\n{GameClient.renderFrame/60} 0:{AppS.dispatches0} 1:{AppS.dispatches1}";
#else
						ShellPage.instance.timeDisplay.Text = t.FormatWithYear();
#endif
						var timeBrush = AppS.Brush( Sim.isWarmup? (Sim.isHistoric ? Colors.Orange: Colors.Gray ): Sim.isHistoric ? Colors.Yellow : Colors.White);
						if(ShellPage.instance.timeDisplay.Foreground != timeBrush)
							ShellPage.instance.timeDisplay.Foreground=timeBrush;
						if(expResource.IsExpanded)
						{
							var resources = city.SampleResources();
							//var panels = expResource.Child<CommunityToolkit.WinUI.UI.Controls.WrapPanel>().Children<StackPanel>();
							var prod = city.stats.production;
							var incoming = new CnV.Resources();
							var store = city.stats.storage;
							foreach(var trade in city.tradesIn)
							{
								incoming += trade.resources;
							}
							foreach(var o in city.outgoing)
							{
								if(o.isReturn)
									incoming += o.resources;
							}
							for(var r = 0;r< CnV.Resources.idCount;r++)
							{
								var rr = resourceItems[r];

								if(incoming[r] > 0)
								{
									rr.incoming = $"{CnV.Resources.cartGlyph} {incoming[r].Format()}";
									var balance = store[r] - resources[r] - incoming[r];
									rr.incomingBrush = AppS.Brush(balance>= 0 ? Colors.Green : Colors.Orange);
								}
								else
								{
									rr.incoming = null;
								
								}
								var p = prod[r];
								rr.production =$"{p:+#,#;-#,#;' --'}/h";
								rr.productionBrush = AppS.Brush((p switch
								{
									> 0 => Colors.White,
									< 0 => Colors.Yellow,
									_ => Colors.LightGray

								}));

								//				var txt = r switch { 0 => res0, 1 => res1, 2 => res2, _ => res3 };
								//				var prod = r switch { 0 => prod0, 1 => prod1, 2 => prod2, _ => prod3 };

								var res = resources[r];
								var storage = store[r];
								rr.here = res.Format();
								rr.storage =  storage.Format(); 
								rr.hereBrush = AppS.Brush( (res >= storage ?
									Colors.Red : res >= storage*3/4 ?
									Colors.Orange : res == 0 ?
									Colors.LightGray : Colors.LightBlue));


								rr.OnPropertyChangedImmediate();
							}
						}
						if(expBuildQueue.IsExpanded)
						{
							foreach(var b in buildQueue)
							{
								b.UpdateText();
							}
						}
						if(expEnlistment.IsExpanded)
						{
							int counter = 0;
							foreach(var b in recruitQueue)
							{
								b.UpdateText(counter);
								++counter;
							}
						}

						if(!hasBeenDisplayed)
						{
							var txt = (expBuildings.Header as DependencyObject).Child<TextBlock>(1);
							txt.UpdateLazy($"Buildings: [{bdd.buildingCount}/{bdd.townHallLevel*10}]");
							var hasHammerTime = city.hammerTime > 0;
							queueText.UpdateLazy($"CS:{city.stats.cs:N0}%{ (hasHammerTime ? " +" +city.hammerTime.Format() : "" )}", hasHammerTime ? Colors.GreenYellow : Colors.White); 
						
							var bd = new List<BuildingCountAndBrush>();
							foreach(var i in bdd.counts)
							{
								if(i.Value > 0)
								{
									bd.Add(new BuildingCountAndBrush()
									{ count = i.Value,
										image = CityBuild.GetBuildingImage(i.Key,BuildingCountAndBrush.width),
										bid = i.Key });
								}
							}
							for(int i = buildingCounts.Count;--i>= 0;)
							{
								var bid = buildingCounts[i].image;
								if(!bd.Any(a => a.image== bid))
								{
									buildingCounts.RemoveAt(i);
								}
							}
							// Add or update
							foreach(var i in bd)
							{
								var prior = buildingCounts.FirstOrDefault(a => a.image == i.image);
								if(prior is not null)
								{
									if(prior.count != i.count)
									{
										prior.count = i.count;
										prior.OnPropertyChanged(nameof(prior.count));
									}
								}
								else
								{
									buildingCounts.Add(i);
								}
							}

							//bd.Add(new BuildingCountAndBrush()
							//{ count = bdd.buildingCount+bdd.towerCount, image = CityBuild.GetBuildingImage(Building.bidTownHall, BuildingCountAndBrush.width) });

							// var button = sender as Button; button.Focus(FocusState.Programmatic);

							//buildingCountGrid.ItemsSource = bd;
						}
					}
					catch(Exception e)
					{
						LogEx(e,report: false);
					}
					finally
					{
						

					}
				});

			}
			catch(Exception e)
			{
				LogEx(e,report: false);
			}
		}

		private static  void City_PropertyChanged(object? sender,PropertyChangedEventArgs e)
		{
			Invalidate();
		}
	
		private void Expander_Expanding(Microsoft.UI.Xaml.Controls.Expander sender,ExpanderExpandingEventArgs args)
		{
			Invalidate();
		}
		public string ResToolTip
		{
			get {
				using var sb = new PooledStringBuilder();
				sb.s.Append(  $"{CnV.Resources.goldGlyph} {Player.active.gold.Format()}\n  +{city.stats.goldProduction.Format()}/h\nRes+Incomming:" );
				var peakRes = city.sampleResources+city.incomingRes;
				var store = city.stats.storage;
				for(int i=0;i<4;++i)
				{
					sb.s.Append('\n').Append(CnV.Resources.ResGlyphC(i)).Append(' ')
						.Append(peakRes[i].Format()).Append('/').Append(store[i].Format());
				}


				return sb.s.ToString();
			}
		}

		//private void scroll_SizeChanged(object sender,ScrollViewerViewChangedEventArgs e)
		//{
		//	if(e.IsIntermediate)
		//		return;
		//	ShellPage.updateHtmlOffsets.SizeChanged();
		//}
		//private void scroll_SizeChanged4(object sender,SizeChangedEventArgs e)
		//{
		//	ShellPage.updateHtmlOffsets.SizeChanged();

		//}
		//internal void ProcessScrollSizeChanged()
		//{
		//	try
		//	{
		//		var baseSize = ((scroll.ActualWidth)/scroll.ZoomFactor).Max(0);
		//		stackPanel.Width = (baseSize -8).Max(0);
		//	}
		//	catch(Exception ex)
		//	{

		//		LogEx(ex);
		//	}
		//		//var expanderWidth = (baseSize -14).Max(0);
		//		//foreach(var ch in stackPanel.Children<Expander>())
		//		//{
		//		//	ch.Width = expanderWidth;
		//		//}
		//}

		static bool commandsDirty;

		internal static void InvalidateCommands()
		{
			commandsDirty =true;
			Invalidate();
		}


		internal static void Invalidate()
		{
			if(Sim.isPastWarmup && instance != null )
				AppS.QueueOnUIThread(BuildCityChanged);
//				instance?.buildCityChangeDebounce.Go();
		
			
		}

		private (int buildingCount, int towerCount, int townHallLevel, Dictionary<BuildingId,int> counts) GetBuildingCounts(City build)
		{
			var buildingCounts = new Dictionary<BuildingId,int>();
			int buildingCount = 0;
			int towerCount = 0;
			try
			{
				var buildings = CityBuild.isPlanner ? build.GetLayoutBuildings() : build.postQueueBuildings;



				foreach(var bdi in buildings)
				{
					var id = bdi.id;
					if(id == 0 || !bdi.isBuilding || id == Building.bidTownHall || id == Building.bidWall)
					{
						continue;
					}

					{
						if(bdi.isTower)
							++towerCount;
						else
							++buildingCount;

						if(!buildingCounts.TryGetValue(id,out var counter))
						{
							buildingCounts.Add(id,0);
							counter = 0;
						}

						buildingCounts[id] = counter + 1;
					}
				}
				return (buildingCount, towerCount, buildings[Building.bspotTownHall].bl, buildingCounts);

			}
			catch(Exception ex)
			{
				LogEx(ex);
			}
			return (buildingCount, towerCount, -1, buildingCounts);
		}

		ObservableCollection<BuildingCountAndBrush> buildingCounts = new();

		internal ObservableCollection<BuildItem> buildQueue = new();
		internal ObservableCollection<RecruitItem> recruitQueue = new();
		internal ObservableCollection<CommandItem> commandItems= new();
		internal ObservableCollection<CommandItem> reinforcementItems= new();
		internal ObservableCollection<CommandItem> incomingItems= new();
		internal ObservableCollection<TradeItem> tradeItems= new();
		internal ResourceItem[] resourceItems = new[] { new ResourceItem() {r= 0},new ResourceItem() {r= 1},new ResourceItem() {r= 2},new ResourceItem() {r= 3}  };
		private void UserControl_Loaded(object sender,RoutedEventArgs e)
		{
			Assert(instance is null);
			instance = this;
			City.buildCityChanged+= (_,_) => {
				Invalidate();
				DialogG.CitySwitched();
			};			
			

		}

		

//		internal static bool hasTempleQueued =>	 instance.buildQueue.Any(a => a.op.bidIfNotMove == Building.bidTemple);

		

		private async void buildQueueListView_DragItemsCompleted(ListViewBase sender,DragItemsCompletedEventArgs args)
		{
			try
			{
				Log(args.DropResult);
				//if(hasTempleQueued) {
				//	AppS.MessageBox("Cannot drag when temples are queued");
				//	return;
				//}
				if(args.DropResult ==DataPackageOperation.Move)
				{
					var item = args.Items.FirstOrDefault() as BuildItem;
					var index = buildQueue.IndexOf(item);
					var bq = city.buildQueue;
					Note.Show($"Change: {dragStartingIndex}=>{index}, {buildQueue.Count} {bq == bqAtDragStart} ");
					if((index != -1 && dragStartingIndex != -1) && (bq==bqAtDragStart))
					{
						
						// async action
						await city.AttemptMove(dragStartingIndex,index,bqAtDragStart);

					}

				}
				else
				{
					
					var items = args.Items.Select( (a) =>  buildQueue.IndexOf( a as BuildItem)).Where(x=>x!=-1).Select(a=>(byte)a);
					
					city.RemoveWithDependencies(new(items),bqAtDragStart);

					
				}
			}
			catch(Exception ex)
			{
				LogEx(ex);
				city.BuildingsOrQueueChanged();
			}
			
			//			Log(args.Items.Format());
			//			LogJson(args);

		}

		BuildQueueType bqAtDragStart; // cache this, if it changes by drag end, everything is invalidated
		int dragStartingIndex;
		private void buildQueueListView_DragItemsStarting(object sender,DragItemsStartingEventArgs e)
		{
			Log(e.Data?.RequestedOperation);
			var i = e.Items?.FirstOrDefault() as BuildItem;
			dragStartingIndex = buildQueue.IndexOf(i);
			// Cannot move if it is currently building
			var city = this.city;
			bqAtDragStart= lastSynchronizedQueue; 
			if(e.Items.Count > 1)
			{
				Note.Show("Multi item reorder not yet supported, moving first selement");
			}
	
		}

		private void buildQueueListView_ContextRequested(UIElement sender,ContextRequestedEventArgs args)
		{
			
			Log(args);
		}


		private void buildQueueListView_DropCompleted(UIElement sender,DropCompletedEventArgs args)
		{
			Log(args.DropResult);
			LogJson(args);
		}
		private void buildQueueListView_DragOver(object sender,DragEventArgs e)
		{
			LogJson(e);

		}

		private async void buildQueueListView_ItemClick(object sender,ItemClickEventArgs e)
		{
			try {
				var i = e.ClickedItem as BuildItem;
				if(i.status == Status.invalid) {
					if(await AppS.DoYesNoBox("Clean","Remove invalid items?") == 1) {
						RemoveInvalidBuildQueueItems();
					}
				}
			}
			catch(Exception _ex) {
				LogEx(_ex);

			}

		}

		private void buildQueueListView_SelectionChanged(object sender,SelectionChangedEventArgs e)
		{
			CityView.selectedBuildCs = buildQueueListView.SelectedItems.Select(a => ((BuildItem)a).op.bspot).ToArray();
			CityView.selectedBuildCsChangeTime = AGame.animationT;
		}

		private void buildingCountGrid_SelectionChanged(object sender,SelectionChangedEventArgs e)
		{
			CityView.selectedBuildingIds = buildingCountGrid.SelectedItems.Select(a => ((BuildingCountAndBrush)a).bid).ToArray();
			CityView.selectedBuildingIdsChangeTime = AGame.animationT;
		}

		private void WoodTap(object sender,TappedRoutedEventArgs e)=> CityUI.Show( Artifact.ArtifactType.axe, sender);
		private void StoneTap(object sender,TappedRoutedEventArgs e)=> CityUI.Show( Artifact.ArtifactType.hammer, sender);
		private void IronTap(object sender,TappedRoutedEventArgs e)=> CityUI.Show( Artifact.ArtifactType.pike, sender);
		private void FoodTap(object sender,TappedRoutedEventArgs e)=> CityUI.Show( Artifact.ArtifactType.sickle, sender);
		private void GoldTap(object sender,TappedRoutedEventArgs e)=> CityUI.Show( Artifact.ArtifactType.chest, sender);

		private void QueueRightTapped(object sender,RightTappedRoutedEventArgs e)=>	CityUI.Show( Artifact.ArtifactType.medallion, sender);

		private void EnlistmentContextRequested(UIElement sender,ContextRequestedEventArgs args)
		{
			args.Handled    = true;
			var flyout = new MenuFlyout();
			
			flyout.AddItem("Amulet..",Symbol.OutlineStar,() =>
			{
				CityUI.Show( Artifact.ArtifactType.amulet, sender);
			});

			if(instance.RecruitQueueListView.SelectedItems.Any())
			{
				flyout.AddItem("Cancel Selected",Symbol.Remove,() =>
				{
					var sel = instance.RecruitQueueListView.SelectedItems;
					if(sel.Any())
						city.RemoveFromRecruit(new(sel.Select(x => instance.recruitQueue.IndexOf(x as RecruitItem))),lastSynchronizedRecruitQueue);
				});
			}
			if(city.recruitQueue.Any())
			{
				flyout.AddItem("Cancel all",Symbol.Cut,() =>
				{
					city.RemoveFromRecruit( Enumerable.Range(0,city.recruitQueue.Length).ToList(), lastSynchronizedRecruitQueue);
					var sel = instance.buildQueueListView.SelectedItems;
					new CnVEventCancelBuildQueue(city.worldC).EnqueueAsap();
				});
			}
			flyout.ShowContext(sender,args);

			
		}

		private void CityIconTapped(object sender,TappedRoutedEventArgs e)
		{
			var city= (sender as FrameworkElement)?.DataContext as City;
			if(city is not null)
			{
				e.Handled=true;
				city.Focus();
			}
		}

		private void EnlistmentClick(object sender,RoutedEventArgs e)
		{
			RecruitDialog.ShowInstance(city);
		}

		private void RecruitTargetsClick(object sender,RoutedEventArgs e)
		{
			RecruitTargetDialog.ShowInstance(city);
		}

		private void AmuletClick(object sender,RoutedEventArgs e)
		{
			CityUI.Show( Artifact.ArtifactType.amulet, sender);
		}

		private void AutobuildTapped(object sender,TappedRoutedEventArgs e)
		{
			AutobuildDialog.ShowInstance();
			e.Handled=true;
		}

		private void TradesClick(object sender,RoutedEventArgs e)
		{
			TradeSettingsDialog.ShowInstance();
			
		}
		internal string tradesToolTip
		{
			get {
				return $"{CnV.Resources.cartGlyph} {city.cartsHome.Format()}/{city.carts.Format()}\n{CnV.Resources.shipGlyph} {city.shipsHome.Format()}/{city.ships.Format()}";
			}
		}

		private void CombatCalc(object sender,RoutedEventArgs e)
		{
			CombatCalcDialog.ShowInstance();
		}

		public void RemoveInvalidBuildQueueItems() {
			try {
					var bq = city.buildQueue;
					var status = city.GetBuildQueueStatuses();
					var remove = new List<byte>();
				if(status.Length < 255) {
					for(int i = 0;i<status.Length;++i) {
						if(status[i] == Status.invalid) {
							remove.Add((byte)i);
						}
					}
					city.RemoveWithDependencies(remove,bq);
				}
				}
				catch(Exception _ex) {
					LogEx(_ex);

				}
		}
	}
	public class BuildingCountAndBrush:INotifyPropertyChanged
	{
		public BuildingId bid;
		public const int width = 32;
		public Microsoft.UI.Xaml.Media.Imaging.BitmapImage image { get; set; }
		public int count { get; set; }

		public void OnPropertyChanged(string members = null) => PropertyChanged?.Invoke(this,new(members));

		public event PropertyChangedEventHandler? PropertyChanged;
	}
	public class BuildItem:INotifyPropertyChanged
	{
		public const int size = 32;
		public BitmapImage image { get; set; }
		public string opText { get; set; }
		public string timeText { get; set; }
		public BuildQueueItem.Status status;
		public Brush timeColor => AppS.Brush( status switch 
		{ Status.building => Colors.White,
			Status.valid => Colors.Green,
			Status.invalid => Colors.Red,
			_=> Colors.Gray
		});

		public string resTip => $"{ status.EnumTitle()}\n{
			(op.op switch
			{
				Op.upgrade or Op.build => (-op.BuildOrUpgradeResourceBalance()).Format(),
				Op.demo or Op.downgrade => op.DemoOrDowngradeResourceBalance().Format(),
				_ => null
			}) }";

		public BuildQueueItem op;

		internal City city;
		public BuildItem(BuildQueueItem item, City city)
		{
			this.city = city;
			op = item;
			image = CityBuild.GetBuildingImage( item.isMove ? Building.bidMove :  item.bid,size);
			var u = op.unpack;
			opText = u.isMove ? "Move" : u.isDemo ? "Destroy" : u.isBuild ? $"Build{(u.pa==false ? " p" : "") }" : $"{u.slvl} to {u.elvl}";
			UpdateText();
		}
		public void UpdateText()
		{
			try
			{
				var q = city.buildQueue;
				TimeSpanS dt;
				if(q.Any() && q[0]== op && city.buildItemEndsAt.isNotZero)
					dt = city.buildItemEndsAt - Sim.simTime;
				else
					dt = new(op.TimeRequired(city));
				var text = dt.ToString();
				if(text != timeText)
				{
					timeText = text;// + BuildingDef.FromId(item.bid).Bn;
					OnPropertyChanged(nameof(this.timeText));
				}
			}
			catch(Exception ex)
			{
				LogEx(ex);
			}

		}

		
		public void ContextRequested(UIElement sender,ContextRequestedEventArgs args)
		{
			args.Handled    = true;
			var flyout = new MenuFlyout();

			if(instance.buildQueue.Any(a=>a.status == Status.invalid))
				{
			flyout.AddItem("Remove Invalid..",Symbol.Bullets,instance.RemoveInvalidBuildQueueItems);

			}
			flyout.AddItem("Medallion..",Symbol.OutlineStar,() =>
			{
				CityUI.Show( Artifact.ArtifactType.medallion, sender);
			});
			if(op.isRes && op.isDemo)
			{
			flyout.AddItem("Powder..",Symbol.Favorite,() =>
			{
				CityUI.Show( Artifact.ArtifactType.black_powder, sender);
			});
			}
//			if(!hasTempleQueued) 
				{

				flyout.AddItem("Remove..",Symbol.Remove,() => {
					var index = instance.buildQueue.IndexOf(this);
					if(index != -1)
						city.RemoveWithDependencies(new(new[] { (byte)index }),lastSynchronizedQueue);
					else {
						Note.Show("Something changed");
						Invalidate();
					}
				});
				if(instance.buildQueueListView.SelectedItems.Any()) {
					flyout.AddItem("Cancel Selected",Symbol.Cancel,() => {
						var sel = instance.buildQueueListView.SelectedItems;

						city.RemoveWithDependencies(new( sel.Select(x => (byte)instance.buildQueue.IndexOf(x as BuildItem))),lastSynchronizedQueue);
					});
				}
				flyout.AddItem("Cancel all",Symbol.Cut,() => {
					var sel = instance.buildQueueListView.SelectedItems;
					new CnVEventCancelBuildQueue(city.worldC).EnqueueAsap();
				});
				flyout.AddItem("Sort",Symbol.Sort,() => {
					if(BuildQueue.HasBuildOpsInFlight(city)) {
						Note.Show("Please try again in a minute");
					}
					else {
						var sel = instance.buildQueueListView.SelectedItems;
						new CnVEventSortBuildQueue(city.worldC).EnqueueAsap();
					}
				});
				flyout.AddItem("Move To Front",Symbol.Up,() => {
					var index = instance.buildQueue.IndexOf(this);
					if(index != -1)
						city.AttemptMove(index,0,lastSynchronizedQueue);
					else
						Invalidate();
				});
				flyout.AddItem("Move To End",Symbol.Back,() => {
					var index = instance.buildQueue.IndexOf(this);
					if(index != -1)
						city.AttemptMove(index,city.buildQueue.Length-1,lastSynchronizedQueue);
					else
						Invalidate();
				});
			}
			flyout.ShowContext(sender,args);

		}
		public void OnPropertyChanged(string members = null) => PropertyChanged?.Invoke(this,new(members));

		public event PropertyChangedEventHandler? PropertyChanged;
	}
	public class RecruitItem:INotifyPropertyChanged
	{
		public const int size = 32;
		public BitmapImage image { get; set; }
		public string count { get; set; }
		public string timeText { get; set; }
		public TroopTypeCount op;

		internal City city;
		public RecruitItem(TroopTypeCount item, City city)
		{
			this.city = city;
			op = item;
			image = Troops.Image(op.t);
			count = item.c.Format();
			///var u = op.unpack;
			//opText = u.isMove ? "Move" : u.isDemo ? "Destroy" : u.isBuild ? $"Build{(u.pa==false ? " p" : "") }" : u.isDowngrade ? $"Down to {u.elvl}" : $"Up to {u.elvl}";
	
		}
		public void UpdateText(int myId)
		{
			try
			{
				var q = city.recruitQueue;
				if(myId < q.Length)
				{
					TimeSpanS dt;
					if(myId ==0 && city.recruitItemEndsAt.isNotZero)
						dt = city.recruitItemEndsAt - Sim.simTime;
					else
						dt = new(op.RecruitTimeRequired(city));
					var text = dt.ToString();
					if(text != timeText)
					{
						timeText = text;// + BuildingDef.FromId(item.bid).Bn;
						OnPropertyChanged(nameof(this.timeText));
					}
				}
			}
			catch(Exception ex)
			{
				LogEx(ex);
			}

		}
		
		public void OnPropertyChanged(string members = null) => PropertyChanged?.Invoke(this,new(members));

		public event PropertyChangedEventHandler? PropertyChanged;
	}
	public class CommandItem:INotifyPropertyChanged
	{
		internal Army army;
		internal bool isOutgoing => army.sourceCid == City.build;
	//	public string sourceCoords=> army.sourceCity.nameAndRemarksAndPlayer;
	//	public string targetCoords=> army.targetCity.nameAndRemarksAndPlayer;
		public string info => $"{army.NextStopTimeString(' ')} {army.splitsS}{(army.isReturn^isOutgoing ? "to" : "from" )} {(isOutgoing ? army.targetCity: army.sourceCity)}";

		//internal void SourceClick(object sender,RoutedEventArgs e)
		//{
		//	CityUI.ShowCity(army.sourceCid,false);
		//}
		internal void TargetClick(object sender,RoutedEventArgs e)
		{
			CityUI.ShowCity(isOutgoing ? army.targetCid : army.sourceCid,false);
		}


		internal string toolTip => army.WorldToolTip().tip;

		public CommandItem(Army army)
		{
			this.army = army;
			//UpdateAction();
			
			
		}

	public BitmapImage action =>
			  ImageHelper.Get(  
								army.isSchedueledNotSent ? "UI/Icons/icon_cmmnds_raid_datetime.png"  :
								army.isRaid ? (
												army.isScheduledToReturn ? "UI/Icons/icon_cmmnds_raid_datetime.png" :
												army.isRepeating ? "UI/Icons/icon_cmmnds_raid_loop.png" : 
												"UI/Icons/icon_cmmnds_raid_once.png" ):
								army.isReturn? "Region/UI/icon_player_own_troops_ret.png"  : 
								army.isSettle ? "Region/UI/icon_player_own_settlement.png" : 
								army.isDefense ? "Region/UI/icon_player_own_support_inc.png" :
								army.isSiege ? "Region/UI/icon_player_alliance_siege.png" :
								"Region/UI/icon_player_own_attack.png");
		
		
		public void ContextRequested(UIElement sender,ContextRequestedEventArgs args)
		{
			args.Handled    = true;
			var flyout = new MenuFlyout();

			if(!army.isReturn || army.isRaid)
			{
				flyout.AddItem("Return",Symbol.Undo,() =>
				{
					if(army.isDefense || army.isSieging) {
						SendTroops.ShowInstance(prior: army);
					}
					else {
						CnVEventReturnTroops.TryReturn(army);
					}
				});
			}
			if( army.canUseWings)
				flyout.AddItem("Speedup",Symbol.Forward,SpeedupDefense);

			flyout.AddItem("Return All",Symbol.Delete,() =>
			{
				new CnVEventReturnTroops(army.sourceC,CnVEventReturnTroops.outgoingAll).EnqueueAsap();
			});
			flyout.AddItem("Return Raids",Symbol.Refresh,() =>
			{
				new CnVEventReturnTroops(army.sourceC,CnVEventReturnTroops.outgoingRaidsFast).EnqueueAsap();
			});
			flyout.ShowContext(sender,args);
		}

		private void SpeedupDefense()
		{
			try
			{
				var aType = Artifact.ArtifactType.Horn;
				var art = Artifact.GetForPlayerRank(aType);
				if(art is null)
				{
					Assert(false);
					return;
				}
				var artifact = art.id;


				var city = army.sourceCity;
				var id = city.outgoing.IndexOf(army);
				Assert(id != -1);
				if(id >= 0)
				{
					var toUse = (int)army.splits;
					var needed = toUse- Player.active.ArtifactCount(artifact);
					if(!Artifact.Get(artifact).IsOkayToUse(toUse))
						return;
					SocketClient.DeferSendStart();

					// do we need to return first
					Assert(army.isDefense);
					// return troops first
					if( army.arrived)
					{
						CnVEventReturnTroops.TryReturn(army); 
					}

					try
					{
						if(needed > 0)
						{
							new CnVEventPurchaseArtifacts((ushort)artifact,(ushort)needed,Player.active.id).EnqueueAsap();
						}

						(new CnVEventUseArtifacts(city.c) { artifactId = (ushort)artifact,count = (ushort)toUse,aux=id }).EnqueueAsap();

					}
					catch(Exception _ex)
					{
						LogEx(_ex);

					}
					finally
					{
						SocketClient.DeferSendEnd();
					}
				}
			}
			catch(Exception _ex)
			{
				LogEx(_ex);

			}
		}

		public void ReinContextRequested(UIElement sender,ContextRequestedEventArgs args)
		{
			args.Handled    = true;
			var flyout = new MenuFlyout();
		

			flyout.AddItem("Return",Symbol.Undo,() =>
			{
				CnVEventReturnTroops.TryReturn(army);
			});
			if( army.canUseWings)
				flyout.AddItem("Speedup",Symbol.Forward,SpeedupDefense);

			flyout.ShowContext(sender,args);
		}


		public void OnPropertyChanged(string members = null) => PropertyChanged?.Invoke(this,new(members));

		public event PropertyChangedEventHandler? PropertyChanged;
	}

	public class ResourceItem:CNotifyPropertyChanged
	{
		internal int r;
		internal void Tapped(object sender,TappedRoutedEventArgs e) => CityUI.Show(
			r switch { 0=>Artifact.ArtifactType.axe,1=>Artifact.ArtifactType.hammer,2=>Artifact.ArtifactType.pike,_=>Artifact.ArtifactType.sickle}, 
			sender);
		internal string incoming;
		internal Brush incomingBrush;

		internal string here;
		internal Brush hereBrush;

		internal string storage;

		internal string production;
		internal Brush productionBrush;
		internal const int height = 28;
		internal ImageSource resIcon => ImageHelper.Get(r switch {
			0=> WoodText.path,
			1=> StoneText.path,
			2=> IronText.path,
			_=> FoodText.path,
			},height
		 );
	}
	public class TradeItem:CNotifyPropertyChanged
	{
		internal TradeOrder trade;
		
		public BitmapImage action { get; set; }
	//	public string sourceCoords=> army.sourceCity.nameAndRemarksAndPlayer;
	//	public string targetCoords=> army.targetCity.nameAndRemarksAndPlayer;
		public string info =>  $"{(trade.isReturning ? trade.returnTime.Format(): trade.time.Format())} { (!isIncoming ? trade.targetCity : trade.sourceCity) }";

		
		internal void InfoClick(object sender,RoutedEventArgs e)
		{
			CityUI.ShowCity(!isIncoming ? trade.targetCid : trade.sourceCid,false);
		}


		internal string toolTip => trade.Format("\n");
		internal bool isIncoming => trade.targetCid == City.build;
		internal bool isOutgoing => !isIncoming;
		internal TradeItem(TradeOrder army)
		{
			this.trade = army;
			
			action =  ImageHelper.Get( trade.isReturning ?  "Region/UI/icon_player_own_troops_ret.png" : 
				isIncoming ?  "Region/UI/icon_player_resource_inc.png" : "Region/UI/icon_player_resource_outgoing.png"  );
			
		}

		public void ContextRequested(UIElement sender,ContextRequestedEventArgs args) {
			args.Handled    = true;
			var flyout = new MenuFlyout();

			if(trade.canCancel && !trade.isCancelled && !trade.isReturning) {

			
			flyout.AddItem("Return",Symbol.Undo,() => {

				new CnVEventCancelTrade(trade.sourceC,trade).EnqueueAsap();

			});
		}	

			flyout.AddItem("Speedup",Symbol.Forward,() =>
			{
				try
				{
					var aType = trade.viaLand ? Artifact.ArtifactType.Wheel : Artifact.ArtifactType.Anchor;
					var art = Artifact.GetForPlayerRank(aType);
					if(art is null)
					{
						Assert(false);
						return;
					}
					var artifact = art.id;
					
					
					var city = trade.sourceCity;
					var id = city.tradesOut.IndexOf(trade);
					Assert(id != -1);
					if(id >= 0)
					{
						const int toUse = 1;
						var needed = toUse- Player.active.ArtifactCount(artifact);
						if(!Artifact.Get(artifact).IsOkayToUse(toUse))
							return;
						SocketClient.DeferSendStart();

						try
						{
							if(needed > 0)
							{
								new CnVEventPurchaseArtifacts((ushort)artifact, (ushort)needed,Player.active.id).EnqueueAsap();
							}

							(new CnVEventUseArtifacts(city.c) { artifactId = (ushort)artifact,count = toUse,aux=id }).EnqueueAsap();

						}
						catch(Exception _ex)
						{
							LogEx(_ex);

						}
						finally
						{
							SocketClient.DeferSendEnd();
						}
					}
				}
				catch(Exception _ex)
				{
					LogEx(_ex);

				}
			});
			
			flyout.ShowContext(sender,args);

		}
		
	}
	
}
