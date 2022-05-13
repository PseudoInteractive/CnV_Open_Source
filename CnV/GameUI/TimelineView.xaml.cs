using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Syncfusion.UI.Xaml.Scheduler;
using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CnV
{
	// Event as appointment

	public sealed partial class TimelineView:Views.UserTab
	{
		public static TimelineView instance;

		internal PlayerId[] playerFilter = Array.Empty<PlayerId>();
		//public ObservableCollection<ScheduleAppointment> appointments = new();
		//	public ObservableCollection<SchedulerResource> resources = new();

		//public ResourceGroupType[] resourceGroupTypes = { ResourceGroupType.None,ResourceGroupType.Resource,ResourceGroupType.Date };
		//internal ResourceGroupType resourceGrouping = ResourceGroupType.None;
		//public ResourceGroupType ResourceGrouping {
		//	get => resourceGrouping; set {
		//		if(value!=resourceGrouping) {
		//			resourceGrouping=value;
		//			AppS.QueueOnUIThread(() => {
		//				schedule.ResourceGroupType = resourceGrouping;

		//			}
		//			);

		//		}
		//	}
		//}
		//	List<SchedulerResource> resources = new();
		public bool ShowArtifacts {
			get => showArtifacts; set {
				if(showArtifacts!=value) {
					showArtifacts=value;
					UpdateDataI();
				}
			}
		}

		public bool ShowQuests {
			get => showQuests; set {
				if(showQuests!=value) {
					showQuests=value;
					UpdateDataI();
				}
			}
		}
		public static string GetNotes(object i,string notes) {
			return (i is ITimelineItem t) ? t.TlNotes() : notes;
		}
		public static Brush GetColor(object i,Brush background) {
			return background;
		}
		void GetDummyAppointments(List<ScheduleAppointment> rv,ServerTime t0,ServerTime t1) {
			var resC = new ObservableCollection<object> { Player.active };

			for(;t0 <= t1;t0 += TimeSpanS.FromDays(1)) {
				var a = new ScheduleAppointment() {
					Id=new object(),
					Subject="Siesta",
					Foreground=AppS.Brush(Colors.White),
					AppointmentBackground=AppS.Brush(Colors.Black),
					Location="Spain",
					StartTime = t0+ TimeSpanS.FromHours(13),
					Notes="Yay!",
					EndTime = t0 + TimeSpanS.FromHours(15),
					//	ResourceIdCollection = resC
				};
				rv.Add(a);
			}
		}

		public TimelineView() {
			Assert(instance == null);
			instance = this;
			this.InitializeComponent();


		}

		public static bool IsVisible() => instance.isFocused;
		public override TabPage defaultPage => TabPage.secondaryTabs;

		public static void NotifyChange() {
			// called from UI thread

			if(!IsVisible())
				return;
			if(instance.isInUpdateI==0)
				AppS.QueueOnUIThread(instance.UpdateDataI);
	
		}

		override public Task VisibilityChanged(bool visible,bool longTerm) {
			//   Log("Vis change" + visible);


			if(visible) {
				if(isInUpdateI==0)
					AppS.QueueOnUIThread(UpdateDataI);
			}
			return base.VisibilityChanged(visible,longTerm: longTerm);
		}
		internal DateTime initialDisplayDate => Sim.simTime;
		int isInUpdateI;

		static ScheduleAppointment CreateAppointment(ITimelineItem e) {
			var timeRange = e.TlTimeRange();
			return new ScheduleAppointment() {
				Id=e,
				Subject=e.TlName(),
				Foreground=e.TlForeground(),
				AppointmentBackground=e.TlBackground(),
				StartTime=timeRange.t0,
				EndTime=timeRange.t1
			};
		}

		private void UpdateDataI() {
			Assert(isInUpdateI == 0);
			Interlocked.Increment(ref isInUpdateI);
			try {
//				Note.Show($"UpdateI start {visibleT0}..{visibleT1}");

				var t0 = visibleT0;
				var t1 = visibleT1;
				var _timeRange = new TimeRangeS(t0,t1);

				//if(ev.Length == 0) {
				//schedule.ShowBusyIndicator=false;
				//	return;
				//}


				List<ScheduleAppointment> appointments = new();
				var players = new HashSet<Player>();
				var alliance = Alliance.MyIdOr0; // only have visibility of alliance members

				foreach(var e in Sim.GetAllEvents()) {



					if(!(e.tlInclude && e.TlTimeRange().Overlaps(_timeRange)))
						continue;
					
				
					if(!IsRelevant(e.TlReferences()))
						continue;

					if(e is CnVEventReturnTroops rt) {
						foreach(var ret in rt.returned) {
							var bad = appointments.FindIndex(a => a.Id is CnVEventSendTroops st && st.troops == ret);
							if(bad != -1) {
								appointments.RemoveAt(bad);
							}
							else {
								if(!ret.isRaid) {
									//Assert(ret.isDefense); // retruning stationed defense
								}

							}

						}
						continue;
					}
					Player p0;
					Player p1 = null;

					var city = e.city;
					p0 = city.player;



					if(e is CnVEventSendTroops st) {
						p1 = st.troops.sourcePlayer;
					}
					var rv = CreateAppointment(e);
					appointments.Add(rv);

				}
				foreach(var b in BattleReport.all) {
					if(!(_timeRange.Overlaps(b.arrival) ))
						continue;
					if(!IsRelevant((b.attackerPid, b.defenderPid)))
						continue;

					var a = b.attackArmy;
					var rv = CreateAppointment(b);
					var p0 = b.targetPlayer;
					var p1 = b.sourcePlayer;
					//					var res1 = AddPlayer(p1);
					//	Assert(p0 != p1);
					//		Assert(res0 != res1);


					appointments.Add(rv);
				}
				foreach(var b in BossReport.reports) {
					if(!(_timeRange.Overlaps(b.time) && (playerFilter.Contains(b.playerId))))
						continue;
					var rv = CreateAppointment(b);


					appointments.Add(rv);
				}
				foreach(var b in TimelineNews.all) {
					if(!(_timeRange.Overlaps(b.t) ))
						continue;
					var rv = CreateAppointment(b);


					appointments.Add(rv);
				}



				if(appointments.Count == 0) {

//					Note.Show("no Appointments!");
					GetDummyAppointments(appointments,t0-TimeSpanS.FromDays(1),t1+TimeSpanS.FromDays(1));
					players.Add(Player.active);
				}
				else {
					//var tn0 = t0-TimeSpanS.FromDays(1);
					//var tn1 = t1-TimeSpanS.FromDays(1);
					//GetDummyAppointments(appointments,tn0,tn0);
					//GetDummyAppointments(appointments,tn1,tn1);

					//players.Add(Player.active);
				}
	//			Note.Show($"Appointments: {appointments.Count()} Players: {players.Count()}");
				schedule.ItemsSource = appointments;


				//	schedule.ResourceCollection = resources;
				// players.SyncList(resources,(a,b) => a == b.Id,PlayerToResource);
				// Must be part of player list we are filtering for
			// And must have intel on at least one participant
			bool IsRelevant( (ushort p0, ushort p1) ps) {
				return alliance== 0 ? playerFilter.ContainsAny(ps)&(Player.IsSubOrMe(ps.p0)|Player.IsSubOrMe(ps.p1) ): (playerFilter.ContainsAny(ps) && (Player.Get(ps.p0).allianceId==alliance |
																				Player.Get(ps.p1).allianceId==alliance));
			}

			}
			catch(Exception ex) {
				LogEx(ex);
			}
			finally {
				Interlocked.Decrement(ref isInUpdateI);
			}

			

			//			schedule.ShowBusyIndicator=false;
		}

		//		internal DayOfWeek firstDayOfWeek => (schedule.DisplayDate + TimeSpan.FromDays(1)).DayOfWeek;

		internal static ImageSource PlayerImage(object id) {
			var p = id as Player;
			return p.avatarImageSmall;
		}

		bool hasLoaded;
		private void OnLoaded(object sender,RoutedEventArgs e) {
			// Add series
			//	UpdateData();
			if(hasLoaded)
				return;
			hasLoaded=true;
			//Note.Show("loaded timeline");
			//this.schedule.TimelineViewSettings.TimeInterval = new System.TimeSpan(0, 60, 0); // hourly ticks

			try {
				Interlocked.Increment(ref isInUpdateI);

				if(PlayerFilter.SelectedItems.IsNullOrEmpty())
					PlayerFilter.SelectedItems.Add(Player.active);
			}
			finally {
				Interlocked.Decrement(ref isInUpdateI);
			}

			//	var t = (Sim.simTime);// - TimeSpanS.FromDays(6)).dateTimeDT;

			this.schedule.AppointmentEditorOpening += Schedule_AppointmentEditorOpening;

			//ResetResources(null,EventArgs.Empty);
			//	Player.playerListChanged -= ResetResources;
			//	Player.playerListChanged += ResetResources;
		}

		private SchedulerResource PlayerToResource(Player p1) {
			var cs = (p1.pid+1).RandomUIColors(0.75f,0.875f);
			return new() {
				Id = p1,
				Name= p1.shortName, // back ground by  player?
				Background = AppS.Brush(cs.c0),
				Foreground=AppS.Brush(Colors.White),


			};
		}
		//private void ResetResources(object sender, EventArgs args) {
		//	var resources = new List<SchedulerResource>();
		//	foreach(var p1 in Player.allSorted) {

		//		var cs = (p1.pid+1).RandomUIColors(0.5f,0.875f);
		//		SchedulerResource res1 = new() {
		//			Id = p1,
		//			Name= p1.shortName, // back ground by  player?
		//			Background = AppS.Brush(cs.c0),
		//			Foreground=AppS.Brush(cs.c1),


		//		};
		//		Assert(!resources.Any(a => a.Id == p1));

		//		resources.Add(res1);
		//	}
		//	this.schedule.ItemsSource = GetDummyAppointments();
		//	this.schedule.ResourceCollection = resources;
		//}

		private void Schedule_AppointmentEditorOpening(object sender,AppointmentEditorOpeningEventArgs e) {
			e.AppointmentEditorOptions = AppointmentEditorOptions.All | (~AppointmentEditorOptions.AllDay & ~AppointmentEditorOptions.Recurrence &~AppointmentEditorOptions.TimeZone);
		}

		private void scheduler_ReminderAlertOpening(object sender,ReminderAlertOpeningEventArgs e) {
			Note.Show(e.FormatJson());
		}

		ServerTime visibleT0;
		ServerTime visibleT1;
		internal static bool showArtifacts;
		internal static bool showQuests;

		private void QueryAppointments(object sender,QueryAppointmentsEventArgs e) {
			//		schedule.ShowBusyIndicator=true;
		//	Note.Show("Query Appointements");
			visibleT0= e.VisibleDateRange.StartDate;
			visibleT1 = e.VisibleDateRange.EndDate+TimeSpanS.FromDays(1);
			if(isInUpdateI==0) {
				//				AppS.QueueOnUIThread(UpdateDataI);
				UpdateDataI();
			}
			else {
			//	Note.Show("Skipped Query Appointements");

			}
		}

		private void PlayerFilterChanged(object sender,PlayerListBox.ChangedEventArgs e) {
			var prior = playerFilter;
			Assert(e.players is not null);
			Assert(playerFilter is not null);
			playerFilter = e.players.IsNullOrEmpty() ? new[] { Player.myId } : e.players;

			if(isInUpdateI==0 && !prior.SequenceEqual(playerFilter)) {
				AppS.QueueOnUIThread(UpdateDataI);
			}
		}

		private void ItemTapped(object sender,AppointmentTappedArgs e) {
			var a = e.Appointment;
			if(a is not null) {
				var i = a.Id as ITimelineItem;
				i?.TlClicked();

			}
			//var id = a?.Id;
			//if(id is BattleReport br) {
			//	BattleReportDialog.ShowInstance(br);
			//}
			//else if(id is CnVEventSendTroops st) {
			//	if(st.troops.isBossHit && BossReport.TryGet(st.troops,out var bossReport))
			//		BossReportDialog.ShowInstance(bossReport);
			//	else
			//		SendTroops.ShowInstance(prior: st.troops);
			//}

		}

		private void ContextOpening(object sender,SchedulerContextFlyoutOpeningEventArgs e) {
			
			//ServerTime t;
			//if(e.MenuType == SchedulerContextFlyoutType.Appointment) {
			//	var menuItem = e.ContextMenu.Items[0] as MenuFlyoutItem;
			//	var id = 
			//	menuItem.IsEnabled=false;
			//	t = e.MenuInfo.Appointment.StartTime;
			//}

			//menuItem.Click += (_,_) => Sim.GotoTime( t );
		}

		private void HistoricClick(object sender,RoutedEventArgs e) {
			var mf = sender as MenuFlyoutItem;
			var mi = mf.DataContext as SchedulerContextFlyoutInfo;
			if(mi.Appointment is not null) {
				Sim.GotoTime((ServerTime)mi.Appointment.StartTime - TimeSpanS.FromMinutes(1));
			}
			else if(mi.DateTime is not null) {
				Sim.GotoTime(mi.DateTime.Value);

			}
		}
	}
}
