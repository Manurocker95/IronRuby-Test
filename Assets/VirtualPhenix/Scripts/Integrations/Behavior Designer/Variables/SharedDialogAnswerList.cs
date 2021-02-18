#if USE_BEHAVIOR_DESIGNER

using UnityEngine;
using BehaviorDesigner.Runtime;
using VirtualPhenix.Controllers;
using VirtualPhenix.Dialog;
using System.Collections.Generic;

namespace VirtualPhenix.Integrations.BehaviorDesignerTree
{
	[System.Serializable]
	public class SharedDialogAnswerList : SharedVariable<List<VP_Dialog.Answer>>
	{
		public static implicit operator SharedDialogAnswerList(List<VP_Dialog.Answer> value) { return new SharedDialogAnswerList { mValue = value }; }
	}
}
#endif