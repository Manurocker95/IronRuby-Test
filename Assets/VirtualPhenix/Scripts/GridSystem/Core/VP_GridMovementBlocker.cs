#if USE_GRID_SYSTEM
using UnityEngine;

namespace VirtualPhenix.GridEngine
{
    [RequireComponent(typeof(VP_GridObject))]
    [System.Serializable]
    public class VP_GridMovementBlocker : VP_MonoBehaviour
    {
        [Header("Block movement"),Space]
        [Tooltip("Stops entities from moving into this grid object's position.")]
        public bool m_blocksMovement = true; 

        [SerializeField] protected VP_GridObject m_ownerGridObject;
        public VP_GridObject OwnerGridObject { get { return m_ownerGridObject; } }

        protected virtual void Reset()
        {
            m_ownerGridObject = GetComponent<VP_GridObject>();
        }

        protected override void Initialize()
        {
            base.Initialize();

            if (!m_ownerGridObject)
                m_ownerGridObject = GetComponent<VP_GridObject>();
        }

        public virtual bool TryBlockMovementFor(VP_GridObject targetGridObject)
        {
            if (!m_blocksMovement)
            {
                return false;
            }
            
            if (!targetGridObject)
            {
                return true;
            }

            return true;
        }
    }

}
#endif