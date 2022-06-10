using Microsoft.UI.Xaml.Controls;

namespace CnV;
public sealed partial class TradeSettingsDialog:DialogG, INotifyPropertyChanged {
	internal static TradeSettingsDialog instance;
	public TradeSettingsDialog() {
		this.InitializeComponent();
		instance = this;

	}


	protected override string title => "Trade Settings";


	public static async void ShowInstance()
	{
		var rv = instance ?? new TradeSettingsDialog();
		// once in case of exception
		rv.settings.city = City.GetBuild();
		rv.settings.InitializeFromCity();
		rv.settings.OnPropertyChanged();
		await rv.Show(true);
	}
	public event PropertyChangedEventHandler? PropertyChanged;
	public void OnPropertyChanged(string? member = null)
	{
		if(this.PropertyChanged is not null)
			AppS.QueueOnUIThread(() => PropertyChanged?.Invoke(this,new(member)));
	}

	
	private void DoneClick(object sender,Microsoft.UI.Xaml.RoutedEventArgs e)
	{
		if( settings.Apply() ) { 
			Done();
		}

	}
}