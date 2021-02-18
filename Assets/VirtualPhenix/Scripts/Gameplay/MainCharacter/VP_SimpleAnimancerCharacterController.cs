using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if USE_ANIMANCER
using Animancer;


using VirtualPhenix.ResourceReference;

namespace VirtualPhenix.Controllers
{
    [System.Serializable]
    public class VP_AnimationSetData
    {
        public string m_animationSetID = "Default";
        public int m_defaultLayer = 0;
        public int m_defaultAnimationGroup = 0;
        public float m_defaultTransitionDuration = -1;
    }
    [DefaultExecutionOrder(VP_ExecutingOrderSetup.CHARACTER_CONTROLLER), AddComponentMenu(VP_PrefixSetup.MAIN_PREFIX + "/Controllers/Simple Player Controller With Animancer")]
    public class VP_SimpleAnimancerCharacterController : VP_SimpleCharacterController
    {
#if USE_ANIMANCER
        [Header("Animancer"), Space]
        [SerializeField] protected VP_AnimationTransitionResources m_animations;
        [SerializeField] protected bool m_useCustomAnimationResources = true;
        [SerializeField] protected VP_AnimationSetData m_animationSetData = new VP_AnimationSetData();

        protected AnimancerState m_currentState;
#endif
        public virtual bool HasAnimations
        {
            get
            {
#if USE_ANIMANCER
                return m_animator != null && m_animations != null;
#else
                return m_animator.HasAnimations;
#endif
            }
        }

	    protected override void PlayAnimation(string _animation, UnityEngine.Events.UnityAction _callback = null, float _speed = -1, int _layer = -1, string _layerName = "Base Layer")
        {
#if USE_ANIMANCER
            if (HasAnimations && m_useCustomAnimationResources)
            {
                m_currentState = m_animator.PlayAnimationFromResources(ref m_animations, _animation,
	                m_animationSetData.m_defaultLayer, m_animationSetData.m_defaultAnimationGroup, m_animationSetData.m_defaultTransitionDuration);

                if (m_currentState != null)
                {
                	if (_speed > 0)
                		m_currentState.Speed = _speed;
	                
                    if (_callback != null)
                        m_currentState.Events.OnEnd = _callback.Invoke;
                }
                else
                {
                    if (_callback != null)
                        _callback.Invoke();
                }
            }
            else if (m_animator != null)
            {
                m_currentState = m_animator.PlayAnimationFromSet(_animation,
                    m_animationSetData.m_defaultLayer, m_animationSetData.m_defaultAnimationGroup,
	                m_animationSetData.m_animationSetID, m_animationSetData.m_defaultTransitionDuration);

                if (m_currentState != null)
                {
                	if (_speed > 0)
                		m_currentState.Speed = _speed;
                	
                    if (_callback != null)
                        m_currentState.Events.OnEnd = _callback.Invoke;
                }
                else
                {
                    if (_callback != null)
                        _callback.Invoke();
                }

            }
            else
            {
                m_currentState = null;
                if (_callback != null)
                    _callback.Invoke();
            }
#else  
	        base.PlayAnimation(_animation, _callback, _speed);
#endif
	           
        }
    }
}
#endif