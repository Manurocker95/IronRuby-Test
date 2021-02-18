using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix.Fade;

namespace VirtualPhenix.ResourceReference
{

    public class VP_FadeEffectReferencer<T> : VP_ResourceReferencer<T, VP_FadeEffect>
    {
        public virtual VP_FadeEffect GetFadeEffect(T _category)
        {
            return base.GetResource(_category);
        }
    }

}
