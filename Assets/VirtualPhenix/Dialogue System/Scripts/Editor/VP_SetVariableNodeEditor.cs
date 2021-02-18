using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XNodeEditor;

namespace VirtualPhenix.Dialog
{


    [CustomNodeEditor(typeof(VP_SetVariableNode))]
    public class VP_SetVariableNodeEditor : VP_DialogBaseNodeEditor
    {

        public override void OnHeaderGUI()
        {
            GUILayout.Label("Set Variable", NodeEditorResources.styles.nodeHeader, GUILayout.Height(30));
        }

        public override void OnBodyGUI()
        {
            serializedObject.Update();

            VP_SetVariableNode node = target as VP_SetVariableNode;

            if (node == null)
                return;

            NodeEditorGUILayout.PortField(target.GetInputPort(VP_DialogSetup.Fields.DIALOG_NODE_INPUT), GUILayout.Width(100));
            NodeEditorGUILayout.PortField(GUIContent.none, target.GetOutputPort(VP_DialogSetup.Fields.DIALOG_NODE_OUTPUT), GUILayout.MinWidth(0));
            EditorGUILayout.Space();
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(VP_DialogSetup.Fields.SAVE_VARIABLES_AS_PLAYERPREFS));

            EditorGUILayout.Space();

            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(VP_DialogSetup.Fields.SET_VARIABLE_VARIABLES));
            EditorGUILayout.Space();
            serializedObject.ApplyModifiedProperties();

        }

        public override int GetWidth()
        {
            return 366;
        }

        public override Color GetTint()
        {
            return base.GetTint();
        }
    }
}