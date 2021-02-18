#if USE_GRID_SYSTEM
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace VirtualPhenix.GridEngine
{
    [System.Serializable]
    public class VP_GridObject : VP_MonoBehaviour
    {
        [Header("Grid Position")]
        [SerializeField] protected Vector2Int m_gridPosition = new Vector2Int(int.MaxValue, int.MaxValue);   // Default grid position on a non set GridObject
        [Header("Grid Tile")]
        [SerializeField] protected VP_GridTile m_currentGridTile;  // Reference to the current GridTile this GridObject is at
        [SerializeField] protected VP_GridTile m_previousGridTile; // Reference to the previous Gridtile we were at

        [Header("Initial Facing Relative Position")]
        [SerializeField] protected Orientations m_initialOrientation = Orientations.North;

        [Header("Current Facing Relative Position")]
        [SerializeField] protected Vector2Int m_facingDirection = Vector2Int.zero;          // Current facing direction

        [Header("Block Movement Object")]

        [SerializeField] protected GameObject m_blockerObject;
        [Header("Block Movement Object")]
        [SerializeField] protected VP_GridMovement m_gridMovement;
        protected Vector2Int m_previousFacingDirection;  // Previous facing direction
        protected bool _initialized = false;            // Initialization flag

        [Header("Hover Tile")]
        [SerializeField] protected bool m_hoverTile = false;            // Initialization flag

        protected List<VP_GridMovementBlocker> m_movementBlockers;

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

        public VP_GridTile CurrentGridTile
        {
            get
            {
                return m_currentGridTile;
            }

            set
            {
                m_currentGridTile = value;
            }
        }

        public VP_GridTile PreviousGridTile
        {
            get
            {
                return m_previousGridTile;
            }
        }
         public Orientations InitialOrientation
        {
            get
            {
                return m_initialOrientation;
            }
            set
            {
                m_initialOrientation = value;
            }
        }

        public Vector2Int FacingDirection
        {
            get
            {
                if (m_facingDirection == Vector2Int.zero)
                {
                    m_facingDirection = Vector2Int.up;
                }
                return m_facingDirection;
            }
            set
            {
                PreviousFacingDirection = m_facingDirection;
                m_facingDirection = value;
            }
        }

        public Vector2Int PreviousFacingDirection
        {
            get
            {
                if (m_previousFacingDirection == Vector2Int.zero)
                {
                    m_previousFacingDirection = FacingDirection;
                }
                return m_previousFacingDirection;
            }
            set
            {
                m_previousFacingDirection = value;
            }
        }

        public Vector2Int FacingGridPosition 
        { 
            get 
            { 
                return m_gridPosition + FacingDirection; 
            } 
        }

        public List<VP_GridMovementBlocker> MovementBlockers 
        {
            get
            {
                return m_movementBlockers;
            } 
        }

        protected virtual void Reset()
        {
            m_initializationTime = InitializationTime.OnStart;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (_initialized)
            {
                _initialized = false;
                RemoveFromTile();
                if (VP_GridManager.Instance != null)
                    VP_GridManager.Instance.RemoveGridObject(this);
            }
        }


        // You should always place GridObjects and GridTiles using their respective Brushes which initialize them automatically
        protected override void Initialize()
        {
            base.Initialize();

            if (!m_blockerObject)
            {
                m_blockerObject = this.gameObject;
            }

            if (m_movementBlockers == null || m_movementBlockers.Count == 0)
            {
                m_movementBlockers = m_blockerObject.GetComponents<VP_GridMovementBlocker>().ToList();
            }
        
            if (!m_gridMovement)
                m_gridMovement = GetComponent<VP_GridMovement>();

            // Initial facing direction
            FacingDirection = VP_GridManager.Instance.GetRelativeNeighborPositionFromOrientation(m_initialOrientation.ToString(), (m_gridPosition.y & 1) == 1);
            if (m_gridMovement != null)
            {
                m_gridMovement.Rotate(FacingGridPosition);
            }

            // Initialize
            if (!_initialized && m_gridPosition != new Vector2Int(int.MaxValue, int.MaxValue))
            {
                InitGridObject(new Vector3Int(m_gridPosition.x, m_gridPosition.y, 0));
            }
        }

        public void InitGridObject(Vector3Int? gridPosition = null)
        {
            _initialized = true;

            // Set the GridPosition
            if (gridPosition.HasValue)
            {
                m_gridPosition = gridPosition.Value.ToVector2IntXY();
            }
            else
            {
                // Set the grid position based on the world position
                m_gridPosition = GetGridPosFromWorldPosition();
            }
            var gm = VP_GridManager.Instance;

            // Check if there is a tile at the GridPosition
            if (gm.ExistsTileAtPosition(m_gridPosition, out VP_GridTile _otherTile))
            {
                VP_GridTile gridTile = gm.GetGridTileAtPosition(m_gridPosition);
                SetCurrentGridTile(gridTile);   // Update the current tile on this GridObject
                AddToTile(gridTile);    // Add this Gridobject to the occupant list on the target Gridtile
            }
            else
            {
                Debug.Log("There is not a tile at this GridObject's GridPosition "+m_gridPosition);
            }

            // Add this GridObject to the GridManager's GridObject List
            gm.AddGridObject(this);

            if (m_movementBlockers == null || m_movementBlockers.Count == 0)
            {
                // Get the movement blockers for this GridObject
                m_movementBlockers = GetComponents<VP_GridMovementBlocker>().ToList();
            }
        }

        // Method to get the GridPosition for the objects WorldPosition, this has been deprecated and will be removed in next update
        public virtual Vector2Int GetGridPosFromWorldPosition()
        {
            if (m_currentGridTile == null)
            {
                return base.transform.position.ToVector2IntXZ();
            }

            return (transform.position).ToVector2IntXZ();
        }

        // Sets the current GridTile
        public virtual void SetCurrentGridTile(VP_GridTile targetTile)
        {
            m_previousGridTile = m_currentGridTile;
            m_currentGridTile = targetTile;
        }

        // Unsets the current GridTile
        public virtual void UnsetCurrentGridTile()
        {
            if (m_currentGridTile != null)
            {
                m_currentGridTile = null;
            }
        }

        // Add this Gridobject to the occupant list on the target Gridtile
        public virtual void AddToTile(VP_GridTile gridTile)
        {
            // If the tile exists
            if (gridTile != null)
            {
                // And it is not already occupied
                if (!gridTile.IsTileOccupied())
                {
                    m_currentGridTile.AddOccupyingGridObject(this);
                }
            }
        }

        // Removes this Gridobject of the occupant list on the current GridTile
        public virtual void RemoveFromTile()
        {
            if (m_currentGridTile != null)
            {
                m_currentGridTile.RemoveOccupyingGridObject(this);
            }
        }

        // This is going to be correctly implemented together with directional blocking on the first update
        public virtual bool BlocksMovementFor(VP_GridObject gridObject)
        {
            for (int i = 0; i < m_movementBlockers.Count; i++)
            {
                if (m_movementBlockers[i].TryBlockMovementFor(gridObject))
                {
                    return true;
                }
            }
            return false;
        }

        public virtual bool BlocksMovement()
        {
            for (int i = 0; i < m_movementBlockers.Count; i++)
            {
                if (m_movementBlockers[i] != null && m_movementBlockers[i].m_blocksMovement)
                {
                    return true;
                }
            }
            return false;
        }

        // Used to set the hovered tile
        protected virtual void OnMouseEnter()
        {
            if (m_hoverTile && CurrentGridTile)
                VP_GridManager.Instance.SetHoveredTile(CurrentGridTile);
        }

        // Used to unset the hovered tile
        protected virtual void OnMouseExit()
        {
            if (m_hoverTile && CurrentGridTile)
                VP_GridManager.Instance.UnsetHoveredTile(CurrentGridTile);
        }
    }

}
#endif