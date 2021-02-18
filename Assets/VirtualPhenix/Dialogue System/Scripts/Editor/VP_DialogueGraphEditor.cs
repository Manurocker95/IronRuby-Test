using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XNodeEditor;

namespace VirtualPhenix.Dialog
{
	[CustomNodeGraphEditor(typeof(VP_DialogGraph))]
	public class VP_DialogueGraphEditor : NodeGraphEditor
    { 
        // The position of the window
        public Rect windowRect = new Rect(10, 30, 200, 200);
        public Rect windowSmallRect = new Rect(10, 30, 20, 20);

        private Rect menuBar;   
        private float menuBarHeight = 20f;
        private float zoom;
        private string assetName;
        private bool m_showingVariables;

        static int id;

        public static int GetID { get { return id; } }

        [SerializeField] public VP_DialogGraph graph;

        [MenuItem("Virtual Phenix/Window/Dialogue Editor %F4")]
        public static void OpenWindow()
        {
            if (id == 0)
            {
                EditorUtility.DisplayDialog("There's no Dialogue Graph already opened.", "Create a new Dialogue Graph by Right Clicking in project tab > Create > Virtual Phoenix > Dialogue System > Dialogue Graph, and double click on it.", "Okay");
            }
            else
            {
                NodeEditorWindow.OnOpen(id, 0);
            }
        }


        public override void OnOpen()
        {
            base.OnOpen();
            assetName = target.name;
            id = target.GetInstanceID();

            graph = (target as VP_DialogGraph);

            graph.RefreshNodeInit();

            FillContextMenu();
        }

        public void FillContextMenu()
        {
            NodeEditorWindow.m_ctxMenuList.Clear();

            for (int i = 0; i < NodeEditorWindow.nodeTypes.Length; i++)
            {
                Type type = NodeEditorWindow.nodeTypes[i];

                //Get node context menu path
                string path = GetNodeMenuName(type);
                if (string.IsNullOrEmpty(path)) 
                    continue;

                NodeEditorWindow.m_ctxMenuList.Add(new NodeWindowContextMenuData(new GUIContent("Create Node/"+path), false, () => {
                    CreateNode(type, Vector2.zero);
                }));
            }

            if (NodeEditorWindow.copyBuffer != null && NodeEditorWindow.copyBuffer.Length > 0)
                NodeEditorWindow.m_ctxMenuList.Add(new NodeWindowContextMenuData(new GUIContent("Paste"), false, () => NodeEditorWindow.current.PasteNodes(Vector2.zero)));

            NodeEditorWindow.m_ctxMenuList.Add(new NodeWindowContextMenuData(new GUIContent("Preferences"), false, () => NodeEditorWindow.OpenPreferences()));
        }

        public override string GetPreferencesName()
        {
            return "Dialogue Editor";
        }

        public override string GetNodeMenuName(System.Type type)
        {
            // TODO THIS NAMESPACE IS IMPORTANT
			if (type.Namespace == "VirtualPhenix.Dialog")
                return base.GetNodeMenuName(type).Replace("VirtualPhenix.Dialog/", "");
			else
                return null;
		}

        public override void OnDropObjects(UnityEngine.Object[] objects)
        {
            
        }

        public override void OnDropObjects(UnityEngine.Object[] objects, XNode.Node node)
        {
            VP_DialogBaseNode baseNode = (VP_DialogBaseNode)node as VP_DialogBaseNode;
            baseNode.OnDropObjects(objects);
        }

        public override void OnGUI()
        {
            base.OnGUI();
            DrawMenuBar();        
        }

        private void DrawMenuBar()
        {
            menuBar = new Rect(0, 0, window.position.width, menuBarHeight);

            GUILayout.BeginArea(menuBar, EditorStyles.toolbar);
            GUILayout.BeginHorizontal();
            if (graph != null)
            {
                if (GUILayout.Button(new GUIContent("Graph: " + assetName), EditorStyles.toolbarButton, GUILayout.Width(195)))
                {
                    UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(AssetDatabase.GetAssetPath(graph), typeof(UnityEngine.Object));
                    Selection.activeObject = obj;
                    EditorGUIUtility.PingObject(obj);
                }
            }
            else
            {
                GUILayout.Label(new GUIContent("No Graph selected"), EditorStyles.toolbarButton, GUILayout.Width(165));
            }
               
            GUILayout.Space(5);
            if (graph.Preset != null)
            {
                if (GUILayout.Button(new GUIContent("Preset: " + graph.Preset.name), EditorStyles.toolbarButton, GUILayout.Width(195)))
                {
                    UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(AssetDatabase.GetAssetPath(graph.Preset), typeof(UnityEngine.Object));
                    Selection.activeObject = obj;
                    EditorGUIUtility.PingObject(obj);
                }

                if (GUILayout.Button(new GUIContent("Set Preset Values"), EditorStyles.toolbarButton, GUILayout.Width(195)))
                {
                    foreach (XNode.Node node in graph.nodes)
                    {
                        if (node is VP_Dialog)
                        {
                            (node as VP_Dialog).SetValuesByPreset(graph.Preset);
                        }
                    }
                }
            }
            GUILayout.Space(5);
            if (GUILayout.Button(new GUIContent("Graph Variables"), EditorStyles.toolbarButton, GUILayout.Width(115)))
            {
                m_showingVariables = !m_showingVariables;
                VP_VariableEditorWindow.InitWindowWithGraph(graph);
            }
            GUILayout.Space(5);
            if (GUILayout.Button(new GUIContent("Preferences"), EditorStyles.toolbarButton, GUILayout.Width(85)))
            {
                SettingsService.OpenUserPreferences("Preferences/Dialogue Editor");
            }
  

            GUILayout.Space(5);
            if (GUILayout.Button(new GUIContent("Save"), EditorStyles.toolbarButton, GUILayout.Width(65)))
            {
                serializedObject.ApplyModifiedProperties();
                serializedObject.Update();
                AssetDatabase.SaveAssets();
            }
         
            GUILayout.Space(5);
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

     
    }
}