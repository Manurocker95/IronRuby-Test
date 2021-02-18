using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace VirtualPhenix.Dialog
{
    public enum VariableScope
    {
        Global,
        Graph
    }

    [NodeTint(VP_DialogSetup.NodeColors.SET_VARIABLE), CreateNodeMenuAttribute("Set Variable")]
    public class VP_SetVariableNode : VP_DialogBaseNode
    {
        [SerializeField] private SetVariableList m_variables;
        [SerializeField] private bool m_saveAsPlayerPrefs = false;

        // Start is called before the first frame update
        protected override void OnEnable()
        {
            base.OnEnable();
            if (m_variables == null)
            {
                m_variables = new SetVariableList();
            }
        }

        void AddToDialogueManager(VP_VariableDataBase m_globalDatabase)
        {
            VP_BoolVariables boolvars = m_globalDatabase.GetBoolVariableList;

            foreach (string v in boolvars.Keys)
            {
                VP_DialogManager.Instance.SetVariable(v, boolvars[v]);

                if (m_saveAsPlayerPrefs)
                {
                    PlayerPrefs.SetInt(v + "_bool", (boolvars[v]) ? 1 : 0);
                }
            }

            VP_IntVariables intvars = m_globalDatabase.GetIntVariableList;

            foreach (string v in intvars.Keys)
            {
                VP_DialogManager.Instance.SetVariable(v, intvars[v]);

                if (m_saveAsPlayerPrefs)
                {
                    PlayerPrefs.SetInt(v + "_int", (intvars[v]));
                }
            }

            VP_FloatVariables floatvars = m_globalDatabase.GetFloatVariableList;

            foreach (string v in floatvars.Keys)
            {
                VP_DialogManager.Instance.SetVariable(v, floatvars[v]);

                if (m_saveAsPlayerPrefs)
                {
                    PlayerPrefs.SetFloat(v + "_float", (floatvars[v]));
                }
            }

            VP_DoubleVariables doublevars = m_globalDatabase.GetDoubleVariableList;

            foreach (string v in doublevars.Keys)
            {
                VP_DialogManager.Instance.SetVariable(v, doublevars[v]);

                if (m_saveAsPlayerPrefs)
                {
                    PlayerPrefs.SetFloat(v + "_double", ((float)doublevars[v]));
                }
            }

            VP_StringVariables stringvars = m_globalDatabase.GetStringVariableList;

            foreach (string v in stringvars.Keys)
            {
                VP_DialogManager.Instance.SetVariable(v, stringvars[v]);

                if (m_saveAsPlayerPrefs)
                {
                    PlayerPrefs.SetString(v + "_string", (stringvars[v]));
                }
            }

            VP_GameObjectVariables govars = m_globalDatabase.GetGameObjectVariableList;

            foreach (string v in govars.Keys)
            {
                VP_DialogManager.Instance.SetVariable(v, govars[v]);
            }

            VP_UnityObjectVariables ugovars = m_globalDatabase.GetUnityObjectVariableList;

            foreach (string v in ugovars.Keys)
            {
                VP_DialogManager.Instance.SetVariable(v, ugovars[v]);
            }
        }

        void AddToGraph(VP_VariableDataBase _database)
        {
            VP_VariableDataBase graphDb = (graph as VP_DialogGraph).GraphVariables;

            VP_BoolVariables boolvars = _database.GetBoolVariableList;

            foreach (string v in boolvars.Keys)
            {
                graphDb.AddBoolVariable(v, boolvars[v]);
                if (m_saveAsPlayerPrefs)
                {
                    PlayerPrefs.SetInt(v+"_bool", (boolvars[v]) ? 1 : 0);
                }
            }

            VP_IntVariables intvars = _database.GetIntVariableList;

            foreach (string v in intvars.Keys)
            {
                graphDb.AddIntVariable(v, intvars[v]);

                if (m_saveAsPlayerPrefs)
                {
                    PlayerPrefs.SetInt(v + "_int", (intvars[v]));
                }
            }

            VP_FloatVariables floatvars = _database.GetFloatVariableList;

            foreach (string v in floatvars.Keys)
            {
                graphDb.AddFloatVariable(v, floatvars[v]);

                if (m_saveAsPlayerPrefs)
                {
                    PlayerPrefs.SetFloat(v + "_float", (floatvars[v]));
                }
            }

            VP_DoubleVariables doublevars = _database.GetDoubleVariableList;

            foreach (string v in doublevars.Keys)
            {
                graphDb.AddDoubleVariable(v, doublevars[v]);

                if (m_saveAsPlayerPrefs)
                {
                    PlayerPrefs.SetFloat(v + "_double", ((float)doublevars[v]));
                }
            }

            VP_StringVariables stringvars = _database.GetStringVariableList;

            foreach (string v in stringvars.Keys)
            {
                graphDb.AddStringVariable(v, stringvars[v]);

                if (m_saveAsPlayerPrefs)
                {
                    PlayerPrefs.SetString(v + "_string", (stringvars[v]));
                }
            }

            VP_GameObjectVariables govars = _database.GetGameObjectVariableList;

            foreach (string v in govars.Keys)
            {
                graphDb.AddGameObjectVariable(v, govars[v]);
            }

            VP_UnityObjectVariables ugovars = _database.GetUnityObjectVariableList;

            foreach (string v in ugovars.Keys)
            {
                graphDb.AddUnityObjectVariable(v, ugovars[v]);
            }
        }

        public override void Trigger()
        {
            foreach (VariableScope scope in m_variables.Keys)
            {
                switch (scope)
                {
                    case VariableScope.Global:
                        AddToDialogueManager(m_variables[scope]);
                        break;
                    case VariableScope.Graph:
                        AddToGraph(m_variables[scope]);
                        break;
                }
            }

            NodePort port = GetOutputPort(VP_DialogSetup.Fields.DIALOG_NODE_OUTPUT);
            VP_DialogManager.OnDialogCompleteAction();

            if (port.ConnectionCount == 0)
            {
                CheckEndSequenceNode();
                return;
            }

            for (int i = 0; i < port.ConnectionCount; i++)
            {
                NodePort connection = port.GetConnection(i);
                (connection.node as VP_DialogBaseNode).Trigger();
            }
        }
    }
}
