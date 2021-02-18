#if USE_BEHAVIOR_DESIGNER

using UnityEngine;
using BehaviorDesigner.Runtime;
using VirtualPhenix.Controllers;
using VirtualPhenix.Dialog;

namespace VirtualPhenix.Integrations.BehaviorDesignerTree
{
	[System.Serializable]
	public class SharedDialogAnswer : SharedVariable<VP_Dialog.Answer>
	{
		public static implicit operator SharedDialogAnswer(VP_Dialog.Answer value) { return new SharedDialogAnswer { mValue = value }; }
	}
}
#endif