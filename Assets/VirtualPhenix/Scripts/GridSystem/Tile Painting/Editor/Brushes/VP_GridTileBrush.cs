#if USE_GRID_SYSTEM && USE_TILEMAP
using System.Collections; 
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace VirtualPhenix.GridEngine
{
    [System.Serializable]
    public class VP_GridTileBrush
    {
        [Header("Tile Prefab")]
        public VP_GridTile m_gridTile;

        [Header("Layer of the tile")]
        public int m_layer = 0;

        [Header("Height in Grid")]
        public int m_height = 0;   
        
        
        [Header("Tile Type")]
        public GRID_TILE_TYPES m_tileType = GRID_TILE_TYPES.DEFAULT;

        [Header("Position Offset")]
        public Vector3 m_offset = Vector3.zero;

        [Header("Rotation")]
        public Vector3 m_rotation = Vector3.zero;

        public VP_GridTileBrush(VP_GridTileBrush gridTileBrush)
        {
            this.m_layer = gridTileBrush.m_layer;
            this.m_tileType = gridTileBrush.m_tileType;
            this.m_gridTile = gridTileBrush.m_gridTile;
            this.m_height = gridTileBrush.m_height;
            this.m_offset = gridTileBrush.m_offset;
            this.m_rotation = gridTileBrush.m_rotation;
        }

        public VP_GridTileBrush(VP_GridTile gridTileBrush)
        {
            this.m_layer = gridTileBrush.TileLayer;
            this.m_tileType = gridTileBrush.TileType;
            this.m_gridTile = gridTileBrush;
            this.m_height = gridTileBrush.TileHeight;
            this.m_offset = gridTileBrush.GridOffset;
            this.m_rotation = gridTileBrush.GridRotation.eulerAngles;
        }

        public VP_GridTileBrush()
        {

        }

        public void ResetParameters()
        {
            m_layer = 0;
            m_tileType = GRID_TILE_TYPES.DEFAULT;
            m_height = 0;
            m_offset = Vector3.zero;
            m_rotation = Vector3.zero;
        }
    }

}
#endif