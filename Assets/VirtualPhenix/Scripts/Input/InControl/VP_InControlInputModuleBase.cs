using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if USE_INCONTROL

using InControl;
namespace VirtualPhenix.Inputs
{
	[AddComponentMenu("")]
	public class VP_InControlInputModuleBase<T, T1> : InControl.InControlInputModule where T : VP_InControlInputModuleKeyData where T1 : VP_InControlInputModuleActions
	{
		[SerializeField] protected T m_inputData;

		protected bool m_initialized = false;
		protected T1 m_actions;
		public string m_actionSetID = "Input Module";
		public bool m_destroyActionOnDisable = false;
		public bool m_allowKeyboardInput = false;

#if UNITY_EDITOR
		protected override void Reset()
		{
			base.Reset();
			m_actionSetID = VP_InputSetup.REGULR_IN_CONTROL_MODULE_ID;
			m_inputData = DefaultData();
		}
#endif

		protected override void Awake()
		{
			base.Awake();
			m_initialized = true;
			
			CreateActions();

			if (m_allowKeyboardInput)
				MapActions();
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			
			if (m_initialized)
			{
				CreateActions();

				if (m_allowKeyboardInput)
					MapActions();
			}
		}

		public virtual void UnMapActions()
		{		
			SubmitAction = null;
			CancelAction = null;
			MoveAction = null;
		}


		public virtual void MapActions()
		{		
			SubmitAction = m_actions.Submit;
			CancelAction = m_actions.Cancel;
			MoveAction = m_actions.Move;
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			
			if (!m_allowKeyboardInput)
				UnMapActions();
			
			if (m_destroyActionOnDisable)
				DestroyActions();
		}

		protected virtual T DefaultData()
		{
			return default(T);
		}

		protected virtual T1 DefaultAction()
		{
			return default(T1);
		}

		protected virtual void CreateActions()
		{
			m_actions = VP_InputManagerBase.Instance.GetOrCreateCastedActionSetOfType<T1, T>(m_actionSetID, DefaultAction(), ref m_inputData);
		}

		protected virtual void DestroyActions()
		{
			VP_InputManagerBase.Instance.DestroyAction(m_actionSetID);
		}
	}

}
#endif