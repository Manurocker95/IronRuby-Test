using UnityEditor;
using UnityEngine;
using XNode;
using XNodeEditor;

namespace VirtualPhenix.Dialog
{
    [CustomNodeEditor(typeof(VP_DialogMultipleOutputs))]
    public class VP_DialogMultipleOutputsEditor : VP_DialogBaseNodeEditor
    {
        public override void OnHeaderGUI()
        {
            GUILayout.Label("Sequence", NodeEditorResources.styles.nodeHeader, GUILayout.Height(30));
        }

        public override void OnBodyGUI()
        {
            serializedObject.Update();

            VP_DialogMultipleOutputs node = target as VP_DialogMultipleOutputs;

            if (node == null)
                return;

            if (node.outputs.Count == 0)
            {
                GUILayout.BeginHorizontal();
                NodeEditorGUILayout.PortField(GUIContent.none, target.GetInputPort(VP_DialogSetup.Fields.DIALOG_NODE_INPUT), GUILayout.MinWidth(0));
                NodeEditorGUILayout.PortField(GUIContent.none, target.GetOutputPort(VP_DialogSetup.Fields.DIALOG_NODE_OUTPUT), GUILayout.MinWidth(0));
                GUILayout.EndHorizontal();
                GUILayout.Space(30);
                NodeEditorGUILayout.DynamicPortList(VP_DialogSetup.Fields.DIALOG_NODE_MULTIPLE_OUTPUT, typeof(VP_DialogBaseNode), serializedObject, NodePort.IO.Output, Node.ConnectionType.Override);

                GUILayout.Space(30);
            }
            else
            {
                GUILayout.BeginHorizontal();
                NodeEditorGUILayout.PortField(GUIContent.none, target.GetInputPort(VP_DialogSetup.Fields.DIALOG_NODE_INPUT), GUILayout.MinWidth(0));
                GUILayout.EndHorizontal();
                GUILayout.Space(30);
                NodeEditorGUILayout.DynamicPortList(VP_DialogSetup.Fields.DIALOG_NODE_MULTIPLE_OUTPUT, typeof(VP_DialogBaseNode), serializedObject, NodePort.IO.Output, Node.ConnectionType.Override);

                GUILayout.Space(30);
            }
            GUILayout.Space(-30);
   
            serializedObject.ApplyModifiedProperties();
        }

        public override int GetWidth()
        {
            return 200;
        }


        public override Color GetTint()
        {
            VP_DialogMultipleOutputs node = target as VP_DialogMultipleOutputs;

            if (node.IsCurrent)
                return SELECTED_COLOR;

            return base.GetTint();
        }

        public void OnDropObjects(Object[] objects)
        {

        }
    }
}


