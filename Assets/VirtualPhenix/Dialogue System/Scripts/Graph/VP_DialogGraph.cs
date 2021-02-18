using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XNode;

namespace VirtualPhenix.Dialog
{
    [CreateAssetMenu(fileName = "New Dialog Graph", menuName = "Virtual Phenix/Dialogue System/Dialog Graph", order = 0)]
    public class VP_DialogGraph : NodeGraph
    {
        private VP_DialogBaseNode m_current;
        [HideInInspector] public VP_DialogChart m_chart;

        [Header("Variables")]
        [SerializeField] private VP_DialogGraphPreset m_dialogPreset = null;

        private List<VP_DialogLog> m_saidDialogs;
        [Header("Variables")]
        [SerializeField] private VP_VariableDataBase m_graphVariables;

        public VP_DialogBaseNode CurrentNode { get { return m_current; } }
        public VP_VariableDataBase GraphVariables { get { return m_graphVariables; } }
        public VP_DialogGraphPreset Preset { get { return m_dialogPreset; } }

        public void Awake()
        {
            this.windowName = VP_DialogSetup.WINDOW_NAME;
            m_saidDialogs = new List<VP_DialogLog>();

            if (m_graphVariables == null)
            {
                m_graphVariables = new VP_VariableDataBase();
            }
        }

        private void OnEnable()
        {
            if (Application.isPlaying)
                StartAllListeners();
        }

        private void OnDisable()
        {
            if (Application.isPlaying)
                StopAllListeners();
        }

        void StartAllListeners()
        {
            var dm = VP_DialogManager.nullableInstance;

            if (dm != null)
                VP_DialogManager.StartListeningToOnDialogEnd(DeleteAllRegisteredDialogs);
        }

        void StopAllListeners()
        {
            var dm = VP_DialogManager.nullableInstance;

            if (dm != null)
                VP_DialogManager.StopListeningToOnDialogEnd(DeleteAllRegisteredDialogs);
        }

        #region Graph Variables

        public FieldData GetVariableValue<T>(string _variableName, T _type)
        {
            return VP_Utils.DialogUtils.GetVariableValueFromDatabase(_variableName, _type, m_graphVariables);
        }
        public FieldData GetVariableFromStringType(string _varName, string _type)
        {
            return VP_Utils.DialogUtils.GetVariableValueStrTypeFromDatabase(_varName, _type, m_graphVariables);
        }
        public string GetVariableStringValue<T>(string _varName, T _type)
        {
            return VP_Utils.DialogUtils.GetVariableValueStringFromDatabase(_varName, _type, m_graphVariables);
        }
        public string GetVariableStringValueFromStringType(string _varName, string _type)
        {
            return VP_Utils.DialogUtils.GetVariableValueStringFromStrTypeFromDatabase(_varName, _type, m_graphVariables);
        }
        /// <summary>
        /// Set a custom generic variable, a value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_name"></param>
        /// <param name="value"></param>
        public void SetVariable<T>(string _name, T value)
        {
            VP_Utils.DialogUtils.SetVariableToDatabase(_name, value, m_graphVariables);
        }
        /// <summary>
        /// Todo graph
        /// </summary>
        /// <param name="_variableName"></param>
        /// <returns></returns>

        public bool GetBoolVariableValue(string _variableName)
        {
            return m_graphVariables.GetBoolVariableValue(_variableName);
        }

        public void ClearBoolVariables()
        {
            m_graphVariables.ClearBoolVariables();
        }

        public void AddBoolVariable(string _variableName, bool _value)
        {
            m_graphVariables.AddBoolVariable(_variableName, _value);
        }

        public void ReplaceBoolVariable(string _variableName, bool _value)
        {
            m_graphVariables.ReplaceBoolVariable(_variableName, _value);
        }

        public void DeleteBoolVariable(string _variableName)
        {
            m_graphVariables.DeleteBoolVariable(_variableName);
        }

        //------------
        public int GetIntVariableValue(string _variableName)
        {
            return m_graphVariables.GetIntVariableValue(_variableName);
        }

        public void ClearIntVariables()
        {
            m_graphVariables.ClearIntVariables();
        }

        public void AddIntVariable(string _variableName, int _value)
        {
            m_graphVariables.AddIntVariable(_variableName, _value);
        }

        public void ReplaceIntVariable(string _variableName, int _value)
        {
            m_graphVariables.ReplaceIntVariable(_variableName, _value);
        }

        public void DeleteIntVariable(string _variableName)
        {
            m_graphVariables.DeleteIntVariable(_variableName);
        }
        //------------
        public float GetFloatVariableValue(string _variableName)
        {
            return m_graphVariables.GetFloatVariableValue(_variableName);
        }

        public void ClearFloatVariables()
        {
            m_graphVariables.ClearFloatVariables();
        }

        public void AddFloatVariable(string _variableName, float _value)
        {
            m_graphVariables.AddFloatVariable(_variableName, _value);
        }

        public void ReplaceFloatVariable(string _variableName, float _value)
        {
            m_graphVariables.ReplaceFloatVariable(_variableName, _value);
        }

        public void DeleteFloatVariable(string _variableName)
        {
            m_graphVariables.DeleteFloatVariable(_variableName);
        }
        //------------
        public double GetDoubleVariableValue(string _variableName)
        {
            return m_graphVariables.GetDoubleVariableValue(_variableName);
        }

        public void ClearDoubleVariables()
        {
            m_graphVariables.ClearDoubleVariables();
        }

        public void AddDoubleVariable(string _variableName, double _value)
        {
            m_graphVariables.AddDoubleVariable(_variableName, _value);
        }

        public void ReplaceDoubleVariable(string _variableName, double _value)
        {
            m_graphVariables.ReplaceDoubleVariable(_variableName, _value);
        }

        public void DeleteDoubleVariable(string _variableName)
        {
            m_graphVariables.DeleteDoubleVariable(_variableName);
        }
        //------------
        public string GetStringVariableValue(string _variableName)
        {
            return m_graphVariables.GetStringVariableValue(_variableName);
        }

        public void ClearStringVariables()
        {
            m_graphVariables.ClearStringVariables();
        }

        public void AddStringVariable(string _variableName, string _value)
        {
            m_graphVariables.AddStringVariable(_variableName, _value);
        }

        public void ReplaceStringVariable(string _variableName, string _value)
        {
            m_graphVariables.ReplaceStringVariable(_variableName, _value);
        }

        public void DeleteStringVariable(string _variableName)
        {
            m_graphVariables.DeleteStringVariable(_variableName);
        }
        //------------
        public GameObject GetGameObjectVariableValue(string _variableName)
        {
            return m_graphVariables.GetGameObjectariableValue(_variableName);
        }

        public void ClearGameObjectVariables()
        {
            m_graphVariables.ClearGameObjectVariables();
        }

        public void AddGameObjectVariable(string _variableName, GameObject _value)
        {
            m_graphVariables.AddGameObjectVariable(_variableName, _value);
        }

        public void ReplaceGameObjectVariable(string _variableName, GameObject _value)
        {
            m_graphVariables.ReplaceGameObjectVariable(_variableName, _value);
        }

        public void DeleteGameObjectVariable(string _variableName)
        {
            m_graphVariables.DeleteGameObjectVariable(_variableName);
        }
        //------------
        public UnityEngine.Object GetUnityObjectVariableValue(string _variableName)
        {
            return m_graphVariables.GetUnityObjectariableValue(_variableName);
        }

        public void ClearUnityObjectVariables()
        {
            m_graphVariables.ClearUnityObjectVariables();
        }

        public void AddUnityObjectVariable(string _variableName, UnityEngine.Object _value)
        {
            m_graphVariables.AddUnityObjectVariable(_variableName, _value);
        }

        public void ReplaceUnityObjectVariable(string _variableName, UnityEngine.Object _value)
        {
            m_graphVariables.ReplaceUnityObjectVariable(_variableName, _value);
        }

        public void DeleteUnityObjectVariable(string _variableName)
        {
            m_graphVariables.DeleteUnityObjectVariable(_variableName);
        }
        #endregion

        /// <summary> Add a node to the graph by type </summary>
        public override Node AddNode(Type type)
        {
            Node node = base.AddNode(type);

            if (node is VP_Dialog && m_dialogPreset != null)
            {
                VP_Dialog dialog = node as VP_Dialog;
                dialog.SetValuesByPreset(m_dialogPreset);
            }

            return node;
        }

        public void Restart()
        {
            this.windowName = VP_DialogSetup.WINDOW_NAME;

            if (m_chart)
                m_current = m_chart.m_currentNode;

            m_current.IsCurrent = true;

        }

        public void SetCurrent(VP_Dialog _newCurrent)
        {
            m_current = _newCurrent;
        }

        public void RefreshNodeInit()
        {
            foreach (Node node in nodes)
            {
                (node as VP_DialogBaseNode).RefreshInit();
            }
        }

        public void Refresh()
        {
            Debug.Log("Refreshing Dialog Graph Data...");

            foreach (Node node in nodes)
            {
                if (node is VP_DialogInitEvent)
                {
                    VP_DialogInitEvent ev = (VP_DialogInitEvent)node;
                    if (ev.GetOutputPort(VP_DialogSetup.Fields.DIALOG_NODE_OUTPUT).node != null)
                    {

                        VP_DialogBaseNode nodeToSend = ev.GetOutputPort(VP_DialogSetup.Fields.DIALOG_NODE_OUTPUT).GetConnection(0).node as VP_DialogBaseNode;
                        RefreshRecursive(nodeToSend, nodeToSend);
                    }
                }
            }
        }

        public void RefreshRecursive(VP_DialogBaseNode node, VP_DialogBaseNode firstNode)
        {

            if (node is VP_Dialog)
            {
                VP_Dialog dialog = node as VP_Dialog;

                if (dialog.answers.Count == 0)
                {
                    if (dialog.GetOutputPort(VP_DialogSetup.Fields.DIALOG_NODE_OUTPUT).node == null)
                    {
                        node.output = dialog.GetOutputPort(VP_DialogSetup.Fields.DIALOG_NODE_OUTPUT).GetConnection(0).node as VP_DialogBaseNode;
                        RefreshRecursive(dialog.GetOutputPort(VP_DialogSetup.Fields.DIALOG_NODE_OUTPUT).GetConnection(0).node as VP_DialogBaseNode, firstNode);
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    for (int _index = 0; _index < dialog.answers.Count; _index++)
                    {
                        if (dialog.answers.Count <= _index)
                        {
                            if (dialog.GetOutputPort(VP_DialogSetup.Fields.DIALOG_NODE_OUTPUT).node == null)
                            {
                                node.output = null;
                                return;
                            }
                            else
                            {
                                node.output = dialog.GetOutputPort(VP_DialogSetup.Fields.DIALOG_NODE_OUTPUT).GetConnection(0).node as VP_DialogBaseNode;
                                RefreshRecursive(dialog.GetOutputPort(VP_DialogSetup.Fields.DIALOG_NODE_OUTPUT).GetConnection(0).node as VP_DialogBaseNode, firstNode);
                            }
                        }
                        else
                        {
                            if (dialog.GetOutputPort(VP_DialogSetup.Fields.DIALOG_NODE_ANSWERS + " " + _index).node != null)
                            {
                                RefreshRecursive(dialog.GetOutputPort(VP_DialogSetup.Fields.DIALOG_NODE_ANSWERS + " " + _index).GetConnection(0).node as VP_DialogBaseNode, firstNode);
                            }
                        }


                    }
                }
            }

            if (node is VP_DialogBranch)
            {
                VP_DialogBranch branch = node as VP_DialogBranch;

                if (branch.GetOutputPort(VP_DialogSetup.Fields.DIALOG_NODE_OUTPUT).node != null)
                {
                    branch.output = branch.GetOutputPort(VP_DialogSetup.Fields.DIALOG_NODE_OUTPUT).GetConnection(0).node as VP_DialogBaseNode;
                    RefreshRecursive(branch.GetOutputPort(VP_DialogSetup.Fields.DIALOG_NODE_OUTPUT).GetConnection(0).node as VP_DialogBaseNode, firstNode);

                }
                else
                {
                    branch.output = null;
                }

                if (branch.GetOutputPort(VP_DialogSetup.Fields.DIALOG_NODE_CONDITION_TRUE).node != null)
                {
                    branch.isTrue = branch.GetOutputPort(VP_DialogSetup.Fields.DIALOG_NODE_CONDITION_TRUE).GetConnection(0).node as VP_DialogBaseNode;
                    RefreshRecursive(branch.GetOutputPort(VP_DialogSetup.Fields.DIALOG_NODE_CONDITION_TRUE).GetConnection(0).node as VP_DialogBaseNode, firstNode);
                }
                else
                {
                    branch.isTrue = null;
                }

                if (branch.GetOutputPort(VP_DialogSetup.Fields.DIALOG_NODE_CONDITION_FALSE).node != null)
                {
                    branch.isFalse = branch.GetOutputPort(VP_DialogSetup.Fields.DIALOG_NODE_CONDITION_FALSE).GetConnection(0).node as VP_DialogBaseNode;
                    RefreshRecursive(branch.GetOutputPort(VP_DialogSetup.Fields.DIALOG_NODE_CONDITION_FALSE).GetConnection(0).node as VP_DialogBaseNode, firstNode);
                }
                else
                {
                    branch.isFalse = null;
                }

            }

            if (node is VP_DialogMultipleOutputs)
            {
                VP_DialogMultipleOutputs sequence = node as VP_DialogMultipleOutputs;

                if (sequence.outputs.Count == 0)
                {
                    if (sequence.GetOutputPort(VP_DialogSetup.Fields.DIALOG_NODE_OUTPUT).node != null)
                    {
                        sequence.output = sequence.GetOutputPort(VP_DialogSetup.Fields.DIALOG_NODE_OUTPUT).GetConnection(0).node as VP_DialogBaseNode;
                        RefreshRecursive(sequence.GetOutputPort(VP_DialogSetup.Fields.DIALOG_NODE_OUTPUT).GetConnection(0).node as VP_DialogBaseNode, firstNode);

                    }
                    else
                    {
                        sequence.output = null;
                    }
                }
                else
                {
                    for (int index = 0; index < sequence.outputs.Count; index++)
                    {
                        if (sequence.outputs.Count <= index)
                        {
                            if (sequence.GetOutputPort(VP_DialogSetup.Fields.DIALOG_NODE_OUTPUT).node != null)
                            {
                                sequence.output = sequence.GetOutputPort(VP_DialogSetup.Fields.DIALOG_NODE_OUTPUT).GetConnection(0).node as VP_DialogBaseNode;
                                RefreshRecursive(sequence.GetOutputPort(VP_DialogSetup.Fields.DIALOG_NODE_OUTPUT).GetConnection(0).node as VP_DialogBaseNode, firstNode);

                            }
                            else
                            {
                                sequence.output = null;
                            }
                        }

                        if (sequence.GetOutputPort(VP_DialogSetup.Fields.DIALOG_NODE_MULTIPLE_OUTPUT + " " + index).node != null)
                        {
                            sequence.outputs[index] = sequence.GetOutputPort(VP_DialogSetup.Fields.DIALOG_NODE_MULTIPLE_OUTPUT + " " + index).GetConnection(0).node as VP_DialogBaseNode;
                            RefreshRecursive(sequence.GetOutputPort(VP_DialogSetup.Fields.DIALOG_NODE_MULTIPLE_OUTPUT + " " + index).GetConnection(0).node as VP_DialogBaseNode, firstNode);
                        }
                        else
                        {
                            sequence.outputs[index] = null;
                        }
                    }

                }
            }
        }

        public void StartDialog(string _key)
        {


            m_current = null;
            foreach (Node node in nodes)
            {
                if (node is VP_DialogInitEvent && node.Outputs.All(y => y.IsConnected))
                {
                    VP_DialogInitEvent ev = (VP_DialogInitEvent)node;
                    if (ev.startEvent == _key)
                    {
                        VP_DialogBaseNode connectedNode = ev.output;

                        if (connectedNode == null)
                        {
                            Debug.LogError("The init node with starting key " + _key + " has null output.");
                            break;
                        }

                        if (m_current != null)
                            m_current.IsCurrent = false;

                        m_current = connectedNode;
                        m_current.IsCurrent = true;

                        break;
                    }
                }
            }

            if (m_current && m_chart)
            {

                m_chart.m_currentNode = m_current;
                m_current.Trigger();
            }
            else
            {
                Debug.LogError("Couldn't find init node for: " + _key);
            }
        }

        #region Log Register
        public void RegisterDialog(VP_DialogCharacterData _character, string _text)
        {
            if (m_saidDialogs == null)
                m_saidDialogs = new List<VP_DialogLog>();

            VP_DialogLog log = new VP_DialogLog(_character, _text);
            m_saidDialogs.Add(log);

            if (m_saidDialogs.Count == 1)
            {
                VP_DialogManager.OnDialogRegisterAbleAction();
            }
        }

        public void DeleteAllRegisteredDialogsByCharacterName(string _characterName)
        {
            if (m_saidDialogs == null)
                m_saidDialogs = new List<VP_DialogLog>();

            List<VP_DialogLog> _logs = new List<VP_DialogLog>(m_saidDialogs);

            foreach (VP_DialogLog log in _logs)
            {
                if (log.m_character.characterName == _characterName)
                    m_saidDialogs.Remove(log);
            }

            if (m_saidDialogs.Count < 1)
                VP_DialogManager.OnDialogRegisterDisableAction();
        }

        public void DeleteAllRegisteredDialogs()
        {
            if (m_saidDialogs == null)
                m_saidDialogs = new List<VP_DialogLog>();

            m_saidDialogs.Clear();
            VP_DialogManager.OnDialogRegisterDisableAction();
        }

        public void DeleteRegisteredDialog(int _index)
        {
            if (m_saidDialogs == null)
                m_saidDialogs = new List<VP_DialogLog>();

            if (_index < m_saidDialogs.Count)
                m_saidDialogs.RemoveAt(_index);

            if (m_saidDialogs.Count < 1)
                VP_DialogManager.OnDialogRegisterDisableAction();
        }

        public VP_DialogLog GetRegisteredDialog(int _index)
        {
            if (m_saidDialogs == null)
                m_saidDialogs = new List<VP_DialogLog>();

            return m_saidDialogs[_index];
        }

        public List<VP_DialogLog> GetAllRegisteredDialogsByCharacterName(string _characterName)
        {
            if (m_saidDialogs == null)
                m_saidDialogs = new List<VP_DialogLog>();

            List<VP_DialogLog> _logs = new List<VP_DialogLog>();

            foreach (VP_DialogLog log in m_saidDialogs)
            {
                if (log.m_character.characterName == _characterName)
                    _logs.Add(log);
            }

            return _logs;
        }

        public List<VP_DialogLog> GetAllRegisteredDialogs()
        {
            if (m_saidDialogs == null)
                m_saidDialogs = new List<VP_DialogLog>();

            return m_saidDialogs;
        }
        #endregion

        public void ContinueText(int i = 0)
        {
            if (m_current is VP_Dialog)
            {
                if (!((VP_Dialog)m_current).ContinueText(i))
                {
                    // 
                }
            }
            else
            {
                m_current.Trigger();
            }
        }

        public void CallChart(VP_Dialog dialog, VP_DialogChart _chart)
        {
            if (_chart && m_chart != _chart)
                m_chart = _chart;

            if (m_current != null)
                m_current.IsCurrent = false;

            m_current = dialog;
            m_current.IsCurrent = true;
            m_chart.m_currentNode = m_current;
            m_chart.TriggerDialog(dialog);
        }

        public void CallChart(VP_Dialog dialog, bool _skipping)
        {

            if (m_current != null)
                m_current.IsCurrent = false;

            m_current = dialog;
            m_current.IsCurrent = true;
            if (m_chart)
            {
                m_chart.m_currentNode = m_current;
                ContinueText();
            }
        }

        public void CallChart(VP_Dialog dialog)
        {
            if (m_chart == null)
            {
                Debug.LogError("No chart in dialogue graph");
                return;
            }


            if (m_current != null)
                m_current.IsCurrent = false;

            m_current = dialog;
            m_current.IsCurrent = true;
            m_chart.m_currentNode = m_current;
            m_chart.TriggerDialog(dialog);
        }

        public bool HasOnStartEvent()
        {
            foreach (Node n in nodes)
            {
                if (n is VP_DialogInitEvent && n.Outputs.All(y => y.IsConnected)) // 
                {
                    if ((n as VP_DialogInitEvent).onStart)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void SetCurrentAfterInit(bool _startDialog = true)
        {
            //current = nodes.Find(x => x is VP_Dialog && x.Inputs.All(y => !y.IsConnected)) as VP_Dialog;
            VP_DialogInitEvent initEv = nodes.Find(x => x is VP_DialogInitEvent && (x as VP_DialogInitEvent).onStart && x.Outputs.All(y => y.IsConnected)) as VP_DialogInitEvent;
            if (initEv != null)
            {
                VP_DialogBaseNode node = initEv.output;

                if (node)
                {
                    if (m_current != null)
                        m_current.IsCurrent = false;

                    m_current = node;
                    m_current.IsCurrent = true;
                }
            }

            if (m_current && m_chart && !VP_DialogManager.IsSpeaking)
            {
                if (m_current is VP_Dialog)
                {
                    m_chart.TriggerDialog(m_current as VP_Dialog);
                }
                else
                {
                    m_current.Trigger();
                }
            }
        }
    }
}
