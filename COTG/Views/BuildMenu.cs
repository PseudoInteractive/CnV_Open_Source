using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Media;
using static COTG.Views.CityBuild;
using Microsoft.UI.Xaml;
using Action = COTG.Views.CityBuild.Action;
namespace COTG.Views
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
		public int bid;
		public bool isAction => action != Action.invalid;
		public bool isBuilding => bid != 0;
		public ImageBrush brush;
		public string toolTip;
		public string header;
		public Windows.UI.Color textColor;
		public CityBuild.Action action = Action.invalid;
		public string accessKey { get; set; }
		public const int width = 64;
		public const int height = 64;

		public BuildMenuItem()
		{
			action = Action.invalid;
			bid = -1;
			textColor = Windows.UI.Colors.Black;

		}
		public BuildMenuItem(int _bid)
		{
			action = Action.invalid;
			{
				bid = _bid;
				var def = BuildingDef.all[_bid];
				header = def.Bn;
				toolTip = def.Ds;
				brush = BuildingBrush(bid, width / 128.0f);
				//	Command = BuildMenuItemCommand.instance;
				var match = shortKeyRegEx.Match(toolTip);
				if (match.Success && match.Groups.Count == 2)
				{
					accessKey = match.Groups[1].Value;
				}
			}
		}
		public BuildMenuItem(string name, Action action, string icon, string toolTip)
		{
			header = name;
			this.action = action;
			brush = CityBuild.BrushFromImage(icon);
			this.toolTip = toolTip;
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
	//		name = BuildingDef.all[_id].Bn;
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

		public int bid
		{
			get
			{
				return (int)this.GetValue(bidProperty);
			}
			set
			{
				this.SetValue(bidProperty, value);
				Background = BuildingBrush(bid, (float)Width / 128.0f);
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

		public int bid
		{
			get
			{
				return (int)this.GetValue(bidProperty);
			}
			set
			{
				this.SetValue(bidProperty, value);
				Background = BuildingBrush(bid, (float)Width / 128.0f);
			}
		}
		public string image
		{
			set
			{
				Background = BrushFromImage(value);
			}
		}
		public BuildingRect()
		{
			Width = 64;
			Height = 64;
		}
	}
}
