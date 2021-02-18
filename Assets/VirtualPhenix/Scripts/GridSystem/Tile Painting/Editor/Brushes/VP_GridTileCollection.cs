#if USE_GRID_SYSTEM && USE_TILEMAP
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NaughtyAttributes;

namespace VirtualPhenix.GridEngine
{
    [CreateAssetMenu(fileName = "New GridTile Collection", menuName = "Virtual Phenix/Grid System/Collections/Create GridTile Collection")]
    public class VP_GridTileCollection : VP_ScriptableObject
    {
        protected static string lastGridTileCollectionEditPrefsKey = "LastUsedGTC";

        [SerializeField, Header("Tiles In Collection")] private List<VP_GridTileBrush> m_gridTileBrushes = new List<VP_GridTileBrush>();
        public virtual List<VP_GridTileBrush> GridTileBrushes
        {
            get
            {
                if (m_gridTileBrushes == null)
                {
                    m_gridTileBrushes = new List<VP_GridTileBrush>();
                }
                return m_gridTileBrushes;
            }
            set
            {
                if (value == null)
                {
                    value = new List<VP_GridTileBrush>();
                }
                m_gridTileBrushes = value;
            }
        }

        public virtual VP_GridTileBrush SelectedGridTileBrush
        {
            get 
            { 
                return GridTileBrushes.Count > 0 && m_selectedGridTileBrushIndex != -1 && GridTileBrushes.Count > m_selectedGridTileBrushIndex ? GridTileBrushes[m_selectedGridTileBrushIndex] : null; 
            }
        }

        public int m_selectedGridTileBrushIndex = 0;

        public virtual void RemoveTile(VP_GridTileBrush tileBrush)
        {
            if (GridTileBrushes.Contains(tileBrush))
            {
                GridTileBrushes.Remove(tileBrush);
            }
        }

        public virtual void RemoveAllTiles()
        {
            GridTileBrushes.Clear();
        }

        public virtual void Save()
        {
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public virtual string GetGUID()
        {
            return AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(this));
        }

        public virtual int GetIndex()
        {
            string[] guids = GetAllGridTileCollectionGUIDs();
            for (int i = 0; i < guids.Length; i++)
            {
                if (guids[i] == GetGUID())
                {
                    return i;
                }
            }
            return 0;
        }

        public virtual void SetLastUsedGridTileCollection()
        {
            EditorPrefs.SetString(lastGridTileCollectionEditPrefsKey, GetGUID());
        }

        public static int GetLastUsedGridTileCollectionIndex()
        {
            //try to find the last used brush collection and return it
            if (EditorPrefs.HasKey(lastGridTileCollectionEditPrefsKey))
            {
                string guid = EditorPrefs.GetString(lastGridTileCollectionEditPrefsKey, "");
                string path = AssetDatabase.GUIDToAssetPath(guid);
                VP_GridTileCollection lastUsedCollection = AssetDatabase.LoadAssetAtPath<VP_GridTileCollection>(path);
                return lastUsedCollection != null ? lastUsedCollection.GetIndex() : 0;
            }
            else
            {
                return 0;
            }
        }

        public static string[] GetAllGridTileCollectionGUIDs()
        {
            return AssetDatabase.FindAssets("t:"+ typeof(VP_GridTileCollection).ToString());
        }

        public static GridTileCollectionList GetBrushCollectionsInProject()
        {
            string[] guids = GetAllGridTileCollectionGUIDs();
            return new GridTileCollectionList(guids);
        }

        public struct GridTileCollectionList
        {
            public List<VP_GridTileCollection> brushCollections;

            public GridTileCollectionList(string[] guids)
            {
                brushCollections = new List<VP_GridTileCollection>();
                for (int i = 0; i < guids.Length; i++)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                    brushCollections.Add(AssetDatabase.LoadAssetAtPath<VP_GridTileCollection>(path));
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