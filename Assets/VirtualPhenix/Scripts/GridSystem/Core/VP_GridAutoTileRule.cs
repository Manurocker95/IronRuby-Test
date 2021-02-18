#if USE_GRID_SYSTEM && USE_TILEMAP
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.GridEngine
{
    public enum GRID_AUTO_TILE_NEIGHBOUR_TYPES
    {
        ANY = 0,
        THIS = 1,
        NOT_THIS = 2
    }

    public enum RULE_FLIP_TYPE
    {
        NONE,
        ROTATE,
        FLIP_X,
        FLIP_Y,
        FLIP_XY
    }

    [System.Serializable]
    public class VP_GridAutoTileRule 
    {
        public RULE_FLIP_TYPE m_flipType;
        public GameObject m_prefab;
        public GRID_AUTO_TILE_NEIGHBOUR_TYPES[] m_neighbourRules = new GRID_AUTO_TILE_NEIGHBOUR_TYPES[9];

        public VP_GridAutoTileRule()
        {
            m_neighbourRules = new GRID_AUTO_TILE_NEIGHBOUR_TYPES[9];
        }
        public virtual bool EvaluateRule(GRID_AUTO_TILE_NEIGHBOUR_TYPES[] neighbours, out int tileRotation, out Vector2 scale)
        {
            tileRotation = 0;
            scale = new Vector2(1, 1);
            if (Match(m_neighbourRules, neighbours))
            {
                return true;
            }
            else if (m_flipType == RULE_FLIP_TYPE.FLIP_X)
            {
                if (Match(VP_GridUtilities.FlipArrayX(m_neighbourRules, new Vector2Int(3, 3)), neighbours))
                {
                    scale.x = -1;
                    return true;
                }
            }
            else if (m_flipType == RULE_FLIP_TYPE.FLIP_Y)
            {
                if (Match(VP_GridUtilities.FlipArrayY(m_neighbourRules, new Vector2Int(3, 3)), neighbours))
                {
                    scale.y = -1;
                    return true;
                }
            }
            else if (m_flipType == RULE_FLIP_TYPE.FLIP_XY)
            {
                if (Match(VP_GridUtilities.FlipArrayX(m_neighbourRules, new Vector2Int(3, 3)), neighbours))
                {
                    scale.x = -1;
                    return true;
                }
                else if (Match(VP_GridUtilities.FlipArrayY(m_neighbourRules, new Vector2Int(3, 3)), neighbours))
                {
                    scale.y = -1;
                    return true;
                }
            }
            else if (m_flipType == RULE_FLIP_TYPE.ROTATE)
            {
                if (Match(VP_GridUtilities.RotateArray(m_neighbourRules, new Vector2Int(3, 3), 1), neighbours))
                {
                    tileRotation = 90;
                    return true;
                }
                else if (Match(VP_GridUtilities.RotateArray(m_neighbourRules, new Vector2Int(3, 3), 2), neighbours))
                {
                    tileRotation = 180;
                    return true;
                }
                else if (Match(VP_GridUtilities.RotateArray(m_neighbourRules, new Vector2Int(3, 3), 3), neighbours))
                {
                    tileRotation = 270;
                    return true;
                }
            }
            return false;
        }
        protected virtual bool Match(GRID_AUTO_TILE_NEIGHBOUR_TYPES[] _toMatch, GRID_AUTO_TILE_NEIGHBOUR_TYPES[] _neighbours)
        {
            for (int i = 0; i < _toMatch.Length; i++)
            {
                if (!EvaluateNeighbour(_toMatch[i], _neighbours[i]))
                    return false;
            }
            return true;
        }

        public virtual bool EvaluateNeighbour(GRID_AUTO_TILE_NEIGHBOUR_TYPES rule, GRID_AUTO_TILE_NEIGHBOUR_TYPES neighbour)
        {
            return rule == GRID_AUTO_TILE_NEIGHBOUR_TYPES.ANY || rule == neighbour;
        }
    }

}
#endif