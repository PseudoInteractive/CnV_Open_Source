using System.Threading.Tasks;
//using Windows.UI.Core;
//using Windows.UI.Core.Preview;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;

namespace CnV.Views
{
	public partial class ShellPage
	{
		
		internal class UpdateHtmlOffsets : Debounce
		{
			long lastUpdateTick;// = 1L<<62; // big but allows room without overflow
			
			public UpdateHtmlOffsets() : base(null)
			{
				runOnUiThread = true;
				debounceDelay = 100;
				throttleDelay = 200;
				base.func = F;
			}

			public void Go(bool _updateLayout)
			{
				if (!_updateLayout && lastUpdateTick == 0) // not initialized
				{
					return;
				}
				this.updateLayout |= _updateLayout;
				base.Go();
			}
			public void SizeChanged()
			{
				// Throttle
				if (Environment.TickCount64 - lastUpdateTick > 100 )
				{
					Go(false);
				}

			}
			public void PopupsChanged()
			{
				Go(false);
			}

			public bool updateLayout;

			Task F()
			{
				

				try
				{
					if(canvas is null || instance?.grid is null)
						return Task.CompletedTask;
					var _updateLayout = this.updateLayout;
					this.updateLayout = false;
					lastUpdateTick = Environment.TickCount64;

				var c = Settings.layoutOffsets[Settings.layout];
				var gridSize = instance.grid.ActualSize;
				gridSize.X = gridSize.X.Max(1.0f);
				gridSize.Y = (gridSize.Y).Max(1.0f);

				float tabWidth = (float)instance.columnTabs.ActualWidth;
				float topTabHeight = (float) instance.rightTabs.ActualHeight;
				int webWidth;
				if (!_updateLayout )
				{
					webWidth =instance.columnHtml.ActualWidth.RoundToInt();
					c.htmlScale = ( (float)webWidth / htmlBaseWidth);
				}
				else
				{
					webWidth = (c.htmlScale*htmlBaseWidth).RoundToInt().Max(4);
					instance.columnHtml.Width = new(webWidth);
				}

			//	var zoom = c.htmlScale;
			///	var htmlVisible = webWidth > 4;
				//int htmlShift = 0;
			///	var canvasScaledX = (webWidth);
				//Assert( (webWidth-htmlBaseWidth*zoom).Abs() < 4.75f );

				//if(!htmlVisible)
				//{
				//	zoom = 0.875f;
				////	htmlShift = (-htmlBaseWidth*zoom).RoundToInt();
				////	canvasScaledX = 0;
				//}

					//				float zoom = htmlVisible || Settings.webZoomSmall <= 0 ? Settings.webZoom : Settings.webZoomSmall;
				
		//		var canvasScaledY = ( canvasBaseYUnscaled);//.RoundToInt();
			//	popupLeftMargin = 0;//((popupLeftOffset * zoom).RoundToInt() - (canvasScaledX-htmlShift) ).Max0();

			//	popupTopMargin = 0;//((popupTopOffset * zoom).RoundToInt() - canvasScaledY).Max0();
			//	if(popupLeftMargin > popupTopMargin)
			//		popupLeftMargin = 0;
			//	else
			//		popupTopMargin = 0;


//					var canvasBaseY = ( canvasScaledY).Max0();

				//canvas.Margin = new Thickness(0, canvasBaseY, 0, 0);


					// only need 1 to avoid collisions
				//	if (!Alliance.alliancesFetched)
				//		return;
					//				return AppS.DispatchOnUIThreadLow( ()	=>

						if(_updateLayout)
						{
							// has it not been modified

							instance.columnChat.Width = new(c.chatWidth.Clamp(0.125f,0.875f)*gridSize.X);
							instance.rowChat.Height = new(c.chatHeight.Clamp(0.125f, 0.875f)*gridSize.Y);
						}
						else
						{
							c.chatWidth = (float)(instance.columnChat.ActualWidth/gridSize.X);
							c.chatHeight = (float)(instance.rowChat.ActualHeight/gridSize.Y);
						}

						//if(CnVServer.view != null && zoom != webZoomLast)
						//{
						//	webZoomLast = zoom;
						//	var _zoom = (htmlVisible ? zoom : 1.0f);
						//	AppS.DispatchOnUIThreadLow(() => CnVServer.ExecuteScriptAsync($"document.body.style.zoom={_zoom};") );
						//}

						{

							//instance.columnPopup.Width = new(popupLeftMargin);
							
								if (_updateLayout)
								{
									tabWidth = (gridSize.X * c.tabWidth);
									topTabHeight = gridSize.Y * c.tabTopHeight.Clamp(1.0f/8.0f,7.0f/8.0f);
									instance.rowTabTop.Height = new (topTabHeight);
								}
								else
								{
									c.tabWidth = (float)(tabWidth / gridSize.X);
									c.tabTopHeight = (float)(topTabHeight/gridSize.Y);
								}

								tabWidth = tabWidth.Min(gridSize.X - 8).Max(8);
								instance.columnTabs.Width = new(tabWidth);
								rightTabsVisible = tabWidth > 0;
								//	instance.columnRender.Width = new GridLength(1, GridUnitType.Star); //	GridLength.Auto;//	instance.grid.RowDefinitions[0].Height = new(canvasYOffset);
							//	instance.grid.RowDefinitions[5].Height = new(40 * zoom); //new GridLength(newWidth1,GridUnitType.Pixel);//	instance.grid.RowDefinitions[0].Height = 
						}
					//	instance.webView.Margin= new(htmlShift, 0, 0, 0);
					TabPage.LayoutChanged();
					AppS.QueueOnUIThread( CityStats.instance.ProcessScrollSizeChanged );
				//	GameClient.wantFastRefresh = true;
				}
					catch (Exception ex)
					{
						LogEx(ex);
					}
				

				return Task.CompletedTask;

			}


		}

		public static bool rightTabsVisible;
		internal static UpdateHtmlOffsets updateHtmlOffsets = new();

	}
}
