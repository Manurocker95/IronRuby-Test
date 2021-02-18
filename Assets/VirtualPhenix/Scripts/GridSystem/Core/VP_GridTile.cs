#if USE_GRID_SYSTEM
#pragma warning disable 67

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityEngine.SceneManagement;
using NaughtyAttributes;

namespace VirtualPhenix.GridEngine
{

    [DefaultExecutionOrder(VP_ExecutingOrderSetup.GridSystem.GRID_TILE)]
    public class VP_GridTile : VP_MonoBehaviour
    {
        public delegate void GridObjectEnterHandler(VP_GridObject gridObject, VP_GridTile gridTile);
        public delegate void GridObjectExitHandler(VP_GridObject gridObject, VP_GridTile gridTile);
        public delegate void GridObjectEndMovementHandler(VP_GridMovement gridMovement, VP_GridTile startGridTile, VP_GridTile endGridTile);
        public event GridObjectEnterHandler OnGridObjectEnter;
        public event GridObjectExitHandler OnGridObjectExit;
        public event GridObjectEndMovementHandler OnGridObjectEndMovement;

        protected bool m_initializedTile = false;    // Initiliazed flag

        [Header("GridTile Settings")]
        [SerializeField] protected GRID_TILE_TYPES m_type;
        [SerializeField] protected Transform m_mainObject;
        [SerializeField] protected Vector2Int m_gridPosition = new Vector2Int(int.MaxValue, int.MaxValue);     // The position on the grid
        [SerializeField] protected Quaternion m_gridDefaultRotation = Quaternion.identity;     // The position on the grid
        [SerializeField] protected int m_tileHeight = 0;        // Height of this tile
        [SerializeField] protected int m_layer = 0;        // Layer-> different layer can be placed in the same grid pos

        [SerializeField] protected bool m_isTileWalkable = true; // Wether or not dynamic GridObjects will be able to move to the tile by default
        [Header("Agents Pivot Offset")]
        [SerializeField] protected Vector3 m_gridObjectsPivotOffset = new Vector3(0, 0.5f, 0);

        [Header("Pathfinding Settings")]
        [SerializeField] protected int m_costOfMovingToTile = 1;        // The cost of moving to this tile (used in pathfinding)
        protected float m_priority;    // Used for AStar pathfinding
        [Header("Manual Neighbors")]
        [SerializeField] protected List<VP_GridTile> m_manualNeighbors = new List<VP_GridTile>(); // This is to be able to assign a tile's neighbors manually when desired

        [Header("GridObjects in Tile")]
        [SerializeField] protected List<VP_GridObject> m_occupyingGridObjects = new List<VP_GridObject>();

        public Vector3 GridObjectsPivotPosition 
        { 
            get 
            { 
                return WorldPosition + m_gridObjectsPivotOffset; 
            } 
        }

        // The Transforms scene position
        public Vector3 WorldPosition 
        { 
            get 
            { 
                return m_mainObject != null ? m_mainObject.position : transform.position; 
            } 
        } 

        public List<VP_GridTile> ManualNeighbors 
        { 
            get 
            { 
                return m_manualNeighbors;
            } 
        } 

        public Vector2Int GridPosition
        {
            get
            {
                return m_gridPosition;
            }

            set
            {
                m_gridPosition = value;
            }
        }

        public Quaternion GridRotation
        {
            get
            {
                return m_gridDefaultRotation;
            }
        }

        public Vector3 GridOffset
        {
            get
            {
                return m_gridObjectsPivotOffset;
            }

        }

        public bool IsTileWalkable
        {
            get
            {
                return m_isTileWalkable;
            }
        }
        
        public int TileHeight
        {
            get
            {
                return m_tileHeight;
            }
            set
            {
                m_tileHeight = value;
            }
        }

        public int TileLayer
        {
            get
            {
                return m_layer;
            }
            set
            {
                m_layer = value;
            }
        }

        public float Priority
        {
            get
            {
                return m_priority;
            }
            set
            {
                m_priority = value;
            }
        }

        public GRID_TILE_TYPES TileType
        {
            get { return m_type; }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (m_initializedTile)
            {
                m_initializedTile = false;
                RemoveTile();
            }
        }

        //  Any GridTile or GridObject requires its GridPosition to be set so it can be initialized
        protected override void Initialize()
        {
            base.Initialize();

            if (!m_initializedTile && m_gridPosition != new Vector2Int(int.MaxValue, int.MaxValue))
            {
                InitializeTile(new Vector3Int(m_gridPosition.x, m_gridPosition.y, m_tileHeight), m_type);
            }
        }

        public virtual void SetType(int _type)
        {
            m_type = (GRID_TILE_TYPES)_type;
        }

        // Method used to initialize the tile
        public virtual void InitializeTile(Vector3Int posInfo, GRID_TILE_TYPES _type)
        {
            m_initializedTile = true;

            m_type = _type;
            // set the positions
            m_gridPosition = posInfo.ToVector2IntXY();
            m_tileHeight = posInfo.z;

            var gridManager = VP_GridManager.Instance;
            if (gridManager != null)
            {               
                // Check if there is already a tile at the target position, if there isn't proceed, if there is and it isn't this destroy the tile
                if (!gridManager.ExistsTileAtPosition(m_gridPosition, out VP_GridTile _otherTile))
                {
                    gridManager.AddGridTile(m_gridPosition, this, this.TileLayer);
                }
                else if (gridManager.GetGridTileAtPosition(m_gridPosition) != this)
                {
                    DestroyImmediate(gameObject);
                }
            }
        }

        // Remove this tile from the tile's dictionary on the GridManager
        protected virtual void RemoveTile()
        {
            var gridManager = VP_GridManager.Instance;
            if (gridManager != null)
                gridManager.RemoveGridTile(TileLayer, this);
        }

        // Checks if a GridObject is able to move to the tile
        public virtual bool CanMoveToTile()
        {
            return (m_isTileWalkable && !IsTileOccupied());
        }

        // Checks if any of the occupying GridObjects block movement
        public virtual bool IsTileOccupied()
        {
            if (m_occupyingGridObjects != null && m_occupyingGridObjects.Count > 0)
            {
                foreach (VP_GridObject obj in m_occupyingGridObjects)
                {
                    if (obj == null)
                        continue;

                    if (obj.BlocksMovement())
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        // adds a GridObject to this tile
        public virtual void AddOccupyingGridObject(VP_GridObject targetGridObject)
        {
            m_occupyingGridObjects.Add(targetGridObject);

            // Invoke the OnGridObjectEnter event
            if (OnGridObjectEnter != null)
            {
                OnGridObjectEnter(targetGridObject, this);
            }
        }

        // Removes a GridObject from this tile
        public virtual void RemoveOccupyingGridObject(VP_GridObject targetGridObject)
        {
            if (m_occupyingGridObjects.Contains(targetGridObject))
            {
                m_occupyingGridObjects.Remove(targetGridObject);
            }

            // Invoke the OnGridObjectExit event
            if (OnGridObjectExit != null)
            {
                OnGridObjectExit(targetGridObject, this);
            }
        }

        // Method which returns the cost of moving to this tile
        public virtual int Cost()
        {
            return m_costOfMovingToTile;
        }

        // Used to set the hovered tile
        protected virtual void OnMouseEnter()
        {
            VP_GridManager.Instance.SetHoveredTile(this);
        }

        // Used to unset the hovered tile
        protected virtual void OnMouseExit()
        {
            VP_GridManager.Instance.UnsetHoveredTile(this);
        }
    }
}
#endif