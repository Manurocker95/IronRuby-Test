using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix
{
	[AddComponentMenu(VP_PrefixSetup.MAIN_PREFIX+"/Events/Simple Event Invoker")]
	public class VP_EventInvoker : VP_MonoBehaviour
	{
		[SerializeField] protected string m_eventName = "";
		[SerializeField] protected bool m_sendParameter = false;
		
#if ODIN_INSPECTOR		
		[Sirenix.Serialization.OdinSerialize] protected object m_parameter;
#else
		[SerializeField] protected object m_parameter;
#endif		

		protected override void Initialize()
		{
			base.Initialize();
			TriggerEvent();
		}
		
		public virtual void TriggerEvent()
		{
			if (m_eventName.IsNotNullNorEmpty())
			{
				Debug.Log("Triggering event");
				
				if (m_sendParameter && m_parameter != null)
					VP_EventManager.TriggerEvent(m_eventName, m_parameter);
				else
					VP_EventManager.TriggerEvent(m_eventName);
			}
		}
	}

}