#if USE_BEHAVIOR_DESIGNER

using UnityEngine;
using BehaviorDesigner.Runtime;

namespace VirtualPhenix.Integrations.BehaviorDesignerTree
{
	[System.Serializable]
	public class SharedBehavior : SharedVariable<BehaviorDesigner.Runtime.Behavior>
	{
		public static implicit operator SharedBehavior(BehaviorDesigner.Runtime.Behavior value) { return new SharedBehavior { mValue = value }; }
	}
}
#endif
