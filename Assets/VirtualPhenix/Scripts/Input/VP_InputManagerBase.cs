using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VirtualPhenix.Inputs;
using VirtualPhenix.ResourceReference;

namespace VirtualPhenix
{

    [System.Serializable]
    public class VP_MappedInputTriggerEvent<T> : UnityEngine.Events.UnityEvent<Inputs.VP_InputActions, T>
    {

    }

    [DefaultExecutionOrder(VP_ExecutingOrderSetup.INPUT_MANAGER_BASE)]
    public abstract class VP_InputManagerBase : VP_SingletonMonobehaviour<VP_InputManagerBase>
    {
	    [Header("Default Action Resources"),Space]
        [SerializeField] protected bool m_createDefaultActions = true;
        [SerializeField] protected VP_InputKeyResources m_defaultActions;
	    [SerializeField] protected bool m_overridePlayerIfExist = true;

        [Header("Generated Actions"), Space]
        [SerializeField] protected bool m_destroyActionsOnDestroy = true;

	    [Header("Runtime Actions -- Can be rebinded -- Serialized for debug")]
#if ODIN_INSPECTOR	    
	    [Sirenix.Serialization.OdinSerialize] protected VP_SerializedInputActions m_createdActions = new VP_SerializedInputActions();
#else	    
        [SerializeField]  protected VP_SerializedInputActions m_createdActions = new VP_SerializedInputActions();
#endif

        [Header("Events"),Space]
	    public UnityEvent<int> OnDeviceAttach = new UnityEvent<int>();
        public UnityEvent<int> OnDeviceDettach = new UnityEvent<int>();
        public UnityEvent<int> OnDeviceChange = new UnityEvent<int>();
	    public UnityEvent<VP_InputActions> OnActionDestroyed = new UnityEvent<VP_InputActions>();

    	[Header("Connected Controllers"),Space]
	    [SerializeField] protected int m_deviceListCount;
	    
        [Header("Event Remap"),Space]
        [SerializeField] protected bool m_useDelegates;

#if ODIN_INSPECTOR
        [Sirenix.Serialization.OdinSerialize] protected VP_MappedInputDictionary<VP_MappedInput> m_mappedActions = new VP_MappedInputDictionary<VP_MappedInput>();
#else
		[SerializeField] protected VP_MappedInputDictionary<VP_MappedInput> m_mappedActions = new VP_MappedInputDictionary<VP_MappedInput>();
#endif
        public VP_MappedInputDictionary<VP_MappedInput> MappedActions { get { return m_mappedActions; } }

        public virtual VP_SerializedInputActions CreatedActions
        {
            get
            {
                return m_createdActions;
            }
        }

        public virtual VP_InputActions PlayerActions
        {
            get
            {
	            return m_createdActions[VP_InputSetup.PLAYER_ID];
            }
        }

	    public virtual int ConnectedControllerCount
	    {
	    	get
	    	{
	    		return m_deviceListCount;
	    	}
	    }

	    protected override void Start()
	    {
	    	base.Start();
	    	
	    	InitDeviceListCount();
	    }

	    protected virtual void InitDeviceListCount()
	    {
	    	m_deviceListCount = Input.GetJoystickNames().Length;
	    }

	    protected override void Reset()
	    {
	    	base.Reset();
	    	m_defaultActions = Resources.Load<VP_InputKeyResources>("Database/Inputs/Input Resources");
	    }

        public virtual void AddActionListener(VP_MappedInput _action, UnityEngine.Events.UnityAction<VP_InputActions, VP_MappedInput> _callback)
        {
            if (m_useDelegates)
            {
                if (_action == null)
                {
                    Debug.LogError("Mapped Input is null");
                    return;
                }

                if (_callback == null)
                {
                    Debug.LogError("Callback is null");
                    return;
                }

                if (!m_mappedActions.Contains(_action))
                {
                    m_mappedActions.Add(_action, new VP_MappedInputTriggerEvent<VP_MappedInput>());
                }

                m_mappedActions[_action].AddListener(_callback);
                _action.AddCallback(_callback);
            }
        }

        public virtual void RemoveActionListener(VP_MappedInput _action, UnityEngine.Events.UnityAction<VP_InputActions, VP_MappedInput> _callback)
        {
            if (m_useDelegates && m_mappedActions.Contains(_action))
            {
                m_mappedActions[_action].RemoveListener(_callback);
                _action.RemoveCallback(_callback);

                if (_action.m_listeners == 0)
                {
                    m_mappedActions.Remove(_action);
                }
            }
        }

        public virtual void TriggerAction(UnityEngine.Events.UnityEvent<VP_InputActions, VP_MappedInput> _callback, VP_InputActions _ia, VP_MappedInput _action)
        {
            //Debug.Log("Trigger action");
            _callback.Invoke(_ia, _action);
        }

        public virtual void CheckAllInputs()
        {
            foreach (var s in m_createdActions.Keys)
                m_createdActions[s].CheckBlock(true);
        }

        public virtual void BlockAllInputs(bool _value)
        {
            foreach (var s in m_createdActions.Keys)
                m_createdActions[s].BlockInput( _value);
        }

        protected override void Initialize()
        {
            base.Initialize();

            if (m_useDelegates && m_mappedActions == null)
            {
                m_mappedActions = new VP_MappedInputDictionary<VP_MappedInput>();
            }

            if (m_createDefaultActions)
                SetDefaultActions();
        }

        public virtual T1 GetOrCreateCastedActionSetOfType<T1, T2>(string _key, T1 _action, ref T2 _initData, int _idx = -1, bool _blockIfNull = false) where T1 : VP_InputActions where T2 : VP_InputKeyData
	    {
		    var ia = m_createdActions.ContainsKey(_key) && m_createdActions[_key] is T1 ? m_createdActions[_key] as T1 : null;
            
		    if (ia == null)
			    return default(T1);
            
		    if (_idx >= 0)
		    {
			    ia.RemapToIndex(_idx, _blockIfNull);
		    }

		    return ia;
	    }

        protected virtual void OnUpdate(ulong _ulong, float _float)
        {

        }

        public virtual void DestroyAction(string _key)
	    {
	    	if (m_createdActions.Contains(_key))
	    	{
                var actio = m_createdActions[_key];
                actio.DestroyAction();

                OnActionDestroyed.Invoke(actio);
	    		m_createdActions.Remove(_key);
	    	}
	    }

        public virtual void RemapInputActionsIndex(string _key, int newIndex, bool _blockIfNull = false)
        {
            if (m_createdActions.ContainsKey(_key))
            {
                m_createdActions[_key].RemapToIndex(newIndex, _blockIfNull);
            }
        }

	    public virtual T GetOrCreateActionSet<T>(string _key, int _idx = -1, bool _blockIfNull = false) where T : VP_InputActions
	    {
		    var ia = m_createdActions.ContainsKey(_key) ? m_createdActions[_key] : CreateGenericNewInputAction<T>(_key);

		    if (_idx >= 0)
		    {
			    ia.RemapToIndex(_idx, _blockIfNull);
		    }

		    return (T)ia;
	    }

	    public virtual T CreateGenericNewInputAction<T>(string key) where T : VP_InputActions
	    {
	    	T ia =default(T);
	    	AddExistingActionSet<T>(key, ia);
	    	return ia;
	    }

	    public virtual void AddExistingActionSet<T>(string _key, T _actionset, bool _replaceIfExistInDic = false) where T : VP_InputActions
	    {
	    	if (!m_createdActions.Contains(_key))
	    	{
	    		m_createdActions.Add(_key, _actionset);
	    	}
	    	else if (_replaceIfExistInDic)
	    	{
	    		m_createdActions[_key] = _actionset;
	    	}
	    }

        public virtual VP_InputActions GetActionSet(string _key, int _idx = -1, bool _blockIfNull = false)
        {
            var ia = m_createdActions.ContainsKey(_key) ? m_createdActions[_key] : PlayerActions;

            if (_idx >= 0)
            {
                ia.RemapToIndex(_idx, _blockIfNull);
            }

            return ia;
        }

        public virtual void SetDefaultActions()
        {

            m_createdActions = new VP_SerializedInputActions();
            if (!m_defaultActions)
                return;

            var ac = m_defaultActions.Resources;
            foreach (var k in ac.Keys)
            {
                var ip = new VP_InputActions();
                ip.SetupInputs(ac[k]);
                m_createdActions.Add(k, ip);
            }
        }

        public virtual void LoadCurrentControls(VP_SerializedInputActions _currentControls, bool _clearAllFirst = false)
        {
            if (_clearAllFirst)
                m_createdActions = new VP_SerializedInputActions();

            foreach (var k in _currentControls.Keys)
            {
                if (!m_createdActions.ContainsKey(k))
                    m_createdActions.Add(k, _currentControls[k]);
                else
                    m_createdActions[k] = _currentControls[k];
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (m_destroyActionsOnDestroy)
            {
                foreach (VP_InputActions ac in m_createdActions.Values)
                {
                    ac.DestroyAction();
                }
            }
        }
    }
}