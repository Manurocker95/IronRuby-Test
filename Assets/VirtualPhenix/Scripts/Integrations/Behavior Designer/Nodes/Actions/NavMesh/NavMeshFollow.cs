#if USE_BEHAVIOR_DESIGNER && USE_ASTAR_PROJECT_PRO
using UnityEngine; 
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Movement;
using BehaviorDesigner.Runtime.Tasks.Movement.AstarPathfindingProject;

namespace VirtualPhenix.Integrations.BehaviorDesignerTree
{
	[TaskDescription("Follow the target specified using AStar Pathfinding Project Pro.")]
	[TaskCategory(VP_PrefixSetup.MAIN_PREFIX+"/Movement/NavMesh")]
	[TaskIcon("Assets/VirtualPhenix/Editor/Icons/Behavior Designer/{SkinColor}SeekIcon.png")]
	public class NavMeshFollow : NavMeshMovement
    {
	    [BehaviorDesigner.Runtime.Tasks.Tooltip("The GameObject that the agent is following")]
	    public SharedGameObject m_target;
	    [BehaviorDesigner.Runtime.Tasks.Tooltip("The GameObject of the agent.")]
	    public SharedGameObject m_self;
	    [BehaviorDesigner.Runtime.Tasks.Tooltip("Start moving towards the target if the target is further than the specified distance")]
	    public SharedFloat m_moveDistance = 2;


	    protected Vector3 m_lastTargetPosition;
	    protected bool m_hasMoved;


        public override void OnStart()
        {
	        base.OnStart();
	        m_lastTargetPosition = m_target.Value.transform.position + Vector3.one * (m_moveDistance.Value + 1);
	        m_hasMoved = false;
        }

        // Follow the target. The task will never return success as the agent should continue to follow the target even after arriving at the destination.
        public override TaskStatus OnUpdate()
        {
            // Move if the target has moved more than the moveDistance since the last time the agent moved.
	        var targetPosition = m_target.Value.transform.position;
	        if ((targetPosition - m_lastTargetPosition).magnitude >= m_moveDistance.Value)
		    {
                SetDestination(targetPosition);
                m_lastTargetPosition = targetPosition;
	            m_hasMoved = true;
	            
		        DoOnNotArrive();
            }
	        else 
	        {
                // Stop moving if the agent is within the moveDistance of the target.
	            if (m_hasMoved && (targetPosition - transform.position).magnitude < m_moveDistance.Value) 
	            {
                    Stop();
	                m_hasMoved = false;
	                m_lastTargetPosition = targetPosition;
                }
            }

            return TaskStatus.Running;
        }


	    protected override void DoOnArrive()
	    {
	    	base.DoOnArrive();
	    }
	    
	    protected override void DoOnNotArrive()
	    {
	    	base.DoOnArrive();
	    }

        public override void OnReset()
        {
            base.OnReset();
            m_target = null;
            m_moveDistance = 2;
        }
    }
}
#endif