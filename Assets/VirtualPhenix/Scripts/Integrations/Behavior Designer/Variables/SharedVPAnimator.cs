#if USE_BEHAVIOR_DESIGNER

using UnityEngine;
using BehaviorDesigner.Runtime;

namespace VirtualPhenix.Integrations.BehaviorDesignerTree
{
	[System.Serializable]
	public class SharedVPAnimator : SharedVariable<VP_Animator>
	{
		public static implicit operator SharedVPAnimator(VP_Animator value) { return new SharedVPAnimator { mValue = value }; }
	}
}
#endif
