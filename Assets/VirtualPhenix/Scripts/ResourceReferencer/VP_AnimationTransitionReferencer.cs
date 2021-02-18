using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

#if USE_ANIMANCER	
using Animancer;
#endif

namespace VirtualPhenix.ResourceReference
{	
	
#if USE_ANIMANCER	
	[System.Serializable]
	public class VP_AnimationTransitionReferencer<T, T1> : VP_ResourceReferencer<T, VP_ClipStateTransitionDictionary<T1>>
    {
	    public virtual VP_ClipStateTransitionDictionary<T1> GetAnimation(T _category)
        {
            return base.GetResource(_category);
        }
        
	    public virtual bool TryGetClip(T1 _id, out ClipState.Transition _value, T layer = default(T))
	    {
		    if (m_resources.ContainsKey(layer))
		    {
			    if (m_resources[layer].Contains(_id))
			    {
				    _value = m_resources[layer][_id];
				    return true;
			    }
		    }

		    _value = new ClipState.Transition();
		    return false;
	    }

	    public virtual ClipState.Transition GetClip(T dic, T1 key)
	    {
		    var d = GetResource(dic);
		    if (d == null)
		    {
		    	Debug.Log("Animation Transition Resources is null for "+name);
		    }
		    return d != null && d.ContainsKey(key) ? d[key] : null;
	    }

		public virtual void PlayAnimancerClip(Animancer.AnimancerComponent _animancer, T form, T1 _animation, float _fixedTime = 0.25f)
	    {
		    if (m_resources.ContainsKey(form) && m_resources[form].ContainsKey(_animation))
		    {
			    _animancer.Play(m_resources[form][_animation], _fixedTime);
		    }
	    }
	    
  
	    public virtual T1 GetIDFromClip(AnimationClip clip, T dic)
	    {
		    var resourc = GetResource(dic);

		    for (int i = 0; i < resourc.Count; i++)
		    {
			    var v = resourc.Values.ElementAt(i);
			    if (v.Clip == clip)
			    {
				    return resourc.Keys.ElementAt(i);
			    }
		    }
	        
		    return default(T1);
	    }
    }
#endif 
}
