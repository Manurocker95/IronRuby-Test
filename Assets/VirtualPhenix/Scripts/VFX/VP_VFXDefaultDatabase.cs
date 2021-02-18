using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix.ResourceReference;

namespace VirtualPhenix
{
    [CreateAssetMenu(fileName = "VFXDatabase", menuName = "Virtual Phenix/Scriptable Objects/VFX", order = 1)]
    public class VP_VFXDefaultDatabase : VP_ResourceReferencer<string, GameObject>
    {
        public virtual GameObject GetVFX(string _key)
        {
            return GetResource(_key);
        }
    }
}
