#if USE_BEHAVIOR_DESIGNER

using UnityEngine;
using BehaviorDesigner.Runtime;
using VirtualPhenix.Controllers;

namespace VirtualPhenix.Integrations.BehaviorDesignerTree
{
	[System.Serializable]
	public class SharedVPCharacterController : SharedVariable<VP_CharacterController>
	{
		public static implicit operator SharedVPCharacterController(VP_CharacterController value) { return new SharedVPCharacterController { mValue = value }; }
	}
}
#endif