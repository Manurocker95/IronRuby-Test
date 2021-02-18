using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if USE_ANIMANCER
namespace VirtualPhenix.Controllers
{
	[DefaultExecutionOrder(VP_ExecutingOrderSetup.CHARACTER_CONTROLLER), AddComponentMenu(VP_PrefixSetup.MAIN_PREFIX + "/Controllers/Prepare Jump Player Controller With Animancer")]
	public class VP_SimpleAnimancerCharacterControllerWithPrepareJump : VP_SimpleAnimancerCharacterController
	{
		[Header("Prepare Jump")]
		[SerializeField] protected bool m_usePrepareJump = true;
		protected bool m_preparingJump = false;
		protected float m_tempVertical;
		protected Vector3 m_tempMoveDir;
		
		protected override void ApplyMovement(float h, float v)
		{
			if (!m_preparingJump || m_jumping)
			{
				base.ApplyMovement(h,v);
			}
		}
		
		
		protected override void HandleJumpToIdleAnimation()
		{
			
			if (m_usePrepareJump)
				m_preparingJump = false;
		    	
			m_landing = true;
			PlayAnimation(VP_AnimationSetup.Characters.LAND,()=>
			{			
				m_landing = false;
				PlayAnimation(VP_AnimationSetup.Characters.IDLE);
			});
		}

		protected override void HandleMovementAnimation(float _speed, bool _sprinting)
		{
			if (!m_jumping && !m_landing && !m_preparingJump)
			{
				string anim = _speed > 0 ? (_sprinting ? VP_AnimationSetup.Characters.SPRINT : (_speed > m_runningThreshold ? VP_AnimationSetup.Characters.RUN : VP_AnimationSetup.Characters.WALK)) : VP_AnimationSetup.Characters.IDLE;
				PlayAnimation(anim);
			}
		}

	    
		protected override void CheckJump(bool _grounded)
		{
			if (m_preparingJump)
				return;

			base.CheckJump(_grounded);
		}
		
		
		public override void PerformJump()
		{

			if (m_usePrepareJump)
			{
				m_tempVertical = m_verticalMovement;
				m_tempMoveDir = m_moveDir;
				m_blocked = true;
				m_preparingJump = true;
				m_jumping = true;
			
				PlayAnimation(VP_AnimationSetup.Characters.PREPARE_JUMP, ()=>
				{
					m_verticalMovement= m_tempVertical;
					m_moveDir = m_tempMoveDir;
					m_blocked = false;
					base.PerformJump();	  
				    
					m_characterController.Move(m_upVector*m_tempVertical*Time.deltaTime);
				    
				});			
			}
			else
			{
				base.PerformJump();
			}
		}

		public override bool CheckIsGrounded()
		{
			bool isGrounded = CheckIsGrounded();
			
			if (!isGrounded)
				OnFloorNotDetected();
				
			return isGrounded;
		}

		protected virtual void OnFloorNotDetected()
		{
			if (m_preparingJump)
				m_preparingJump = false;
		}

	}
}
#endif