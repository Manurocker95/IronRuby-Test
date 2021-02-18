#if USE_GRID_SYSTEM
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.GridEngine
{
    [RequireComponent(typeof(VP_GridObject))]
    public class VP_InteractiveObject : VP_MonoBehaviour
    {
        [Header("Settings")]
        public VP_GridObject m_GridObject;
        [Header("Target Layer")]
        public LayerMask m_Layers;
        [Header("Activate After Movement Ended")]
        public bool m_WaitTillMovementEnds; // This will be added on update 1.1


        protected void Reset()
        {
            m_Layers = LayerMask.NameToLayer("Everything");
            if (!m_GridObject)
                m_GridObject = GetComponent<VP_GridObject>();
        }

        protected override void Start()
        {
            base.Start();
            if (m_GridObject != null && m_GridObject.CurrentGridTile != null)
            {
                m_GridObject.CurrentGridTile.OnGridObjectEnter += OnGridObjectEnteredTileCallBack;
                m_GridObject.CurrentGridTile.OnGridObjectExit += OnGridObjectExitedTileCallBack;
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (m_GridObject != null && m_GridObject.CurrentGridTile != null)
            {
                m_GridObject.CurrentGridTile.OnGridObjectEnter -= OnGridObjectEnteredTileCallBack;
                m_GridObject.CurrentGridTile.OnGridObjectExit -= OnGridObjectExitedTileCallBack;
            }
        }

        // Callback to invoke the OnEnterTile events when a gridobject with the target layer enters the same gridtile as this
        protected virtual void OnGridObjectEnteredTileCallBack(VP_GridObject gridObject, VP_GridTile gridTile)
        {
            if (0 != (m_Layers.value & 1 << gridObject.gameObject.layer))
            {
                OnEnterTileMethod(gridObject, gridTile);
            }
        }

        // Callback to invoke the OnEnterTile events when a gridobject with the target layer exits the same gridtile as this
        protected virtual void OnGridObjectExitedTileCallBack(VP_GridObject gridObject, VP_GridTile gridTile)
        {
            if (0 != (m_Layers.value & 1 << gridObject.gameObject.layer))
            {
                OnExitTileMethod(gridObject, gridTile);
            }
        }

        protected virtual void OnEnterTileMethod(VP_GridObject gridObject, VP_GridTile gridTile)
        {

        }

        protected virtual void OnExitTileMethod(VP_GridObject gridObject, VP_GridTile gridTile)
        {

        }



    }

}
#endif