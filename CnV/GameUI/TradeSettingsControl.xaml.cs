using Microsoft.UI.Xaml.Controls;

namespace CnV;
public sealed partial class TradeSettingsControl:UserControl, INotifyPropertyChanged {
	public TradeSettingsControl() {
		this.InitializeComponent();
	}

	internal async void WoodSource_PropertyChanged(object?sender,City city) {
		Assert(city!= null);

				var rv = await AppS.DoYesNoBox("Sources",$"Set source for all res to {city}?","Yes","Yes and set targets","No");
				if(rv == -1)
					return;

				stoneSource.SetCity(city,false);
				ironSource.SetCity( city,false);
				foodSource.SetCity( city,false);
				if(rv == 0) {
					woodDest.SetCity( city,false);
					stoneDest.SetCity( city,false);
					ironDest.SetCity( city,false );
					foodDest.SetCity( city,false );

				}
				OnPropertyChanged();
	}
	private async void WoodDest_PropertyChanged(object sender,City city) {
		Assert(city!= null);
		var rv = await AppS.DoYesNoBox("Destinations",$"Set destination for all res to {city}?","Yes","No",String.Empty);
				if(rv != 1)
					return;

				stoneDest.SetCity( city,false);
				ironDest.SetCity( city ,false);
				foodDest.SetCity( city ,false);

				OnPropertyChanged();
	}


	internal City city = City.invalid;

//	protected override string title => "Trade Settings";

	static double ToNumberBoxValue(int val) => val;

	public void InitializeFromCity()
	{

		var city = this.city;
		var mo = city.GetMOForRead();
		woodSource.SetCity(mo.requestCities[0].AsCity(),false);
		stoneSource.SetCity(mo.requestCities[1].AsCity(),false);
		ironSource.SetCity(mo.requestCities[2].AsCity(),false);
		foodSource.SetCity(mo.requestCities[3].AsCity(),false);

		woodReq.Value = ToNumberBoxValue(mo.resRequest[0]);
		stoneReq.Value = ToNumberBoxValue(mo.resRequest[1]);
		ironReq.Value = ToNumberBoxValue(mo.resRequest[2]);
		foodReq.Value = ToNumberBoxValue(mo.resRequest[3]);

		woodDest.SetCity(mo.sendCities[0].AsCity(),false);
		stoneDest.SetCity(mo.sendCities[1].AsCity(),false);
		ironDest.SetCity(mo.sendCities[2].AsCity(),false);
		foodDest.SetCity(mo.sendCities[3].AsCity(),false);

		woodSend.Value = ToNumberBoxValue(mo.resSend[0]);
		stoneSend.Value =ToNumberBoxValue( mo.resSend[1]);
		ironSend.Value = ToNumberBoxValue(mo.resSend[2]);
		foodSend.Value =ToNumberBoxValue( mo.resSend[3]);
		protectResources.IsChecked = mo.protectResources;
		cartReserve.Value = mo.cartReserve;
		shipReserve.Value = mo.shipReserve;

		woodOrder.Value = ToNumberBoxValue(city.requestedResources.wood);
		stoneOrder.Value = ToNumberBoxValue(city.requestedResources.stone);
		ironOrder.Value = ToNumberBoxValue(city.requestedResources.iron);
		foodOrder.Value = ToNumberBoxValue(city.requestedResources.food);
		maxTravel.Value = city.requestedResourceMaxTravel.TotalHours;
		payRate.Value = city.resourcePaymentRate;
		limitRequestsToAlliance.IsChecked = city.limitRequestsToAlliance;

		OnPropertyChanged();


	}
	
	public event PropertyChangedEventHandler? PropertyChanged;
	public void OnPropertyChanged(string? member = null)
	{
		if(this.PropertyChanged is not null)
			AppS.QueueOnUIThread(() => PropertyChanged?.Invoke(this,new(member)));
	}

	
	internal bool Apply()
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
		var reqHash0 = HashCode.Combine(city.requestedResources,city.requestedResourceMaxTravel,city.resourcePaymentRate,city.limitRequestsToAlliance);
		var _resourcePaymentRate=  payRate.FloatValue();
		var _requestsOnlyForAlliance = limitRequestsToAlliance.IsChecked.GetValueOrDefault();
		var _requestedResourceMaxTravel = TimeSpanS.FromHours(maxTravel.Value);
		var _requestedResources = new Resources(
			woodOrder.IntValue(),stoneOrder.IntValue(),ironOrder.IntValue(),foodOrder.IntValue()
			);
		var goldCost = CnVEventCityTradeRequest.GoldRequiredForRequest(city,_requestedResources,_resourcePaymentRate);
		if(goldCost > 0 && goldCost >= Player.active.gold) {
			AppS.MessageBox($"Not enough gold for request, need {goldCost - Player.active.gold} more gold");
			return false;
		}
		else {

			var reqHash1 = HashCode.Combine(_requestedResources,_requestedResourceMaxTravel,_resourcePaymentRate,_requestsOnlyForAlliance);
			if(reqHash0 != reqHash1) {
				new CnVEventCityTradeRequest(city.c,_requestedResources,_resourcePaymentRate,_requestedResourceMaxTravel,_requestsOnlyForAlliance).EnqueueAsap();
			}
			return true;
		}

	}

	
}