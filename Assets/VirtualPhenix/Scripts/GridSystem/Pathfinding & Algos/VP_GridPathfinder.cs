#if USE_GRID_SYSTEM
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace VirtualPhenix.GridEngine
{
    public class VP_GridPathfinder : VP_MonoBehaviour
    {

        [Header("Target GridObject")]
        /// the target the character should pathfind to
        public VP_GridObject m_TargetGridObject;
        [Header("Target GridTile")]
        /// The goal Tile
        public VP_GridTile m_DestinationTile;

        [Header("Movement Settings")]
        public bool m_AnimateMovement = false;
        public bool m_RotateTowardsDirection = false;

        [Header("Debug")]
        /// Whether or not we should draw debug spheres to show the current gridtiles path of the character (on the inspector)
        public bool m_DebugDrawPath;
        /// The index of the next point target point in the path
        public int m_NextPathpointIndex;

        /// The current path
        public List<VP_GridTile> Path = new List<VP_GridTile>();

        protected VP_GridObject _gridObject;
        protected VP_GridMovement _gridMovement;

        /// <summary>
        /// On Awake we grab our components
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            _gridMovement = GetComponent<VP_GridMovement>();
            _gridObject = GetComponent<VP_GridObject>();
        }

        /// <summary>
        /// Sets a new destination the character will pathfind to
        /// </summary>
        /// <param name="destinationGridTile"></param>
        public virtual void SetNewDestination(VP_GridTile destinationGridTile)
        {
            m_DestinationTile = destinationGridTile;
            DeterminePath(_gridObject.CurrentGridTile, destinationGridTile);
        }

        /// <summary>
        /// On Update, we draw the path if needed, determine the next waypoint, and move to it if needed
        /// </summary>
        protected virtual void Update()
        {
            if (m_DestinationTile == null)
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
            if ((m_DestinationTile == null) || (m_NextPathpointIndex < 0) || Path.Count <= 0)
            {
                return;
            }
            else
            {
                _gridMovement.TryMoveTo(Path[m_NextPathpointIndex], m_AnimateMovement, m_RotateTowardsDirection);
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
            m_NextPathpointIndex = -1;

            Path = VP_GridAStar.Search(startingGridTile, targetGridTile);
            if (Path != null && Path.Count > 0 && Path.Contains(targetGridTile))
            {

                m_NextPathpointIndex = 0;
            }
        }

        /// <summary>
        /// Determines the next path point 
        /// </summary>
        protected virtual void DetermineNextPathpoint()
        {
            if (Path == null)
            {
                return;
            }

            if (Path.Count <= 0)
            {
                return;
            }
            if (m_NextPathpointIndex < 0)
            {
                return;
            }

            if (_gridObject.GridPosition.GridDistance(Path[m_NextPathpointIndex].GridPosition) <= 0)
            {
                if (m_NextPathpointIndex + 1 < Path.Count)
                {
                    m_NextPathpointIndex++;
                }
                else
                {
                    m_NextPathpointIndex = -1;
                }
            }
            else
            {
                // Try to recalculate the path since the current one is blocked
                DeterminePath(_gridObject.CurrentGridTile, m_DestinationTile);
            }
        }

#if UNITY_EDITOR

        /// <summary>
        /// Draws wire spheres on top of each tile in the current path and a line to visualize the path
        /// </summary>
        protected virtual void OnDrawGizmosSelected()
        {
            if (m_DebugDrawPath)
            {
                if (Path != null && Path.Count > 0)
                {
                    Handles.color = Color.grey;
                    for (int i = 0; i < Path.Count; i++)
                    {
                        var height = Vector3.up * .5f;

                        if (i == Path.Count - 1)
                            Handles.color = Color.green;

                        Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
                        Handles.DrawWireCube(Path[i].WorldPosition + height, Vector3.one * 0.3f);
                        Handles.CubeHandleCap(0, Path[i].WorldPosition + height, Quaternion.identity, 0.3f, EventType.Repaint);

                        if (i > 0)
                            Debug.DrawLine(Path[i - 1].WorldPosition + height, Path[i].WorldPosition + height, Color.yellow, 0.0f, true);
                    }
                }
            }
        }
#endif
    }
}
#endif