using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if USE_INCONTROL
using InControl;
#endif

namespace VirtualPhenix.Inputs
{
    [System.Serializable]
    public class VP_InControlPlayerActions : VP_PlayerActions
    {
#if USE_INCONTROL
        [Header("In Control Player Actions"),Space]
        public readonly InControl.PlayerAction Attack;
	    public readonly InControl.PlayerAction ButtAttack;
	    public readonly InControl.PlayerAction Interact;
        public readonly InControl.PlayerAction Jump;
        public readonly InControl.PlayerAction Confirm;
        public readonly InControl.PlayerAction Cancel;
	    public readonly InControl.PlayerAction SpecialAttack;
        public readonly InControl.PlayerAction Menu;
        public readonly InControl.PlayerAction Sprint;

        public readonly InControl.PlayerAction Right;
        public readonly InControl.PlayerAction Up;
        public readonly InControl.PlayerAction Down;
        public readonly InControl.PlayerAction Left;
        public readonly InControl.PlayerTwoAxisAction Move;

        public readonly InControl.PlayerAction DpadRight;
        public readonly InControl.PlayerAction DpadUp;
        public readonly InControl.PlayerAction DpadDown;
        public readonly InControl.PlayerAction DpadLeft;
        public readonly InControl.PlayerTwoAxisAction DPad;


        public readonly InControl.PlayerAction RotateRight;
        public readonly InControl.PlayerAction RotateUp;
        public readonly InControl.PlayerAction RotateDown;
        public readonly InControl.PlayerAction RotateLeft;
        public readonly InControl.PlayerTwoAxisAction Rotate;

#if ODIN_INSPECTOR
        [Sirenix.Serialization.OdinSerialize]
#endif
        public List<IInputControl> PlayerActionList { get; set; }
#endif

        public VP_InControlPlayerActions()
        {
#if USE_INCONTROL
            PlayerActionList = new List<IInputControl>();

	        ButtAttack = CreatePlayerAction(VP_InputSetup.InControlPlayerActionIds.BUTT_ATTACK_ID);
	        SpecialAttack = CreatePlayerAction(VP_InputSetup.InControlPlayerActionIds.SPECIAL_ATTACK_ID);
            Attack = CreatePlayerAction(VP_InputSetup.InControlPlayerActionIds.ATTACK_ID);
            Confirm = CreatePlayerAction(VP_InputSetup.InControlPlayerActionIds.CONFIRM_ID);
            Sprint = CreatePlayerAction(VP_InputSetup.InControlPlayerActionIds.SPRINT_ID);
            Jump = CreatePlayerAction(VP_InputSetup.InControlPlayerActionIds.JUMP_ID);
            Interact = CreatePlayerAction(VP_InputSetup.InControlPlayerActionIds.INTERACT_ID);
            Cancel = CreatePlayerAction(VP_InputSetup.InControlPlayerActionIds.CANCEL_ID);
            Menu = CreatePlayerAction(VP_InputSetup.InControlPlayerActionIds.MENU_ID);

            Right = CreatePlayerAction(VP_InputSetup.InControlPlayerActionIds.MOVE_RIGHT_ID);
            Left = CreatePlayerAction(VP_InputSetup.InControlPlayerActionIds.MOVE_LEFT_ID);
            Up = CreatePlayerAction(VP_InputSetup.InControlPlayerActionIds.MOVE_UP_ID);
            Down = CreatePlayerAction(VP_InputSetup.InControlPlayerActionIds.MOVE_DOWN_ID);
            Move = CreateTwoAxisPlayerAction(Left, Right, Down, Up);

            DpadRight = CreatePlayerAction(VP_InputSetup.InControlPlayerActionIds.DPAD_RIGHT_ID);
            DpadLeft = CreatePlayerAction(VP_InputSetup.InControlPlayerActionIds.DPAD_LEFT_ID);
            DpadUp = CreatePlayerAction(VP_InputSetup.InControlPlayerActionIds.DPAD_UP_ID);
            DpadDown = CreatePlayerAction(VP_InputSetup.InControlPlayerActionIds.DPAD_DOWN_ID);
            DPad = CreateTwoAxisPlayerAction(DpadLeft, DpadRight, DpadDown, DpadUp);


            RotateRight = CreatePlayerAction(VP_InputSetup.InControlPlayerActionIds.ROTATE_RIGHT_ID);
            RotateLeft = CreatePlayerAction(VP_InputSetup.InControlPlayerActionIds.ROTATE_LEFT_ID);
            RotateUp = CreatePlayerAction(VP_InputSetup.InControlPlayerActionIds.ROTATE_UP_ID);
            RotateDown = CreatePlayerAction(VP_InputSetup.InControlPlayerActionIds.ROTATE_DOWN_ID);
            Rotate = CreateTwoAxisPlayerAction(RotateLeft, RotateRight, RotateDown, RotateUp);

            AddPlayerActionsToList();

            CreateExtraInputs();
#endif
        }

        public override Vector2 GetMovement()
        {
#if USE_INCONTROL
            return Move.Value;
#else
            return base.GetMovement();
#endif
        }

	    public override bool ButtSmashIsPressed()
	    {
#if USE_INCONTROL
		    return !m_blocked && ((m_useUnityKeys && base.ButtSmashIsPressed()) || (ButtAttack.IsPressed || base.ButtSmashIsPressed()));
#else
		    return !m_blocked && base.ButtSmashIsPressed();
#endif
	    }

	    public override bool ButtSmashWasPressed()
	    {
#if USE_INCONTROL
		    return !m_blocked && ((m_useUnityKeys && base.ButtSmashWasPressed()) || (ButtAttack.WasPressed || base.ButtSmashWasPressed()));
#else
		    return !m_blocked && base.ButtSmashWasPressed();
#endif
	    }

	    public override bool ButtSmashWasReleased()
	    {
#if USE_INCONTROL
		    return !m_blocked && ((m_useUnityKeys && base.ButtSmashWasReleased()) || (ButtAttack.WasReleased || base.ButtSmashWasReleased()));
#else
		    return !m_blocked && base.ButtSmashWasReleased();
#endif
	    }
         

        public virtual void CreateExtraInputs()
        {

        } 
        
        public virtual void AddPlayerActionsToList()
        {
#if USE_INCONTROL
            if (PlayerActionList == null)
                PlayerActionList = new List<IInputControl>();

	        PlayerActionList.Add(SpecialAttack);
	        PlayerActionList.Add(ButtAttack);
            PlayerActionList.Add(Attack);
            PlayerActionList.Add(Menu);
            PlayerActionList.Add(DPad);
            PlayerActionList.Add(Interact);
            PlayerActionList.Add(Cancel);
            PlayerActionList.Add(Move);
            PlayerActionList.Add(Rotate);
#endif
        }  
        
        public virtual void AddExtraPlayerActionsToList()
        {

        }

        public override void SetupInputs()
        {
            base.SetupInputs();
           // RemapToIndex(m_playerIndex, true);
        }

        public override void CheckBlock(bool _blockIfNull = false)
        {
#if USE_INCONTROL
            var devices = InputManager.Devices;
	        m_blocked = _blockIfNull && m_playerIndex > 0 && (devices == null || m_playerIndex > devices.Count || devices.Count > 0);
	        
	        if (m_blocked && m_useUnityKeysIfNoGamePad)
	        {
	        	m_useUnityKeys = true;
	        	m_combineBoth = false;
	        	m_blocked = false;
	        }
	        
            Device = (m_playerIndex >= 0 && devices != null && m_playerIndex < devices.Count && devices.Count > 0) ? devices[m_playerIndex] : null;
            // Debug.Log("Block if null " + _blockIfNull + " _devices Count: " + devices.Count + " and index: " + m_playerIndex);
#else
             m_blocked = _blockIfNull;
#endif
        }


        public override void RemapToIndex(int _idx, bool _blockIfNull = false)
	    {
            m_playerIndex = _idx;
            CheckBlock(_blockIfNull);
        }

        public override float HorizontalMoveValue()
        {
#if USE_INCONTROL
#if (UNITY_EDITOR || UNITY_STANDALONE)
            return m_blocked ? 0f : (m_useUnityKeys ? base.HorizontalMoveValue() : (m_combineBoth ? base.HorizontalMoveValue() + Move.X : Move.X));
#else
            return m_blocked ? 0f : Move.X;
#endif
#else
            return m_blocked ? 0f : base.HorizontalMoveValue();
#endif
        }

        public override float VerticalMoveValue()
        {
#if USE_INCONTROL
#if UNITY_EDITOR || UNITY_STANDALONE
            return m_blocked ? 0f : (m_useUnityKeys ? base.VerticalMoveValue() : (m_combineBoth ? base.VerticalMoveValue() + Move.Y : Move.Y));
#else
            return m_blocked ? 0f : Move.Y;
#endif
#else
            return m_blocked ? 0f : base.VerticalMoveValue();
#endif
        }


        public override void SetupInputs(VP_InputKeyData _keysToCopy, bool _blockIfNull = false)
        {
            base.SetupInputs(_keysToCopy, _blockIfNull);

            var m_defaultData = _keysToCopy as VP_InControlInputData;
#if USE_INCONTROL
            BindInputs
            (
                m_defaultData.ConfirmKeys, m_defaultData.ConfirmInControl,
                m_defaultData.JumpKeys, m_defaultData.JumpInControl,
                m_defaultData.InteractKeys, m_defaultData.InteractInControl,
                m_defaultData.CancelKeys, m_defaultData.CancelInControl,
                m_defaultData.MenuKeys, m_defaultData.MenuInControl,
                m_defaultData.RightKeys, m_defaultData.RightInControl,
                m_defaultData.LeftKeys, m_defaultData.LeftInControl,
                m_defaultData.UpKeys, m_defaultData.UpInControl,
                m_defaultData.DownKeys, m_defaultData.DownInControl,
                m_defaultData.DpadRightKeys, m_defaultData.DpadRightInControl,
                m_defaultData.DpadLeftKeys, m_defaultData.DpadLeftInControl,
                m_defaultData.DpadUpKeys, m_defaultData.DpadUpInControl,
                m_defaultData.DpadDownKeys, m_defaultData.DpadDownInControl,
                m_defaultData.RotateRightKeys, m_defaultData.RotateRightInControl,
                m_defaultData.RotateLeftKeys, m_defaultData.RotateLeftInControl,
                m_defaultData.RotateUpKeys, m_defaultData.RotateUpInControl,
                m_defaultData.RotateDownKeys, m_defaultData.RotateDownInControl,
	            m_defaultData.SprintKeys, m_defaultData.SprintInControl,
	            m_defaultData.SpecialAttackKeys, m_defaultData.SpecialAttackInControl,
	            m_defaultData.AttackKeys, m_defaultData.AttackInControl,
	            m_defaultData.ButtAttackKeys, m_defaultData.ButtAttackInControl
           );

	        m_combineBoth = m_defaultData.m_combineWithUnityKeys;
	        
	        m_useUnityKeys = m_defaultData.m_useUnityKeys;
	        m_useUnityKeysIfNoGamePad = m_defaultData.m_useUnityKeysIfNoGamePad;
#endif
        }

#if USE_INCONTROL

        public virtual void BindInputs
        (  
            Key[] ConfirmKeys, InputControlType[] ConfirmInControl,
            Key[] JumpKeys, InputControlType[] JumpInControl,
            Key[] InteractKeys, InputControlType[] InteractInControl,
            Key[] CancelKeys, InputControlType[] CancelInControl,
            Key[] MenuKeys, InputControlType[] MenuInControl,
            Key[] RightKeys, InputControlType[] RightInControl,
            Key[] LeftKeys, InputControlType[] LeftInControl,
            Key[] UpKeys, InputControlType[] UpInControl,
            Key[] DownKeys, InputControlType[] DownInControl,            
            Key[] DpadRightKeys, InputControlType[] DpadRightInControl,
            Key[] DpadLeftKeys, InputControlType[] DpadLeftInControl,
            Key[] DpadUpKeys, InputControlType[] DpadUpInControl,
            Key[] DpadDownKeys, InputControlType[] DpadDownInControl,            
            Key[] RotateRightKeys, InputControlType[] RotateRightInControl,
            Key[] RotateLeftKeys, InputControlType[] RotateLeftInControl,
            Key[] RotateUpKeys, InputControlType[] RotateUpInControl,
            Key[] RotateDownKeys, InputControlType[] RotateDownInControl,
            Key[] SprintKeys, InputControlType[] SprintInControl,
	        Key[] SpecialAttackKeys, InputControlType[] SpecialAttackInControl,
	        Key[] AttackKeys, InputControlType[] AttackInControl,
	        Key[] ButtAttackKeys, InputControlType[] ButtAttackInControl,
            bool IncludeUnknownControllers = true, 
            uint MaxAllowedBindings = 4,
            bool UnsetDuplicateBindingsOnSet = true
        )
	    {
	    	
		    foreach(Key k in ButtAttackKeys)
			    Attack.AddDefaultBinding(k);
			    
		    foreach (InputControlType type in ButtAttackInControl)
			    Attack.AddDefaultBinding(type);
			    
	    	
		    foreach(Key k in AttackKeys)
			    Attack.AddDefaultBinding(k);
		    foreach (InputControlType type in AttackInControl)
			    Attack.AddDefaultBinding(type);
			    
		    foreach(Key k in SpecialAttackKeys)
			    SpecialAttack.AddDefaultBinding(k);
		    foreach (InputControlType type in SpecialAttackInControl)
			    SpecialAttack.AddDefaultBinding(type);
        	
	        foreach(Key k in SprintKeys)
		        Sprint.AddDefaultBinding(k);
            foreach (InputControlType type in SprintInControl)
                Sprint.AddDefaultBinding(type);


	        foreach(Key k in ConfirmKeys)
		        Confirm.AddDefaultBinding(k);
            foreach (InputControlType type in ConfirmInControl)
                Confirm.AddDefaultBinding(type);

	        foreach(Key k in JumpKeys)
		        Jump.AddDefaultBinding(k);
            foreach (InputControlType type in JumpInControl)
                Jump.AddDefaultBinding(type);


	        foreach(Key k in InteractKeys)
		        Interact.AddDefaultBinding(k);
            foreach (InputControlType type in InteractInControl)
                Interact.AddDefaultBinding(type);


	        foreach(Key k in CancelKeys)
		        Cancel.AddDefaultBinding(k);
            foreach (InputControlType type in CancelInControl)
                Cancel.AddDefaultBinding(type);

	        foreach(Key k in MenuKeys)
		        Menu.AddDefaultBinding(k);
            foreach (InputControlType type in MenuInControl)
	            Menu.AddDefaultBinding(type);


	        foreach(Key k in RightKeys)
		        Right.AddDefaultBinding(k);
	        foreach (InputControlType type in RightInControl)
		        Right.AddDefaultBinding(type);


	        foreach(Key k in LeftKeys)
		        Left.AddDefaultBinding(k);
	        foreach (InputControlType type in LeftInControl)
		        Left.AddDefaultBinding(type);


	        foreach(Key k in UpKeys)
		        Up.AddDefaultBinding(k);				
	        foreach (InputControlType type in UpInControl)
		        Up.AddDefaultBinding(type);


	        foreach(Key k in DownKeys)
		        Down.AddDefaultBinding(k);				
	        foreach (InputControlType type in DownInControl)
		        Down.AddDefaultBinding(type);

	        foreach(Key k in DpadRightKeys)
		        DpadRight.AddDefaultBinding(k);
            foreach (InputControlType type in DpadRightInControl)
                DpadRight.AddDefaultBinding(type);


	        foreach(Key k in DpadLeftKeys)
		        DpadLeft.AddDefaultBinding(k);
            foreach (InputControlType type in DpadLeftInControl)
                DpadLeft.AddDefaultBinding(type);

	        foreach(Key k in DpadUpKeys)
		        DpadUp.AddDefaultBinding(k);
            foreach (InputControlType type in DpadUpInControl)
                DpadUp.AddDefaultBinding(type);


	        foreach(Key k in DpadDownKeys)
		        DpadDown.AddDefaultBinding(k);
            foreach (InputControlType type in DpadDownInControl)
                DpadDown.AddDefaultBinding(type);

	        foreach(Key k in RotateRightKeys)
		        RotateRight.AddDefaultBinding(k);
            foreach (InputControlType type in RotateRightInControl)
	            RotateRight.AddDefaultBinding(type);


	        foreach(Key k in RotateLeftKeys)
		        RotateLeft.AddDefaultBinding(k);
            foreach (InputControlType type in RotateLeftInControl)
                RotateLeft.AddDefaultBinding(type);



	        foreach(Key k in RotateUpKeys)
		        RotateUp.AddDefaultBinding(k);
            foreach (InputControlType type in RotateUpInControl)
                RotateUp.AddDefaultBinding(type);


	        foreach(Key k in RotateDownKeys)
		        RotateDown.AddDefaultBinding(k);		        
            foreach (InputControlType type in RotateDownInControl)
                RotateDown.AddDefaultBinding(type);

            BindExtraInputs();

            ListenToBindings(IncludeUnknownControllers, MaxAllowedBindings, UnsetDuplicateBindingsOnSet);
        }

        protected virtual void ListenToBindings(bool IncludeUnknownControllers = true,
            uint MaxAllowedBindings = 4,
            bool UnsetDuplicateBindingsOnSet = true)
        {
            ListenOptions.IncludeUnknownControllers = IncludeUnknownControllers;
            ListenOptions.MaxAllowedBindings = MaxAllowedBindings;

            ListenOptions.UnsetDuplicateBindingsOnSet = UnsetDuplicateBindingsOnSet;
        }

#endif

        protected virtual void BindExtraInputs()
        {

        }

	    

        public override bool SprintIsPressed()
        {
#if USE_INCONTROL
            return !m_blocked && ((m_useUnityKeys && base.SprintIsPressed()) || (Sprint.IsPressed || base.SprintIsPressed()));
#else
            return !m_blocked && base.SprintIsPressed();
#endif
        }

        public override bool JumpIsPressed()
        {
#if USE_INCONTROL
            return !m_blocked && ((m_useUnityKeys && base.JumpIsPressed()) || (Jump.IsPressed || base.JumpIsPressed()));
#else
            return !m_blocked &&base.JumpIsPressed();
#endif
        }

        public override bool JumpWasPressed()
        {
#if USE_INCONTROL
	        return !m_blocked && ((m_useUnityKeys && base.JumpWasPressed()) || (!m_useUnityKeys && (Jump.WasPressed || base.JumpWasPressed())));
#else
            return !m_blocked && base.JumpWasPressed();
#endif
        }

        public override bool JumpWasReleased()
        {
#if USE_INCONTROL
            return !m_blocked && ((m_useUnityKeys && base.JumpWasReleased()) || (Jump.WasReleased || base.JumpWasReleased()));
#else
            return !m_blocked && base.JumpWasReleased();
#endif
        }
        
        public override bool SelectInputIsPressed()
        {
#if USE_INCONTROL
            return !m_blocked && ((m_useUnityKeys && base.SelectInputIsPressed()) || (Confirm.IsPressed || base.SelectInputIsPressed()));
#else
            return !m_blocked && base.SelectInputIsPressed();
#endif
        }

        public override bool SelectInputWasPressed()
        {
#if USE_INCONTROL
            return !m_blocked && ((m_useUnityKeys && base.SelectInputWasPressed()) || (Confirm.WasPressed || base.SelectInputWasPressed()));
#else
            return !m_blocked && base.SelectInputWasPressed();
#endif
        }

        public override bool SelectInputWasReleased()
        {
#if USE_INCONTROL
            return !m_blocked && ((m_useUnityKeys && base.SelectInputWasReleased()) || (Confirm.WasReleased || base.SelectInputWasReleased()));
#else
            return !m_blocked && base.SelectInputWasReleased();
#endif
        }

        public override bool InteractInputIsPressed()
        {
#if USE_INCONTROL
            return !m_blocked && ((m_useUnityKeys && base.InteractInputIsPressed()) || (Interact.IsPressed || base.InteractInputIsPressed()));
#else
            return !m_blocked && base.InteractInputIsPressed();
#endif
        }

        public override bool InteractInputWasPressed()
        {
#if USE_INCONTROL
            return !m_blocked && ((m_useUnityKeys && base.InteractInputWasPressed()) || (Interact.WasPressed || base.InteractInputWasPressed()));
#else
            return !m_blocked && base.InteractInputWasPressed();
#endif
        }

        public override bool InteractInputWasReleased()
        {
#if USE_INCONTROL
            return !m_blocked && ((m_useUnityKeys && base.InteractInputWasReleased()) || (Interact.WasReleased || base.InteractInputWasReleased()));
#else
            return !m_blocked && base.InteractInputWasReleased();
#endif
        }


        public override bool AttackInputWasPressed()
        {
#if USE_INCONTROL
            return !m_blocked && ((m_useUnityKeys && base.AttackInputWasPressed()) || (Attack.WasPressed || base.AttackInputWasPressed()));
#else
            return !m_blocked && base.AttackInputWasPressed();
#endif
        }

        public override bool AttackInputWasReleased()
        {
#if USE_INCONTROL
            return !m_blocked && ((m_useUnityKeys && base.AttackInputWasReleased()) || (Attack.WasReleased || base.AttackInputWasReleased()));
#else
            return !m_blocked && base.AttackInputWasReleased();
#endif
        }


        public override bool AttackInputIsPressed()
        {
#if USE_INCONTROL
            return !m_blocked && ((m_useUnityKeys && base.AttackInputIsPressed()) || (Attack.IsPressed || base.AttackInputIsPressed()));
#else
            return !m_blocked && base.AttackInputIsPressed();
#endif
        }
    }
}