#if USE_BEHAVIOR_DESIGNER
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using VirtualPhenix.Controllers;

namespace VirtualPhenix.Integrations.BehaviorDesignerTree
{
	public class NavMeshMovement : BehaviorDesigner.Runtime.Tasks.Movement.NavMeshMovement
	{
		[Header("VP AStar - Config"),Space]
		[BehaviorDesigner.Runtime.Tasks.Tooltip("If the node ends or continuously applies")]
		public bool m_successOnArrive = true;
		[BehaviorDesigner.Runtime.Tasks.Tooltip("If true, it returns Running-> The node is block and only calls OnDoBlock()")]
		public SharedBool m_blocked = false;
		
		[Header("VP Character Controller Movement"),Space]
		[BehaviorDesigner.Runtime.Tasks.Tooltip("VP Character Controller")]
		public SharedVPCharacterController m_characterController;

		[BehaviorDesigner.Runtime.Tasks.Tooltip("Move Character in tree")]
		protected bool m_moveCharacterByTree = false;

		[Header("Body Animations"),Space]
		[BehaviorDesigner.Runtime.Tasks.Tooltip("VP Animator that can use Mechanim, Animancer...")]
		public SharedVPAnimator m_animator;

		[BehaviorDesigner.Runtime.Tasks.Tooltip("Use Animations")]
		public SharedBool m_useAnimations = true;

		[BehaviorDesigner.Runtime.Tasks.Tooltip("More than this value, the Idle goes Walk")]
		public SharedFloat m_distanceForChangingMovementAnimation = 0.25f;
		
#if USE_ANIMANCER
		[BehaviorDesigner.Runtime.Tasks.Tooltip("Use Animancer")]
		public SharedBool m_useAnimancer = true;
#endif

		[BehaviorDesigner.Runtime.Tasks.Tooltip("Walk Animation parameter in case of Mechanim or Animation Key in Animancer")]
		public SharedVPAnimatorParameterVariable m_walkAnimation;

		[BehaviorDesigner.Runtime.Tasks.Tooltip("Run Animation parameter in case of Mechanim or Animation Key in Animancer")]
		public SharedVPAnimatorParameterVariable m_runAnimation;

		[BehaviorDesigner.Runtime.Tasks.Tooltip("Force to Idle Animation parameter in case of Mechanim or Animation Key in Animancer")]
		public SharedVPAnimatorParameterVariable m_idleAnimation;

		[Header("Run Config"),Space]
		[BehaviorDesigner.Runtime.Tasks.Tooltip("If the target is further than this, the seeker changes to RUN animation. -1 don't use it")]
		public SharedFloat m_runSpeed = 10f;

		[BehaviorDesigner.Runtime.Tasks.Tooltip("If can run or not.")]
		public SharedBool m_run = true;

		[Header("AIM IK"), Space]
		[BehaviorDesigner.Runtime.Tasks.Tooltip("Reference used by VP_LookAnimator for Head IK")]
	
		public SharedTransform m_ikReferenceGO;
		public bool m_setReferenceIKPosition = false;
		public bool m_useIK = true;
		public bool m_headIKToTarget = true;
		public bool m_quitIKFromTargetOnReach = true;
		[BehaviorDesigner.Runtime.Tasks.Tooltip("Head IK variable")]
		public SharedVPLookAnimatorVariable m_ikAnimator;


		[Header("Debug"),Space]
		public bool m_debugTarget = true;
		public Color m_debugTargetPointColor = Color.red;
		public float m_debugTargetRadius = 0.25f;


		protected bool m_canRun = true;
		protected bool m_isRunning = false;
		protected bool m_useChara;
		protected bool m_usingPathFinding = false;
		protected bool m_destroyReferenceOnComplete = false;
		
		protected bool m_started = false;
		
		public override void OnReset()
		{
			base.OnReset();
			
			SetAnimatorNames();
		}
		
		public virtual void SetAnimatorNames()
		{
			m_idleAnimation = new VP_AnimatorParameter() { AnimationName = VP_AnimationSetup.IDs.IDLE, ParameterType = VirtualPhenix.Dialog.VariableTypes.String };
			m_walkAnimation = new VP_AnimatorParameter() { AnimationName = VP_AnimationSetup.IDs.WALK, ParameterType = VirtualPhenix.Dialog.VariableTypes.Bool };
			m_runAnimation = new VP_AnimatorParameter() { AnimationName = VP_AnimationSetup.IDs.RUN, ParameterType = VirtualPhenix.Dialog.VariableTypes.Bool };
		}
		
		public override void OnStart()
		{
			base.OnStart();

			m_started = true;

			if (m_useIK && m_ikReferenceGO.Value == null)
			{
				m_setReferenceIKPosition = true;
				m_destroyReferenceOnComplete = true;

				var go = GameObject.Find("Behavior Reference") ?? GameObject.Instantiate(new GameObject("Behavior Reference"));
				m_ikReferenceGO = go.transform;
			}

			m_useChara = CharacterController != null && CharacterController.UseVPCharacterControllerToHandleAIMovement();
			m_usingPathFinding = m_useChara && CharacterController.IsUsingPathfindingForMovement();

			m_canRun = m_run.Value && m_runAnimation.Value.AnimationName.IsNotNullNorEmpty();
			m_useAnimations = m_useAnimations.Value && m_walkAnimation.Value.AnimationName.IsNotNullNorEmpty();
		}

		public override void OnBehaviorComplete()
		{
			base.OnBehaviorComplete();

			if (m_destroyReferenceOnComplete)
				GameObject.Destroy(m_ikReferenceGO.Value.gameObject);
		}

		public virtual VP_LookAnimator IKAnimator
		{
			get
			{
				return m_ikAnimator.Value;
			}
		}

		public virtual VP_CharacterController CharacterController
		{
			get
			{
				return m_characterController.Value;
			}
		}

		public override void OnDrawGizmos()
		{
			base.OnDrawGizmos();
			if (m_debugTarget && m_started && navMeshAgent != null && !navMeshAgent.isStopped)
			{
				Gizmos.color = m_debugTargetPointColor;
				Gizmos.DrawSphere(navMeshAgent.destination, m_debugTargetRadius);
			}

		}

		protected override bool SetDestination(Vector3 target)
		{
			if (m_useIK && m_setReferenceIKPosition && m_headIKToTarget && m_ikReferenceGO.Value != null)
			{
				m_ikReferenceGO.Value.position = target;
				SetIKTarget(m_ikReferenceGO.Value);
			}

			return (m_useChara) ? m_characterController.Value.GoToPoint(target, true) : base.SetDestination(target);
		}

		protected override UnityEngine.Vector3 Velocity()
		{
			return (m_useChara) ? m_characterController.Value.Velocity() : base.Velocity();
		}

		protected override void UpdateRotation(bool update)
		{
			if (m_useChara)
				m_characterController.Value.UpdateRotation(update);
			else
				base.UpdateRotation(update);
		}

		protected new bool SamplePosition(Vector3 position)
		{
			return (m_useChara) ? m_characterController.Value.SamplePosition(position) : base.SamplePosition(position);
		}

		protected override bool HasPath()
		{
			return (m_useChara) ? m_characterController.Value.HasPath() : base.HasPath();
		}

		protected override bool HasArrived()
		{
			return (m_useChara) ? m_characterController.Value.HasArrivedToTarget() : base.HasArrived();
		}

		public override void OnEnd()
		{
			if (m_useChara)
			{
				m_characterController.Value.OnEndPath(stopOnTaskEnd.Value);
			}
			else
			{
				base.OnEnd();
			}
		}

		public virtual void SetIKTarget(Transform _target)
		{
			if (IKAnimator != null)
				IKAnimator.SetTarget(_target);
		}

		public virtual void DoOnBlock()
		{

		}


		public virtual bool NeedToReturnSuccess()
		{
			return false;
		}

		public virtual bool NeedToReturnFailure()
		{
			return false;
		}

		public virtual bool IsBlocked()
		{
			return m_blocked.Value;
		}

		public virtual void UpdateBody()
		{
			
		}

		public override TaskStatus OnUpdate()
		{
			if (NeedToReturnFailure())
			{
				return TaskStatus.Failure;
			}
			
			if (NeedToReturnSuccess())
			{
				return TaskStatus.Success;
			}
			
			if (IsBlocked())
			{
				DoOnBlock();

				return TaskStatus.Running;
			}

			UpdateBody();

			if (HasArrived())
			{
				DoOnArrive();

				if (m_successOnArrive)
					return TaskStatus.Success;
			}
			else
			{
				DoOnNotArrive();
			}

			return TaskStatus.Running;
		}

		protected virtual void DoOnNotArrive()
		{
			CheckIsRunning();

			var characterController = CharacterController;
			m_useChara = characterController != null && characterController.UseVPCharacterControllerToHandleAIMovement();

			if (m_useChara)
			{
				m_moveCharacterByTree = characterController.MoveByBehaviorTree();

				if (m_moveCharacterByTree)
					characterController.UpdateCharacterMovementExternally();
			}

		}

		protected virtual void DoOnArrive()
		{
			if (m_useIK && m_quitIKFromTargetOnReach)
			{
				SetIKTarget(null);
			}

			Stop();
		}

		protected override void Stop()
		{
			PlayIdleAnimation();
			if (m_useChara)
			{
				m_characterController.Value.StopAIMovement();
			}
			else
			{
				base.Stop();
			}
		}


		protected virtual void CheckIsRunning()
		{
			if (navMeshAgent.remainingDistance > m_distanceForChangingMovementAnimation.Value)
			{
				if (m_canRun)
				{
					SetSpeed(m_isRunning ? m_runSpeed.Value : speed.Value);

					if (m_isRunning)
					{
						PlayRunningAnimation();
					}
					else
					{
						PlayWalkingAnimation();
					}
				}
				else
				{
					m_isRunning = false;
					PlayWalkingAnimation();
				}
			}
		}

		protected virtual void SetSpeed(float _speed)
		{
			if (m_useChara)
				m_characterController.Value.SetAgentSpeed();
			else
				navMeshAgent.speed = _speed;
		}

		protected virtual void PlayIdleAnimation()
		{
			m_runAnimation.Value.SetParameterValue(false);
			m_walkAnimation.Value.SetParameterValue(false);
			SetAnimatorParameters(new VP_AnimatorParameter[]{ m_idleAnimation.Value, m_walkAnimation.Value, m_runAnimation.Value });
		}

		protected virtual void PlayWalkingAnimation()
		{		
			m_runAnimation.Value.SetParameterValue(false);
			m_walkAnimation.Value.SetParameterValue(true);
			SetAnimatorParameters(new VP_AnimatorParameter[]{ m_walkAnimation.Value, m_runAnimation.Value });
		}

		protected virtual void PlayRunningAnimation()
		{
			m_runAnimation.Value.SetParameterValue(true);
			m_walkAnimation.Value.SetParameterValue(true);
			SetAnimatorParameters(new VP_AnimatorParameter[]{ m_runAnimation.Value, m_walkAnimation.Value });
		}



		protected virtual void SetAnimatorParameter(VP_AnimatorParameter _parameter)
		{
			if (m_useAnimations.Value)
			{
				if (m_animator.Value != null)
				{
					m_animator.Value.SetParameter(_parameter, m_useAnimancer.Value);
				}

			}
		}
		
		protected virtual void SetAnimatorParameters(VP_AnimatorParameter[] _parameters)
		{
			if (m_useAnimations.Value)
			{
				if (m_animator.Value != null)
				{
					m_animator.Value.SetParameters(_parameters, m_useAnimancer.Value);
				}

			}
		}
	}
}
#endif