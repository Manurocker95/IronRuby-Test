#if USE_BEHAVIOR_DESIGNER
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections.Generic;

namespace VirtualPhenix.Integrations.BehaviorDesignerTree
{
	[TaskDescription("Triggers an event with Event Manager")]
	[TaskCategory(VP_PrefixSetup.MAIN_PREFIX+"/Events")]
	[TaskIcon("Assets/VirtualPhenix/Editor/Icons/Behavior Designer/EventTrigger.png")]
    public class TriggerEvent : Action
    {
        [BehaviorDesigner.Runtime.Tasks.Tooltip("Event Name")]
        [TextArea] public SharedString m_eventTrigger;
	    [TextArea] public SharedVariable m_parameter;

        public override void OnStart()
        {
	        if (m_eventTrigger.Value.IsNotNullNorEmpty())
	        {
	        	if (m_parameter.GetValue() != null)
	        	{
	        		VP_EventManager.TriggerEvent(m_eventTrigger.Value, m_parameter);
	        	}
	        	else
	        	{
	        		VP_EventManager.TriggerEvent(m_eventTrigger.Value);
	        	}
	        }
		        
        }

        public override TaskStatus OnUpdate()
        {
	        if (m_eventTrigger.Value.IsNullOrEmpty())
                return TaskStatus.Failure;

            return TaskStatus.Success;
        }


        public override void OnReset()
        {
	        m_eventTrigger = string.Empty;
        }
    }
}
#endif
