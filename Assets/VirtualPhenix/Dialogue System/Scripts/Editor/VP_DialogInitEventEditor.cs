using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XNodeEditor;

namespace VirtualPhenix.Dialog
{
    [CustomNodeEditor(typeof(VP_DialogInitEvent))]
    public class VP_DialogInitEventEditor : VP_DialogBaseNodeEditor
    {
        public override void OnHeaderGUI()
        {
            GUILayout.Label("Start Event", NodeEditorResources.styles.nodeHeader, GUILayout.Height(30));
        }

        public override void OnBodyGUI()
        {
            serializedObject.Update();

            VP_DialogInitEvent node = target as VP_DialogInitEvent;

            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(VP_DialogSetup.Fields.DIALOG_NODE_ON_START_EVENT));
            
            if (!node.onStart)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty(VP_DialogSetup.Fields.DIALOG_NODE_TEST_KEY), GUIContent.none);
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(VP_DialogSetup.Fields.DIALOG_NODE_START_EVENT));
            }

            EditorGUILayout.Space();
            NodeEditorGUILayout.PortField(target.GetOutputPort(VP_DialogSetup.Fields.DIALOG_NODE_OUTPUT), GUILayout.Width(197));
          
            serializedObject.ApplyModifiedProperties();
        }

        public override int GetWidth()
        {
            return 230;
        }
    }
}