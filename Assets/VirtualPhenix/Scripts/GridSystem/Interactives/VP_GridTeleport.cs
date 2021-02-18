#if USE_GRID_SYSTEM
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.GridEngine
{
    public class VP_GridTeleport : VP_InteractiveObject
    {
        [Header("Teleport Settings")]
        public bool m_Activable = true;
        public VP_GridTeleport m_TargetTeleport;
        [Header("Direction to face when teleporting from this")]
        public Vector2Int m_DirectionToFaceOnTeleport;

        protected List<VP_GridObject> _ignoreList = new List<VP_GridObject>();

        protected override void OnEnterTileMethod(VP_GridObject gridObject, VP_GridTile gridTile)
        {
            if (!m_Activable)
                return;

            // Check if it is on the ignore list
            if (_ignoreList.Contains(gridObject))
            {
                return;
            }

            // Check if the target gridObject is teleportable
            if (IsTeleportable(gridObject))
            {
                Activate(gridObject);
            }
        }

        protected override void OnExitTileMethod(VP_GridObject gridObject, VP_GridTile gridTile)
        {
            RemoveFromIgnoreList(gridObject);
        }

        protected virtual void Activate(VP_GridObject gridObject)
        {
            AddToIgnoreList(gridObject);
            m_TargetTeleport.AddToIgnoreList(gridObject);
            var destinationTile = m_TargetTeleport.m_GridObject.CurrentGridTile;
            var gridMovement = gridObject.GetComponent<VP_GridMovement>();

            StartCoroutine(ActivateRoutine(destinationTile, gridMovement));
        }

        protected virtual IEnumerator ActivateRoutine(VP_GridTile destinationTile, VP_GridMovement gridMovement)
        {
            yield return null;

            // Stop the current movement
            gridMovement.TryStop();

            // Wait for 1 frame till the movement actually stops
            //yield return null;

            gridMovement.TryMoveTo(destinationTile, false, false, false);
            gridMovement.RotateTo(m_TargetTeleport.m_GridObject.GridPosition + m_DirectionToFaceOnTeleport, false);
        }

        protected virtual bool IsTeleportable(VP_GridObject gridObject)
        {
            // Check if the gridObject has a movement component attached to it
            var gridMovement = gridObject.GetComponent<VP_GridMovement>();
            if (!gridMovement)
                return false;
            // Check if there is a target tile
            var destinationTile = m_TargetTeleport.m_GridObject.CurrentGridTile;
            if (!destinationTile)
                return false;
            // Check if it is occupied
            if (!destinationTile.CanMoveToTile())
                return false;

            return true;
        }

        // Adds an object to the ignore list, which will prevent that object to be moved by the teleporter while it's in that list
        public virtual void AddToIgnoreList(VP_GridObject gridObjectToIgnore)
        {
            _ignoreList.Add(gridObjectToIgnore);
        }

        // Adds an object to the ignore list, which will prevent that object to be moved by the teleporter while it's in that list
        public virtual void RemoveFromIgnoreList(VP_GridObject gridObjectToIgnore)
        {
            if (_ignoreList.Contains(gridObjectToIgnore))
                _ignoreList.Remove(gridObjectToIgnore);
        }
    }

}
#endif