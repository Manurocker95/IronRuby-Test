using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.ResourceReference
{
    public class VP_AnimationClipReferencer<T> : VP_ResourceReferencer<T, AnimationClip>
    {
        public virtual AnimationClip GetAnimation(T _category)
        {
            return base.GetResource(_category);
        }
    }
}
