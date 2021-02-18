#if USE_GRID_SYSTEM
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.GridEngine
{
    public class VP_GridRotatingCubeMovement : VP_GridMovement
    {

        [Header("Model To Rotate")]
        public Transform m_ModelHolder;

        public Quaternion m_TargetRotation
        {
            get;
            protected set;
        }
        protected bool _animateRoutineFinished = true;

        protected override void Update()
        {
            base.Update();
        }

        public override IEnumerator AnimateMoveToRoutine(VP_GridTile targetGridTile)
        {
            // Storing the origin grid tile for later
            var originalGridTile = m_gridObject.CurrentGridTile;
            // Rotating 
            var directionOfMovement = targetGridTile.GridPosition - originalGridTile.GridPosition;
            var forward = new Vector3(0, -directionOfMovement.y, 0);
            var up = new Vector3(directionOfMovement.x, 0f, 0f);
            // Only rotate around the direction axis (forward or sideways)
            var rotationAdd = forward != Vector3.zero ? Quaternion.LookRotation(forward) : Quaternion.LookRotation(Vector3.forward, up);

            // Store the starting and final rotations to lerp between them
            var startingRotation = m_ModelHolder.localRotation;
            var finalRotation = rotationAdd * m_ModelHolder.localRotation;
            // Store rotation in case the movement is cancelled
            m_TargetRotation = finalRotation;
            _animateRoutineFinished = false;

            float t = 0;
            for (t = Time.deltaTime / MoveAnimDuration; t < 1; t += Time.deltaTime / MoveAnimDuration)
            {
                // Rotating animation
                m_ModelHolder.localRotation = Quaternion.LerpUnclamped(startingRotation, finalRotation, MoveAnimCurve.Evaluate(t));
                // Lerp position based on the animation curve
                m_mainObject.position = Vector3.LerpUnclamped(originalGridTile.GridObjectsPivotPosition, targetGridTile.GridObjectsPivotPosition, MoveAnimCurve.Evaluate(t));
                // Swap position if we reached the desired threshold
                if (t >= SwapPositionThreshold)
                {
                    if (m_gridObject.CurrentGridTile != targetGridTile)
                    {
                        SwapPosition(targetGridTile);
                    }
                }
                // wait for the next frame
                yield return null;
            }
            // Set the position to the tile's worldposition
            m_mainObject.position = targetGridTile.GridObjectsPivotPosition;
            // Set the rotation to the final rotation
            m_ModelHolder.localRotation = finalRotation;
            // Reset the movement cancel trigger
            _animateRoutineFinished = true;
            // Reset the movement variables
            EndMovement();
        }

        protected override void CancelMovement()
        {
            if (!m_IsMoving)
            {
                return;
            }

            // Resolve position
            if (m_TargetGridTile)
            {
                m_mainObject.position = (m_TargetGridTile == m_gridObject.CurrentGridTile) ? m_TargetGridTile.GridObjectsPivotPosition : m_StartGridTile.GridObjectsPivotPosition;
            }

            // Resolve rotation
            if (!_animateRoutineFinished)
            {
                m_ModelHolder.localRotation = m_TargetRotation;
                _animateRoutineFinished = true;
            }

            StopMoveRoutine();
            StopRotateRoutine();
            EndMovement();
        }

        public override void Rotate(Vector2Int targetDirection)
        {
            return;
        }
    }

}
#endif