#if USE_BEHAVIOR_DESIGNER
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Movement;
using System.Collections.Generic;

namespace VirtualPhenix.Integrations.BehaviorDesignerTree
{
	[TaskDescription("Patrol using NavMesh Agent.")]
	[TaskCategory(VP_PrefixSetup.MAIN_PREFIX+"/Movement/NavMesh")]
	[TaskIcon("Assets/VirtualPhenix/Editor/Icons/Behavior Designer/{SkinColor}PatrolIcon.png")]
	public class NavMeshPatrol : NavMeshMovement
	{
		[Header("Patrol")]
        [BehaviorDesigner.Runtime.Tasks.Tooltip("Should the agent patrol the waypoints randomly?")]
        public SharedBool m_randomPatrol = false;
        [BehaviorDesigner.Runtime.Tasks.Tooltip("The length of time that the agent should pause when arriving at a waypoint")]
        public SharedFloat m_waypointPauseDuration = 0;
        [BehaviorDesigner.Runtime.Tasks.Tooltip("The waypoints to move to")]
	    public SharedWaypointList m_waypoints;
        [BehaviorDesigner.Runtime.Tasks.Tooltip("Game object with animator that triggers animation while walking")]
        public SharedGameObject m_target;
        [BehaviorDesigner.Runtime.Tasks.Tooltip("The length of time that the agent should pause when arriving at a waypoint")]
        public SharedFloat m_animatorSpeed = 0.5f;
        [BehaviorDesigner.Runtime.Tasks.Tooltip("Speed parameter name")]
        public SharedString m_animatorSpeedParameter = "speed";

        // The current index that we are heading towards within the waypoints array
        protected int m_waypointIndex;
        protected float m_waypointReachedTime;

        public override void OnStart()
        {
            base.OnStart();

            CheckNewWaypoint();

	        m_waypoints.Value[m_waypointIndex].m_selected = true;
	        m_waypointReachedTime = -m_waypointPauseDuration.Value;
            SetDestination(Target());
        }

	    protected override void DoOnArrive()
	    {
	    	base.DoOnArrive();
	    	
		    if (m_waypointReachedTime == -m_waypointPauseDuration.Value)
		    {
			    StopSpeed();
			    m_waypointReachedTime = Time.time;
		    }
		    // wait the required duration before switching waypoints.
		    if (m_waypointReachedTime + m_waypointPauseDuration.Value <= Time.time) 
		    {
			    CheckNewWaypoint();
		    }
	    }

        protected virtual void StopSpeed()
        {


        }

        protected virtual void ResumeSpeed()
        {
	    
        }

        protected virtual void CheckNewWaypoint()
        {
	        m_waypoints.Value[m_waypointIndex].m_selected = false;

            if (m_randomPatrol.Value)
            {
                if (m_waypoints.Value.Count == 1)
                {
                    m_waypointIndex = 0;
                }
                else
                {
                    // prevent the same waypoint from being selected
                    var newWaypointIndex = m_waypointIndex;
                    while (newWaypointIndex == m_waypointIndex)
                    {
                        newWaypointIndex = Random.Range(0, m_waypoints.Value.Count);
                    }
                    m_waypointIndex = newWaypointIndex;
                }
            }
            else
            {
                m_waypointIndex = (m_waypointIndex + 1) % m_waypoints.Value.Count;
            }
           
            SetDestination(Target());

	        m_waypoints.Value[m_waypointIndex].m_selected = true;

            ResumeSpeed();
        }

        // Return the current waypoint index position
	    protected virtual Vector3 Target()
        {
            return m_waypoints.Value[m_waypointIndex].transform.position;
        }

        // Reset the public variables
        public override void OnReset()
        {
            base.OnReset();

            m_randomPatrol = false;
            m_waypointPauseDuration = 0;
            m_waypoints = null;
        }
    }
}
#endif