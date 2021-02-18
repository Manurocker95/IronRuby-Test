#if USE_GRID_SYSTEM
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.GridEngine
{
    public class VP_GridAttackTriggerDirectional : VP_GridAttackTrigger
    {

        [Header("Directions to Affect")]
        public List<Vector2Int> m_TargetDirections = new List<Vector2Int>();

        [Header("Maximun Range")]
        public int m_MaximunRange = 10;
        [Header("Stops Direction On Enemy")]
        public bool m_StopsAtEnemy = true;

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

            if (m_TargetDirections.Count > 0)
            {
                foreach (Vector2Int direction in m_TargetDirections)
                {
                    var directionWithFacingDir = direction.TransformDirection(m_AttackDirection);
                    var startPosition = _gridObject != null ? _gridObject.GridPosition : directionWithFacingDir;
                    for (int i = 1; i <= m_MaximunRange; i++)
                    {
                        var targetPos = startPosition + directionWithFacingDir * i;
                        if (!positions.Contains(targetPos))
                        {
                            positions.Add(targetPos);
                        }

                        if (m_StopsAtEnemy)
                        {
                            var gridObjectAtPos = VP_GridManager.Instance.GetGridObjectAtPosition(targetPos);
                            if (gridObjectAtPos != null)
                            {
                                break;
                            }
                        }
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