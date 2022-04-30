﻿using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Syncfusion.UI.Xaml.Scheduler;
using Microsoft.UI;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CnV
{
	// Event as appointment

	public sealed partial class TimelineView:Views.UserTab
	{
		public static TimelineView instance;

		internal Player[] playerFilter;
		//public ObservableCollection<ScheduleAppointment> appointments = new();
		public ObservableCollection<SchedulerResource> resources = new();

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


		void  GetDummyAppointments(List<ScheduleAppointment> rv,ServerTime t0, ServerTime t1) {
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
					ResourceIdCollection = resC
				};
				rv.Add(a);
			}
		}

		public TimelineView() {
			Assert(instance == null);
			instance = this;
			this.InitializeComponent();
			

		}


		public override TabPage defaultPage => TabPage.secondaryTabs;


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
		private void UpdateDataI() {
			Assert(isInUpdateI == 0);
			Interlocked.Increment(ref isInUpdateI);
			try {
				Note.Show($"UpdateI start {visibleT0}..{visibleT1}");

				var t0 = visibleT0;
				var t1 = visibleT1;
				var _timeRange = new TimeRangeS(t0,t1);
				var ev = Sim.GetAllEvents().Where(a => a.sfInclude && a.sfTimeRange.Overlaps(_timeRange) &&
				(playerFilter.IsNullOrEmpty()||a.ReferencesPlayers(playerFilter))).Cast<object>().Concat(
					BattleReport.all.Where(a => _timeRange.Overlaps(a.arrival)&&
				(playerFilter.IsNullOrEmpty()||a.ReferencesPlayers(playerFilter))).Cast<object>()
					).ToArray();
				//if(ev.Length == 0) {
				//schedule.ShowBusyIndicator=false;
				//	return;
				//}


				List<ScheduleAppointment> appointments = new();
				var players = new HashSet<Player>();

				foreach(var o in ev) {
					Player p0;
					Player p1 = null;
					ScheduleAppointment rv = null;
					if(o is CnVEvent e) {
						var city = e.city;
						p0 = city.player;



						if(e is CnVEventSendTroops st) {
							p1 = st.troops.sourcePlayer;
						}
						var timeRange = e.sfTimeRange;
						rv = new ScheduleAppointment() {
							Id=e,
							Subject=e.sfName,
							Foreground=e.sfForeground,
							AppointmentBackground=e.sfBackground,
							Location=e.city.FormatName(false,false,false),
							StartTime=timeRange.t0,
							EndTime=timeRange.t1,
							Notes=e.sfNotes
						};

					}
					else if(o is BattleReport b) {
						var a = b.attackArmy;
						rv = new ScheduleAppointment() {
							Id=b,
							Subject=a.uiName,
							Foreground=AppS.Brush(Colors.White),
							AppointmentBackground=a.uiBrush,
							Location=a.targetCity.FormatName(false,false,false),
							StartTime=a.arrival,
							EndTime=a.arrival+TimeSpanS.FromMinutes(50),
							Notes = b.Format()
						};
						p0 = b.targetPlayer;
						p1 = b.sourcePlayer;
						//					var res1 = AddPlayer(p1);
						//	Assert(p0 != p1);
						//		Assert(res0 != res1);

					}
					else {
						Assert(false);
						p0=null;
					}
					if(rv is not null) {
						rv.ResourceIdCollection = new(new[] { p0 });
						players.Add(p0);
						if(p0!=p1 && p1 is not null) {
							rv.ResourceIdCollection.Add(p1);
							players.Add(p1);
						}
						appointments.Add(rv);
					}
				}

				if(appointments.Count == 0) {

					Note.Show("no Appointments!");
					GetDummyAppointments(appointments,t0-TimeSpanS.FromDays(1),t1+TimeSpanS.FromDays(1));
					players.Add(Player.active);
				}
				else {
					var tn0 = t0-TimeSpanS.FromDays(1);
					var tn1 = t1-TimeSpanS.FromDays(1);
					GetDummyAppointments(appointments,tn0,tn0);
					GetDummyAppointments(appointments,tn1,tn1);

					players.Add(Player.active);
				}
				schedule.ItemsSource = appointments;


				Note.Show($"Appointments: {appointments.Count()} Players: {players.Count()}");
				//	schedule.ResourceCollection = resources;
				// players.SyncList(resources,(a,b) => a == b.Id,PlayerToResource);

			}
			catch(Exception ex) {
				LogEx(ex);
			}
			finally {
				Interlocked.Decrement(ref isInUpdateI);
			}

			//			schedule.ShowBusyIndicator=false;
		}

		internal DayOfWeek firstDayOfWeek => (schedule.DisplayDate + TimeSpan.FromDays(1)).DayOfWeek;

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
			Note.Show("loaded timeline");
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

		private void QueryAppointments(object sender,QueryAppointmentsEventArgs e) {
			//		schedule.ShowBusyIndicator=true;
			Note.Show("Query Appointements");
			visibleT0= e.VisibleDateRange.StartDate;
			visibleT1 = e.VisibleDateRange.EndDate+TimeSpanS.FromDays(1);
			if(isInUpdateI==0) {
				//				AppS.QueueOnUIThread(UpdateDataI);
				UpdateDataI();
			}
			else {
				Note.Show("Skipped Query Appointements");
			
			}
		}

		private void PlayerFilterChanged(object sender,PlayerListBox.ChangedEventArgs e) {
			playerFilter = e.players;
			if(isInUpdateI==0) {
				AppS.QueueOnUIThread(UpdateDataI);
			}
		}

		private void ItemTapped(object sender,AppointmentTappedArgs e) {
			var a = e.Appointment;
			var id = a?.Id;
			if(id is BattleReport br) {
				BattleReportDialog.ShowInstance(br);
			}
			else if(id is CnVEventSendTroops st) {
				if(st.troops.isBossHit && BossReport.TryGet(st.troops,out var bossReport))
					BossReportDialog.ShowInstance(bossReport);
				else
					SendTroops.ShowInstance(prior: st.troops);
			}

		}

		
	}
}
