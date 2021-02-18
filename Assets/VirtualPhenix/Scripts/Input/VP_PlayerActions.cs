using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Inputs
{
    [System.Serializable]
    public partial class VP_PlayerActions : VP_InputActions
    {
        [Header("Generic Usage for No Gamepad"), Space]

        public bool m_useUnityKeys = false;
        public bool m_combineBoth = false;

        public bool m_useUnityKeysIfNoGamePad = false;

        [Header("KeyCode Actions"), Space]
        public KeyCode[] AttackKey = new KeyCode[] { KeyCode.F };
        public KeyCode[] InteractKey = new KeyCode[] { KeyCode.E };
        public KeyCode[] JumpKey = new KeyCode[] { KeyCode.Space }; 
	    public KeyCode[] ButtSmashKey = new KeyCode[] { KeyCode.Space }; 
        public KeyCode[] ConfirmKey = new KeyCode[] { KeyCode.E };
        public KeyCode[] CancelKey = new KeyCode[] { KeyCode.X };
        public KeyCode[] MenuKey = new KeyCode[] { KeyCode.Return };
        public KeyCode[] SprintKey = new KeyCode[] { KeyCode.LeftShift };

        public string HorizontalMoveAxis = "Horizontal";
        public string VerticalMoveAxis = "Vertical";

        public string HorizontalDpadAxis = "HorizontalDpad";
        public string VertialDpadAxis = "VerticalDpad";

        public string HorizontalRotateAxis = "Mouse_X";
        public string VerticalRotateAxis = "Mouse_Y";

        public override void SetupInputs(VP_InputKeyData _copyKeys, bool _blockIfNull = false)
        {
            base.SetupInputs(_copyKeys, _blockIfNull);

	        SetKeys(_copyKeys.InteractKey, _copyKeys.JumpKey, _copyKeys.ConfirmKey,
		        _copyKeys.ButtKey, _copyKeys.AttackKey,
               _copyKeys.CancelKey, _copyKeys.MenuKey, _copyKeys.SprintKey,
               _copyKeys.HorizontalMoveAxis, _copyKeys.VerticalMoveAxis,
               _copyKeys.HorizontalDpadAxis, _copyKeys.VertialDpadAxis,
               _copyKeys.HorizontalRotateAxis, _copyKeys.VerticalRotateAxis);
        }

	    public virtual void SetKeys(KeyCode[] _interactKey, KeyCode[] _jumpKey, KeyCode[] _confirmKey, KeyCode[] _buttSmash, KeyCode[] _attackKey,  KeyCode[] _cancelKey, KeyCode[] _menuKey, KeyCode[] _sprintKey,
            string _horizontalMove, string _VerticalMoveAxis, string _HorizontalDpadAxis, string _VertialDpadAxis, string _HorizontalRotateAxis, string _VerticalRotateAxis)
	    {
		    AttackKey = new KeyCode[_attackKey.Length];

		    for (int i = 0; i < _attackKey.Length; i++)
			    AttackKey[i] = _attackKey[i];
			    
		    ButtSmashKey = new KeyCode[_buttSmash.Length];

		    for (int i = 0; i < _buttSmash.Length; i++)
			    ButtSmashKey[i] = _buttSmash[i];
        	
            InteractKey = new KeyCode[_interactKey.Length];

            for (int i = 0; i < _interactKey.Length; i++)
                InteractKey[i] = _interactKey[i];

            JumpKey = new KeyCode[_jumpKey.Length];

            for (int i = 0; i < _jumpKey.Length; i++)
                JumpKey[i] = _jumpKey[i];

            ConfirmKey = new KeyCode[_confirmKey.Length];

            for (int i = 0; i < _confirmKey.Length; i++)
                ConfirmKey[i] = _confirmKey[i];

            CancelKey = new KeyCode[_cancelKey.Length];

            for (int i = 0; i < _cancelKey.Length; i++)
                CancelKey[i] = _cancelKey[i];

            MenuKey = new KeyCode[_menuKey.Length];

            for (int i = 0; i < _menuKey.Length; i++)
                MenuKey[i] = _menuKey[i];

            SprintKey = new KeyCode[_sprintKey.Length];

            for (int i = 0; i < _sprintKey.Length; i++)
                SprintKey[i] = _sprintKey[i];

            HorizontalMoveAxis = _horizontalMove;
            VerticalMoveAxis = _VerticalMoveAxis;
            HorizontalDpadAxis = _HorizontalDpadAxis;
            VertialDpadAxis = _VertialDpadAxis;
            HorizontalRotateAxis = _HorizontalRotateAxis;
            VerticalRotateAxis = _VerticalRotateAxis;
        }

        public virtual bool CheckKeyPressed(ref KeyCode[] _keys)
        {
            for (int i = 0; i < _keys.Length; i++)
            {
                if (Input.GetKey(_keys[i]))
                {
                    return true;
                }
            }

            return false;
        }

        public virtual bool CheckKeyPressedDown(ref KeyCode[] _keys)
        {
            for (int i = 0; i < _keys.Length; i++)
            {
                if (Input.GetKeyDown(_keys[i]))
                {
                    return true;
                }
            }

            return false;
        }

        public virtual bool CheckKeyPressedUp(ref KeyCode[] _keys)
        {
            for (int i = 0; i < _keys.Length; i++)
            {
                if (Input.GetKeyUp(_keys[i]))
                {
                    return true;
                }
            }

            return false;
        }

	    public override bool ButtSmashIsPressed()
	    {
		    if (m_blocked)
			    return false;

		    return CheckKeyPressed(ref ButtSmashKey);
	    }

	    public override bool ButtSmashWasPressed()
	    {
		    if (m_blocked)
			    return false;

		    return CheckKeyPressedDown(ref ButtSmashKey);
	    }

	    public override bool ButtSmashWasReleased()
	    {
		    if (m_blocked)
			    return false;

		    return CheckKeyPressedUp(ref ButtSmashKey);
	    }


        public override bool JumpIsPressed()
        {
            if (m_blocked)
                return false;

            return CheckKeyPressed(ref JumpKey);
        }

        public override bool JumpWasPressed()
        {
            if (m_blocked)
                return false;

            return CheckKeyPressedDown(ref JumpKey);
        }

        public override bool JumpWasReleased()
        {
            if (m_blocked)
                return false;

            return CheckKeyPressedUp(ref JumpKey);
        }

        public override float HorizontalMoveValue()
        {
            if (m_blocked)
            {
                return base.HorizontalMoveValue();
            }
            else
            {
                return Input.GetAxis(HorizontalMoveAxis);
            }
        }

        public override float VerticalMoveValue()
        {
            if (m_blocked)
            {
                return base.VerticalMoveValue();
            }
            else
            {
                return Input.GetAxis(VerticalMoveAxis);
            }
        }

        public override float HorizontalDPadValue()
        {
            if (m_blocked)
            {
                return base.HorizontalDPadValue();
            }
            else
            {
                return Input.GetAxis(HorizontalDpadAxis);
            }
        }

        public override float VerticalDPadValue()
        {
            if (m_blocked)
            {
                return base.VerticalDPadValue();
            }
            else
            {
                return Input.GetAxis(VertialDpadAxis);
            }
        }

        public override float HorizontalRotateValue()
        {
            if (m_blocked)
            {
                return base.HorizontalRotateValue();
            }
            else
            {
                return Input.GetAxis(HorizontalRotateAxis);
            }
        }

        public override float VerticalRotateValue()
        {
            if (m_blocked)
            {
                return base.VerticalRotateValue();
            }
            else
            {
                return Input.GetAxis(VerticalRotateAxis);
            }
        }

        public override bool SelectInputIsPressed()
        {
            if (m_blocked)
                return false;

            return CheckKeyPressed(ref ConfirmKey);
        }

        public override bool SelectInputWasPressed()
        {
            if (m_blocked)
                return false;

            return CheckKeyPressedDown(ref ConfirmKey);
        }

        public override bool SelectInputWasReleased()
        {
            if (m_blocked)
                return false;

            return CheckKeyPressedUp(ref ConfirmKey);
        }

        public override bool InteractInputIsPressed()
        {
            if (m_blocked)
                return false;

            return CheckKeyPressed(ref InteractKey);
        }

        public override bool InteractInputWasPressed()
        {
            if (m_blocked)
                return false;

            return CheckKeyPressedDown(ref InteractKey);
        }

        public override bool InteractInputWasReleased()
        {
            if (m_blocked)
                return false;

            return CheckKeyPressedUp(ref InteractKey);
        }
        
        public override bool AttackInputIsPressed()
        {
            if (m_blocked)
                return false;

            return CheckKeyPressed(ref AttackKey);
        }

        public override bool AttackInputWasPressed()
        {
            if (m_blocked)
                return false;

            return CheckKeyPressedDown(ref AttackKey);
        }

        public override bool AttackInputWasReleased()
        {
            if (m_blocked)
                return false;

            return CheckKeyPressedUp(ref AttackKey);
        }
    }
}