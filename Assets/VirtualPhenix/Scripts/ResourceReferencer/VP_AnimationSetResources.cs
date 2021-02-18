using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if USE_ANIMANCER
using Animancer;
#endif

namespace VirtualPhenix
{
#if USE_ANIMANCER
    [System.Serializable]
    [CreateAssetMenu(fileName = "Animancer Set Referencer", menuName = "Virtual Phenix/Resource Dictionary/Animation/Set of Animancer Animation Resources", order = 1)]
    public class VP_AnimationSetResources : VP_AnimationSetReferencer<string>
    {
        public virtual bool TryGetClipFromSet(string _animationID, int _layer, out ClipState.Transition clip, string _id = "Default")
        {
            clip = GetAnimationInLayer(_id, _layer, _animationID);
            return clip != null;
        }
       
    }
#endif
}