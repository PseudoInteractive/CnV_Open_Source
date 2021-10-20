using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using COTG.Game;
using COTG.Converters;
using Windows.Foundation;
using COTG.Helpers;
using COTG.JSON;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System.Threading.Tasks;
using System.ComponentModel;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace COTG.Views
{
	public sealed partial class ResSettings : UserControl, IANotifyPropertyChanged
	{

		#region PropertyChanged
		public void CallPropertyChanged(string member = null)
		{
			PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(member));
		}
		public void OnPropertyChanged(string member = null)
		{
			if(PropertyChanged is not null) ((IANotifyPropertyChanged)this).IOnPropertyChanged();
		}
		public event PropertyChangedEventHandler PropertyChanged;
		#endregion

		public ResourcesNullable req;
		public ResourcesNullable max;
//		public bool applyRequested = true;
//		public bool applySend = true;
		public ResourceFilter reqFilter; 
		public ResourceFilter sendFilter;
		public int? cartReserve; // todo
		public int? shipReserve;

		public int? ReqHub => reqHub.city?.cid;
		public int? SendHub => sendHub.city?.cid;

		public TradeSettings _TradeSettingsSel;
		public static TradeSettings[] tradeSettingsItemsSource;
		public TradeSettings tradeSettingsSel
		{
			get => _TradeSettingsSel;
			set
			{
				if (_TradeSettingsSel != value)
				{
					_TradeSettingsSel = value;
					req.wood = value.requestWood;
					req.stone = value.requestStone;
					req.iron = value.requestIron;
					req.food = value.requestFood; ;
					//	SettingsPage.sendWood = value.destWood != 0;
					//	SettingsPage.sendStone = value.destStone != 0;
					//		SettingsPage.sendIron = value.destIron != 0;
					//		SettingsPage.sendFood = value.destFood != 0;
					max.wood = value.sendWood;
					max.stone = value.sendStone;
					max.iron = value.sendIron;
					max.food = value.sendFood;
					// TODO:  fix this
					//Bindings.Update();
					OnPropertyChanged();
				}
			}
		}

		public async Task InitTradeSettings(City city, int _sourceHub, int _targetHub,ResourceFilter reqFilter,ResourceFilter targetFilter) 
		{
			var curSettings = await CitySettings.GetTradeResourcesSettings(city.cid);
			this.reqFilter = reqFilter;
			this.sendFilter = targetFilter;
			reqHub.city = _sourceHub == 0? null : City.Get(_sourceHub);
			sendHub.city = _targetHub==0?null:City.Get(_targetHub);
			cartReserve = curSettings.cartReserve;
			shipReserve = curSettings.shipReserve;
			if (curSettings.req.isNonZero)
			{
				this.req.wood = curSettings.req.wood;
				this.req.stone = curSettings.req.stone;
				this.req.iron = curSettings.req.iron;
				this.req.food = curSettings.req.food;
			}
			else if(req.isNull)
			{
				req.wood = (125000);
				req.stone = 125 * 1000;
				req.iron =  125 * 1000;
				req.food = 200 * 1000;
			}

			if (curSettings.max.isNonZero)
			{
				this.max.wood = curSettings.max.wood;
				this.max.stone = curSettings.max.stone;
				this.max.iron = curSettings.max.iron;
				this.max.food = curSettings.max.food;
			}
			else
			{
				if(max.isNull)
				{
					max.wood = (300 * 1000).Max(curSettings.storage.wood*3/4);
					max.stone = (300 * 1000).Max(curSettings.storage.stone* 3 / 4);
					max.iron = (300 * 1000).Max(curSettings.storage.iron * 3 / 4);
					max.food = (350 * 1000).Max(curSettings.storage.food * 3 / 4);
				}
			}

			OnPropertyChanged();
			reqHub.OnPropertyChanged();
			sendHub.OnPropertyChanged();
		}
		public ResSettings()
		{
			this.InitializeComponent();
		}
	}
}
