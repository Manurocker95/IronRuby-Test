#if USE_NEW_INPUT_SYSTEM
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace VirtualPhenix.Inputs
{
    public class VP_NewPlayerInputSystemMapper : VP_MonoBehaviour
    {
        [Header("Reference to Input Map")]
        [SerializeField] protected PlayerInput m_input;
        [SerializeField] protected string m_actionMap = "Player";

        protected virtual void Reset()
        {
            transform.TryGetComponentInChildren<PlayerInput>(out m_input);
        }

        protected override void Initialize()
        {
            base.Initialize();

            VP_NewInputSystemManager.Instance.SetPlayerInputToActionSet(m_input, m_actionMap);
        }
    }
}
#endif