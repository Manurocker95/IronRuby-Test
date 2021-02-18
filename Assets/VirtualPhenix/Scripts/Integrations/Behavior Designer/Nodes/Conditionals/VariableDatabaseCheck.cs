#if USE_BEHAVIOR_DESIGNER
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix;
using VirtualPhenix.Dialog;

namespace VirtualPhenix.Integrations.BehaviorDesignerTree
{
	[TaskCategory(VP_PrefixSetup.MAIN_PREFIX+"/Variables")]
	[BehaviorDesigner.Runtime.Tasks.TaskDescription("Compares values in database.")]
  
    public class VariableDatabaseCheck : Conditional
    {
        [SerializeField] private SharedVariableDatabase m_database;
        [SerializeField] private SharedString m_variableName;
        [SerializeField] private SharedVariableType m_variableType;
        [SerializeField] private SharedVariableComparison m_variableComparison;


        [SerializeField] private SharedString m_strValue = "";
        [SerializeField] private SharedBool m_boolValue = true;
        [SerializeField] private SharedInt m_intValue = 0;
        [SerializeField] private SharedFloat m_floatValue = 0;
        [SerializeField] private SharedDouble m_doubleValue = 0;


        public override void OnStart()
        {
            base.OnStart();
        }

        public bool Check(VP_VariableDataBase database, string _varname, VariableTypes _varType, VariableComparison _comparison)
        {
            if (string.IsNullOrEmpty(_varname))
                return false;

            switch (_varType)
            {
                case VariableTypes.Bool:
                    return database.CheckBoolVariable(_varname, m_boolValue.Value, _comparison);
                case VariableTypes.Int:
                    return database.CheckIntVariable(_varname, m_intValue.Value, _comparison);
                case VariableTypes.Double:
                    return database.CheckDoubleVariable(_varname, m_doubleValue.Value, _comparison);
                case VariableTypes.String:
                    return database.CheckStringVariable(_varname, m_strValue.Value, _comparison);
                case VariableTypes.Float:
                    return database.CheckFloatVariable(_varname, m_floatValue.Value, _comparison);
            }

            return false;
        }

        public override TaskStatus OnUpdate()
        {
            if (m_database.Value == null)
                return TaskStatus.Failure;


            return Check(m_database.Value, m_variableName.Value, m_variableType.Value, m_variableComparison.Value) ? TaskStatus.Success : TaskStatus.Failure;
        }
    }
}
#endif