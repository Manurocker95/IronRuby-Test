using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XNodeEditor;

namespace VirtualPhenix.Dialog
{
    /// <summary> Base class to derive custom Node editors from. Use this to create your own custom inspectors and editors for your nodes. </summary>
    [CustomNodeEditor(typeof(VP_DialogBaseNode))]
    public class VP_DialogBaseNodeEditor : NodeEditor
    {
        public override void OnHeaderGUI()
        {
            GUILayout.Label((target as VP_DialogBaseNode).m_renamed ? target.name : "Dialog Node", NodeEditorResources.styles.nodeHeader, GUILayout.Height(30));
        }

        public override Color GetTint()
        {
            VP_DialogBaseNode node = target as VP_DialogBaseNode;

            if (node.IsCurrent)
                return SELECTED_COLOR;

            return node.overrideColor ? (node.overrideNodeColor) : base.GetTint();
        }

        public override void AddContextMenuItems(GenericMenu menu)
        {
            VP_DialogGraph graph = target.graph as VP_DialogGraph;

            if (graph.Preset != null)
            {
                // Get selected nodes which are part of this graph
                XNode.Node[] selectedNodes = Selection.objects.Select(x => x as XNode.Node).Where(x => x != null && x.graph == graph).ToArray();

                menu.AddItem(new GUIContent("Set Preset Values"), false, () => { SetSelectedNodespresetValues(graph, selectedNodes); });
            }

            base.AddContextMenuItems(menu);
        }

        /// <summary>
        /// Can set the preset values 
        /// </summary>
        public void SetSelectedNodespresetValues(VP_DialogGraph graph, XNode.Node[] selectedNodes)
        {
            foreach (XNode.Node node in selectedNodes)
            {
                if (node is VP_Dialog)
                {
                    (node as VP_Dialog).SetValuesByPreset(graph.Preset);
                }
            }
        }

    }
}