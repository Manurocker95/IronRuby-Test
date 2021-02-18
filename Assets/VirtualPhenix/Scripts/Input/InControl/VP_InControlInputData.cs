using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if USE_INCONTROL
using InControl;
#endif


namespace VirtualPhenix.Inputs
{
    [System.Serializable]
    public partial class VP_InControlInputData : VP_InputKeyData
	{
		[Header("Config when InControl"), Space]
		public bool m_useUnityKeys = false;
		public bool m_combineWithUnityKeys = false;
		public bool m_useUnityKeysIfNoGamePad = false;
		
#if USE_INCONTROL
        [Header("InControl Keys"), Space]
        [Header("Attack"), Space]
        public Key[] AttackKeys = new Key[] { Key.F };
        public InputControlType[] AttackInControl = new InputControlType[] { InputControlType.Action4 };       
        
		public Key[] SpecialAttackKeys = new Key[] { Key.G };
		public InputControlType[] SpecialAttackInControl = new InputControlType[] { InputControlType.RightBumper };   
        
		public Key[] ButtAttackKeys = new Key[] { Key.Space };
		public InputControlType[] ButtAttackInControl = new InputControlType[] { InputControlType.Action1 };   
        
        
        [Header("Confirm"), Space]
        public Key[] ConfirmKeys = new Key[] { Key.E };
        public InputControlType[] ConfirmInControl = new InputControlType[] { InputControlType.Action2 };

        [Header("Jump"), Space]
        public Key[] JumpKeys = new Key[] { Key.Space };
        public InputControlType[] JumpInControl = new InputControlType[] { InputControlType.Action1 };

        [Header("Interact"), Space]
        public Key[] InteractKeys = new Key[] { Key.Space };
        public InputControlType[] InteractInControl = new InputControlType[] { InputControlType.Action2 };

        [Header("Cancel"), Space]
        public Key[] CancelKeys = new Key[] { Key.X };
        public InputControlType[] CancelInControl = new InputControlType[] { InputControlType.Action1 };

        [Header("Menu"), Space]
        public Key[] MenuKeys = new Key[] { Key.Return };
        public InputControlType[] MenuInControl = new InputControlType[] { InputControlType.Command };

        [Header("Move"), Space]
        public Key[] RightKeys = new Key[] { Key.D };
        public InputControlType[] RightInControl = new InputControlType[] { InputControlType.LeftStickRight };
        public Key[] LeftKeys = new Key[] { Key.A };
        public InputControlType[] LeftInControl = new InputControlType[] { InputControlType.LeftStickLeft };
        public Key[] UpKeys = new Key[] { Key.W };
        public InputControlType[] UpInControl = new InputControlType[] { InputControlType.LeftStickUp };
        public Key[] DownKeys = new Key[] { Key.S };
        public InputControlType[] DownInControl = new InputControlType[] { InputControlType.LeftStickDown };

        [Header("Sprint"), Space]
        public Key[] SprintKeys = new Key[] { Key.LeftShift };
        public InputControlType[] SprintInControl = new InputControlType[] { InputControlType.RightBumper };

        [Header("DPad"), Space]
        public Key[] DpadRightKeys = new Key[] { Key.RightArrow };
        public InputControlType[] DpadRightInControl = new InputControlType[] { InputControlType.DPadRight };
        public Key[] DpadLeftKeys = new Key[] { Key.LeftArrow };
        public InputControlType[] DpadLeftInControl = new InputControlType[] { InputControlType.DPadLeft };
        public Key[] DpadUpKeys = new Key[] { Key.UpArrow };
        public InputControlType[] DpadUpInControl = new InputControlType[] { InputControlType.DPadUp };
        public Key[] DpadDownKeys = new Key[] { Key.DownArrow };
        public InputControlType[] DpadDownInControl = new InputControlType[] { InputControlType.DPadDown };

        [Header("Rotate"), Space]
        public Key[] RotateRightKeys = new Key[] { };
        public InputControlType[] RotateRightInControl = new InputControlType[] { InputControlType.RightStickRight };
        public Key[] RotateLeftKeys = new Key[] { };
        public InputControlType[] RotateLeftInControl = new InputControlType[] { InputControlType.RightStickLeft };
        public Key[] RotateUpKeys = new Key[] { };
        public InputControlType[] RotateUpInControl = new InputControlType[] { InputControlType.RightStickUp };
        public Key[] RotateDownKeys = new Key[] { };
        public InputControlType[] RotateDownInControl = new InputControlType[] { InputControlType.RightStickDown };
#endif

        public VP_InControlInputData()
        {

        }

        public VP_InControlInputData(VP_InputKeyData _copyKeys)
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
