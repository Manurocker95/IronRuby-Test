#if USE_BEHAVIOR_DESIGNER

using UnityEngine;
using BehaviorDesigner.Runtime;
using VirtualPhenix.Dialog;

namespace VirtualPhenix.Integrations.BehaviorDesignerTree
{	
	[System.Serializable]
	public class SharedVariableType : SharedVariable<VariableTypes>
	{
		public static implicit operator SharedVariableType(VariableTypes value) { return new SharedVariableType { mValue = value }; }
	}
}
#endif
