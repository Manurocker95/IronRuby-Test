using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.ResourceReference
{
    public class VP_ColorReferencer<T> : VP_ResourceReferencer<T, Color>
    {
        public virtual Color GetColor(T _category)
        {
            return base.GetResource(_category);
        }
    }
}
