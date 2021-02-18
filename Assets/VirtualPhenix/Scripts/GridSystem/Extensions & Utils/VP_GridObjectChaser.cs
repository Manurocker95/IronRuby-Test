#if USE_GRID_SYSTEM
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.GridEngine
{

    public class VP_GridObjectChaser : MonoBehaviour
    {

        public List<VP_GridTile> m_PathToTarget = new List<VP_GridTile>();

        [Header("Sight Range Parameters")]
        public VP_GridRangeParameters m_SightRangeParamters;

        [Header("Target Settings")]
        public VP_GridObject m_TargetToChase;
        //[HideInInspector]
        public VP_GridObject m_CurrentTarget;

        [Header("Update Interval")]
        public float m_UpdateInterval = 0.3f;

        protected float _intervalTimeLeft;
        protected VP_GridObject _gridObject;
        protected VP_GridMovement _gridMovement;
        [Header("Attack")]
        public bool TryToAttackWhenPossible;
        public VP_GridAttack m_Attack;

        protected virtual void Awake()
        {
            _gridObject = GetComponent<VP_GridObject>();
            _gridMovement = GetComponent<VP_GridMovement>();
            _intervalTimeLeft = m_UpdateInterval;
        }

        protected virtual void Update()
        {
            // Interval timer
            if (_intervalTimeLeft > 0f)
            {
                _intervalTimeLeft = _intervalTimeLeft - Time.deltaTime;
                if (_intervalTimeLeft <= 0f)
                {
                    // Set the timer
                    _intervalTimeLeft = m_UpdateInterval;
                }
                else
                {
                    return;
                }
            }

            GetTargetInRange();
            ChaseTarget();
        }

        public virtual void GetTargetInRange()
        {
            // Reset the current target
            if (m_CurrentTarget != null)
            {
                ResetTarget();
            }

            var tilesInRange = VP_GridRangeAlgorithms.SearchByParameters(_gridObject.CurrentGridTile, m_SightRangeParamters);
            // Chase the chosen target when it is in range
            if (m_TargetToChase != null)
            {
                foreach (VP_GridTile tile in tilesInRange)
                {
                    var gridObjectAtTile = VP_GridManager.Instance.GetGridObjectAtPosition(tile.GridPosition);
                    if (gridObjectAtTile != null && gridObjectAtTile == m_TargetToChase && gridObjectAtTile != _gridObject)
                    {
                        SetTarget(gridObjectAtTile);
                        break;
                    }
                }
            }
        }

        public virtual void SetTarget(VP_GridObject targetGridObject)
        {
            m_CurrentTarget = targetGridObject;
        }

        public virtual void ResetTarget()
        {
            m_CurrentTarget = null;
        }

        protected virtual void ChaseTarget()
        {
            m_PathToTarget.Clear();
            if (m_CurrentTarget == null)
            {
                GetTargetInRange();
            }

            if (m_CurrentTarget == null)
            {
                return;
            }

            // Attack
            if (TryToAttackWhenPossible)
            {
                var targetPosition = m_CurrentTarget.GridPosition;
                var gridObjectAtPosition = VP_GridManager.Instance.GetGridObjectAtPosition(targetPosition);
                if (gridObjectAtPosition != null && m_Attack != null)
                {
                    var healthComponents = m_Attack.GetVictimsFromTriggerAtPosition(targetPosition);
                    if (healthComponents != null && healthComponents.Count > 0)
                    {
                        var attkResult = m_Attack.TryAttack(targetPosition);
                        if (attkResult == AttackResult.Success)
                        {
                            return;
                        }
                    }
                }
            }

            // Movement
            if (_gridObject.GridPosition.GridDistance(m_CurrentTarget.GridPosition) > 1)
            {
                // Calculate a path and try moving towards the target
                m_PathToTarget = VP_GridAStar.Search(_gridObject.CurrentGridTile, m_CurrentTarget.CurrentGridTile);
                if (m_PathToTarget != null && m_PathToTarget.Count > 0 && m_PathToTarget.Contains(m_CurrentTarget.CurrentGridTile))
                {
                    _gridMovement.TryMoveTo(m_PathToTarget[0]);
                }
            }
            else // Rotation
            {
                var targetDirection = m_CurrentTarget.GridPosition;
                var rotated = _gridMovement.TryRotateTo(targetDirection);
            }
        }
    }

}
#endif