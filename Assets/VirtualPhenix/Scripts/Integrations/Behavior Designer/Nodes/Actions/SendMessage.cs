#if USE_BEHAVIOR_DESIGNER
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections.Generic;

namespace VirtualPhenix.Integrations.BehaviorDesignerTree
{
	[TaskDescription("Send message to call object's methods without casting")]
	[TaskCategory(VP_PrefixSetup.MAIN_PREFIX+"/Reflection")]
	[TaskIcon("Assets/VirtualPhenix/Editor/Icons/Behavior Designer/SendMessageIcon.png")]
    public class SendMessage : Action
    {
        [BehaviorDesigner.Runtime.Tasks.Tooltip("Object")]
        [TextArea] public SharedGameObject m_objectToCall;

        [BehaviorDesigner.Runtime.Tasks.Tooltip("Method Name")]
        [TextArea] public SharedString m_methodName;

        public override void OnStart()
        {
	        if (!m_methodName.Value.IsNullOrEmpty() && m_objectToCall.Value != null)
	        {
		        m_objectToCall.Value.SendMessage(m_methodName.Value);
	        }
        }


	    public override TaskStatus OnUpdate()
	    {
		    if (m_methodName.Value.IsNullOrEmpty())
			    return TaskStatus.Failure;

		    return TaskStatus.Success;
	    }
    }

}
#endif