#if USE_ANIMANCER
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Object = UnityEngine.Object;
using Animancer;
using VirtualPhenix.ResourceReference;

namespace VirtualPhenix
{
    [DefaultExecutionOrder(VP_ExecutingOrderSetup.CUSTOM_ANIMATOR), AddComponentMenu(VP_PrefixSetup.MAIN_PREFIX+"/Animator/VP Animator Without Sets")]
	/// <summary>
	/// This class uses simple animations instead of full sets
	/// </summary>
	public class VP_SimpleAnimator : VP_Animator
	{
    	
		[Header("Simple Animations"),Space]
		[SerializeField] protected VP_AnimationTransitionResources m_customAnimations;

		public override AnimancerState PlayAnimationFromSet(string _animationKey, int layer = -1, int dicIndex = 0, string _animationSetID = "Default", float _fadeSpeed = -1)
		{
			var state =  PlayAnimationFromResources(m_customAnimations, _animationKey, layer, dicIndex, _fadeSpeed);
			return state;
		}
	

		public override bool TryGetClip(string _animation, out Animancer.ClipState.Transition _transition, int _layer = 0, string _actionSet = "Default")
		{
			if (!m_customAnimations)
			{
				Debug.Log("No custom animations on " + name);
				_transition = null;
				return false;
			}
			//Debug.Log("Try Get Clip " + name);
			return m_customAnimations.TryGetClip(_animation, out _transition, _layer);
		}
        
		public override float GetAnimationSpeed(string _animation, int _layer = 0, string _actionSet = "Default")
		{
			if (TryGetClip(_animation, out Animancer.ClipState.Transition _transition, _layer))
			{
				return _transition.Speed;
			}
			
			return 1.5f;
		}
	    
		public override float GetAnimationFadeDuration(string _animation, int _layer = 0, string _actionSet = "Default")
		{
			if (TryGetClip(_animation, out Animancer.ClipState.Transition _transition, _layer))
			{
				return _transition.FadeDuration;
			}
			
			return 0.25f;
		}
		
		public override bool HasCurrentAnimationComparedID(string ID, int layer, string keyset = "Default")
		{
			if (!IsPlaying())
				return false;
			    
			var _state = Layers[layer].CurrentState;  
		    
			return m_customAnimations.GetIDFromClip(_state.Clip, layer) == ID;
		}

		public override string CurrentAnimationID(int layer, string keyset = "Default")
		{
			if (!IsPlaying())
				return "";
			    
			var _state = Layers[layer].CurrentState;  
		   
			return m_customAnimations.GetIDFromClip(_state.Clip, layer);
		}

	}
}
#endif
