#if USE_BEHAVIOR_DESIGNER

using UnityEngine;
using BehaviorDesigner.Runtime;

namespace VirtualPhenix.Integrations.BehaviorDesignerTree
{
	[System.Serializable]
	public class SharedBasicAnimatorParameter : SharedVariable<VirtualPhenix.Animations.VP_BasicAnimatorParameter>
	{
		public static implicit operator SharedBasicAnimatorParameter(VirtualPhenix.Animations.VP_BasicAnimatorParameter value) { return new SharedBasicAnimatorParameter { mValue = value }; }
	}
}
#endif
