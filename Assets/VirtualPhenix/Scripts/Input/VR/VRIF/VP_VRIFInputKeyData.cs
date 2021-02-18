using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using VirtualPhenix.Inputs.VR;
using VirtualPhenix.Inputs;


#if USE_VRIF
using BNG;
#endif

namespace VirtualPhenix.Integrations.Inputs.VR
{
    public class VP_VRIFInputKeyData : VP_VRInputKeyData
    {
#if USE_VRIF
#if ODIN_INSPECTOR
        [Sirenix.Serialization.OdinSerialize] public VP_VRIFPlayerActionBinding Sprint = new VP_VRIFPlayerActionBinding();
        [Sirenix.Serialization.OdinSerialize] public VP_VRIFPlayerActionBinding Jump = new VP_VRIFPlayerActionBinding();
        [Sirenix.Serialization.OdinSerialize] public VP_VRIFPlayerActionBinding Confirm = new VP_VRIFPlayerActionBinding();
        [Sirenix.Serialization.OdinSerialize] public VP_VRIFPlayerActionBinding Interact = new VP_VRIFPlayerActionBinding();
        [Sirenix.Serialization.OdinSerialize] public VP_VRIFPlayerActionBinding Attack = new VP_VRIFPlayerActionBinding();
        [Sirenix.Serialization.OdinSerialize] public VP_VRIFPlayerActionBinding Move = new VP_VRIFPlayerActionBinding();
        [Sirenix.Serialization.OdinSerialize] public VP_VRIFPlayerActionBinding Grab = new VP_VRIFPlayerActionBinding();
        [Sirenix.Serialization.OdinSerialize] public VP_VRIFPlayerActionBinding Shoot = new VP_VRIFPlayerActionBinding();
#else
        public VP_VRIFPlayerActionBinding Sprint = new VP_VRIFPlayerActionBinding();
        public VP_VRIFPlayerActionBinding Jump = new VP_VRIFPlayerActionBinding();
        public VP_VRIFPlayerActionBinding Confirm = new VP_VRIFPlayerActionBinding();
        public VP_VRIFPlayerActionBinding Interact= new VP_VRIFPlayerActionBinding();
        public VP_VRIFPlayerActionBinding Attack = new VP_VRIFPlayerActionBinding();
        public VP_VRIFPlayerActionBinding Move = new VP_VRIFPlayerActionBinding();
        public VP_VRIFPlayerActionBinding Grab = new VP_VRIFPlayerActionBinding();
        public VP_VRIFPlayerActionBinding Shoot = new VP_VRIFPlayerActionBinding();
#endif

        public bool m_overrideToggleValues = true;
        public List<ControllerBinding> m_toggleBetweenMovementTypes = new List<ControllerBinding>() { ControllerBinding.None };
#endif
        public VP_VRIFInputKeyData()
        {

        }

        public VP_VRIFInputKeyData(VP_InputKeyData _copyKeys)
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