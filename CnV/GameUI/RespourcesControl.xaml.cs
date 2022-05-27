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

		public int RssWood
		{
			get => Rss.wood; 
			set => Rss = Rss with { wood = value };
		}
		public int RssStone
		{
			get => Rss.stone; 
			set => Rss = Rss with { stone = value };
		}

		public int RssIron
		{
			get => Rss.iron; 
			set => Rss = Rss with { iron = value };
		}
		public int RssFood
		{
			get => Rss.food; 
			set => Rss = Rss with { food = value };
		}


	}
}
