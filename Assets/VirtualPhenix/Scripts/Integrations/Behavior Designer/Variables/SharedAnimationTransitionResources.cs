#if USE_BEHAVIOR_DESIGNER && USE_ANIMANCER

using UnityEngine;
using BehaviorDesigner.Runtime;
using VirtualPhenix.ResourceReference;

namespace VirtualPhenix.Integrations.BehaviorDesignerTree
{
	[System.Serializable]
	public class SharedAnimationTransitionResources : SharedVariable<VP_AnimationTransitionResources>
	{
		public static implicit operator SharedAnimationTransitionResources(VP_AnimationTransitionResources value) { return new SharedAnimationTransitionResources { mValue = value }; }
	}
}
#endif
