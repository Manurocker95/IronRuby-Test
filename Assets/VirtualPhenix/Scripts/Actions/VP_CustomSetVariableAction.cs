using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix.Dialog;
using VirtualPhenix.Variables;

namespace VirtualPhenix.Actions
{
    public enum GENERAL_FIELD_TYPES
    {
        INT,
        FLOAT,
        DOUBLE,
        BOOL,
        STRING,
        GAMEOBJECT,
        MONOBEHAVIOUR

    }


    public class VP_CustomSetVariableAction : VP_CustomActions
    {
        public enum VARIABLE_PROCEDENCE
        {
            GAME_MANAGER,
            DIALOG_MANAGER,
            CUSTOM
        }

        [SerializeField] private VARIABLE_PROCEDENCE m_variableDB = VARIABLE_PROCEDENCE.GAME_MANAGER;
        [SerializeField] private GENERAL_FIELD_TYPES m_variableTypeToAdd = GENERAL_FIELD_TYPES.STRING;
        [SerializeField] private string m_variableName = "";
        [SerializeField] private int m_iVal = 0;
        [SerializeField] private float m_fVal = 0f;
        [SerializeField] private double m_dVal = 0.0;
        [SerializeField] private bool m_bVal = false;
        [SerializeField] private string m_sVal = "";
        [SerializeField] private GameObject m_goVal = null;

        public override void InvokeAction()
        {
            base.InvokeAction();
            if (m_variableDB == VARIABLE_PROCEDENCE.DIALOG_MANAGER)
            {
                VP_VariableDataBase m_gameVariableDB = VP_DialogManager.Instance.GlobalVariables;

                switch (m_variableTypeToAdd)
                {
                    case GENERAL_FIELD_TYPES.INT:
                        if (m_gameVariableDB == null)
                        {
                            Debug.LogError("There is no database selected.");
                        }
                        else if (!string.IsNullOrEmpty(m_variableName))
                        {
                            m_gameVariableDB.ReplaceVariableValue(m_variableName, m_iVal);
                        }
                        break;
                    case GENERAL_FIELD_TYPES.FLOAT:
                        if (m_gameVariableDB == null)
                        {
                            Debug.LogError("There is no database selected.");
                        }
                        else if (!string.IsNullOrEmpty(m_variableName))
                        {
                            m_gameVariableDB.ReplaceVariableValue(m_variableName, m_fVal);
                        }
                        break;
                    case GENERAL_FIELD_TYPES.DOUBLE:
                        if (m_gameVariableDB == null)
                        {
                            Debug.LogError("There is no database selected.");
                        }
                        else if (!string.IsNullOrEmpty(m_variableName))
                        {
                            m_gameVariableDB.ReplaceVariableValue(m_variableName, m_dVal);
                        }
                        break;
                    case GENERAL_FIELD_TYPES.STRING:
                        if (m_gameVariableDB == null)
                        {
                            Debug.LogError("There is no database selected.");
                        }
                        else if (!string.IsNullOrEmpty(m_variableName))
                        {
                            m_gameVariableDB.ReplaceVariableValue(m_variableName, m_sVal);
                        }
                        break;
                    case GENERAL_FIELD_TYPES.BOOL:
                        if (m_gameVariableDB == null)
                        {
                            Debug.LogError("There is no database selected.");
                        }
                        else if (!string.IsNullOrEmpty(m_variableName))
                        {
                            m_gameVariableDB.ReplaceVariableValue(m_variableName, m_bVal);
                        }
                        break;
                    case GENERAL_FIELD_TYPES.GAMEOBJECT:
                        if (m_gameVariableDB == null)
                        {
                            Debug.LogError("There is no database selected.");
                        }
                        else if (!string.IsNullOrEmpty(m_variableName))
                        {
                            m_gameVariableDB.ReplaceVariableValue(m_variableName, m_goVal);
                        }
                        break;
                }


            }
        }

    }

}
