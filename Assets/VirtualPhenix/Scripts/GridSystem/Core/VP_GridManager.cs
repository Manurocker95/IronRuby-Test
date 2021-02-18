#if USE_GRID_SYSTEM
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using NaughtyAttributes;
using UnityEditor;

namespace VirtualPhenix.GridEngine
{
    [DefaultExecutionOrder(VP_ExecutingOrderSetup.GRID_MANAGER)]
    public class VP_GridManager : VP_SingletonMonobehaviour<VP_GridManager>
    {
        public static new VP_GridManager Instance
        {
            get
            {
                if (m_instance == null) 
                { 
                    m_instance = (VP_GridManager)FindObjectOfType(typeof(VP_GridManager)); 
                }

                return (VP_GridManager)m_instance;
            }
        }


        [Header("Grid Settings")]
        [SerializeField] protected Grid m_grid; // Reference to the current Grid being used
        public bool m_usesHeight = false; // Wether or not we should use height for the pathfinding
        public bool m_replaceSameLayer = false;
        public bool m_canPlaceInSamePosition = false;

        [Header("Grid Values")]
        [SerializeField] protected VP_SerializableDictionary<int, VP_SerializableDictionary<Vector2Int, VP_GridTile>> m_gridTiles = new VP_SerializableDictionary<int, VP_SerializableDictionary<Vector2Int, VP_GridTile>>();// Grid tiles dictionary
        [SerializeField] protected List<VP_GridObject> m_GridObjects = new List<VP_GridObject>();// Grid objects list

        [Header("Neighboring")]
        [SerializeField] protected NeighboringTypes m_neighboringType;

        [ShowIf("ShowCustomNeighbors")]
        [SerializeField] protected List<Vector2Int> m_customNeighbors = new List<Vector2Int>();  // List of the custom neighbors (if chosen so)
        public bool ShowCustomNeighbors { get { return m_neighboringType == NeighboringTypes.Custom; } }

        [Header("Hovered Tile")]
        public VP_GridTile m_HoveredGridTile = null; // Currently highlighted GridTile

        // Holders
        protected Transform _gridTilesHolder;
        
        public Transform GridTilesHolder
        {
            get
            {
                if (_gridTilesHolder == null) 
                { 
                    GetHolders(); 
                }

                return _gridTilesHolder;
            }
            protected set { _gridTilesHolder = value; }
        }

        protected Transform _gridObjectsHolder;
        public Transform GridObjectsHolder
        {
            get
            {
                if (_gridObjectsHolder == null) { GetHolders(); }
                return _gridObjectsHolder;
            }
            protected set { _gridObjectsHolder = value; }
        }

        public Grid Grid
        {
            get
            {
                return m_grid;
            }
            set 
            { 
                m_grid = value; 
            }
        }

        protected override void Reset()
        {
            base.Reset();
            m_grid = gameObject.GetOrAddComponent<Grid>();
        }

        protected override void Initialize()
        {
            base.Initialize();

            if (m_gridTiles == null)
            {
                m_gridTiles = new VP_SerializableDictionary<int, VP_SerializableDictionary<Vector2Int, VP_GridTile>>();
            }

            // Update holders
            GetHolders();
        }

        public VP_SerializableDictionary<Vector2Int,VP_GridTile> GetGridTilesAtLayerAndPositions(int _layer)
        {
            Debug.Log("Layer contains: " + m_gridTiles.Contains(_layer));

            return m_gridTiles.Contains(_layer) ? m_gridTiles[_layer] : new VP_SerializableDictionary<Vector2Int, VP_GridTile>();
        }

        public List<VP_GridTile> GetGridTilesAtLayer(int _layer)
        {
            var tiles = GetGridTilesAtLayerAndPositions(_layer);
            Debug.Log("Tiles " + tiles.Count);

            List<VP_GridTile> ret = new List<VP_GridTile>();

            foreach (Vector2Int v in tiles.Keys)
            {
                ret.Add(tiles[v]);
            }

            return ret;
        }  
        
        
        public VP_SerializableDictionary<Vector2Int,VP_GridTile> GetGridTilesAtLayerAndPositionsEditor(int _layer)
        {
            VP_SerializableDictionary<Vector2Int, VP_GridTile> dictionary = new VP_SerializableDictionary<Vector2Int, VP_GridTile>();
             int childCount = GridTilesHolder.childCount;

            for (int i = 0; i < childCount; i++)
            {
                Transform child = GridTilesHolder.GetChild(i);
                var gridTileComp = child.GetComponent<VP_GridTile>();
                if (gridTileComp)
                {
                    if ( _layer == gridTileComp.TileLayer)
                    {
                        dictionary.Add(gridTileComp.GridPosition, gridTileComp);
                    }
                }
            }

            return dictionary;
        }
        
        public List<VP_GridTile> GetGridTilesAtLayerEditor(int _layer)
        {
            var tiles = GetGridTilesAtLayerAndPositionsEditor(_layer);

            List<VP_GridTile> ret = new List<VP_GridTile>();

            foreach (Vector2Int v in tiles.Keys)
            {
                ret.Add(tiles[v]);
            }

            return ret;
        }

        public List<GameObject> GetGridTilesGameObjectsAtLayerEditor(int _layer)
        {
            var tiles = GetGridTilesAtLayerAndPositionsEditor(_layer);

            List<GameObject> ret = new List<GameObject>();

            foreach (Vector2Int v in tiles.Keys)
            {
                ret.Add(tiles[v].gameObject);
            }

            return ret;
        }

        public List<Vector2Int> GetNeighborPositions(bool oddRow = false)
        {
            var gridPositions = new List<Vector2Int>();
            switch (m_neighboringType)
            {
                case NeighboringTypes.Rect4Directions:
                    gridPositions = defaultRectangle4Directions;
                    break;
                case NeighboringTypes.Rect8Directions:
                    gridPositions = defaultRectangle8Directions;
                    break;
                case NeighboringTypes.HexagonDefault:
                    gridPositions = oddRow ? defaultOddHexagonalDirections : defaultEvenHexagonalDirections;
                    break;
                case NeighboringTypes.Custom:
                    gridPositions = m_customNeighbors;
                    break;
            }

            return gridPositions;
        }

        public Vector3Int GetRotationAxisVector()
        {
            switch (m_grid.cellSwizzle)
            {
                case Grid.CellSwizzle.XYZ:
                default:
                    return Vector3.forward.ToVector3Int();
                case Grid.CellSwizzle.XZY:
                    return Vector3Int.up;
                case Grid.CellSwizzle.YXZ:
                    return Vector3.forward.ToVector3Int();
                case Grid.CellSwizzle.YZX:
                    return Vector3Int.right;
                case Grid.CellSwizzle.ZXY:
                    return Vector3Int.up;
                case Grid.CellSwizzle.ZYX:
                    return Vector3Int.right;
            }
        }

        // Adds the tile to the tile dictionary if the position is not already occupied
        public void AddGridTile(Vector2Int gridPosition, VP_GridTile gridTile, int layer)
        {
           // Debug.Log("Adding to layer");
            if (!ExistsTileAtPosition(gridPosition, out VP_GridTile otherTile, layer))
            {
        
                if (m_gridTiles.ContainsKey(layer))
                {
                    m_gridTiles[layer][gridPosition] = gridTile;
                }
                else
                {
                    m_gridTiles.Add(layer, new VP_SerializableDictionary<Vector2Int, VP_GridTile>() { { gridPosition, gridTile } });
                }
            }
        }

        // Removes the target tile from the tile dictionary
        public void RemoveGridTile(int layer, VP_GridTile gridTile)
        {
            if (m_gridTiles[layer].ContainsValue(gridTile))
            {
                m_gridTiles[layer].Remove(gridTile.GridPosition);
            }
        }

        // Removes the tile at the target position
        public void RemoveGridTileAtPosition(int layer, Vector2Int gridPosition)
        {
            if (m_gridTiles[layer].ContainsKey(gridPosition))
                m_gridTiles[layer].Remove(gridPosition);
        }

        // Checks if a tile exists at the target position
        public bool ExistsTileAtPosition(Vector2Int gridPosition, out VP_GridTile _otherTile, int _layer = 0)
        {
            bool exists = false;
            if (Application.isPlaying)
            {
                exists = m_gridTiles.Contains(_layer) ? m_gridTiles[_layer].ContainsKey(gridPosition) : false;
                _otherTile = exists ? GetGridTileAtPosition(gridPosition, _layer) : null;
                return exists;
            }
            _otherTile = GetGridTileAtPositionInEditor(gridPosition, _layer);
            exists = _otherTile != null;
            return exists;
        }

        // Returns the tile at the target position, if there is one
        public VP_GridTile GetGridTileAtPosition(Vector2Int gridPosition, int _layer = 0)
        {
            if (!Application.isPlaying)
            {
                return GetGridTileAtPositionInEditor(gridPosition, _layer);
            }
            else
            {
                if (!ExistsTileAtPosition(gridPosition, out VP_GridTile otherTile, _layer))
                {
                    return null;
                }

                return m_gridTiles[_layer][gridPosition];
            }
        }

        // Returns the tile at the target position, if there is one
        public void SetGridTileAtItsPosition(VP_GridTile gridTile, int _layer = 0)
        {

            var pos = gridTile.GridPosition;
            if (ExistsTileAtPosition(pos, out VP_GridTile otherTile, _layer))
            {
                m_gridTiles[_layer][pos] = gridTile;
                return;
            }

            AddGridTile(pos, gridTile, _layer);
        }


        // Sets the currently hovered tile
        public void SetHoveredTile(VP_GridTile gridTile)
        {
            m_HoveredGridTile = gridTile;
        }

        // Unsets the currently hovered tile
        public void UnsetHoveredTile(VP_GridTile gridTile)
        {
            if (m_HoveredGridTile == gridTile)
                m_HoveredGridTile = null;
        }

        // Adds a GridObject to the GridObject List
        public void AddGridObject(VP_GridObject gridObject)
        {
            if (!m_GridObjects.Contains(gridObject))
            {
                m_GridObjects.Add(gridObject);
            }
        }

        // Removes a GridObject to the GridObject List
        public void RemoveGridObject(VP_GridObject gridObject)
        {
            if (m_GridObjects.Contains(gridObject))
            {
                m_GridObjects.Remove(gridObject);
            }
        }

        public bool ExistsGridObjectAtPosition(Vector2Int gridPosition, int _layer = 0)
        {
            return (GetGridObjectAtPosition(gridPosition, _layer) != null);
        }

        public VP_GridObject GetGridObjectAtPosition(Vector2Int gridPosition, int _layer = 0)
        {
            if (!Application.isPlaying)
            {
                return GetGridObjectAtPositionInEditor(gridPosition, _layer);
            }
            else
            {
                foreach (VP_GridObject gObj in m_GridObjects)
                {
                    if (gObj == null)
                        continue;

                    if (gObj.GridPosition == gridPosition)
                    {
                        return gObj;
                    }
                }

                return null;
            }
        }

        // Default rectangle directions 
        public static List<Vector2Int> defaultRectangle4Directions = new List<Vector2Int>() {
        new Vector2Int(0, 1), // top
        new Vector2Int(1, 0), // right
        new Vector2Int(0, -1),// bottom
                new Vector2Int(-1, 0) // left
    };

        // Default rectangle 8 directions (diagonals) 
        public static List<Vector2Int> defaultRectangle8Directions = new List<Vector2Int>() {
        new Vector2Int(0, 1), // top
        new Vector2Int(1, 1), // top-right
        new Vector2Int(1, 0), // right
        new Vector2Int(1, -1), // bottom-right
        new Vector2Int(0, -1), // bottom
        new Vector2Int(-1, -1), // bottom-left
        new Vector2Int(-1, 0), // left
        new Vector2Int(-1, 1) // top-left     
    };

        public static List<Vector2Int> defaultEvenHexagonalDirections = new List<Vector2Int>() {
        new Vector2Int(0, 1),// top-right
        new Vector2Int(1, 0),// right
        new Vector2Int(0, -1), // bottom-right
        new Vector2Int(-1, -1),// bottom-left
        new Vector2Int(-1, 0), // left
        new Vector2Int(-1, 1) // top-left 
    };

        public static List<Vector2Int> defaultOddHexagonalDirections = new List<Vector2Int>() {
        new Vector2Int(1, 1), // top-right
        new Vector2Int(1, 0), // right
        new Vector2Int(1, -1), // bottom-right
        new Vector2Int(0, -1), // bottom-left
        new Vector2Int(-1, 0), // left
        new Vector2Int(0, 1), // top-left 
    };

        public static List<string> Rectangle8DirOrientationsList = new List<string>() {
        "North",
        "NorthEast",
        "East",
        "SouthEast",
        "South",
        "SouthWest",
        "West",
        "NorthWest"
    };

        public static List<string> Rectangle4DirOrientationsList = new List<string>() {
        Rectangle8DirOrientationsList[0],
        Rectangle8DirOrientationsList[2],
        Rectangle8DirOrientationsList[4],
        Rectangle8DirOrientationsList[6],
    };

        public static List<string> HexagonDirOrientationsList = new List<string>() {
        Rectangle8DirOrientationsList[1],
        Rectangle8DirOrientationsList[2],
        Rectangle8DirOrientationsList[3],
        Rectangle8DirOrientationsList[5],
        Rectangle8DirOrientationsList[6],
        Rectangle8DirOrientationsList[7]
    };

        public Vector2Int GetRelativeNeighborPositionFromOrientation(string orientation, bool oddRow = false)
        {
            switch (m_neighboringType)
            {
                case NeighboringTypes.Rect4Directions:
                case NeighboringTypes.Custom:
                default:
                    if (!Rectangle4DirOrientationsList.Contains(orientation))
                    {
                        var index = Rectangle8DirOrientationsList.IndexOf(orientation);
                        index++;
                        if (index >= Rectangle8DirOrientationsList.Count)
                            index = 0;
                        orientation = Rectangle8DirOrientationsList[index];
                    }
                    return GetNeighborPositions()[Rectangle4DirOrientationsList.IndexOf(orientation)];
                case NeighboringTypes.Rect8Directions:
                    return GetNeighborPositions()[Rectangle8DirOrientationsList.IndexOf(orientation)];
                case NeighboringTypes.HexagonDefault:
                    if (!HexagonDirOrientationsList.Contains(orientation))
                    {
                        var index = Rectangle8DirOrientationsList.IndexOf(orientation);
                        index++;
                        orientation = Rectangle8DirOrientationsList[index];
                    }
                    return GetNeighborPositions(oddRow)[HexagonDirOrientationsList.IndexOf(orientation)];
            }
        }

        public Quaternion OrientationToRotation(Vector2Int initialPosition, Orientations orientation)
        {
            var targetPosition = initialPosition + GetRelativeNeighborPositionFromOrientation(orientation.ToString(), (initialPosition.y & 1) == 1);
            return PositionToRotation(initialPosition, targetPosition);
        }

        public Quaternion PositionToRotation(Vector2Int initialPosition, Vector2Int targetPosition)
        {
            var rotation = Quaternion.identity;
            var initialWorldPosition = m_grid.GetCellCenterWorld(initialPosition.ToVector3IntXY0());
            var targetWorldPosition = m_grid.GetCellCenterWorld(targetPosition.ToVector3IntXY0());
            var relativeVector = targetWorldPosition - initialWorldPosition;

            if (m_grid.cellSwizzle == GridLayout.CellSwizzle.XZY)
            {
                rotation = Quaternion.LookRotation(relativeVector);
            }
            else if (m_grid.cellSwizzle == GridLayout.CellSwizzle.XYZ)
            {
                var rotatedVectorToTarget = Quaternion.Euler(0, 0, 90) * relativeVector;
                rotation = Quaternion.LookRotation(Vector3.forward, rotatedVectorToTarget);
            }

            return rotation;
        }



        // Returns the neighbor of the tile at the target position, if there is one
        public virtual VP_GridTile NeighborAtPosition(VP_GridTile gridTile, Vector2Int gridPosition)
        {
            var neighbors = WalkableNeighbors(gridTile);

            foreach (VP_GridTile tile in neighbors)
            {
                if (tile.GridPosition == gridPosition)
                {
                    return tile;
                }
            }

            return null;
        }

        // Returns a list with the neighbors of the tile
        // TODO: Check in ALL layers
        public virtual List<VP_GridTile> WalkableNeighbors(VP_GridTile gridTile, bool ignoresHeight = false, bool unoccupiedTilesOnly = true, VP_GridTile goalTile = null, List<Vector2Int> customDirections = null,int layer = 0)
        {
            List<VP_GridTile> results = new List<VP_GridTile>();
            var directions = customDirections != null ? customDirections : GetNeighborPositions((gridTile.GridPosition.y & 1) == 1);

            foreach (Vector2Int dir in directions)
            {
                Vector2Int newVector = dir + gridTile.GridPosition;
                if (ExistsTileAtPosition(newVector, out VP_GridTile otherTile, layer))
                {
                    VP_GridTile targetTile = GetGridTileAtPosition(newVector, layer);
                    if (targetTile != null)
                    {
                        if (targetTile.IsTileWalkable || (goalTile != null && targetTile == goalTile))
                        {
                            if (unoccupiedTilesOnly && targetTile.IsTileOccupied() && (goalTile == null || (goalTile != null && targetTile != goalTile)))
                                continue;

                            if (m_usesHeight && !ignoresHeight)
                            {
                                if (Mathf.Abs(Mathf.Abs(gridTile.TileHeight) - Mathf.Abs(targetTile.TileHeight)) <= 1)
                                {
                                    results.Add(targetTile);
                                }
                            }
                            else
                            {
                                results.Add(targetTile);
                            }
                        }
                    }
                }
            }

            // Add manual neighbors to the result
            foreach (VP_GridTile tile in gridTile.ManualNeighbors)
            {
                if (!results.Contains(tile))
                {
                    results.Add(tile);
                }
            }

            results.Distinct();
            return results;
        }

        public virtual List<VP_GridTile> Neighbors(VP_GridTile gridTile, bool ignoresHeight = false, List<Vector2Int> customDirections = null, int layer = 0)
        {
            List<VP_GridTile> results = new List<VP_GridTile>();
            var directions = customDirections != null ? customDirections : GetNeighborPositions((gridTile.GridPosition.y & 1) == 1); ;

            foreach (Vector2Int dir in directions)
            {
                Vector2Int newVector = dir + gridTile.GridPosition;
                if (ExistsTileAtPosition(newVector, out VP_GridTile otherTile, layer))
                {
                    VP_GridTile targetTile = GetGridTileAtPosition(newVector, layer);
                    if (targetTile != null)
                    {
                        results.Add(targetTile);
                    }
                }
            }

            // Add manual neighbors to the result
            foreach (VP_GridTile tile in gridTile.ManualNeighbors)
            {
                results.Add(tile);
            }

            results.Distinct();
            return results;
        }

        public virtual VP_GridTile InstantiateGridTile(VP_GridTile gridTilePrefab, Vector2Int gridPosition, int heightInGrid = 0, int layer = 0, GRID_TILE_TYPES type = GRID_TILE_TYPES.DEFAULT, Vector3? offsetPosition = null, Quaternion? rotation = null, Transform targetParent = null)
        {
            var cellWorldPosition = m_grid.GetCellCenterWorld(gridPosition.ToVector3IntXY0());
            var offset = offsetPosition.HasValue ? cellWorldPosition + offsetPosition.Value : cellWorldPosition;
            var rot = rotation.HasValue ? rotation.Value : Quaternion.identity;
            return InstantiateGridTile(gridTilePrefab, gridPosition, heightInGrid, layer, type, rotation, offset, targetParent);
        }

        // Instantiates tiles to the grid
        public virtual VP_GridTile InstantiateGridTile(VP_GridTile gridTilePrefab, Vector2Int gridPosition, int heightInGrid = 0, int layer = 0, GRID_TILE_TYPES type = GRID_TILE_TYPES.DEFAULT, Quaternion? rotation = null, Vector3? worldPosition = null, Transform targetParent = null)
        {
            if (gridTilePrefab == null || layer < 0)
                return null;

            // Check if there is another tile at the target position
            if (ExistsTileAtPosition(gridPosition, out VP_GridTile _otherTile, layer))
            {
                //Debug.Log("Couldn't instantiate the tile at the target position, there is another tile there.");
                return null;
            }

            VP_GridTile instantiatedGridTile = null;
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                instantiatedGridTile = (VP_GridTile)PrefabUtility.InstantiatePrefab(gridTilePrefab, gameObject.scene);
                if (instantiatedGridTile != null)
                {
                    Undo.RegisterCreatedObjectUndo((Object)instantiatedGridTile.gameObject, "Paint Gridobject Prefab");
                }
            }
            else
            {
#endif
                instantiatedGridTile = Instantiate(gridTilePrefab) as VP_GridTile;
#if UNITY_EDITOR
            }
#endif

            if (instantiatedGridTile == null)
                return null;

            // Transform values
            var targetWorldPosition = worldPosition.HasValue ? worldPosition.Value : m_grid.GetCellCenterWorld(gridPosition.ToVector3IntXY0());
            var quatRotation = rotation.HasValue ? rotation.Value : Quaternion.identity;
            var parent = targetParent != null ? targetParent : GridTilesHolder;
            instantiatedGridTile.transform.parent = parent;
            instantiatedGridTile.transform.position = targetWorldPosition;
            instantiatedGridTile.transform.rotation = quatRotation;
            // Set the tile's settings
            instantiatedGridTile.GridPosition = gridPosition;
            instantiatedGridTile.TileHeight = heightInGrid;
            instantiatedGridTile.SetType((int)type);
            instantiatedGridTile.TileLayer = layer;
   
            return instantiatedGridTile;
        }

        public virtual void EraseGridTileAtPosition(Vector2Int gridPosition, int _layer)
        {
            var tileAtPosition = GetGridTileAtPosition(gridPosition, _layer);
            if (tileAtPosition != null)
            {
                EraseGridTile(tileAtPosition);
            }
        }

        public virtual void EraseGridTile(VP_GridTile gridTile)
        {
            if (gridTile != null)
                DestroyImmediate(gridTile.gameObject);
        }


        public virtual void MoveTileToPosition(VP_GridTile gridTile, Vector2Int gridPosition, Vector3 offset, int _layer)
        {
            if (gridTile == null)
                return;

            // Check if there is another tile at the target position
            if (ExistsTileAtPosition(gridPosition, out VP_GridTile otherTile, _layer))
            {
                //Debug.Log("Couldn't move the tile to the target position, there is another tile there.");
                return;
            }

            RemoveGridTile(_layer, gridTile);
            AddGridTile(gridPosition, gridTile, _layer);
            var targetWorldPosition = m_grid.GetCellCenterWorld(gridPosition.ToVector3IntXY0()) + offset;
            gridTile.transform.position = targetWorldPosition;
            gridTile.GridPosition = gridPosition;
        }

        // Instantiates objects to the grid
        public virtual VP_GridObject InstantiateGridObject(VP_GridObject gridObjectPrefab, Vector2Int gridPosition, Orientations? initialOrientation = null, Transform targetParent = null, bool? checkTileAtPosition = true, int layer = 0)
        {
            if (gridObjectPrefab == null)
                return null;

            // Check if there is another object at the target position
            var gridObjectAtPosition = GetGridObjectAtPosition(gridPosition, layer);
            if (gridObjectAtPosition != null)
            {
                Debug.Log("Couldn't instantiate the GridObject at the target position, there is another gridobject there.");
                return null;
            }

            // Check if there is another tile at the target position
            if (checkTileAtPosition.HasValue && checkTileAtPosition.Value && !ExistsTileAtPosition(gridPosition, out VP_GridTile otherTile, layer))
            {
                Debug.Log("Couldn't instantiate the GridObject at the target position, because there is no tile there.");
                return null;
            }

            // Check if the tile is walkable
            var tileAtPosition = GetGridTileAtPosition(gridPosition, layer);
            if (checkTileAtPosition.HasValue && checkTileAtPosition.Value && !tileAtPosition.IsTileWalkable)
                return null;

            VP_GridObject instantiatedGridObject = null;
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                instantiatedGridObject = (VP_GridObject)PrefabUtility.InstantiatePrefab(gridObjectPrefab, gameObject.scene);
                if (instantiatedGridObject != null)
                {
                    Undo.RegisterCreatedObjectUndo((Object)instantiatedGridObject.gameObject, "Paint GridObject Prefab");
                }
            }
            else
            {
#endif
                instantiatedGridObject = Instantiate(gridObjectPrefab) as VP_GridObject;
#if UNITY_EDITOR
            }
#endif

            // Transform Values
            var cellPosition = tileAtPosition != null ? tileAtPosition.GridObjectsPivotPosition : m_grid.GetCellCenterWorld(gridPosition.ToVector3IntXY0());
            var targetWorldPosition = cellPosition;
            var orientation = initialOrientation.HasValue ? initialOrientation.Value : instantiatedGridObject.InitialOrientation;
            var parent = targetParent != null ? targetParent : GridObjectsHolder;
            instantiatedGridObject.transform.parent = parent;
            instantiatedGridObject.transform.position = targetWorldPosition;
            // Set the GridObjects settings
            instantiatedGridObject.GridPosition = gridPosition;
            instantiatedGridObject.CurrentGridTile = tileAtPosition;
            instantiatedGridObject.InitialOrientation = orientation;

            return instantiatedGridObject;
        }

        public virtual void EraseGridObjectAtPosition(Vector2Int gridPosition, int layer)
        {
            var objectAtPosition = GetGridObjectAtPosition(gridPosition, layer);
            if (objectAtPosition != null)
            {
                EraseGridObject(objectAtPosition);
            }
        }

        public virtual void EraseGridObject(VP_GridObject gridObject)
        {
            if (gridObject != null)
                DestroyImmediate(gridObject.gameObject);
        }

        protected virtual void GetHolders()
        {
            if (_gridObjectsHolder == null)
            {
                _gridObjectsHolder = transform.Find("GridObjects");
                if (_gridObjectsHolder == null)
                {
                    _gridObjectsHolder = new GameObject("GridObjects").transform;
                    _gridObjectsHolder.SetParent(transform);
                    _gridObjectsHolder.localPosition = Vector3.zero;
                }
            }

            if (_gridTilesHolder == null)
            {
                _gridTilesHolder = transform.Find("GridTiles");
                if (_gridTilesHolder == null)
                {
                    _gridTilesHolder = new GameObject("GridTiles").transform;
                    _gridTilesHolder.SetParent(transform);
                    _gridTilesHolder.localPosition = Vector3.zero;
                }
            }
        }

        protected virtual VP_GridTile GetGridTileAtPositionInEditor(Vector2Int gridPosition, int _layer)
        {
            int childCount = GridTilesHolder.childCount;

            for (int i = 0; i < childCount; i++)
            {
                Transform child = GridTilesHolder.GetChild(i);
                var gridTileComp = child.GetComponent<VP_GridTile>();
                if (gridTileComp)
                {
                    if (gridPosition == gridTileComp.GridPosition && _layer == gridTileComp.TileLayer)
                    {
                        return gridTileComp;
                    }
                }
            }

            return null;
        }

        protected virtual VP_GridObject GetGridObjectAtPositionInEditor(Vector2Int gridPosition, int _layer)
        {
            int childCount = GridObjectsHolder.childCount;

            for (int i = 0; i < childCount; i++)
            {
                Transform child = GridObjectsHolder.GetChild(i);
                var gridObjectComp = child.GetComponent<VP_GridObject>();
                if (gridObjectComp)
                {
                    if (gridPosition == gridObjectComp.GridPosition)
                    {
                        return gridObjectComp;
                    }
                }
            }

            return null;
        }
    }
}
#endif