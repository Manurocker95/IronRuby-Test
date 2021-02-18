using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix.Inputs;

namespace VirtualPhenix
{
	[DefaultExecutionOrder(VP_ExecutingOrderSetup.INPUT_MANAGER)]
    public class VP_InputManager<T, T0> : VP_InputManagerBase where T : VP_PlayerActions where T0 : VP_InputKeyData
    {
        [Header("Generic Data")]
        [SerializeField] protected bool m_createDefaultPlayerActions = true;
#if ODIN_INSPECTOR
        [Sirenix.Serialization.OdinSerialize] protected T0 m_defaultData = default(T0);
#else
        [SerializeField] protected T0 m_defaultData = default(T0);
#endif	    
        protected override void Reset()
        {
            base.Reset();

            m_defaultData = DefaultInputKeyValue();
        }

        protected override void Initialize()
	    {
		    base.Initialize();
		    
		    if (m_defaultData == null)
			    m_defaultData = DefaultInputKeyValue();

		    if (m_createDefaultPlayerActions)
			    CreatePlayerInputs();
			   
        }

        public virtual T GetCastedActionSet(string _key, int _idx = -1, bool _blockIfNull = false)
        {
            var ia = m_createdActions.ContainsKey(_key) && m_createdActions[_key] is T ? m_createdActions[_key] as T : PlayerActions as T;
	        Debug.Log("Remapping to Index "+_idx + " for set: "+_key);
            if (_idx >= 0)
            {
            	Debug.Log("Remapping to Index "+_idx);
                ia.RemapToIndex(_idx, _blockIfNull);
            }

            return ia;
        }

	    public override T1 GetOrCreateCastedActionSetOfType<T1, T2>(string _key, T1 _action, ref T2 _initData, int _idx = -1, bool _blockIfNull = false)
	    {
		    var ia = m_createdActions.ContainsKey(_key) && m_createdActions[_key] is T1 ? m_createdActions[_key] as T1 : CreateCustomInputsAs<T1, T2>(_key, _action, ref _initData);
            
		    if (_idx >= 0)
		    {
			    ia.RemapToIndex(_idx, _blockIfNull);
		    }

		    return ia;
	    }

	    public virtual T GetOrCreateCastedActionSet(string _key, int _idx = -1, bool _blockIfNull = false)
	    {
		    var ia = m_createdActions.ContainsKey(_key) && m_createdActions[_key] is T ? m_createdActions[_key] as T : CreateCustomInputs(_key) as T;
            
		    if (_idx >= 0)
		    {
			    ia.RemapToIndex(_idx, _blockIfNull);
		    }

		    return ia;
	    }


	    protected virtual T0 CreateDataFromCopy(VP_InputKeyData _keyData)
	    {
	    	return default(T0);
	    }

        protected virtual T CreatePlayerInputs()
	    {
	    	T m_playerActions;
		    if (m_createdActions.ContainsKey(VP_InputSetup.PLAYER_ID) && !m_overridePlayerIfExist)
		    {
		    	m_playerActions = (T)m_createdActions[VP_InputSetup.PLAYER_ID];
		    	return m_playerActions;
		    }
		    
            m_playerActions = DefaultInputActionValue();
            CreateInputActions(VP_InputSetup.PLAYER_ID, out m_playerActions, DefaultInputKeyValue());
            return m_playerActions;
        }

	    protected virtual T1 CreateCustomInputsAs<T1, T2>(string _playerID, T1 inputs, ref T2 inputData) where T1 : VP_InputActions where T2 : VP_InputKeyData
	    {
		    CreateInputActionsAs(_playerID, inputs, ref inputData);
		    return inputs;
	    }

        protected virtual T CreateCustomInputs(string _playerID)
        {
            T inputs = null;
            CreateInputActions(_playerID, out inputs, DefaultInputKeyValue());
            return inputs;
        }

	    public override void SetDefaultActions()
	    {
		    m_createdActions = new VP_SerializedInputActions();
		    if (!m_defaultActions)
		    {
			    return;
		    }

		    var ac = m_defaultActions.Resources;
            if (ac != null)
            {
                foreach (var k in ac.Keys)
                {
                    var ip = DefaultInputActionValue();
                    CreateDefaultActionFromData(k, ac[k], out ip);
                }
            }
	    }


	    protected virtual void CreateDefaultActionFromData(string k, VP_InputKeyData _keyData, out T _actions)
	    {
		    _actions = DefaultInputActionValue();
	    }

	    public virtual void CreateInputActionsAs<T1, T2>(string _key, T1 _actions, ref T2 _keyData) where T1 : VP_InputActions where T2 : VP_InputKeyData
	    {
		    _actions.SetupInputs(_keyData);
            
		    if (m_createdActions.ContainsKey(_key))
			    m_createdActions[_key] = (_actions);
		    else
			    m_createdActions.Add(_key, _actions);
	    }

        public virtual void CreateInputActions(string _key, out T _actions, T0 _keyData)
        {
            _actions = DefaultInputActionValue();
	        _actions.SetupInputs(_keyData);
            
            if (m_createdActions.ContainsKey(_key))
                m_createdActions[_key] = (_actions);
            else
            	m_createdActions.Add(_key, _actions);
        }

        public virtual T DefaultInputActionValue()
        {
            return default(T);
        }

        public virtual T0 DefaultInputKeyValue()
        {
            return default(T0);
        }


        public virtual void SetBlockInputs(bool _value, string _key="Player")
        {
            if (m_createdActions.ContainsKey(_key))
            {
                m_createdActions[_key].m_blocked = _value;
            }
        }
    }
}
