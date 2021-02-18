#if USE_GRID_SYSTEM
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace VirtualPhenix.GridEngine
{
    public class InteractOnGridObject : VP_InteractiveObject
    {
        [Header("Events")]
        public UnityEvent m_OnEnterTile, m_OnExitTile;

        protected override void OnEnterTileMethod(VP_GridObject gridObject, VP_GridTile gridTile)
        {
            m_OnEnterTile.Invoke();
        }

        protected override void OnExitTileMethod(VP_GridObject gridObject, VP_GridTile gridTile)
        {
            m_OnExitTile.Invoke();
        }
    }

}
#endif