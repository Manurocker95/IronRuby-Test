using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if USE_INCONTROL
using InControl;
#endif

namespace VirtualPhenix.Inputs
{
#if USE_INCONTROL
    public partial class VP_InputActions : PlayerActionSet
#else
    public partial class VP_InputActions
#endif
    {
        public bool FlyingLeftRollWasPressed()
        {
            return !m_blocked;
        }

        public bool FlyingRightRollWasPressed()
        {
            return !m_blocked;
        }

        public bool FlyIsPressed()
        {
            return !m_blocked;
        }
    }
}