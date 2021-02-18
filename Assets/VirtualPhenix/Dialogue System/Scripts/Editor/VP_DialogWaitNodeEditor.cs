using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNodeEditor;
using UnityEditor;

namespace VirtualPhenix.Dialog
{
    [CustomNodeEditor(typeof(VP_DialogWaitNode))]
    public class VP_DialogWaitNodeEditor : VP_DialogBaseNodeEditor
    {
        public override void OnHeaderGUI()
        {
            GUILayout.Label("Wait", NodeEditorResources.styles.nodeHeader, GUILayout.Height(30));
        }
        public override void OnBodyGUI()
        {

            serializedObject.Update();

            VP_DialogEvent node = target as VP_DialogEvent;
            NodeEditorGUILayout.PortField(target.GetInputPort(VP_DialogSetup.Fields.DIALOG_NODE_INPUT), GUILayout.Width(100));
            NodeEditorGUILayout.PortField(GUIContent.none, target.GetOutputPort(VP_DialogSetup.Fields.DIALOG_NODE_OUTPUT), GUILayout.MinWidth(0));
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty(VP_DialogSetup.Fields.DIALOG_NODE_WAIT_TIME), GUILayout.MinWidth(0));
            GUILayout.Space(10);

            serializedObject.ApplyModifiedProperties();
        }

        public override int GetWidth()
        {
            return 236;
        }
    }
}

