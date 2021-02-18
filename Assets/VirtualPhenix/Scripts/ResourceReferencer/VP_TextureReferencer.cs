using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.ResourceReference
{
    public class VP_TextureReferencer<T> : VP_ResourceReferencer<T, Texture>
    {
        public virtual Texture GetTexture(T _category)
        {
            return base.GetResource(_category);
        }
    }
}
