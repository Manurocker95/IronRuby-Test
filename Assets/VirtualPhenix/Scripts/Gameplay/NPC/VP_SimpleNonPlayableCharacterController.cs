using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace VirtualPhenix.Controllers
{
	[DefaultExecutionOrder(VP_ExecutingOrderSetup.NON_PLAYABLE_CONTROLLER), AddComponentMenu(VP_PrefixSetup.MAIN_PREFIX + "/Controllers/NPC Controller With Character Controller")]
	public class VP_SimpleNonPlayableCharacterController : VP_NonPlayableCharacterController
	{
		[Header("Character Controller Movement")]
		[SerializeField] protected CharacterController m_characterController;
		[SerializeField] protected bool m_useCharacterController = true;

		protected override void Reset()
		{
			base.Reset();

			transform.TryGetComponentInParent(out m_characterController);
			
		}
		
		protected override void Initialize()
		{
			base.Initialize();
			
			if (!m_characterController)
				transform.TryGetComponentInParent(out m_characterController);

			CheckCharacterControllerComponent();
		}

		protected virtual void CheckCharacterControllerComponent()
		{
			if (m_useCharacterController && !m_characterController)
			{
				transform.TryGetComponentInParent<CharacterController>(out m_characterController);
			}
			else if (!m_useCharacterController)
			{
				if (m_characterController != null)
				{
					m_characterController.enabled = false;
				}
			}
		}
		
		public override void ForceJumpAndButtAttack(float _maxHeight, UnityAction<VP_CharacterController> _OnJumpPerformedCallback = null, UnityAction<VP_CharacterController> _OnFlyPerformedCallback = null, UnityAction<VP_CharacterController> _buttCallback = null, UnityAction<VP_CharacterController> _regularLandCallback = null)
		{
			Debug.Log("Force jump");
			if (m_usePathFindingMovement)
			{
				m_moveDir = Vector3.zero;
				HandleMovementAnimation(0, false);
				m_characterController.enabled = true;
				m_pathfindingAgent.EnableAgent(false);
			}
			
			base.ForceJumpAndButtAttack(_maxHeight, _OnJumpPerformedCallback, _OnFlyPerformedCallback, _buttCallback, _regularLandCallback);
		}
		
		
		protected override Vector3 CalculateCameraBasedMoveDirection(float h, float v)
		{
			Vector3 camForward_Dir = Vector3.Scale(m_camera.transform.forward, new Vector3(1, 0, 1)).normalized;
			Vector3 move = m_invertXZ ? (v * camForward_Dir + h * m_camera.transform.right) : (h * camForward_Dir + v * m_camera.transform.right);

			if (move.magnitude > 1f)
				move.Normalize();

			// Calculate the rotation for the player
			move = m_mainObject.InverseTransformDirection(move);

			// Get Euler angles
			float turnAmount = (!m_landing || (m_landing && m_canRotateOnLand)) ? Mathf.Atan2(move.x, move.z) : 0f;

			m_mainObject.Rotate(0, turnAmount * m_rotationSpeed * Time.deltaTime, 0);

			move = m_mainObject.forward * move.magnitude;
			return move;
		}

		protected override Vector3 CalculateRegularMoveDirection(float h, float v)
		{
			Vector3 move;

			move = new Vector3(h, 0f, v);
			move = m_mainObject.TransformDirection(m_moveDir);

			// Get Euler angles
			m_mainObject.Rotate(0, h * m_rotationSpeed * Time.deltaTime, 0);

			return move;
		}

		public override bool CheckIsRunning(float _speed, bool _setIsRunning = true)
		{
			if (_setIsRunning)
			{
				m_isRunning = m_pathfindingAgent.DistanceToTargetPoint() >= m_runningThreshold;
				return m_isRunning;
			}
			else
			{
				return m_pathfindingAgent.DistanceToTargetPoint() >= m_runningThreshold;
			}
		}
		
		protected override Vector3 CalculateNPCDirection()
		{	
			if (m_blocked)
				return Vector3.zero;
			
			Vector3 dir = GetNormalizedDirVector(m_pathfindingAgent.TargetPoint);
						
			Quaternion targetRot = Quaternion.LookRotation(dir);
			m_mainObject.rotation = Quaternion.Slerp(m_mainObject.rotation, targetRot, Time.deltaTime * m_rotationSpeed);
 
			float _speed = m_pathfindingAgent.Speed;// (m_isRunning ? 1 : m_runningThreshold - 0.1f, m_sprinting);
			m_isRunning = CheckIsRunning(_speed);
			HandleMovementAnimation(_speed, m_sprinting);
			m_currentSpeed = CalculateCurrentSpeed();
			return m_currentSpeed * dir;						
		}
		
		public override void UpdateCharacterMovementExternally()
		{
			bool grounded = CheckIsGrounded();
			
			if (m_useCharacterController && m_characterController != null && m_characterController.enabled)
			{
				if (grounded || (m_jumping && m_canMoveOnJump))
				{
					if (m_canCharacterMove && (m_pathfindingAgent && m_pathfindingAgent.HasPath()))
					{
						m_moveDir = CalculateNPCDirection();
					}
				}

				
				m_verticalMovement -= m_gravity * Time.deltaTime;

				//Check jump AFTER gravity, because check jump clamps the vertical speed at 0 when grounded.
				CheckJump(grounded);
				
				if (m_characterController.enabled)
				{
					m_characterController.Move(m_moveDir * Time.deltaTime);
					m_characterController.Move(m_upVector * m_verticalMovement * Time.deltaTime);
				}
				
				if (!CheckIsGrounded() && CheckForceButtConditions())
				{
					m_forceButt = true;
				}
			}	
			else
			{
				if (m_usePathFindingMovement && m_pathfindingAgent)
				{					
					m_isRunning = m_pathfindingAgent.DistanceToTarget() > m_runningThreshold;
					m_currentSpeed = ((m_landing && !m_canMoveOnLand) || !m_canCharacterMove) ? 0f : (m_sprinting && m_canSprint ? m_runningSpeed : m_movementSpeed);           
					
					HandleMovementAnimation(m_currentSpeed > 0f ? (m_isRunning ? 1 : m_runningThreshold - 0.1f) : 0f, m_sprinting);
				}
			}
		}
		
		protected override bool CheckForceButtConditions()
		{
			return base.CheckForceButtConditions() && IsPositionInForceHeight();
		}
		
		public override void OnButtSmashPerformed(VP_CharacterController _performer)
		{
			base.OnButtSmashPerformed(_performer);
			
			if (m_usePathFindingMovement)
			{
				m_characterController.enabled = false;
				m_pathfindingAgent.EnableAgent(true);
			}
		}
		
		public override void OnRegularLandPerformed(VP_CharacterController _performer)
		{
			base.OnRegularLandPerformed(_performer);
			
			 if (m_usePathFindingMovement)
			 {
				 m_characterController.enabled = false;
				 m_pathfindingAgent.EnableAgent(true);
			 }
		}
		
		protected override void ApplyMovement(float h, float v)
		{
			if (m_moveInUpdate)
			{
				UpdateCharacterMovementExternally();
			}			
		}
		
		public override void PerformJump()
		{
			base.PerformJump();
			//Debug.Log("Jump with MoveDir Y: "+  m_moveDir.y );
		}
		
		public virtual Vector3 GetMoveDirVector(Vector3 _point)
		{
			Vector3 dir = _point-m_mainObject.position;
			Vector3 normalizedDir = dir.normalized;
			
			return ((Mathf.Pow(m_movementSpeed, 2) * Time.deltaTime)/dir.magnitude)*normalizedDir;
		}
		
		public override bool IsUsingPathfindingForMovement()
		{
			return m_useCharacterController && m_usePathFindingMovement;	
		}
		
		public virtual Vector3 GetNormalizedDirVector(Vector3 _point)
		{
			Vector3 dir = _point-m_mainObject.position;
			dir.y = m_floorYPos;
			
			if (dir.magnitude > 1)
			{
				dir.Normalize();
			}
			
			return dir;
		}
		
		public override bool GoToPoint(Vector3 _point, bool _unlock)
		{
			if (m_useCharacterControllerInTree && m_pathfindingAgent)
			{
				if (!m_useCharacterController || m_usePathFindingMovement)
				{
					if (m_characterController)
						m_characterController.enabled = false;
						
					return base.GoToPoint(_point, _unlock);
				}
				
				m_pathfindingAgent.SetCharacterControllerDestination(_point, false);
			}

			return true;
		}
	}
}