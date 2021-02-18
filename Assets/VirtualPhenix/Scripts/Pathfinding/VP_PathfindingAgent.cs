using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using VirtualPhenix.Controllers.Components;

namespace VirtualPhenix.Pathfinding
{
	[DefaultExecutionOrder(VP_ExecutingOrderSetup.PATHFINDING_AGENT), AddComponentMenu(VP_PrefixSetup.MAIN_PREFIX+"/Pathfinding/VP Pathfinding Agent")]
	public class VP_PathfindingAgent : VP_CharacterComponent
	{
		public enum PATHFINDING_TYPE
		{
			ASTAR,
			NAVMESH
		}
		
		[Header("Non Playable Character"), Space]
		[SerializeField] protected PATHFINDING_TYPE m_pathfindingType = PATHFINDING_TYPE.NAVMESH;
		
		[Header("Navmesh Agent"), Space]
		[SerializeField] protected Transform m_mainObject;
		[SerializeField] protected NavMeshAgent m_navmeshAgent;
		
		[Header("Target"), Space]
		[SerializeField] protected Vector3 m_targetPoint;
		
		[Header("Movement Configuration"), Space]
		[SerializeField] protected float m_speed = 5f;
		[SerializeField] protected float m_angularSpeed = 5f;
		[SerializeField] protected float m_stopDistance = 1f;
		[SerializeField] protected float m_thresholdToReach = 0.1f;
		[SerializeField] protected float m_sampleHeight = 2f;
		
		[Header("Character Controller Check"), Space]
		[SerializeField] protected bool m_manualTarget = false;
		
		protected bool m_startUpdateRotation;
		
		public virtual Vector3 TargetPoint { get { return m_targetPoint; } }
		public virtual NavMeshAgent NavMeshAgent { get { return m_navmeshAgent; } }
		public virtual bool IsStopped { get { return m_navmeshAgent != null ? m_navmeshAgent.isStopped : true; } }
		public virtual PATHFINDING_TYPE PathfindingType { get { return m_pathfindingType; } }
		public virtual bool IsManualTarget { get { return m_manualTarget; } }
		public virtual Transform MainObject { get { return m_mainObject; } }
		public virtual float Speed { get { return m_navmeshAgent.speed; } }
		
		protected override void Initialize()
		{
			base.Initialize();
			
#if !USE_ASTAR_PROJECT_PRO	
			if (m_pathfindingType == PATHFINDING_TYPE.ASTAR)
				m_pathfindingType = PATHFINDING_TYPE.NAVMESH;
#endif

			if (!m_mainObject)
				m_mainObject = this.transform;
			
			SetupAgent();
		}

		public virtual Vector3 AgentDestination()
		{
			if (m_pathfindingType == PATHFINDING_TYPE.NAVMESH)
			{
				if (m_navmeshAgent)
				{
					return m_navmeshAgent.destination;
				}
			}
			
			return Vector3.zero;
		}

		protected override void Reset()
		{
			base.Reset();

			if (!m_mainObject)
				m_mainObject = this.transform;

			transform.TryGetComponent<NavMeshAgent>(out m_navmeshAgent);
		}

		public virtual void SetupAgent()
		{
			if (m_pathfindingType == PATHFINDING_TYPE.NAVMESH)
			{
				if (m_navmeshAgent && m_navmeshAgent.isOnNavMesh)
				{
					m_startUpdateRotation = m_navmeshAgent.updateRotation;
					m_navmeshAgent.speed = m_speed;
					m_navmeshAgent.angularSpeed = m_angularSpeed;
					m_navmeshAgent.stoppingDistance = m_stopDistance;
				}
			}
		}

		public virtual void SetCharacterControllerDestination(Vector3 _target, bool _unlock)
		{
			m_targetPoint = _target;
			m_manualTarget = true;
			
			EnableAgent(_unlock);
		}

		public virtual void EnableAgent(bool _active)
		{
			m_navmeshAgent.enabled = _active;
		}

		public virtual void SetDestination(Vector3 _target)
		{
			m_targetPoint = _target;
		}

		public virtual void GoToPoint(Vector3 _point)
		{
			m_manualTarget = false;
			SetDestination(_point);
			if (m_navmeshAgent && m_navmeshAgent.isOnNavMesh)
			{
				m_navmeshAgent.SetDestination(_point);
			}
		}
		
		public virtual void GoToPoint(Transform _trs)
		{
			if (_trs != null)
				GoToPoint(_trs.position);
		}
		
		public virtual void UnlockAgent()
		{
			m_navmeshAgent.isStopped = false;
			m_navmeshAgent.updatePosition = true;
			m_navmeshAgent.updateRotation = true;
		}
		
		public virtual void BlockAgent()
		{
			m_navmeshAgent.isStopped = true;
			m_navmeshAgent.updatePosition = false;
			m_navmeshAgent.updateRotation = false;
		}
		
		public virtual void OnEndPath(bool _stop)
		{
			if (_stop) 
			{
				Stop();
			} 
			else 
			{
				SetUpdateRotation(m_startUpdateRotation);
			}
		}
		
		public virtual bool HasPathToPoint()
		{
			//Debug.Log("Has path to point" + (m_manualTarget && DistanceToTargetPoint() > m_thresholdToReach) + " bc " +m_manualTarget + " and "+ DistanceToTargetPoint());
			return m_manualTarget && DistanceToTargetPoint() > m_thresholdToReach;
		}
		
		public virtual bool HasPath()
		{
			return (HasPathToPoint()) || (m_navmeshAgent.hasPath && m_navmeshAgent.remainingDistance > m_thresholdToReach);
		}

		public virtual bool HasArrivedToTargetPoint()
		{
			return DistanceToTargetPoint() <= m_thresholdToReach;
		}

		public virtual float DistanceToTargetPoint()
		{
			if (!m_mainObject)
				m_mainObject = transform;
			
			return Vector3.Distance(m_mainObject.position, m_targetPoint);
		}
		
		public virtual void Stop()
		{
			if (m_manualTarget)
				m_manualTarget = false;
			
			SetUpdateRotation(m_startUpdateRotation);
			if (m_navmeshAgent.hasPath) 
			{
				m_navmeshAgent.isStopped = true;
			}
		}
		
		public virtual bool HasArrivedToTarget()
		{
			if (m_manualTarget)
				return HasArrivedToTargetPoint();
			
			// The path hasn't been computed yet if the path is pending.
			float remainingDistance;
			
			if (m_navmeshAgent.pathPending) 
			{
				remainingDistance = float.PositiveInfinity;
			} 
			else 
			{
				remainingDistance = m_navmeshAgent.remainingDistance;
			}

			return remainingDistance <= m_thresholdToReach;
		}
		
		public virtual void SetUpdateRotation(bool _value)
		{
			m_navmeshAgent.updateRotation = _value;
			m_navmeshAgent.updateUpAxis = _value;
		}
		
		public virtual bool SamplePosition(Vector3 position)
		{
			NavMeshHit hit;
			return NavMesh.SamplePosition(position, out hit, m_navmeshAgent.height * m_sampleHeight, NavMesh.AllAreas);
		}
		
		public virtual bool PointReached()
		{
			return m_navmeshAgent.remainingDistance <= m_thresholdToReach;
		}
		
		public virtual float DistanceToTarget()
		{
			return m_navmeshAgent.remainingDistance;
		}
		
		public virtual void SetSpeed(float _speed, bool _saveIt = true)
		{
			if (_saveIt)
				m_speed = _speed;
				
			m_navmeshAgent.speed = _speed;
		}
		
		public virtual void SetAngularSpeed(float _speed, bool _saveIt = true)
		{
			if (_saveIt)
				m_angularSpeed = _speed;
				
			m_navmeshAgent.angularSpeed = _speed;
		}
		
		
		public virtual void SetStoppingDistance(float _distance, bool _saveIt = true)
		{
			if (_saveIt)
				m_stopDistance = _distance;
				
			m_navmeshAgent.stoppingDistance = _distance;
		}
	}

}