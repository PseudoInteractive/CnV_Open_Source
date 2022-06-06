using Microsoft.UI.Xaml;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CnV
{
	public sealed partial class RefineDialogue:DialogG, INotifyPropertyChanged
	{
		public static RefineDialogue? instance;

		protected override string title => "Refine";
		public RefineDialogue() {
			this.InitializeComponent();
			instance=this;
		}

		public RefineItem[] items { get; set; }

		City city;

		private void ResetItems() => items = new RefineItem[] { new(0,city),new(1,city),new(2,city),new(3,city) };
		public static async void ShowInstance() {
			if(!City.GetBuild().canRefine) {
				AppS.MessageBox("A level 10 Sorc tower is needed to refine.");
				return;
			}
			var rv = instance ?? new RefineDialogue();
			rv.city = City.GetBuild();
			rv.ResetItems();
			rv.city.PropertyChanged+=rv.City_PropertyChanged;
			
			rv.OnPropertyChanged();
			await rv.Show(false);
			rv.city.PropertyChanged -= rv.City_PropertyChanged;
		}

		private void City_PropertyChanged(object? sender,PropertyChangedEventArgs e) {
			DoPropertyChanged();
		}

		public static void ShowInstanceClick(object sender,RoutedEventArgs e) => ShowInstance();
		public void DoRefine(int id) {
			var res = new Resources();
			var city = this.city;
			if(!items[id].count.IsInRange(1,1_000_000)) {

				AppS.MessageBox("Value range is [1 .. 1m]");
				return;
			}
			res[id] = items[id].count;
			Assert(res.allPositive);
			var cost = res * -1000;
			if(!(city.SampleResources() + cost).allPositive) {
				AppS.MessageBox("Not enough res for refine");
			}
			else {

				new CnVEventRefine(city.c,res).EnqueueAsap();
				items[id].count = 0;
			}

			OnPropertyChanged();

		}

		private void WoodClick(object sender,RoutedEventArgs e) => DoRefine(0);
		private void StoneClick(object sender,RoutedEventArgs e) => DoRefine(1);
		private void IronClick(object sender,RoutedEventArgs e) => DoRefine(2);
		private void FoodClick(object sender,RoutedEventArgs e) => DoRefine(3);

		internal string existingRefines => Player.active.refinesAndGold.ToString();

		public event PropertyChangedEventHandler? PropertyChanged;
		public void OnPropertyChanged(string? member = null) {
			if(this.PropertyChanged is not null)
				AppS.QueueOnUIThread(DoPropertyChanged);
		}

		private void DoPropertyChanged() {
		
			PropertyChanged?.Invoke(this,new(null));
			foreach(var item in items) { item.OnPropertyChanged(); }
			
		}

		public static void Changed(string? member = null) {
			if(instance is not null)
				instance.OnPropertyChanged(member);
		}


	}
	public sealed class RefineItem:INotifyPropertyChanged
	{
		private int count1;

		public event PropertyChangedEventHandler? PropertyChanged;
		public void OnPropertyChanged(string? member = null) {
			PropertyChanged?.Invoke(this,new(member));
		}

		public RefineItem(int id, City city) {
			this.id = id;
			this.city = city;
			this.count1 = res/1000;
		}
		City city;
		public int id { get; set; }
		public int res => city.SampleResources()[id];
		public string resS => $"{(count*1000).Format()}/{res.Format()}";
		public int count {
			get => count1;
			set {
				if(count1 != value) {
					count1=value;
					OnPropertyChanged();
				}
			}
		}
	}

}
