#if USE_BEHAVIOR_DESIGNER

using UnityEngine;
using BehaviorDesigner.Runtime;

namespace VirtualPhenix.Integrations.BehaviorDesignerTree
{
	[System.Serializable]
	public class SharedVPAnimatorParameterVariable : SharedVariable<VP_AnimatorParameter>
	{
		public static implicit operator SharedVPAnimatorParameterVariable(VP_AnimatorParameter value) { return new SharedVPAnimatorParameterVariable { mValue = value }; }
	}
}
#endif
