using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix.Inputs;

namespace VirtualPhenix.Controllers
{
    [DefaultExecutionOrder(VP_ExecutingOrderSetup.CHARACTER_CONTROLLER), AddComponentMenu("")]
    /// <summary>
    /// This characters will be playable
    /// </summary>
    public class VP_PlayableCharacterController : VP_CharacterController
    {
        /// <summary>
        /// Only Playable controllers have Input Actions ofc -> Controlled NPCs will inherit from this class too
        /// </summary>
        [Header("Input Actions - Playable"), Space]
        [SerializeField] protected VP_InputActions m_inputActions;
        [SerializeField] protected string m_inputActionsID = "Player";
        [SerializeField] protected int m_controllerIndex = 0;
        [SerializeField] protected bool m_blockIfNoController = true;
	    [SerializeField] protected bool m_enableAlwaysOnEditor = false;
        public bool m_useInput = true;

        public virtual VP_InputActions InputActionSet
        {
            get
            {
                return m_inputActions;
            }
        }

        protected override void Initialize()
        {
            base.Initialize();
            SetActionSet();
        }

        protected override void StartAllListeners()
        {
            VP_InputManagerBase.Instance.OnDeviceAttach.AddListener(CheckAttachedDevice);
            VP_InputManagerBase.Instance.OnDeviceDettach.AddListener(CheckDettachedDevice);

            base.StartAllListeners();
        }

        protected override void StopAllListeners()
        {
            if (!VP_MonobehaviourSettings.Quiting)
            {
                VP_InputManagerBase.Instance.OnDeviceAttach.RemoveListener(CheckAttachedDevice);
                VP_InputManagerBase.Instance.OnDeviceDettach.RemoveListener(CheckDettachedDevice);
            }

            base.StopAllListeners();
        }

        protected virtual void CheckAttachedDevice(int _idx)
        {
            if (m_controllerIndex.Equals(_idx))
            {
                SetActionSet();
            }
        }

        protected virtual void CheckDettachedDevice(int _idx)
        {
            SetActionSet();
        }

        protected virtual void CheckNotBlockingOnSet()
        {
#if UNITY_STANDALONE || UNITY_EDITOR
	        if (m_enableAlwaysOnEditor)
            	m_blockIfNoController = false;
#endif
        }


        public virtual void SetActionSet(string _inputAction = "")
        {
            if (_inputAction.IsNullOrEmpty())
                _inputAction = m_inputActionsID;
            else
                m_inputActionsID = _inputAction;

            CheckNotBlockingOnSet();

            m_inputActions = VP_InputManagerBase.Instance.GetActionSet(_inputAction, m_controllerIndex, m_blockIfNoController);

            if (m_inputActions == null)
            {
                m_inputActions = new VP_InputActions();
                m_inputActions.m_blocked = true;
            }
        }

        public virtual void BlockActionSet(bool _value)
        {
            if (m_inputActions != null)
            {
                if (_value)
                {
                    m_inputActions.BlockInput(true);
                }
                else
                {
                    m_inputActions.CheckBlock(m_blockIfNoController);
                }
            }
        }
        
	    protected override bool ButtSmashIsPressed()
	    {
		    return m_useInput && m_inputActions != null ? m_inputActions.ButtSmashIsPressed() : false;
	    }
        
	    protected override bool ButtSmashWasPressed()
	    {
		    return m_useInput && m_inputActions != null ? m_inputActions.ButtSmashWasPressed() : false;
	    }
        
        
        protected override bool JumpIsPressed()
        {
            return m_useInput && m_inputActions != null ? m_inputActions.JumpIsPressed() : false;
        }

        protected override bool JumpWasPressed()
        {
            return m_useInput && m_inputActions != null ? m_inputActions.JumpWasPressed() : false;
        }


        protected override bool InteractWasPressed()
        {
            return m_useInput && m_inputActions != null ? m_inputActions.InteractInputWasPressed() : false;
        }

        protected override bool IsSprinting()
        {
            return m_useInput && m_inputActions != null ? m_inputActions.SprintIsPressed() : false;
        }

        protected override float GetVerticalValue()
        {
            return m_useInput && m_inputActions != null ? m_inputActions.VerticalMoveValue() : 0f;
        }

        protected override float GetHorizontalValue()
        {
            return m_useInput && m_inputActions != null ? m_inputActions.HorizontalMoveValue() : 0f;
        }

        protected override bool CanCharacterMove()
        {
            return m_useInput && m_canCharacterMove && !m_blocked && !m_interacting;
        }
    }
}