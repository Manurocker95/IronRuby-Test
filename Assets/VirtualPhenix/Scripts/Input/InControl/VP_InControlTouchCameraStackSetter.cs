#if PHOENIX_URP_BLIT_PASS && USE_INCONTROL
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace VirtualPhenix
{
	[AddComponentMenu(VP_PrefixSetup.MAIN_PREFIX+"/Camera/InControl TouchCamera Stack Setter")]
	public class VP_InControlTouchCameraStackSetter : VP_CameraStackSetter
	{
		public override void SetInitialCameras()
		{
			base.SetInitialCameras();
			
			var icm = VP_InControlInputManager.Instance;
			if (icm != null)
			{
				SetCameraStack(icm.TouchManagerCamera, m_cameras);
			}
		}
	}
}
#endif