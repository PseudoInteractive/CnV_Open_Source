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


namespace CnV
{
	public sealed partial class BossReportDialog:DialogG,INotifyPropertyChanged
	{
		static BossReportDialog instance;
		protected override string title => report?.ToString();
		BossReport report;

		internal string attackRatio => $"Kill Ratio: {report.attackRatio:P}";
		internal string loot => report.loot.Format("\n");
		internal Brush slainBrush => AppS.Brush(report.slain ? Microsoft.UI.Colors.DarkGreen : Microsoft.UI.Colors.Maroon);

		public ObservableCollection<Artifact> artifacts = new();
		public BossReportDialog() 
		{
			this.InitializeComponent();
			instance =this;
			
			//if(target is not null)
			//	Target=target;
		}
		public static void ShowInstance(BossReport report)
		{
			AppS.QueueOnUIThread(async () => {
				var rv = instance ?? new BossReportDialog();
				rv.artifacts.Clear();
				rv.report = report;
				//	rv.HeroContent.Focus(FocusState.Programmatic);
				//	rv.count.Value = artifact.owned. Max(1);
				rv.OnPropertyChanged();
				rv.Show(false);

				foreach(var a in report.artifacts) {
					await Task.Delay(500);
					var art = Artifact.Get(a);
					if(art is not null) {
						rv.artifacts.Add(art);
					}
				}
			});
			
		}

		public ImageSource background => report?.icon;

		

		public event PropertyChangedEventHandler? PropertyChanged;
		public void OnPropertyChanged(string? member = null)
		{
			if (this.PropertyChanged is not null) 
				AppS.QueueOnUIThread(() => PropertyChanged?.Invoke(this,new(member)));
		}

		private async void ShowClick(object sender,RoutedEventArgs e) {
		
			await UserTab.ShowOrAdd<NPCHistory>();

			NPCHistory.instance.grid.SetFocus(report);
			Hide(true);
		}
	}
}
