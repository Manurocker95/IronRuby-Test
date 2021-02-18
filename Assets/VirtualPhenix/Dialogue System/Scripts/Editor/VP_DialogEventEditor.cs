using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XNodeEditor;

namespace VirtualPhenix.Dialog
{
    [CustomNodeEditor(typeof(VP_DialogEvent))]
    public class VP_DialogEventEditor : VP_DialogBaseNodeEditor
    {
        public override void OnHeaderGUI()
        {
            GUILayout.Label("Event Trigger", NodeEditorResources.styles.nodeHeader, GUILayout.Height(30));
        }
        public override void OnBodyGUI() {
            
            serializedObject.Update();

            VP_DialogEvent node = target as VP_DialogEvent;
            NodeEditorGUILayout.PortField(target.GetInputPort(VP_DialogSetup.Fields.DIALOG_NODE_INPUT), GUILayout.Width(100));
            NodeEditorGUILayout.PortField(GUIContent.none, target.GetOutputPort(VP_DialogSetup.Fields.DIALOG_NODE_OUTPUT), GUILayout.MinWidth(0));
            EditorGUILayout.Space();
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(VP_DialogSetup.Fields.DIALOG_NODE_TRIGGER_TIME));
            EditorGUILayout.Space();

            if (node.m_triggerTime == VP_DialogEvent.TRIGGER_TIME.ON_CHARACTER_SPEAK)
            {
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(VP_DialogSetup.Fields.DIALOG_NODE_TRIGGER_CHARACTER));
            }
            else if (node.m_triggerTime == VP_DialogEvent.TRIGGER_TIME.ON_DIALOG_ANSWER_SELECTED || node.m_triggerTime == VP_DialogEvent.TRIGGER_TIME.ON_DIALOG_NUMBER_SELECTED)
            {
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(VP_DialogSetup.Fields.DIALOG_NODE_TRIGGER_SAME_CHOICE));
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(VP_DialogSetup.Fields.DIALOG_NODE_TRIGGER_CHOICE));
            }

            EditorGUILayout.Space();
            //NodeEditorGUILayout.DynamicPortListWithoutHandles(VP_DialogSetup.Fields.DIALOG_NODE_TRIGGER, typeof(VP_DialogBaseNode), serializedObject, XNode.NodePort.IO.Output, XNode.Node.ConnectionType.Multiple);
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(VP_DialogSetup.Fields.DIALOG_NODE_TRIGGER));
          
            EditorGUILayout.Space();
            //NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(VP_DialogSetup.Fields.DIALOG_NODE_TRIGGER_STRING));
            NodeEditorGUILayout.DynamicPortListWithoutHandles(VP_DialogSetup.Fields.DIALOG_NODE_TRIGGER_STRING, typeof(VP_DialogBaseNode), serializedObject, XNode.NodePort.IO.Output, XNode.Node.ConnectionType.Multiple);
            //NodeEditorGUILayout.DynamicPortListWithoutHandles(VP_DialogSetup.Fields.DIALOG_NODE_TRIGGER_LIST, typeof(VP_DialogEventData), serializedObject, XNode.NodePort.IO.Output, XNode.Node.ConnectionType.Multiple);

            EditorGUILayout.Space();
            serializedObject.ApplyModifiedProperties();
        }

        public override int GetWidth()
        {
            return 336;
        }
    }
}