using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix.ResourceReference;

#if USE_ANIMANCER
using Animancer;
#endif

namespace VirtualPhenix
{
#if USE_ANIMANCER
    public class VP_AnimationSetReferencer<T> : VP_ResourceReferencer<T, Dictionary<int, Dictionary<string, ClipState.Transition>>>
    {
        public virtual Dictionary<int, Dictionary<string, ClipState.Transition>> GetFullAnimationSet(T _ID)
        {
            return GetResource(_ID);
        }

        public virtual Dictionary<string, ClipState.Transition> GetAnimationSetInLayer(T _ID, int _layer)
        {
            return GetResource(_ID)[_layer];
        }

        public virtual ClipState.Transition GetAnimationInLayer(T _ID, int _layer, string _animationID)
        {
            return GetResource(_ID)[_layer][_animationID];
        }
        
	    public virtual string GetIDFromClip(AnimationClip clip, int _layer, T k)
	    {
		    var resourc = GetFullAnimationSet(k)[_layer];

		    for (int i = 0; i < resourc.Count; i++)
		    {
			    var v = resourc.Values.ElementAt(i);
			    if (v.Clip == clip)
			    {
				    return resourc.Keys.ElementAt(i);
			    }
		    }
	        
		    return "";
	    }
        
	    
	    public virtual string GetIDFromClip(ClipState.Transition transition, int _layer, T k)
        {
	        var resourc = GetFullAnimationSet(k)[_layer];
	        if (!resourc.ContainsValue(transition))
		        return "";
		        
	        for (int i = 0; i < resourc.Count; i++)
	        {
	        	var v = resourc.Values.ElementAt(i);
	        	if (v == transition || v.Clip.name == transition.Clip.name)
	        	{
	        		return resourc.Keys.ElementAt(i);
	        	}
	        }
	        
	        return "";
        }
        
	    public virtual bool IsClipFromID(string _id, ClipState.Transition transition, int _layer, T k)
	    {
		    var resourc = GetFullAnimationSet(k)[_layer];
		    if (!resourc.ContainsValue(transition))
			    return false;
		        
		    return resourc[_id] == transition;
	    }
        
    }
#endif
}
