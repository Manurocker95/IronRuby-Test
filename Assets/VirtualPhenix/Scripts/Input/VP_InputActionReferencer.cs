using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix.Inputs;

namespace VirtualPhenix.ResourceReference
{
    public class VP_InputActionReferencer<T> : VP_ResourceReferencer<T, VP_InputKeyData>
    {
        public virtual VP_InputKeyData GetInputKeyData(T _category)
        {
            return base.GetResource(_category);
        }
    }
}
