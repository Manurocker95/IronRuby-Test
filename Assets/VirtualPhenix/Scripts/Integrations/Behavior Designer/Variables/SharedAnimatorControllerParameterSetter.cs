#if USE_BEHAVIOR_DESIGNER

using UnityEngine;
using BehaviorDesigner.Runtime;
using VirtualPhenix.Animations;

namespace VirtualPhenix.Integrations.BehaviorDesignerTree
{
	[System.Serializable]
	public class SharedAnimatorControllerParameterSetter : SharedVariable<VP_AnimatorParameterSetter>
	{
		public static implicit operator SharedAnimatorControllerParameterSetter(VP_AnimatorParameterSetter value) { return new SharedAnimatorControllerParameterSetter { mValue = value }; }
	}
}
#endif
