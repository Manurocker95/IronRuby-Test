#if USE_BEHAVIOR_DESIGNER

using UnityEngine;
using BehaviorDesigner.Runtime;

namespace VirtualPhenix.Integrations.BehaviorDesignerTree
{
	[System.Serializable]
	public class SharedAudioClip2 : SharedVariable<AudioClip>
	{
		public static implicit operator SharedAudioClip2(AudioClip value) { return new SharedAudioClip2 { mValue = value }; }
	}
}
#endif
