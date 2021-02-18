#if USE_GRID_SYSTEM && USE_TILEMAP
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using VirtualPhenix.GridEngine;
using VirtualPhenix;

namespace UnityEditor.Tilemaps
{
    [CustomEditor(typeof(VP_TilePaletteGridObjectBrush))]
    public class VP_RemakeGridObjectBrushEditor : VP_GridBrushEditorBase
    {
        // Target brush
        public VP_TilePaletteGridObjectBrush GridObjectBrush { get { return target as VP_TilePaletteGridObjectBrush; } }
        // Editor Preview Collection/Selection
        protected Vector2 _scrollViewScrollPosition = new Vector2();
        protected VP_GridObjectCollection m_collection;
        protected int m_SelectedGridObjectCollectionIndex = 0;
        // OnScene Preview
        protected int m_LastPreviewRefreshHash;
        protected GridLayout m_LastGrid;
        protected GameObject m_LastBrushTarget;
        protected BoundsInt? m_LastBounds;
        protected GridBrushBase.Tool? m_LastTool;
        protected VP_GridObjectCollection.GridObjectCollectionList m_gridObjectCollectionList;
        // Editor cached colors
        public static Color red = VP_GridUtilities.ColorFromRGB(239, 80, 80);
        public static Color green = VP_GridUtilities.ColorFromRGB(93, 173, 57);
        public static Color PrimarySelectedColor = VP_GridUtilities.ColorFromRGB(10, 153, 220);
        public static VP_RemakeGridObjectBrushEditor Instance { get; protected set; }
        public VP_GridObjectCollection Collection { get { return m_collection; } }
        protected virtual void OnEnable()
        {
            Instance = this;
            Undo.undoRedoPerformed += ClearLastPreview;
            ReloadCollections();
            m_SelectedGridObjectCollectionIndex = VP_GridObjectCollection.GetLastUsedGridObjectCollectionIndex();
        }

        protected virtual void OnDisable()
        {
            Undo.undoRedoPerformed -= ClearLastPreview;
        }

        protected virtual void ClearLastPreview()
        {
            ClearPreviewAll();
            m_LastPreviewRefreshHash = 0;
        }

        public virtual bool CreateCollection(out VP_GridObjectCollection _collection, string _name = "new SO")
        {
            string whereToSaveAll = VP_Utils.GetProjectAssetsFolderToSave("Choose the folder where to save the Scriptable Object");

            if (whereToSaveAll.IsNullOrEmpty())
            {
                _collection = null;
                return false;
            }

            string assetPathAndName = UnityEditor.AssetDatabase.GenerateUniqueAssetPath(whereToSaveAll + "/" + _name + ".asset");

            _collection = ScriptableObject.CreateInstance<VP_GridObjectCollection>();
            UnityEditor.AssetDatabase.CreateAsset(_collection, assetPathAndName);

            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
            UnityEditor.EditorUtility.FocusProjectWindow();

            return true;
        }

        public virtual bool LoadCollection(out VP_GridObjectCollection _collection)
        {
            string whereToSaveAll = VP_Utils.ChooseFilePathInsideAssets("Choose the Grid Tile Collection");
            if (whereToSaveAll.IsNullOrEmpty())
            {
                _collection = null;
                return false;
            }

            _collection = (VP_GridObjectCollection)AssetDatabase.LoadAssetAtPath(whereToSaveAll, typeof(VP_GridObjectCollection));
            return _collection != null;
        }

        public virtual void ReloadCollections()
        {
            m_gridObjectCollectionList = VP_GridObjectCollection.GetGridObjectCollectionsInProject();
        }

        public override void OnPaintInspectorGUI()
        {
            if (GridObjectBrush.pickedObject)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Currently Picked Objects (Pick Tool)", EditorStyles.boldLabel);
                if (GUILayout.Button("Unpick Objects"))
                {
                    GridObjectBrush.ResetPick();
                    return;
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space(7.5f);

                int rowLength = 1;
                int maxRowLength = GridObjectBrush.Size.x;
                var previewSize = Mathf.Min(((Screen.width - 35) / maxRowLength), 100);

                _scrollViewScrollPosition = EditorGUILayout.BeginScrollView(_scrollViewScrollPosition, false, false);

                if (maxRowLength < 1)
                {
                    maxRowLength = 1;
                }

                foreach (VP_GridObjectBrushCell tileBrush in GridObjectBrush.Cells)
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

                    GUIContent btnContent = tileBrush != null && tileBrush.GridObject != null ?
                    new GUIContent(AssetPreview.GetAssetPreview(tileBrush.GridObject.gameObject), tileBrush.GridObject.gameObject.name) :
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
                var go = GridObjectBrush;

                SerializedObject serializedObject_brushObject = null;
                int prevSelectedGridTileCollectionIndex = m_SelectedGridObjectCollectionIndex;
                if (m_collection != null)
                {
                    serializedObject_brushObject = new SerializedObject(m_collection);
                }

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Active GridTile Collection:");

                m_SelectedGridObjectCollectionIndex = EditorGUILayout.Popup(m_SelectedGridObjectCollectionIndex, m_gridObjectCollectionList.GetNameList());
                if (prevSelectedGridTileCollectionIndex != m_SelectedGridObjectCollectionIndex || m_collection == null) //select only when brush collection changed or is null
                {
                    if (m_gridObjectCollectionList.brushCollections != null && m_gridObjectCollectionList.brushCollections.Count > 0)
                    {
                        m_collection = m_gridObjectCollectionList.brushCollections[m_SelectedGridObjectCollectionIndex];
                        m_collection.SetLastUsedGridObjectCollection();
                        GridObjectBrush.ClearCellFromEditor();
                        var tileBrush = m_collection.m_SelectedGridObjectBrush;
                        if (tileBrush != null)
                        {
                            GridObjectBrush.SetCellFromEditor(Vector3Int.zero, tileBrush.m_GridObject, tileBrush.m_InitialOrientation);
                        }
                    }
                }

                if (GUILayout.Button("+"))
                {
                    Debug.Log("Create a new collection");

                    VP_GridObjectCollection col = null;
                    if (CreateCollection(out col, "New Grid Tile Collection"))
                    {
                        m_collection = col;
                    }
                }

                if (GUILayout.Button("..."))
                {
                    VP_Utils.PingObject(m_collection);
                }

                if (GUILayout.Button("Reload List"))
                {
                    Debug.Log("Reloading");
                }
                
          /*      if (GUILayout.Button("Find"))
                {
                    Debug.Log("Reloading");
                    var ls = VP_Utils.FindAssetsByType<VP_TilePaletteGridObjectBrush>("UnityEditor.Tilemaps.");

                    if (ls.Count > 0)
                        VP_Utils.PingObject(ls[0]);
                }
                */
                
                EditorGUILayout.EndHorizontal();

                if (m_collection == null)
                    return;

                EditorGUILayout.Space(7.5f);
                int rowLength = 1;
                int maxRowLength = Mathf.FloorToInt((Screen.width - 15) / 100);
                int columns = Mathf.CeilToInt((m_collection.GridObjectBrushes.Count / maxRowLength)) * 3;
                _scrollViewScrollPosition = EditorGUILayout.BeginScrollView(_scrollViewScrollPosition, false, false);

                if (maxRowLength < 1)
                {
                    maxRowLength = 1;
                }

                foreach (VP_GridObjectBrush tileBrush in m_collection.GridObjectBrushes)
                {
                    //check if brushObject is null, if so skip this brush
                    if (tileBrush == null || tileBrush.m_GridObject == null)
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
                    if (m_collection.m_SelectedGridObjectBrush != null && m_collection.m_SelectedGridObjectBrush.m_GridObject != null && m_collection.m_SelectedGridObjectBrush.m_GridObject == tileBrush.m_GridObject)
                    {
                        GUI.backgroundColor = PrimarySelectedColor;
                        if (GridObjectBrush.EditorCell != null && GridObjectBrush.EditorCell.GridObject != tileBrush.m_GridObject)
                        {
                            GridObjectBrush.SetCellFromEditor(Vector3Int.zero, tileBrush.m_GridObject, tileBrush.m_InitialOrientation);
                        }
                    }

                    //Create the brush entry in the scroll view and check if the user clicked on the created button (change the currently selected/edited brush accordingly and add it to the current brushes if possible)
                    GUIContent btnContent = new GUIContent(AssetPreview.GetAssetPreview(tileBrush.m_GridObject.gameObject), tileBrush.m_GridObject.gameObject.name);
                    if (GUILayout.Button(btnContent, GUILayout.Width(100), GUILayout.Height(100)))
                    {
                        //select the currently edited brush and deselect all selected brushes
                        if (m_collection.m_SelectedGridObjectBrush != tileBrush)
                        {

                            m_collection.m_SelectedGridObjectBrushIndex = m_collection.GridObjectBrushes.IndexOf(tileBrush);
                            GridObjectBrush.SetCellFromEditor(Vector3Int.zero, tileBrush.m_GridObject, tileBrush.m_InitialOrientation);
                        }
                        else
                        {
                            //m_Collection.m_SelectedGridTileBrush = null;
                            m_collection.m_SelectedGridObjectBrushIndex = -1;
                            GridObjectBrush.ClearCellFromEditor();
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
                if (GUILayout.Button(new GUIContent("+", "Add a GridObject to the collection."), GUILayout.Width(100), GUILayout.Height(100)))
                {
                    VP_AddGridObjectBrushPopup.Initialize(m_collection.GridObjectBrushes);
                }
                Color guiBGColor = GUI.backgroundColor;

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndScrollView();

                EditorGUILayout.BeginHorizontal();
                GUI.backgroundColor = green;
                if (GUILayout.Button(new GUIContent("Add GridObject", "Add a GridObject to the collection.")))
                {
                    VP_AddGridObjectBrushPopup.Initialize(m_collection.GridObjectBrushes);
                }
                EditorGUI.BeginDisabledGroup(m_collection.m_SelectedGridObjectBrush == null || m_collection.m_SelectedGridObjectBrush.m_GridObject == null);
                GUI.backgroundColor = red;
                //remove selected brushes button
                if (GUILayout.Button(new GUIContent("Remove Selected GridObject", "Removes the selected gridobject from the collection.")))
                {
                    if (m_collection.m_SelectedGridObjectBrush != null)
                    {
                        m_collection.RemoveObject(m_collection.m_SelectedGridObjectBrush);
                        m_collection.m_SelectedGridObjectBrushIndex = -1;
                        m_collection.Save();
                    }
                }
                EditorGUI.EndDisabledGroup();
                //remove all brushes button
                EditorGUI.BeginDisabledGroup(m_collection.GridObjectBrushes.Count == 0);
                if (GUILayout.Button(new GUIContent("Remove All GridObjects", "Removes all tiles from the collection.")) && RemoveAllBrushes_Dialog(m_collection.GridObjectBrushes.Count))
                {
                    m_collection.RemoveAllObjects();
                    m_collection.Save();
                }
                EditorGUI.EndDisabledGroup();

                EditorGUILayout.EndHorizontal();
                GUI.backgroundColor = guiBGColor;

                if (m_collection.GridObjectBrushes != null && m_collection.GridObjectBrushes.Count > 0 && m_collection.m_SelectedGridObjectBrush != null && m_collection.m_SelectedGridObjectBrush.m_GridObject != null)
                {
                    EditorGUILayout.Space(10f);
                    EditorGUILayout.BeginHorizontal();
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.LabelField("GridObject Settings:" + "  (" + m_collection.m_SelectedGridObjectBrush.m_GridObject.gameObject.name + ")", EditorStyles.boldLabel);
                    if (GUILayout.Button(new GUIContent("Reset Settings", "Restores the settings for the current GridObject."), GUILayout.MaxWidth(120)))
                    {
                        m_collection.m_SelectedGridObjectBrush.ResetParameters();
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space(5f);
                    EditorGUILayout.BeginHorizontal();
                    m_collection.m_SelectedGridObjectBrush.m_InitialOrientation = (Orientations)EditorGUILayout.EnumPopup(new GUIContent("Initial Orientation", "Changes the initial orientation of the gridobject."), m_collection.m_SelectedGridObjectBrush.m_InitialOrientation);
                    EditorGUILayout.EndHorizontal();
                    if (EditorGUI.EndChangeCheck())
                    {
                        // Update the cell's settings
                        GridObjectBrush.SetCellFromEditor(Vector3Int.zero, m_collection.m_SelectedGridObjectBrush.m_GridObject, m_collection.m_SelectedGridObjectBrush.m_InitialOrientation);
                    }
                }
                EditorGUILayout.Space(10f);
                /*
                if (GUI.changed && m_Collection != null)
                {
                    m_Collection.Save();
                }
                */
#endregion
            }
        }

        public void RotateSelectedTile(GridBrushBase.RotationDirection rotateDirection)
        {

            if (m_collection != null && m_collection.GridObjectBrushes != null && m_collection.GridObjectBrushes.Count > 0 && m_collection.m_SelectedGridObjectBrush != null && m_collection.m_SelectedGridObjectBrush.m_GridObject != null)
            {
                /*
                m_Collection.m_SelectedGridObjectBrush.m_Rotation = (Quaternion.Euler(m_Collection.m_SelectedGridObjectBrush.m_Rotation) * orientation).eulerAngles;
                EditorUtility.SetDirty(this);
                */
            }

        }


        public override bool canChangeZPosition
        {
            get { return false; }
            set { }
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
            var go = GridObjectBrush;

            BoundsInt gizmoRect = position;
            bool refreshPreviews = false;
            if (Event.current.type == EventType.Layout)
            {
                int newPreviewRefreshHash = GetHash(gridLayout, brushTarget, position, tool, go);
                refreshPreviews = newPreviewRefreshHash != m_LastPreviewRefreshHash;
                if (refreshPreviews)
                    m_LastPreviewRefreshHash = newPreviewRefreshHash;
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
                gizmoRect = new BoundsInt(position.min - go.Pivot, go.Size);
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
            var go = GridObjectBrush;
            Vector3Int min = position - go.Pivot;
            Vector3Int max = min + go.Size;
            BoundsInt bounds = new BoundsInt(min, max - min);

            var pvmanager = VP_GridPreviewManager.Instance;
            if (pvmanager == null)
                return;

            if (brushTarget != null && gridLayout != null)
            {
                foreach (Vector3Int location in bounds.allPositionsWithin)
                {
                    Vector3Int brushPosition = location - min;
                    VP_GridObjectBrushCell cell = go.Cells[go.GetCellIndex(brushPosition)];
                    if (cell.GridObject != null)
                    {
                        SetPreviewCell(gridLayout, location, cell);
                    }
                }
            }

            m_LastGrid = gridLayout;
            m_LastBounds = bounds;
            m_LastBrushTarget = brushTarget;
            m_LastTool = GridBrushBase.Tool.Paint;
        }

        public virtual void BoxFillPreview(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
        {
            if (brushTarget != null)
            {
                var pvmanager = VP_GridPreviewManager.Instance;
                if (pvmanager == null)
                    return;
                
                var go = GridObjectBrush;

                foreach (Vector3Int location in position.allPositionsWithin)
                {
                    Vector3Int local = location - position.min;
                    VP_GridObjectBrushCell cell = go.Cells[go.GetCellIndexWrapAround(local.x, local.y, local.z)];
                    if (cell.GridObject != null)
                    {
                        SetPreviewCell(gridLayout, location, cell);
                    }
                }
            }

            m_LastGrid = gridLayout;
            m_LastBounds = position;
            m_LastBrushTarget = brushTarget;
            m_LastTool = GridBrushBase.Tool.Box;
        }

        public virtual void ClearPreview()
        {
            if (m_LastGrid == null || m_LastBounds == null || m_LastBrushTarget == null || m_LastTool == null)
                return;

            var pvmanager = VP_GridPreviewManager.Instance;
            if (pvmanager != null)
            {
                switch (m_LastTool)
                {
                    /*
                    case GridBrushBase.Tool.FloodFill:
                        {
                            map.ClearAllEditorPreviewTiles();
                            break;
                        }
                    */
                    case GridBrushBase.Tool.Box:
                        {
                            Vector3Int min = m_LastBounds.Value.position;
                            Vector3Int max = min + m_LastBounds.Value.size;
                            BoundsInt bounds = new BoundsInt(min, max - min);
                            foreach (Vector3Int location in bounds.allPositionsWithin)
                            {
                                ClearPreviewCell(location);
                            }
                            break;
                        }
                    case GridBrushBase.Tool.Paint:
                        {
                            BoundsInt bounds = m_LastBounds.Value;
                            foreach (Vector3Int location in bounds.allPositionsWithin)
                            {
                                ClearPreviewCell(location);
                            }
                            break;
                        }
                }
            }

            m_LastBrushTarget = null;
            m_LastGrid = null;
            m_LastBounds = null;
            m_LastTool = null;
        }

        private static void SetPreviewCell(GridLayout grid, Vector3Int position, VP_GridObjectBrushCell cell)
        {
            if (cell.GridObject == null || grid == null)
                return;

            var pvmanager = VP_GridPreviewManager.Instance;
            if (pvmanager == null)
                return;

            pvmanager.InstantiatePreviewGridObjectAtPosition(cell.GridObject, position.ToVector2IntXY(), Vector3.zero, cell.Orientation);
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

        private static int GetHash(GridLayout gridLayout, GameObject brushTarget, BoundsInt position, GridBrushBase.Tool tool, VP_TilePaletteGridObjectBrush brush)
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