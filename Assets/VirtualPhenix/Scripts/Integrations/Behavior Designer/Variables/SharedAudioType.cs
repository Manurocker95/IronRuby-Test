#if USE_BEHAVIOR_DESIGNER

using UnityEngine;
using BehaviorDesigner.Runtime;

namespace VirtualPhenix.Integrations.BehaviorDesignerTree
{
	[System.Serializable]
	public class SharedAudioType : SharedVariable<VP_AudioSetup.AUDIO_TYPE>
	{
		public static implicit operator SharedAudioType(VP_AudioSetup.AUDIO_TYPE value) { return new SharedAudioType { mValue = value }; }
	}
}
#endif
