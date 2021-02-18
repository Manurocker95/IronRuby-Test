#if USE_BEHAVIOR_DESIGNER

using UnityEngine;
using BehaviorDesigner.Runtime;

namespace VirtualPhenix.Integrations.BehaviorDesignerTree
{
	[System.Serializable]
	public class SharedDouble : SharedVariable<double>
	{
		public static implicit operator SharedDouble(double value) { return new SharedDouble { mValue = value }; }
	}
}
#endif
