using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Inputs.VR
{
    public class VP_VRInputKeyData : VP_InputKeyData
    {
        public enum MOVEMENT_TYPE
        {
            CONTINUOUS,
            TELEPORT
        }

        [Header("Config when VR keys are set"), Space]
        public bool m_useUnityKeys = false;
        public bool m_combineWithUnityKeys = false;
        public bool m_canChangeBetweenMovementTypes = false;

        public MOVEMENT_TYPE m_movementType = MOVEMENT_TYPE.TELEPORT;



        public VP_VRInputKeyData()
        {

        }

        public VP_VRInputKeyData(VP_InputKeyData _copyKeys)
        {
            if (_copyKeys == null)
            {
                _copyKeys = new VP_InputKeyData();
            }

	        SetKeys(_copyKeys.AttackKey, _copyKeys.InteractKey, _copyKeys.JumpKey, _copyKeys.ButtKey, _copyKeys.ConfirmKey,
                _copyKeys.CancelKey, _copyKeys.MenuKey, _copyKeys.SprintKey,
                _copyKeys.HorizontalMoveAxis, _copyKeys.VerticalMoveAxis,
                _copyKeys.HorizontalDpadAxis, _copyKeys.VertialDpadAxis,
                _copyKeys.HorizontalRotateAxis, _copyKeys.VerticalRotateAxis);
        }
    }
}