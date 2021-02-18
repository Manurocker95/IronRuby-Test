#if USE_NEW_INPUT_SYSTEM
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace VirtualPhenix.Inputs
{
    public class VP_NewInputSystemPlayerAction : VP_PlayerActions
    {
        [SerializeField] protected PlayerInput m_playerInput;

        protected Dictionary<string, List<InputAction.CallbackContext>> m_pressedCallbacks;

        public PlayerInput PlayerInput { get { return m_playerInput; } }
        public InputActionAsset PlayerInputAsset { get { return m_playerInput.actions; } }

        public VP_NewInputSystemPlayerAction()
        {

        }

        public VP_NewInputSystemPlayerAction(PlayerInput _input)
        {
            SetPlayerInput(_input);
        }

        public void SetPlayerInput(PlayerInput _input)
        {
            m_playerInput = _input;
            StartInputListeners();
        }

        public override void DestroyAction()
        {
            base.DestroyAction();
            StopInputListeners();
        }

        public virtual void StartInputListeners()
        {
            m_playerInput.onDeviceLost += OnDeviceLost;
            m_playerInput.onDeviceRegained += OnDeviceRegained;
            m_playerInput.onControlsChanged += OnControlsChanged;
            m_playerInput.onActionTriggered += CheckInput;

            foreach (var ev in m_playerInput.actionEvents)
                ev.AddListener(CheckInput);
        }

        public virtual void StopInputListeners()
        {
            m_playerInput.onDeviceLost -= OnDeviceLost;
            m_playerInput.onDeviceRegained -= OnDeviceRegained;
            m_playerInput.onControlsChanged -= OnControlsChanged;
            m_playerInput.onActionTriggered -= CheckInput;

            foreach (var ev in m_playerInput.actionEvents)
                ev.RemoveListener(CheckInput);
        }

        public virtual void OnControlsChanged(PlayerInput _input)
        {
            VP_NewInputSystemManager.Instance.OnControlsChanged(this);
        }

        public virtual void OnDeviceRegained(PlayerInput _input)
        {
            VP_NewInputSystemManager.Instance.OnDeviceRegained(this);
        }

        public virtual void OnDeviceLost(PlayerInput _input)
        {
            VP_NewInputSystemManager.Instance.OnDeviceLost(this);
        }

        public void CheckInput(InputAction.CallbackContext value)
        {
            VP_NewInputSystemManager.Instance.InputTriggered(this, value);
        }
    }
}
#endif