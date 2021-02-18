#if USE_GRID_SYSTEM && USE_TILEMAP
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.Tilemaps;

namespace VirtualPhenix.GridEngine
{
    public class VP_AddGridObjectBrushPopup : VP_EditorWindow<VP_AddGridObjectBrushPopup>
    {
        public List<VP_GridObjectBrush> m_brushes;

        public override string WindowName { get { return "Add GridObject Brush"; } }
        protected VP_GridObjectBrush m_gridObjectBrushToAdd = new VP_GridObjectBrush();

        public static void Initialize(List<VP_GridObjectBrush> m_Brushes)
        {
            
            if (Instance != null)
            {
                return;
            }

            m_instance = (VP_AddGridObjectBrushPopup)EditorWindow.GetWindowWithRect(typeof(VP_AddGridObjectBrushPopup), new Rect(0, 0, 400, 220));
            GUIContent titleContent = new GUIContent(Instance.WindowName);
            Instance.titleContent = titleContent;
            Instance.m_brushes = m_Brushes;
            Instance.ShowUtility();
            Instance.Repaint();
        }

        public override void CreateHowToText()
        {
            if (m_howToTexts == null)
                InitHowToTextList();

            CreateHowToTitle(ref m_howToTexts, "ADD GRID OBJECT");

            m_howToTexts.Add(new VP_HTUText()
            {
                Text = "Drag the prefab with VP_GridObject component attached to\n" +
                "the field \"GridObject Prefab\". Configure it, and click Play.\n\n" +
                "It will be added to GridObject Brush collection.",
                m_labelType = VP_EditorWindowStyleSetup.RegularTextStyle,
                m_spaces = new KeyValuePair<bool, int>(true, 2)
            });

            VP_HowToUseWindow.ShowWindow(HowToWindowWidth, HowToWindowHeight, m_howToTexts.ToArray());
        }


        protected override void OnGUI()
        {
            EditorGUILayout.Space();
            m_gridObjectBrushToAdd.m_GridObject = (VP_GridObject)EditorGUILayout.ObjectField("GridObject Prefab", m_gridObjectBrushToAdd.m_GridObject, typeof(VP_GridObject), false);
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();

            GUI.backgroundColor = VP_RemakeGridObjectBrushEditor.red;
            if (GUILayout.Button("Cancel"))
            {
                this.Close();
            }

            GUI.backgroundColor = VP_RemakeGridObjectBrushEditor.green;
            if (GUILayout.Button("Add"))
            {
                if (m_gridObjectBrushToAdd != null && m_gridObjectBrushToAdd.m_GridObject != null && PrefabUtility.IsPartOfAnyPrefab(m_gridObjectBrushToAdd.m_GridObject.gameObject))
                {
                    m_brushes.Add(new VP_GridObjectBrush(m_gridObjectBrushToAdd));

                    if (VP_RemakeGridObjectBrushEditor.Instance != null)
                    {
                        EditorUtility.SetDirty(VP_RemakeGridObjectBrushEditor.Instance);
                        VP_RemakeGridObjectBrushEditor.Instance.Collection.Save();
                    }
                }
                Instance.Close();
            }
            EditorGUILayout.EndHorizontal();
        }

        protected override void OnDestroy()
        {
            if (m_instance == this)
            {
                m_instance = null;
            }
        }
    }
}
#endif