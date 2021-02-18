using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if PHOENIX_ACTIONS
using VirtualPhenix.Actions;
#endif

using UnityEngine.Events;
using VirtualPhenix.Variables;
using VirtualPhenix.Dialog;

namespace VirtualPhenix
{
    [System.Serializable]
    public class VP_GameEventCondition
    {
        public bool m_applyCondition = true;
        public VariableTypes m_variableType = VariableTypes.String;
        public VariableComparison m_comparison = VariableComparison.Equal;
        public string m_variableName = "";
        public bool m_overrideValue = false;

        [Header("Variable Value To Check (Use only Typed one)")]
        public int m_iVal = 0;
        public float m_fVal = 0f;
        public double m_dVal = 0.0;
        public bool m_bVal = false;
        public string m_sVal = "";
        public GameObject m_goVal = null;
       


        public VP_GameEventCondition()
        {

        }

        public bool ConditionTrue()
        {
            switch (m_variableType)
            {
                case VariableTypes.Int:
                    return VP_DialogManager.Instance.GlobalVariables.CheckVariableValue(m_variableName, m_iVal, m_comparison);
                case VariableTypes.Bool:
                    return VP_DialogManager.Instance.GlobalVariables.CheckVariableValue(m_variableName, m_bVal, m_comparison);
                case VariableTypes.Double:
                    return VP_DialogManager.Instance.GlobalVariables.CheckVariableValue(m_variableName, m_dVal, m_comparison);
                case VariableTypes.Float:
                    return VP_DialogManager.Instance.GlobalVariables.CheckVariableValue(m_variableName, m_fVal, m_comparison);
                case VariableTypes.String:
                    return VP_DialogManager.Instance.GlobalVariables.CheckVariableValue(m_variableName, m_sVal, m_comparison);
                case VariableTypes.GameObject:
                    return VP_DialogManager.Instance.GlobalVariables.CheckVariableValue(m_variableName, m_goVal, m_comparison);
            }


            return false;
        }

        public void OverrideVariable()
        {
            if (!m_overrideValue)
                return;

            switch (m_variableType)
            {
                case VariableTypes.Int:
                    VP_DialogManager.Instance.GlobalVariables.ReplaceVariableValue(m_variableName, m_iVal);
                    break;
                case VariableTypes.Bool:
                    VP_DialogManager.Instance.GlobalVariables.ReplaceVariableValue(m_variableName, m_bVal);
                    break;
                case VariableTypes.Double:
                    VP_DialogManager.Instance.GlobalVariables.ReplaceVariableValue(m_variableName, m_dVal);
                    break;
                case VariableTypes.Float:
                    VP_DialogManager.Instance.GlobalVariables.ReplaceVariableValue(m_variableName, m_fVal);
                    break;
                case VariableTypes.String:
                    VP_DialogManager.Instance.GlobalVariables.ReplaceVariableValue(m_variableName, m_sVal);
                    break;
                case VariableTypes.GameObject:
                    VP_DialogManager.Instance.GlobalVariables.ReplaceVariableValue(m_variableName, m_goVal);
                    break;
            }
        }
    }


    [DefaultExecutionOrder(VP_ExecutingOrderSetup.GAME_EVENT), AddComponentMenu(VP_PrefixSetup.MAIN_PREFIX + "/Events/Game Event Trigger With Conditions")]
    public class VP_GameEventsWithCondition : VP_GameEvent
    {
        [Header("GAME VARIABLE CONDITIONS"), Space(10)]
        [SerializeField] private VP_GameConditionEventListDictionary m_conditions = new VP_GameConditionEventListDictionary();

        public override void DoEvent()
        {
            base.DoEvent();

            if (m_conditions == null)
                return;

            foreach (List<VP_GameEventCondition> conditionList in m_conditions.Keys)
            {
                int _count = 0;
                foreach (VP_GameEventCondition condition in conditionList)
                {
                    _count = 0;
                    if (!condition.m_applyCondition || (condition.m_applyCondition && condition.ConditionTrue()))
                    {
                        _count++;
                        if (m_conditions[conditionList].Count > 0 && _count >= m_conditions[conditionList].Count)
                        {
                            m_conditions[conditionList][0].Invoke();
                            break;
                        }
                    }
                    else
                    {
                        if (m_conditions[conditionList].Count > 1)
                        {
                            m_conditions[conditionList][1].Invoke();
                            break;
                        }
                    }
                }
            }
        }

        public virtual void OverrideVariableValue()
        {
            if (m_conditions == null)
                return;

            foreach (List<VP_GameEventCondition> conditionList in m_conditions.Keys)
            {
                foreach (VP_GameEventCondition condition in conditionList)
                {
                    condition.OverrideVariable();
                }
            }

        }
    }

}
