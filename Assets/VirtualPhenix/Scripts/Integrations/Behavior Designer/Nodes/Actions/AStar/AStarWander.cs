#if USE_BEHAVIOR_DESIGNER && USE_ASTAR_PROJECT_PRO
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Movement;
using BehaviorDesigner.Runtime.Tasks.Movement.AstarPathfindingProject;
using VirtualPhenix.Integrations.BehaviorDesignerTree;
using VirtualPhenix;
using VirtualPhenix.Controllers;

namespace VirtualPhenix.Integrations.BehaviorDesignerTree
{
	[TaskDescription("Go to point and Bounce using AStar Pathfinding Project Pro.")]
	[TaskCategory(VP_PrefixSetup.MAIN_PREFIX+"/Movement/AStar")]
	[TaskIcon("Assets/VirtualPhenix/Editor/Icons/Behavior Designer/{SkinColor}SeekIcon.png")]
	public class AStarWander : AStarMovement
	{		
		[Header("Wander Calculation Config")]
		[BehaviorDesigner.Runtime.Tasks.Tooltip("Wander position is calculated around this position if not null")]
		public SharedTransform m_referenceTrs;
		[BehaviorDesigner.Runtime.Tasks.Tooltip("Toggle this to use m_referenceTRS (if not null)")]
		public SharedBool m_usePosition = true;
		[BehaviorDesigner.Runtime.Tasks.Tooltip("Set this position (overriding sphere Y pos)")]	
		public SharedBool m_setYPos = true;
		public SharedFloat m_customYPos = 0f;
		
		[Header("Wander Distance"),Space]
		[BehaviorDesigner.Runtime.Tasks.Tooltip("Minimum distance ahead of the current position to look ahead for a destination")]
		public SharedFloat minWanderDistance = 20;
		[BehaviorDesigner.Runtime.Tasks.Tooltip("Maximum distance ahead of the current position to look ahead for a destination")]
		public SharedFloat maxWanderDistance = 20;
		
		[Header("Wander Rate"),Space]
		[BehaviorDesigner.Runtime.Tasks.Tooltip("The amount that the agent rotates direction")]
		public SharedFloat wanderRate = 2;
		
		[Header("Wander Pause"),Space]
		[BehaviorDesigner.Runtime.Tasks.Tooltip("The minimum length of time that the agent should pause at each destination")]
		public SharedFloat minPauseDuration = 0;
		[BehaviorDesigner.Runtime.Tasks.Tooltip("The maximum length of time that the agent should pause at each destination (zero to disable)")]
		public SharedFloat maxPauseDuration = 0;
		
		[Header("Wander Retries"),Space]
		[BehaviorDesigner.Runtime.Tasks.Tooltip("The maximum number of retries per tick (set higher if using a slow tick time)")]
		public SharedInt targetRetries = 1;

		[Header("Debugging values"),Space]
		public float waitedTime = 0f;
		public float pauseTime;
		public bool m_waiting = false;
		
		public bool m_debugWanderRadius = true;

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
			
			//Debug.Log("on not arrive");
			
			if (m_waiting) 
			{
				waitedTime+=Time.deltaTime;
				
				if (waitedTime >= pauseTime)
				{
					ContinueNoChara();
				}			
			}				
		}

		protected override void DoOnArrive()
		{
			base.DoOnArrive();
		
			//Stop();
		
			// The agent should pause at the destination only if the max pause duration is greater than 0
			if (maxPauseDuration.Value > 0) 
			{
				if (!m_waiting)
				{
					waitedTime = -0f;
					pauseTime = Random.Range(minPauseDuration.Value, maxPauseDuration.Value);
					
					if (m_useChara)
						CharacterController.SetPlayerMove(false);
					
					m_waiting = true;
//					Debug.Log("Start waiting "+pauseTime);
				}
				else
                {
					waitedTime += Time.deltaTime;

					if (waitedTime >= pauseTime)
					{
						ContinueNoChara();
					}
				}
			} 
			else
			{
				ContinueNoChara();
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

        public override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

			if (m_debugWanderRadius && m_referenceTrs.Value != null && m_usePosition.Value)
            {
				Gizmos.color = Color.blue;
				Gizmos.DrawWireSphere(m_referenceTrs.Value.position, maxWanderDistance.Value);
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