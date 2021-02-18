using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if USE_BEHAVIOR_DESIGNER
using BehaviorDesigner.Runtime;

namespace VirtualPhenix.Integrations.BehaviorDesignerTree
{
	[AddComponentMenu(VP_PrefixSetup.MAIN_PREFIX+"/Integrations/Behavior Designer/Behavior Designer Variable Setter")]
	public class VP_BehaviorDesignerVariableSetter : VP_MonoBehaviour
	{
		[SerializeField] protected BehaviorTree m_behaviorTree;

		protected override void Initialize()
		{
			base.Initialize();
			
		}
		
		public virtual void SetVariableValue<T>(string _variableName, T _value)
		{
			if (m_behaviorTree != null)
			{
				SetVariableValueToBehaviorTree(m_behaviorTree, _variableName, _value);
			}
		}
		
		public virtual void SetVariableValueToBehaviorTree<T>(BehaviorTree _behaviorTree, string _variableName, T _value)
		{
			_behaviorTree.SetVariableValue(_variableName, _value);
		}
	}

}
#endif