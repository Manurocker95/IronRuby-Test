using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Dialog
{
    [System.Serializable]
    public class VP_VariableDataBase
    {
        [SerializeField] private VP_BoolVariables m_boolVariables;
        [SerializeField] private VP_IntVariables m_intVariables;
        [SerializeField] private VP_FloatVariables m_floatVariables;
        [SerializeField] private VP_DoubleVariables m_doubleVariables;
        [SerializeField] private VP_StringVariables m_stringVariables;
        [SerializeField] private VP_GameObjectVariables m_gameObjectVariables;
        [SerializeField] private VP_UnityObjectVariables m_unityObjectVariables;

        public ICollection<bool> GetBoolVariables { get { return m_boolVariables.Values; } }
        public VP_BoolVariables GetBoolVariableList { get { return m_boolVariables; } }
        public VP_BoolVariables SetBoolVariableList { set { m_boolVariables = value; } }
        public ICollection<int> GetIntVariables { get { return m_intVariables.Values; } }
        public VP_IntVariables GetIntVariableList { get { return m_intVariables; } }
        public VP_IntVariables SetIntVariableList { set { m_intVariables = value; } }
        public ICollection<float> GetFloatVariables { get { return m_floatVariables.Values; } }
        public VP_FloatVariables GetFloatVariableList { get { return m_floatVariables; } }
        public VP_FloatVariables SetFloatVariableList { set { m_floatVariables = value; } }
        public ICollection<double> GetDoubleVariables { get { return m_doubleVariables.Values; } }
        public VP_DoubleVariables GetDoubleVariableList { get { return m_doubleVariables; } }
        public VP_DoubleVariables SetDoubleVariableList { set { m_doubleVariables = value; } }
        public ICollection<string> GetStringVariables { get { return m_stringVariables.Values; } }
        public VP_StringVariables GetStringVariableList { get { return m_stringVariables; } }
        public VP_StringVariables SetStringVariableList { set { m_stringVariables = value; } }
        public ICollection<GameObject> GetGameObjectVariables { get { return m_gameObjectVariables.Values; } }
        public VP_GameObjectVariables GetGameObjectVariableList { get { return m_gameObjectVariables; } }
        public VP_GameObjectVariables SetGameObjectVariableList { set { m_gameObjectVariables = value; } }
        public ICollection<UnityEngine.Object> GetUnityObjectVariables { get { return m_unityObjectVariables.Values; } }
        public VP_UnityObjectVariables GetUnityObjectVariableList { get { return m_unityObjectVariables; } }
        public VP_UnityObjectVariables SetUnityObjectVariableList { set { m_unityObjectVariables = value; } }

        public VP_VariableDataBase()
        {
            InitDB();
        }

        public void InitDB()
        {
            if (m_boolVariables == null)
                m_boolVariables = new VP_BoolVariables();      
            
            if (m_intVariables == null)
                m_intVariables = new VP_IntVariables();

            if (m_floatVariables == null)
                m_floatVariables = new VP_FloatVariables();
            
            if (m_doubleVariables == null)
                m_doubleVariables = new VP_DoubleVariables();     
            
            if (m_stringVariables == null)
                m_stringVariables = new VP_StringVariables();      
            
            if (m_gameObjectVariables == null)
                m_gameObjectVariables = new VP_GameObjectVariables();     
            
            if (m_unityObjectVariables == null)
                m_unityObjectVariables = new VP_UnityObjectVariables();            
            
        }

        public void CopyFrom(VP_VariableDataBase _newDatabase)
        {
            VP_BoolVariables bvar = _newDatabase.GetBoolVariableList;
            foreach (string b in bvar.Keys)
            {
                ReplaceBoolVariable(b, bvar[b]);
            }

            VP_IntVariables ivar = _newDatabase.GetIntVariableList;
            foreach (string i in ivar.Keys)
            {
                ReplaceIntVariable(i, ivar[i]);
            }

            VP_FloatVariables fvar = _newDatabase.GetFloatVariableList;
            foreach (string f in fvar.Keys)
            {
                ReplaceFloatVariable(f, fvar[f]);
            }

            VP_DoubleVariables dvar = _newDatabase.GetDoubleVariableList;
            foreach (string d in dvar.Keys)
            {
                ReplaceDoubleVariable(d, dvar[d]);
            }

            VP_StringVariables svar = _newDatabase.GetStringVariableList;
            foreach (string s in svar.Keys)
            {
                ReplaceStringVariable(s, svar[s]);
            }

            VP_GameObjectVariables gvar = _newDatabase.GetGameObjectVariableList;
            foreach (string g in gvar.Keys)
            {
                ReplaceGameObjectVariable(g, gvar[g]);
            }

            VP_UnityObjectVariables ovar = _newDatabase.GetUnityObjectVariableList;
            foreach (string o in ovar.Keys)
            {
                ReplaceUnityObjectVariable(o, gvar[o]);
            }
        }

        public void ClearAll()
        {
            ClearBoolVariables();
            ClearIntVariables();
            ClearFloatVariables();
            ClearStringVariables();
            ClearDoubleVariables();
            ClearGameObjectVariables();
            ClearUnityObjectVariables();
        }

        public void ReplaceVariableValue<T>(string _variableName, T _type)
        {
            if (string.IsNullOrEmpty(_variableName))
            {
                Debug.LogError("VARIABLE NAME IS NULL");
                return;
            }

            if (typeof(T).Equals(typeof(string)))
            {
                ReplaceStringVariable(_variableName, (string)System.Convert.ChangeType(_type, typeof(string)));
            }
            else if (typeof(T).Equals(typeof(bool)))
            {
                ReplaceBoolVariable(_variableName, (bool)System.Convert.ChangeType(_type, typeof(bool)));
            }
            else if (typeof(T).Equals(typeof(int)))
            {
                ReplaceIntVariable(_variableName, (int)System.Convert.ChangeType(_type, typeof(int)));
            }
            else if (typeof(T).Equals(typeof(float)))
            {
                ReplaceFloatVariable(_variableName, (float)System.Convert.ChangeType(_type, typeof(float)));
            }
            else if (typeof(T).Equals(typeof(double)))
            {
                ReplaceDoubleVariable(_variableName, (double)System.Convert.ChangeType(_type, typeof(double)));
            }
            else if (typeof(T).Equals(typeof(UnityEngine.GameObject)))
            {
                ReplaceGameObjectVariable(_variableName, (GameObject)System.Convert.ChangeType(_type, typeof(GameObject)));
            }
            else if (typeof(T).Equals(typeof(UnityEngine.Object)))
            {
                ReplaceUnityObjectVariable(_variableName, (Object)System.Convert.ChangeType(_type, typeof(Object)));
            }
            else
            {
                Debug.LogError("Unknown or not accepted variable type: " + typeof(T) + " in variable with name " + _variableName);
            }
        }

        public void ReplaceVariable<T>(string _variableName, T _type)
        {
            ReplaceVariableValue(_variableName, _type);
        }

        public void AddVariableValue<T>(string _variableName, T _type)
        {
            if (string.IsNullOrEmpty(_variableName))
            {
                Debug.LogError("VARIABLE NAME IS NULL");
                return;
            }

            if (typeof(T).Equals(typeof(string)))
            {
                AddStringVariable(_variableName, (string)System.Convert.ChangeType(_type, typeof(string)));
            }
            else if (typeof(T).Equals(typeof(bool)))
            {
                AddBoolVariable(_variableName, (bool)System.Convert.ChangeType(_type, typeof(bool)));
            }
            else if (typeof(T).Equals(typeof(int)))
            {
                AddIntVariable(_variableName, (int)System.Convert.ChangeType(_type, typeof(int)));
            }
            else if (typeof(T).Equals(typeof(float)))
            {
                AddFloatVariable(_variableName, (float)System.Convert.ChangeType(_type, typeof(float)));
            }
            else if (typeof(T).Equals(typeof(double)))
            {
                AddDoubleVariable(_variableName, (double)System.Convert.ChangeType(_type, typeof(double)));
            }
            else if (typeof(T).Equals(typeof(UnityEngine.GameObject)))
            {
                AddGameObjectVariable(_variableName, (GameObject)System.Convert.ChangeType(_type, typeof(GameObject)));
            }
            else if (typeof(T).Equals(typeof(UnityEngine.Object)))
            {
                AddUnityObjectVariable(_variableName, (Object)System.Convert.ChangeType(_type, typeof(Object)));
            }
            else
            {
                Debug.LogError("Unknown or not accepted variable type: " + typeof(T) + " in variable with name " + _variableName);
            }
        }

        public bool CheckVariableValue<T>(string _variableName, T _type, VariableComparison _comparison = VariableComparison.Equal)
        {
            if (string.IsNullOrEmpty(_variableName))
            {
                Debug.LogError("VARIABLE NAME IS NULL");
                return false;
            }

            if (typeof(T).Equals(typeof(string)))
            {
                return CheckBoolVariable(_variableName, (bool)System.Convert.ChangeType(_type, typeof(bool)), VariableComparison.Equal);
            }
            else if (typeof(T).Equals(typeof(bool)))
            {
                return CheckStringVariable(_variableName, (string)System.Convert.ChangeType(_type, typeof(string)), VariableComparison.Equal);
            }
            else if (typeof(T).Equals(typeof(int)))
            {
                return CheckIntVariable(_variableName, (int)System.Convert.ChangeType(_type, typeof(int)), _comparison);
            }
            else if (typeof(T).Equals(typeof(float)))
            {
                return CheckFloatVariable(_variableName, (float)System.Convert.ChangeType(_type, typeof(float)), _comparison);
            }
            else if (typeof(T).Equals(typeof(double)))
            {
                return CheckDoubleVariable(_variableName, (double)System.Convert.ChangeType(_type, typeof(double)), _comparison);
            }
            else if (typeof(T).Equals(typeof(UnityEngine.GameObject)))
            {
                return CheckGameObjectVariable(_variableName, (GameObject)System.Convert.ChangeType(_type, typeof(GameObject)), VariableComparison.Equal);
            }
            else if (typeof(T).Equals(typeof(UnityEngine.Object)))
            {
                return CheckUnityObjectVariable(_variableName, (Object)System.Convert.ChangeType(_type, typeof(Object)), VariableComparison.Equal);
            }
            else
            {
                Debug.LogError("Unknown or not accepted variable type: " + typeof(T) + " in variable with name " + _variableName);
            }

            return false;
        }

        public FieldData GetVariableValue<T>(string _variableName, T _type)
        {
            if (string.IsNullOrEmpty(_variableName))
            {
                Debug.LogError("VARIABLE NAME IS NULL");
                return null;
            }

            if (typeof(T).Equals(typeof(string)))
            {                               
                return new Field<string>(_variableName, GetStringVariableValue(_variableName));
            }
            else if (typeof(T).Equals(typeof(bool)))
            {
                return new Field<bool>(_variableName, GetBoolVariableValue(_variableName));
            }
            else if (typeof(T).Equals(typeof(int)))
            {
                return new Field<int>(_variableName, GetIntVariableValue(_variableName));
            }
            else if (typeof(T).Equals(typeof(float)))
            {
                return new Field<float>(_variableName, GetFloatVariableValue(_variableName));
            }
            else if (typeof(T).Equals(typeof(double)))
            {
                return new Field<double>(_variableName, GetDoubleVariableValue(_variableName));
            }
            else if (typeof(T).Equals(typeof(UnityEngine.GameObject)))
            {
                return new Field<UnityEngine.GameObject>(_variableName, GetGameObjectariableValue(_variableName));
            }
            else if (typeof(T).Equals(typeof(UnityEngine.Object)))
            {
                return new Field<UnityEngine.Object>(_variableName, GetUnityObjectariableValue(_variableName));
            }
            else
            {
                Debug.LogError("Unknown or not accepted variable type: "+typeof(T)+" in variable with name " + _variableName );
            }

            return null;
        }
        
        public FieldData GetVariableValueStrType (string _variableName, string _type)
        {
            if (string.IsNullOrEmpty(_variableName))
            {
                Debug.LogError("VARIABLE NAME IS NULL");
                return null;
            }

            if (_type.Equals("string") || _type.Equals("String"))
            {                               
                return new Field<string>(_variableName, GetStringVariableValue(_variableName));
            }
            else if (_type.Equals("bool") || _type.Equals("Bool"))
            {
                return new Field<bool>(_variableName, GetBoolVariableValue(_variableName));
            }
            else if (_type.Equals("int") || _type.Equals("Int"))
            {
                return new Field<int>(_variableName, GetIntVariableValue(_variableName));
            }
            else if (_type.Equals("float") || _type.Equals("Float"))
            {
                return new Field<float>(_variableName, GetFloatVariableValue(_variableName));
            }
            else if (_type.Equals("double") || _type.Equals("Double"))
            {
                return new Field<double>(_variableName, GetDoubleVariableValue(_variableName));
            }
            else if (_type.Equals("GameObject") || _type.Equals("gameobject"))
            {
                return new Field<UnityEngine.GameObject>(_variableName, GetGameObjectariableValue(_variableName));
            }
            else if(_type.Equals("unityobject") || _type.Equals("UnityObject"))
            {
                return new Field<UnityEngine.Object>(_variableName, GetUnityObjectariableValue(_variableName));
            }
            else
            {
                Debug.LogError("Unknown or not accepted variable type: "+_type+" in variable with name " + _variableName );
            }

            return null;
        }
        
        public int GetBoolVariableCount()
        {
            return m_boolVariables.Count;
        }

        public int GetIntVariableCount()
        {
            return m_intVariables.Count;
        }

        public int GetDoubleVariableCount()
        {
            return m_doubleVariables.Count;
        }

        public int GetFloatVariableCount()
        {
            return m_floatVariables.Count;
        }

        public int GetStringVariableCount()
        {
            return m_stringVariables.Count;
        }

        public int GetGameObjectVariableCount()
        {
            return m_gameObjectVariables.Count;
        }

        public int GetUnityObjectVariableCount()
        {
            return m_unityObjectVariables.Count;
        }

        #region Bool

        public bool CheckBoolVariable(string _varName, bool _compareValue, VariableComparison _varComparison)
        {
            return (!string.IsNullOrEmpty(_varName) && (m_boolVariables.ContainsKey(_varName)) && m_boolVariables[_varName] == _compareValue);
        }

        public bool GetBoolVariableValue(string _variableName)
        {
            return (m_boolVariables.ContainsKey(_variableName)) ? m_boolVariables[_variableName] : false;
        }

        public void ClearBoolVariables()
        {
            m_boolVariables.Clear();
        }

        public void AddBoolVariable(string _variableName, bool _value)
        {
            if (!m_boolVariables.ContainsKey(_variableName))
            {
                m_boolVariables.Add(_variableName, _value);
            }
            else
            {
                m_boolVariables[_variableName] = _value;
            }
        }

        public void ReplaceBoolVariable(string _variableName, bool _value)
        {
            if (m_boolVariables.ContainsKey(_variableName))
            {
                m_boolVariables[_variableName] = _value;
            }
            else
            {
                m_boolVariables.Add(_variableName, _value);
            }
        }

        public void DeleteBoolVariable(string _variableName)
        {
            if (m_boolVariables.ContainsKey(_variableName))
            {
                m_boolVariables.Remove(_variableName);
            }
        }
        #endregion

        #region Int

        public bool CheckIntVariable(string _varName, int _compareValue, VariableComparison _varComparison)
        {
            if (!string.IsNullOrEmpty(_varName) && m_intVariables.ContainsKey(_varName))
            {
                switch (_varComparison)
                {
                    case VariableComparison.Equal:
                        return m_intVariables[_varName] == _compareValue;
                    case VariableComparison.Mayor:
                        return m_intVariables[_varName] > _compareValue; 
                    case VariableComparison.MayorEqual:
                        return m_intVariables[_varName] >= _compareValue;
                    case VariableComparison.Minor:
                        return m_intVariables[_varName] < _compareValue;
                    case VariableComparison.MinorEqual:
                        return m_intVariables[_varName] <= _compareValue;
                    default:
                        return false;
                }
            }

            return false;
        }

        public int GetIntVariableValue(string _variableName)
        {
            return (m_intVariables.ContainsKey(_variableName)) ? m_intVariables[_variableName] : 0;
        }

        public void ClearIntVariables()
        {
            m_intVariables.Clear();
        }

        public void AddIntVariable(string _variableName, int _value)
        {
            if (!m_intVariables.ContainsKey(_variableName))
            {
                m_intVariables.Add(_variableName, _value);
            }
            else
            {
                m_intVariables[_variableName] = _value;
            }
        }

        public void ReplaceIntVariable(string _variableName, int _value)
        {
            if (m_intVariables.ContainsKey(_variableName))
            {
                m_intVariables[_variableName] = _value;
            }
            else
            {
                m_intVariables.Add(_variableName, _value);
            }
        }

        public void DeleteIntVariable(string _variableName)
        {
            if (m_intVariables.ContainsKey(_variableName))
            {
                m_intVariables.Remove(_variableName);
            }
        }
        #endregion

        #region Float
        public bool CheckFloatVariable(string _varName, float _compareValue, VariableComparison _varComparison)
        {
            if (!string.IsNullOrEmpty(_varName) && m_floatVariables.ContainsKey(_varName))
            {
                switch (_varComparison)
                {
                    case VariableComparison.Equal:
                        return m_floatVariables[_varName] == _compareValue;
                    case VariableComparison.Mayor:
                        return m_floatVariables[_varName] > _compareValue;
                    case VariableComparison.MayorEqual:
                        return m_floatVariables[_varName] >= _compareValue;
                    case VariableComparison.Minor:
                        return m_floatVariables[_varName] < _compareValue;
                    case VariableComparison.MinorEqual:
                        return m_floatVariables[_varName] <= _compareValue;
                    default:
                        return false;
                }
            }

            return false;
        }

        public float GetFloatVariableValue(string _variableName)
        {
            return (m_floatVariables.ContainsKey(_variableName)) ? m_floatVariables[_variableName] : 0f;
        }

        public void ClearFloatVariables()
        {
            m_floatVariables.Clear();
        }

        public void AddFloatVariable(string _variableName, float _value)
        {
            if (!m_floatVariables.ContainsKey(_variableName))
            {
                m_floatVariables.Add(_variableName, _value);
            }
            else
            {
                m_floatVariables[_variableName] = _value;
            }
        }

        public void ReplaceFloatVariable(string _variableName, float _value)
        {
            if (m_floatVariables.ContainsKey(_variableName))
            {
                m_floatVariables[_variableName] = _value;
            }
            else
            {
                m_floatVariables.Add(_variableName, _value);
            }
        }

        public void DeleteFloatVariable(string _variableName)
        {
            if (m_floatVariables.ContainsKey(_variableName))
            {
                m_floatVariables.Remove(_variableName);
            }
        }
        #endregion

        #region Double
        public bool CheckDoubleVariable(string _varName, double _compareValue, VariableComparison _varComparison)
        {
            if (!string.IsNullOrEmpty(_varName) && m_doubleVariables.ContainsKey(_varName))
            {
                switch (_varComparison)
                {
                    case VariableComparison.Equal:
                        return m_doubleVariables[_varName] == _compareValue;
                    case VariableComparison.Mayor:
                        return m_doubleVariables[_varName] > _compareValue;
                    case VariableComparison.MayorEqual:
                        return m_doubleVariables[_varName] >= _compareValue;
                    case VariableComparison.Minor:
                        return m_doubleVariables[_varName] < _compareValue;
                    case VariableComparison.MinorEqual:
                        return m_doubleVariables[_varName] <= _compareValue;
                    default:
                        return false;
                }
            }

            return false;
        }
        public double GetDoubleVariableValue(string _variableName)
        {
            return (m_doubleVariables.ContainsKey(_variableName)) ? m_doubleVariables[_variableName] : 0.0;
        }

        public void ClearDoubleVariables()
        {
            m_doubleVariables.Clear();
        }

        public void AddDoubleVariable(string _variableName, double _value)
        {
            if (!m_doubleVariables.ContainsKey(_variableName))
            {
                m_doubleVariables.Add(_variableName, _value);
            }
            else
            {
                m_doubleVariables[_variableName] = _value;
            }
        }

        public void ReplaceDoubleVariable(string _variableName, double _value)
        {
            if (m_doubleVariables.ContainsKey(_variableName))
            {
                m_doubleVariables[_variableName] = _value;
            }
            else
            {
                m_doubleVariables.Add(_variableName, _value);
            }
        }

        public void DeleteDoubleVariable(string _variableName)
        {
            if (m_doubleVariables.ContainsKey(_variableName))
            {
                m_doubleVariables.Remove(_variableName);
            }
        }
        #endregion

        #region String
        public bool CheckStringVariable(string _varName, string _compareValue, VariableComparison _varComparison)
        {
            if (!string.IsNullOrEmpty(_varName) && m_stringVariables.ContainsKey(_varName))
            {
                return m_stringVariables[_varName] == _compareValue;
            }

            return false;
        }

        public string GetStringVariableValue(string _variableName)
        {
            return (m_stringVariables.ContainsKey(_variableName)) ? m_stringVariables[_variableName] : "NOT FOUND";
        }

        public void ClearStringVariables()
        {
            m_stringVariables.Clear();
        }

        public void AddStringVariable(string _variableName, string _value)
        {
            if (!m_stringVariables.ContainsKey(_variableName))
            {
                m_stringVariables.Add(_variableName, _value);
            }
            else
            {
                m_stringVariables[_variableName] = _value;
            }
        }

        public void ReplaceStringVariable(string _variableName, string _value)
        {
            if (m_stringVariables.ContainsKey(_variableName))
            {
                m_stringVariables[_variableName] = _value;
            }
            else
            {
                m_stringVariables.Add(_variableName, _value);
            }
        }

        public void DeleteStringVariable(string _variableName)
        {
            if (m_stringVariables.ContainsKey(_variableName))
            {
                m_stringVariables.Remove(_variableName);
            }
        }
        #endregion

        #region GameObject
        public bool CheckGameObjectVariable(string _varName, GameObject _compareValue, VariableComparison _varComparison)
        {
            if (!string.IsNullOrEmpty(_varName) && m_gameObjectVariables.ContainsKey(_varName))
            {
                return m_gameObjectVariables[_varName] == _compareValue;
            }

            return false;
        }
        public GameObject GetGameObjectariableValue(string _variableName)
        {
            return (m_gameObjectVariables.ContainsKey(_variableName)) ? m_gameObjectVariables[_variableName] : null;
        }

        public void ClearGameObjectVariables()
        {
            m_gameObjectVariables.Clear();
        }

        public void AddGameObjectVariable(string _variableName, GameObject _value)
        {
            if (!m_gameObjectVariables.ContainsKey(_variableName))
            {
                m_gameObjectVariables.Add(_variableName, _value);
            }
            else
            {
                m_gameObjectVariables[_variableName] = _value;
            }
        }

        public void ReplaceGameObjectVariable(string _variableName, GameObject _value)
        {
            if (m_gameObjectVariables.ContainsKey(_variableName))
            {
                m_gameObjectVariables[_variableName] = _value;
            }
            else
            {
                m_gameObjectVariables.Add(_variableName, _value);
            }
        }

        public void DeleteGameObjectVariable(string _variableName)
        {
            if (m_gameObjectVariables.ContainsKey(_variableName))
            {
                m_gameObjectVariables.Remove(_variableName);
            }
        }
        #endregion

        #region Unity Object
        public bool CheckUnityObjectVariable(string _varName, Object _compareValue, VariableComparison _varComparison)
        {
            if (!string.IsNullOrEmpty(_varName) && m_unityObjectVariables.ContainsKey(_varName))
            {
                return m_unityObjectVariables[_varName] == _compareValue;
            }

            return false;
        }
        public UnityEngine.Object GetUnityObjectariableValue(string _variableName)
        {
            return (m_unityObjectVariables.ContainsKey(_variableName)) ? m_gameObjectVariables[_variableName] : null;
        }

        public void ClearUnityObjectVariables()
        {
            m_unityObjectVariables.Clear();
        }

        public void AddUnityObjectVariable(string _variableName, UnityEngine.Object _value)
        {
            if (!m_unityObjectVariables.ContainsKey(_variableName))
            {
                m_unityObjectVariables.Add(_variableName, _value);
            }
            else
            {
                m_unityObjectVariables[_variableName] = _value;
            }
        }

        public void ReplaceUnityObjectVariable(string _variableName, UnityEngine.Object _value)
        {
            if (m_unityObjectVariables.ContainsKey(_variableName))
            {
                m_unityObjectVariables[_variableName] = _value;
            }
            else
            {
                m_unityObjectVariables.Add(_variableName, _value);
            }
        }

        public void DeleteUnityObjectVariable(string _variableName)
        {
            if (m_unityObjectVariables.ContainsKey(_variableName))
            {
                m_unityObjectVariables.Remove(_variableName);
            }
        }
        #endregion
    }

}
