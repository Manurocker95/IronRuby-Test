#if USE_GRID_SYSTEM && USE_TILEMAP
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace VirtualPhenix.GridEngine
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "3D Auto Tile Data", menuName = "Virtual Phenix/Grid System/Auto Tile/Auto Tile Data", order = 1)]
    public class VP_GridAutoTileData : VP_ScriptableObject
    {
        public GameObject m_mainPrefab;
        public List<VP_GridAutoTileRule> m_rules = new List<VP_GridAutoTileRule>();
        public List<VP_GridTile> m_friendlyTiles = new List<VP_GridTile>();
        public bool m_isBorderFriendly = false;

        public Vector2Int m_size = new Vector2Int(1, 1);

        public virtual Vector2Int GetSize(int rotation)
        {
            return (rotation % 180 != 0) ? new Vector2Int(m_size.y, m_size.x) : m_size;
        }

        public virtual Tuple<GameObject, int, Vector2> GetTile(Vector2Int position, int rotation)
        {
            var gridManager = VP_GridManager.Instance;
            var realSize = GetSize(rotation);

            var matrix = new VP_GridTile[] 
            {
                    gridManager.GetGridTileAtPosition(new Vector2Int(position.x-realSize.x, position.y+realSize.y)),
                    gridManager.GetGridTileAtPosition(new Vector2Int(position.x, position.y+realSize.y)),
                    gridManager.GetGridTileAtPosition(new Vector2Int(position.x+realSize.x, position.y+realSize.y)),
                    gridManager.GetGridTileAtPosition(new Vector2Int(position.x-realSize.x, position.y)),
                    null,
                    gridManager.GetGridTileAtPosition(new Vector2Int(position.x+realSize.x, position.y)),
                    gridManager.GetGridTileAtPosition(new Vector2Int(position.x-realSize.x, position.y-realSize.y)),
                    gridManager.GetGridTileAtPosition(new Vector2Int(position.x, position.y-realSize.y)),
                    gridManager.GetGridTileAtPosition(new Vector2Int(position.x+realSize.x, position.y-realSize.y))
            };
            
            //pre-process the neighbours to just this or not this
            var neighbours = new GRID_AUTO_TILE_NEIGHBOUR_TYPES[matrix.Length];
            for (int i = 0; i < matrix.Length; i++)
            {
                if (matrix[i] == null)
                {
                    neighbours[i] = m_isBorderFriendly ? GRID_AUTO_TILE_NEIGHBOUR_TYPES.THIS : GRID_AUTO_TILE_NEIGHBOUR_TYPES.NOT_THIS;
                }
                else
                {
                    // matrix[i] == m_parentTile || 
                    neighbours[i] = matrix[i] != null && (m_friendlyTiles.Contains(matrix[i])) ? GRID_AUTO_TILE_NEIGHBOUR_TYPES.THIS : GRID_AUTO_TILE_NEIGHBOUR_TYPES.NOT_THIS;
                }
            }
            //rotate the neighbours to match the current brush rotation
            int rotations = ((360 - rotation) % 360) / 90;
            if (rotations > 0)
                neighbours = VP_GridUtilities.RotateArray(neighbours, new Vector2Int(3, 3), rotations);
            //evaluate the rules until one matches
            foreach (var rule in m_rules)
            {
                if (rule.EvaluateRule(neighbours, out int tileRotation, out Vector2 scale))
                {
                    return Tuple.Create(rule.m_prefab, tileRotation, scale);
                }
            }
            var result = m_mainPrefab;
            return Tuple.Create(result, 0, Vector2.one);
        }
    }
}
#endif