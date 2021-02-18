#if USE_GRID_SYSTEM
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.GridEngine
{

    public abstract class VP_GridAttackTrigger : MonoBehaviour
    {

        [HideInInspector] public VP_GridAttack m_Attack;

        [Header("Fixed Attack Direction")]
        public bool m_OverrideAttackDirection;
        public Vector2Int m_CustomAttackDirection = Vector2Int.up;
        public Vector2Int m_AttackDirection { get; private set; }

        protected VP_GridObject _gridObject;

        protected virtual void Start()
        {
            if (m_Attack)
            {
                _gridObject = m_Attack.m_GridObject;
            }
        }

        public virtual bool TrySetAttackDirection(Vector2Int direction)
        {
            if (m_OverrideAttackDirection)
            {
                m_AttackDirection = m_CustomAttackDirection;
                return false;
            }

            m_AttackDirection = direction;
            return true;
        }

        public abstract List<VP_GridHealth> GetVictims();

        public abstract VP_GridHealth GetVictimAtPosition(Vector2Int position);

        public abstract List<Vector2Int> GetTargetPositions();

        public abstract List<VP_GridTile> GetTargetTiles();

        public abstract bool HasVictimAtPosition(Vector2Int position);
    }

}
#endif