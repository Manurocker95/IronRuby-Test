#if USE_GRID_SYSTEM && USE_TILEMAP
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NaughtyAttributes;

namespace VirtualPhenix.GridEngine
{
    [CreateAssetMenu(fileName = "New GridObject Collection", menuName = "Virtual Phenix/Grid System/Collections/Create GridObject Collection")]
    public class VP_GridObjectCollection : VP_ScriptableObject
    {
        [SerializeField, Header("GridObjects In Collection")] private List<VP_GridObjectBrush> m_gridObjectBrushes = new List<VP_GridObjectBrush>();
        public List<VP_GridObjectBrush> GridObjectBrushes
        {
            get
            {
                if (m_gridObjectBrushes == null)
                {
                    m_gridObjectBrushes = new List<VP_GridObjectBrush>();
                }
                return m_gridObjectBrushes;
            }
            set
            {
                if (value == null)
                {
                    value = new List<VP_GridObjectBrush>();
                }
                m_gridObjectBrushes = value;
            }
        }

        public VP_GridObjectBrush m_SelectedGridObjectBrush
        {
            get { return GridObjectBrushes.Count > 0 && m_SelectedGridObjectBrushIndex != -1 && m_gridObjectBrushes.Count > m_SelectedGridObjectBrushIndex ? m_gridObjectBrushes[m_SelectedGridObjectBrushIndex] : null; }
        }

        [NaughtyAttributes.ReadOnly]
        public int m_SelectedGridObjectBrushIndex = 0;

        public void RemoveObject(VP_GridObjectBrush objectBrush)
        {
            if (m_gridObjectBrushes.Contains(objectBrush))
            {
                m_gridObjectBrushes.Remove(objectBrush);
            }
        }

        public void RemoveAllObjects()
        {
            m_gridObjectBrushes.Clear();
        }

        public void Save()
        {
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public string GetGUID()
        {
            return AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(this));
        }

        public int GetIndex()
        {
            string[] guids = GetAllGridObjectCollectionGUIDs();
            for (int i = 0; i < guids.Length; i++)
            {
                if (guids[i] == GetGUID())
                {
                    return i;
                }
            }
            return 0;
        }

        public void SetLastUsedGridObjectCollection()
        {
            EditorPrefs.SetString(VP_GridTileMapperSetup.LAST_GRID_OBJECT_COLLECTION_EDIT_PLAYER_PREF_KEY, GetGUID());
        }

        public static int GetLastUsedGridObjectCollectionIndex()
        {
            //try to find the last used brush collection and return it
            if (EditorPrefs.HasKey(VP_GridTileMapperSetup.LAST_GRID_OBJECT_COLLECTION_EDIT_PLAYER_PREF_KEY))
            {
                string guid = EditorPrefs.GetString(VP_GridTileMapperSetup.LAST_GRID_OBJECT_COLLECTION_EDIT_PLAYER_PREF_KEY, "");
                string path = AssetDatabase.GUIDToAssetPath(guid);
                VP_GridObjectCollection lastUsedCollection = AssetDatabase.LoadAssetAtPath<VP_GridObjectCollection>(path);
                return lastUsedCollection == null ? 0 : lastUsedCollection.GetIndex();
            }
            else
            {
                return 0;
            }
        }

        public static string[] GetAllGridObjectCollectionGUIDs()
        {
            return AssetDatabase.FindAssets("t:"+ typeof(VP_GridObjectCollection).ToString());
        }

        public static GridObjectCollectionList GetGridObjectCollectionsInProject()
        {
            string[] guids = GetAllGridObjectCollectionGUIDs();
            return new GridObjectCollectionList(guids);
        }

        public struct GridObjectCollectionList
        {
            public List<VP_GridObjectCollection> brushCollections;

            public GridObjectCollectionList(string[] guids)
            {
                brushCollections = new List<VP_GridObjectCollection>();
                for (int i = 0; i < guids.Length; i++)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                    brushCollections.Add(AssetDatabase.LoadAssetAtPath<VP_GridObjectCollection>(path));
                }
            }

            public string[] GetNameList()
            {
                List<string> names = new List<string>();
                for (int i = 0; i < brushCollections.Count; i++)
                {
                    if (brushCollections[i] != null)
                    {
                        names.Add(brushCollections[i].name);
                    }
                    else
                    {
                        brushCollections.Remove(brushCollections[i]);
                    }
                }
                return names.ToArray();
            }
        }
    }


}
#endif