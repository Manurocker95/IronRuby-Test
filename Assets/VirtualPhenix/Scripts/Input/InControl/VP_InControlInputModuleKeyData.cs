using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if USE_INCONTROL
using InControl;
#endif


namespace VirtualPhenix.Inputs
{
	[System.Serializable]
	public class VP_InControlInputModuleKeyData : VP_InputKeyData
	{
	#if USE_INCONTROL	
		[Header("Submit"), Space]
		public Key[] SubmitKeys = new Key[] { Key.E, Key.Space };
		public InputControlType[] SubmitInControl = new InputControlType[] { InputControlType.Action2 };

		[Header("Cancel"), Space]
		public Key[] CancelKeys = new Key[] { Key.X, Key.Escape };
		public InputControlType[] CancelInControl = new InputControlType[] { InputControlType.Action1 };

		[Header("Move"), Space]
		public Key[] RightKeys = new Key[] { Key.D, Key.RightArrow };
		public InputControlType[] RightInControl = new InputControlType[] { InputControlType.LeftStickRight, InputControlType.DPadRight };
		public Key[] LeftKeys = new Key[] { Key.A, Key.LeftArrow };
		public InputControlType[] LeftInControl = new InputControlType[] { InputControlType.LeftStickLeft, InputControlType.DPadLeft };
		public Key[] UpKeys = new Key[] { Key.W, Key.UpArrow };
		public InputControlType[] UpInControl = new InputControlType[] { InputControlType.LeftStickUp,InputControlType.DPadUp };
		public Key[] DownKeys = new Key[] { Key.S,  Key.DownArrow };
		public InputControlType[] DownInControl = new InputControlType[] { InputControlType.LeftStickDown,InputControlType.DPadDown };
#endif

		public VP_InControlInputModuleKeyData()
		{

		}

		public VP_InControlInputModuleKeyData(VP_InputKeyData _copyKeys)
		{
			if (_copyKeys == null)
			{
				_copyKeys = new VP_InputKeyData();
			}

			SetKeys(_copyKeys.AttackKey, _copyKeys.InteractKey, _copyKeys.JumpKey,_copyKeys.ButtKey, _copyKeys.ConfirmKey, 
				_copyKeys.CancelKey, _copyKeys.MenuKey, _copyKeys.SprintKey,
				_copyKeys.HorizontalMoveAxis, _copyKeys.VerticalMoveAxis,
				_copyKeys.HorizontalDpadAxis, _copyKeys.VertialDpadAxis,
				_copyKeys.HorizontalRotateAxis, _copyKeys.VerticalRotateAxis);
		}
	}
}