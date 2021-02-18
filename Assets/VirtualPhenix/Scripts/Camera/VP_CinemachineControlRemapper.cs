using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix
{
    [AddComponentMenu(VP_PrefixSetup.MAIN_PREFIX + "/Inputs/Cinemachine Control Remapper")]
    public class VP_CinemachineControlRemapper: VP_Monobehaviour
    {
        [Header("Configuration"), Space]
        [SerializeField] protected bool m_remap = true;

        [Header("Original Axis"), Space]
        [SerializeField] protected string m_originalXAxis = "Mouse X";
        [SerializeField] protected string m_originalYAxis = "Mouse Y";

        protected virtual void Reset()
        {
            m_startListeningTime = StartListenTime.None;
            m_stopListeningTime = StopListenTime.None;
            m_initializationTime = InitializationTime.OnAwake;
        }

        protected override void Initialize()
        {
            base.Initialize();
#if !USE_CINEMACHINE
            Debug.LogError("Cinemachine remapper can't remap because #USE_CINEMACHINE is not defined.");
#else
            if (m_remap)
                Cinemachine.CinemachineCore.GetInputAxis += GetAxisCustom;
#endif
        }

        public virtual float GetAxisCustom(string axisData)
        {
            return 0f;
        }
    }
}
