using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.ResourceReference
{
    public class VP_SpriteReferencer<T> : VP_ResourceReferencer<T, Sprite>
    {
        public virtual Sprite GetSprite(T _category)
        {
            return base.GetResource(_category);
        }
    }
}
