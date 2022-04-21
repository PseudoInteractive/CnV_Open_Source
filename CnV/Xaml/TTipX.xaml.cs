using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CnV
{
	public sealed partial class TTipX:TeachingTip
	{

		public static Dictionary<string,TTipX> all = new();
		public TTipX()
		{
			this.InitializeComponent();
		}

		internal TTip tip;






//		public static readonly DependencyProperty IdProperty = DependencyProperty.Register(
//  "Id",
//  typeof(double),
//  typeof(TTipX),
//  new PropertyMetadata(0)
//);
//		public double Id
//		{
//			get { return (double)GetValue(IdProperty); }
//			set { SetValue(IdProperty,value); }
//		}

		
		private void TTLoaded(object sender,RoutedEventArgs e)
		{
			
			if(all.ContainsKey(Name))
			{
				Assert(all[Name] == this);
			}
			else
			{
				all.Add(Name,this);
			}
		}
	}
}
