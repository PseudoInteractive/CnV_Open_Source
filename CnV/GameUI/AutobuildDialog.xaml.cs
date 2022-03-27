using Microsoft.UI.Xaml.Media;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CnV;

using static Building;

public sealed partial class AutobuildDialog:DialogG,INotifyPropertyChanged
{
	internal static AutobuildDialog instance;
	public AutobuildDialog()
	{
		this.InitializeComponent();
		instance = this;
	}
	internal NotifyList<AutobuildItem> items = new();
	internal City city => City.GetBuild();

	protected override string title => "Autobuild Settings";

	public void UpdateItems()
	{
		var city = this.city ;
		var mo =  city.GetMOForWrite();
		autobuildOn.IsChecked = mo.autobuildOn;
		items.Clear();
		for(var bid = bidAutobuildStart;bid<=bidAutobuildLast;++bid)
		{
			if(IsBidRes(bid))
			{
				continue;
			}
			var bdef = BuildingDef.FromId(bid);
			if(bdef.dimg is null)
				continue;
			var setting = mo.autoBuildLevels[bid];
			var item = new AutobuildItem() { bid = bid, on = setting > 0,level = setting.Abs() };
			items.Add(item);
		}
		items.OnReset();
	}
	public static void ShowInstance()
		{
			var rv = instance ?? new AutobuildDialog();
			rv.UpdateItems();
		
			rv.OnPropertyChanged();
			rv.Show(false);
			
		}
		public event PropertyChangedEventHandler? PropertyChanged;
		public void OnPropertyChanged(string? member = null)
		{
			if (this.PropertyChanged is not null) 
				AppS.QueueOnUIThread(() => PropertyChanged?.Invoke(this,new(member)));
		}

	private void DoneClick(object sender,Microsoft.UI.Xaml.RoutedEventArgs e)
	{
		var city = this.city ;
		var mo =  city.GetMOForWrite();
		mo.autobuildOn = autobuildOn.IsChecked.GetValueOrDefault();
		foreach(var i in items)
		{
			mo.autoBuildLevels[i.bid] = (sbyte)(i.on ? i.level : -i.level);
		}
		(new CnVEventCityAutobuild(city.c) { autobuildOn = mo.autobuildOn,autoBuildLevels=mo.autoBuildLevels }).EnqueueAsap();
		Done();
	}
}
internal class AutobuildItem
{
	internal BuildingId bid;
	internal bool on = true;
	internal int level = 10;
	internal BuildingDef def => BuildingDef.FromId(bid);
	internal const int imageSize = 32;
	internal ImageSource image => CityBuild.GetBuildingImage(bid,imageSize);
	internal string name => def.Bn;

}
