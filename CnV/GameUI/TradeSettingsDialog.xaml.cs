namespace CnV;
public sealed partial class TradeSettingsDialog:DialogG, INotifyPropertyChanged
{
	internal static TradeSettingsDialog instance;
	public TradeSettingsDialog()
	{
		this.InitializeComponent();
		instance = this;

	}

	internal async void WoodSource_PropertyChanged(object? sender,PropertyChangedEventArgs m)
	{

		if(m.PropertyName==nameof(woodSource.city))
		{
			//if(woodSource.city != null)
			{
				var rv = await AppS.DoYesNoBox("Sources","Set hub for all res?","Yes","Yes and set targets","No");
				if(rv == -1)
					return;

				stoneSource.city = woodSource.city;
				ironSource.city = woodSource.city;
				foodSource.city = woodSource.city;
				if(rv == 0)
				{
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

	public void UpdateItems()
	{

		var city = this.city;
		var mo = city.GetMOForWrite();
		woodSource.city =mo.requestCities[0].AsCity();
		stoneSource.city =mo.requestCities[1].AsCity();
		ironSource.city =mo.requestCities[2].AsCity();
		foodSource.city =mo.requestCities[3].AsCity();

		woodReq.Value = mo.resRequest[0];
		stoneReq.Value = mo.resRequest[1];
		ironReq.Value = mo.resRequest[2];
		foodReq.Value = mo.resRequest[3];

		woodDest.city =mo.sendCities[0].AsCity();
		stoneDest.city =mo.sendCities[1].AsCity();
		ironDest.city =mo.sendCities[2].AsCity();
		foodDest.city =mo.sendCities[3].AsCity();

		woodSend.Value = mo.resSend[0];
		stoneSend.Value = mo.resSend[1];
		ironSend.Value = mo.resSend[2];
		foodSend.Value = mo.resSend[3];
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
		var priorHash = city.GetMOForRead().tradeHashCode;
		var mo = new CityMO();
		mo.sendCities=new(woodDest.cid,stoneDest.cid,ironDest.cid,foodDest.cid);
		mo.requestCities=new(woodSource.cid,stoneSource.cid,ironSource.cid,foodSource.cid);
		mo.resSend = new(
			woodSend.Value.RoundToInt(),
			stoneSend.Value.RoundToInt(),
			ironSend.Value.RoundToInt(),
			foodSend.Value.RoundToInt());
		mo.resRequest= new(
		woodReq.Value.RoundToInt(),
		stoneReq.Value.RoundToInt(),
		ironReq.Value.RoundToInt(),
		foodReq.Value.RoundToInt());
		mo.RemoveTradesTo(city.cid);
		var newHash = mo.tradeHashCode;
		Note.Show($"Trade settings changed: {newHash != priorHash}");
		if(newHash != priorHash)
		{
			(new CnVEventCityTradeSettings(city.c)
			{
				resRequest = mo.resRequest,
				requestCities = mo.requestCities,
				resSend = mo.resSend,
				sendCities = mo.sendCities,
				cartReserve = mo.cartReserve,
				shipReserve=mo.shipReserve
			}).EnqueueAsap();
		}

		Done();
	}
}