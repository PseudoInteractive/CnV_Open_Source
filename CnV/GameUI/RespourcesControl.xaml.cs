using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CnV
{
	 public  sealed partial class ResourcesControl:UserControl
	{
		public ResourcesControl()
		{
			this.InitializeComponent();
			
		}

		public bool allowNegative { get; set; } = false;

		public static readonly DependencyProperty RssProperty = DependencyProperty.Register(
		"Rss",
		typeof(Resources),
		typeof(ResourcesControl),
		new PropertyMetadata(default));

		public Resources Rss
		{
			get { return (Resources)GetValue(RssProperty); }
			set { SetValue(RssProperty,value); }
		}

		public static readonly DependencyProperty DescProperty = DependencyProperty.Register(
		"Desc",
		typeof(string),
		typeof(ResourcesControl),
		new PropertyMetadata(default));

		public string Desc
		{
			get { return (string)GetValue(DescProperty); }
			set { SetValue(DescProperty,value); }
		}

		public string _Desc(int id) => $"{CnV.Resources.ResGlyph(id)} {Desc}";

		// Only called on UI change
		private void Set(int resIndex, int v)
		{
			if(!allowNegative) {
				if(v < 0)
					return;
			}
			var rss = Rss;
			if(rss[resIndex]==v)
				return;
			rss[resIndex] = v;
			Rss = rss;
			changed?.Invoke(this, EventArgs.Empty );
			
		}

		public int RssWood {
			get => Rss.wood;
			set => Set(0,value);
		}
		
		public int RssStone
		{
			get => Rss.stone; 
			set => Set(1,value);
		}

		public int RssIron
		{
			get => Rss.iron; 
			set => Set(2,value);
		}
		public int RssFood
		{
			get => Rss.food; 
			set => Set(3,value);
		}

		public event EventHandler changed;


	}
}
