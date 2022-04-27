using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Media;
using static CnV.CityBuild;
using Microsoft.UI.Xaml;
using Microsoft.UI;

namespace CnV.Views
{
	public enum BuildPhase
	{
		cabins,
		buildings,
		teardown,
	}

	public class BuildMenuItemGroup
	{
		public string title { get; set; }
		public List<BuildMenuItem> items { get; set; } = new();
	}
	

	public class BuildMenuItem
	{
		public BuildingId bid;
		public bool isAction => action != CityBuild.CityBuildAction.invalid;
		public bool isBuilding => bid != 0;
		public BitmapImage image;
		public float opacity=1;
		public string tip;
		internal string tooltip {
			get 
				{
				try {

					var city = City.GetBuild();
					if(isBuilding) {
						var def = BuildingDef.FromId(bid);
						var op = new BuildQueueItem(0,1,bid,CityView.selectedPoint);
						return $"{def.Ds}\n{-op.BuildOrUpgradeResourceBalance()}\n{op.TimeRequired(city)}\nKeep Level: {def.Thl}";
					}
					if(CityView.selectedPoint.isInCity) {

						var bd = city.postQueueBuildings[CityView.selectedPoint];
						var def = BuildingDef.FromId(bd.bid);
						if(bd.bid != 0) {
							switch(action) {

								case CityBuildAction.destroy:
									if(bd.bl > 0) {

										var op = new BuildQueueItem(bd.bl,0,bd.bid,CityView.selectedPoint);
										return $"Destroy {def.Bn}\n{op.DemoOrDowngradeResourceBalance()}\n{op.TimeRequired(city)}";
									}
									break;
								case CityBuildAction.build:
									Assert(false);
									break;

								case CityBuildAction.upgrade:
									if(bd.bl < 10 && !bd.isRes) {
										var op = new BuildQueueItem(bd.bl,(byte)(bd.bl+1).Min(10),bd.bid,CityView.selectedPoint);
										return $"Upgrade {def.Bn}\n{-op.BuildOrUpgradeResourceBalance()}\n{op.TimeRequired(city)}";
									}
									break;
								case CityBuildAction.downgrade:
									if(bd.bl > 0 && !bd.isRes) {
										var op = new BuildQueueItem(bd.bl,(byte)(bd.bl-1).Max(0),bd.bid,CityView.selectedPoint);
										return $"Downgrade {def.Bn}\n{op.DemoOrDowngradeResourceBalance()}\n{op.TimeRequired(city)}";
									}
									break;
							}
						}
					}
				
				}
				catch(Exception ex) { LogEx(ex); }
				return tip;
			}
		}
		public string header;
		//public Color textColor;
		public CityBuild.CityBuildAction action = CityBuild.CityBuildAction.invalid;
		public string accessKey { get; set; }
		public const int width = 64;
		public const int height = 64;

		public BuildMenuItem()
		{
			action = CityBuild.CityBuildAction.invalid;
			bid = 0;
		//	textColor = Colors.Black;

		}
		public BuildMenuItem(BuildingId _bid)
		{
			action = CityBuild.CityBuildAction.invalid;
			{
				bid = _bid;
				var def = BuildingDef.FromId(_bid);
				header = def.Bn;
				var toolTip = def.Ds;
				image = GetBuildingImage( bid , width);
				//	Command = BuildMenuItemCommand.instance;
				var match = shortKeyRegEx.Match(toolTip);
				if (match.Success && match.Groups.Count == 2)
				{
					accessKey = match.Groups[1].Value;
				}
			}
		}
		public BuildMenuItem(string name, CityBuild.CityBuildAction action, string icon, string toolTip)
		{
			header = name;
			this.action = action;
			image = ImageHelper.Get(icon,width);
			this.tip = toolTip;
		}

		internal void UpdateValidity(bool seemsValid, int townHallLevel)
		{
			if(!seemsValid)
				opacity = 0.25f;
			else if( BuildingDef.FromId(bid).Thl > townHallLevel)
				opacity = 0.5f;
			else
				opacity = 1.0f;

		}
	}

	//// nested types not supported
	//public class QuickBuildItem
	//{
	//	public int bid;
	//	public string name { get; set; }
	//	public ImageBrush brush { get; set; }
	//	public QuickBuildItem(int _id)
	//	{
	//		bid = _id;
	//		name = BuildingDef.FromId(_id).Bn;
	//		brush = CityBuild.BuildingBrush(_id, 0.5f);
	//	}
	//	public QuickBuildItem(CityBuild.Action _id, string _name, string image)
	//	{
	//		bid = (int)_id;
	//		name = _name;
	//		brush = CityBuild.BrushFromImage(image);

	//	}
	//}
	public class BuildingButton : Microsoft.UI.Xaml.Controls.Button
	{
		/// <summary>
		/// Identifies the <see cref="Image id"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty bidProperty =
			DependencyProperty.Register("bid", typeof(int), typeof(BuildingButton), new PropertyMetadata(null));

		public BuildingId bid
		{
			get
			{
				return (BuildingId)this.GetValue(bidProperty);
			}
			set
			{
				this.SetValue(bidProperty, value);
				Background = BuildingBrush(value, Width.RoundToInt() );
			}
		}

		public BuildingButton()
		{
			Width = 64;
			Height = 64;
		}
	}
	public class BuildingRect : Microsoft.UI.Xaml.Controls.Canvas
	{
		/// <summary>
		/// Identifies the <see cref="Image bid"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty bidProperty =
			DependencyProperty.Register("bid", typeof(int), typeof(BuildingButton), new PropertyMetadata(null));

		public BuildingId bid
		{
			get
			{
				return (BuildingId)this.GetValue(bidProperty);
			}
			set
			{
				this.SetValue(bidProperty, value);
				Background = BuildingBrush(value, Width.RoundToInt());
			}
		}
		//public string image
		//{
		//	set
		//	{
		//		Background = BrushFromImage(value);
		//	}
//		}
		public BuildingRect()
		{
			Width = 64;
			Height = 64;
		}
	}
}
