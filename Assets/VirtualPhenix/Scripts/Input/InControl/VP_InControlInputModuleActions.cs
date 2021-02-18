using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if USE_INCONTROL
using InControl;
#endif

namespace VirtualPhenix.Inputs
{
	[System.Serializable]
	public class VP_InControlInputModuleActions : VP_PlayerActions
	{
#if USE_INCONTROL
		public readonly PlayerAction Submit;
		public readonly PlayerAction Cancel;
		
		public readonly PlayerAction Left;
		public readonly PlayerAction Right;
		public readonly PlayerAction Up;
		public readonly PlayerAction Down;
		public readonly PlayerTwoAxisAction Move;

#if ODIN_INSPECTOR
		[Sirenix.Serialization.OdinSerialize]
#endif
		public List<IInputControl> PlayerActionList { get; set; }
#endif



		public VP_InControlInputModuleActions()
		{
#if USE_INCONTROL
			PlayerActionList = new List<IInputControl>();

			Submit = CreatePlayerAction(VP_InputSetup.InControlPlayerActionIds.SUBMIT_ID);
			Cancel = CreatePlayerAction(VP_InputSetup.InControlPlayerActionIds.CANCEL_ID);
			
			Right = CreatePlayerAction(VP_InputSetup.InControlPlayerActionIds.MOVE_RIGHT_ID);
			Left = CreatePlayerAction(VP_InputSetup.InControlPlayerActionIds.MOVE_LEFT_ID);
			Up = CreatePlayerAction(VP_InputSetup.InControlPlayerActionIds.MOVE_UP_ID);
			Down = CreatePlayerAction(VP_InputSetup.InControlPlayerActionIds.MOVE_DOWN_ID);
			Move = CreateTwoAxisPlayerAction(Left, Right, Down, Up);

			AddPlayerActionsToList();

			CreateExtraInputs();
#endif
		}
		
		public virtual void CreateExtraInputs()
		{

		} 
        
                
		public virtual void AddExtraPlayerActionsToList()
		{

		}

        
		public virtual void AddPlayerActionsToList()
		{
#if USE_INCONTROL
			if (PlayerActionList == null)
				PlayerActionList = new List<IInputControl>();

			PlayerActionList.Add(Submit);
			PlayerActionList.Add(Cancel);
			PlayerActionList.Add(Move);
#endif
		} 
		
		
		public override void SetupInputs()
		{
			base.SetupInputs();
			// RemapToIndex(m_playerIndex, true);
		}
		
		public override void RemapToIndex(int _idx, bool _blockIfNull = false)
		{
			m_playerIndex = _idx;
			CheckBlock(_blockIfNull);
		}
		

		public override void SetupInputs(VP_InputKeyData _keysToCopy, bool _blockIfNull = false)
		{
			base.SetupInputs(_keysToCopy, _blockIfNull);

			var m_defaultData = _keysToCopy as VP_InControlInputModuleKeyData;
#if USE_INCONTROL
			BindInputs
			(
				m_defaultData.SubmitKeys, m_defaultData.SubmitInControl,
				m_defaultData.CancelKeys, m_defaultData.CancelInControl,
				m_defaultData.RightKeys, m_defaultData.RightInControl,
				m_defaultData.LeftKeys, m_defaultData.LeftInControl,
				m_defaultData.UpKeys, m_defaultData.UpInControl,
				m_defaultData.DownKeys, m_defaultData.DownInControl
			);
#endif
		}
		
		
#if USE_INCONTROL

		public virtual void BindInputs
		(  
			Key[] SubmitKeys, InputControlType[]SubmitInControl ,
			Key[] CancelKeys, InputControlType[] CancelInControl,
			Key[] RightKeys, InputControlType[] RightInControl,
			Key[] LeftKeys, InputControlType[] LeftInControl,
			Key[] UpKeys, InputControlType[] UpInControl,
			Key[] DownKeys, InputControlType[] DownInControl,           
			bool IncludeUnknownControllers = true, 
			uint MaxAllowedBindings = 4,
			bool UnsetDuplicateBindingsOnSet = true
		)
		{
			foreach(Key k in SubmitKeys)
				Submit.AddDefaultBinding(k);
			foreach (InputControlType type in SubmitInControl)
				Submit.AddDefaultBinding(type);


			foreach(Key k in CancelKeys)
				Cancel.AddDefaultBinding(k);
			foreach (InputControlType type in CancelInControl)
				Cancel.AddDefaultBinding(type);

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

	}

}