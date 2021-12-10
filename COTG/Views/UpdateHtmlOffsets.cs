using System.Threading.Tasks;
//using Windows.UI.Core;
//using Windows.UI.Core.Preview;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;

namespace CnV.Views
{
	using System.Text.Json.Serialization;

	[MessagePackObject]
	public class LayoutOffsets
	{
		[JsonInclude]
		[Key(0)]
		public float htmlScale;
		[JsonInclude]
		[Key(1)]
		public float tabWidth;
		[JsonInclude]
		[Key(2)]
		public float tabTopHeight;
		[JsonInclude]
		[Key(3)]
		public float chatHeight;
		[JsonInclude]
		[Key(4)]
		public float chatWidth;

		public LayoutOffsets()
		{
		}

		public LayoutOffsets(float htmlScale, float tabWidth, float tabTopHeight, float chatHeight, float chatWidth)
		{
			this.htmlScale=htmlScale;
			this.tabWidth=tabWidth;
			this.tabTopHeight=tabTopHeight;
			this.chatHeight=chatHeight;
			this.chatWidth=chatWidth;
		}


	}

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
				this.updateLayout = _updateLayout;
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
				if (canvas is null || instance?.grid is null)
					return Task.CompletedTask;

				try
				{
					lastUpdateTick = Environment.TickCount64;

				var c = SettingsPage.layoutOffsets[SettingsPage.layout];
				var gridSize = instance.grid.ActualSize;
				gridSize.X = gridSize.X.Max(1.0f);
				gridSize.Y = (gridSize.Y).Max(1.0f);

				float tabWidth = (float)instance.columnTabs.ActualWidth;
				float topTabHeight = (float) instance.topTabs.ActualHeight;
				int webWidth;
				if (!updateLayout )
				{
					webWidth =instance.columnHtml.ActualWidth.RoundToInt();
					c.htmlScale = ( (float)webWidth / htmlBaseWidth);
				}
				else
				{
					webWidth = (c.htmlScale*htmlBaseWidth).RoundToInt();
					instance.columnHtml.Width = new(webWidth);
				}

				var zoom = c.htmlScale;
				var htmlVisible = zoom > 0.125f;
				int htmlShift = 0;
				var canvasScaledX = (webWidth);
				Assert( (webWidth-htmlBaseWidth*zoom).Abs() < 0.75f );

				if(!htmlVisible)
				{
					zoom = 0.875f;
					htmlShift = (-htmlBaseWidth*zoom).RoundToInt();
					canvasScaledX = 0;
				}

					//				float zoom = htmlVisible || SettingsPage.webZoomSmall <= 0 ? SettingsPage.webZoom : SettingsPage.webZoomSmall;
				
				var canvasScaledY = (zoom * canvasBaseYUnscaled).RoundToInt();
				popupLeftMargin = ((popupLeftOffset * zoom).RoundToInt() - (canvasScaledX-htmlShift) ).Max0();

				popupTopMargin = ((popupTopOffset * zoom).RoundToInt() - canvasScaledY).Max0();
				var canvasBaseY = (popupTopMargin + canvasScaledY).Max0();

				canvas.Margin = new Thickness(0, canvasBaseY, 0, 0);


					// only need 1 to avoid collisions
					if(popupLeftMargin > popupTopMargin)
						popupLeftMargin = 0;
					else
						popupTopMargin = 0;
				//	if (!Alliance.alliancesFetched)
				//		return;
					//				return AppS.DispatchOnUIThreadLow( ()	=>

						if(updateLayout)
						{
							// has it not been modified

							instance.columnChat.Width = new(c.chatWidth*gridSize.X);
							instance.rowChat.Height = new(c.chatHeight*gridSize.Y);
						}
						else
						{
							c.chatWidth = (float)(instance.columnChat.ActualWidth/gridSize.X);
							c.chatHeight = (float)(instance.rowChat.ActualHeight/gridSize.Y);
						}

						if(JSClient.view != null && zoom != webZoomLast)
						{
							webZoomLast = zoom;
							JSClient.view.ExecuteScriptAsync($"document.body.style.zoom={(htmlVisible ? zoom : 1.0f)};");
						}

						{

							instance.columnPopup.Width = new(popupLeftMargin);
							
								if (updateLayout)
								{
									tabWidth = (gridSize.X * c.tabWidth.Max(1.0f/512f));
									topTabHeight = gridSize.Y * c.tabTopHeight;
									instance.rowTabTop.Height = new (topTabHeight);
								}
								else
								{
									c.tabWidth = (float)(tabWidth / gridSize.X);
									c.tabTopHeight = (float)(topTabHeight/gridSize.Y);
								}

								tabWidth = tabWidth.Min(gridSize.X - canvasScaledX + popupLeftMargin - 8).Max(0);
								instance.columnTabs.Width = new(tabWidth);
								rightTabsVisible = tabWidth > 0;
								//	instance.columnRender.Width = new GridLength(1, GridUnitType.Star); //	GridLength.Auto;//	instance.grid.RowDefinitions[0].Height = new(canvasYOffset);
								instance.grid.RowDefinitions[5].Height = new(40 * zoom); //new GridLength(newWidth1,GridUnitType.Pixel);//	instance.grid.RowDefinitions[0].Height = 
						}
						instance.webView.Margin= new(htmlShift, 0, 0, 0);
					TabPage.LayoutChanged();
						AGame.wantFastRefresh = true;
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
