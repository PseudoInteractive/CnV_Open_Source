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
	public sealed partial class SendResDialogue:DialogG,INotifyPropertyChanged
	{
		internal static SendResDialogue? instance;
		protected override string title => $"Send from {source} to {destination}"; 
		internal Resources res;

		internal City source;
		internal City destination;

		public static Task<bool> ShowInstance(City city,City target, Resources? _res, bool? _viaWater, bool? palaceDonation)
		{
			if(city == target) {
				Note.Show("Cannot send to self");
				return Task.FromResult(false);
			}
			var rv = instance ?? new SendResDialogue();
			rv.source = city;
			rv.destination = target;
			if(palaceDonation.HasValue) {
				rv.palaceDonation.IsChecked = palaceDonation.Value;
			}
			if(_viaWater.HasValue)
				rv.transport.SelectedIndex = _viaWater.Value.Switch(0,1);
			if(_res is not null) {
				rv.res = _res.Value;
				rv.OnPropertyChanged();
			}
			else
				rv.SetDefault();
			
			return rv.Show(false);

		}
		internal bool isPalaceDonation => palaceDonation.IsChecked.GetValueOrDefault();

		private void SetDefault()
		{
			res = source.SampleResources();
			if(isPalaceDonation) {
				res.iron=res.food=0;
			}
			res.LimitToTransportInPlace(bandWidth);
			OnPropertyChanged();
		}

		internal void SetMax(int id)
		{
			var extra = bandWidth - res.sum;
			var extra2 = (source.SampleResources()[id] - res[id]).Min(extra);
			res[id] += extra2;
			OnPropertyChanged();
		}


		public SendResDialogue()
		{
			this.InitializeComponent();
			instance = this;
		}
		internal string FreeRes(int i) => source.SampleResources()[i].Format();

		bool viaWater => transport.SelectedIndex == 1;
		bool viaLand => transport.SelectedIndex == 0;
		internal int bandWidth =>  viaLand ? (int)source.cartsHome*1000 : (int)source.shipsHome*10_000;
		internal RoutedEventHandler SetMaxDel(int id) => (_,_) => SetMax(id);


		public event PropertyChangedEventHandler? PropertyChanged;
		public void OnPropertyChanged(string? member = null)
		{
			if (this.PropertyChanged is not null) 
				AppS.QueueOnUIThread(() => PropertyChanged?.Invoke(this,new(member)));
		}
		public static void Changed(string? member = null)
		{
			if(instance is not null)
				instance.OnPropertyChanged(member);
		}

		internal string shipsStr => $"Ships {source.shipsHome}/{source.ships}, travel time: {source.player.TravelTime(source.c.DistanceToD(destination.cid),true).Format()}";
		internal string cartsStr => $"Carts {source.cartsHome}/{source.carts}, travel time: {source.player.TravelTime(source.c.DistanceToD(destination.cid),false).Format()}";
	
		private void SetMax0(object sender,RoutedEventArgs e) => SetMax(0);
		private void SetMax1(object sender,RoutedEventArgs e) => SetMax(1);
		private void SetMax2(object sender,RoutedEventArgs e) => SetMax(2);
		private void SetMax3(object sender,RoutedEventArgs e) => SetMax(3);
		internal string cartRequirments { get {
				var sb = new PooledStringBuilder();
				sb.s.Append("Will use: ");
				TradeOrder.FormatTransport(sb.s,viaLand ? res.sum.DivideRoundUp(1_000) : 0,!viaLand ? res.sum.DivideRoundUp(10_000) : 0);
				return sb.s.ToString();
				}
			}

		private void SendClick(object sender,RoutedEventArgs e) {
			if(source.underSiege) {
				AppS.MessageBox("City is under siege");
				return;
			}
			if(source == destination) {
				AppS.MessageBox("Cannot send to self");
				return;
		
			}
			if(isPalaceDonation&& (!destination.isBlessed)) {
				AppS.MessageBox("Palace donations can only be sent to blessed cities");
				return;
			}
			if(isPalaceDonation&& (res.iron + res.food) > 0) {
				AppS.MessageBox("Food and iron cannot be sent as a palace donation");
				return;
			}

			if(res.sum == 0) {
				AppS.MessageBox("That was easy.");
				return;

			}
			var viaWater = this.viaWater; // fetch before lock

			if(viaWater && !destination.isOnWater) {
				AppS.MessageBox("Destination must be on water to send via ships.");
				return;
			}
			if(!viaWater && (destination.c.continentId != source.c.continentId)  ) {
				AppS.MessageBox("Carts cannot travel cross continent.");
				return;
			}
			if( (destination.c.continentId != source.c.continentId)  ) {
				AppS.MessageBox("Carts cannot travel cross continent.");
				return;
			}
			if( bandWidth < res.sum) {
				AppS.MessageBox("Not enough ships or carts to send this much");
				return;
			}
			if( !(source.sampleResources - res).allPositive ) {
				AppS.MessageBox("Insufficient resources");
				return;

			}


			Done();
			
//			using(var locker = Sim.eventQLock.Enter)
			{
				
				var trade = new TradeOrder(source: source.c,target: destination.c,departure: Sim.simTime,viaWater: viaWater,isTempleTrade: isPalaceDonation,resources: res);

				new CnVEventTrade(source.c,trade: trade).EnqueueAsap();
			}

			Note.Show($"Sent {(isPalaceDonation? "palace donaton " : "")} {res.Format()} from {source} to {destination}");
			
			
		}

		private void transport_SelectionChange(object sender,SelectionChangedEventArgs e)
		{
		//	if(source is not null)
		//		SetDefault();
		}

		private void ClearClick(object sender,RoutedEventArgs e)
		{
			res = default;
			Changed();
		}

		private void SetMax(object sender,RoutedEventArgs e) {
			SetDefault();
			Changed();
		}

		private void ResNumberChanged(NumberBox sender,NumberBoxValueChangedEventArgs args) {
			FilterPositive(sender,args);
			Changed();
		}
	}
}
