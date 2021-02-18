using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if USE_CINEMACHINE
using Cinemachine;
#endif

namespace VirtualPhenix
{
    [AddComponentMenu(VP_PrefixSetup.MAIN_PREFIX + "/Inputs/Cinemachine InputManager Control Remapper")]
    public class VP_CinemachineInputManagerControlRemapper : VP_CinemachineControlRemapper
    {
        [Header("Remapped Axis"), Space]
        [SerializeField] protected string m_remappedXAxis = "Mouse X";
        [SerializeField] protected string m_remappedYAxis = "Mouse Y";

        public override float GetAxisCustom(string axisData)
        {
            if (axisData.Equals(m_originalXAxis))
            {
                return UnityEngine.Input.GetAxis(m_remappedXAxis);
            }
            else if (axisData.Equals(m_originalYAxis))
            {
                return UnityEngine.Input.GetAxis(m_remappedYAxis); 
            }
            return UnityEngine.Input.GetAxis(axisData);
        }
    }

}