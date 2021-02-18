using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace VirtualPhenix.Dialog
{
    public class VP_VariableEditorWindow : EditorWindow
    {
        public static VP_DialogGraph m_currentGraph;

        static VP_VariableDataBase m_variableDatabase = null;
        static string m_currentVariableGraphName = "Variable DB";

        static VP_BoolVariables m_boolVariables;
        static VP_IntVariables m_intVariables;
        static VP_FloatVariables m_floatVariables;
        static VP_DoubleVariables m_doubleVariables;
        static VP_StringVariables m_stringVariables;
        static VP_GameObjectVariables m_gameObjectVariables;
        static VP_UnityObjectVariables m_unityObjectVariables;

        int m_selGridInt = 0;

        string[] m_tabs = 
        { 
            "Bool", 
            "Int", 
            "Float",
            "Double",
            "String",
            "GameObject",
            "Unity Object"
        };

        string m_variableName = "Var";
        bool boolvalue;
        int intvalue;
        float floatvalue;
        double doublevalue;
        string stringvalue = "";
        GameObject gameobjectvalue;
        UnityEngine.Object unityobjectvalue;
        Vector2 scroll;

        private void OnEnable()
        {
            if (m_currentGraph != null && m_variableDatabase == null)
            {
                InitWindowWithGraph(m_currentGraph);
            }
        }

        public static void InitWindowWithGraph(VP_DialogGraph _graph)
        {
            if (_graph != null)
            {
                m_variableDatabase = _graph.GraphVariables;
                m_currentVariableGraphName = _graph.name;
                m_currentGraph = _graph;
                RefreshVariables();

                 // Get existing open window or if none, make a new one:
                 VP_VariableEditorWindow window = GetWindow<VP_VariableEditorWindow>("Variable Editor");
                window.Show();
            }            
        }

        static void RefreshVariables()
        {
            if (m_currentGraph != null && m_variableDatabase == null)
            {
                m_variableDatabase = m_currentGraph.GraphVariables;
                m_currentVariableGraphName = m_currentGraph.name;
            }

            m_boolVariables = m_variableDatabase.GetBoolVariableList;
            m_intVariables = m_variableDatabase.GetIntVariableList;
            m_floatVariables = m_variableDatabase.GetFloatVariableList;
            m_doubleVariables = m_variableDatabase.GetDoubleVariableList;
            m_stringVariables = m_variableDatabase.GetStringVariableList;
            m_gameObjectVariables = m_variableDatabase.GetGameObjectVariableList;
            m_unityObjectVariables = m_variableDatabase.GetUnityObjectVariableList;
        }

        void AddValue<T>(T value)
        {
            if (string.IsNullOrEmpty(m_variableName))
            {
                Debug.LogError("Can't add blank space variables");
                return;
            }

            if (typeof(T).Equals(typeof(string)))
            {
                string _val = (string)System.Convert.ChangeType(value, typeof(string));
                if (m_stringVariables.ContainsKey(m_variableName))
                {
                    m_stringVariables[m_variableName] = _val;
                }
                else
                {
                    m_stringVariables.Add(m_variableName, _val);
                }
            }
            else if (typeof(T).Equals(typeof(int)))
            {
                int _val = (int)System.Convert.ChangeType(value, typeof(int));
                if (m_intVariables.ContainsKey(m_variableName))
                {
                    m_intVariables[m_variableName] = _val;
                }
                else
                {
                    m_intVariables.Add(m_variableName, _val);
                }
            }
            else if (typeof(T).Equals(typeof(float)))
            {
                float _val = (float)System.Convert.ChangeType(value, typeof(float));
                if (m_floatVariables.ContainsKey(m_variableName))
                {
                    m_floatVariables[m_variableName] = _val;
                }
                else
                {
                    m_floatVariables.Add(m_variableName, _val);
                }
            }
            else if (typeof(T).Equals(typeof(double)))
            {
                double _val = (double)System.Convert.ChangeType(value, typeof(double));
                if (m_doubleVariables.ContainsKey(m_variableName))
                {
                    m_doubleVariables[m_variableName] = _val;
                }
                else
                {
                    m_doubleVariables.Add(m_variableName, _val);
                }
            }
            else if (typeof(T).Equals(typeof(GameObject)))
            {
                GameObject _val = (GameObject)System.Convert.ChangeType(value, typeof(GameObject));
                if (m_gameObjectVariables.ContainsKey(m_variableName))
                {
                    m_gameObjectVariables[m_variableName] = _val;
                }
                else
                {
                    m_gameObjectVariables.Add(m_variableName, _val);
                }
            }
            else if (typeof(T).Equals(typeof(UnityEngine.Object)))
            {
                UnityEngine.Object _val = (UnityEngine.Object)System.Convert.ChangeType(value, typeof(UnityEngine.Object));
                if (m_unityObjectVariables.ContainsKey(m_variableName))
                {
                    m_unityObjectVariables[m_variableName] = _val;
                }
                else
                {
                    m_unityObjectVariables.Add(m_variableName, _val);
                }
            }
            else if (typeof(T).Equals(typeof(bool)))
            {
                bool _val = (bool)System.Convert.ChangeType(value, typeof(bool));
                if (m_boolVariables.ContainsKey(m_variableName))
                {
                    m_boolVariables[m_variableName] = _val;
                }
                else
                {
                    m_boolVariables.Add(m_variableName, _val);
                }
            }
        }

        void RemoveValue<T>(T value)
        {
            if (string.IsNullOrEmpty(m_variableName))
            {
                Debug.LogError("Can't add blank space variables");
                return;
            }

            if (typeof(T).Equals(typeof(string)))
            {
                if (m_stringVariables.ContainsKey(m_variableName))
                {
                    m_stringVariables.Remove(m_variableName);
                }
            }
            else if (typeof(T).Equals(typeof(int)))
            {
                if (m_intVariables.ContainsKey(m_variableName))
                {
                    m_intVariables.Remove(m_variableName);
                }
            }
            else if (typeof(T).Equals(typeof(float)))
            {
                if (m_floatVariables.ContainsKey(m_variableName))
                {
                    m_floatVariables.Remove(m_variableName);
                }
            }
            else if (typeof(T).Equals(typeof(double)))
            {
                if (m_doubleVariables.ContainsKey(m_variableName))
                {
                    m_doubleVariables.Remove(m_variableName);
                }
            }
            else if (typeof(T).Equals(typeof(GameObject)))
            {
                if (m_gameObjectVariables.ContainsKey(m_variableName))
                {
                    m_gameObjectVariables.Remove(m_variableName);
                }
            }
            else if (typeof(T).Equals(typeof(UnityEngine.Object)))
            {
                if (m_unityObjectVariables.ContainsKey(m_variableName))
                {
                    m_unityObjectVariables.Remove(m_variableName);
                }
            }
            else if (typeof(T).Equals(typeof(bool)))
            {
                if (m_boolVariables.ContainsKey(m_variableName))
                {
                    m_boolVariables.Remove(m_variableName);
                }
            }
        }

        void RemoveVariable<T>(T value, string _varName)
        {
            if (string.IsNullOrEmpty(_varName))
            {
                Debug.LogError("Can't add blank space variables");
                return;
            }

            if (typeof(T).Equals(typeof(string)))
            {
                if (m_stringVariables.ContainsKey(_varName))
                {
                    m_stringVariables.Remove(_varName);
                }
            }
            else if (typeof(T).Equals(typeof(int)))
            {
                if (m_intVariables.ContainsKey(_varName))
                {
                    m_intVariables.Remove(_varName);
                }
            }
            else if (typeof(T).Equals(typeof(float)))
            {
                if (m_floatVariables.ContainsKey(_varName))
                {
                    m_floatVariables.Remove(_varName);
                }
            }
            else if (typeof(T).Equals(typeof(double)))
            {
                if (m_doubleVariables.ContainsKey(_varName))
                {
                    m_doubleVariables.Remove(_varName);
                }
            }
            else if (typeof(T).Equals(typeof(GameObject)))
            {
                if (m_gameObjectVariables.ContainsKey(_varName))
                {
                    m_gameObjectVariables.Remove(_varName);
                }
            }
            else if (typeof(T).Equals(typeof(UnityEngine.Object)))
            {
                if (m_unityObjectVariables.ContainsKey(_varName))
                {
                    m_unityObjectVariables.Remove(_varName);
                }
            }
            else if (typeof(T).Equals(typeof(bool)))
            {
                if (m_boolVariables.ContainsKey(_varName))
                {
                    m_boolVariables.Remove(_varName);
                }
            }
        }

        protected virtual void OnGUI()
        {
            if (m_variableDatabase == null)
            {
                return;
            }
            GUILayout.Label("Variable Editor of "+m_currentVariableGraphName+" graph", EditorStyles.boldLabel);
            GUILayout.Space(20);

            scroll = GUILayout.BeginScrollView(scroll, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            GUILayout.BeginHorizontal("Box");
            m_selGridInt = GUILayout.SelectionGrid(m_selGridInt, m_tabs, m_tabs.Length);
            GUILayout.EndHorizontal();
            GUILayout.Space(20);
            
            switch (m_selGridInt)
            {
                case 0: // Bool
                    GUILayout.BeginHorizontal();
                    m_variableName = GUILayout.TextField(m_variableName, 20);
                    boolvalue = GUILayout.Toggle(boolvalue, "New Bool Value");
                    GUILayout.EndHorizontal();
                    GUILayout.Space(20);
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Set Variable"))
                    {
                        AddValue(boolvalue);
                    }
                    if (GUILayout.Button("Remove Variable"))
                    {
                        RemoveValue(boolvalue);
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.Space(20);
                    GUILayout.Label("Bool variables " + m_boolVariables.Count, EditorStyles.boldLabel);
                    List<string> boolvar = new List<string>(m_boolVariables.Keys);

                    foreach (string variableName in boolvar)
                    {
                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button("-", GUILayout.Width(25)))
                        {
                            RemoveVariable(true, variableName);
                        }
                        else
                        {
                            GUILayout.Space(20);
                            m_boolVariables[variableName] = GUILayout.Toggle(m_boolVariables[variableName], variableName);
                        }

                        GUILayout.EndHorizontal();
                     
                    }

                    break;
                case 1:
                    GUILayout.BeginHorizontal();
                    m_variableName = GUILayout.TextField(m_variableName, 20);
                    intvalue = EditorGUILayout.IntField("New Int Value", intvalue);
                    GUILayout.EndHorizontal();
                    GUILayout.Space(20);
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Set Variable"))
                    {
                        AddValue(intvalue);
                    }
                    if (GUILayout.Button("Remove Variable"))
                    {
                        RemoveValue(intvalue);
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.Space(20);
                    GUILayout.Label("Int variables " + m_intVariables.Count, EditorStyles.boldLabel);
                    List<string> intvar = new List<string>(m_intVariables.Keys);

                    foreach (string variableName in intvar)
                    {
                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button("-", GUILayout.Width(25)))
                        {
                            RemoveVariable(1, variableName);
                        }
                        else
                        {
                            GUILayout.Space(20);
                            m_intVariables[variableName] = EditorGUILayout.IntField(variableName, m_intVariables[variableName]);
                        }

                        GUILayout.EndHorizontal();
                        
                    }

                    break;
                case 2:
                    GUILayout.BeginHorizontal();
                    m_variableName = GUILayout.TextField(m_variableName, 20);
                    floatvalue = EditorGUILayout.FloatField("New Float Value", floatvalue);
                    GUILayout.EndHorizontal();
                    GUILayout.Space(20);
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Set Variable"))
                    {
                        AddValue(floatvalue);
                    }
                    if (GUILayout.Button("Remove Variable"))
                    {
                        RemoveValue(floatvalue);
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.Space(20);
                    GUILayout.Label("Float variables " + m_floatVariables.Count, EditorStyles.boldLabel);
                    List<string> floatvar = new List<string>(m_floatVariables.Keys);

                    foreach (string variableName in floatvar)
                    {
                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button("-", GUILayout.Width(25)))
                        {
                            RemoveVariable(0f, variableName);
                        }
                        else
                        {
                            GUILayout.Space(20);
                            m_floatVariables[variableName] = EditorGUILayout.FloatField(variableName, m_floatVariables[variableName]);
                        }

                        GUILayout.EndHorizontal();
                      
                    }
                    break;
                case 3:
                    GUILayout.BeginHorizontal();
                    m_variableName = GUILayout.TextField(m_variableName, 20);
                    doublevalue = EditorGUILayout.DoubleField("New Double Value", doublevalue);
                    GUILayout.EndHorizontal();
                    GUILayout.Space(20);
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Set Variable"))
                    {
                        AddValue(doublevalue);
                    }
                    if (GUILayout.Button("Remove Variable"))
                    {
                        RemoveValue(doublevalue);
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.Space(20);
                    GUILayout.Label("Double variables " + m_doubleVariables.Count, EditorStyles.boldLabel);
                    List<string> doublevar = new List<string>(m_doubleVariables.Keys);

                    foreach (string variableName in doublevar)
                    {
                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button("-", GUILayout.Width(25)))
                        {
                            RemoveVariable(0.0, variableName);
                        }
                        else
                        {
                            GUILayout.Space(20);
                            m_doubleVariables[variableName] = EditorGUILayout.DoubleField(variableName, m_doubleVariables[variableName]);
                        }

                        GUILayout.EndHorizontal();
                       
                    }

                    break;
                case 4:
                    GUILayout.BeginHorizontal();
                    m_variableName = GUILayout.TextField(m_variableName, 20);
                    stringvalue = GUILayout.TextField(stringvalue, 20);
                    GUILayout.EndHorizontal();
                    GUILayout.Space(20);
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Set Variable"))
                    {
                        AddValue(stringvalue);
                    }
                    if (GUILayout.Button("Remove Variable"))
                    {
          
                        RemoveValue(stringvalue);
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.Space(20);
                    GUILayout.Label("String variables " + m_stringVariables.Count, EditorStyles.boldLabel);
                    List<string> strvar = new List<string>(m_stringVariables.Keys);

                    foreach (string variableName in strvar)
                    {
                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button("-", GUILayout.Width(25)))
                        {
                            RemoveVariable(true, variableName);
                        }
                        else
                        {
                            GUILayout.Space(20);
                            m_stringVariables[variableName] = EditorGUILayout.TextField(variableName, m_stringVariables[variableName]);
                        }

                        GUILayout.EndHorizontal();
                    }
                    break;
                case 5:
                    GUILayout.BeginHorizontal();
                    m_variableName = GUILayout.TextField(m_variableName, 20);
                    gameobjectvalue = (GameObject)EditorGUILayout.ObjectField("New Game Object Value", gameobjectvalue, typeof(GameObject), true);
                    GUILayout.EndHorizontal();
                    GUILayout.Space(20);
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Set Variable"))
                    {
                        AddValue(gameobjectvalue);
                    }
                    if (GUILayout.Button("Remove Variable"))
                    {
                        RemoveValue(gameobjectvalue);
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.Space(20);
                    GUILayout.Label("GameObject variables " + m_gameObjectVariables.Count, EditorStyles.boldLabel);
                    List<string> govar = new List<string>(m_gameObjectVariables.Keys);

                    foreach (string variableName in govar)
                    {
                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button("-", GUILayout.Width(25)))
                        {
                            RemoveVariable(gameobjectvalue, variableName);
                        }
                        else
                        {
                            GUILayout.Space(20);
                            m_gameObjectVariables[variableName] = (GameObject)EditorGUILayout.ObjectField(variableName, m_gameObjectVariables[variableName], typeof(GameObject), true);
                        }

                        GUILayout.EndHorizontal();
                    }
                    break;
                default:
                    GUILayout.BeginHorizontal();
                    m_variableName = GUILayout.TextField(m_variableName, 20);
                    unityobjectvalue = (UnityEngine.Object)EditorGUILayout.ObjectField("New Unity Object Value", unityobjectvalue, typeof(UnityEngine.Object), true);
                    GUILayout.EndHorizontal();
                    GUILayout.Space(20);
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Set Variable"))
                    {
                        AddValue(unityobjectvalue);
                    }
                    if (GUILayout.Button("Remove Variable"))
                    {
                        RemoveValue(unityobjectvalue);
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.Space(20);
                    GUILayout.Label("Unity Object variables " + m_unityObjectVariables.Count, EditorStyles.boldLabel);
                    List<string> ugovar = new List<string>(m_unityObjectVariables.Keys);

                    foreach (string variableName in ugovar)
                    {
                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button("-", GUILayout.Width(25)))
                        {
                            RemoveVariable(unityobjectvalue, variableName);
                        }
                        else
                        {
                            GUILayout.Space(20);
                            m_unityObjectVariables[variableName] = (UnityEngine.Object)EditorGUILayout.ObjectField(variableName, m_unityObjectVariables[variableName], typeof(UnityEngine.Object), true);
                        }

                        GUILayout.EndHorizontal();
                    }
                    break;

            }

            GUILayout.EndScrollView();
        }
    }
}
