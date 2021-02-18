#if USE_NEW_INPUT_SYSTEM
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace VirtualPhenix.Inputs
{
    [DefaultExecutionOrder(VP_ExecutingOrderSetup.NEW_INPUT_MANAGER_BASE)]
    public class VP_NewInputSystemManager : VP_InputManager<VP_NewInputSystemPlayerAction, VP_NewInputSystemKeyData>
    {
        public static new VP_NewInputSystemManager Instance
        {
            get
            {
                return (VP_NewInputSystemManager)m_instance;
            }
        }

        [Header("Debug input context"), Space]
        [SerializeField] protected bool m_debugContext;

        protected override void Initialize()
        {
            m_useDelegates = true;
            m_mappedActions = new VP_MappedInputDictionary<VP_MappedInput>();

            base.Initialize();
        }


        public override VP_NewInputSystemPlayerAction DefaultInputActionValue()
        {
            return new VP_NewInputSystemPlayerAction();
        }

        public override VP_NewInputSystemKeyData DefaultInputKeyValue()
        {
            return new VP_NewInputSystemKeyData();
        }


        public virtual void OnDeviceRegained(VP_NewInputSystemPlayerAction _input)
        {
            OnDeviceAttach.Invoke(_input.PlayerInput.playerIndex);
        }  
        
        public virtual void OnControlsChanged(VP_NewInputSystemPlayerAction _input)
        {
            OnDeviceChange.Invoke(_input.PlayerInput.playerIndex);
        }

        public virtual void OnDeviceLost(VP_NewInputSystemPlayerAction _input)
        {
            OnDeviceDettach.Invoke(_input.PlayerInput.playerIndex);
        }

        protected override void InitDeviceListCount()
        {
            m_deviceListCount = InputSystem.devices.Count;
        }

        public virtual void SetPlayerInputToActionSet(PlayerInput _input, string _set)
        {
            if (m_createdActions.Contains(_set) && m_createdActions[_set] is VP_NewInputSystemPlayerAction)
            {
                ((VP_NewInputSystemPlayerAction)m_createdActions[_set]).SetPlayerInput(_input);
            }
        }

        public virtual bool ExistMappedInputWithInputActionAsset(InputActionAsset _actionSet, InputAction _action, out VP_NewInputSystemMappedInput _mapped)
        {
            foreach (VP_MappedInput map in m_mappedActions.Keys)
            {
                if (map is VP_NewInputSystemMappedInput)
                {
                    var remap = ((VP_NewInputSystemMappedInput)map);
                    if (remap.m_inputActionAsset.Equals(_actionSet) && remap.m_action.Equals(_action))
                    {
                        _mapped = remap;
                        return true;
                    }
                }
            }
       
            _mapped = null;
            return false;
        }


        public virtual VP_NewInputSystemMappedInput AddActionListener(InputActionAsset _actionAsset, InputAction _action, UnityEngine.Events.UnityAction<VP_InputActions, VP_MappedInput> _callback)
        {
            VP_NewInputSystemMappedInput ret = null;

            if (_action != null)
            {
                if (!ExistMappedInputWithInputActionAsset(_actionAsset, _action, out ret))
                {
                    ret = new VP_NewInputSystemMappedInput() { m_inputActionAsset = _actionAsset, m_action = _action };
                    base.AddActionListener(ret, _callback);
                    return ret;
                }

                base.AddActionListener(ret, _callback);
            }

            return ret;
        }

        public virtual void RemoveActionListener(InputActionAsset _actionAsset, InputAction _action, UnityEngine.Events.UnityAction<VP_InputActions, VP_MappedInput> _callback)
        {
            VP_NewInputSystemMappedInput ret = null;
            if (ExistMappedInputWithInputActionAsset(_actionAsset, _action, out ret))
            {
                if (ret.HasCallback(_callback))
                {
                    base.RemoveActionListener(ret, _callback);
                }
            }
        }
        public virtual void RemoveActionListener(PlayerInput _playerInput, InputAction _action, UnityEngine.Events.UnityAction<VP_InputActions, VP_MappedInput> _callback)
        {
            RemoveActionListener(_playerInput.actions, _action, _callback);
        }

        public virtual VP_NewInputSystemMappedInput AddActionListener(PlayerInput _playerInput, InputAction _action, UnityEngine.Events.UnityAction<VP_InputActions, VP_MappedInput> _callback)
        {
            return AddActionListener(_playerInput.actions, _action, _callback);
        }

        public virtual bool ExistMappedInputWithAnyInputActionAsset(InputActionAsset _actionSet, out VP_NewInputSystemMappedInput _mapped)
        {
            foreach (VP_MappedInput map in m_mappedActions.Keys)
            {
                if (map is VP_NewInputSystemMappedInput)
                {
                    var remap = ((VP_NewInputSystemMappedInput)map);
                    if (remap.m_inputActionAsset.Equals(_actionSet) && remap.m_action == null)
                    {
                        _mapped = remap;
                        return true;
                    }
                }
            }

            _mapped = null;
            return false;
        }


        public virtual VP_NewInputSystemMappedInput AddActionListener(InputActionAsset _actionAsset, UnityEngine.Events.UnityAction<VP_InputActions, VP_MappedInput> _callback)
        {
            VP_NewInputSystemMappedInput ret = null;

            if (_actionAsset != null)
            {
                ret = new VP_NewInputSystemMappedInput() { m_inputActionAsset = _actionAsset, m_action = null };
                base.AddActionListener(ret, _callback);
                return ret;
            }

            return ret;
        }

        public virtual void RemoveActionListener(InputActionAsset _actionAsset, UnityEngine.Events.UnityAction<VP_InputActions, VP_MappedInput> _callback)
        {
            VP_NewInputSystemMappedInput ret = null;
            if (ExistMappedInputWithAnyInputActionAsset(_actionAsset, out ret))
            {
                if (ret.HasCallback(_callback))
                {
                    base.RemoveActionListener(ret, _callback);
                }
            }
        }
        public virtual void RemoveActionListener(PlayerInput _playerInput, UnityEngine.Events.UnityAction<VP_InputActions, VP_MappedInput> _callback)
        {
            RemoveActionListener(_playerInput.actions, _callback);
        }

        public virtual VP_NewInputSystemMappedInput AddActionListener(PlayerInput _playerInput, UnityEngine.Events.UnityAction<VP_InputActions, VP_MappedInput> _callback)
        {
            return AddActionListener(_playerInput.actions, _callback);
        }

        public virtual void InputTriggered(VP_NewInputSystemPlayerAction _action, InputAction.CallbackContext _value)
        {
            if (m_debugContext)
                Debug.Log("Triggered " + _value.action);

            foreach (VP_MappedInput map in m_mappedActions.Keys)
            {
                if (map is VP_NewInputSystemMappedInput)
                {
                    var remap = (VP_NewInputSystemMappedInput)map;

                    if (remap.m_inputActionAsset.Equals(_action.PlayerInputAsset) && (_value.action.Equals(remap.m_action) || remap.m_action == null))
                    {
                        remap.m_generatedContext = _value;

                        TriggerAction(m_mappedActions[remap], _action, remap);
                    }
                }
            }
        }
    }
}
#endif