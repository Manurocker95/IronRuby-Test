#if USE_BEHAVIOR_DESIGNER

using UnityEngine;
using BehaviorDesigner.Runtime;

namespace VirtualPhenix.Integrations.BehaviorDesignerTree
{
	[System.Serializable]
	public class SharedWaypoint : SharedVariable<VP_Waypoint>
	{
		public static implicit operator SharedWaypoint(VP_Waypoint value) { return new SharedWaypoint { mValue = value }; }
	}
}
#endif
