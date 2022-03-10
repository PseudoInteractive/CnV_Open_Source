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
		public string toolTip;
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
				toolTip = def.Ds;
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
			this.toolTip = toolTip;
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
