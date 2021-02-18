#if USE_BEHAVIOR_DESIGNER

using UnityEngine;
using BehaviorDesigner.Runtime;

namespace VirtualPhenix.Integrations.BehaviorDesignerTree
{
	[System.Serializable]
	public class SharedAvatarMask : SharedVariable<AvatarMask>
	{
		public static implicit operator SharedAvatarMask(AvatarMask value) { return new SharedAvatarMask { mValue = value }; }
	}
}
#endif
