using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Syncfusion.UI.Xaml.Charts;

using CnV;
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
		public ObservableCollection<ScheduleAppointment> appointments = new();
		public ObservableCollection<SchedulerResource> resources = new();
		public ResourceGroupType[] resourceGroupTypes = { ResourceGroupType.None,ResourceGroupType.Resource,ResourceGroupType.Date };
		internal ResourceGroupType resourceGrouping = ResourceGroupType.None;
		public ResourceGroupType ResourceGrouping {
			get => resourceGrouping; set {
				if(value!=resourceGrouping) {
					resourceGrouping=value;
					AppS.QueueOnUIThread(() => {
						schedule.ResourceGroupType = resourceGrouping;
					
					}
					);
					
				}
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
				AppS.QueueOnUIThread(UpdateDataI);
			}
			return base.VisibilityChanged(visible,longTerm: longTerm);
		}

		private void UpdateDataI() {
			var t0 = visibleT0;
			var t1 = visibleT1;
			var timeRange = new TimeRangeS(t0,t1);
			var ev = Sim.retired.Where(a => a.sfInclude && a.sfTimeRange.Overlaps(timeRange) && 
			(playerFilter.IsNullOrEmpty()||a.ReferencesPlayers(playerFilter) )).Cast<object>().Concat(
				BattleReport.all.Where(a=>timeRange.Overlaps(a.arrival)).Cast<object>()		
				).ToArray();
			//if(ev.Length == 0) {
			//schedule.ShowBusyIndicator=false;
			//	return;
			//}
			
			List<SchedulerResource> resources1 = new();



			ev.SyncList(appointments, (e,s) => s.Id==e, (_o) => {
				object o = _o;
				if(o is CnVEvent e) { 
				var city = e.city;
				var p0 = city.player;
				var res = AddPlayer(p0);
				
				SchedulerResource res1 = null;
				if(e is CnVEventSendTroops st) {
					var p1 = st.troops.sourcePlayer;
					if(p1 != p0) {
						res1=AddPlayer(p1);
					}

				}
				var timeRange = e.sfTimeRange;
				var rv = new ScheduleAppointment() {
					Id=e,
					Subject=e.sfName,
					Foreground=e.sfForeground,
					AppointmentBackground=e.sfBackground,
					Location=e.city.FormatName(false,false,false),
					StartTime=timeRange.t0,
					EndTime=timeRange.t1,
					Notes=e.sfNotes
				};
				rv.ResourceIdCollection = new(new[] { res.Id });
				Assert(res1 != res);
				if(res1 != null ) { rv.ResourceIdCollection.Add(res1.Id); }
				return rv;
			}
			else if(o is BattleReport b ){
					var a = b.attackArmy;
					var rv = new ScheduleAppointment() {
					Id=b,
					Subject=a.uiName,
					Foreground=AppS.Brush(Colors.White) ,
					AppointmentBackground=a.uiBrush,
					Location=a.targetCity.FormatName(false,false,false),
					StartTime=a.arrival,
					EndTime=a.arrival+TimeSpanS.FromMinutes(30),
				};
					var p0 = b.targetPlayer;
					var p1 = b.sourcePlayer;
					var res0 = AddPlayer(p0);
					var res1 = AddPlayer(p1);
					rv.ResourceIdCollection = new(new[] { res0.Id,res1.Id });
					return rv;
			}
			else {
					Assert(false);
					return new();
				}
			}
			);;

			SchedulerResource AddPlayer(Player p1) {
				var res1 = resources1.FirstOrDefault(r => r.Id==p1);
				if(res1 == null) {
					res1 = resources.FirstOrDefault(r => r.Id==p1); // try to reuse
					if(res1 == null) {
						var cs = p1.pid.RandomUIColors(0.25f,0.875f);
						res1  = new() {
							Id = p1,
							Name= p1.shortName, // back ground by  player?
							Background = AppS.Brush(cs.c0),
							Foreground=AppS.Brush(cs.c1),
	

					};
					}
					Assert(!resources1.Any(a => a.Id == p1));

					resources1.Add(res1);
				}

				return res1;
			}
			if(!resources1.Any())
				AddPlayer(Player.active);
			resources1.SyncList(resources, (a,b)=>a.Id == b.Id );


//			schedule.ShowBusyIndicator=false;
		}

		internal DayOfWeek firstDayOfWeek => (schedule.DisplayDate + TimeSpan.FromDays(1)).DayOfWeek;

		internal static ImageSource PlayerImage(object id) {
			var p = id as Player;
			return p.avatarImage;
		}
		private void OnLoaded(object sender,RoutedEventArgs e) {
			// Add series
		//	UpdateData();
			//this.schedule.TimelineViewSettings.TimeInterval = new System.TimeSpan(0, 60, 0); // hourly ticks
			var t = (Sim.simTime - TimeSpanS.FromDays(6)).dateTimeDT;
			
			this.schedule.DisplayDate = t;
			this.schedule.AppointmentEditorOpening += Schedule_AppointmentEditorOpening;
			


		}
		private void Schedule_AppointmentEditorOpening(object sender, AppointmentEditorOpeningEventArgs e)
		{
			e.AppointmentEditorOptions = AppointmentEditorOptions.All | (~AppointmentEditorOptions.AllDay & ~AppointmentEditorOptions.Recurrence &~AppointmentEditorOptions.TimeZone);
		}

		private void scheduler_ReminderAlertOpening(object sender,ReminderAlertOpeningEventArgs e) {
			Note.Show(e.FormatJson());
		}

		ServerTime visibleT0;
		ServerTime visibleT1;

		private void QueryAppointments(object sender,QueryAppointmentsEventArgs e) {
	//		schedule.ShowBusyIndicator=true;
			visibleT0= e.VisibleDateRange.StartDate;
			visibleT1 = e.VisibleDateRange.EndDate+TimeSpanS.FromDays(1);
			AppS.QueueOnUIThread(UpdateDataI);
		}

		private void PlayerFilterChanged(object sender,PlayerListBox.ChangedEventArgs e) {
			playerFilter = e.players;
			AppS.QueueOnUIThread(UpdateDataI);
		}

		private void ItemTapped(object sender,AppointmentTappedArgs e) {
			var a = e.Appointment;
			var id = a?.Id;
			if(id is BattleReport br) {
				BattleReportDialog.ShowInstance(br);
			}
			else if(id is CnVEventSendTroops st) {
				SendTroops.ShowInstance(prior:st.troops);
			}
		}
	}
}
