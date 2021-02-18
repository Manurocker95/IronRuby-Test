
#if USE_BEHAVIOR_DESIGNER

using UnityEngine;
using BehaviorDesigner.Runtime;
using VirtualPhenix.Controllers;
using VirtualPhenix.Dialog;

namespace VirtualPhenix.Integrations.BehaviorDesignerTree
{
	[System.Serializable]
	public class SharedVPDialogMessage : SharedVariable<VP_DialogMessage>
	{
		public static implicit operator SharedVPDialogMessage(VP_DialogMessage value) { return new SharedVPDialogMessage { mValue = value }; }
	}
}
#endif