#if USE_GRID_SYSTEM
using System.Collections; 
using System.Collections.Generic; 
using UnityEngine; 
using UnityEditor; 
using NaughtyAttributes; 


namespace VirtualPhenix.GridEngine
{

    [RequireComponent(typeof(VP_GridMovement))]
    public class VP_GridCharacterPathfinder : VP_MonoBehaviour
    {

        [Header("Settings")]
        [SerializeField] protected bool m_followPath = false;
        [SerializeField] protected VP_GridTile m_targetGridTile;

        [Header("Movement Settings")]
        [SerializeField] protected bool m_animateMovement = true;
        [SerializeField] protected bool m_rotateTowardsDirection = false;
        [SerializeField] protected bool m_checksMovementCooldown = true;
        [SerializeField] protected bool m_debugDrawPath = false;

        //[Header("Debug")]
        /// Whether or not we should draw debug spheres to show the current path of the character (on the inspector)
        //public bool m_DebugDrawPath;
        /// The current path
        [SerializeField] protected List<VP_GridTile> m_path = new List<VP_GridTile>();

        /// The index of the next point target point in the path
        [SerializeField] protected int m_nextPathpointIndex;

        [SerializeField] protected VP_GridObject m_gridObject;
        [SerializeField] protected VP_GridMovement m_gridMovement;

        /// <summary>
        /// On Awake we grab our components
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            if (!m_gridMovement)
                m_gridMovement = GetComponent<VP_GridMovement>();

            if (!m_gridObject)
                m_gridObject = GetComponent<VP_GridObject>();
        }

        /// <summary>
        /// Sets a new destination the character will pathfind to
        /// </summary>
        /// <param name="destinationGridTile"></param>
        public virtual void SetNewDestination(VP_GridTile destinationGridTile, bool startMoving = true)
        {
            m_targetGridTile = destinationGridTile;
            DeterminePath(m_gridObject.CurrentGridTile, destinationGridTile);
            if (startMoving)
            {
                StartMoving();
            }
        }

        public virtual void StartMoving()
        {
            if (!m_followPath)
                m_followPath = true;
        }

        public virtual void StopMoving()
        {
            if (m_followPath)
                m_followPath = false;
        }

        /// <summary>
        /// On Update, we draw the path if needed, determine the next waypoint, and move to it if needed
        /// </summary>
        protected virtual void Update()
        {
            if (!m_followPath || m_targetGridTile == null)
            {
                return;
            }
            DetermineNextPathpoint();
            MoveGridObject();
        }

        /// <summary>
        /// Moves the controller towards the next point
        /// </summary>
        protected virtual void MoveGridObject()
        {
            if ((m_targetGridTile == null) || (m_nextPathpointIndex < 0) || m_path.Count <= 0)
            {
                return;
            }
            else
            {
                m_gridMovement.TryMoveTo(m_path[m_nextPathpointIndex], m_animateMovement, m_rotateTowardsDirection, m_checksMovementCooldown);
            }
        }

        /// <summary>
        /// Determines the path to the target GridTile. NextPathPointIndex will be -1 if a path couldn't be found
        /// </summary>
        /// <param name="startingGridTile"></param>
        /// <param name="targetGridTile"></param>
        /// <returns></returns>
        protected virtual void DeterminePath(VP_GridTile startingGridTile, VP_GridTile targetGridTile)
        {
            m_nextPathpointIndex = -1;

            m_path = VP_GridAStar.Search(startingGridTile, targetGridTile);
            if (m_path != null && m_path.Count > 0 && m_path.Contains(targetGridTile))
            {

                m_nextPathpointIndex = 0;
            }
        }

        /// <summary>
        /// Determines the next path point 
        /// </summary>
        protected virtual void DetermineNextPathpoint()
        {
            if (m_path == null)
            {
                return;
            }

            if (m_path.Count <= 0)
            {
                return;
            }
            if (m_nextPathpointIndex < 0)
            {
                return;
            }

            if (m_gridObject.GridPosition.GridDistance(m_path[m_nextPathpointIndex].GridPosition) <= 0)
            {
                if (m_nextPathpointIndex + 1 < m_path.Count)
                {
                    m_nextPathpointIndex++;
                }
                else
                {
                    m_nextPathpointIndex = -1;
                }
            }
            else
            {
                // Try to recalculate the path since the current one is blocked
                DeterminePath(m_gridObject.CurrentGridTile, m_targetGridTile);
            }
        }
        
#if UNITY_EDITOR

        /// <summary>
        /// Draws wire spheres on top of each tile in the current path and a line to visualize the path
        /// </summary>
        protected virtual void OnDrawGizmosSelected() 
        {
            if (m_debugDrawPath) {
                if (m_path != null && m_path.Count > 0) {
                    Handles.color = Color.grey;
                    for (int i = 0; i < m_path.Count; i++) {
                        var height = Vector3.up * .5f;

                        if (i == m_path.Count - 1)
                            Handles.color = Color.green;

                        Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
                        Handles.DrawWireCube(m_path[i].WorldPosition + height, Vector3.one * 0.3f);
                        Handles.CubeHandleCap(0, m_path[i].WorldPosition + height, Quaternion.identity, 0.3f, EventType.Repaint);

                        if (i > 0)
                            Debug.DrawLine(m_path[i - 1].WorldPosition + height, m_path[i].WorldPosition + height, Color.yellow, 0.0f, true);
                    }
                }
            }
        }
#endif 

    }
}
#endif