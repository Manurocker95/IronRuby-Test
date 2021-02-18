using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XNodeEditor;

namespace VirtualPhenix.Dialog
{


    [CustomNodeEditor(typeof(VP_DialogBranch))]
    public class VP_DialogBranchEditor : VP_DialogBaseNodeEditor {

        public override void OnHeaderGUI()
        {
            GUILayout.Label("Branch", NodeEditorResources.styles.nodeHeader, GUILayout.Height(30));
        }

        public override void OnBodyGUI() {
            serializedObject.Update();

            VP_DialogBranch node = target as VP_DialogBranch;

            if (node == null)
                return;

            NodeEditorGUILayout.PortField(target.GetInputPort(VP_DialogSetup.Fields.DIALOG_NODE_INPUT));
            EditorGUILayout.Space();


            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("m_variableComparison"));
            //node.m_variableComparison = (ConditionComparison)EditorGUILayout.EnumPopup("Variable Comparison", node.m_variableComparison);
            NodeEditorGUILayout.TestReorderList(VP_DialogSetup.Fields.VARIABLE_LIST, node.graphVariableList, typeof(VP_DialogBaseNode), serializedObject, XNode.NodePort.IO.None, XNode.Node.ConnectionType.Multiple);
            EditorGUILayout.Space();
            NodeEditorGUILayout.TestReorderList(VP_DialogSetup.Fields.GLOBAL_VARIABLE_LIST, node.globalVariableList, typeof(VP_DialogBaseNode), serializedObject, XNode.NodePort.IO.None, XNode.Node.ConnectionType.Multiple);

            EditorGUILayout.Space();
            NodeEditorGUILayout.PortField(target.GetOutputPort(VP_DialogSetup.Fields.DIALOG_NODE_CONDITION_TRUE));
            NodeEditorGUILayout.PortField(target.GetOutputPort(VP_DialogSetup.Fields.DIALOG_NODE_CONDITION_FALSE));

            serializedObject.ApplyModifiedProperties();

        }

        bool testing()
        {
            return true;
        }

        public override int GetWidth() {
            return 366;
        }

        public override Color GetTint()
        {
            return base.GetTint();
        }
    }
}