#if USE_GRID_SYSTEM
using UnityEngine; 
using System.Collections; 

namespace VirtualPhenix.GridEngine
{
    // Allows you to click anywhere on screen, which will determine a new target and the character will pathfind its way to it
    [RequireComponent(typeof(VP_GridCharacterPathfinder))]
    public class VP_GridMouseDrivenController : MonoBehaviour
    {

        protected VP_GridCharacterPathfinder _characterPathfinder;

        // On awake we get the GridPathfinder component
        protected virtual void Awake()
        {
            _characterPathfinder = GetComponent<VP_GridCharacterPathfinder>();
        }

        // On Update we look for a mouse click
        protected virtual void Update()
        {
            DetectMouse();
        }

        // If the mouse is clicked, we make the currently hovered tile the pathfinding target
        protected virtual void DetectMouse()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (VP_GridManager.Instance.m_HoveredGridTile != null)
                {
                    _characterPathfinder.SetNewDestination(VP_GridManager.Instance.m_HoveredGridTile);
                }
            }
        }
    }
}

#endif