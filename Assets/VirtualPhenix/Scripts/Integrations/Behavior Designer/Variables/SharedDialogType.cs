#if USE_BEHAVIOR_DESIGNER

using UnityEngine;
using BehaviorDesigner.Runtime;
using VirtualPhenix.Dialog;

namespace VirtualPhenix.Integrations.BehaviorDesignerTree
{
	[System.Serializable]
	public class SharedDialogType : SharedVariable<DIALOG_TYPE>
	{
		public static implicit operator SharedDialogType(DIALOG_TYPE value) { return new SharedDialogType { mValue = value }; }
	}
}
#endif
