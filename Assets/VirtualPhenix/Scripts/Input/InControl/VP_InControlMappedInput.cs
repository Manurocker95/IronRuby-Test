#if USE_INCONTROL
using InControl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Inputs
{

    [System.Serializable]
    public class VP_InControlMappedInput : VP_MappedInput
    {
        public InControl.IInputControl m_inputAction;

        public VP_InControlMappedInput()
        {

        }

        public VP_InControlMappedInput (IInputControl _inControlAction)
        {
            m_inputAction = _inControlAction;
        }

        public override bool HasChanged() => m_inputAction != null ? m_inputAction.HasChanged : false;
        public override bool IsPressed() => m_inputAction != null ? m_inputAction.IsPressed : false;
        public override bool WasPressed() => m_inputAction != null ? m_inputAction.WasPressed : false;
        public override bool WasReleased() => m_inputAction != null ? m_inputAction.WasReleased : false;
    }
}
#endif