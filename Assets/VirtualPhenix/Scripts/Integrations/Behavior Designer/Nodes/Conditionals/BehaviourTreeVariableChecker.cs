#if USE_BEHAVIOR_DESIGNER
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix;
using VirtualPhenix.Dialog;

namespace VirtualPhenix.Integrations.BehaviorDesignerTree
{
	[TaskCategory(VP_PrefixSetup.MAIN_PREFIX+"/AI/BT Variable Check")]
    [TaskIcon("Assets/Behavior Designer Tutorials/Tasks/Editor/{SkinColor}CanSeeObjectIcon.png")]

    public class BehaviourTreeVariableChecker : Conditional
	{
		[Header("Tree"), Space]
		[SerializeField] private SharedBehaviorTree m_tree;
	    
		[Header("Variable Config"), Space]
        [SerializeField] private SharedString m_variableName;
		[SerializeField] private SharedVariableType m_variableType;
        
		[Header("Comparison"), Space]
        [SerializeField] private SharedVariableComparison m_variableComparison;

		[Header("Possible Values"), Space]
        [SerializeField] private SharedString m_strValue;
        [SerializeField] private SharedBool m_boolValue;
        [SerializeField] private SharedInt m_intValue;
        [SerializeField] private SharedFloat m_floatValue;
        [SerializeField] private SharedDouble m_doubleValue;


		[Header("If the variable is not present, we exit"), Space]
        [SerializeField] private SharedBool m_failureOnNotExist = true;

        private SharedVariable m_variable;

        public override void OnStart()
        {
	        base.OnStart();
            
	        if (m_tree == null)
		        m_tree = Owner.gameObject.GetComponent<BehaviorTree>();
        }

        public bool Check(SharedVariable var)
        {
            if (var.GetValue() == null)
                return false;

            switch (m_variableType.Value)
            {
                case VariableTypes.Bool:
                    if (!(var.GetValue() is bool))
                        return false;

                    bool _graphVar = (bool)var.GetValue();

                    return (m_boolValue.Value == _graphVar);
                case VariableTypes.Double:
                    if (!(var.GetValue() is double))
                        return false;

                    switch (m_variableComparison.Value)
                    {
                        case VariableComparison.Equal:
                            return (m_doubleValue.Value == (double)var.GetValue());
                        case VariableComparison.Mayor:
                            return (m_doubleValue.Value < (double)var.GetValue());
                        case VariableComparison.Minor:
                            return (m_doubleValue.Value > (double)var.GetValue());
                        case VariableComparison.MinorEqual:
                            return (m_doubleValue.Value >= (double)var.GetValue());
                        case VariableComparison.MayorEqual:
                            return (m_doubleValue.Value <= (double)var.GetValue());
                        default:
                            return false;
                    }
                case VariableTypes.Int:
                    if (!(var.GetValue() is int))
                        return false;

                    switch (m_variableComparison.Value)
                    {
                        case VariableComparison.Equal:
                            return (m_intValue.Value == (int)var.GetValue());
                        case VariableComparison.Mayor:
                            return (m_intValue.Value < (int)var.GetValue());
                        case VariableComparison.Minor:
                            return (m_intValue.Value > (int)var.GetValue());
                        case VariableComparison.MinorEqual:
                            return (m_intValue.Value >= (int)var.GetValue());
                        case VariableComparison.MayorEqual:
                            return (m_intValue.Value <= (int)var.GetValue());
                        default:
                            return false;
                    }
                case VariableTypes.Float:
                    if (!(var.GetValue() is float))
                        return false;

                    switch (m_variableComparison.Value)
                    {
                        case VariableComparison.Equal:
                            return (m_floatValue.Value == (float)var.GetValue());
                        case VariableComparison.Mayor:
                            return (m_floatValue.Value < (float)var.GetValue());
                        case VariableComparison.Minor:
                            return (m_floatValue.Value > (float)var.GetValue());
                        case VariableComparison.MinorEqual:
                            return (m_floatValue.Value >= (float)var.GetValue());
                        case VariableComparison.MayorEqual:
                            return (m_floatValue.Value <= (float)var.GetValue());
                        default:
                            return false;
                    }
                case VariableTypes.String:
                    return m_strValue.Value.Equals((string)var.GetValue());

                default:
                    return false;
            }
        }

        public override TaskStatus OnUpdate()
        {
            if (m_tree.Value == null)
                return TaskStatus.Failure;

            m_variable = m_tree.Value.GetVariable(m_variableName.Value);

            if (m_variable == null || m_variable.GetValue() == null)
            {
                return m_failureOnNotExist.Value ? TaskStatus.Failure : TaskStatus.Running;
            }

            return Check(m_tree.Value.GetVariable(m_variableName.Value)) ? TaskStatus.Success : TaskStatus.Failure;
        }
    }
}
#endif