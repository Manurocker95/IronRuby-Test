using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Inputs.VR
{
    [System.Serializable]
    public class VP_VRPlayerActions : VP_PlayerActions
    {
        public enum VRButtons
        {
            Grip,
            Trigger,
            Pad
        }

        public virtual void TeleportToPoint(Vector3 point, Quaternion rotation)
        {
         
        }

        public virtual bool GrabIsPressed()
        {
            // TODO override for 
            return !m_blocked;
        }

        public virtual bool GrabWasPressed()
        {
            return !m_blocked;
        }

        public virtual bool GrabWasReleased()
        {
            return !m_blocked;
        }

        public virtual bool ShootIsPressed()
        {
            // TODO override for 
            return !m_blocked;
        }

        public virtual bool ShootWasPressed()
        {
            return !m_blocked;
        }

        public virtual bool ShootWasReleased()
        {
            return !m_blocked;
        }
    }
}