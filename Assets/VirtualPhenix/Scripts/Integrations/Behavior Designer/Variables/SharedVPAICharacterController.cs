#if USE_BEHAVIOR_DESIGNER

using UnityEngine;
using BehaviorDesigner.Runtime;
using VirtualPhenix.Controllers;

namespace VirtualPhenix.Integrations.BehaviorDesignerTree
{
	[System.Serializable]
	public class SharedVPAICharacterController : SharedVariable<VP_NonPlayableCharacterController>
	{
		public static implicit operator SharedVPAICharacterController(VP_NonPlayableCharacterController value) { return new SharedVPAICharacterController { mValue = value }; }
	}
}
#endif