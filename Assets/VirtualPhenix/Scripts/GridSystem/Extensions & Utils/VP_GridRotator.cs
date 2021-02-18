#if USE_GRID_SYSTEM
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.GridEngine
{
    public class VP_GridRotator : MonoBehaviour
    {
        [Header("Settings")]
        public bool m_Activated = false;
        public Vector3 m_DegreesPerSecond = new Vector3(0f, 15f, 0f);

        protected void Update()
        {
            if (m_Activated)
            {
                transform.Rotate(m_DegreesPerSecond * Time.deltaTime, Space.World);
            }
        }

        public void Activate()
        {
            m_Activated = true;
        }

        public void Deactivate()
        {
            m_Activated = false;
        }
    }

}
#endif