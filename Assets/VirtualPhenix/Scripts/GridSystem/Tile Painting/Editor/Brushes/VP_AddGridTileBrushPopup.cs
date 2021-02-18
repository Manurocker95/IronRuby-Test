#if USE_GRID_SYSTEM && USE_TILEMAP
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.Tilemaps;

namespace VirtualPhenix.GridEngine
{
    public class VP_AddGridTileBrushPopup : VP_EditorWindow<VP_AddGridTileBrushPopup>
    {
        public List<VP_GridTileBrush> m_brushes;

        protected VP_GridTileBrush m_gridTileBrushToAdd = new VP_GridTileBrush();

        public override string WindowName { get { return "Add GridTile Brush"; } }

        public static void Initialize(List<VP_GridTileBrush> _brushes)
        {
            if (Instance != null)
            {
                return;
            }

            m_instance = (VP_AddGridTileBrushPopup)EditorWindow.GetWindowWithRect(typeof(VP_AddGridTileBrushPopup), new Rect(0, 0, 400, 220));//ScriptableObject.CreateInstance<AddGridTileBrushPopup>();
            GUIContent titleContent = new GUIContent(Instance.WindowName);
            Instance.titleContent = titleContent;
            Instance.m_brushes = _brushes;
            Instance.ShowUtility();
            Instance.Repaint();
        }

        public override void CreateHowToText()
        {
            if (m_howToTexts == null)
                InitHowToTextList();

            CreateHowToTitle(ref m_howToTexts, "ADD GRID TILE");

            m_howToTexts.Add(new VP_HTUText()
            {
                Text = "Drag the prefab with VP_GridTile component attached to\n" +
                "the field \"GridTile\". Configure it, and click Play.\n\n" +
                "It will be added to GridTile Brush collection.",
                m_labelType = VP_EditorWindowStyleSetup.RegularTextStyle,
                m_spaces = new KeyValuePair<bool, int>(true, 2)
            });

            VP_HowToUseWindow.ShowWindow(HowToWindowWidth, HowToWindowHeight, m_howToTexts.ToArray());
        }


        protected override void OnGUI()
        {
            base.OnGUI();
            EditorGUILayout.Space();
            m_gridTileBrushToAdd.m_gridTile = (VP_GridTile)EditorGUILayout.ObjectField("GridTile Prefab", m_gridTileBrushToAdd.m_gridTile, typeof(VP_GridTile), false);
            m_gridTileBrushToAdd.m_layer = EditorGUILayout.IntField("Tile Layer", m_gridTileBrushToAdd.m_layer);
            m_gridTileBrushToAdd.m_layer = Mathf.Clamp(m_gridTileBrushToAdd.m_layer, 0, m_gridTileBrushToAdd.m_layer);
            m_gridTileBrushToAdd.m_height = EditorGUILayout.IntField("Tile Height", m_gridTileBrushToAdd.m_height);
            m_gridTileBrushToAdd.m_tileType = (GRID_TILE_TYPES)EditorGUILayout.EnumPopup("Tile Type", m_gridTileBrushToAdd.m_tileType);
            m_gridTileBrushToAdd.m_offset = EditorGUILayout.Vector3Field("Position Offset", m_gridTileBrushToAdd.m_offset);
            m_gridTileBrushToAdd.m_rotation = EditorGUILayout.Vector3Field("Rotation Offset", m_gridTileBrushToAdd.m_rotation);
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();

            GUI.backgroundColor = VP_TilePaletteGridTileBrushEditor.red;
            if (GUILayout.Button("Cancel"))
            {
                this.Close();
            }

            GUI.backgroundColor = VP_TilePaletteGridTileBrushEditor.green;
            if (GUILayout.Button("Add"))
            {
                if (m_gridTileBrushToAdd != null && m_gridTileBrushToAdd.m_gridTile != null && PrefabUtility.IsPartOfAnyPrefab(m_gridTileBrushToAdd.m_gridTile.gameObject))
                {
                    m_brushes.Add(new VP_GridTileBrush(m_gridTileBrushToAdd));

                    if (VP_TilePaletteGridTileBrushEditor.Instance != null)
                    {
                        EditorUtility.SetDirty(VP_TilePaletteGridTileBrushEditor.Instance);
                        VP_TilePaletteGridTileBrushEditor.Instance.Collection.Save();
                    }
                }
                this.Close();
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