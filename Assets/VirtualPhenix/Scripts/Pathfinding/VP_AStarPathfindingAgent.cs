using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if USE_ASTAR_PROJECT_PRO
using Pathfinding;
using VirtualPhenix.Pathfinding;

namespace VirtualPhenix.Integrations.Pathfinding
{
	[DefaultExecutionOrder(VP_ExecutingOrderSetup.PATHFINDING_AGENT),AddComponentMenu(VP_PrefixSetup.MAIN_PREFIX+ "/Pathfinding/VP Pathfinding with AStar")]
	public class VP_AStarPathfindingAgent : VP_PathfindingAgent
	{
	
		[Header("A* Pathfinding Project Pro"), Space]
		[SerializeField] protected AIPath m_astar;
		[SerializeField] protected Seeker m_seeker;		
		[SerializeField] protected bool m_use2D = false;		
		[SerializeField] protected bool m_blockOnStop = true;		
		[SerializeField] protected bool m_enableNavmeshIfMissing = true;		
	
		[SerializeField] protected bool m_useNavMeshAsFallback = true;
	
		public override float Speed { get { return m_pathfindingType == PATHFINDING_TYPE.NAVMESH ? base.Speed : m_astar.maxSpeed; } }
	
		protected override void Initialize()
		{
		
			
#if USE_ASTAR_PROJECT_PRO
			if (m_useNavMeshAsFallback && m_pathfindingType == PATHFINDING_TYPE.ASTAR && (m_astar == null || m_seeker == null || m_astar.enabled == false))
			{
				if (m_navmeshAgent != null && m_enableNavmeshIfMissing)
				{
					m_navmeshAgent.enabled = true;
				}
				
				m_pathfindingType = PATHFINDING_TYPE.NAVMESH;
			}
#endif

			base.Initialize();
		}

	
		public override void SetUpdateRotation(bool _value)
		{
			if (m_pathfindingType == PATHFINDING_TYPE.NAVMESH)
			{
				base.SetUpdateRotation(_value);
			}
		}
		
		
		public override bool HasArrivedToTarget()
		{
			if (m_pathfindingType == PATHFINDING_TYPE.NAVMESH)
			{
				return base.HasArrivedToTarget();
			}
			
			if (m_manualTarget)
				return HasArrivedToTargetPoint();
			
			var direction = transform.InverseTransformPoint(m_astar.destination);
			
			if (m_use2D) 
			{
				direction.z = 0;
			} 
			else 
			{
				direction.y = 0;
			}
		
			return direction.magnitude < m_thresholdToReach;
		}

		public override void UnlockAgent()
		{
			if (m_pathfindingType == PATHFINDING_TYPE.NAVMESH)
			{
				base.UnlockAgent();
				return;
			}
			
			m_astar.isStopped = false;
			m_astar.canSearch = true;
			m_astar.canMove = true;
		}
		
		public override bool SamplePosition(Vector3 position)
		{
			if (m_pathfindingType == PATHFINDING_TYPE.NAVMESH)
			{
				return base.SamplePosition(position);
			}
			
			
			var direction = transform.InverseTransformDirection(AstarPath.active.GetNearest(position).position - position);
			direction.y = 0;
			return direction.sqrMagnitude < m_thresholdToReach;
		}

		public override void SetupAgent()
		{
			if (m_pathfindingType == PATHFINDING_TYPE.NAVMESH)
			{
				base.SetupAgent();
				return;
			}

			if (m_astar)
			{
				m_astar.maxSpeed = m_speed;
				m_astar.slowdownDistance = m_stopDistance;
			}
		}

		public override void GoToPoint(Vector3 _point)
		{
			if (m_pathfindingType == PATHFINDING_TYPE.NAVMESH)
			{
				base.GoToPoint(_point);
				return;
			}
			UnlockAgent();
			m_manualTarget = false;
			SetDestination(_point);
			m_astar.destination = _point;
			//Debug.Log("Dest "+m_astar.destination);
		}
		
		public override Vector3 AgentDestination()
		{
			if (m_pathfindingType == PATHFINDING_TYPE.NAVMESH)
			{
				return base.AgentDestination();
			}
			
			if (m_astar)
				return m_astar.destination;
			
			return Vector3.zero;
		}
		
		public override void GoToPoint(Transform _trs)
		{
			if (m_pathfindingType == PATHFINDING_TYPE.NAVMESH)
			{
				base.GoToPoint(_trs);
				return;
			}
			
			if (_trs != null)
			{
				GoToPoint(_trs.position);
			}

		}
		
		public override void Stop()
		{
			if (m_pathfindingType == PATHFINDING_TYPE.NAVMESH)
			{
				base.Stop();
				return;
			}
			
			if (m_manualTarget)
				m_manualTarget = false;
			
			if (m_astar)
			{
				m_astar.isStopped = true;
				
				if (m_blockOnStop)
				{
					BlockAgent();
				}
			}
		}
		
		public override void EnableAgent(bool _active)
		{
			if (m_pathfindingType == PATHFINDING_TYPE.NAVMESH)
			{
				base.EnableAgent(_active);
				return;
			}
			m_seeker.enabled = _active;
			m_astar.enabled = _active;
		}
		
		public override bool HasPath()
		{
			if (m_pathfindingType == PATHFINDING_TYPE.NAVMESH)
			{
				return base.HasPath();
			}
			
			return HasPathToPoint() || (m_astar.hasPath && m_astar.remainingDistance > m_thresholdToReach);
		}

		public override void OnEndPath(bool _stop)
		{
			if (m_pathfindingType == PATHFINDING_TYPE.NAVMESH)
			{
				base.OnEndPath(_stop);
				return;
			}
			
			if (_stop) 
			{
				Stop();
			} 
		}
		
		public override void BlockAgent()
		{	
			if (m_pathfindingType == PATHFINDING_TYPE.NAVMESH)
			{
				base.BlockAgent();
				return;
			}
			
			m_astar.canSearch = false;
			m_astar.canMove = false;
		}
		
		public override bool PointReached()
		{
			if (m_pathfindingType == PATHFINDING_TYPE.NAVMESH)
			{
				return base.PointReached();
			}
			
			return m_astar.reachedDestination;
		}
		
		public override float DistanceToTarget()
		{
			if (m_pathfindingType == PATHFINDING_TYPE.NAVMESH)
			{
				return base.DistanceToTarget();
			}
			
			return m_astar.remainingDistance;
		}
		
		public override void SetSpeed(float _speed, bool _saveIt = true)
		{
			if (m_pathfindingType == PATHFINDING_TYPE.NAVMESH)
			{
				base.SetSpeed(_speed, _saveIt);
				return;
			}
			
			if (_saveIt)
				m_speed = _speed;
				
			m_astar.maxSpeed = _speed;
		}
		
		public override void SetAngularSpeed(float _speed, bool _saveIt = true)
		{
			if (m_pathfindingType == PATHFINDING_TYPE.NAVMESH)
			{
				base.SetAngularSpeed(_speed, _saveIt);
				return;
			}
		}
		
		public override void SetStoppingDistance(float _distance, bool _saveIt = true)
		{
			if (m_pathfindingType == PATHFINDING_TYPE.NAVMESH)
			{
				base.SetStoppingDistance(_distance, _saveIt);
				return;
			}		
			
			if (_saveIt)
				m_stopDistance = _distance;
				
			
			if (m_astar)
				m_astar.slowdownDistance = _distance;
		}
		
		public override bool IsStopped
		{
			get
			{	if (m_pathfindingType == PATHFINDING_TYPE.NAVMESH)
					return base.IsStopped;
				else
					return  m_astar ? m_astar.isStopped : true;
			}
		}
	}

}

#endif