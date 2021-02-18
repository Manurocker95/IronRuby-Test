#if USE_GRID_SYSTEM
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace VirtualPhenix.GridEngine
{
	public class VP_GridFPSSet : MonoBehaviour
	{
		public int TargetFPS;

		protected virtual void Start()
		{
			Application.targetFrameRate = TargetFPS;
		}
	}

}
#endif