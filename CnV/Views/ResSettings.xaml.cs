using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using CnV.Game;
using CnV.Converters;
using Windows.Foundation;
using CnV.Helpers;

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

namespace CnV.Views
{
	using CnV.GameData;
	using Game;

	public sealed partial class ResSettings : UserControl, IANotifyPropertyChanged
	{

		#region PropertyChanged
		public void CallPropertyChanged(string member = null)
		{
			PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(member));
		}
		public void OnPropertyChanged(string member = null)
		{
			if(PropertyChanged is not null) ((IANotifyPropertyChanged)this).IOnPropertyChanged(member);
		}
		public event PropertyChangedEventHandler? PropertyChanged;
		#endregion

		public ResourcesNullable req;
		public ResourcesNullable max;
//		public bool applyRequested = true;
//		public bool applySend = true;
		public ResourceFilter reqFilter; 
		public ResourceFilter sendFilter;
		public int? cartReserve; // todo
		public int? shipReserve;

		public int? ReqHub => reqHub._city?.cid;
		public int? SendHub => sendHub._city?.cid;

		//public TradeSettings _TradeSettingsSel;
		//public static TradeSettings[] tradeSettingsItemsSource;
		//public TradeSettings tradeSettingsSel
		//{
		//	get => _TradeSettingsSel;
		//	set
		//	{
		//		if (_TradeSettingsSel != value)
		//		{
		//			_TradeSettingsSel = value;
		//			req.wood = value.requestWood;
		//			req.stone = value.requestStone;
		//			req.iron = value.requestIron;
		//			req.food = value.requestFood; ;
		//			//	Settings.sendWood = value.destWood != 0;
		//			//	Settings.sendStone = value.destStone != 0;
		//			//		Settings.sendIron = value.destIron != 0;
		//			//		Settings.sendFood = value.destFood != 0;
		//			max.wood = value.sendWood;
		//			max.stone = value.sendStone;
		//			max.iron = value.sendIron;
		//			max.food = value.sendFood;
		//			// TODO:  fix this
		//			//Bindings.Update();
		//			OnPropertyChanged();
		//		}
		//	}
		//}

		public void InitTradeSettings(City city, int _sourceHub, int _targetHub,ResourceFilter reqFilter,ResourceFilter targetFilter) 
		{
			var curSettings = city.GetMOForRead();
			this.reqFilter = reqFilter;
			this.sendFilter = targetFilter;
			reqHub.SetCity( _sourceHub == 0? null : City.Get(_sourceHub),false);
			sendHub.SetCity( _targetHub==0?null:City.Get(_targetHub), false);
			cartReserve = curSettings.cartReserve;
			shipReserve = curSettings.shipReserve;
			if (curSettings.resRequest.isNonZero)
			{
				this.req.wood = curSettings.resRequest.wood;
				this.req.stone = curSettings.resRequest.stone;
				this.req.iron = curSettings.resRequest.iron;
				this.req.food = curSettings.resRequest.food;
			}
			else if(req.isNull)
			{
				req.wood = (125000);
				req.stone = 125 * 1000;
				req.iron =  125 * 1000;
				req.food = 200 * 1000;
			}

			if (curSettings.resSend.isNonZero)
			{
				this.max.wood = curSettings.resSend.wood;
				this.max.stone = curSettings.resSend.stone;
				this.max.iron = curSettings.resSend.iron;
				this.max.food = curSettings.resSend.food;
			}
			else
			{
				if(max.isNull)
				{
					max.wood = (300 * 1000).Max(city.stats.storage.wood*3/4);
					max.stone = (300 * 1000).Max(city.stats.storage.stone* 3 / 4);
					max.iron = (300 * 1000).Max(city.stats.storage.iron * 3 / 4);
					max.food = (350 * 1000).Max(city.stats.storage.food * 3 / 4);
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
