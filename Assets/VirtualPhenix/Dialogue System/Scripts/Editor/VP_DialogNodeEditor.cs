using UnityEditor;
using UnityEngine;
using XNode;
using XNodeEditor;

namespace VirtualPhenix.Dialog
{
    [CustomNodeEditor(typeof(VP_Dialog))]
    public class VP_DialogNodeEditor : VP_DialogBaseNodeEditor
    {
        public override void OnHeaderGUI()
        {
            GUILayout.Label("Dialog", NodeEditorResources.styles.nodeHeader, GUILayout.Height(30));
        }

        public override void OnBodyGUI()
        {
            serializedObject.Update();

            VP_Dialog node = target as VP_Dialog;

            bool hideInfoInGraph = node.m_hideInfoInGraph;
            if (hideInfoInGraph)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty(VP_DialogSetup.Fields.DIALOG_NODE_HIDE_SHOW_INFO), new GUIContent("Hide Info"));
                if (node.answers.Count == 0)
                {
                    GUILayout.BeginHorizontal();
                    NodeEditorGUILayout.PortField(GUIContent.none, target.GetInputPort(VP_DialogSetup.Fields.DIALOG_NODE_INPUT), GUILayout.MinWidth(0));
                    NodeEditorGUILayout.PortField(GUIContent.none, target.GetOutputPort(VP_DialogSetup.Fields.DIALOG_NODE_OUTPUT), GUILayout.MinWidth(0));
                    GUILayout.EndHorizontal();
                }
                else
                {
                    NodeEditorGUILayout.PortField(GUIContent.none, target.GetInputPort(VP_DialogSetup.Fields.DIALOG_NODE_INPUT));
                }

                bool useLoc = node.m_useLocalization;
                EditorGUILayout.PropertyField(serializedObject.FindProperty(VP_DialogSetup.Fields.DIALOG_NODE_USE_LOCALIZATION), new GUIContent("Localization"));

                if (useLoc)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(VP_DialogSetup.Fields.DIALOG_NODE_TEST_KEY), new GUIContent("Key T"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(VP_DialogSetup.Fields.DIALOG_NODE_KEY), new GUIContent("Key in Text Dictionary"));
                }
                else
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(VP_DialogSetup.Fields.DIALOG_NODE_KEY), new GUIContent("Text"));
                }

                NodeEditorGUILayout.DynamicPortList(VP_DialogSetup.Fields.DIALOG_NODE_ANSWERS, typeof(VP_DialogBaseNode), serializedObject, NodePort.IO.Output, Node.ConnectionType.Multiple);
            }
            else
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty(VP_DialogSetup.Fields.DIALOG_NODE_HIDE_SHOW_INFO), new GUIContent("Hide Info"));

                GUILayout.Space(10);
                EditorGUILayout.PropertyField(serializedObject.FindProperty(VP_DialogSetup.Fields.DIALOG_NODE_DIALOG_TYPE), new GUIContent("Background"), GUILayout.MinWidth(40));
                EditorGUILayout.PropertyField(serializedObject.FindProperty(VP_DialogSetup.Fields.DIALOG_NODE_CHARACTER), new GUIContent("Character"), GUILayout.MinWidth(20));
                if (node.answers.Count == 0)
                {
                    GUILayout.BeginHorizontal();
                    NodeEditorGUILayout.PortField(GUIContent.none, target.GetInputPort(VP_DialogSetup.Fields.DIALOG_NODE_INPUT), GUILayout.MinWidth(0));
                    NodeEditorGUILayout.PortField(GUIContent.none, target.GetOutputPort(VP_DialogSetup.Fields.DIALOG_NODE_OUTPUT), GUILayout.MinWidth(0));
                    GUILayout.EndHorizontal();
                }
                else
                {
                    NodeEditorGUILayout.PortField(GUIContent.none, target.GetInputPort(VP_DialogSetup.Fields.DIALOG_NODE_INPUT));
                }
                GUILayout.Space(-30);

                GUILayout.Space(30);

                EditorGUILayout.PropertyField(serializedObject.FindProperty(VP_DialogSetup.Fields.DIALOG_NODE_POSITION_DATA), GUILayout.MinWidth(0));


                GUILayout.Space(15);

              
                EditorGUILayout.PropertyField(serializedObject.FindProperty(VP_DialogSetup.Fields.DIALOG_NODE_USE_LOCALIZATION), new GUIContent("Localization"));
	            bool useLoc = node.m_useLocalization;
	            
                if (useLoc)
                {
                	EditorGUILayout.PropertyField(serializedObject.FindProperty(VP_DialogSetup.Fields.DIALOG_NODE_USE_KEY_FOR_LOCALIZATION), new GUIContent("Use Key"));

                	bool useKey = node.m_useKey;
                	if (useKey)
                	{
                		
	                	EditorGUILayout.PropertyField(serializedObject.FindProperty(VP_DialogSetup.Fields.DIALOG_NODE_TEST_KEY), new GUIContent("Key T"));
	                	EditorGUILayout.PropertyField(serializedObject.FindProperty(VP_DialogSetup.Fields.DIALOG_NODE_KEY), new GUIContent("Key in Text Dictionary"));
                	}
                	else
                	{
	                	EditorGUILayout.PropertyField(serializedObject.FindProperty(VP_DialogSetup.Fields.DIALOG_NODE_KEY), new GUIContent("Original Text"));
                	}
                }
                else
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(VP_DialogSetup.Fields.DIALOG_NODE_KEY), new GUIContent("Text"));
                }

                EditorGUILayout.PropertyField(serializedObject.FindProperty(VP_DialogSetup.Fields.DIALOG_NODE_WAIT_FOR_INPUT), new GUIContent("Wait for Input"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty(VP_DialogSetup.Fields.DIALOG_NODE_SHOW_DIRECTLY), new GUIContent("Show Directly"));

                GUILayout.Space(15);
                EditorGUILayout.PropertyField(serializedObject.FindProperty(VP_DialogSetup.Fields.DIALOG_NODE_AUDIOCLIP), new GUIContent("Audio"), GUILayout.MinWidth(20));
                EditorGUILayout.PropertyField(serializedObject.FindProperty(VP_DialogSetup.Fields.DIALOG_NODE_WAIT_AUDIO_END), new GUIContent("Wait for Audio End"), GUILayout.MinWidth(20));
                GUILayout.Space(15);

                if (node.answers.Count > 0)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(VP_DialogSetup.Fields.DIALOG_NODE_DEFAULT_ANSWER), new GUIContent("Auto-Answer"));

                    bool autoAnswer = node.m_autoAnswer > 0;
                    if (autoAnswer)
                        EditorGUILayout.PropertyField(serializedObject.FindProperty(VP_DialogSetup.Fields.DIALOG_NODE_TIME_TO_ANSWER), new GUIContent("Time To Answer"));

                    EditorGUILayout.PropertyField(serializedObject.FindProperty(VP_DialogSetup.Fields.DIALOG_NODE_TEXT_AND_ANSWERS), new GUIContent("Text and Answers"), GUILayout.MinWidth(20));

                }
                NodeEditorGUILayout.DynamicPortList(VP_DialogSetup.Fields.DIALOG_NODE_ANSWERS, typeof(VP_DialogBaseNode), serializedObject, NodePort.IO.Output, Node.ConnectionType.Multiple);

            }

            serializedObject.ApplyModifiedProperties();
        }

        public override int GetWidth()
        {
            return 300;
        }

        public override Color GetTint()
        {
            VP_Dialog node = target as VP_Dialog;

            if (node.IsCurrent)
                return SELECTED_COLOR;

            if (node.characterData == null || node.overrideColor)
            {
                return base.GetTint();
            }
            else
            {
                Color col = node.characterData.color;
                col.a = 1;
                return col;
            }
        }

        public void OnDropObjects(Object[] objects)
        {
       
        }
    }
}