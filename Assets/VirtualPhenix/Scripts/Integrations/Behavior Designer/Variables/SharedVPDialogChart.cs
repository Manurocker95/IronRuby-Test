
#if USE_BEHAVIOR_DESIGNER

using UnityEngine;
using BehaviorDesigner.Runtime;
using VirtualPhenix.Controllers;
using VirtualPhenix.Dialog;

namespace VirtualPhenix.Integrations.BehaviorDesignerTree
{
	[System.Serializable]
	public class SharedVPDialogChart : SharedVariable<VP_DialogChart>
	{
		public static implicit operator SharedVPDialogChart(VP_DialogChart value) { return new SharedVPDialogChart { mValue = value }; }
	}
}
#endif