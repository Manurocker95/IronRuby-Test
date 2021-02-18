using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Inputs
{
    public static partial class VP_InputSetup
    {
        public const string PLAYER_ID = "Player";
	    public const string REGULR_IN_CONTROL_MODULE_ID = "Input Module";

        public partial class InControlPlayerActionIds
        {
#if USE_INCONTROL
	        public const string SPECIAL_ATTACK_ID = "Special Attack";
            public const string ATTACK_ID = "Attack";
            public const string CONFIRM_ID = "Confirm";
            public const string JUMP_ID = "Jump";
            public const string INTERACT_ID = "Interact";
            public const string CANCEL_ID = "Cancel";
	        public const string SUBMIT_ID = "Submit";
            public const string MENU_ID = "Menu";
            public const string MOVE_LEFT_ID = "Move Left";
            public const string MOVE_RIGHT_ID = "Move Right";
            public const string MOVE_UP_ID = "Move Up";
            public const string MOVE_DOWN_ID = "Move Down";
            public const string DPAD_LEFT_ID = "DPad Left";
            public const string DPAD_RIGHT_ID = "DPad Right";
            public const string DPAD_UP_ID = "DPad Up";
            public const string DPAD_DOWN_ID = "DPad Down";
            public const string ROTATE_LEFT_ID = "Rotate Left";
            public const string ROTATE_RIGHT_ID = "Rotate Right";
            public const string ROTATE_UP_ID = "Rotate Up";
            public const string ROTATE_DOWN_ID = "Rotate Down";
            public const string SPRINT_ID = "Sprint";
	        public const string BUTT_ATTACK_ID = "Butt Smash";
#endif
        }
    }
}