#if USE_GRID_SYSTEM && USE_TILEMAP
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using NaughtyAttributes;

namespace VirtualPhenix.GridEngine
{
    [System.Serializable]
    public class VP_GridObjectBrush
    {
        [Header("Tile Prefab")]
        public VP_GridObject m_GridObject;

        [Header("Initial Orientation")]
        public Orientations m_InitialOrientation = Orientations.North;

        public VP_GridObjectBrush(VP_GridObjectBrush gridTileBrush)
        {
            this.m_GridObject = gridTileBrush.m_GridObject;
            this.m_InitialOrientation = gridTileBrush.m_InitialOrientation;
        }

        public VP_GridObjectBrush() { }

        public void ResetParameters()
        {
            m_InitialOrientation = Orientations.North;
        }
    }

}
#endif