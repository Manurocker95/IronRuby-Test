#if PHOENIX_URP_BLIT_PASS
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace VirtualPhenix
{
	[AddComponentMenu(VP_PrefixSetup.MAIN_PREFIX+"/Camera/Camera Stack Setter"), 
	 DefaultExecutionOrder(VP_ExecutingOrderSetup.CAMERA_STACK_SETTER)]
	public class VP_CameraStackSetter : VP_MonoBehaviour
	{
		[Header("Cameras to stack to")]
		[SerializeField] protected Camera[] m_cameras;
		[Header("overlay camera")]
		[SerializeField] protected Camera m_overlayCamera;
		
		protected override void Initialize()
		{
			base.Initialize();
			SetInitialCameras();
		}

		protected override void StartAllListeners()
		{
			base.StartAllListeners();
			VP_EventManager.StartListening<Camera, Camera[]>(VP_EventSetup.Camera.SET_STACK_TO_CAMERAS, SetCameraStack);
		}
		
		protected override void StopAllListeners()
		{
			base.StopAllListeners();
			VP_EventManager.StopListening<Camera, Camera[]>(VP_EventSetup.Camera.SET_STACK_TO_CAMERAS, SetCameraStack);
		}

		public virtual void SetInitialCameras()
		{
			SetCameraStack(m_overlayCamera, m_cameras);
		}

		public virtual void SetCameraStack(Camera _overlayCamera, IEnumerable<Camera> _cameras)
		{
			if (_overlayCamera != null && _cameras != null)
			{
				foreach (Camera cam in m_cameras)
				{
					var cameraData = cam.GetUniversalAdditionalCameraData();
					var stack = cameraData.cameraStack;
					if (!stack.Contains(_overlayCamera))
					{
						stack.Add(_overlayCamera);
					}
				}
			}
		}
	}
}
#endif

