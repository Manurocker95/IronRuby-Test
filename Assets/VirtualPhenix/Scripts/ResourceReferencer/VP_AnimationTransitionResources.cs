using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if USE_ANIMANCER
using Animancer;
#endif

namespace VirtualPhenix.ResourceReference
{
#if USE_ANIMANCER	
    [System.Serializable]
    [CreateAssetMenu(fileName = "Animation Animancer Transition Referencer", menuName = "Virtual Phenix/Resource Dictionary/Animation/Animancer Animation Resources", order = 1)]
	public class VP_AnimationTransitionResources : VP_AnimationTransitionReferencer<int, string>
    {
        
	   
    }
   #endif 
}