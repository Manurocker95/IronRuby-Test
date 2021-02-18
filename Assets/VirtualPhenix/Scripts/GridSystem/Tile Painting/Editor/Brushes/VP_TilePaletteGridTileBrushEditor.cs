#if USE_GRID_SYSTEM && USE_TILEMAP
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using VirtualPhenix.GridEngine;
using VirtualPhenix;

namespace UnityEditor.Tilemaps
{
    [CustomEditor(typeof(VP_TilePaletteGridTileBrush))]
    public class VP_TilePaletteGridTileBrushEditor : VP_GridBrushEditorBase
    {
        // Target brush
        public VP_TilePaletteGridTileBrush GridTileBrush { get { return target as VP_TilePaletteGridTileBrush; } }
        // Editor Preview Collection/Selection
        protected Vector2 m_scrollViewScrollPosition = new Vector2();
        public VP_GridTileCollection m_collection;
        public bool m_saveCollectionOnChange;
        public int m_selectedGridTileCollectionIndex = 0;
        // OnScene Preview
        protected int m_lastPreviewRefreshHash;
        protected GridLayout m_lastGrid;
        protected GameObject m_lastBrushTarget;
        protected BoundsInt? m_lastBounds;
        protected int m_selectIndex;

        protected GridBrushBase.Tool? m_lastTool;
        protected VP_GridTileCollection.GridTileCollectionList m_gridTileCollectionList;
        // Editor cached colors
        public static Color red = VP_GridUtilities.ColorFromRGB(239, 80, 80);
        public static Color green = VP_GridUtilities.ColorFromRGB(93, 173, 57);
        public static Color PrimarySelectedColor = VP_GridUtilities.ColorFromRGB(10, 153, 220);
        public static VP_TilePaletteGridTileBrushEditor Instance { get; private set; }
        public VP_GridTileCollection Collection { get { return m_collection; } }

        void OnEnable()
        {
            Instance = this;
            Undo.undoRedoPerformed += ClearLastPreview;
            ReloadCollections();
            m_selectedGridTileCollectionIndex = VP_GridTileCollection.GetLastUsedGridTileCollectionIndex();
        }

        void OnDestroy()
        {
            Undo.undoRedoPerformed -= ClearLastPreview;
        }

        private void ClearLastPreview()
        {
            ClearPreviewAll();
            m_lastPreviewRefreshHash = 0;
        }

        public bool CreateCollection(out VP_GridTileCollection _collection, string _name = "new SO")
        {
            string whereToSaveAll = VP_Utils.GetProjectAssetsFolderToSave("Choose the folder where to save the Scriptable Object");

            if (whereToSaveAll.IsNullOrEmpty())
            {
                _collection = null;
                return false;
            }

            string assetPathAndName = UnityEditor.AssetDatabase.GenerateUniqueAssetPath(whereToSaveAll + "/" + _name + ".asset");

            _collection = ScriptableObject.CreateInstance<VP_GridTileCollection>();
            UnityEditor.AssetDatabase.CreateAsset(_collection, assetPathAndName);

            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
            UnityEditor.EditorUtility.FocusProjectWindow();

            return true;
        }

        public bool LoadCollection(out VP_GridTileCollection _collection)
        {
            string whereToSaveAll = VP_Utils.ChooseFilePathInsideAssets("Choose the Grid Tile Collection");
            if (whereToSaveAll.IsNullOrEmpty())
            {
                _collection = null;
                return false;
            }

            _collection = (VP_GridTileCollection)AssetDatabase.LoadAssetAtPath(whereToSaveAll, typeof(VP_GridTileCollection));
            return _collection != null;
        }

        public virtual void ReloadCollections()
        {
            m_gridTileCollectionList = VP_GridTileCollection.GetBrushCollectionsInProject();
        }

        [UnityEditor.MenuItem("Virtual Phenix/Grid System/Utils/Find Instances of GridTile")]
        public static void Find()
        {
            Debug.Log("Searching");
            var ls = VP_Utils.FindAssetsByType<VP_TilePaletteGridTileBrush>();
            Debug.Log(ls.Count);
            if (ls.Count > 0)
            {
                VP_Utils.PingObject(ls[0]);
            }
        }

        protected virtual void DeleteAllTilesInLayer()
        {
            var tiles = VP_GridManager.Instance.GetGridTilesGameObjectsAtLayerEditor(m_selectIndex);
            foreach (GameObject go in tiles)
                DestroyImmediate(go);
        }
        protected virtual void SelectAllTilesInCurrentLayer()
        {
            var tiles = VP_GridManager.Instance.GetGridTilesGameObjectsAtLayerEditor(m_selectIndex);
            Selection.objects = tiles.ToArray();
        }

        public override void OnPaintInspectorGUI()
        {
            if (GridTileBrush.PickedTile)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Currently Picked Tiles (Pick Tool)", EditorStyles.boldLabel);
                if (GUILayout.Button("Unpick Tiles"))
                {
                    GridTileBrush.ResetPick();
                    return;
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space(7.5f);

                int rowLength = 1;
                int maxRowLength = GridTileBrush.Size.x;
                var previewSize = Mathf.Min(((Screen.width - 35) / maxRowLength), 100);

                m_scrollViewScrollPosition = EditorGUILayout.BeginScrollView(m_scrollViewScrollPosition, false, false);

                if (maxRowLength < 1)
                {
                    maxRowLength = 1;
                }

                foreach (VP_GridTileBrushCell tileBrush in GridTileBrush.Cells)
                {
                    //check if row is longer than max row length
                    if (rowLength > maxRowLength)
                    {
                        rowLength = 1;
                        EditorGUILayout.EndHorizontal();
                    }
                    //begin row if rowLength == 1
                    if (rowLength == 1)
                    {
                        EditorGUILayout.BeginHorizontal();
                    }

                    GUIContent btnContent = tileBrush != null && tileBrush.GridTile != null ?
                    new GUIContent(AssetPreview.GetAssetPreview(tileBrush.GridTile.gameObject), tileBrush.GridTile.gameObject.name) :
                    new GUIContent("", "There is no tile at this position.");
                    if (GUILayout.Button(btnContent, GUILayout.Width(previewSize), GUILayout.Height(previewSize)))
                    {

                    }
                    rowLength++;
                }

                //check if row is longer than max row length
                if (rowLength > maxRowLength)
                {
                    rowLength = 1;
                    EditorGUILayout.EndHorizontal();
                }
                if (rowLength == 1)
                {
                    EditorGUILayout.BeginHorizontal();
                }

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndScrollView();
            }
            else
            { // If there is no tile picked show the collections GUI
#region GridTile Collection
                SerializedObject serializedObject_brushObject = null;
                int prevSelectedGridTileCollectionIndex = m_selectedGridTileCollectionIndex;
                if (m_collection != null)
                {
                    serializedObject_brushObject = new SerializedObject(m_collection);
                }
                
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Active GridTile Collection:");
                m_selectedGridTileCollectionIndex = EditorGUILayout.Popup(m_selectedGridTileCollectionIndex, m_gridTileCollectionList.GetNameList());
                if (prevSelectedGridTileCollectionIndex != m_selectedGridTileCollectionIndex || m_collection == null) //select only when brush collection changed or is null
                {
                    if (m_gridTileCollectionList.brushCollections != null && m_gridTileCollectionList.brushCollections.Count > 0)
                    {
                        m_collection = m_gridTileCollectionList.brushCollections[m_selectedGridTileCollectionIndex];
                        m_collection.SetLastUsedGridTileCollection();
                        GridTileBrush.ClearCellFromEditor();
                        var tileBrush = m_collection.SelectedGridTileBrush;
                        if (tileBrush != null)
                        {

                            GridTileBrush.SetCellFromEditor(Vector3Int.zero, tileBrush.m_gridTile, tileBrush.m_height, tileBrush.m_offset, Quaternion.Euler(tileBrush.m_rotation), tileBrush.m_layer, tileBrush.m_tileType);
                        }
                    }
                }

                if (GUILayout.Button("+"))
                {
                    Debug.Log("Create a new collection");

                    VP_GridTileCollection col = null;
                    if (CreateCollection(out col, "New Grid Tile Collection"))
                    {
                        m_collection = col;
                    }
                }
                if (GUILayout.Button("..."))
                {
                    if (m_collection == null)
                    {
                        Debug.Log("collection is null");
                    }

                    VP_Utils.PingObject(m_collection);
                }  
                
                if (GUILayout.Button("Reload Collections"))
                {
                    ReloadCollections();
                }
                /*
                if (GUILayout.Button("Find"))
                {
                    Debug.Log("Searching");
                    var ls = VP_Utils.FindAssetsByType<VP_TilePaletteGridTileBrush>("UnityEditor.Tilemaps.");

                    if (ls.Count > 0)
                    {
                        VP_Utils.PingObject(ls[0]);
                    }
                }
                */

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                m_selectIndex = EditorGUILayout.IntField(m_selectIndex, GUILayout.Width(50), GUILayout.Height(20));
                m_selectIndex = Mathf.Clamp(m_selectIndex, 0, m_selectIndex);
                if (GUILayout.Button("Select Tiles In Layer"))
                {
                    SelectAllTilesInCurrentLayer();
                }
                DrawButtonWithColor("Delete Tiles In Layer", DeleteAllTilesInLayer, Color.red, Color.white);
                
                EditorGUILayout.EndHorizontal();

                if (m_collection == null)
                    return;

                EditorGUILayout.Space(7.5f);
                int rowLength = 1;
                int maxRowLength = Mathf.FloorToInt((Screen.width - 15) / 100);
                int columns = Mathf.CeilToInt((m_collection.GridTileBrushes.Count / maxRowLength)) * 3;
                m_scrollViewScrollPosition = EditorGUILayout.BeginScrollView(m_scrollViewScrollPosition, false, false);

                if (maxRowLength < 1)
                {
                    maxRowLength = 1;
                }

                foreach (VP_GridTileBrush tileBrush in m_collection.GridTileBrushes)
                {
                    //check if brushObject is null, if so skip this brush
                    if (tileBrush == null || tileBrush.m_gridTile == null)
                    {
                        continue;
                    }

                    //check if row is longer than max row length
                    if (rowLength > maxRowLength)
                    {
                        rowLength = 1;
                        EditorGUILayout.EndHorizontal();
                    }
                    //begin row if rowLength == 1
                    if (rowLength == 1)
                    {
                        EditorGUILayout.BeginHorizontal();
                    }

                    //change color
                    Color guiColor = GUI.backgroundColor;
                    if (m_collection.SelectedGridTileBrush != null && m_collection.SelectedGridTileBrush.m_gridTile != null && m_collection.SelectedGridTileBrush.m_gridTile == tileBrush.m_gridTile)
                    {
                        GUI.backgroundColor = PrimarySelectedColor;
                        if (GridTileBrush.EditorCell != null && GridTileBrush.EditorCell.GridTile != tileBrush.m_gridTile)
                        {
                            GridTileBrush.SetCellFromEditor(Vector3Int.zero, tileBrush.m_gridTile, tileBrush.m_height, tileBrush.m_offset, Quaternion.Euler(tileBrush.m_rotation), tileBrush.m_layer, tileBrush.m_tileType);
                        }
                    }

                    //Create the brush entry in the scroll view and check if the user clicked on the created button (change the currently selected/edited brush accordingly and add it to the current brushes if possible)
                    GUIContent btnContent = new GUIContent(AssetPreview.GetAssetPreview(tileBrush.m_gridTile.gameObject), tileBrush.m_gridTile.gameObject.name);
                    if (GUILayout.Button(btnContent, GUILayout.Width(100), GUILayout.Height(100)))
                    {
                        //select the currently edited brush and deselect all selected brushes
                        if (m_collection.SelectedGridTileBrush != tileBrush)
                        {
                            m_collection.m_selectedGridTileBrushIndex = m_collection.GridTileBrushes.IndexOf(tileBrush);
                            GridTileBrush.SetCellFromEditor(Vector3Int.zero, tileBrush.m_gridTile, tileBrush.m_height, tileBrush.m_offset, Quaternion.Euler(tileBrush.m_rotation), tileBrush.m_layer, tileBrush.m_tileType);
                        }
                        else
                        {
                            m_collection.m_selectedGridTileBrushIndex = -1;
                            GridTileBrush.ClearCellFromEditor();
                        }
                    }
                    GUI.backgroundColor = guiColor;
                    rowLength++;
                }

                //check if row is longer than max row length
                if (rowLength > maxRowLength)
                {
                    rowLength = 1;
                    EditorGUILayout.EndHorizontal();
                }
                if (rowLength == 1)
                {
                    EditorGUILayout.BeginHorizontal();
                }
                //add button
                if (GUILayout.Button(new GUIContent("+", "Add a GridTile to the collection."), GUILayout.Width(100), GUILayout.Height(100)))
                {
                    VP_AddGridTileBrushPopup.Initialize(m_collection.GridTileBrushes);
                }
                Color guiBGColor = GUI.backgroundColor;
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndScrollView();

                EditorGUILayout.BeginHorizontal();
                GUI.backgroundColor = green;
                if (GUILayout.Button(new GUIContent("Add GridTile", "Add a GridTile to the collection.")))
                {
                    VP_AddGridTileBrushPopup.Initialize(m_collection.GridTileBrushes);
                }
                EditorGUI.BeginDisabledGroup(m_collection.SelectedGridTileBrush == null || m_collection.SelectedGridTileBrush.m_gridTile == null);
                GUI.backgroundColor = red;
                //remove selected brushes button
                if (GUILayout.Button(new GUIContent("Remove Selected Tile", "Removes the selected gridtile from the collection.")))
                {
                    if (m_collection.SelectedGridTileBrush != null)
                    {
                        m_collection.RemoveTile(m_collection.SelectedGridTileBrush);
                        m_collection.m_selectedGridTileBrushIndex = -1;
                        m_collection.Save();
                    }
                }
                EditorGUI.EndDisabledGroup();
                //remove all brushes button
                EditorGUI.BeginDisabledGroup(m_collection.GridTileBrushes.Count == 0);
                if (GUILayout.Button(new GUIContent("Remove All Tiles", "Removes all tiles from the collection.")) && RemoveAllBrushes_Dialog(m_collection.GridTileBrushes.Count))
                {
                    m_collection.RemoveAllTiles();
                    m_collection.Save();
                }
                EditorGUI.EndDisabledGroup();

                EditorGUILayout.EndHorizontal();
                GUI.backgroundColor = guiBGColor;

                if (m_collection.GridTileBrushes != null && m_collection.GridTileBrushes.Count > 0 && m_collection.SelectedGridTileBrush != null && m_collection.SelectedGridTileBrush.m_gridTile != null)
                {
                    EditorGUILayout.Space(10f);
                    EditorGUILayout.BeginHorizontal();
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.LabelField("Tile Settings:" + "  (" + m_collection.SelectedGridTileBrush.m_gridTile.gameObject.name + ")", EditorStyles.boldLabel);
                    if (GUILayout.Button(new GUIContent("Locate Tile", "Locate the current selected tile."), GUILayout.MaxWidth(120)))
                    {
                        VP_Utils.PingObject(GridTileBrush.CellTileObject);
                    }

                    if (GUILayout.Button(new GUIContent("Reset Settings", "Restores the settings for the current GridTile."), GUILayout.MaxWidth(120)))
                    {
                        m_collection.SelectedGridTileBrush.ResetParameters();
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space(5f);
                    EditorGUILayout.BeginHorizontal();
                    m_collection.SelectedGridTileBrush.m_tileType = (GRID_TILE_TYPES)EditorGUILayout.EnumPopup("Tile Type", m_collection.SelectedGridTileBrush.m_tileType);
                    if (GUILayout.Button(new GUIContent("Reset Type", "Restores the tile type to DEFAULT."), GUILayout.MaxWidth(230)))
                    {
                        m_collection.SelectedGridTileBrush.m_tileType = GRID_TILE_TYPES.DEFAULT;
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space(5f);
                    EditorGUILayout.BeginHorizontal();
                    m_collection.SelectedGridTileBrush.m_layer = EditorGUILayout.IntField(new GUIContent("Tile Layer", "Changes the layer parameter of the tile."), m_collection.SelectedGridTileBrush.m_layer);
                    m_collection.SelectedGridTileBrush.m_layer = Mathf.Clamp(m_collection.SelectedGridTileBrush.m_layer, 0, m_collection.SelectedGridTileBrush.m_layer);
                    if (GUILayout.Button(new GUIContent("Reset Layer", "Restores the tile layer to 0."), GUILayout.MaxWidth(230)))
                    {
                        m_collection.SelectedGridTileBrush.m_layer = 0;
                    }
                    EditorGUILayout.EndHorizontal();
                    
                    EditorGUILayout.BeginHorizontal();
                     m_collection.SelectedGridTileBrush.m_height = EditorGUILayout.IntField(new GUIContent("Height in Grid", "Changes the height parameter of the tile."), m_collection.SelectedGridTileBrush.m_height);
                    if (GUILayout.Button(new GUIContent("Reset Height", "Restores the tile height to 0."), GUILayout.MaxWidth(230)))
                    {
                        m_collection.SelectedGridTileBrush.m_height = 0;
                    }
                    EditorGUILayout.EndHorizontal();
                    
                    EditorGUILayout.BeginHorizontal();
                    m_collection.SelectedGridTileBrush.m_offset = EditorGUILayout.Vector3Field(new GUIContent("Position Offsets", "Changes the position offset from the Cell center."), m_collection.SelectedGridTileBrush.m_offset);
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    m_collection.SelectedGridTileBrush.m_rotation = EditorGUILayout.Vector3Field(new GUIContent("Rotation Offset", "Changes the rotation offset from the current orientation."), m_collection.SelectedGridTileBrush.m_rotation);
                    EditorGUILayout.EndHorizontal();
                    //GridTileBrush.LockedZPosition = EditorGUILayout.Toggle(new GUIContent("Locked Z Position", "Locks Z position"), GridTileBrush.LockedZPosition);
                    if (EditorGUI.EndChangeCheck())
                    {
                        // Update the cell's settings
                        GridTileBrush.SetCellFromEditor(Vector3Int.zero, m_collection.SelectedGridTileBrush.m_gridTile, m_collection.SelectedGridTileBrush.m_height, m_collection.SelectedGridTileBrush.m_offset, Quaternion.Euler(m_collection.SelectedGridTileBrush.m_rotation), m_collection.SelectedGridTileBrush.m_layer, m_collection.SelectedGridTileBrush.m_tileType);
                    }
                }
                EditorGUILayout.Space(10f);
                //m_saveCollectionOnChange = EditorGUILayout.Toggle(new GUIContent("Save Automatically", "Save on Change"), m_saveCollectionOnChange);

                if (GUILayout.Button("Save Collection"))//if (GUI.changed && m_collection != null && m_saveCollectionOnChange)
                {
                    m_collection.Save();
                }
                
#endregion
            }
        }

        public void RotateSelectedTile(Quaternion orientation)
        {
            if (m_collection != null && m_collection.GridTileBrushes != null && m_collection.GridTileBrushes.Count > 0 && m_collection.SelectedGridTileBrush != null && m_collection.SelectedGridTileBrush.m_gridTile != null)
            {
                m_collection.SelectedGridTileBrush.m_rotation = (Quaternion.Euler(m_collection.SelectedGridTileBrush.m_rotation) * orientation).eulerAngles;
                EditorUtility.SetDirty(this);
            }
        }

        public override bool canChangeZPosition
        {
            get { return !GridTileBrush.LockedZPosition; }
            set { GridTileBrush.LockedZPosition = !value; }
        }

        bool RemoveAllBrushes_Dialog(int brushCount)
        {
            return EditorUtility.DisplayDialog(
                "Remove all GridTiles?",
                "Are you sure you want to remove all GridTiles (" + brushCount + ") from this collection?",
                "Remove all",
                "Cancel");
        }

        public override void OnPaintSceneGUI(GridLayout gridLayout, GameObject brushTarget, BoundsInt position, GridBrushBase.Tool tool, bool executing)
        {
            BoundsInt gizmoRect = position;
            bool refreshPreviews = false;
            if (Event.current.type == EventType.Layout)
            {
                int newPreviewRefreshHash = GetHash(gridLayout, brushTarget, position, tool, GridTileBrush);
                refreshPreviews = newPreviewRefreshHash != m_lastPreviewRefreshHash;
                if (refreshPreviews)
                    m_lastPreviewRefreshHash = newPreviewRefreshHash;
            }
            // Move preview - To be fully implemented on the next version
            /*
            if (tool == GridBrushBase.Tool.Move)
            {
                if (refreshPreviews && executing)
                {
                    ClearPreview();
                    PaintPreview(gridLayout, brushTarget, position.min);
                }
            }
            // Paint preview
            else*/
            if (tool == GridBrushBase.Tool.Paint || tool == GridBrushBase.Tool.Erase)
            {
                if (refreshPreviews)
                {
                    ClearPreviewAll();
                    if (tool != GridBrushBase.Tool.Erase)
                    {
                        PaintPreview(gridLayout, brushTarget, position.min);
                    }
                }
                gizmoRect = new BoundsInt(position.min - GridTileBrush.Pivot, GridTileBrush.Size);
            }
            // BoxFill Preview
            else if (tool == GridBrushBase.Tool.Box)
            {
                if (refreshPreviews)
                {
                    ClearPreviewAll();
                    BoxFillPreview(gridLayout, brushTarget, position);
                }
            }

            base.OnPaintSceneGUI(gridLayout, brushTarget, gizmoRect, tool, executing);

            // Paint the hovered grid position onto the scene
            var labelText = "Grid Position: " + position.position;
            if (position.size.x > 1 || position.size.y > 1)
            {
                labelText += " Size: " + position.size;
            }
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.black;
            Handles.Label(gridLayout.CellToWorld(position.position), labelText, style);
        }

        public virtual void PaintPreview(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
        {
            Vector3Int min = position - GridTileBrush.Pivot;
            Vector3Int max = min + GridTileBrush.Size;
            BoundsInt bounds = new BoundsInt(min, max - min);

            var pvmanager = VP_GridPreviewManager.Instance;
            if (pvmanager == null)
                return;

            if (brushTarget != null && gridLayout != null)
            {
                foreach (Vector3Int location in bounds.allPositionsWithin)
                {
                    Vector3Int brushPosition = location - min;
                    VP_GridTileBrushCell cell = GridTileBrush.Cells[GridTileBrush.GetCellIndex(brushPosition)];
                    if (cell.GridTile != null)
                    {
                        SetPreviewCell(gridLayout, location, cell);
                    }
                }
            }

            m_lastGrid = gridLayout;
            m_lastBounds = bounds;
            m_lastBrushTarget = brushTarget;
            m_lastTool = GridBrushBase.Tool.Paint;
        }

        public virtual void BoxFillPreview(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
        {
            if (brushTarget != null)
            {
                var pvmanager = VP_GridPreviewManager.Instance;
                if (pvmanager == null)
                    return;

                foreach (Vector3Int location in position.allPositionsWithin)
                {
                    Vector3Int local = location - position.min;
                    VP_GridTileBrushCell cell = GridTileBrush.Cells[GridTileBrush.GetCellIndexWrapAround(local.x, local.y, local.z)];
                    if (cell.GridTile != null)
                    {
                        SetPreviewCell(gridLayout, location, cell);
                    }
                }
            }

            m_lastGrid = gridLayout;
            m_lastBounds = position;
            m_lastBrushTarget = brushTarget;
            m_lastTool = GridBrushBase.Tool.Box;
        }

        public virtual void ClearPreview()
        {
            if (m_lastGrid == null || m_lastBounds == null || m_lastBrushTarget == null || m_lastTool == null)
                return;

            var pvmanager = VP_GridPreviewManager.Instance;
            if (pvmanager != null)
            {
                switch (m_lastTool)
                {
                    
                    case GridBrushBase.Tool.FloodFill:
                        {
                            BoundsInt bounds = m_lastBounds.Value;
                            foreach (Vector3Int location in bounds.allPositionsWithin)
                            {
                                ClearPreviewCell(location);
                            }
                            break;
                        }
                    case GridBrushBase.Tool.Box:
                        {
                            Vector3Int min = m_lastBounds.Value.position;
                            Vector3Int max = min + m_lastBounds.Value.size;
                            BoundsInt bounds = new BoundsInt(min, max - min);
                            foreach (Vector3Int location in bounds.allPositionsWithin)
                            {
                                ClearPreviewCell(location);
                            }
                            break;
                        }
                    case GridBrushBase.Tool.Paint:
                        {
                            BoundsInt bounds = m_lastBounds.Value;
                            foreach (Vector3Int location in bounds.allPositionsWithin)
                            {
                                ClearPreviewCell(location);
                            }
                            break;
                        }
                }
            }

            m_lastBrushTarget = null;
            m_lastGrid = null;
            m_lastBounds = null;
            m_lastTool = null;
        }

        private static void SetPreviewCell(GridLayout grid, Vector3Int position, VP_GridTileBrushCell cell)
        {
            if (cell.GridTile == null || grid == null)
                return;

            var pvmanager = VP_GridPreviewManager.Instance;
            if (pvmanager == null)
                return;

            pvmanager.InstantiatePreviewTileAtPosition(cell.GridTile, position.ToVector2IntXY(), cell.Offset, cell.Orientation);
        }

        private static void ClearPreviewCell(Vector3Int location)
        {
            var pvmanager = VP_GridPreviewManager.Instance;
            if (pvmanager == null)
                return;

            pvmanager.ClearPreviewObjectAtPosition(location.ToVector2IntXY());
        }

        private static void ClearPreviewAll()
        {
            var pvmanager = VP_GridPreviewManager.Instance;
            if (pvmanager == null)
                return;

            pvmanager.ClearAllPreviewTiles();
        }

        public override void OnMouseLeave()
        {
            ClearPreviewAll();
        }

        public override void OnToolDeactivated(GridBrushBase.Tool tool)
        {
            ClearPreviewAll();
        }

        private static int GetHash(GridLayout gridLayout, GameObject brushTarget, BoundsInt position, GridBrushBase.Tool tool, VP_TilePaletteGridTileBrush brush)
        {
            int hash = 0;
            unchecked
            {
                hash = hash * 33 + (gridLayout != null ? gridLayout.GetHashCode() : 0);
                hash = hash * 33 + (brushTarget != null ? brushTarget.GetHashCode() : 0);
                hash = hash * 33 + position.GetHashCode();
                hash = hash * 33 + tool.GetHashCode();
                hash = hash * 33 + (brush != null ? brush.GetHashCode() : 0);
            }

            return hash;
        }
    }
}
#endif