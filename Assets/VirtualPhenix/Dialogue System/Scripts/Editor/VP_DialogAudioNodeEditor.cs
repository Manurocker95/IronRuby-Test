using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNodeEditor;
using UnityEditor;

namespace VirtualPhenix.Dialog
{
    [CustomNodeEditor(typeof(VP_DialogAudioNode))]
    public class VP_DialogAudioNodeEditor : VP_DialogBaseNodeEditor
    {
        public override void OnHeaderGUI()
        {
            GUILayout.Label("Audio", NodeEditorResources.styles.nodeHeader, GUILayout.Height(30));
        }
        public override void OnBodyGUI()
        {

            serializedObject.Update();

            VP_DialogEvent node = target as VP_DialogEvent;
            NodeEditorGUILayout.PortField(target.GetInputPort(VP_DialogSetup.Fields.DIALOG_NODE_INPUT), GUILayout.Width(100));
            NodeEditorGUILayout.PortField(GUIContent.none, target.GetOutputPort(VP_DialogSetup.Fields.DIALOG_NODE_OUTPUT), GUILayout.MinWidth(0));
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty(VP_DialogSetup.Fields.DIALOG_NODE_AUDIO_PREV_KEY), GUILayout.MinWidth(0));
            GUILayout.Space(10);

            EditorGUILayout.PropertyField(serializedObject.FindProperty(VP_DialogSetup.Fields.DIALOG_NODE_AUDIO_ACTION), GUILayout.MinWidth(0));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(VP_DialogSetup.Fields.DIALOG_NODE_AUDIO_TYPE), GUILayout.MinWidth(0));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(VP_DialogSetup.Fields.DIALOG_NODE_AUDIO_CLIP), GUILayout.MinWidth(0));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(VP_DialogSetup.Fields.DIALOG_NODE_AUDIO_KEY_DATA), GUIContent.none);
            EditorGUILayout.PropertyField(serializedObject.FindProperty(VP_DialogSetup.Fields.DIALOG_NODE_AUDIO_KEY), GUILayout.MinWidth(0));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(VP_DialogSetup.Fields.DIALOG_NODE_AUDIO_LOOP), GUILayout.MinWidth(0));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(VP_DialogSetup.Fields.DIALOG_NODE_AUDIO_ADD), GUILayout.MinWidth(0));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(VP_DialogSetup.Fields.DIALOG_NODE_AUDIO_REMOVE), GUILayout.MinWidth(0));
            GUILayout.Space(10);
            EditorGUILayout.LabelField("Volume");
            //EditorGUILayout.PropertyField(serializedObject.FindProperty(VP_DialogSetup.Fields.DIALOG_NODE_AUDIO_VOLUME), GUILayout.MinWidth(0));

            Rect position = new Rect(90, 263, 200, 20);
            GUIContent label = new GUIContent("Volume");
           
            EditorGUI.BeginChangeCheck();
            var newValue = EditorGUI.Slider(position, serializedObject.FindProperty(VP_DialogSetup.Fields.DIALOG_NODE_AUDIO_VOLUME).floatValue, 0f, 1f);
            // Only assign the value back if it was actually changed by the user.
            // Otherwise a single value will be assigned to all objects when multi-object editing,
            // even when the user didn't touch the control.
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.FindProperty(VP_DialogSetup.Fields.DIALOG_NODE_AUDIO_VOLUME).floatValue = newValue;
            }
          
            EditorGUILayout.Space();
            GUILayout.Space(50);
            serializedObject.ApplyModifiedProperties();
        }

        public override int GetWidth()
        {
            return 336;
        }
    }
}

