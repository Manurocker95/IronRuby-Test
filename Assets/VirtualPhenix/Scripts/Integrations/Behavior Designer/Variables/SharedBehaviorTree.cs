#if USE_BEHAVIOR_DESIGNER

using UnityEngine;
using BehaviorDesigner.Runtime;

namespace VirtualPhenix.Integrations.BehaviorDesignerTree
{
	[System.Serializable]
	public class SharedBehaviorTree : SharedVariable<BehaviorDesigner.Runtime.BehaviorTree>
	{
		public static implicit operator SharedBehaviorTree(BehaviorDesigner.Runtime.BehaviorTree value) { return new SharedBehaviorTree { mValue = value }; }
	}
}
#endif
