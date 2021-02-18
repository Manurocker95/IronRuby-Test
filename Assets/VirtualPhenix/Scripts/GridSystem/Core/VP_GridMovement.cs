#if USE_GRID_SYSTEM
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.GridEngine
{
    [RequireComponent(typeof(VP_GridObject))]
    [ExecuteInEditMode]
    public class VP_GridMovement : VP_MonoBehaviour
    {
        public delegate void StartMovementHandler(VP_GridMovement gridMovement, VP_GridTile startGridTile, VP_GridTile endGridTile);
        public delegate void EndMovementHandler(VP_GridMovement gridMovement, VP_GridTile startGridTile, VP_GridTile endGridTile);
        public event StartMovementHandler OnMovementStart;
        public event EndMovementHandler OnMovementEnd;

        [SerializeField] protected Transform m_mainObject;

        [Header("Movement Cooldown")]
        [Tooltip("Helper for cooldown period between tile movements.")]
        [SerializeField] protected VP_GridCooldown _cooldown;
        public VP_GridCooldown Cooldown { get { return _cooldown; } }

        [Header("Rotating triggers Cooldown")]
        [Tooltip("This will make this object rotate slowly and reset the movement cooldown whenever it does rotate.")]
        [SerializeField]
        public bool _rotationCausesMovementCooldown;

        [Header("Movement Animation")]
        [Tooltip("Time in seconds to animate the movement to another tile")]
        public float MoveAnimDuration = 0.4f;
        public AnimationCurve MoveAnimCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [Range(0f, 1f), Tooltip("The point from 0-1 along the movement animation at which the GridObject occupies the destination tile.")]
        [Header("Occupy Target Tile Treshold")]
        public float SwapPositionThreshold = 0f;

        [Header("Rotation Animation")]
        [Tooltip("Time in seconds to animate to face a new direction, should be snappy and happens before/during movement")]
        public float RotateAnimDuration = 0.1f;
        public AnimationCurve RotateAnimCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [Header("Facing Direction Treshold")]
        [Range(0f, 1f), Tooltip("The point from 0-1 along the movement animation at which the GridObject occupies the destination tile.")]
        public float SwapFacingDirectionThreshold = 0f;
        protected IEnumerator _animateMoveRoutine;
        protected IEnumerator _animateRotateRoutine;
        [SerializeField] protected VP_GridObject m_gridObject;

        [Header("Animations")]
        public Animator m_Animator;
        public string m_MoveAnimationTriggerName = "Move";
        public string m_RotateAnimationTriggerName = "Rotate";

        public bool m_IsMoving
        {
            get;
            protected set;
        }
        public Transform MainObject
        {
            get
            {
                if (m_mainObject == null)
                    m_mainObject = transform;

                return m_mainObject;
            }
        }

        public VP_GridTile m_StartGridTile
        {
            get;
            protected set;
        }

        public VP_GridTile m_TargetGridTile
        {
            get;
            protected set;
        }

        public Vector2Int m_TargetDirection
        {
            get;
            protected set;
        }

        protected override void Initialize()
        {
            if (!m_gridObject)
                m_gridObject = GetComponent<VP_GridObject>();

            base.Initialize();
        }

        protected virtual void Reset()
        {
            if (!m_mainObject)
                m_mainObject = transform;
        }

        protected virtual void Update()
        {
            Cooldown.Update();
        }

        protected virtual void StartMovement(VP_GridTile targetGridTile)
        {
            // Set the movement to true
            m_IsMoving = true;

            // Invoke the onMovementStart event
            if (OnMovementStart != null)
            {
                OnMovementStart(this, m_gridObject.PreviousGridTile, m_gridObject.CurrentGridTile);
            }
        }

        public virtual void EndMovement()
        {
            // Since we've already moved set the target gridtile to null
            m_TargetGridTile = null;
            // Since we're already rotated set the targetdirection to null
            m_TargetDirection = Vector2Int.zero;
            // Reset the ismoving variable
            m_IsMoving = false;

            // Invoke the onMovementEnd event
            if (OnMovementEnd != null)
            {
                OnMovementEnd(this, m_gridObject.PreviousGridTile, m_gridObject.CurrentGridTile);
            }
        }

        // Try to move to a neighbor of the current tile in the target position
        public virtual MovementResult TryMoveToNeighborInPosition(Vector2Int gridPosition, bool animate = true, bool andRotate = true, bool checkMovementCooldown = true)
        {
            var targetGridTile = VP_GridManager.Instance.NeighborAtPosition(m_gridObject.CurrentGridTile, gridPosition);

            return targetGridTile == null ? MovementResult.NoTileAtPosition : TryMoveTo(targetGridTile, animate, andRotate, checkMovementCooldown);
        }

        // Try to move using a target position in the grid
        public virtual MovementResult TryMoveToPosition(Vector2Int gridPosition, bool animate = true, bool andRotate = true, bool checkMovementCooldown = true)
        {
            var targetGridTile = VP_GridManager.Instance.GetGridTileAtPosition(gridPosition);

            return targetGridTile == null ? MovementResult.NoTileAtPosition : TryMoveTo(targetGridTile, animate, andRotate, checkMovementCooldown);
        }

        // Try to move using a target tile in the grid
        public virtual MovementResult TryMoveTo(VP_GridTile targetGridTile, bool animate = true, bool andRotate = true, bool checkMovementCooldown = true)
        {
            // Set the current and target tile
            m_StartGridTile = m_gridObject.CurrentGridTile;
            m_TargetGridTile = targetGridTile;
            // Current position and position we'd like to move to
            Vector2Int fromGridPos = m_gridObject.GridPosition;
            Vector2Int toGridPos = targetGridTile.GridPosition;


            // Checks for the cooldown or if it is moving
            if (checkMovementCooldown && (m_IsMoving || Cooldown.InProgress))
            {
                return MovementResult.Cooldown;
            }

            // Rotation
            if (andRotate)
            {
                // Try to rotate the object in the direction
                var rotated = TryRotateTo(toGridPos, animate);
                // Return the correct movementresult if we actually turned and triggered the cooldown
                if (rotated && _rotationCausesMovementCooldown && Cooldown.InProgress)
                {
                    return MovementResult.Turned;
                }
            }

            // Declare a new local variable to store wether the movement was succesful or not 
            MovementResult movementResult = MovementResult.Moved;
            // Move if possible else return the failed result
            if (m_gridObject.CurrentGridTile != null && targetGridTile.CanMoveToTile())
            {
                var moved = MoveTo(targetGridTile, animate);
                if (!moved)
                {
                    movementResult = MovementResult.Failed;
                }
            }
            else
            {
                movementResult = MovementResult.Failed;
            }

            return movementResult;
        }

        // Don't pass any argument to reset it to its default value (set on the inspector)
        public virtual void TriggerCooldown(float? time = null)
        {
            Cooldown.Reset((!time.HasValue) ? null : time);
        }

        public virtual bool MoveTo(VP_GridTile gridTile, bool animate = true, bool triggerCooldown = true)
        {

            // If the tile is occupied return
            if (gridTile.IsTileOccupied())
            {
                return false;
            }

            // Animated movement
            if (animate)
            {
                AnimateMoveTo(gridTile);
            }
            else
            {
                // Non animated movement (instant)
                // Update the movement variables
                StartMovement(gridTile);
                // Update variables on the owner GridObject
                SwapPosition(gridTile);
                // Set the transform's position to the target GridTile's world position
                MainObject.position = gridTile.GridObjectsPivotPosition;
                // Reset the movement variables
                EndMovement();
            }

            // Trigger the cooldown
            if (triggerCooldown)
            {
                TriggerCooldown();
            }


            return true;
        }

        public virtual bool TryRotateTo(Vector2Int targetPosition, bool animate = true, bool checkMovementCooldown = true)
        {
            /*
            // If we are already facing that direction return
            if (direction == _gridObject.m_FacingDirection)
            {
                return true;
            }
            // If the desired direction is zero return
            if (direction == Vector2Int.zero)
            {
                return false;
            }
            */
            // Checks if the cooldown is in progress
            if (checkMovementCooldown && Cooldown.InProgress)
            {
                return false;
            }

            // Rotate 
            RotateTo(targetPosition, animate);
            return true;
        }

        public virtual void RotateTo(Vector2Int targetPosition, bool animate = true)
        {
            // Set the target direction
            m_TargetDirection = targetPosition;

            // Animated movement
            if (animate)
            {
                AnimateRotateTo(targetPosition);
            }
            else
            {
                // Non animated rotation (instant)
                // Update the transforms rotation
                Rotate(targetPosition);
                // Set the facing direction variable on the owner gridobject
                var relativeDirection = m_gridObject.GridPosition - targetPosition;
                SwapFacingDirection(relativeDirection);
            }
            // Trigger the Cooldown 
            if (_rotationCausesMovementCooldown)
            {
                Cooldown.Reset(null);
            }
        }

        public virtual void AnimateRotateTo(Vector2Int targetPosition)
        {
            // Stop the last animate movement routine if it has not stopped yet
            StopRotateRoutine();

            // Assign and start the animations coroutine
            _animateRotateRoutine = AnimateRotateToRoutine(targetPosition);
            StartCoroutine(_animateRotateRoutine);
        }

        public virtual IEnumerator AnimateRotateToRoutine(Vector2Int targetPosition)
        {
            // Trigger the animation on the animator
            if (m_Animator != null)
            {
                m_Animator.SetTriggerIfExists(m_RotateAnimationTriggerName);
            }
            var relativeDirection = targetPosition - m_gridObject.GridPosition;
            var originallocalRotation = MainObject.localRotation;
            var targetlocalRotation = PositionToRotation(targetPosition);
            float t = 0;
            for (t = Time.deltaTime / RotateAnimDuration; t < 1; t += Time.deltaTime / MoveAnimDuration)
            {
                // Lerp rotation based on the animation curve
                MainObject.localRotation = Quaternion.LerpUnclamped(originallocalRotation, targetlocalRotation, RotateAnimCurve.Evaluate(t));
                // Swap position if we reached the desired threshold
                if (t >= SwapFacingDirectionThreshold)
                {
                    if (m_gridObject.FacingDirection != relativeDirection)
                    {
                        SwapFacingDirection(relativeDirection);
                    }
                }

                // wait for the next frame
                yield return null;
            }

            // Swap the facing rotation
            if (m_gridObject.FacingDirection != relativeDirection)
            {
                SwapFacingDirection(relativeDirection);
            }

            // Set the position to the tile's worldposition
            MainObject.localRotation = targetlocalRotation;
        }

        public virtual void AnimateMoveTo(VP_GridTile targetGridTile)
        {
            // Stop the last animate movement routine if it has not stopped yet
            StopMoveRoutine();

            // Update the movement variables)
            StartMovement(targetGridTile);
            // Assign and start the animations coroutine
            _animateMoveRoutine = AnimateMoveToRoutine(targetGridTile);
            StartCoroutine(_animateMoveRoutine);
        }

        public virtual IEnumerator AnimateMoveToRoutine(VP_GridTile targetGridTile)
        {
            // Trigger the animation on the animator
            if (m_Animator != null)
            {
                m_Animator.SetTriggerIfExists(m_MoveAnimationTriggerName);
            }

            var originalGridTile = m_gridObject.CurrentGridTile;
            float t = 0;
            for (t = Time.deltaTime / MoveAnimDuration; t < 1; t += Time.deltaTime / MoveAnimDuration)
            {
                // Lerp position based on the animation curve
                MainObject.position = Vector3.LerpUnclamped(originalGridTile.GridObjectsPivotPosition, targetGridTile.GridObjectsPivotPosition, MoveAnimCurve.Evaluate(t));
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

            if (m_gridObject.CurrentGridTile != targetGridTile)
            {
                SwapPosition(targetGridTile);
            }

            // Set the position to the tile's worldposition
            MainObject.position = targetGridTile.GridObjectsPivotPosition;

            // Reset the movement variables
            EndMovement();
        }

        public virtual void Rotate(Vector2Int targetPosition)
        {
            MainObject.localRotation = PositionToRotation(targetPosition);
        }

        public virtual Quaternion PositionToRotation(Vector2Int targetPosition)
        {
            var gm = VP_GridManager.Instance;
            var rotation = MainObject.localRotation;
            var initialWorldPosition = gm.Grid.GetCellCenterWorld(m_gridObject.GridPosition.ToVector3IntXY0());
            var targetWorldPosition = gm.Grid.GetCellCenterWorld(targetPosition.ToVector3IntXY0());
            var relativeVector = targetWorldPosition - initialWorldPosition;

            if (gm.Grid.cellSwizzle == GridLayout.CellSwizzle.XZY)
            {
                rotation = Quaternion.LookRotation(relativeVector);
            }
            else if (gm.Grid.cellSwizzle == GridLayout.CellSwizzle.XYZ)
            {
                var rotatedVectorToTarget = Quaternion.Euler(0, 0, 90) * relativeVector;
                rotation = Quaternion.LookRotation(Vector3.forward, rotatedVectorToTarget);
            }

            return rotation;
        }

        // Reset the previous tile, swap your grid position to the new tile and occupy it 
        public virtual void SwapPosition(VP_GridTile targetGridTile)
        {
            // Update variables on the owner GridObject
            m_gridObject.RemoveFromTile();
            m_gridObject.SetCurrentGridTile(targetGridTile);
            m_gridObject.AddToTile(targetGridTile);
            m_gridObject.GridPosition = targetGridTile.GridPosition;
        }

        // Swap the facing direction a new one 
        public virtual void SwapFacingDirection(Vector2Int targetDirection)
        {
            // Update variables on the owner GridObject
            m_gridObject.FacingDirection = targetDirection;
        }

        public virtual bool TryStop()
        {
            if (!m_IsMoving)
            {
                return false;
            }
            CancelMovement();

            return true;
        }

        protected virtual void CancelMovement()
        {
            if (!m_IsMoving)
            {
                return;
            }

            // Resolve position
            if (m_TargetGridTile)
            {
                MainObject.position = (m_TargetGridTile == m_gridObject.CurrentGridTile) ? m_TargetGridTile.GridObjectsPivotPosition : m_StartGridTile.GridObjectsPivotPosition;
            }

            // Resolve rotation
            if (m_TargetDirection != Vector2Int.zero)
            {
                if (m_TargetDirection == m_gridObject.FacingDirection)
                {
                    Rotate(m_TargetDirection);
                }
                else
                {
                    Rotate(m_gridObject.FacingDirection);
                }
            }

            StopMoveRoutine();
            StopRotateRoutine();
            EndMovement();
        }

        protected virtual void StopMoveRoutine()
        {
            if (_animateMoveRoutine == null)
            {
                return;
            }
            StopCoroutine(_animateMoveRoutine);
            _animateMoveRoutine = null;
        }

        protected virtual void StopRotateRoutine()
        {
            if (_animateRotateRoutine == null)
            {
                return;
            }
            StopCoroutine(_animateRotateRoutine);
            _animateRotateRoutine = null;
        }
    }

}
#endif