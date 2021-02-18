using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Debugging
{
	[AddComponentMenu(VP_PrefixSetup.MAIN_PREFIX+"/Debugging/Log Displat"),DefaultExecutionOrder(VP_ExecutingOrderSetup.LOG_DISPLAYER)]
	public class VP_LogDisplayer : VP_MonoBehaviour
	{
		[Header("Log Text"),Space]
		[SerializeField, TextArea] protected string m_debugLog;
		
		[Header("Log Config"),Space]
		[SerializeField] protected DEBUG_COLOR m_logColor = DEBUG_COLOR.AQUA;
		[SerializeField] protected string m_logTag = "";
		[SerializeField] protected bool m_useCustomIcon = false;
		
		protected override void Initialize()
		{
			base.Initialize();
			DisplayLog(m_debugLog, m_logColor, m_logTag, m_useCustomIcon);
		}
		
		public virtual void DisplayLog(string _log, DEBUG_COLOR _color = DEBUG_COLOR.NONE, string _tag = "", bool _useIcon = false)
		{
			VP_Debug.Log(_log, _color, _tag, _useIcon);
		}
	}

}