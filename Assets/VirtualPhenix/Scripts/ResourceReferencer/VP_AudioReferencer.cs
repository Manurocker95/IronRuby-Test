using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.ResourceReference
{
    public class VP_AudioReferencer<T> : VP_ResourceReferencer<T, AudioClip>
    {
        public virtual AudioClip GetAudio(T _category)
        {
            return base.GetResource(_category);
        }
    }
}
