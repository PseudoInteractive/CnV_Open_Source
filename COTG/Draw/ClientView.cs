using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static CnV.View;

namespace CnV;

	using System.Numerics;

	internal static partial class ClientView
	{
		static async void DoZoom(float delta, bool skipPan)
		{


			var dZoom   = delta.SignOr0() * 0.0f + delta * (1.0f / 256);
			var newZoom = (cameraZoom * MathF.Exp(dZoom)).Clamp(1, maxZoom);
			var cBase   = new Vector2(); ////GetCanvasPosition(pt.Position) - halfSpan;

			var skipMove = skipPan;

			if(IsCityView())
			{
				if(await AutoSwitchCityView())
				{
					if(!skipPan)
					{
						cBase         =  (City.build.CidToWorldV() - cameraC) * cameraZoom;
						AGame.CameraC += 0.25f * (City.build.CidToWorldV() - cameraC); // nudge towards center
					}
				}
			}
			else
			{
				skipMove = true;
			}


			if(!skipMove)
			{
				// when zooming in in city mode, constrain to city
				var c0 = cBase / cameraZoom;
				var c1 = cBase / newZoom;
				AGame.CameraC += c0 - c1;
			}


			cameraZoom = newZoom;
			AutoSwitchViewMode();
			ClearHover();
			//    ChatTab.L("CWheel " + wheel);
		}

		private static async Task<bool> AutoSwitchCityView()
		{
			if(cameraZoom <= cityZoomThreshold)
				return false;


			var   wc        = cameraC.RoundToInt();
			var   target    = wc;
			float bestScore = float.MaxValue;
			// Try a different city

			for(int x = 0; x <= 0; ++x)
				for(int y = 0; y <= 0; ++y)
				{
					var   dxy = (x, y);
					float lg  = dxy.Length();
					if(lg > bestScore)
						continue;

					var probe = wc.Sum(dxy);

					if(City.CanVisit(probe.WorldToCid()))
					{
						target    = probe;
						bestScore = dxy.Length();

					}
				}
			if(bestScore < float.MaxValue)
			{
				var cid = target.WorldToCid();
				if(cid != City.build)
				{
					try
					{
						if(!await JSClient.CitySwitch(target.WorldToCid(), true))
						{
							EnsureNotCityView();
						}
					}
					catch(UIException ex)
					{
						LogEx(ex);
						EnsureNotCityView();

					}
				}
				return true;
			}
			return false;
		}

		public static void ClearHover()
		{
			if(!IsCityView())
				ToolTips.contToolTip = null;
			lastCanvasC      = 0;
			lastCont         = -1;
			ToolTips.toolTip = null;
			CityView.hovered = AUtil.NanXY;
			Spot.viewHover   = 0;
			Player.viewHover = 0;
		}

		public static void AutoSwitchViewMode()
		{
			var _viewMode = cameraZoom >= cityZoomThreshold ? ViewMode.city : cameraZoom > cityZoomWorldThreshold ? ViewMode.region : ViewMode.world;
			if(_viewMode != View.viewMode)
			{
				SetViewMode(_viewMode);
			}
		}
		public static void EnsureNotCityView()
		{
			if(cameraZoom > cameraZoomRegionDefault)
			{
				cameraZoom = cameraZoomRegionDefault;
				AutoSwitchViewMode();
			}
		}

		public static bool BringCidIntoWorldView(this int cid, bool lazy)
		{
			var newC = cid.CidToWorldV();
			var dc   = newC - AGame.cameraC;
			if(!lazy ||
				!AGame.clip.ContainsSquare(dc, 0.5f *GameClient.pixelScale))
					//		c0.X < CnV.AGame.c(dc.X.Abs() + 0.5f) * AGame.pixelScale >= AGame.halfSpan.X ||
					//	   (dc.Y.Abs() + 0.5f) * AGame.pixelScale >= AGame.halfSpan.Y)
			{
				var thresh = lazy ? 0.75f : 0.25f;
				// only move if moving more than about 1 city span
				if(Vector2.Distance(AGame.cameraC, newC) >= thresh)
				{

					AGame.CameraC = newC;
					if(cid != City.build && (!City.CanVisit(cid) || !Spot.CanChangeCity(cid)))
						EnsureNotCityView();

					return true;
				}

			}
			return false;
		}
		public static void SetViewMode(ViewMode _viewMode, bool leaveZoom = false)
		{
			//	var _webviewHasFocus = pwebviewHasFocus.HasValue ? pwebviewHasFocus.Value : webviewHasFocus;
			var priorView = View.viewMode;
			//	var priorWebviewHasFocus = webviewHasFocus;
			View.viewMode = _viewMode;
			//	webviewHasFocus = _webviewHasFocus;

			if(priorView == View.viewMode)
				return;

			//Log($"!Focus5: {hasKeyboardFocus}");
			//hasKeyboardFocus = 0;
			if(!leaveZoom && priorView != View.viewMode)
			{
				if(View.viewMode == ViewMode.world)
				{
					if(View.cameraZoom > View.cityZoomWorldThreshold)
						View.cameraZoom = View.cameraZoomWorldDefault;
				}
				else if(View.viewMode == ViewMode.region)
				{
					if(View.cameraZoom  > View.cityZoomThreshold ||
						View.cameraZoom < View.cityZoomWorldThreshold)
						View.cameraZoom = View.cameraZoomRegionDefault;
				}
				else
				{
					if(View.cameraZoom < View.cityZoomThreshold)
						View.cameraZoom = View.cityZoomDefault;
				}
			}

			//	ShellPage.isOverPopup = false;// reset
			//var isWorld = IsWorldView();
			//				ShellPage.isHitTestVisible = !webviewHasFocus;
			//	UpdateFocus();
			//AppS.DispatchOnUIThreadLow(() =>
			//{
			//	//	instance.webFocus.IsChecked = webviewHasFocus;
			//	//	Log($"!FocusWeb: {hasKeyboardFocus} w{webviewHasFocus} w2{webviewHasFocus2}");
			//	//		ShellPage.isOverPopup = false;// reset again in case it changed
			//	ShellPage.canvas.IsHitTestVisible = ShellPage.isOverPopup
			//	ShellPage.canvas.Visibility = !ShellPage.canvasVisible ? Visibility.Collapsed : Visibility.Visible;
			//	AGame.UpdateMusic();

			//	if (!webviewHasFocus && priorWebviewHasFocus)
			//	{
			//		TakeKeyboardFocus();
			//	}
			//});
			Audio.UpdateMusic();
		}
}

