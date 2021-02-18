using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if USE_BEHAVIOR_DESIGNER
using BehaviorDesigner.Runtime;
using VirtualPhenix.AI;

namespace VirtualPhenix.Integrations.AI
{
	[AddComponentMenu(VP_PrefixSetup.MAIN_PREFIX+"/AI/VP Behavior Designer Tree")]
    public class VP_BehaviorDesignerTree : VP_BehaviorTree
    {
	    [Header("Behavior Designer Tree"),Space]
	    [SerializeField] protected BehaviorTree m_behaviorTree;
	    
	    [SerializeField] protected VP_BehaviorDesignerTreeVariableDictionary m_initialVariables = new VP_BehaviorDesignerTreeVariableDictionary();
#if ODIN_INSPECTOR
	    [Sirenix.Serialization.OdinSerialize] protected VP_SerializableDictionary<string, object> m_initialObject = new VP_SerializableDictionary<string, object>();
#endif
	    public BehaviorTree BehaviorTree { get { return m_behaviorTree; } }
	    public VP_BehaviorDesignerTreeVariableDictionary InitialSharedVariables { get { return m_initialVariables; } }


	    protected override void Reset()
	    {
	    	base.Reset();
	    	
	    	m_behaviorTree = GetComponent<BehaviorTree>();
	    }

	    protected override void Initialize()
	    {
	    	base.Initialize();
	    	
	    	SetInitialVariables();
	    }

	    public override void SetInitialVariables()
	    {
	    	base.SetInitialVariables();
	    	
	    	if (m_initialVariables != null)
	    	{
	    		foreach (string str in m_initialVariables.Keys)
	    		{
	    			SetVariableToBehaviorTree(str, m_initialVariables[str]);
	    		}
	    	}
	    }

	   	public override bool HasVariable(string _name)
	    {
		    var variables = GetAllVariablesFromBehaviorTree();
		    foreach (SharedVariable var in variables)
		    {
		    	if (var.Name.Equals(_name))
			    	return true;
		    }
		    
		    return false;
	    }

	    public override void SetVariableToBehaviorTree<T>(string _name, T _value)
	    {
	    	m_behaviorTree.SetVariableValue(_name, _value);
	    }
	    
	    public virtual SharedVariable GetVariableFromBehaviorTree(string _name)
	    {
	    	return m_behaviorTree.GetVariable(_name);
	    }
	    
	    public virtual List<SharedVariable> GetAllVariablesFromBehaviorTree()
	    {
	    	return m_behaviorTree.GetAllVariables();
	    }
    }
}
#endif