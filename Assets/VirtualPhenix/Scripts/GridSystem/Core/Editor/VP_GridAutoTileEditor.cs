#if USE_GRID_SYSTEM && USE_TILEMAP
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace VirtualPhenix.GridEngine
{
    [CustomEditor(typeof(VP_GridAutoTileData), true)]
    public class VP_GridAutoTileEditor : Editor
    {
        private UnityEditorInternal.ReorderableList _editor_rules;


        public override void OnInspectorGUI()
        {
            var tile = target as VP_GridAutoTileData;
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            GUILayout.Label("MainPrefab");
            tile.m_mainPrefab = (GameObject)EditorGUILayout.ObjectField(tile.m_mainPrefab, typeof(GameObject), false, GUILayout.Height(80));

            GUILayout.EndVertical();
            GUILayout.Box(AssetPreview.GetAssetPreview(tile.m_mainPrefab) ?? null, GUILayout.Width(100), GUILayout.Height(100));
            GUILayout.EndHorizontal();

            tile.m_size = EditorGUILayout.Vector2IntField("Size", tile.m_size);
            if (GUI.changed)
            {
                if (tile.m_size.x < 1)
                    tile.m_size.x = 1;
                if (tile.m_size.y < 1)
                    tile.m_size.y = 1;
                EditorUtility.SetDirty(tile);
            }

            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_isBorderFriendly"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_friendlyTiles"));
            GUILayout.Label("Rules");
            _editor_rules.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }


        public void OnEnable()
        {
            _editor_rules = new UnityEditorInternal.ReorderableList(serializedObject, serializedObject.FindProperty("m_rules"), true, true, true, true);
            _editor_rules.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = _editor_rules.serializedProperty.GetArrayElementAtIndex(index);
                //rect.y += 2;
                EditorGUI.PropertyField(new Rect(rect.x, rect.y, 120, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("m_prefab"), GUIContent.none);
                var reference = element.FindPropertyRelative("m_prefab").objectReferenceValue;
                if (reference)
                {
                    try
                    {
                        EditorGUI.DrawPreviewTexture(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight, 120, rect.height - EditorGUIUtility.singleLineHeight * 2), AssetPreview.GetAssetPreview(reference));
                    }
                    catch
                    {
                    }
                }
                EditorGUI.LabelField(new Rect(rect.x + 130, rect.y, 50, EditorGUIUtility.singleLineHeight), "Match+");
                EditorGUI.PropertyField(new Rect(rect.x + 183, rect.y, 80, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("m_flipType"), GUIContent.none);

                //EditorGUI.PropertyField(new Rect(rect.x + 140, rect.y, 120, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("NeighbourRules"), GUIContent.none);
                var list = element.FindPropertyRelative("m_neighbourRules");
                var size = element.FindPropertyRelative("m_neighbourRules").arraySize;
                if (size != 9)
                {
                    list.ClearArray();
                    list.arraySize = 9;
                }
                rect = new Rect(rect.x + 130, rect.y + 10 + EditorGUIUtility.singleLineHeight, rect.width - 140, rect.height);
                NeighbourField(new Rect(rect.x, rect.y, 30, 30), list.GetArrayElementAtIndex(0));
                NeighbourField(new Rect(rect.x + 32, rect.y, 30, 30), list.GetArrayElementAtIndex(1));
                NeighbourField(new Rect(rect.x + 64, rect.y, 30, 30), list.GetArrayElementAtIndex(2));

                NeighbourField(new Rect(rect.x, rect.y + 32, 30, 30), list.GetArrayElementAtIndex(3));
                list.GetArrayElementAtIndex(4).intValue = 0;
                NeighbourField(new Rect(rect.x + 64, rect.y + 32, 30, 30), list.GetArrayElementAtIndex(5));

                NeighbourField(new Rect(rect.x, rect.y + 64, 30, 30), list.GetArrayElementAtIndex(6));
                NeighbourField(new Rect(rect.x + 32, rect.y + 64, 30, 30), list.GetArrayElementAtIndex(7));
                NeighbourField(new Rect(rect.x + 64, rect.y + 64, 30, 30), list.GetArrayElementAtIndex(8));
            };
            _editor_rules.elementHeight = 140;
        }

        bool NeighbourField(Rect rect, SerializedProperty neighbour)
        {
            if (GUI.Button(new Rect(rect.x, rect.y, rect.width, rect.height), NeighbourToString((GRID_AUTO_TILE_NEIGHBOUR_TYPES)neighbour.intValue)))
            {
                neighbour.intValue++;
                if (neighbour.intValue > 2)
                    neighbour.intValue = 0;
            }
            return true;
        }

        string NeighbourToString(GRID_AUTO_TILE_NEIGHBOUR_TYPES type)
        {
            switch (type)
            {
                case GRID_AUTO_TILE_NEIGHBOUR_TYPES.THIS:
                    return "O";
                case GRID_AUTO_TILE_NEIGHBOUR_TYPES.NOT_THIS:
                    return "X";
            }
            return "";
        }
    }
}
#endif