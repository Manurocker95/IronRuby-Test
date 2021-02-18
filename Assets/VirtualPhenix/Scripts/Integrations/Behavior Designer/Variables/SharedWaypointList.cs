#if USE_BEHAVIOR_DESIGNER

using UnityEngine;
using BehaviorDesigner.Runtime;
using VirtualPhenix.Controllers;
using VirtualPhenix.Dialog;
using System.Collections.Generic;

namespace VirtualPhenix.Integrations.BehaviorDesignerTree
{
	[System.Serializable]
	public class SharedWaypointList : SharedVariable<List<VP_Waypoint>>
	{
		public static implicit operator SharedWaypointList(List<VP_Waypoint> value) { return new SharedWaypointList { mValue = value }; }
	}
}
#endif