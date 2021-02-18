#if USE_BEHAVIOR_DESIGNER

using UnityEngine;
using BehaviorDesigner.Runtime;
using VirtualPhenix.Controllers;
using VirtualPhenix.Dialog;

namespace VirtualPhenix.Integrations.BehaviorDesignerTree
{
	[System.Serializable]
	public class SharedVPCharacterData : SharedVariable<VP_DialogCharacterData>
	{
		public static implicit operator SharedVPCharacterData(VP_DialogCharacterData value) { return new SharedVPCharacterData { mValue = value }; }
	}
}
#endif