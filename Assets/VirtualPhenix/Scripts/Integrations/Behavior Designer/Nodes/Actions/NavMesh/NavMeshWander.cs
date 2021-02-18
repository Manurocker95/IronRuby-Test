using UnityEngine;

#if USE_BEHAVIOR_DESIGNER
using BehaviorDesigner.Runtime.Tasks.Movement;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;
using VirtualPhenix.Controllers;

namespace VirtualPhenix.Integrations.BehaviorDesignerTree
{
	[TaskDescription("Wander using the Unity NavMesh.")]
	[TaskCategory(VP_PrefixSetup.MAIN_PREFIX+"/Movement/NavMesh")]
	[BehaviorDesigner.Runtime.Tasks.HelpURL("https://www.opsive.com/support/documentation/behavior-designer-movement-pack/")]
	[TaskIcon("Assets/VirtualPhenix/Editor/Icons/Behavior Designer/{SkinColor}WanderIcon.png")]
	public class NavMeshWander : NavMeshMovement
	{
		[Header("Wanter Properties"),Space]
		public SharedTransform m_referenceTrs;
		public SharedBool m_usePosition;
		public SharedBool m_setYPos = true;
		public SharedBool m_recalculate = false;
		public SharedFloat m_customYPos = 0f;
		

		[BehaviorDesigner.Runtime.Tasks.Tooltip("Minimum distance ahead of the current position to look ahead for a destination")]
		public SharedFloat minWanderDistance = 20;
		[BehaviorDesigner.Runtime.Tasks.Tooltip("Maximum distance ahead of the current position to look ahead for a destination")]
		public SharedFloat maxWanderDistance = 20;
		
		[BehaviorDesigner.Runtime.Tasks.Tooltip("The amount that the agent rotates direction")]
		public SharedFloat wanderRate = 2;
		[BehaviorDesigner.Runtime.Tasks.Tooltip("The minimum length of time that the agent should pause at each destination")]
		public SharedFloat minPauseDuration = 0;
		[BehaviorDesigner.Runtime.Tasks.Tooltip("The maximum length of time that the agent should pause at each destination (zero to disable)")]
		public SharedFloat maxPauseDuration = 0;
		[BehaviorDesigner.Runtime.Tasks.Tooltip("The maximum number of retries per tick (set higher if using a slow tick time)")]
		public SharedInt targetRetries = 1;

		[Space]
		public float waitedTime = 0f;
		public float pauseTime;
		public bool m_waiting = false;
		
		public override void OnStart()
		{
			base.OnStart();
			if (TrySetTarget())
			{
				waitedTime = -0f;
			}
		}

		protected override void DoOnNotArrive()
		{
			base.DoOnNotArrive();
			
			if (m_recalculate.Value)
			{
				m_recalculate = false;
				ContinueNoChara();
			}
			
			if (m_waiting) 
			{
				waitedTime+=Time.deltaTime;
				
				if (waitedTime >= pauseTime)
				{
					m_waiting = false;
					ContinueNoChara();
				}			
			}				
		}

        protected override bool HasArrived()
        {
			if (m_waiting)
				return false;

            return base.HasArrived();
        }

        protected override void CheckIsRunning()
        {
			if (m_waiting)
            {
				PlayIdleAnimation();
				return;
            }

            base.CheckIsRunning();
        }

        protected override void DoOnArrive()
		{
			base.DoOnArrive();
		
			if (!m_waiting)
            {
				// The agent should pause at the destination only if the max pause duration is greater than 0
				if (maxPauseDuration.Value > 0)
				{
					waitedTime = 0f;
					pauseTime = Random.Range(minPauseDuration.Value, maxPauseDuration.Value);

					if (m_useChara)
						CharacterController.SetPlayerMove(false);
					
					m_waiting = true;

				}
				else
				{
					ContinueNoChara();
				}
			}
		}

		void ContinueNoChara()
		{
			waitedTime = -0f;
			m_waiting = false;
			TrySetTarget();
			
			if (m_useChara)
				CharacterController.SetPlayerMove(true);
		}

		void Continue(VirtualPhenix.Controllers.VP_CharacterController _controller)
		{
			if (!m_useChara)
			{
				ContinueNoChara();
				return;
			}	
				
			if (_controller == m_characterController.Value)
			{
				ContinueNoChara();
			}			
		}

		protected virtual bool TrySetTarget()
		{
			var direction = transform.forward;
			var validDestination = false;
			var attempts = targetRetries.Value;
			var destination = transform.position;
			
			while (!validDestination && attempts > 0) 
			{
				if (m_referenceTrs.Value == null || !m_usePosition.Value)
				{
					direction = direction + Random.insideUnitSphere * wanderRate.Value;
					destination = transform.position + direction.normalized * Random.Range(minWanderDistance.Value, maxWanderDistance.Value);				
				}
				else
				{
					destination = VP_Utils.Math.RandomPointAroundPosition(m_referenceTrs.Value.position, Random.Range(minWanderDistance.Value, maxWanderDistance.Value));
				}
				if (m_setYPos.Value)
				{
					destination.y = m_customYPos.Value;
				}
				
				validDestination = SamplePosition(destination);
				attempts--;
			}
			if (validDestination) 
			{
				SetDestination(destination);
			}
			
			return validDestination;
		}

		// Reset the public variables
		public override void OnReset()
		{
			base.OnReset();
			minWanderDistance = 20;
			maxWanderDistance = 20;
			wanderRate = 2;
			minPauseDuration = 0;
			maxPauseDuration = 0;
			targetRetries = 1;
			waitedTime = 0;
		}
	}
}
#endif