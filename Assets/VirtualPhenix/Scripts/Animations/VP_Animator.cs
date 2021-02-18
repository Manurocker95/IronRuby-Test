using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if USE_ANIMANCER
using Animancer;
#else
using VirtualPhenix.Controllers.Components;
#endif

using VirtualPhenix.Animations;

namespace VirtualPhenix
{
    [System.Serializable]
    public class VP_MaskData
    {
        public int m_layer = 1;
        public AvatarMask m_avatarMask;
        public string m_maskName = "Mouth Mask";

        public VP_MaskData()
        {
            m_layer = 1;
            m_avatarMask = null;
            m_maskName = "Mouth Mask";
        }
    }

#if USE_ANIMANCER
	[DefaultExecutionOrder(VP_ExecutingOrderSetup.CUSTOM_ANIMATOR),AddComponentMenu(VP_PrefixSetup.MAIN_PREFIX+"/Animator/VP Animator")]
    public class VP_Animator : NamedAnimancerComponent
#else

    [DefaultExecutionOrder(VP_ExecutingOrderSetup.CUSTOM_ANIMATOR), AddComponentMenu(VP_PrefixSetup.MAIN_PREFIX+"/Animator/VP Animator")]
    public class VP_Animator : VP_CharacterComponent
#endif
    {
        /************************************************************************************************************************/
        #region Fields and Properties
        /************************************************************************************************************************/

#if USE_ANIMANCER
        [Header("Animancer Animation Set")]
        [SerializeField] protected string m_currentAnimationSet = "Default";
        [SerializeField] protected VP_AnimationSetResources m_animationSet;
        [SerializeField] protected VP_MaskData[] m_avatarMask;
        public VP_AnimationSetResources AnimationSet => m_animationSet;

        [SerializeField, Tooltip("The main Animator Controller that this object will play")]
        protected ControllerState.Transition _Controller;

        /// <summary>[<see cref="SerializeField"/>]
        /// The transition containing the main <see cref="RuntimeAnimatorController"/> that this object plays.
        /// </summary>
        public ref ControllerState.Transition Controller => ref _Controller;
#else
        [Header("Regular Animator")]
        [SerializeField] protected Animator m_animator;
        
        public virtual Animator Animator { get { return m_animator; } }
        public virtual bool HasAnimations { get { return m_animator != null && m_animator.runtimeAnimatorController != null; } }
#endif

/************************************************************************************************************************/
#endregion
/************************************************************************************************************************/
#region Initialisation
/************************************************************************************************************************/
#if USE_ANIMANCER
        protected override void Awake()
        {
            base.Awake();

            foreach (VP_MaskData mask in m_avatarMask)
            {
                SetAvatarMask(mask.m_avatarMask, mask.m_layer, mask.m_maskName);
            }
        }

	    public virtual void SetAvatarMask(AvatarMask _mask, int _layer = 1, string _editorName = "Secondary Layer")
        {
            if (_mask == null)
            {
                Debug.LogError("Mask is null in "+name);
                return;
            }
            // Set the mask for layer 1 (this automatically creates the layer).
            Layers[_layer].SetMask(_mask);
            Layers[_layer].SetDebugName(_editorName);
        }
#endif

	    public virtual float GetAnimationSpeed(string _animation, int _layer = 0, string _actionSet = "Default")
	    {
#if USE_ANIMANCER
            if (m_animationSet.TryGetClipFromSet(_animation, _layer, out ClipState.Transition _transition, _actionSet))
		    {
                m_currentAnimationSet = _actionSet;

                return _transition.Speed;
		    }
		    return 1f;
#else
		    // TODO CHECK BY ANIMATION NAME
		    return m_animator.GetCurrentAnimatorStateInfo(_layer).speed;			
#endif
	    }
	    
	    public virtual void SetParameters(KeyValuePair<string, object>[] _parameters, int dic = 0, string set = "Default", bool _useAnimancer = true)
	    {
	    	for (int i = 0; i < _parameters.Length; i++)
	    	{	    		
		    	string name = _parameters[i].Key;
		    	object parameter = _parameters[i].Value;
#if USE_ANIMANCER
		    	if (!_useAnimancer && (_Controller != null || _Controller.Controller != null))
		    	{
			    	if (parameter is bool)
			    	{
				    	SetBool(name, (bool)parameter);
			    	}
			    	else if (parameter is float)
			    	{
				    	SetFloat(name, (float)parameter);
			    	}
			    	else if (parameter is int)
			    	{
				    	SetInteger(name, (int)parameter);
			    	}
			    	else
			    	{
				    	SetTrigger(name);
			    	}
		    	}
		    	else if (_useAnimancer)
		    	{
			    	PlayAnimationFromSet(name, (int)parameter, dic, set);
			    	break;
		    	}
#else
		    	if (parameter is bool)
		    	{
		    		SetBool(name, (bool)parameter);
		    	}
		    	else if (parameter is float)
		    	{
		    		SetFloat(name, (float)parameter);
		    	}
		    	else if (parameter is int)
		    	{
		    		SetInteger(name, (int)parameter);
		    	}
		    	else
		    	{
		    		SetTrigger(name);
		    	}
#endif
	    	}    	
	    }
	    
	    public virtual void SetParameters(VP_AnimatorParameter[] _parameters, bool _useAnimancer = true)
	    {
	    	foreach (VP_AnimatorParameter _parameter in _parameters)
	    	{
	    		if (_parameter == null)
		    		continue;
	    		
		    	string name = _parameter.AnimationName;
		    	object parameter = _parameter.GetParameter();
		    	int dic = _parameter.GroupIndex;
		    	string set = _parameter.AnimationSet;
	    	
#if USE_ANIMANCER
		    	if (!_useAnimancer && (_Controller != null || _Controller.Controller != null))
		    	{
			    	if (parameter is bool)
			    	{
				    	SetBool(name, (bool)parameter);
			    	}
			    	else if (parameter is float)
			    	{
				    	SetFloat(name, (float)parameter);
			    	}
			    	else if (parameter is int)
			    	{
				    	SetInteger(name, (int)parameter);
			    	}
			    	else
			    	{
				    	SetTrigger(name);
			    	}
		    	}
		    	else if (_useAnimancer)
		    	{
			    	PlayAnimationFromSet(name, _parameter.MaskIndex, dic, set);
			    	break;
		    	}
#else
		    	if (parameter is bool)
		    	{
		    		SetBool(name, (bool)parameter);
		    	}
		    	else if (parameter is float)
		    	{
		    		SetFloat(name, (float)parameter);
		    	}
		    	else if (parameter is int)
		    	{
		    		SetInteger(name, (int)parameter);
		    	}
		    	else
		    	{
		    		SetTrigger(name);
		    	}
#endif
	    	}	    	
	    }
	    
	    public virtual void SetParameter(VP_AnimatorParameter _parameter, bool _useAnimancer = true)
	    {
	    	string name = _parameter.AnimationName;
	    	object parameter = _parameter.GetParameter();
	    	int dic = _parameter.GroupIndex;
		    string set = _parameter.AnimationSet;
	    	
#if USE_ANIMANCER
		    if (!_useAnimancer && (_Controller != null || _Controller.Controller != null))
		    {
			    if (parameter is bool)
			    {
				    SetBool(name, (bool)parameter);
			    }
			    else if (parameter is float)
			    {
				    SetFloat(name, (float)parameter);
			    }
			    else if (parameter is int)
			    {
				    SetInteger(name, (int)parameter);
			    }
			    else
			    {
				    SetTrigger(name);
			    }
		    }
		    else if (_useAnimancer)
		    {
		    	PlayAnimationFromSet(name, _parameter.MaskIndex, dic, set);
		    }
#else
		    if (parameter is bool)
		    {
		    	SetBool(name, (bool)parameter);
		    }
		    else if (parameter is float)
		    {
		    	SetFloat(name, (float)parameter);
		    }
		    else if (parameter is int)
		    {
		    	SetInteger(name, (int)parameter);
		    }
		    else
		    {
		    	SetTrigger(name);
		    }
#endif
	    }
	    
	    public virtual void SetParameter(string name, object parameter, int layer = -1, int dic = 0, string set = "Default", bool _useAnimancer = true)
	    {
#if USE_ANIMANCER
		    if (!_useAnimancer && (_Controller != null || _Controller.Controller != null))
		    {
			    if (parameter is bool)
			    {
				    SetBool(name, (bool)parameter);
			    }
			    else if (parameter is float)
			    {
				    SetFloat(name, (float)parameter);
			    }
			    else if (parameter is int)
			    {
				    SetInteger(name, (int)parameter);
			    }
			    else
			    {
				    SetTrigger(name);
			    }
		    }
		    else if (_useAnimancer)
		    {
		    	PlayAnimationFromSet(name, layer, dic, set);
		    }
#else
		    if (parameter is bool)
		    {
		    	SetBool(name, (bool)parameter);
		    }
		    else if (parameter is float)
		    {
		    	SetFloat(name, (float)parameter);
		    }
		    else if (parameter is int)
		    {
		    	SetInteger(name, (int)parameter);
		    }
		    else
		    {
		    	SetTrigger(name);
		    }
#endif
	    }
	    
	    public virtual void SetParameter<T>(string name, T parameter, int _layer = -1, int dic = 0, string set = "Default", bool _useAnimancer = true)
	    {
#if USE_ANIMANCER
		    if (!_useAnimancer && (_Controller != null || _Controller.Controller != null))
		    {
			    if (parameter is bool)
			    {
				    SetBool(name, (bool)System.Convert.ChangeType(parameter, typeof(bool)));
			    }
			    else if (parameter is float)
			    {
				    SetFloat(name, (float)System.Convert.ChangeType(parameter, typeof(float)));
			    }
			    else if (parameter is int)
			    {
				    SetInteger(name, (int)System.Convert.ChangeType(parameter, typeof(int)));
			    }
			    else
			    {
				    SetTrigger(name);
			    }
		    }
		    else if (_useAnimancer)
		    {
		    	PlayAnimationFromSet(name, _layer, dic, set);
		    }
#else
		    if (parameter is bool)
		    {
		    	SetBool(name, (bool)System.Convert.ChangeType(parameter, typeof(bool)));
		    }
		    else if (parameter is float)
		    {
		    	SetFloat(name, (float)System.Convert.ChangeType(parameter, typeof(float)));
		    }
		    else if (parameter is int)
		    {
		    	SetInteger(name, (int)System.Convert.ChangeType(parameter, typeof(int)));
		    }
		    else
		    {
		    	SetTrigger(name);
		    }
#endif
	    }
	    
	    public virtual float GetAnimationFadeDuration(string _animation, int _layer = 0, string _actionSet = "Default")
	    {
#if USE_ANIMANCER
		    if (m_animationSet.TryGetClipFromSet(_animation, _layer, out ClipState.Transition _transition, _actionSet))
		    {
                m_currentAnimationSet = _actionSet;

                return _transition.FadeDuration;
		    }
		    return 0.25f;
#else
		    // TODO CHECK BY ANIMATION NAME
		    return 0.25f;			
#endif
	    }
	    
	    #if USE_ANIMANCER
	    public virtual bool TryGetClip(string _animation, out Animancer.ClipState.Transition _transition, int _layer = 0, string _actionSet = "Default")
	    {
		    if (!m_animationSet)
		    {
			    VP_Debug.Log("No custom animations on " + name);
			    _transition = null;
			    return false;
		    }

		    return m_animationSet.TryGetClipFromSet(_animation, _layer, out _transition, _actionSet);
	    }
        #endif

#if UNITY_EDITOR
/// <summary>[Editor-Only]
/// Called by the Unity Editor when this component is first added (in Edit Mode) and whenever the Reset command
/// is executed from its context menu.
/// <para></para>
/// Sets <see cref="PlayAutomatically"/> = false by default so that <see cref="OnEnable"/> will play the
/// <see cref="Controller"/> instead of the first animation in the
/// <see cref="NamedAnimancerComponent.Animations"/> array.
/// </summary>

        protected override void Reset()
        {
#if USE_ANIMANCER
            base.Reset();

            if (Animator != null)
            {
                Controller = Animator.runtimeAnimatorController;
                Animator.runtimeAnimatorController = null;
            }

            PlayAutomatically = false;
#else
            m_animator = GetComponent<Animator>();
#endif
        }
#endif

	    public virtual void PlayAnimatorState(string _name, int _layer = -1, string layerName = "Base Layer")
	    {
#if USE_ANIMANCER
	    	
	    	if (Animator != null)
		    	Animator.Play($"{layerName}.{_name}", _layer, 0.25f);
		    else
			    PlayAnimationFromSet(_name, _layer);
#else
		    if (m_animator)
		    	m_animator.Play($"{layerName}.{_name}", _layer, 0.25f);
#endif
	    }

#if USE_ANIMANCER
        /************************************************************************************************************************/
	    public virtual AnimancerState SetNewSetAndPlay(ref VP_AnimationSetResources set, string _animationKey, int layer = -1, int dicIndex = 0, string _animationSetID = "Default", float _fadeSpeed = -1f)
        {
            m_animationSet = set;

            if (m_animationSet == null || _animationKey.IsNullOrEmpty())
                return null;

            var clip = m_animationSet.GetAnimationInLayer(_animationSetID, dicIndex, _animationKey);
            if (clip == null)
                return null;

            if (_fadeSpeed == -1)
                _fadeSpeed = clip.FadeDuration;

            m_currentAnimationSet = _animationSetID;

	        return layer >= 0 ? Layers[layer].Play(clip, _fadeSpeed) : Play(clip, _fadeSpeed);
        }

        public virtual AnimancerState PlayAnimationInLayer(Animancer.ClipState.Transition _animationTransition, int layer = -1, float _fadeSpeed = -1f)
        {
            var clip = _animationTransition;
            if (clip == null)
                return null;

            if (_fadeSpeed == -1)
                _fadeSpeed = clip.FadeDuration;

            return layer >= 0 ? Layers[layer].Play(clip, _fadeSpeed) : Play(clip, _fadeSpeed);
        }
        
	    public virtual AnimancerState PlayAnimationFromSet(string _animationKey, int layer = -1, int dicIndex = 0, string _animationSetID = "Default", float _fadeSpeed = -1f)
        {
            if (m_animationSet == null || _animationKey.IsNullOrEmpty())
                return null;

            m_currentAnimationSet = _animationSetID;

            var clip = m_animationSet.GetAnimationInLayer(_animationSetID, dicIndex, _animationKey);
            if (clip == null)
                return null;

            if (_fadeSpeed == -1)
                _fadeSpeed = clip.FadeDuration;

	        return layer >= 0 ? Layers[layer].Play(clip, _fadeSpeed) : Play(clip, _fadeSpeed);
        }

        public virtual void ForceAnimationFromSet(string key)
	    {
		    PlayAnimationFromSet(key);
	    }

	    public virtual void ForceAnimationFromSetInMouth(string key)
	    {
		    PlayAnimationFromSet(key, 1);
	    }

        public virtual AnimancerState PlayAnimationFromResources(ResourceReference.VP_AnimationTransitionResources set, string _animationKey, int layer = -1, int dicIndex = 0, float _fadeSpeed = -1f)
        {
            if (set == null || _animationKey.IsNullOrEmpty())
                return null;

            var clip = set.GetClip(dicIndex, _animationKey);
            if (clip == null)
            {
                Debug.Log("Clip not found with key " + _animationKey);

                return null;
            }

            if (_fadeSpeed == -1)
                _fadeSpeed = clip.FadeDuration;

	        return layer >= 0 ? Layers[layer].Play(clip, _fadeSpeed) : Play(clip, _fadeSpeed);
        }

	    public virtual AnimancerState PlayAnimationFromResources(ref ResourceReference.VP_AnimationTransitionResources set, string _animationKey, int layer = -1, int dicIndex = 0, float _fadeSpeed = -1f)
        {
            if (set == null || _animationKey.IsNullOrEmpty())
                return null;

            var clip = set.GetClip(dicIndex, _animationKey);
            if (clip == null)
                return null;

            if (_fadeSpeed == -1)
                _fadeSpeed = clip.FadeDuration;

	        return layer >= 0 ? Layers[layer].Play(clip, _fadeSpeed) : Play(clip, _fadeSpeed);
        }

	    public virtual bool HasCurrentAnimationComparedID(string ID, int layer, string keyset = "Default")
	    {
		    if (!IsPlaying())
			    return false;
			    
		    var _state = Layers[layer].CurrentState;

            m_currentAnimationSet = keyset;

            return m_animationSet.GetIDFromClip(_state.Clip, layer, keyset) == ID;
	    }

	    public virtual string CurrentAnimationID(int layer, string keyset = "Default")
	    {
		    if (!IsPlaying())
			    return "";
			    
		    var _state = Layers[layer].CurrentState;

            m_currentAnimationSet = keyset;

            return m_animationSet.GetIDFromClip(_state.Clip, layer, keyset);
	    }

	    public virtual string CurrentAnimationClipName(int layer)
	    {
		    if (!IsPlaying())
			    return "";
			    
		    var _state = Layers[layer].CurrentState;  
		    
		    return _state.Clip.name;
	    }

        public virtual bool HasEvents(int layer, out AnimancerState _state)
	    {
		    _state = Layers[layer].CurrentState;

		    if (!IsPlaying())
			    return false;
			
		    return _state.HasEvents;
	    }

        public virtual void StartDialogSequenceStandUp()
	    {
		    AnimancerState state = PlayAnimationFromSet(VP_AnimationSetup.NPC.START_TALK);
		    state.Events.OnEnd = () =>
		    {
			    var state2 = PlayAnimationFromSet(VP_AnimationSetup.NPC.TALK);
		    };
	    }

        public virtual void EndDialogSequenceStandUp(string _afterEnd)
	    {
		    var state3 = PlayAnimationFromSet(VP_AnimationSetup.NPC.END_TALK);
		    state3.Events.OnEnd = () => PlayAnimationFromSet(_afterEnd);
	    }

        public virtual void PlayDialogSequenceStandUp(string _afterEnd)
	    {
		    AnimancerState state = PlayAnimationFromSet(VP_AnimationSetup.NPC.START_TALK);
		    state.Events.OnEnd = () =>
		    {
			    var state2 = PlayAnimationFromSet(VP_AnimationSetup.NPC.TALK);
			    state2.Events.OnEnd = () =>
			    {
				    var state3 = PlayAnimationFromSet(VP_AnimationSetup.NPC.END_TALK);
				    state3.Events.OnEnd = () => PlayAnimationFromSet(_afterEnd);
			    };
		    };
	    }

        public virtual void PlayDialogSequenceSit(string _afterEnd)
	    {
		    AnimancerState state = PlayAnimationFromSet(VP_AnimationSetup.NPC.START_TALK_SIT);
		    state.Events.OnEnd = () =>
		    {
			    var state2 = PlayAnimationFromSet(VP_AnimationSetup.NPC.TALK_SIT);
			    state2.Events.OnEnd = () =>
			    {
				    var state3 = PlayAnimationFromSet(VP_AnimationSetup.NPC.END_TALK_SIT);
				    state3.Events.OnEnd = () => PlayAnimationFromSet(_afterEnd);
			    };
		    };
	    }

        /************************************************************************************************************************/

        /// <summary>
        /// Called by Unity when this component becomes enabled and active.
        /// <para></para>
        /// Plays the <see cref="Controller"/> if <see cref="PlayAutomatically"/> is false (otherwise it plays the
        /// first animation in the <see cref="NamedAnimancerComponent.Animations"/> array).
        /// </summary>
        protected override void OnEnable()
        {
            PlayController();
            base.OnEnable();
        }

        /************************************************************************************************************************/

	    public virtual bool HasController()
	    {
	    	return _Controller != null && _Controller.Controller != null;
	    }

        public override void GatherAnimationClips(ICollection<AnimationClip> clips)
        {
            base.GatherAnimationClips(clips);

            if (_Controller != null &&
                _Controller.Controller != null)
                clips.Gather(_Controller.Controller.animationClips);
        }
#endif
        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
        #region Animator Controller Wrappers
        /************************************************************************************************************************/
#if USE_ANIMANCER
        /// <summary>
        /// Transitions to the <see cref="Controller"/> over its specified
        /// <see cref="AnimancerState.Transition{TState}.FadeDuration"/>.
        /// <para></para>
        /// Returns the <see cref="AnimancerState.Transition{TState}.State"/>.
        /// </summary>
        public virtual ControllerState PlayController()
        {
            if (_Controller == null || _Controller.Controller == null)
            {
	            //	Debug.Log("Controller State is Null");
            	return null;
            }
            
            // Don't just return the result of Transition because it is an AnimancerState which we would need to cast.
            Play(_Controller);
            return _Controller.State;
        }
#endif
        /************************************************************************************************************************/
        #region Cross Fade
        /************************************************************************************************************************/
#if USE_ANIMANCER
        /// <summary>
        /// Starts a transition from the current state to the specified state using normalized times.
        /// </summary>
        public void CrossFade(int stateNameHash,
            float transitionDuration = AnimancerPlayable.DefaultFadeDuration,
            int layer = -1,
            float normalizedTime = float.NegativeInfinity)
        {
            var controllerState = PlayController();
            controllerState.Playable.CrossFade(stateNameHash, transitionDuration, layer, normalizedTime);
        }
        /************************************************************************************************************************/

        /// <summary>
        /// Starts a transition from the current state to the specified state using normalized times.
        /// </summary>
        public AnimancerState CrossFade(string stateName,
            float transitionDuration = AnimancerPlayable.DefaultFadeDuration,
            int layer = -1,
            float normalizedTime = float.NegativeInfinity)
        {
            if (States.TryGet(name, out var state))
            {
                Play(state, transitionDuration);

                if (layer >= 0)
                    state.LayerIndex = layer;

                if (normalizedTime != float.NegativeInfinity)
                    state.NormalizedTime = normalizedTime;

                return state;
            }
            else
            {
                var controllerState = PlayController();
                controllerState.Playable.CrossFade(stateName, transitionDuration, layer, normalizedTime);
                return controllerState;
            }
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Starts a transition from the current state to the specified state using times in seconds.
        /// </summary>
        public void CrossFadeInFixedTime(int stateNameHash,
            float transitionDuration = AnimancerPlayable.DefaultFadeDuration,
            int layer = -1,
            float fixedTime = 0)
        {
            var controllerState = PlayController();
            controllerState.Playable.CrossFadeInFixedTime(stateNameHash, transitionDuration, layer, fixedTime);
        }


        /************************************************************************************************************************/

        /// <summary>
        /// Starts a transition from the current state to the specified state using times in seconds.
        /// </summary>
        public AnimancerState CrossFadeInFixedTime(string stateName,
            float transitionDuration = AnimancerPlayable.DefaultFadeDuration,
            int layer = -1,
            float fixedTime = 0)
        {
            if (States.TryGet(name, out var state))
            {
                Play(state, transitionDuration);

                if (layer >= 0)
                    state.LayerIndex = layer;

                state.Time = fixedTime;

                return state;
            }
            else
            {
                var controllerState = PlayController();
                controllerState.Playable.CrossFadeInFixedTime(stateName, transitionDuration, layer, fixedTime);
                return controllerState;
            }
        }

#endif
        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
        #region Play
        /************************************************************************************************************************/
#if USE_ANIMANCER
        /// <summary>[Coroutine]
        /// Plays each clip in the <see cref="Animations"/> array one after the other. Mainly useful for testing and
        /// showcasing purposes.
        /// </summary>
        public virtual IEnumerator PlayAnimationsInSequence(ClipState.Transition[] _animations)
	    {
		    for (int i = 0; i < _animations.Length; i++)
		    {
			    var state = Play(_animations[i]);

			    if (state != null)
				    yield return state;
		    }

		    Stop();
	    }
#endif


        /// <summary>
        /// Plays the specified state immediately, starting from a particular normalized time.
        /// </summary>
        public void Play(int stateNameHash,
            int layer = -1,
            float normalizedTime = float.NegativeInfinity)
        {
#if USE_ANIMANCER
            var controllerState = PlayController();
            controllerState.Playable.Play(stateNameHash, layer, normalizedTime);
#endif
        }

        /************************************************************************************************************************/
#if USE_ANIMANCER

        /// <summary>
        /// Plays the specified state immediately, starting from a particular normalized time.
        /// </summary>
        public AnimancerState Play(string stateName,
            int layer = -1,
            float normalizedTime = float.NegativeInfinity)
        {
            if (States.TryGet(name, out var state))
            {
                Play(state);

                if (layer >= 0)
                    state.LayerIndex = layer;

                if (normalizedTime != float.NegativeInfinity)
                    state.NormalizedTime = normalizedTime;

                return state;
            }
            else
            {
                var controllerState = PlayController();
                controllerState.Playable.Play(stateName, layer, normalizedTime);
                return controllerState;
            }
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Plays the specified state immediately, starting from a particular time (in seconds).
        /// </summary>
        public void PlayInFixedTime(int stateNameHash,
            int layer = -1,
            float fixedTime = 0)
        {
            var controllerState = PlayController();
            controllerState.Playable.PlayInFixedTime(stateNameHash, layer, fixedTime);
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Plays the specified state immediately, starting from a particular time (in seconds).
        /// </summary>
        public AnimancerState PlayInFixedTime(string stateName,
            int layer = -1,
            float fixedTime = 0)
        {
            if (States.TryGet(name, out var state))
            {
                Play(state);

                if (layer >= 0)
                    state.LayerIndex = layer;

                state.Time = fixedTime;

                return state;
            }
            else
            {
                var controllerState = PlayController();
                controllerState.Playable.PlayInFixedTime(stateName, layer, fixedTime);
                return controllerState;
            }
        }
#endif
        /************************************************************************************************************************/
#endregion
        /************************************************************************************************************************/
#region Parameters
        /************************************************************************************************************************/
#if USE_ANIMANCER

        /// <summary>Gets the value of the specified boolean parameter.</summary>
        public bool GetBool(int id) => _Controller.State.Playable.GetBool(id);
        /// <summary>Gets the value of the specified boolean parameter.</summary>
        public bool GetBool(string name) => _Controller.State.Playable.GetBool(name);
        /// <summary>Sets the value of the specified boolean parameter.</summary>
        public void SetBool(int id, bool value) => _Controller.State.Playable.SetBool(id, value);
        /// <summary>Sets the value of the specified boolean parameter.</summary>
        public void SetBool(string name, bool value) => _Controller.State.Playable.SetBool(name, value);

        /// <summary>Gets the value of the specified float parameter.</summary>
        public float GetFloat(int id) => _Controller.State.Playable.GetFloat(id);
        /// <summary>Gets the value of the specified float parameter.</summary>
        public float GetFloat(string name) => _Controller.State.Playable.GetFloat(name);
        /// <summary>Sets the value of the specified float parameter.</summary>
        public void SetFloat(int id, float value) => _Controller.State.Playable.SetFloat(id, value);
        /// <summary>Sets the value of the specified float parameter.</summary>
        public void SetFloat(string name, float value) => _Controller.State.Playable.SetFloat(name, value);

        /// <summary>Gets the value of the specified integer parameter.</summary>
        public int GetInteger(int id) => _Controller.State.Playable.GetInteger(id);
        /// <summary>Gets the value of the specified integer parameter.</summary>
        public int GetInteger(string name) => _Controller.State.Playable.GetInteger(name);
        /// <summary>Sets the value of the specified integer parameter.</summary>
        public void SetInteger(int id, int value) => _Controller.State.Playable.SetInteger(id, value);
        /// <summary>Sets the value of the specified integer parameter.</summary>
        public void SetInteger(string name, int value) => _Controller.State.Playable.SetInteger(name, value);

        /// <summary>Sets the specified trigger parameter to true.</summary>
        public void SetTrigger(int id) => _Controller.State.Playable.SetTrigger(id);
        /// <summary>Sets the specified trigger parameter to true.</summary>
        public void SetTrigger(string name) => _Controller.State.Playable.SetTrigger(name);
        /// <summary>Resets the specified trigger parameter to false.</summary>
        public void ResetTrigger(int id) => _Controller.State.Playable.ResetTrigger(id);
        /// <summary>Resets the specified trigger parameter to false.</summary>
        public void ResetTrigger(string name) => _Controller.State.Playable.ResetTrigger(name);

        /// <summary>Gets the details of one of the <see cref="Controller"/>'s parameters.</summary>
        public AnimatorControllerParameter GetParameter(int index) => _Controller.State.Playable.GetParameter(index);
        /// <summary>Gets the number of parameters in the <see cref="Controller"/>.</summary>
        public int GetParameterCount() => _Controller.State.Playable.GetParameterCount();

        /// <summary>Indicates whether the specified parameter is controlled by an <see cref="AnimationClip"/>.</summary>
        public bool IsParameterControlledByCurve(int id) => _Controller.State.Playable.IsParameterControlledByCurve(id);
        /// <summary>Indicates whether the specified parameter is controlled by an <see cref="AnimationClip"/>.</summary>
        public bool IsParameterControlledByCurve(string name) => _Controller.State.Playable.IsParameterControlledByCurve(name);

#else
 /// <summary>Gets the value of the specified boolean parameter.</summary>
        public virtual bool GetBool(int id) => m_animator.GetBool(id);
        /// <summary>Gets the value of the specified boolean parameter.</summary>
        public virtual bool GetBool(string name) => m_animator.GetBool(name);
        /// <summary>Sets the value of the specified boolean parameter.</summary>
        public virtual void SetBool(int id, bool value) => m_animator.SetBool(id, value);
        /// <summary>Sets the value of the specified boolean parameter.</summary>
        public virtual void SetBool(string name, bool value) => m_animator.SetBool(name, value);

        /// <summary>Gets the value of the specified float parameter.</summary>
        public virtual float GetFloat(int id) => m_animator.GetFloat(id);
        /// <summary>Gets the value of the specified float parameter.</summary>
        public virtual float GetFloat(string name) => m_animator.GetFloat(name);
        /// <summary>Sets the value of the specified float parameter.</summary>
        public virtual void SetFloat(int id, float value) => m_animator.SetFloat(id, value);
        /// <summary>Sets the value of the specified float parameter.</summary>
        public virtual void SetFloat(string name, float value) => m_animator.SetFloat(name, value);

        /// <summary>Gets the value of the specified integer parameter.</summary>
        public virtual int GetInteger(int id) => m_animator.GetInteger(id);
        /// <summary>Gets the value of the specified integer parameter.</summary>
        public virtual int GetInteger(string name) => m_animator.GetInteger(name);
        /// <summary>Sets the value of the specified integer parameter.</summary>
        public virtual void SetInteger(int id, int value) => m_animator.SetInteger(id, value);
        /// <summary>Sets the value of the specified integer parameter.</summary>
        public virtual void SetInteger(string name, int value) => m_animator.SetInteger(name, value);

        /// <summary>Sets the specified trigger parameter to true.</summary>
        public virtual void SetTrigger(int id) => m_animator.SetTrigger(id);
        /// <summary>Sets the specified trigger parameter to true.</summary>
        public virtual void SetTrigger(string name) => m_animator.SetTrigger(name);
        /// <summary>Resets the specified trigger parameter to false.</summary>
        public virtual void ResetTrigger(int id) => m_animator.ResetTrigger(id);
        /// <summary>Resets the specified trigger parameter to false.</summary>
        public virtual void ResetTrigger(string name) => m_animator.ResetTrigger(name);
#endif
        /************************************************************************************************************************/
#endregion
        /************************************************************************************************************************/
#region Misc
        /************************************************************************************************************************/
        // Layers.
        /************************************************************************************************************************/
#if USE_ANIMANCER

        /// <summary>Gets the weight of the layer at the specified index.</summary>
        public float GetLayerWeight(int layerIndex) => _Controller.State.Playable.GetLayerWeight(layerIndex);
        /// <summary>Sets the weight of the layer at the specified index.</summary>
        public void SetLayerWeight(int layerIndex, float weight) => _Controller.State.Playable.SetLayerWeight(layerIndex, weight);

        /// <summary>Gets the number of layers in the <see cref="Controller"/>.</summary>
        public int GetLayerCount() => _Controller.State.Playable.GetLayerCount();

        /// <summary>Gets the index of the layer with the specified name.</summary>
        public int GetLayerIndex(string layerName) => _Controller.State.Playable.GetLayerIndex(layerName);
        /// <summary>Gets the name of the layer with the specified index.</summary>
        public string GetLayerName(int layerIndex) => _Controller.State.Playable.GetLayerName(layerIndex);

        /************************************************************************************************************************/
        // States.
        /************************************************************************************************************************/

        /// <summary>Returns information about the current state.</summary>
        public AnimatorStateInfo GetCurrentAnimatorStateInfo(int layerIndex = 0) => _Controller.State.Playable.GetCurrentAnimatorStateInfo(layerIndex);
        /// <summary>Returns information about the next state being transitioned towards.</summary>
        public AnimatorStateInfo GetNextAnimatorStateInfo(int layerIndex = 0) => _Controller.State.Playable.GetNextAnimatorStateInfo(layerIndex);

        /// <summary>Indicates whether the specified layer contains the specified state.</summary>
        public bool HasState(int layerIndex, int stateID) => _Controller.State.Playable.HasState(layerIndex, stateID);

        /************************************************************************************************************************/
        // Transitions.
        /************************************************************************************************************************/

        /// <summary>Indicates whether the specified layer is currently executing a transition.</summary>
        public bool IsInTransition(int layerIndex = 0) => _Controller.State.Playable.IsInTransition(layerIndex);

        /// <summary>Gets information about the current transition.</summary>
        public AnimatorTransitionInfo GetAnimatorTransitionInfo(int layerIndex = 0) => _Controller.State.Playable.GetAnimatorTransitionInfo(layerIndex);

        /************************************************************************************************************************/
        // Clips.
        /************************************************************************************************************************/

        /// <summary>Gets information about the <see cref="AnimationClip"/>s currently being played.</summary>
        public AnimatorClipInfo[] GetCurrentAnimatorClipInfo(int layerIndex = 0) => _Controller.State.Playable.GetCurrentAnimatorClipInfo(layerIndex);
        /// <summary>Gets information about the <see cref="AnimationClip"/>s currently being played.</summary>
        public void GetCurrentAnimatorClipInfo(int layerIndex, List<AnimatorClipInfo> clips) => _Controller.State.Playable.GetCurrentAnimatorClipInfo(layerIndex, clips);
        /// <summary>Gets the number of <see cref="AnimationClip"/>s currently being played.</summary>
        public int GetCurrentAnimatorClipInfoCount(int layerIndex = 0) => _Controller.State.Playable.GetCurrentAnimatorClipInfoCount(layerIndex);

        /// <summary>Gets information about the <see cref="AnimationClip"/>s currently being transitioned towards.</summary>
        public AnimatorClipInfo[] GetNextAnimatorClipInfo(int layerIndex = 0) => _Controller.State.Playable.GetNextAnimatorClipInfo(layerIndex);
        /// <summary>Gets information about the <see cref="AnimationClip"/>s currently being transitioned towards.</summary>
        public void GetNextAnimatorClipInfo(int layerIndex, List<AnimatorClipInfo> clips) => _Controller.State.Playable.GetNextAnimatorClipInfo(layerIndex, clips);
        /// <summary>Gets the number of <see cref="AnimationClip"/>s currently being transitioned towards.</summary>
        public int GetNextAnimatorClipInfoCount(int layerIndex = 0) => _Controller.State.Playable.GetNextAnimatorClipInfoCount(layerIndex);

#endif
        /************************************************************************************************************************/
#endregion
        /************************************************************************************************************************/
#endregion
        /************************************************************************************************************************/
    }
}