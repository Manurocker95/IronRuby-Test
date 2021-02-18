#if USE_GRID_SYSTEM && USE_TILEMAP
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.GridEngine
{
    public class VP_GridAutoTile : VP_GridTile
    {
        [SerializeField] protected VP_GridAutoTileData m_tileData;
    }
}
#endif