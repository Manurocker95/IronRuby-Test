using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XNodeEditor;

namespace VirtualPhenix.Dialog
{


    [CustomNodeEditor(typeof(VP_ChooseNumberNode))]
    public class VP_ChooseNumberNodeEditor : VP_DialogBaseNodeEditor
    {

        public override void OnHeaderGUI()
        {
            GUILayout.Label("Choose Number", NodeEditorResources.styles.nodeHeader, GUILayout.Height(30));
        }

        public override void OnBodyGUI()
        {
            serializedObject.Update();

            VP_ChooseNumberNode node = target as VP_ChooseNumberNode;

            if (node == null)
                return;

            NodeEditorGUILayout.PortField(target.GetInputPort(VP_DialogSetup.Fields.DIALOG_NODE_INPUT), GUILayout.Width(100));

            EditorGUILayout.Space();

            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(VP_DialogSetup.Fields.CHOOSE_NUMBER_NODE_MESSAGE));
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(VP_DialogSetup.Fields.CHOOSE_NUMBER_NODE_TRANSLATE));
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(VP_DialogSetup.Fields.CHOOSE_NUMBER_NODE_CANCANCEL));

            bool cancelled = node.CanCancel;

            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(VP_DialogSetup.Fields.DIALOG_NODE_POSITION_DATA));

            EditorGUILayout.Space();
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(VP_DialogSetup.Fields.CHOOSE_NUMBER_NODE_MIN));
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(VP_DialogSetup.Fields.CHOOSE_NUMBER_NODE_MAX));
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(VP_DialogSetup.Fields.CHOOSE_NUMBER_NODE_COMPARISON));
            EditorGUILayout.Space();
            bool compare = node.Comparison != VariableComparison.None;
            if (compare)
            {
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(VP_DialogSetup.Fields.CHOOSE_NUMBER_NODE_NUMBER));

                EditorGUILayout.Space();


                NodeEditorGUILayout.PortField(target.GetOutputPort(VP_DialogSetup.Fields.DIALOG_NODE_CONDITION_TRUE));
                NodeEditorGUILayout.PortField(target.GetOutputPort(VP_DialogSetup.Fields.DIALOG_NODE_CONDITION_FALSE));

            }
            else
            {
                EditorGUILayout.Space();
                NodeEditorGUILayout.PortField(target.GetOutputPort(VP_DialogSetup.Fields.DIALOG_NODE_OUTPUT), GUILayout.MinWidth(0));

            }
            EditorGUILayout.Space();
            if (cancelled)
                NodeEditorGUILayout.PortField(target.GetOutputPort(VP_DialogSetup.Fields.CHOOSE_NUMBER_NODE_CANCELOUTPUT), GUILayout.MinWidth(0));
            serializedObject.ApplyModifiedProperties();

        }

        public override int GetWidth()
        {
            return 306;
        }

        public override Color GetTint()
        {
            return base.GetTint();
        }
    }
}