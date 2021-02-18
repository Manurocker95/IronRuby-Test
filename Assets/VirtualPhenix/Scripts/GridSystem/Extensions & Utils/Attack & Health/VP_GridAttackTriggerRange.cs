#if USE_GRID_SYSTEM
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.GridEngine
{
    public class VP_GridAttackTriggerRange : VP_GridAttackTrigger
    {
        [Header("Range Parameters")]
        public VP_GridRangeParameters m_RangeParameters;


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

        public override List<VP_GridTile> GetTargetTiles()
        {
            var targetTiles = VP_GridRangeAlgorithms.SearchByParameters(_gridObject.CurrentGridTile, m_RangeParameters);

            return targetTiles;
        }

        public override List<Vector2Int> GetTargetPositions()
        {
            var positions = new List<Vector2Int>();

            var targetTiles = GetTargetTiles();

            foreach (VP_GridTile tile in targetTiles)
            {
                if (!positions.Contains(tile.GridPosition))
                {
                    positions.Add(tile.GridPosition);
                }
            }

            return positions;
        }

        public override bool HasVictimAtPosition(Vector2Int position)
        {
            return GetVictimAtPosition(position) != null;
        }
    }

}
#endif