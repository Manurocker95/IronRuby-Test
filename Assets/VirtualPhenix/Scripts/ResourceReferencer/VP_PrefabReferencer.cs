using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.ResourceReference
{
    public class VP_PrefabReferencer<T> : VP_ResourceReferencer<T, GameObject>
    {
        public virtual GameObject GetPrefab(T _category)
        {
            return base.GetResource(_category);
        }
    }
}
