#if USE_GRID_SYSTEM
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.GridEngine
{
    public class VP_GridAttackTriggerPosition : VP_GridAttackTrigger
    {
        [Header("Positions to Affect")]
        public List<Vector2Int> m_TargetGridPositions = new List<Vector2Int>();

        public override List<VP_GridHealth> GetVictims()
        {
            List<VP_GridHealth> victims = new List<VP_GridHealth>();

            var targetPositions = GetTargetPositions();
            foreach (Vector2Int targetPos in targetPositions)
            {
                var gridObjectAtPos = VP_GridManager.Instance.GetGridObjectAtPosition(targetPos);
                if (gridObjectAtPos != null)
                {
                    var healthComp = gridObjectAtPos.GetComponent<VP_GridHealth>();
                    if (healthComp != null && !victims.Contains(healthComp))
                    {
                        victims.Add(healthComp);
                    }
                }
            }

            return victims;
        }

        public override VP_GridHealth GetVictimAtPosition(Vector2Int position)
        {
            var targetPositions = GetTargetPositions();
            foreach (Vector2Int targetPos in targetPositions)
            {
                if (targetPos != position)
                {
                    continue;
                }

                var gridObjectAtPos = VP_GridManager.Instance.GetGridObjectAtPosition(targetPos);
                if (gridObjectAtPos != null)
                {
                    var healthComp = gridObjectAtPos.GetComponent<VP_GridHealth>();
                    if (healthComp != null)
                    {
                        return healthComp;
                    }
                }
            }

            return null;
        }

        public override List<Vector2Int> GetTargetPositions()
        {
            var positions = new List<Vector2Int>();

            if (m_TargetGridPositions.Count > 0)
            {
                foreach (Vector2Int pos in m_TargetGridPositions)
                {
                    var posWithFacingDir = pos.TransformDirection(m_AttackDirection);
                    var targetPos = _gridObject != null ? _gridObject.GridPosition + posWithFacingDir : posWithFacingDir;
                    if (!positions.Contains(targetPos))
                    {
                        positions.Add(targetPos);
                    }
                }
            }

            return positions;
        }

        public override List<VP_GridTile> GetTargetTiles()
        {
            var positions = GetTargetPositions();
            var tiles = new List<VP_GridTile>();

            foreach (Vector2Int pos in positions)
            {
                var tileAtPosition = VP_GridManager.Instance.GetGridTileAtPosition(pos);
                if (tileAtPosition && !tiles.Contains(tileAtPosition))
                {
                    tiles.Add(tileAtPosition);
                }
            }

            return tiles;
        }

        public override bool HasVictimAtPosition(Vector2Int position)
        {
            return GetVictimAtPosition(position) != null;
        }
    }

}
#endif