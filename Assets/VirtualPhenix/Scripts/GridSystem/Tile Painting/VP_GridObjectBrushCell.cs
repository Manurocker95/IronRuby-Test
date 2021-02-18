#if USE_GRID_SYSTEM&& USE_TILEMAP
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.GridEngine
{
    [System.Serializable]
    public class VP_GridObjectBrushCell
    {

        [SerializeField] protected VP_GridObject m_gridObject;
        [SerializeField] protected Orientations m_orientation = Orientations.North;
        [SerializeField] protected int m_layer;

        public VP_GridObject GridObject { get { return m_gridObject; } set { m_gridObject = value; } }
        public Orientations Orientation { get { return m_orientation; } set { m_orientation = value; } }
        public int Layer { get { return m_layer; } set { m_layer = value; } }

        public override int GetHashCode()
        {
            int hash = 0;
            unchecked
            {
                hash = GridObject != null ? GridObject.GetInstanceID() : 0;
            }
            return hash;
        }
    }

}
#endif