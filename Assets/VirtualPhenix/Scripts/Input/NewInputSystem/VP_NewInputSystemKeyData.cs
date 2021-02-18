#if USE_NEW_INPUT_SYSTEM
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace VirtualPhenix.Inputs
{
    public class VP_NewInputSystemKeyData : VP_InputKeyData
    {
        public InputActionAsset m_inputActionAsset;
    }
}
#endif