using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XNodeEditor;

namespace VirtualPhenix.Dialog
{


    [CustomNodeEditor(typeof(VP_DialogLogNode))]
    public class VVP_DialogLogNodeEditor : VP_DialogBaseNodeEditor
    {

        public override void OnHeaderGUI()
        {
            GUILayout.Label("Debug Log", NodeEditorResources.styles.nodeHeader, GUILayout.Height(30));
        }

        public override void OnBodyGUI()
        {
            serializedObject.Update();

            VP_DialogLogNode node = target as VP_DialogLogNode;

            if (node == null)
                return;

            NodeEditorGUILayout.PortField(target.GetInputPort(VP_DialogSetup.Fields.DIALOG_NODE_INPUT), GUILayout.Width(100));
            NodeEditorGUILayout.PortField(target.GetOutputPort(VP_DialogSetup.Fields.DIALOG_NODE_OUTPUT), GUILayout.MinWidth(0));
            EditorGUILayout.Space();

            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(VP_DialogSetup.Fields.CHOOSE_NUMBER_NODE_MESSAGE));
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(VP_DialogSetup.Fields.DEBUG_LOG_NODE_TYPE));
    
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