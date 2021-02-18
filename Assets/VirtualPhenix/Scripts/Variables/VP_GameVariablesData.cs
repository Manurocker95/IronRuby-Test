using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix.Dialog;

namespace VirtualPhenix.Variables
{
    [System.Serializable]
    public class VP_GameVariablesData
    {
        [SerializeField] private bool m_alwaysReplace = false;
        [SerializeField] private Dictionary<string, FieldData> m_gameVariables;
        public Dictionary<string, FieldData> GameVariables { get { return m_gameVariables; } set { m_gameVariables = value; } }

        public FieldData GetVariableValue<T>(string _key)
        {
            return (m_gameVariables.ContainsKey(_key)) ? m_gameVariables[_key] : null;
        }

        public FieldData GetVariableValue(string _key)
        {
            return (m_gameVariables.ContainsKey(_key)) ? m_gameVariables[_key] : null;
        }

        public void AddVariable<T>(Field<T> _var)
        {
            string varName = _var.Name;
            if (!m_gameVariables.ContainsKey(varName))
            {
                m_gameVariables.Add(varName, _var);
            }
            else
            {
                if (m_alwaysReplace)
                {
                    ReplaceVariableValue(_var);
                }
            }
        }

        public void ReplaceVariableValue<T>(Field<T> _var, bool _addIt = false)
        {
            string varName = _var.Name;
            if (m_gameVariables.ContainsKey(varName))
            {
                m_gameVariables[varName] = _var;
            }
            else
            {
                if (_addIt)
                {
                    AddVariable(_var);
                }
            }
        }

        public void RemoveVariableValue(string varName)
        {
            if (m_gameVariables.ContainsKey(varName))
            {
                m_gameVariables.Remove(varName);
            }
        }

        public enum VALUE_CHECK
        {
            EQUAL,
            MAJOR,
            MINOR
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        public bool CheckTVariable<T>(string _varName, T _value, VALUE_CHECK _valueCheck)
        {
            if (string.IsNullOrEmpty(_varName))
                return false;

            Field<T> var = (Field<T>)GetVariableValue<Field<T>>(_varName);

            if (var == null)
                return false;

            switch (_valueCheck)
            {
                case VALUE_CHECK.EQUAL:
                    return var.IsTheSameAs(_value);
            }

            return false;
        }

        public bool CheckIntVariable(string _varName, int _value, VALUE_CHECK _valueCheck)
        {
            if (string.IsNullOrEmpty(_varName))
                return false;

            Field<int> var = (Field<int>)GetVariableValue<Field<int>>(_varName);

            if (var == null)
                return false;

            switch (_valueCheck)
            {
                case VALUE_CHECK.EQUAL:
                    return var.Value == _value;
                case VALUE_CHECK.MAJOR:
                    return _value > var.Value;
                case VALUE_CHECK.MINOR:
                    return _value < var.Value;
            }

            return false;
        }

        public bool CheckFloatVariable(string _varName, float _value, VALUE_CHECK _valueCheck)
        {
            if (string.IsNullOrEmpty(_varName))
                return false;

            Field<float> var = (Field<float>)GetVariableValue<Field<float>>(_varName);

            if (var == null)
                return false;

            switch (_valueCheck)
            {
                case VALUE_CHECK.EQUAL:
                    return var.Value == _value;
                case VALUE_CHECK.MAJOR:
                    return _value > var.Value;
                case VALUE_CHECK.MINOR:
                    return _value < var.Value;
            }

            return false;
        }


        public bool CheckFloatVariable(string _varName, double _value, VALUE_CHECK _valueCheck)
        {
            if (string.IsNullOrEmpty(_varName))
                return false;

            Field<double> var = (Field<double>)GetVariableValue<Field<double>>(_varName);

            if (var == null)
                return false;

            switch (_valueCheck)
            {
                case VALUE_CHECK.EQUAL:
                    return var.Value == _value;
                case VALUE_CHECK.MAJOR:
                    return _value > var.Value;
                case VALUE_CHECK.MINOR:
                    return _value < var.Value;
            }

            return false;
        }

        public bool CheckBoolVariable(string _varName, bool _value, VALUE_CHECK _valueCheck)
        {
            if (string.IsNullOrEmpty(_varName))
                return false;

            Field<bool> var = (Field<bool>)GetVariableValue<Field<bool>>(_varName);

            if (var == null)
                return false;

            switch (_valueCheck)
            {
                case VALUE_CHECK.EQUAL:
                    return var.Value == _value;
            }

            return false;
        }

        public bool CheckBoolVariable(string _varName, string _value, VALUE_CHECK _valueCheck)
        {
            if (string.IsNullOrEmpty(_varName))
                return false;

            Field<string> var = (Field<string>)GetVariableValue<Field<string>>(_varName);

            if (var == null)
                return false;

            switch (_valueCheck)
            {
                case VALUE_CHECK.EQUAL:
                    return var.Value == _value;
            }

            return false;
        }

        public bool CheckBoolVariable(string _varName, GameObject _value, VALUE_CHECK _valueCheck)
        {
            if (string.IsNullOrEmpty(_varName))
                return false;

            Field<GameObject> var = (Field<GameObject>)GetVariableValue<Field<GameObject>>(_varName);

            if (var == null)
                return false;

            switch (_valueCheck)
            {
                case VALUE_CHECK.EQUAL:
                    return var.Value == _value;
            }

            return false;
        }

    }
}