using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VirtualPhenix.Variables;
using VirtualPhenix.Dialog;

namespace VirtualPhenix.Actions
{
    public enum CONDITION_ACTION_TYPE
    {
        GAME_VARIABLE,
        GAME_UTILS
    }
    public enum CONDITION_ACTION_SELECTION_TYPE
    {
        AND,
        OR
    }

    [System.Serializable]
    public class VP_ConditionCallback : SerializableCallback<bool>
    {

    }

    [System.Serializable]
    public class VP_CustomBranchCondition
    {
        [Header("Current condition values")]
        [SerializeField] private CONDITION_ACTION_TYPE m_conditionType = CONDITION_ACTION_TYPE.GAME_VARIABLE;       
        [SerializeField] private GENERAL_FIELD_TYPES m_variableType = GENERAL_FIELD_TYPES.STRING;
        [SerializeField] private VariableComparison m_comparison = VariableComparison.Equal;
        [SerializeField] private string m_variableName = "";

        [Header("Variable Value To Check (Use only Typed one)")]
        [SerializeField] private int m_iVal = 0;
        [SerializeField] private float m_fVal = 0f;
        [SerializeField] private double m_dVal = 0;
        [SerializeField] private bool m_bVal = false;
        [SerializeField] private string m_sVal = "";
        [SerializeField] private GameObject m_goVal = null;
        [Header("Scriptable Object Utils")]
        [SerializeField] private VP_ConditionCallback m_utilMethod = new VP_ConditionCallback();

        public VP_CustomBranchCondition()
        {
        
        }

        public bool IsTrue()
        {
            if (m_conditionType == CONDITION_ACTION_TYPE.GAME_UTILS)
            {
                if (m_utilMethod == null)
                    return false;

                return m_utilMethod.Invoke();
            }
            else if (m_conditionType == CONDITION_ACTION_TYPE.GAME_VARIABLE)
            {
                switch (m_variableType)
                {
                    case GENERAL_FIELD_TYPES.INT:
                        return VP_DialogManager.Instance.GlobalVariables.CheckVariableValue(m_variableName, m_iVal, m_comparison);
                    case GENERAL_FIELD_TYPES.BOOL:
                        return VP_DialogManager.Instance.GlobalVariables.CheckVariableValue(m_variableName, m_bVal, m_comparison);
                    case GENERAL_FIELD_TYPES.DOUBLE:
                        return VP_DialogManager.Instance.GlobalVariables.CheckVariableValue(m_variableName, m_dVal, m_comparison);
                    case GENERAL_FIELD_TYPES.FLOAT:
                        return VP_DialogManager.Instance.GlobalVariables.CheckVariableValue(m_variableName, m_fVal, m_comparison);
                    case GENERAL_FIELD_TYPES.STRING:
                        return VP_DialogManager.Instance.GlobalVariables.CheckVariableValue(m_variableName, m_sVal, m_comparison);
                    case GENERAL_FIELD_TYPES.GAMEOBJECT:
                        return VP_DialogManager.Instance.GlobalVariables.CheckVariableValue(m_variableName, m_goVal, m_comparison);
                }
            }

            return false;
        }
    }

    public class VP_CustomBranchConditionAction : VP_CustomActions
    {

#if PHOENIX_ACTIONS
        [SerializeField] private CONDITION_ACTION_SELECTION_TYPE m_conditionCumpliment = CONDITION_ACTION_SELECTION_TYPE.AND;
        [SerializeField] private VP_BranchConditionActionListOfListDictionary m_actionSequence = new VP_BranchConditionActionListOfListDictionary();
#endif
        public override void InitActions(System.Action _initCallback = null, System.Action _invokeCallback = null, System.Action<int> _indexCallback = null)
        {
#if PHOENIX_ACTIONS
            foreach (List<VP_CustomBranchCondition> conditions in m_actionSequence.Keys)
            {
                int count = conditions.Count;
                int counter = 0;
                
                foreach (VP_CustomBranchCondition condition in conditions)
                {
                    if (condition.IsTrue())
                    {
                        counter++;
                    }
                }
                
                if ((m_conditionCumpliment == CONDITION_ACTION_SELECTION_TYPE.AND && counter >= count) || (m_conditionCumpliment == CONDITION_ACTION_SELECTION_TYPE.OR && counter > 0))
                {
                    foreach (VP_CustomActions action in m_actionSequence[conditions][0])
                    {
                        if (action is VP_CustomSetListIndex)
                        {
                            action.InitActions(null, null, _indexCallback);
                        }
                        else if (action is VP_CustomBranchConditionAction)
                        {
                            action.m_interactableObjectName = m_interactableObjectName;
                            action.InitActions(_initCallback, _invokeCallback, _indexCallback);
                        }
                        else
                        {
                            action.InitActions(null, null, null);
                        }
                    }
                }
                else if (m_actionSequence[conditions].Count > 0)
                {
                    foreach (VP_CustomActions action in m_actionSequence[conditions][1])
                    {
                        if (action is VP_CustomSetListIndex)
                        {
                            action.InitActions(null, null, _indexCallback);
                        }
                        else if (action is VP_CustomBranchConditionAction)
                        {
                            action.m_interactableObjectName = m_interactableObjectName;
                            action.InitActions(_initCallback, _invokeCallback, _indexCallback);
                        }
                        else
                        {
                            action.InitActions(null, null, null);
                        }
                    }
                }
            }

            base.InitActions(null, null, null);
#endif
        }
    }

}
