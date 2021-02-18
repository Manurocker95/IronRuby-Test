#if USE_BEHAVIOR_DESIGNER

using UnityEngine;
using BehaviorDesigner.Runtime;
using VirtualPhenix.Dialog;

namespace VirtualPhenix.Integrations.BehaviorDesignerTree
{	
	[System.Serializable]
	public class SharedVariableComparison : SharedVariable<VariableComparison>
	{
		public static implicit operator SharedVariableComparison(VariableComparison value) { return new SharedVariableComparison { mValue = value }; }
	}
}
#endif
