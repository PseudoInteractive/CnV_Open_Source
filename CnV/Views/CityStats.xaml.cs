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

namespace CnV
{
	public sealed partial class CityStats:UserControl,INotifyPropertyChanged
	{
		public static CityStats instance;
		public CityStats()
		{
			this.InitializeComponent();
			buildQueue.CollectionChanged+=BuildQueue_CollectionChanged;
		}
		
		public event PropertyChangedEventHandler? PropertyChanged;
		public void OnPropertyChanged(string? member = null)
		{
			if (this.PropertyChanged is not null) 
				AppS.DispatchOnUIThread(() => PropertyChanged?.Invoke(this,new(member)));
		}
		public static void Changed(string? member = null)
		{
			if(instance is not null)
				instance.OnPropertyChanged(member);
		}
		private void CityBox_SelectionChanged(object sender,SelectionChangedEventArgs e)
		{
			var sel = cityBox.SelectedItem as City;
			if(sel != null && sel.cid != City.build)
			{
				City.ProcessCoordClick(sel.cid,false,AppS.keyModifiers);
			}
		}
		

		private void CitySubmitted(ComboBox sender,ComboBoxTextSubmittedEventArgs args)
		{
			var text = args.Text.ToLower();

			var items = City.gridCitySource;
			foreach(var it in items)
			{
				// its good
				if(it.nameAndRemarks == text)
				{
					return;
				}
			}

			args.Handled = true;
			//foreach (var it in items)
			//{
			//	if (it.nameAndRemarks.ToLower().StartsWith(text))
			//	{
			//		sender.Text = it.nameAndRemarks;
			//		sender.SelectedItem = it;
			//		return;
			//	}
			//}
			// try contains
			foreach(var it in items)
			{
				if(it.nameAndRemarks.ToLower().Contains(text))
				{
					sender.Text         = it.nameAndRemarks;
					sender.SelectedItem = it;
					return;
				}
			}
			// todo!
		}
		private static void BuildQueue_CollectionChanged(object? sender,NotifyCollectionChangedEventArgs e)
		{
			// invalidate
		
			instance?.cityQueueChangeDebounce.Go();
//			displayQueue = BuildQueueType.Empty;
			//Log(e.Action);
			//LogJson(e);
			// This is coming from the UI to us
			// Validate that the queue is still valid

			//switch(e.Action)
			//{
			//	case NotifyCollectionChangedAction.Move:
			//		{
						
			//			var city = instance.city;
			//			if(city.AttempMove(e.OldStartingIndex,e.NewStartingIndex))
			//			{
			//			}
			//			NotifyBuildQueueChange();
			//			break;
			//		}
			//	case NotifyCollectionChangedAction.Remove:
			//		{
			//			return;
			//			var city = instance.city;
			//			if(city.AttempRemove(e.OldStartingIndex))
			//			{
			//			}
			//			NotifyBuildQueueChange();
			//			break;
			//		}
			//	case NotifyCollectionChangedAction.Add:
			//		{

			//			return;
			//			var city = instance.city;
			//			if(city.AttemptAdd(e.NewStartingIndex, e.NewItems.Cast<BuildItem>().Select( a => a.op ).ToArray() ))
			//			{
			//			}
			//			NotifyBuildQueueChange();
			//			break;
			//		}
			//	default:
			//		Assert(false);
			//		break;
			//}
		}

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
		static void UpdateAllQueues()
		{
			UpdateBuildQueue();
			UpdateCommandItems();
			UpdateRecruitQueue();
			UpdateTradeItems();
			
			ClearLastDisplayed();
			Changed();
			
			

		}

		static void UpdateBuildQueue()
		{
			// Don't notify whole we are doing stuff
			try
			{
				instance.buildQueue.CollectionChanged -= BuildQueue_CollectionChanged;
				//var firstVisible = instance.buildQueueListView.vis
				var anyRemoved = false;
				var city = instance.city;
				var displayQueue = city.buildQueue;
				lastSynchronizedQueue = displayQueue;
				int lg = displayQueue.Length;
				var bq = instance.buildQueue;
				//for(int i = bq.Count;--i>= 0;)
				//{
				//	var op = bq[i].op;
				//	if(!displayQueue.Any(a => a == op))
				//	{
				//		bq.RemoveAt(i);
				//	}
				//}

				// Add or update
				for(int i = 0;i<lg;++i)
				{
					var op = displayQueue[i];
					int cur = -1;
					for(int j = i;j<bq.Count;++j)
					{
						if(bq[j].op == op)
						{
							cur = j;
							break;
						}
					}
					if(cur == -1)
					{
						var bi = new BuildItem(op,city);
						bq.Insert(i,bi);
					}
					else
					{
						if(cur != i)
						{
							bq.Move(cur,i);

						}

					}
				}
				// sequence is done, remove extras if any
				for(var i = bq.Count;--i>= lg;)
				{
					anyRemoved=true;
					bq.RemoveAt(i);
				}

				Assert(bq.Count == lg);
				for(int i=0;i<lg;++i)
				{
					Assert(bq[i].op == displayQueue[i]);
				}
				// keep first in view
				if(anyRemoved && bq.Any() )
				{
					instance.buildQueueListView.ScrollIntoView(bq.First());
				}
			}
			catch ( Exception ex)
			{
				LogEx(ex);
			}
			// restore callback
			instance.buildQueue.CollectionChanged += BuildQueue_CollectionChanged;

		}
		static void UpdateRecruitQueue()
		{
			// Don't notify whole we are doing stuff
			try
			{
				
				//var firstVisible = instance.buildQueueListView.vis
				var anyRemoved = false;
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
				for(int i = 0;i<lg;++i)
				{
					var op = displayQueue[i];
					int cur = -1;
					for(int j = i;j<bq.Count;++j)
					{
						if(bq[j].op == op)
						{
							cur = j;
							break;
						}
					}
					if(cur == -1)
					{
						var bi = new RecruitItem(op,city);
						bq.Insert(i,bi);
					}
					else
					{
						if(cur != i)
						{
							bq.Move(cur,i);

						}

					}
				}
				// sequence is done, remove extras if any
				for(var i = bq.Count;--i>= lg;)
				{
					anyRemoved=true;
					bq.RemoveAt(i);
				}

				Assert(bq.Count == lg);
				for(int i=0;i<lg;++i)
				{
					Assert(bq[i].op == displayQueue[i]);
				}
				// keep first in view
				if(anyRemoved && bq.Any() )
				{
					instance.RecruitQueueListView.ScrollIntoView(bq.First());
				}
			}
			catch ( Exception ex)
			{
				LogEx(ex);
			}
			// restore callback

		}

		static void UpdateCommandItems()
		{
			// Don't notify whole we are doing stuff
			try
			{
				
				//var firstVisible = instance.buildQueueListView.vis
				var anyRemoved = false;
				var city = instance.city;
				var displayQueue = city.outgoing;
				int lg = displayQueue.Length;
				var bq = instance.commandItems;
			

				// Add or update
				for(int i = 0;i<lg;++i)
				{
					var op = displayQueue[i];
					int cur = -1;
					for(int j = i;j<bq.Count;++j)
					{
						if(bq[j].army == op )
						{
							cur = j;
							break;
						}
					}
					if(cur == -1)
					{
						var bi = new CommandItem(op);
						bq.Insert(i,bi);
					}
					else
					{
						if(cur != i)
						{
							bq.Move(cur,i);

						}

					}
				}
				// sequence is done, remove extras if any
				for(var i = bq.Count;--i>= lg;)
				{
					anyRemoved=true;
					bq.RemoveAt(i);
				}

				Assert(bq.Count == lg);
				for(int i=0;i<lg;++i)
				{
					Assert(bq[i].army == displayQueue[i]);
				}
				// keep first in view
				if(anyRemoved && bq.Any() )
				{
					instance.CommandsListView.ScrollIntoView(bq.First());
				}
			}
			catch ( Exception ex)
			{
				LogEx(ex);
			}
			// restore callback

		}
		static void UpdateTradeItems()
		{
			// Don't notify while we are doing stuff
			try
			{
				
				//var firstVisible = instance.buildQueueListView.vis
				
				var city = instance.city;
				var displayQueue = city.tradesOut.Concat(city.tradesIn).OrderBy(a=>a.isReturning ? a.returnTime : a.arrival).ToArray();
				int lg = displayQueue.Length;
				var bq = instance.tradeItems;
				bq.Clear();

				// Add or update
				for(int i = 0;i<lg;++i)
				{
					
					var bi = new TradeItem(displayQueue[i]);
					bq.Add(bi);
				}
				

			}
			catch ( Exception ex)
			{
				LogEx(ex);
			}
			// restore callback

		}
		internal DebounceA cityQueueChangeDebounce = new(UpdateAllQueues) { runOnUiThread=true,debounceDelay=100 };

		internal string TroopsHomeS => city?.troopsHere.Format(separator: ',');
		internal string TroopsOwnedS => city?.troopsOwned.Format(separator: ',');
		internal string IncomingReinforcements => city?.incomingReinforcements.Format(separator: ',');

		public string commandsTitle => $"Commands {commandItems.Count}/{city.commandSlots}";
		public string tradesTitle => $"Trades {tradeItems.Count}";

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
		static bool updateInProgress;
		public void UpdateUI()
		{
			try
			{
				//	var i = CityStats.instance;
				
				if(City.GetBuild().IsInvalid())
					return;
				// building counts

				if(updateInProgress)
					return;
				updateInProgress = true;

				AppS.DispatchOnUIThreadIdle(() =>
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
						var t = CnVServer.simTime;
#if DEBUG
						ShellPage.instance.timeDisplay.Text = $"{t.FormatWithYear()} Frame {GameClient.renderFrame/60}";
#else
						ShellPage.instance.timeDisplay.Text = t.FormatWithYear();
#endif
						if(!hasBeenDisplayed)
						{
							ResToolTip.Content=
								$"Storage:\n{city.stats.storage.Format("\n")}";

							
						}
						if(expResource.IsExpanded)
						{
							var resources = city.SampleResources();
							var panels = expResource.Child<CommunityToolkit.WinUI.UI.Controls.WrapPanel>().Children<StackPanel>();
							for(var r = 0;r< CnV.Resources.idCount;r++)
							{
								var ch = panels.ElementAt(r).Children<TextBlock>();
								Assert(ch.Count==2);
								var txt = ch.ElementAt(0);
								var prod = ch.ElementAt(1);

								//				var txt = r switch { 0 => res0, 1 => res1, 2 => res2, _ => res3 };
								//				var prod = r switch { 0 => prod0, 1 => prod1, 2 => prod2, _ => prod3 };

								var res = resources[r];
								var storage = city.stats.storage[r];
								txt.UpdateLazy($"{res:N0}",(res >= storage ?
									Colors.Red : res >= storage*3/4 ?
									Colors.Orange : res == 0 ?
									Colors.LightGray : Colors.LightGreen));

								var p = city.stats.production[r];
								prod.UpdateLazy($"{CnV.Resources.names[r]}/h:{p:+#,#;-#,#;' --'}",(p switch
								{
									> 0 => Colors.White,
									< 0 => Colors.Yellow,
									_ => Colors.LightGray

								}));
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
							foreach(var b in recruitQueue)
							{
								b.UpdateText();
							}
						}

						if(!hasBeenDisplayed)
						{
							var txt = (expBuildings.Header as DependencyObject).Child<TextBlock>(1);
							txt.UpdateLazy($"Buildings: [{bdd.buildingCount}/{bdd.townHallLevel*10}]");
							var hasHammerTime = city.hammerTime > 0;
							queueText.UpdateLazy($"Queue cs:{city.stats.cs:N0}{ (hasHammerTime ? " +" +city.hammerTime.Format() : "" )}", hasHammerTime ? Colors.GreenYellow : Colors.White); 
						
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
						updateInProgress = false;

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

		private void scroll_SizeChanged(object sender,ScrollViewerViewChangedEventArgs e)
		{
			if(e.IsIntermediate)
				return;
			ShellPage.updateHtmlOffsets.SizeChanged();
		}
		private void scroll_SizeChanged4(object sender,SizeChangedEventArgs e)
		{
			ShellPage.updateHtmlOffsets.SizeChanged();

		}
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


		internal static void Invalidate()
		{
			
			instance?.cityQueueChangeDebounce.Go();
		
			
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
		internal ObservableCollection<TradeItem> tradeItems= new();
		private void UserControl_Loaded(object sender,RoutedEventArgs e)
		{
			Assert(instance is null);
			instance = this;
			City.buildCityChanged+=Invalidate;
			

		}

		

		

		private async void buildQueueListView_DragItemsCompleted(ListViewBase sender,DragItemsCompletedEventArgs args)
		{
			try
			{
				Log(args.DropResult);
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
					var items = args.Items.Select( (a) =>  buildQueue.IndexOf( a as BuildItem));
					
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

		private void buildQueueListView_ItemClick(object sender,ItemClickEventArgs e)
		{

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

		private void WoodTap(object sender,TappedRoutedEventArgs e)=> Artifact.Show( Artifact.ArtifactType.axe, sender);
		private void StoneTap(object sender,TappedRoutedEventArgs e)=> Artifact.Show( Artifact.ArtifactType.hammer, sender);
		private void IronTap(object sender,TappedRoutedEventArgs e)=> Artifact.Show( Artifact.ArtifactType.pike, sender);
		private void FoodTap(object sender,TappedRoutedEventArgs e)=> Artifact.Show( Artifact.ArtifactType.sickle, sender);
		private void GoldTap(object sender,TappedRoutedEventArgs e)=> Artifact.Show( Artifact.ArtifactType.chest, sender);

		private void QueueRightTapped(object sender,RightTappedRoutedEventArgs e)=>	Artifact.Show( Artifact.ArtifactType.medallion, sender);

		private void EnlistmentContextRequested(UIElement sender,ContextRequestedEventArgs args)
		{
			args.Handled    = true;
			var flyout = new MenuFlyout();
			flyout.SetXamlRoot(sender);
			
			flyout.AddItem("Amulet..",Symbol.OutlineStar,() =>
			{
				Artifact.Show( Artifact.ArtifactType.amulet, sender);
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
					new CnVEventCancelBuildQueue(city.worldC).Execute();
				});
			}
			// Todo: Sort

			if(args.TryGetPosition(sender,out var c))
			{
				flyout.ShowAt(sender,c);
			}
			else if(args.TryGetPosition(CityStats.instance,out var c2))
			{
				flyout.ShowAt(CityStats.instance,c2);
			}
			else
			{
				flyout.ShowAt(sender,new());
				Assert(false); 
			}
			
		}

		private void CityIconTapped(object sender,TappedRoutedEventArgs e)
		{
			var city= (sender as FrameworkElement)?.DataContext as City;
			if(city is not null)
			{
				e.Handled=true;
				city.DoClick();
			}
		}

		private void EnlistmentClick(object sender,RoutedEventArgs e)
		{
			RecruitDialog.ShowInstance(city);
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
		public BuildQueueItem op;

		internal City city;
		public BuildItem(BuildQueueItem item, City city)
		{
			this.city = city;
			op = item;
			image = CityBuild.GetBuildingImage(item.isMove ? Building.bidMove : item.bid,size);
			var u = op.unpack;
			opText = u.isMove ? "Move" : u.isDemo ? "Destroy" : u.isBuild ? $"Build{(u.pa==false ? " p" : "") }" : u.isDowngrade ? $"Down to {u.elvl}" : $"Up to {u.elvl}";
			UpdateText();
		}
		public void UpdateText()
		{
			try
			{
				var q = city.buildQueue;
				TimeSpanS dt;
				if(q.Any() && q[0]== op && city.buildItemEndsAt.isNotZero)
					dt = city.buildItemEndsAt - CnVServer.simTime;
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
			flyout.SetXamlRoot(sender);
			
			flyout.AddItem("Medallion..",Symbol.OutlineStar,() =>
			{
				Artifact.Show( Artifact.ArtifactType.medallion, sender);
			});
			flyout.AddItem(StandardUICommandKind.Delete.Create( () =>
			{
				var index = instance.buildQueue.IndexOf(this);
				if(index != -1)
					city.RemoveWithDependencies(new(new[]{ index } ),lastSynchronizedQueue);
				else
					Note.Show("Something changed");
			}));
			flyout.AddItem("Cancel Selected",Symbol.Remove,() =>
			{
				var sel = instance.buildQueueListView.SelectedItems;

				city.RemoveWithDependencies(sel.Any() ? new( sel.Select( x=> instance.buildQueue.IndexOf(x as BuildItem ))) :new(new[] { instance.buildQueue.IndexOf(this) }) ,lastSynchronizedQueue);
			});
			flyout.AddItem("Cancel all",Symbol.Cut,() =>
			{
				var sel = instance.buildQueueListView.SelectedItems;
				new CnVEventCancelBuildQueue(city.worldC).Execute();
			});
			flyout.AddItem("Sort",Symbol.Sort,() =>
			{
				var sel = instance.buildQueueListView.SelectedItems;
				new CnVEventSortBuildQueue(city.worldC).Execute();
			});
			flyout.AddItem("Move To Front",Symbol.Up,() =>
			{
				var index = instance.buildQueue.IndexOf(this);
				if(index != -1)
					city.AttemptMove(index,0,lastSynchronizedQueue);
			}); 
			flyout.AddItem("Move To End",Symbol.Back,() =>
			{
				var index = instance.buildQueue.IndexOf(this);
				if(index != -1)
					city.AttemptMove(index,city.buildQueue.Length-1,lastSynchronizedQueue);
			});
			
			// Todo: Sort

			if(args.TryGetPosition(sender,out var c))
			{
				flyout.ShowAt(sender,c);
			}
			else if(args.TryGetPosition(CityStats.instance,out var c2))
			{
				flyout.ShowAt(CityStats.instance,c2);
			}
			else
			{
				flyout.ShowAt(sender,new());
				Assert(false); 
			}
			//VisualTreeHelper.GetParent(args.OriginalSource
			//LogJson(args);
			//Log(args.OriginalSource);
			//LogJson(sender);
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
			UpdateText();
		}
		public void UpdateText()
		{
			try
			{
				var q = city.recruitQueue;
				TimeSpanS dt;
				if(q.Any() && q[0]== op && city.recruitItemEndsAt.isNotZero)
					dt = city.recruitItemEndsAt - CnVServer.simTime;
				else
					dt = new(op.RecruitTimeRequired(city));
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
		
		public void OnPropertyChanged(string members = null) => PropertyChanged?.Invoke(this,new(members));

		public event PropertyChangedEventHandler? PropertyChanged;
	}
	public class CommandItem:INotifyPropertyChanged
	{
		internal Army army;
		
		public BitmapImage action { get; set; }
	//	public string sourceCoords=> army.sourceCity.nameAndRemarksAndPlayer;
	//	public string targetCoords=> army.targetCity.nameAndRemarksAndPlayer;
		public string info => $"{army.arrival.Format()} to {army.targetPlayer}";

		internal void SourceClick(object sender,RoutedEventArgs e)
		{
			CityUI.ShowCity(army.sourceCid,false);
		}
		internal void TargetClick(object sender,RoutedEventArgs e)
		{
			CityUI.ShowCity(army.targetCid,false);
		}


		internal string toolTip => army.troops.Format(separator:',');

		public CommandItem(Army army)
		{
			this.army = army;
			
			action =  ImageHelper.Get(  army.isReturn? "Region/UI/icon_player_own_troops_ret.png"  : 
								army.isSettle ? "Region/UI/icon_player_own_settlement.png" : 
								army.isDefense ? "Region/UI/icon_player_own_support_inc.png" :
								"Region/UI/icon_player_own_attack.png");
			
		}
		
		public void ContextRequested(UIElement sender,ContextRequestedEventArgs args)
		{
			args.Handled    = true;
			var flyout = new MenuFlyout();
			flyout.SetXamlRoot(sender);
			
			
			flyout.AddItem("Return",Symbol.Undo,() =>
			{
				new CnVEventReturnTroops(army).EnqueueAsap();
				
			});
			
			flyout.AddItem("Cancel Selected",Symbol.Cut,() =>
			{
			});
			flyout.AddItem("Cancel all",Symbol.Clear,() =>
			{
			});
			
			// Todo: Sort

			if(args.TryGetPosition(sender,out var c))
			{
				flyout.ShowAt(sender,c);
			}
			else if(args.TryGetPosition(CityStats.instance,out var c2))
			{
				flyout.ShowAt(CityStats.instance,c2);
			}
			else
			{
				flyout.ShowAt(sender,new());
				Assert(false); 
			}
			//VisualTreeHelper.GetParent(args.OriginalSource
			//LogJson(args);
			//Log(args.OriginalSource);
			//LogJson(sender);
		}
		public void OnPropertyChanged(string members = null) => PropertyChanged?.Invoke(this,new(members));

		public event PropertyChangedEventHandler? PropertyChanged;
	}

	public class TradeItem:INotifyPropertyChanged
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


		internal string toolTip => trade.resources.Format(separator:",");
		internal bool isIncoming => trade.targetCid == City.build;
		internal bool isOutgoing => !isIncoming;
		internal TradeItem(TradeOrder army)
		{
			this.trade = army;
			
			action =  ImageHelper.Get( trade.isReturning ?  "Region/UI/icon_player_own_troops_ret.png" : 
				isIncoming ?  "Region/UI/icon_player_resource_inc.png" : "Region/UI/icon_player_resource_outgoing.png"  );
			
		}
		
		public void ContextRequested(UIElement sender,ContextRequestedEventArgs args)
		{
			args.Handled    = true;
			var flyout = new MenuFlyout();
			flyout.SetXamlRoot(sender);
			
			
			//flyout.AddItem("Return",Symbol.Undo,() =>
			//{
			//	new CnVEventReturnTroops(army).EnqueueAsap();
				
			//});
			
			flyout.AddItem("Cancel Selected",Symbol.Cut,() =>
			{
			});
			flyout.AddItem("Cancel all",Symbol.Clear,() =>
			{
			});
			
			// Todo: Sort

			if(args.TryGetPosition(sender,out var c))
			{
				flyout.ShowAt(sender,c);
			}
			else if(args.TryGetPosition(CityStats.instance,out var c2))
			{
				flyout.ShowAt(CityStats.instance,c2);
			}
			else
			{
				flyout.ShowAt(sender,new());
				Assert(false); 
			}
			//VisualTreeHelper.GetParent(args.OriginalSource
			//LogJson(args);
			//Log(args.OriginalSource);
			//LogJson(sender);
		}
		public void OnPropertyChanged(string members = null) => PropertyChanged?.Invoke(this,new(members));

		public event PropertyChangedEventHandler? PropertyChanged;
	}
}
