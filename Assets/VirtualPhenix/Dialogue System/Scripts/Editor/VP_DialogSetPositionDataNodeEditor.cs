using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using XNodeEditor;

namespace VirtualPhenix.Dialog
{
    [CustomNodeEditor(typeof(VP_DialogSetPositionDataNode))]
    public class VP_DialogSetPositionDataNodeEditor : VP_DialogBaseNodeEditor
    {
        public override void OnHeaderGUI()
        {
            GUILayout.Label("Position", NodeEditorResources.styles.nodeHeader, GUILayout.Height(30));
        }
        public override void OnBodyGUI()
        {

            serializedObject.Update();

            VP_DialogSetPositionDataNode node = target as VP_DialogSetPositionDataNode;
            NodeEditorGUILayout.PortField(target.GetInputPort(VP_DialogSetup.Fields.DIALOG_NODE_INPUT), GUILayout.Width(100));
            NodeEditorGUILayout.PortField(GUIContent.none, target.GetOutputPort(VP_DialogSetup.Fields.DIALOG_NODE_OUTPUT), GUILayout.MinWidth(0));
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty(VP_DialogSetup.Fields.DIALOG_NODE_POSITION_DATA), GUILayout.MinWidth(0));
            GUILayout.Space(15);
            EditorGUILayout.PropertyField(serializedObject.FindProperty(VP_DialogSetup.Fields.DIALOG_NODE_USE_DEFAULT_POS), GUILayout.MinWidth(0));

            bool useDefault = node.m_useDefaultPosition;
            GUILayout.Space(10);
            if (!useDefault)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty(VP_DialogSetup.Fields.DIALOG_NODE_POSITION_NEW_VALUE), GUILayout.MinWidth(0));
            }
            GUILayout.Space(10);
            bool setTextRect = node.m_setTextPosition;
            EditorGUILayout.PropertyField(serializedObject.FindProperty(VP_DialogSetup.Fields.DIALOG_NODE_SET_TEXT_POS), GUILayout.MinWidth(0));
            GUILayout.Space(10);

            if (setTextRect)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty(VP_DialogSetup.Fields.DIALOG_NODE_TOP_BOT_MUG), GUILayout.MinWidth(0));
                EditorGUILayout.PropertyField(serializedObject.FindProperty(VP_DialogSetup.Fields.DIALOG_NODE_LEFT_RIGHT_MUG), GUILayout.MinWidth(0));
                EditorGUILayout.PropertyField(serializedObject.FindProperty(VP_DialogSetup.Fields.DIALOG_NODE_TOP_BOT_REG), GUILayout.MinWidth(0));
                EditorGUILayout.PropertyField(serializedObject.FindProperty(VP_DialogSetup.Fields.DIALOG_NODE_LEFT_RIGHT_REG), GUILayout.MinWidth(0));
            }
            GUILayout.Space(10);
            EditorGUILayout.PropertyField(serializedObject.FindProperty(VP_DialogSetup.Fields.DIALOG_NODE_SET_BG_POS), GUILayout.MinWidth(0));
            GUILayout.Space(10);
            bool setBGRect = node.m_setBGPosition;
            if (setBGRect)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty(VP_DialogSetup.Fields.DIALOG_NODE_TOP_BOT_BG), GUILayout.MinWidth(0));
                EditorGUILayout.PropertyField(serializedObject.FindProperty(VP_DialogSetup.Fields.DIALOG_NODE_LEFT_RIGHT_BG), GUILayout.MinWidth(0));          
            }
            GUILayout.Space(10);
            EditorGUILayout.PropertyField(serializedObject.FindProperty(VP_DialogSetup.Fields.DIALOG_NODE_SET_GO_POS), GUILayout.MinWidth(0));
            GUILayout.Space(10);
            bool setGOName = node.m_useGameObjectPosition;
            if (setGOName)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty(VP_DialogSetup.Fields.DIALOG_NODE_SET_GO_NAME), GUILayout.MinWidth(0));

            }
            GUILayout.Space(10);
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