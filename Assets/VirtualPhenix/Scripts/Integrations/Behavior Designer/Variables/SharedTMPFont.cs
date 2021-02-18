#if USE_BEHAVIOR_DESIGNER

using UnityEngine;
using BehaviorDesigner.Runtime;
using VirtualPhenix.Dialog;
using TMPro;

namespace VirtualPhenix.Integrations.BehaviorDesignerTree
{
	[System.Serializable]
	public class SharedTMPFont : SharedVariable<TMP_FontAsset>
	{
		public static implicit operator SharedTMPFont(TMP_FontAsset value) { return new SharedTMPFont { mValue = value }; }
	}
}
#endif
