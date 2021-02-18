using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix.Inputs;

#if USE_INCONTROL
using InControl;
#endif

namespace VirtualPhenix
{
	[DefaultExecutionOrder(VP_ExecutingOrderSetup.IN_CONTROL_INPUT_MANAGER),AddComponentMenu(VP_PrefixSetup.MAIN_PREFIX+"/Inputs/VP Input Manager With InControl")]
	public class VP_InControlInputManager : VP_InControlInputManagerBase<VP_InControlPlayerActions, VP_InControlInputData>
	{
		public static new VP_InControlInputManager Instance
		{
			get
			{
				return (VP_InControlInputManager)m_instance;
			}
		}

		protected override void Reset()
		{
			base.Reset();
			m_defaultData = new VP_InControlInputData();
		}

		public override VP_InControlPlayerActions DefaultInputActionValue()
		{
			return new VP_InControlPlayerActions();
		}
		
		public override VP_InControlInputData DefaultInputKeyValue()
		{
			return new VP_InControlInputData(m_defaultData);
		}

		protected override VP_InControlPlayerActions CreatePlayerInputs()
		{
			var m_playerActions = DefaultInputActionValue();
			CreateInputActions(VP_InputSetup.PLAYER_ID, out m_playerActions, DefaultInputKeyValue());
			return m_playerActions;
		}
		
		protected override VP_InControlInputData CreateDataFromCopy(VP_InputKeyData _keyData)
		{
			return new VP_InControlInputData(_keyData);
		}
		
		public override void CreateInputActions(string _key, out VP_InControlPlayerActions _actions, VP_InControlInputData _keyData)
		{
			_actions = DefaultInputActionValue();
			_actions.SetupInputs(_keyData, m_blockInputsIfControllerNotAvailable);

			if (!m_createdActions.ContainsKey(_key))
				m_createdActions.Add(_key, _actions);
			else
				m_createdActions[_key] = _actions;
		}

		
		protected override void CreateDefaultActionFromData(string k, VP_InputKeyData _keyData, out VP_InControlPlayerActions ip)
		{
			CreateInputActions(k, out ip, _keyData is VP_InControlInputData ? (VP_InControlInputData)_keyData : CreateDataFromCopy(_keyData));
		}
		      
		public override VP_InControlPlayerActions GetCastedActionSet(string _key, int _idx = -1, bool _blockIfNull = false)
		{
  
			var ia = m_createdActions.ContainsKey(_key) && m_createdActions[_key] is VP_InControlPlayerActions ? m_createdActions[_key] as VP_InControlPlayerActions: PlayerActions as VP_InControlPlayerActions;

			if (_idx >= 0)
			{
				ia.RemapToIndex(_idx, _blockIfNull);
			}

			return ia;
		}
	}
#if USE_INCONTROL
	[System.Serializable]
	public class VP_InControlActionTriggerEvent : UnityEngine.Events.UnityEvent<VP_InputActions, IInputControl>
	{

	}
#endif

	[DefaultExecutionOrder(VP_ExecutingOrderSetup.IN_CONTROL_INPUT_MANAGER)]
	/// <summary>
	/// Remember to change in Scripting Order-> InControl.InputManager to -999 or below
	/// </summary>
	public class VP_InControlInputManagerBase<T, T1> : VP_InputManager<T, T1> where T : VP_InControlPlayerActions where T1 : VP_InControlInputData
	{
		[Header("Block if controller not available")]
		[SerializeField] protected bool m_blockInputsIfControllerNotAvailable = true;

#if USE_INCONTROL
		[Header("In Control Touch Manager")]
		[SerializeField] protected TouchManager m_touchManager;
		[SerializeField] protected bool m_activateTouchCameraInMobile = false;
		[SerializeField] protected bool m_activateTouchManagerInMobile = true;
		[SerializeField] protected bool m_activateTouchCameraInOtherPlatform = false;
		[SerializeField] protected bool m_activateTouchManagerInOtherPlatform = false;

		public bool ActivateTouchCameraIfMobile { get { return m_activateTouchCameraInMobile; } }
		public TouchManager TouchManager { get { return m_touchManager; } }
		public Camera TouchManagerCamera { get { return m_touchManager != null ? m_touchManager.touchCamera : null; } }
#endif

		protected override void Initialize()
		{
			base.Initialize();
			#if USE_INCONTROL
			if (m_touchManager == null)
				transform.TryGetComponentInChildren<TouchManager>(out m_touchManager);
			#endif
			
		}

		protected override void InitDeviceListCount()
		{
#if USE_INCONTROL
			m_deviceListCount = InputManager.Devices.Count;
#else
			base.InitDeviceListCount();
#endif
		}

		protected override void Start()
		{
			base.Start();

#if USE_INCONTROL
			if (m_touchManager != null)
			{
				
#if UNITY_ANDROID || UNITY_IOS
				m_touchManager.enabled = m_activateTouchManagerInMobile;
				ShowHideTouchCamera(m_activateTouchCameraInMobile);
#else
				m_touchManager.enabled = m_activateTouchCameraInOtherPlatform;
				ShowHideTouchCamera(m_activateTouchManagerInOtherPlatform);
#endif
			}

				
#endif
		}

		protected override void StartAllListeners()
		{
			base.StartAllListeners();
#if USE_INCONTROL
			InputManager.OnDeviceAttached += OnDeviceAttached;
			InputManager.OnDeviceDetached += OnDeviceDetached;
			InputManager.OnUpdate += OnUpdate;
			InputManager.OnReset += OnInControlInputManagerReset;
			InputManager.OnSetup += OnInControlInputManagerSetup;
#endif
		}

		protected override void StopAllListeners()
		{
			base.StopAllListeners();

#if USE_INCONTROL
			InputManager.OnDeviceAttached -= OnDeviceAttached;
			InputManager.OnDeviceDetached -= OnDeviceDetached;
			InputManager.OnUpdate -= OnUpdate;
			InputManager.OnReset -= OnInControlInputManagerReset;
			InputManager.OnSetup -= OnInControlInputManagerSetup;
#endif
		}

		public virtual void ShowHideTouchCamera(bool _value)
		{
#if USE_INCONTROL
			var tc = TouchManagerCamera;
			if (tc != null)
			{
				tc.gameObject.SetActive(_value);
			}
#endif
		}

		public override void DestroyAction(string _key)
		{
			if (m_createdActions.Contains(_key))
			{
				OnActionDestroyed.Invoke(m_createdActions[_key]);
#if USE_INCONTROL
				VP_InputActions action = m_createdActions[_key];
	    		
				if (action is VP_InControlPlayerActions)
				{
					((VP_InControlPlayerActions)action).Destroy();
				}
#endif
				m_createdActions.Remove(_key);
			}
		}

		protected virtual void OnInControlInputManagerSetup()
		{

		}

		protected virtual void OnInControlInputManagerReset()
		{

		}

#if USE_INCONTROL
		protected virtual void OnDeviceAttached(InputDevice device)
		{
			int idx = InputManager.Devices.IndexOf(device);
			OnDeviceAttach.Invoke(idx);
		}
        
		protected virtual void OnDeviceDetached(InputDevice device)
		{
			// TODO-> Check if I can get the dettached idx
			OnDeviceAttach.Invoke(InputManager.Devices.Count);
		}   
		
		protected virtual void OnActiveDeviceChanged(InputDevice device)
		{
			OnDeviceChange.Invoke(InputManager.Devices.IndexOf(device));
		}
#endif

		protected override void OnUpdate(ulong _ulong, float _float)
		{
#if USE_INCONTROL
			if (m_useDelegates && m_mappedActions.Count > 0)
			{
				foreach (var created in m_createdActions.Keys)
				{
					var action = m_createdActions[created];
					if (action is T)
					{
						T ac = (T) action as T;
						List<IInputControl> ioL = ac.PlayerActionList;

						foreach (IInputControl io in ioL)
						{
							if (io.WasPressed)
							{
								foreach (var kvp in m_mappedActions.Keys)
								{
									VP_InControlMappedInput mapped = (VP_InControlMappedInput)kvp;
									if (io == mapped.m_inputAction)
									{
										TriggerAction(m_mappedActions[kvp], ac, kvp);
										break;
									}
								}
							}
						}
					}	
				}
			}
#endif
		}

#if USE_INCONTROL
		public InputDevice InControlInputManager
		{
			get
			{
				return InputManager.ActiveDevice;
			}
		}
#endif

		public int InControlDeviceCounts
		{
			get
			{
#if USE_INCONTROL
				return InputManager.Devices.Count;
#else
				return Input.GetJoystickNames().Length;
#endif
			}
		}


		protected override T1 CreateDataFromCopy(VP_InputKeyData _keyData)
		{
			return DefaultInputKeyValue();
		}

		public override T DefaultInputActionValue()
		{
			return default(T);
		}

		public override void CreateInputActions(string _key, out T _actions, T1 _keyData)
		{
			_actions = DefaultInputActionValue();
			_actions.SetupInputs(_keyData, m_blockInputsIfControllerNotAvailable);

			if (!m_createdActions.ContainsKey(_key))
				m_createdActions.Add(_key, _actions);
			else
				m_createdActions[_key] = _actions;
		}

		public override T GetCastedActionSet(string _key, int _idx = -1, bool _blockIfNull = false)
		{
  
			var ia = m_createdActions.ContainsKey(_key) && m_createdActions[_key] is T ? m_createdActions[_key] as T : PlayerActions as T;

			if (_idx >= 0)
			{
				ia.RemapToIndex(_idx, _blockIfNull);
			}

			return ia;
		}
	}
}
