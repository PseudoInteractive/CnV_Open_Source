using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Microsoft.UI;


namespace CnV;

	internal static partial class ClientView
	{
		internal static void UpdateLazy(this Microsoft.UI.Xaml.Controls.TextBlock txt, string _txt, Color? color=null)
		{
			if(_txt != txt.Text)
			{
				txt.Text = _txt;
			}
			if(color is not null)
			{ 
				var brush = AppS.Brush(color.Value);
				if(!object.ReferenceEquals(txt.Foreground, brush))
				{
					txt.Foreground = brush;
				}
			}
		}

		public static void UpdateStatsUI()
		{
			CityStats.instance?.UpdateUI();

		
		}

	}

