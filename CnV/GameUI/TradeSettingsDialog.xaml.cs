using Microsoft.UI.Xaml.Controls;

namespace CnV;
public sealed partial class TradeSettingsDialog:DialogG, INotifyPropertyChanged {
	internal static TradeSettingsDialog instance;
	public TradeSettingsDialog() {
		this.InitializeComponent();
		instance = this;

	}

	internal async void WoodSource_PropertyChanged(object? sender,PropertyChangedEventArgs m) {

		if(m.PropertyName==nameof(woodSource.city)) {

			woodSource.PropertyChanged-=WoodSource_PropertyChanged;
			//if(woodSource.city != null)
			{
				var rv = await AppS.DoYesNoBox("Sources","Set hub for all res?","Yes","Yes and set targets","No");
				if(rv == -1)
					return;

				stoneSource.city = woodSource.city;
				ironSource.city = woodSource.city;
				foodSource.city = woodSource.city;
				if(rv == 0) {
					woodDest.city = woodSource.city;
					stoneDest.city = woodSource.city;
					ironDest.city = woodSource.city;
					foodDest.city = woodSource.city;

				}
				OnPropertyChanged();
			}
		}

	}

	internal City city => City.GetBuild();

	protected override string title => "Trade Settings";

	static double ToNumberBoxValue(int val) => val;

	public void UpdateItems()
	{

		var city = this.city;
		var mo = city.GetMOForRead();
		woodSource.city =mo.requestCities[0].AsCity();
		stoneSource.city =mo.requestCities[1].AsCity();
		ironSource.city =mo.requestCities[2].AsCity();
		foodSource.city =mo.requestCities[3].AsCity();

		woodReq.Value = ToNumberBoxValue(mo.resRequest[0]);
		stoneReq.Value = ToNumberBoxValue(mo.resRequest[1]);
		ironReq.Value = ToNumberBoxValue(mo.resRequest[2]);
		foodReq.Value = ToNumberBoxValue(mo.resRequest[3]);

		woodDest.city =mo.sendCities[0].AsCity();
		stoneDest.city =mo.sendCities[1].AsCity();
		ironDest.city =mo.sendCities[2].AsCity();
		foodDest.city =mo.sendCities[3].AsCity();

		woodSend.Value = ToNumberBoxValue(mo.resSend[0]);
		stoneSend.Value =ToNumberBoxValue( mo.resSend[1]);
		ironSend.Value = ToNumberBoxValue(mo.resSend[2]);
		foodSend.Value =ToNumberBoxValue( mo.resSend[3]);
		protectResources.IsChecked = mo.protectResources;
		cartReserve.Value = mo.cartReserve;
		shipReserve.Value = mo.shipReserve;

		woodRequest.Value = ToNumberBoxValue(city.requestedResources.wood);
		stoneRequest.Value = ToNumberBoxValue(city.requestedResources.stone);
		ironRequest.Value = ToNumberBoxValue(city.requestedResources.iron);
		foodRequest.Value = ToNumberBoxValue(city.requestedResources.food);
		maxTravel.Value = city.requestedResourceMaxTravel.TotalHours;
		payRate.Value = city.resourcePaymentRate;

		OnPropertyChanged();


	}
	public static void ShowInstance()
	{
		var rv = instance ?? new TradeSettingsDialog();
		rv.woodSource.PropertyChanged-=rv.WoodSource_PropertyChanged;
		if(!rv.Hide(false))
		{
			rv.UpdateItems();

			rv.Show(false);
			Task.Delay(2000).ContinueWith((_) => rv.woodSource.PropertyChanged+=rv.WoodSource_PropertyChanged);
		}

	}
	public event PropertyChangedEventHandler? PropertyChanged;
	public void OnPropertyChanged(string? member = null)
	{
		if(this.PropertyChanged is not null)
			AppS.QueueOnUIThread(() => PropertyChanged?.Invoke(this,new(member)));
	}

	
	private void DoneClick(object sender,Microsoft.UI.Xaml.RoutedEventArgs e)
	{
		var city = this.city;
		
		var mo = city.cloneMO; ;
		var priorHash = mo.tradeHashCode;
		mo.protectResources = protectResources.IsChecked.GetValueOrDefault();
		mo.cartReserve = cartReserve.Value.RoundToInt().Clamp(0,100).AsByte();
		mo.shipReserve = shipReserve.Value.RoundToInt().Clamp(0,100).AsByte();
		mo.sendCities=new(woodDest.cid,stoneDest.cid,ironDest.cid,foodDest.cid);
		mo.requestCities=new(woodSource.cid,stoneSource.cid,ironSource.cid,foodSource.cid);
		mo.resSend = new(
			woodSend.IntValue(),
			stoneSend.IntValue(),
			ironSend.IntValue(),
			foodSend.IntValue());
		mo.resRequest= new(
		woodReq.IntValue(),
		stoneReq.IntValue(),
		ironReq.IntValue(),
		foodReq.IntValue());
		mo.RemoveTradesTo(city.cid);
		var newHash = mo.tradeHashCode;
		Note.Show($"Trade settings changed: {newHash != priorHash}");
		if(newHash != priorHash)
		{
			new CnVEventCityTradeSettings(city.c,mo).EnqueueAsap();
		}
		var reqHash0 = HashCode.Combine(city.requestedResources,city.requestedResourceMaxTravel,city.resourcePaymentRate);
		var _resourcePaymentRate=  payRate.FloatValue();
		var _requestedResourceMaxTravel = TimeSpanS.FromHours(maxTravel.Value);
		var _requestedResources = new Resources(
			woodRequest.IntValue(),stoneRequest.IntValue(),ironRequest.IntValue(),foodRequest.IntValue()
			);
		var goldCost = CnVEventCityTradeRequest.GoldRequiredForRequest(city,_requestedResources,_resourcePaymentRate);
		if(goldCost > 0 && goldCost >= Player.active.gold) {
			AppS.MessageBox($"Not enough gold for request, need {goldCost - Player.active.gold} more gold");
		}
		else {

			var reqHash1 = HashCode.Combine(_requestedResources,_requestedResourceMaxTravel,_resourcePaymentRate);
			if(reqHash0 != reqHash1) {
				new CnVEventCityTradeRequest(city.c,_requestedResources,_resourcePaymentRate,_requestedResourceMaxTravel).EnqueueAsap();
			}
			Done();
		}

	}
}