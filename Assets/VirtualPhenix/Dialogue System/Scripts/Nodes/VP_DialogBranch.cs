using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using XNode;

namespace VirtualPhenix.Dialog
{
    public enum VariableTypes
    {
        Bool, 
        Int,
        Float,
        Double,
        String,
        UnityObject,
        GameObject
    }

    [System.Serializable]
    public class VP_VariableCompareData
    {
        public VariableTypes VarType = VariableTypes.Bool;
        public string varName = "";
        public bool boolvalue;
        public int intvalue;
        public float floatvalue;
        public double doublevalue;
        public string stringvalue;
        public GameObject gameobjectvalue;
        public UnityEngine.Object unityobjectvalue;
        public VariableComparison variablecomparison;

        public bool Check(VP_VariableDataBase _graph)
        {
            if (string.IsNullOrEmpty(varName))
                return false;

            switch (VarType)
            {
                case VariableTypes.Bool:
                    bool _graphVar = _graph.GetBoolVariableValue(varName);

                    return (boolvalue == _graphVar);
                case VariableTypes.Double:
                    switch (variablecomparison)
                    {
                        case VariableComparison.Equal:
                            return (doublevalue == _graph.GetDoubleVariableValue(varName));
                        case VariableComparison.Mayor:
                            return (doublevalue < _graph.GetDoubleVariableValue(varName));
                        case VariableComparison.Minor:
                            return (doublevalue > _graph.GetDoubleVariableValue(varName));
                        case VariableComparison.MinorEqual:
                            return (doublevalue >= _graph.GetDoubleVariableValue(varName));
                        case VariableComparison.MayorEqual:
                            return (doublevalue <= _graph.GetDoubleVariableValue(varName));
                        default:
                            return false;
                    }
                case VariableTypes.Int:
                    switch (variablecomparison)
                    {
                        case VariableComparison.Equal:
                            return (intvalue == _graph.GetIntVariableValue(varName));
                        case VariableComparison.Mayor:
                            return (intvalue < _graph.GetIntVariableValue(varName));
                        case VariableComparison.Minor:
                            return (intvalue > _graph.GetIntVariableValue(varName));
                        case VariableComparison.MinorEqual:
                            return (intvalue >= _graph.GetDoubleVariableValue(varName));
                        case VariableComparison.MayorEqual:
                            return (intvalue <= _graph.GetDoubleVariableValue(varName));
                        default:
                            return false;
                    }
                case VariableTypes.Float:
                    switch (variablecomparison)
                    {
                        case VariableComparison.Equal:
                            return (floatvalue == _graph.GetFloatVariableValue(varName));
                        case VariableComparison.Mayor:
                            return (floatvalue < _graph.GetFloatVariableValue(varName));
                        case VariableComparison.Minor:
                            return (floatvalue > _graph.GetFloatVariableValue(varName));
                        case VariableComparison.MinorEqual:
                            return (floatvalue >= _graph.GetDoubleVariableValue(varName));
                        case VariableComparison.MayorEqual:
                            return (floatvalue <= _graph.GetDoubleVariableValue(varName));
                        default:
                            return false;
                    }
                case VariableTypes.String:
                    return stringvalue == _graph.GetStringVariableValue(varName);
                case VariableTypes.GameObject:
                    return gameobjectvalue == _graph.GetGameObjectariableValue(varName);
                case VariableTypes.UnityObject:
                    return unityobjectvalue == _graph.GetUnityObjectariableValue(varName);

                default:
                    return true;
            }
        }
    }

    public enum VariableComparison
    {
        Equal,
        Mayor,
        Minor,
        MayorEqual,
        MinorEqual,
        None // No comparison
    }

    public enum ConditionComparison
    {
        And,
        Or
    }

    [NodeTint(VP_DialogSetup.NodeColors.BRANCH_NODE), CreateNodeMenuAttribute("Branch")]
    public class VP_DialogBranch : VP_DialogBaseNode
    {
        public ConditionComparison m_variableComparison;
        public ConditionComparison m_variableAndConditionComparison;
        public List<VP_VariableCompareData> graphVariableList;
        public List<VP_VariableCompareData> globalVariableList;
        /// <summary>
        /// Is true? node
        /// </summary>
        [Output] public VP_DialogBaseNode isTrue;
        /// <summary>
        /// Is false? node
        /// </summary>
        [Output] public VP_DialogBaseNode isFalse;

        private bool success;

        protected override void OnEnable()
        {
            base.OnEnable();
            if (graphVariableList == null)
                graphVariableList = new List<VP_VariableCompareData>();

            if (globalVariableList == null)
                globalVariableList = new List<VP_VariableCompareData>();
        }

        public override void Trigger()
        {
            if (m_needToSkip)
            {
                m_needToSkip = false;
            }

            bool success = IsSuccess();

            //Trigger next nodes
            NodePort port;
            if (success)
                port = GetOutputPort(VP_DialogSetup.Fields.DIALOG_NODE_CONDITION_TRUE);
            else
                port = GetOutputPort(VP_DialogSetup.Fields.DIALOG_NODE_CONDITION_FALSE);

            VP_DialogManager.OnDialogCompleteAction();
            if (port == null)
            {
                port = GetOutputPort(VP_DialogSetup.Fields.DIALOG_NODE_OUTPUT);
            }
            
            if (port.ConnectionCount > 0)
            {
                for (int i = 0; i < port.ConnectionCount; i++)
                {
                    NodePort connection = port.GetConnection(i);
                    (connection.node as VP_DialogBaseNode).Trigger();
                }
            }
            else
            {
                CheckEndSequenceNode();
                return;
            }
            
        }

        public bool IsSuccess()
        {
            if (graphVariableList.Count == 0 && globalVariableList.Count == 0)
                return true;
          
            VP_VariableDataBase dg = (this.graph as VP_DialogGraph).GraphVariables;
           
            int counter = 0;

            if (graphVariableList.Count > 0)
            {
                // Perform condition
                for (int i = 0; i < graphVariableList.Count; i++)
                {
                    if (m_variableComparison == ConditionComparison.And)
                    {
                        if (!graphVariableList[i].Check(dg))
                        {
                            return false;
                        }
                    }
                    else if (m_variableComparison == ConditionComparison.Or)
                    {
                        if (graphVariableList[i].Check(dg))
                        {
                            return true;
                        }
                    }

                    counter++;
                }
            }


            int counter2 = 0;

            dg = VP_DialogManager.Instance.GlobalVariables;
            if (globalVariableList.Count > 0)
            {
                // Perform condition
                for (int i = 0; i < globalVariableList.Count; i++)
                {
                    if (m_variableComparison == ConditionComparison.And)
                    {
                        if (!globalVariableList[i].Check(dg))
                        {
                            return false;
                        }
                    }
                    else if (m_variableComparison == ConditionComparison.Or)
                    {
                        if (globalVariableList[i].Check(dg))
                        {
                            return true;
                        }
                    }

                    counter2++;
                }

            }

            return (counter == graphVariableList.Count && counter2 == globalVariableList.Count);
        }

        public override void OnCreateConnection(NodePort from, NodePort to)
        {
            base.OnCreateConnection(from, to);

            if (from == GetOutputPort(VP_DialogSetup.Fields.DIALOG_NODE_OUTPUT))
            {
                Debug.LogError("This node must be set from Is True or Is False");
                from.Disconnect(0);
            }

            if (from == GetOutputPort(VP_DialogSetup.Fields.DIALOG_NODE_CONDITION_TRUE))
            {
                isTrue = to.node as VP_DialogBaseNode;
            }

            if (from == GetOutputPort(VP_DialogSetup.Fields.DIALOG_NODE_CONDITION_FALSE))
            {
                isFalse = to.node as VP_DialogBaseNode;
            }
        }

        public override void OnRemoveConnection(NodePort port)
        {
            base.OnRemoveConnection(port);

            if (port == GetOutputPort(VP_DialogSetup.Fields.DIALOG_NODE_CONDITION_TRUE))
            {
                isTrue = null;
            }

            if (port == GetOutputPort(VP_DialogSetup.Fields.DIALOG_NODE_CONDITION_FALSE))
            {
                isFalse = null;
            }
        }
    }
}