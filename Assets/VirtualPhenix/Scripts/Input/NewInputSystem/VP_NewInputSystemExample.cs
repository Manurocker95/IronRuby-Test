#if USE_NEW_INPUT_SYSTEM
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix.Inputs;
using UnityEngine.InputSystem;

namespace VirtualPhenix.Testing
{
    public class VP_NewInputSystemExample : VP_MonoBehaviour
    {
        VP_NewInputSystemMappedInput m_logMap;
        [Header("Config"), Space]
        [SerializeField] protected bool m_usePlayerInputAsListeningParameter = false;
        [SerializeField] protected bool m_canUseAnyInput = false;
        [Header("Player Input"), Space]
        [SerializeField] protected PlayerInput m_playerInput;
        [SerializeField] protected string m_action = "Attack"; 
        [Header("Remap it"), Space]
        [SerializeField] protected bool m_mapPlayerInput;
        [SerializeField] protected string m_vpActionSet = "Player";

        private void Reset()
        {
            m_initializationTime = InitializationTime.OnStart;
            m_startListeningTime = StartListenTime.OnEnable;
            m_stopListeningTime = StopListenTime.OnDisable;
        }

        protected override void Initialize()
        {
            base.Initialize();


            if (m_mapPlayerInput && m_vpActionSet.IsNotNullNorEmpty()  && m_playerInput != null)
            {
                
                VP_NewInputSystemManager.Instance.SetPlayerInputToActionSet(m_playerInput, m_vpActionSet);
            }


            StartAllListeners();
        }

        protected override void StartAllListeners()
        {
            if (m_initialized && m_playerInput != null)
            {
                if (!m_canUseAnyInput)
                {
                    m_logMap = VP_NewInputSystemManager.Instance.AddActionListener(m_playerInput, m_playerInput.actions.FindAction(m_action), ShowLog);
                }
                else
                {
                    m_logMap = VP_NewInputSystemManager.Instance.AddActionListener(m_playerInput, ShowLog);
                }
            }
        }


        protected override void StopAllListeners()
        {
            if (m_initialized && m_logMap != null)
            {
                if (!m_usePlayerInputAsListeningParameter)
                {
                    VP_NewInputSystemManager.Instance.RemoveActionListener(m_logMap, ShowLog);
                }
                else if (m_playerInput != null)
                {
                    if (!m_canUseAnyInput)
                    {
                        VP_NewInputSystemManager.Instance.RemoveActionListener(m_playerInput, m_playerInput.actions.FindAction(m_action), ShowLog);
                    }
                    else
                    {
                        VP_NewInputSystemManager.Instance.RemoveActionListener(m_playerInput, ShowLog);
                    }
                }
            }
        }

        // Update is called once per frame
        void ShowLog(VP_InputActions _action, VP_MappedInput _triggered)
        {
            Debug.Log(((VP_NewInputSystemMappedInput)_triggered).m_generatedContext.action);
        }
    }

}
#endif