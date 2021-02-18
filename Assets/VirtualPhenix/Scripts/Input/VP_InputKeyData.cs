using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Inputs
{
    [System.Serializable]
    public partial class VP_InputKeyData 
    {
        [Header("KeyCode Actions"), Space]
        public KeyCode[] AttackKey = new KeyCode[] { KeyCode.F };
        public KeyCode[] InteractKey = new KeyCode[] { KeyCode.E };
        public KeyCode[] JumpKey = new KeyCode[] { KeyCode.Space };
	    public KeyCode[] ButtKey = new KeyCode[] { KeyCode.Space };
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

	    public virtual void SetKeys(KeyCode[] _attacktKey, KeyCode[] _interactKey, KeyCode[] _jumpKey, KeyCode[] _buttKey, KeyCode[] _confirmKey, KeyCode[] _cancelKey, KeyCode[] _menuKey, KeyCode[] _sprintKey,
            string _horizontalMove, string _VerticalMoveAxis, string _HorizontalDpadAxis, string _VertialDpadAxis, string _HorizontalRotateAxis, string _VerticalRotateAxis)
        {
            AttackKey = new KeyCode[_attacktKey.Length];

            for (int i = 0; i < _attacktKey.Length; i++)
	            AttackKey[i] = _attacktKey[i];
                
            ButtKey = new KeyCode[_buttKey.Length];

            for (int i = 0; i < _buttKey.Length; i++)
                ButtKey[i] = _buttKey[i];

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
    }
}
