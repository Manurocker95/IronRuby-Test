#if USE_BEHAVIOR_DESIGNER

using UnityEngine;
using BehaviorDesigner.Runtime;
using VirtualPhenix.Controllers;
using VirtualPhenix.Dialog;

namespace VirtualPhenix.Integrations.BehaviorDesignerTree
{
	[System.Serializable]
	public class SharedVPDialogPositionData : SharedVariable<VP_DialogPositionData>
	{
		public static implicit operator SharedVPDialogPositionData(VP_DialogPositionData value) { return new SharedVPDialogPositionData { mValue = value }; }
	}
}
#endif