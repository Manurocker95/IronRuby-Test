#if USE_GRID_SYSTEM&& USE_TILEMAP
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.GridEngine
{
    [System.Serializable]
    public class VP_GridTileBrushCell
    {
        [SerializeField] protected VP_GridTile m_gridTile;
        [SerializeField] protected int m_height = 0;
        [SerializeField] protected int m_layer = 0;
        [SerializeField] protected GRID_TILE_TYPES m_type = GRID_TILE_TYPES.DEFAULT;
        [SerializeField] protected Vector3 m_offset = Vector3.zero;
        [SerializeField] protected Quaternion m_orientation = Quaternion.identity;

        public VP_GridTile GridTile { get { return m_gridTile; } set { m_gridTile = value; } }
        public int Height { get { return m_height; } set { m_height = value; } }
        public int Layer { get { return m_layer; } set { m_layer = value; } }
        public GRID_TILE_TYPES TileType { get { return m_type; } set { m_type = value; } }
        public Vector2 Offset { get { return m_offset; } set { m_offset = value; } }
        public Quaternion Orientation { get { return m_orientation; } set { m_orientation = value; } }

        public override int GetHashCode()
        {
            int hash = 0;
            unchecked
            {
                hash = GridTile != null ? GridTile.GetInstanceID() : 0;
                hash = hash * 33 + m_orientation.GetHashCode();
            }
            return hash;
        }
    }
}
#endif