using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix.AI;
using VirtualPhenix.Actions;
using VirtualPhenix.Pathfinding;
using VirtualPhenix.Controllers.Components;

namespace VirtualPhenix.Controllers
{
	[DefaultExecutionOrder(VP_ExecutingOrderSetup.CHARACTER_CONTROLLER),AddComponentMenu("")]
	public class VP_NonPlayableCharacterController : VP_CharacterController
	{
		[Header("The movement flow from BT goes through here?"), Space]
		[SerializeField] protected bool m_useCharacterControllerInTree = true;

		[Header("VPPathfinding can handle all agent stuff"), Space]
		[SerializeField] protected bool m_usePathFindingMovement = false;

		[Header("If Behavior Tree is Calling Movement Update or not"), Space]
		[SerializeField] protected bool m_callMovementInBehaviorTree = true;

		[Header("NPCs can update their movement in Update or be called elsewhere"), Space]
		/// <summary>
		/// Movement is updated in Update. If False, the movement should be updated elsewhere
		/// </summary>
		[SerializeField] protected bool m_moveInUpdate = false;

		[Header("Behavior Tree"), Space]
		[SerializeField] protected VP_BehaviorTree m_behaviorTree;
        
		[Header("Pathfinding"), Space]
		[SerializeField] protected VP_PathfindingAgent m_pathfindingAgent;

		protected bool m_forceJump = false;
		protected bool m_forceButt = false;
		protected float m_forcedHeight = 2.5f;
		protected float m_jumpStartPosY = 0f;
		protected UnityEngine.Events.UnityAction<VP_CharacterController> m_OnJumpPerformedCallback = null;
		protected UnityEngine.Events.UnityAction<VP_CharacterController> m_OnFlyPerformedCallback = null;
		protected UnityEngine.Events.UnityAction<VP_CharacterController> m_OnbuttCallback = null;
		protected UnityEngine.Events.UnityAction<VP_CharacterController> m_OnRegularLandCallback = null;


		public virtual bool IsMoveInUpdate
		{
			get
			{
				return m_moveInUpdate;
			}
		}

		protected override void Initialize()
        {
            base.Initialize();

            if (!m_behaviorTree)
            {
	            transform.TryGetComponentInChildren<VP_BehaviorTree>(out m_behaviorTree);
            } 
        }

		public override bool MoveByBehaviorTree()
		{
			return m_callMovementInBehaviorTree;
		}

		public override bool UseVPCharacterControllerToHandleAIMovement()
		{
			return m_useCharacterControllerInTree;
		}

		public override void CheckMainComponents(VP_CharacterComponent _component)
		{
			base.CheckMainComponents(_component);
			if (_component is VP_BehaviorTree && m_behaviorTree == null)
			{
				m_behaviorTree = (VP_BehaviorTree)_component;
				return;
			}

			if (_component is VP_PathfindingAgent && m_pathfindingAgent == null)
			{
				m_pathfindingAgent = (VP_PathfindingAgent)_component;
				return;
			}
		}

		public override void ForceJumpAndButtAttack(float _maxHeight, UnityEngine.Events.UnityAction<VP_CharacterController> _OnJumpPerformedCallback = null, UnityEngine.Events.UnityAction<VP_CharacterController> _OnFlyPerformedCallback = null, UnityEngine.Events.UnityAction<VP_CharacterController> _buttCallback = null, UnityEngine.Events.UnityAction<VP_CharacterController> _regularLandCallback = null)
		{
			m_forceJump = true;
			m_jumpStartPosY = m_mainObject.position.y;
			m_forcedHeight = _maxHeight;
			
			//Debug.Log("Will jump at height "+m_forcedHeight);
			
			if (_OnJumpPerformedCallback != null)
			{
				if (m_OnJumpPerformedCallback == null)
					m_OnJumpPerformedCallback = _OnJumpPerformedCallback;
				else
					m_OnJumpPerformedCallback += _OnJumpPerformedCallback;
			}

			if (_OnFlyPerformedCallback != null)
			{
				if (m_OnFlyPerformedCallback == null)
					m_OnFlyPerformedCallback = _OnFlyPerformedCallback;
				else
					m_OnFlyPerformedCallback += _OnFlyPerformedCallback;
			}

			if (_buttCallback != null)
			{
				if (m_OnbuttCallback == null)
					m_OnbuttCallback = _buttCallback;
				else
					m_OnbuttCallback += _buttCallback;
			}

			if (_regularLandCallback != null)
			{
				if (m_OnRegularLandCallback == null)
					m_OnRegularLandCallback = _regularLandCallback;
				else
					m_OnRegularLandCallback += _regularLandCallback;
			}

		}
		
		public override void PerformJump()
		{
			base.PerformJump();
			
			m_forceJump = false;
		}
		
		protected override void OnDrawGizmos()
		{
			base.OnDrawGizmos();
			
			if (m_pathfindingAgent)
			{
				if (m_pathfindingAgent.HasPathToPoint())
				{
					Gizmos.color = Color.yellow;
					Gizmos.DrawRay(m_pathfindingAgent.MainObject.position, m_pathfindingAgent.TargetPoint);
				
					Gizmos.color = Color.yellow;
					Gizmos.DrawSphere(m_pathfindingAgent.TargetPoint, 0.2f);
				}

				
				Gizmos.color = Color.red;
				Gizmos.DrawSphere(m_pathfindingAgent.AgentDestination(), 0.2f);

			}
		}
		
		public override bool GoToPoint(Vector3 _point, bool _unlock)
		{
			if (m_pathfindingAgent)
			{
				if (_unlock)
					m_pathfindingAgent.UnlockAgent();
			
				m_pathfindingAgent.GoToPoint(_point);
			
			}

			return true;
		}
		
		protected virtual Vector3 CalculateNPCDirection()
		{
			return Vector3.zero;
		}
		
		protected virtual void SetPathfindingTarget(Vector3 point, bool _unlock)
		{
			if (m_pathfindingAgent)
			{
				if (_unlock)
					m_pathfindingAgent.UnlockAgent();
			
				m_pathfindingAgent.GoToPoint(point);
			}

		}
		
		protected override void PerformJumpFlyingAction()
		{
			base.PerformJumpFlyingAction();
			
			m_forceJump = false;
		}
		
		protected override bool CheckJumpToIdleConditions(bool _grounded)
		{
			return _grounded && !m_forceJump && !m_forceButt;
		}
		
		protected override bool JumpWasPressed()
		{
			return m_forceJump;
		}
		
		protected override bool ButtSmashWasPressed()
		{
			return m_forceButt;
		}
		
	    protected override void Reset()
	    {
	    	base.Reset();
		    m_startListeningTime = StartListenTime.OnEnable;
		    m_stopListeningTime = StopListenTime.OnDisable;
		    m_behaviorTree = GetComponentInChildren<VP_BehaviorTree>();
		    m_pathfindingAgent = GetComponentInChildren<VP_PathfindingAgent>();
			m_animator = GetComponentInChildren<VP_Animator>();
	    }

		public override void OnRegularLandPerformed(VP_CharacterController _performer)
		{
			if (m_OnRegularLandCallback != null)
			{
				Debug.Log("Triggering On Regular Land");
				m_OnRegularLandCallback.Invoke(this);
				m_OnRegularLandCallback = null;
				m_OnbuttCallback = null;
			}
		}

		protected virtual bool IsPositionInForceHeight()
		{
			return Mathf.Abs(m_mainObject.position.y - m_jumpStartPosY) >= m_forcedHeight;
		}

		protected virtual bool CheckForceButtConditions()
		{
			return (m_canButtAttack && !m_buttAttack && m_jumping && !m_landing && !m_forceButt);
		}
	    
		public override void OnJumpActionPerformed(VP_CharacterController _performer)
		{
			m_forceJump = false;
			
			if (m_OnJumpPerformedCallback != null)
			{
				m_OnJumpPerformedCallback.Invoke(_performer);
				m_OnJumpPerformedCallback = null;
			}
		}

		public override void OnFlyActionPerformed(VP_CharacterController _performer)
		{
			m_forceButt = false;
			
			if (m_OnFlyPerformedCallback != null)
			{
				m_OnFlyPerformedCallback.Invoke(_performer);
				m_OnFlyPerformedCallback = null;
			}
		}
		
		public override void OnButtSmashPerformed(VP_CharacterController _performer)
		{
			Debug.Log("Butt Smash Performed!");
			
			if (m_OnbuttCallback != null)
			{
				m_OnbuttCallback.Invoke(_performer);
				m_OnbuttCallback = null;
				m_OnRegularLandCallback = null;
			}
		}
		
		public override bool SamplePosition(Vector3 position)
		{
			if (m_pathfindingAgent)
			{
				return m_pathfindingAgent.SamplePosition(position);
			}
			
			return true;
		}
	    
		public override void SetAgentSpeed()
		{
			if (m_pathfindingAgent)
			{
				m_pathfindingAgent.SetSpeed(m_isRunning ? m_runningSpeed : m_movementSpeed);
			}
		}
	    
		public override bool HasPath()
		{
			if (m_pathfindingAgent)
			{
				return m_pathfindingAgent.HasPath();
			}
			
			return false;
		}
		
		public override void UpdateRotation(bool update)
		{
			base.UpdateRotation(update);

			if (m_pathfindingAgent)
			{
				m_pathfindingAgent.SetUpdateRotation(update);
			}
		}
		
		public override bool HasArrivedToTarget()
		{
			if (m_pathfindingAgent)
			{
				return m_pathfindingAgent.HasArrivedToTarget();
			}
			
			return base.HasArrivedToTarget();
		}
		
		public override void OnEndPath(bool _stop)
		{
			if (m_pathfindingAgent)
			{
				m_pathfindingAgent.OnEndPath(_stop);
			}
		}
		
		public override void StopAIMovement()
		{
			if (m_pathfindingAgent)
			{
				m_pathfindingAgent.Stop();
			}		
		}

    }
}