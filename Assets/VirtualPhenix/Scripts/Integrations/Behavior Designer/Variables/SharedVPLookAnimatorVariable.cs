
#if USE_BEHAVIOR_DESIGNER

using UnityEngine;
using BehaviorDesigner.Runtime;

namespace VirtualPhenix.Integrations.BehaviorDesignerTree
{
    [System.Serializable]
    public class SharedVPLookAnimatorVariable : SharedVariable<VP_LookAnimator>
    {
        public static implicit operator SharedVPLookAnimatorVariable(VP_LookAnimator value) { return new SharedVPLookAnimatorVariable { mValue = value }; }
    }
}
#endif
