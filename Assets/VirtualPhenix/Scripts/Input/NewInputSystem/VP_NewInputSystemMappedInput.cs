#if USE_NEW_INPUT_SYSTEM
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace VirtualPhenix.Inputs
{
    public class VP_NewInputSystemMappedInput : VP_MappedInput
    {
        public InputActionAsset m_inputActionAsset;
        public InputAction m_action;


        public InputAction.CallbackContext m_generatedContext;

        public VP_NewInputSystemMappedInput()
        {

        }

        public VP_NewInputSystemMappedInput(InputActionAsset _actionSet)
        {
            m_inputActionAsset = _actionSet;
        }
    }
}
#endif