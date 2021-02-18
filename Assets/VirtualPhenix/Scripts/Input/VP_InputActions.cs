using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if USE_INCONTROL
using InControl;
#endif

namespace VirtualPhenix.Inputs
{
    [System.Serializable]
#if USE_INCONTROL
    public partial class VP_InputActions : PlayerActionSet
#else
    public partial class VP_InputActions
#endif
    {
        [Header("General Configuration"), Space]
        public bool m_blocked = false;
        public int m_playerIndex = -1;

        public VP_InputActions()
        {

        }

        public virtual void BlockInput(bool _value)
        {
            m_blocked = _value;
        }     
        
        public virtual void CheckBlock(bool _blockIfNull = false)
        {

        }

        public virtual void SetupInputs()
        {

        }

        public virtual void SetupInputs(VP_InputKeyData keys, bool _blockIfNull = false)
        {

        }

        public static VP_InputActions CreateWithDefaultBindings()
        {
            var playerActions = new VP_InputActions();
            
            return playerActions;
        }

        public virtual Vector2 GetMovement()
        {
            return Vector2.zero;
        }

        public virtual void RemapToIndex(int _idx, bool _blockIfNull = false)
	    {
		    m_playerIndex = _idx;
		    m_blocked = _blockIfNull && _idx >= 0;
        }

	    public virtual bool ButtSmashIsPressed()
	    {
		    return false;
	    }

	    public virtual bool ButtSmashWasPressed()
	    {
		    return false;
	    }

	    public virtual bool ButtSmashWasReleased()
	    {
		    return false;
	    }
         

        public virtual bool JumpIsPressed()
        {
            return false;
        }

        public virtual bool JumpWasPressed()
        {
            return false;
        }

        public virtual bool JumpWasReleased()
        {
            return false;
        }
         
        public virtual bool SprintIsPressed()
        {
            return false;
        }

        public virtual bool SprintWasPressed()
        {
            return false;
        }

        public virtual bool SprintWasReleased()
        {
            return false;
        }

        public virtual float HorizontalMoveValue()
        {
            return 0f;
        }

        public virtual float VerticalMoveValue()
        {
            return 0f;
        }

        public virtual float HorizontalDPadValue()
        {
            return 0f;
        }

        public virtual float VerticalDPadValue()
        {
            return 0f;
        }

        public virtual float HorizontalRotateValue()
        {
            return 0f;
        }

        public virtual float VerticalRotateValue()
        {
            return 0f;
        }

        public virtual bool AttackInputWasPressed()
        {
            return false;
        }

        public virtual bool AttackInputWasReleased()
        {
            return false;
        }

        public virtual bool AttackInputIsPressed()
        {
            return false;
        }

        public virtual bool SelectInputIsPressed()
        {
            return false;
        }

        public virtual bool SelectInputWasPressed()
        {
            return false;
        }

        public virtual bool SelectInputWasReleased()
        {
            return false;
        }

        public virtual bool InteractInputIsPressed()
        {
            return false;
        }

        public virtual bool InteractInputWasPressed()
        {
            return false;
        }

        public virtual bool InteractInputWasReleased()
        {
            return false;
        }


        public virtual void DestroyAction()
        {

        }
    }

}