using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if USE_BEHAVIOR_DESIGNER
using BehaviorDesigner.Runtime;

using VirtualPhenix.AI;

namespace VirtualPhenix.Integrations.AI
{
	[System.Serializable]
	public class VP_BehaviorDesignerTreeVariableDictionary : VP_SerializableDictionary<string, SharedVariable>
	{
		
	}
}

#endif