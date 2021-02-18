using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XNodeEditor;

namespace VirtualPhenix.Dialog
{


    [CustomNodeEditor(typeof(VP_DialogRandomNumberNode))]
    public class VP_DialogRandomNumberNodeEditor : VP_DialogBaseNodeEditor
    {

        public override void OnHeaderGUI()
        {
            GUILayout.Label("Random Number", NodeEditorResources.styles.nodeHeader, GUILayout.Height(30));
        }

        public override void OnBodyGUI()
        {
            serializedObject.Update();

            VP_DialogRandomNumberNode node = target as VP_DialogRandomNumberNode;

            if (node == null)
                return;

            NodeEditorGUILayout.PortField(target.GetInputPort(VP_DialogSetup.Fields.DIALOG_NODE_INPUT), GUILayout.Width(100));
            NodeEditorGUILayout.PortField(target.GetOutputPort(VP_DialogSetup.Fields.DIALOG_NODE_OUTPUT), GUILayout.MinWidth(0));
            EditorGUILayout.Space();

            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("m_numberType"));


            EditorGUILayout.Space();
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("m_minByVariable"));
            if (node.MinByVariable)
            {
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("m_minVariable"));
            }
            else
            {
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(VP_DialogSetup.Fields.CHOOSE_NUMBER_NODE_MIN));
            }
            EditorGUILayout.Space();
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("m_maxByVariable"));

            if (node.MaxByVariable)
            {
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("m_maxVariable"));
            }
            else
            {
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(VP_DialogSetup.Fields.CHOOSE_NUMBER_NODE_MAX));
            }

            serializedObject.ApplyModifiedProperties();
        }


        public override int GetWidth()
        {
            return 226;
        }

        public override Color GetTint()
        {
            return base.GetTint();
        }
    }
}