
//using Windows.UI.Core;

namespace CnV;
using static GameClient;

 static partial class View
{
	internal const float viewHoverZGain = 0.5f / 64.0f;
	
	internal const float viewHoverZDamping = 0.75f;
	internal const float viewHoverElevationFreq = 20.0f;

	internal const float cameraControlFrequencyTight = 30.0f;
	internal const float cameraControlFrequencyNormal = 8.0f;
	internal const float cameraControlDampingTight = 1.125f;
	internal const float cameraControlDampingNormal = 1.0f;
	internal static float panV1 = 0.125f;
	internal static float panV0 = panV1*0.75f;
	static float panSlowdown = 1.0f;
	const float viewControlTightnessDecay = 4;
	// Velocity
	// public static Vector3 viewVW = default;
	internal static double simT;
	internal static double simTimePerFrameGuess;
	internal static double simTimePerFrameSmoothed;

	internal static void StepViewToPresent()
	{
		{
			int splits = (timeSinceLastFrame*targetStepsPerSecond).RoundToInt().Max(1);
			Assert(timeSinceLastFrame <= 0.25f);
			var dt = (float)(timeSinceLastFrame / splits);
			Assert(View.targetZ >= View.viewMinZ);
			Assert(View.targetZ <= View.viewMaxZ);
			for(int step = 0;step<splits;++step)
			{
				var viewVW0 = viewVW;
				//
				// Camera (ViewW, ViewWV, ViewWTarget, VW
				//
				var damping = cameraControlDampingNormal;//,cameraControlDampingTight);
				var viewKs = cameraControlFrequencyNormal.FreqencyToKsAndKd(damping);//,cameraControlFrequencyTight).FreqencyToKsAndKd(damping);
				if( View.isCoasting)
				{
					float lg = viewVW.ToV2().Length();
					System.Numerics.Vector2 dr;
					if(lg <= panV0 * viewW.Z )
					{
						dr = viewVW.ToV2() *viewKs.kd*dt;
					}
					else
					{
						dr = viewVW.ToV2()*panSlowdown*dt; // inertia
					}
					viewVW = viewVW with { X= viewVW.X - dr.X,Y = viewVW.Y - dr.Y };
				}
				else
				{

						viewVW += ((viewTargetW - viewW) * viewKs.ks - viewVW*viewKs.kd)*dt;
				
				}
				var dW = (viewVW0+viewVW)*(0.5f * dt); // Mid point
				viewW  += dW;
				viewW.Z = viewW.Z.Clamp(viewMinZ,viewMaxZ);
				
				if(View.isCoasting)
				{
					viewTargetW = viewW;
				}
			//	viewControlTightness -= viewControlTightness*viewControlTightnessDecay*dt;

			//	eventTimeOffsetLag += (ShellPage.instance.eventTimeOffset - eventTimeOffsetLag) * dt*8.0f;

			}
		}
		{

			//		var _serverNow = CnVServer.serverTime;
			var dt = (float)timeSinceLastFrame.Min(1.0f/8.0f);// ((float)(timerT - animationT)).Min(0.25f);// (float)gameTime.ElapsedGameTime.TotalSeconds; // max delta is 1s
			{                                                      //	lastDrawTime = _serverNow;
				if(Player.viewHover != Player.maxId)
					Player.viewHoverFade = (Player.viewHoverFade + dt*2).Min(1.0f);
				Player.priorViewHoverFade = (Player.priorViewHoverFade - dt).Max(0.0f);

				var hover = lastCanvasC;

				if(hover != 0 && World.GetTile(hover.CidToWorldC()).type != 0)
				{
					if(!viewHovers.Exists(a => a.cid == lastCanvasC))
					{
						viewHovers.Add((lastCanvasC, 1.0f / 32.0f, 0.0f));
					}
				}
				{
					var removeMe = -1;
					int count = viewHovers.Count;
					for(int i = 0;i < count;++i)
					{
						var cid = viewHovers[i].cid;

						float z = viewHovers[i].z;
						float vz = viewHovers[i].vz;
						var kd = (viewHoverElevationFreq);
						var ks = AMath.CritDampingKs(kd);

						vz += (((cid == hover ? 1.0f : 0.0f) - z) * ks - vz * kd) * dt;
						z += vz * (float)dt;
						viewHovers[i] = (cid, z, vz);


						if(z <= 1.0f / 32.0f)
						{
							removeMe = i;
						}
					}
					// Just remove one per frame, we'll get the rest next time,
					if(removeMe != -1)
						viewHovers.RemoveAt(removeMe);

				}
			}

		} }
		}
	




