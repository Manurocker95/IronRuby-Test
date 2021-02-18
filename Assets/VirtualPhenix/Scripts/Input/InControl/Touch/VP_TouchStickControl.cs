#if USE_INCONTROL
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

namespace VirtualPhenix.Inputs
{
	public class VP_TouchStickControl : TouchStickControl
	{
		[Header("Map to Buttons on limits")]
		[SerializeField] protected bool m_isAnalgoStick = true;
		[SerializeField] protected bool m_submitButtons = true;
		[SerializeField] protected ButtonTarget m_upTarget = ButtonTarget.DPadUp;
		[SerializeField] protected bool m_upTargetState = false;
		[SerializeField] protected ButtonTarget m_downTarget = ButtonTarget.DPadDown;
		[SerializeField] protected bool m_downTargetState = false;
		[SerializeField] protected ButtonTarget m_rightTarget = ButtonTarget.DPadRight;
		[SerializeField] protected bool m_rightTargetState = false;
		[SerializeField] protected ButtonTarget m_leftTarget = ButtonTarget.DPadLeft;
		[SerializeField] protected bool m_leftTargetState = false;
		
		protected Vector2 m_stickValue;
		
		
		
		public override void TouchMoved( InControl.Touch touch )
		{

			base.TouchMoved(touch);
			
			//	m_stickValue.x = value.x;
			//m_stickValue.y = value.y;
			
			
		}

		public override void SubmitControlState( ulong updateTick, float deltaTime )
		{
			if (m_isAnalgoStick)
				base.SubmitControlState(updateTick, deltaTime);
			
			if (m_submitButtons)
			{
				// Change Value in TouchStickControl to protected
				if (m_stickValue.x == -1)
				{
					SubmitButtonState( m_leftTarget, true, updateTick, deltaTime );
				}
				else
				{
					SubmitButtonState( m_leftTarget, false, updateTick, deltaTime );
				}
				
				if (m_stickValue.x == 1)
				{
					SubmitButtonState( m_rightTarget, true, updateTick, deltaTime );
				}
				else
				{
					SubmitButtonState( m_rightTarget, false, updateTick, deltaTime );
				}
				
				if (m_stickValue.y == -1)
				{
					SubmitButtonState( m_downTarget, true, updateTick, deltaTime );
				}
				else
				{
					SubmitButtonState( m_downTarget, false, updateTick, deltaTime );
				}
				
				if (m_stickValue.y == 1)
				{
					SubmitButtonState( m_upTarget, true, updateTick, deltaTime );
				}
				else
				{
					SubmitButtonState( m_upTarget, false, updateTick, deltaTime );
				}
			}
			
		}

	}
}
#endif