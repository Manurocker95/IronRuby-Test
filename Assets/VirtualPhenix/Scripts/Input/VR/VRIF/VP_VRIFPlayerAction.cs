using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if USE_VRIF

using VirtualPhenix.Inputs.VR;
using VirtualPhenix.Inputs;
using BNG;

namespace VirtualPhenix.Integrations.Inputs.VR
{
    [System.Serializable]
    public class VP_VRIFPlayerActionBinding
    {
#if USE_VRIF
        public bool m_checkHand = false;
        public ControllerHand[] m_hands = new ControllerHand[] { ControllerHand.Left };

        public GrabbedControllerBinding[] m_holdBinding = new GrabbedControllerBinding[] { GrabbedControllerBinding.Grip };
        public GrabbedControllerBinding[] m_downBidning = new GrabbedControllerBinding[] { GrabbedControllerBinding.GripDown };
        public GrabbedControllerBinding[] m_upBidning = new GrabbedControllerBinding[] { GrabbedControllerBinding.GripDown };
        public InputAxis[] m_axis = new InputAxis[] { InputAxis.LeftThumbStickAxis };

        public Vector2 GetAxisValue(InputBridge _inputBridge)
        {
            Vector2 sum = Vector2.zero;

            if (_inputBridge == null)
                return sum;

            foreach (InputAxis axis in m_axis)
            {
                sum += (_inputBridge.GetInputAxisValue(axis));
            }

            return sum;
        }

        public bool IsPressed(InputBridge _inputBridge)
        {
            if (_inputBridge == null)
                return false;
            
            foreach (ControllerHand hand in m_hands)
            {
                foreach (GrabbedControllerBinding bind in m_holdBinding)
                {
                    if (_inputBridge.GetGrabbedControllerBinding(bind, hand))
                        return true;
                }
            }

            return false;
        }

        public bool WasPressed(InputBridge _inputBridge)
        {
            if (_inputBridge == null)
                return false;

            foreach (ControllerHand hand in m_hands)
            {
                foreach (GrabbedControllerBinding bind in m_downBidning)
                {
                    if (_inputBridge.GetGrabbedControllerBinding(bind, hand))
                        return true;
                }
            }

            return false;
        }

        public bool WasReleased(InputBridge _inputBridge)
        {
            if (_inputBridge == null)
                return false;

            foreach (ControllerHand hand in m_hands)
            {
                foreach (GrabbedControllerBinding bind in m_upBidning)
                {
                    if (_inputBridge.GetGrabbedControllerBinding(bind, hand))
                        return true;
                }
            }

            return false;
        }
#endif
    }

    [System.Serializable]
    public class VP_VRIFPlayerAction : VP_VRPlayerActions
    {
        public VP_VRIFPlayerActionBinding Sprint = new VP_VRIFPlayerActionBinding();
        public VP_VRIFPlayerActionBinding Jump = new VP_VRIFPlayerActionBinding();
        public VP_VRIFPlayerActionBinding Confirm = new VP_VRIFPlayerActionBinding();
        public VP_VRIFPlayerActionBinding Interact = new VP_VRIFPlayerActionBinding();
        public VP_VRIFPlayerActionBinding Attack = new VP_VRIFPlayerActionBinding();
        public VP_VRIFPlayerActionBinding Move = new VP_VRIFPlayerActionBinding();
        public VP_VRIFPlayerActionBinding Grab = new VP_VRIFPlayerActionBinding();
        public VP_VRIFPlayerActionBinding Shoot = new VP_VRIFPlayerActionBinding();

        public List<VP_VRIFPlayerActionBinding> BindingList { get; set; }

        public override void SetupInputs(VP_InputKeyData _copyKeys, bool _blockIfNull = false)
        {
            base.SetupInputs(_copyKeys, _blockIfNull);
            VP_VRIFInputKeyData kd = (VP_VRIFInputKeyData)_copyKeys;

            var manager = VP_VRIFInputManager.Instance;
            var loc = manager.LocomotionManager;

            switch (kd.m_movementType)
            {
                case VP_VRInputKeyData.MOVEMENT_TYPE.TELEPORT:
                    loc.DefaultLocomotion = LocomotionType.Teleport;
                    break;
                case VP_VRInputKeyData.MOVEMENT_TYPE.CONTINUOUS:
                    loc.DefaultLocomotion = LocomotionType.SmoothLocomotion;
                    break;
                default:
                    loc.DefaultLocomotion = LocomotionType.None;
                    break;
            }

            if (kd.m_overrideToggleValues)
            {
                if (!kd.m_canChangeBetweenMovementTypes)
                {
                    loc.locomotionToggleInput = new List<ControllerBinding>() { ControllerBinding.None };
                }
                else
                {
                    loc.locomotionToggleInput = new List<ControllerBinding>();
                    foreach (ControllerBinding c in kd.m_toggleBetweenMovementTypes)
                    {
                        loc.locomotionToggleInput.Add(c);
                    }
                }
            }

            Sprint = kd.Sprint;
            Jump = kd.Jump;
            Confirm = kd.Confirm;
            Interact = kd.Interact;
            Move = kd.Move;
            Grab = kd.Grab;
            Shoot = kd.Shoot;

            BindingList = new List<VP_VRIFPlayerActionBinding>()
            {
                Sprint,
                Jump,
                Confirm,
                Interact,
                Attack,
                Move,
                Grab,
                Shoot
            };
        }

        public virtual void VibrateController(float frequency, float amplitude, float duration, ControllerHand hand = ControllerHand.Right)
        {
            InputBridge.Instance.VibrateController(frequency, amplitude, duration, hand);
        }

        public override void TeleportToPoint(Vector3 point, Quaternion rotation)
        {
            VP_VRIFInputManager.Instance.PlayerTeleport.TeleportPlayer(point, rotation);
        }

        public override float HorizontalMoveValue()
        {
#if USE_VRIF
            return m_blocked ? 0f : (m_useUnityKeys ? (m_combineBoth ? (Move.GetAxisValue(InputBridge.Instance).x + base.HorizontalMoveValue()) : base.HorizontalMoveValue()) : Move.GetAxisValue(InputBridge.Instance).x);
#else
            return !m_blocked ? base.HorizontalMoveValue() : 0f;
#endif
        }

        public override float VerticalMoveValue()
        {
#if USE_VRIF
            return m_blocked ? 0f : (m_useUnityKeys ? (m_combineBoth ? (Move.GetAxisValue(InputBridge.Instance).y + base.VerticalMoveValue()) : base.VerticalMoveValue()) : Move.GetAxisValue(InputBridge.Instance).y);
#else
            return !m_blocked ? base.VerticalMoveValue() : 0f;
#endif
        }


        public virtual bool BindingIsPressed(GrabbedControllerBinding _binding, ControllerHand _hand)
        {
#if USE_VRIF
            return !m_blocked && ((m_useUnityKeys && base.JumpIsPressed()) || (InputBridge.Instance.GetGrabbedControllerBinding(_binding, _hand) || base.JumpIsPressed()));
#else
            return !m_blocked && base.JumpIsPressed();
#endif
        }

        public virtual bool BindingIsPressed(VP_VRIFPlayerActionBinding _binding)
        {
#if USE_VRIF
            return !m_blocked && ((m_useUnityKeys && base.JumpIsPressed()) || (_binding.IsPressed(InputBridge.Instance) || base.JumpIsPressed()));
#else
            return !m_blocked && base.JumpIsPressed();
#endif
        }

        public virtual bool BindingWasPressed(VP_VRIFPlayerActionBinding _binding)
        {

#if USE_VRIF
            return !m_blocked && ((m_useUnityKeys && base.JumpWasPressed()) || (_binding.WasPressed(InputBridge.Instance) || base.JumpWasPressed()));
#else
            return !m_blocked && base.JumpWasPressed();
#endif
        }

        public virtual bool BindingWasReleased(VP_VRIFPlayerActionBinding _binding)
        {
#if USE_VRIF
            return !m_blocked && ((m_useUnityKeys && base.JumpWasReleased()) || (_binding.WasReleased(InputBridge.Instance) || base.JumpWasReleased()));
#else
            return !m_blocked && base.JumpWasReleased();
#endif
        }

        public override bool GrabIsPressed()
        {
#if USE_VRIF
            return !m_blocked && ((m_useUnityKeys && base.GrabIsPressed()) || (Grab.IsPressed(InputBridge.Instance) || base.GrabIsPressed()));
#else
            return !m_blocked && base.GrabIsPressed();
#endif
        }

        public override bool GrabWasPressed()
        {

#if USE_VRIF
            return !m_blocked && ((m_useUnityKeys && base.GrabWasPressed()) || (Grab.WasPressed(InputBridge.Instance) || base.GrabWasPressed()));
#else
            return !m_blocked && base.GrabWasPressed();
#endif
        }

        public override bool GrabWasReleased()
        {
#if USE_VRIF
            return !m_blocked && ((m_useUnityKeys && base.GrabWasReleased()) || (Grab.WasReleased(InputBridge.Instance) || base.GrabWasReleased()));
#else
            return !m_blocked && base.GrabWasReleased();
#endif
        }

        public override bool ShootIsPressed()
        {
#if USE_VRIF
            return !m_blocked && ((m_useUnityKeys && base.ShootIsPressed()) || (Shoot.IsPressed(InputBridge.Instance) || base.ShootIsPressed()));
#else
            return !m_blocked && base.ShootIsPressed();
#endif
        }

        public override bool ShootWasPressed()
        {

#if USE_VRIF
            return !m_blocked && ((m_useUnityKeys && base.ShootWasPressed()) || (Shoot.WasPressed(InputBridge.Instance) || base.ShootWasPressed()));
#else
            return !m_blocked && base.ShootWasPressed();
#endif
        }

        public override bool ShootWasReleased()
        {
#if USE_VRIF
            return !m_blocked && ((m_useUnityKeys && base.ShootWasReleased()) || (Shoot.WasReleased(InputBridge.Instance) || base.ShootWasReleased()));
#else
            return !m_blocked && base.ShootWasReleased();
#endif
        }

        public override bool SprintIsPressed()
        {
#if USE_VRIF
            return !m_blocked && ((m_useUnityKeys && base.SprintIsPressed()) || (Sprint.IsPressed(InputBridge.Instance) || base.SprintIsPressed()));
#else
            return !m_blocked && base.SprintIsPressed();
#endif
        }

        public override bool JumpIsPressed()
        {
#if USE_VRIF
            return !m_blocked && ((m_useUnityKeys && base.JumpIsPressed()) || (Jump.IsPressed(InputBridge.Instance) || base.JumpIsPressed()));
#else
            return !m_blocked && base.JumpIsPressed();
#endif
        }

        public override bool JumpWasPressed()
        {

#if USE_VRIF
            return !m_blocked && ((m_useUnityKeys && base.JumpWasPressed()) || (Jump.WasPressed(InputBridge.Instance) || base.JumpWasPressed()));
#else
            return !m_blocked && base.JumpWasPressed();
#endif
        }

        public override bool JumpWasReleased()
        {
#if USE_VRIF
            return !m_blocked && ((m_useUnityKeys && base.JumpWasReleased()) || (Jump.WasReleased(InputBridge.Instance) || base.JumpWasReleased()));
#else
            return !m_blocked && base.JumpWasReleased();
#endif
        }

        public override bool SelectInputIsPressed()
        {
#if USE_VRIF
            return !m_blocked && ((m_useUnityKeys && base.SelectInputIsPressed()) || (Confirm.IsPressed(InputBridge.Instance) || base.SelectInputIsPressed()));
#else
            return !m_blocked && base.SelectInputIsPressed();
#endif
        }

        public override bool SelectInputWasPressed()
        {
#if USE_VRIF
            return !m_blocked && ((m_useUnityKeys && base.SelectInputWasPressed()) || (Confirm.WasPressed(InputBridge.Instance) || base.SelectInputWasPressed()));
#else
            return !m_blocked && base.SelectInputWasPressed();
#endif
        }

        public override bool SelectInputWasReleased()
        {
#if USE_VRIF
            return !m_blocked && ((m_useUnityKeys && base.SelectInputWasReleased()) || (Confirm.WasReleased(InputBridge.Instance) || base.SelectInputWasReleased()));
#else
            return !m_blocked && base.SelectInputWasReleased();
#endif
        }

        public override bool InteractInputIsPressed()
        {
#if USE_VRIF
            return !m_blocked && ((m_useUnityKeys && base.InteractInputIsPressed()) || (Interact.IsPressed(InputBridge.Instance) || base.InteractInputIsPressed()));
#else
            return !m_blocked && base.InteractInputIsPressed();
#endif
        }

        public override bool InteractInputWasPressed()
        {
#if USE_VRIF
            return !m_blocked && ((m_useUnityKeys && base.InteractInputWasPressed()) || (Interact.WasPressed(InputBridge.Instance) || base.InteractInputWasPressed()));
#else
            return !m_blocked && base.InteractInputWasPressed();
#endif
        }

        public override bool InteractInputWasReleased()
        {
#if USE_VRIF
            return !m_blocked && ((m_useUnityKeys && base.InteractInputWasReleased()) || (Interact.WasReleased(InputBridge.Instance) || base.InteractInputWasReleased()));
#else
            return !m_blocked && base.InteractInputWasReleased();
#endif
        }


        public override bool AttackInputWasPressed()
        {
#if USE_VRIF
            return !m_blocked && ((m_useUnityKeys && base.AttackInputWasPressed()) || (Attack.WasPressed(InputBridge.Instance) || base.AttackInputWasPressed()));
#else
            return !m_blocked && base.AttackInputWasPressed();
#endif
        }

        public override bool AttackInputWasReleased()
        {
#if USE_VRIF
            return !m_blocked && ((m_useUnityKeys && base.AttackInputWasReleased()) || (Attack.WasReleased(InputBridge.Instance) || base.AttackInputWasReleased()));
#else
            return !m_blocked && base.AttackInputWasReleased();
#endif
        }


        public override bool AttackInputIsPressed()
        {
#if USE_VRIF
            return !m_blocked && ((m_useUnityKeys && base.AttackInputIsPressed()) || (Attack.IsPressed(InputBridge.Instance) || base.AttackInputIsPressed()));
#else
            return !m_blocked && base.AttackInputIsPressed();
#endif
        }
    }
}
#endif