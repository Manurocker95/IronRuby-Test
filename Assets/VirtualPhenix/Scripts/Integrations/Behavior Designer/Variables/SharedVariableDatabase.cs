#if USE_BEHAVIOR_DESIGNER

using UnityEngine;
using BehaviorDesigner.Runtime;
using VirtualPhenix.Controllers;
using VirtualPhenix.Dialog;
using System.Collections.Generic;

namespace VirtualPhenix.Integrations.BehaviorDesignerTree
{
	[System.Serializable]
	public class SharedVariableDatabase : SharedVariable<VP_VariableDataBase>
	{
		public static implicit operator SharedVariableDatabase(VP_VariableDataBase value) { return new SharedVariableDatabase { mValue = value }; }
	}
}
#endif