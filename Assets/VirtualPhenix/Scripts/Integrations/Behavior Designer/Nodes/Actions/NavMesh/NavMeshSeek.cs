using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if USE_BEHAVIOR_DESIGNER
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Movement;

namespace VirtualPhenix.Integrations.BehaviorDesignerTree
{
	[TaskDescription("Seek the target specified using Navmesh.")]
	[TaskCategory(VP_PrefixSetup.MAIN_PREFIX+"/Movement/NavMesh")]
	[TaskIcon("Assets/VirtualPhenix/Editor/Icons/Behavior Designer/{SkinColor}SeekIcon.png")]
	public class NavMeshSeek : NavMeshMovement
	{
		[BehaviorDesigner.Runtime.Tasks.Tooltip("Object that seeks. Null = this.transform")]
		public SharedTransform m_mainObject;
		
		[BehaviorDesigner.Runtime.Tasks.Tooltip("The GameObject that the agent is moving towards")]
		public SharedGameObject m_target;
		
		[BehaviorDesigner.Runtime.Tasks.Tooltip("If target is null then use the target position")]
		public SharedVector3 m_targetPosition;
		
		[BehaviorDesigner.Runtime.Tasks.Tooltip("If the target is further than this, the seeker gives up. <=0 = never gives up.")]
		public SharedFloat m_maxDistanceToSeek = 50f;
		

		[BehaviorDesigner.Runtime.Tasks.Tooltip("If the target is further than this, the seeker changes to RUN animation. -1 don't use it")]
		public SharedFloat m_distanceToRun = 15f;
		
		[BehaviorDesigner.Runtime.Tasks.Tooltip("Variable used as toggle for stopping the process. Always returns Failure.")]
		public SharedBool m_stopSeeking = false;		
		
	
		
		[BehaviorDesigner.Runtime.Tasks.Tooltip("VP Event triggered on Reach")]
		public SharedString m_onReachedEvent = "ON_SEEK_REACH";
	
		protected bool m_canGiveUp = true;

		
		public override void OnStart()
		{
			base.OnStart();

			m_canRun = m_canRun && m_distanceToRun.Value >= 0;	
			m_canGiveUp = m_maxDistanceToSeek.Value > 0;
	
			if (m_mainObject.Value == null)
				m_mainObject = transform;

			SetDestination(Target());
		}
		

		// Seek the destination. Return success once the agent has reached the destination.
		// Return running if the agent hasn't reached the destination yet
		public override TaskStatus OnUpdate()
		{
			if (m_blocked.Value)
			{
				if (m_stopSeeking.Value)
				{
					return TaskStatus.Failure;
				}
				
				return TaskStatus.Running;
			}
			
			float distance = CheckDistanceToTarget();
			
			if (m_canGiveUp)
			{				
				if (m_stopSeeking.Value || distance >= m_maxDistanceToSeek.Value)
				{
					return TaskStatus.Failure;
				}
			}
		
			if (HasArrived()) 
			{
				DoOnArrive();
				return TaskStatus.Success;
			}
			else
			{		
				m_isRunning = distance >= m_distanceToRun.Value;
				CheckIsRunning();
			}


			SetDestination(Target());

			return TaskStatus.Running;
		}
        
		protected override void DoOnArrive()
		{
			base.DoOnArrive();
			VP_EventManager.TriggerEvent(m_onReachedEvent.Value, transform);
		}
		
        
		protected virtual float CheckDistanceToTarget()
		{
			Vector3 compVec = m_target.Value != null ? m_target.Value.transform.position : m_targetPosition.Value;
			return Vector3.Distance(m_mainObject.Value.position, compVec);
		}
        
		// Return targetPosition if target is null
		protected virtual Vector3 Target()
		{
			if (m_target.Value != null) {
				return m_target.Value.transform.position;
			}
			return m_targetPosition.Value;
		}

		public override void OnReset()
		{
			base.OnReset();
			m_target = null;
			m_targetPosition = Vector3.zero;
		}
	}
}
#endif