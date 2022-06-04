using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CnV
{
	public sealed partial class DailyDialog:DialogG,INotifyPropertyChanged
	{
		static DailyDialog instance;
		protected override string title => "Fortune Teller";
		public ObservableCollection<Artifact> artifacts = new();
		public DailyDialog() 
		{
			this.InitializeComponent();
			instance =this;
			
			//if(target is not null)
			//	Target=target;
		}
		public static async Task ShowInstance(params Artifact[] artifact)
		{
			var rv = instance ?? new DailyDialog();
			Assert(rv.artifacts.IsNullOrEmpty());
			rv.artifacts.Clear();
		//	rv.HeroContent.Focus(FocusState.Programmatic);
		//	rv.count.Value = artifact.owned. Max(1);
			rv.OnPropertyChanged();
			var t = rv.Show(false);

			foreach(var a in artifact)
			{
				if(a is not null)
				{
					var _a = a;
					await Task.Delay(500);
					rv.artifacts.Add(_a);
				}
			}
			await t;
			// claim last ones
			while( rv.artifacts.Any() ) {
				var art = rv.artifacts.First();
				rv.artifacts.RemoveAt(0);
				ClaimArtifact(art);
				AppS.MessageBox($"Auto Claimed: {art.name}",hero: art.imageUrl);
				await Task.Delay(1000);
			}

			
		}

		public ImageSource background => ImageHelper.Get("UI/menues/fortune_teller/background_welcome.jpg");

		

		public event PropertyChangedEventHandler? PropertyChanged;
		public void OnPropertyChanged(string? member = null)
		{
			if (this.PropertyChanged is not null) 
				AppS.QueueOnUIThread(() => PropertyChanged?.Invoke(this,new(member)));
		}

		private void ItemClick(object sender,ItemClickEventArgs e) {
			if(Settings.wantUISounds) {
				ElementSoundPlayer.Play(ElementSoundKind.Invoke);
			}

			var art = e.ClickedItem as Artifact;
			artifacts.Remove(art);
			
			ClaimArtifact(art);
			if(artifacts.Count == 0) {
				Hide(true);
				AppS.QueueIdleTask(DailyRewardTask,5000);
			}

		}

		private static void ClaimArtifact(Artifact art) {
			try {
				SocketClient.DeferSendStart();
				new CnVEventPurchaseArtifacts((ushort)art.id,(ushort)1,Player.active.id,free: true).EnqueueAsap();

				// use Zirconia immediately
				if(art.type == Artifact.ArtifactType.zirconia || art.type == Artifact.ArtifactType.karma) {
					(new CnVEventUseArtifacts(City.build) { artifactId = (ushort)art.id,count = 1,aux=0 }).EnqueueAsap();
				}

			}
			catch(Exception ex) {
				LogEx(ex);
			}
			finally {
				SocketClient.DeferSendEnd();
			}
		}

		internal static void DailyRewardTask()
		{
			var nextClaim = Player.nextDailyClaim;
			// wait for next
			{
				var dt = nextClaim - Sim.simTime;
				if(dt > 0) {
					AppS.QueueIdleTask(DailyRewardTask,((dt+60)*1000 /IServerTime.timeScale).RoundToInt());
					return;

				}
			}
			{
				var interval = TimeSpanS.FromHours(12);
				// Hack:
				Player.nextDailyClaim = new(Sim.simTime.seconds.TruncateToMultipleOf((uint)interval.seconds) + (uint)interval.seconds);
			}
			AppS.QueueOnUIThread( async () =>
			{
				var rnd = new XXRand(Sim.simTime.seconds);
				await ShowInstance(Artifact.GetArtifactDrop(-1,ref rnd),Artifact.GetArtifactDrop(-1,ref rnd),Artifact.GetForPlayerRank(Artifact.ArtifactType.zirconia));
				DailyRewardTask();

			}
			);
		}
	}
}
