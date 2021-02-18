using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#if USE_ANIMANCER
using Animancer;
#endif

namespace VirtualPhenix.Dialog
{
    public class VP_DialogAnimator : VP_MonoBehaviour
    {
        public enum ANIMATION_BASE
        {
            NONE,
            ANIMATE_ALWAYS_WITHOUT_CHARACTER,
            ANIMATE_WITHOUT_CHARACTER,
            ON_CHARACTER,
            ON_CHARACTER_NAME
        }

        public enum EndAnimation
        {
            OnTextShown,
            OnTextContinue,
            OnTextEnd,
            Never
        }

        [Header("Animation")]
        /// <summary>
        /// Animator component
        /// </summary>
        [SerializeField] protected VP_Animator m_animator;

#if USE_ANIMANCER
        [SerializeField] protected bool m_useAnimancer;
#endif

        [SerializeField] protected bool m_speaking = false;
        /// <summary>
        /// You need to set this true manually for ANIMATE_WITHOUT_CHARACTER to work
        /// </summary>
        protected bool m_speakRefresh = false;

        [SerializeField] protected bool m_useFullTransition = false;
        [SerializeField] protected string m_idleAnim = "IDLE";
        [SerializeField] protected string m_startTalkingAnim = "START_TALK";
        [SerializeField] protected string m_talkingAnim = "TALK";
        [SerializeField] protected string m_endTalkingAnim = "END_TALK";
        [SerializeField] protected bool m_useMouth = false;
        [SerializeField] protected string m_idleMuthAnim = "MOUTH_IDLE";
        [SerializeField] protected string m_talkMuthAnim = "MOUTH_TALK";

        [SerializeField] protected EndAnimation m_endBody = EndAnimation.OnTextEnd;
        [SerializeField] protected EndAnimation m_endMouth = EndAnimation.OnTextShown;
        [SerializeField] protected EndAnimation m_considerEnd = EndAnimation.OnTextEnd;

        [SerializeField] protected UnityEvent m_event;


        [Header("Character")]
        /// <summary>
        /// Character related-> When this character speaks, the animator will play
        /// </summary>
        [SerializeField] protected VP_DialogCharacterData m_character = null;
        /// <summary>
        /// It starts if the character is the same file, just the name...
        /// </summary>
        [SerializeField] protected ANIMATION_BASE m_animationCheck = ANIMATION_BASE.ON_CHARACTER;
        /// <summary>
        /// Target designed mainly for IK
        /// </summary>
        [SerializeField] protected Transform m_target = null;

#if USE_ANIMANCER
        AnimancerState m_state;
        AnimancerState m_stateMouth;
#endif

        protected override void Initialize()
        {
            base.Initialize();

            if (m_animationCheck != ANIMATION_BASE.NONE)
            {
                   transform.TryGetComponentInChildren<VP_Animator>(out m_animator);
            }
          

            StopMouthAnimations();
            StopBodyAnimations();

        }

        protected void Reset()
        {
            m_initializationTime = InitializationTime.OnAwake;
            m_startListeningTime = StartListenTime.OnEnable;
            m_stopListeningTime = StopListenTime.OnDisable;
        }

        protected override void StartAllListeners()
        {
            base.StartAllListeners();
            VP_DialogManager.StartListeningToOnCharacterSpeak(OnCharacterSpeaking);
            VP_DialogManager.StartListeningToOnTextShown(OnTextShown);
            VP_DialogManager.StartListeningToOnDialogComplete(OnTextContinued);
            VP_DialogManager.StartListeningToOnDialogEnd(OnEndText);
        }

        protected override void StopAllListeners()
        {
            base.StartAllListeners();
            VP_DialogManager.StopListeningToOnCharacterSpeak(OnCharacterSpeaking);
            VP_DialogManager.StopListeningToOnTextShown(OnTextShown);
            VP_DialogManager.StopListeningToOnTextShown(OnTextContinued);
            VP_DialogManager.StopListeningToOnDialogEnd(OnEndText);
        }

        protected virtual bool IsCharacter(VP_DialogCharacterData _character)
        {
            if (m_animationCheck != ANIMATION_BASE.NONE)
            {
                if (_character && m_character)
                {
                    switch (m_animationCheck)
                    {
                        case ANIMATION_BASE.ON_CHARACTER:
                            if (m_character == _character)
                            {
                                return true;
                            }
                            break;
                        case ANIMATION_BASE.ON_CHARACTER_NAME:
                            if (m_character.characterName == _character.characterName)
                            {
                                return true;
                            }
                            break;
                    }
                }
                else if (_character && !m_character)
                {
                    switch (m_animationCheck)
                    {
                        case ANIMATION_BASE.ANIMATE_ALWAYS_WITHOUT_CHARACTER:
                            return true;
                        case ANIMATION_BASE.ANIMATE_WITHOUT_CHARACTER:
                            if (m_speakRefresh)
                            {
                                // it means it is not us, so no more refresh
                                m_speakRefresh = false;
                                OnTextShown();
                            }
                            break;
                    }
                }
                else
                {
                    switch (m_animationCheck)
                    {
                        case ANIMATION_BASE.ANIMATE_ALWAYS_WITHOUT_CHARACTER:
                            return true;
                        case ANIMATION_BASE.ANIMATE_WITHOUT_CHARACTER:
                            if (m_speakRefresh)
                            {
                                return true;
                            }
                            break;
                    }
                }
            }

            return false;
        }


        public virtual void StopMouthAnimations()
        {
#if USE_ANIMANCER
            if (m_useAnimancer)
                m_stateMouth = m_animator.PlayAnimationFromSet(m_idleMuthAnim, 1);
#endif
        }

        public virtual void StopBodyAnimations()
        {
#if USE_ANIMANCER
            if (m_useAnimancer)
            {
                if (m_useFullTransition)
                {

                    m_state = m_animator.PlayAnimationFromSet(m_endTalkingAnim);
                    m_state.Events.OnEnd = () =>
                    {

                        m_state = m_animator.PlayAnimationFromSet(m_idleAnim);
                    };
                }
                else
                {
                    m_state = m_animator.PlayAnimationFromSet(m_idleAnim);
                }
            }
            else
            {
                m_animator.SetBool(m_talkingAnim, false);
            }
#else
            m_animator.SetBool(m_talkingAnim, false);
#endif

        }


        public void OnTextContinued()
        {
            if (m_speaking)
            {
                if (m_considerEnd == EndAnimation.OnTextContinue)
                    m_speaking = false;

                if (m_endMouth == EndAnimation.OnTextContinue)
                    StopMouthAnimations();


                if (m_endBody == EndAnimation.OnTextContinue)
                    StopBodyAnimations();
            }
        }

       
        protected virtual void OnCharacterSpeaking(VP_DialogCharacterData _character)
        {
            if (IsCharacter(_character))
            {
                if (!m_speaking)
                {
                    m_speaking = true;

                    if (m_animator)
                    {
#if USE_ANIMANCER
                        if (m_useAnimancer)
                        {
                            if (!m_speaking)
                            {
                                if (m_useFullTransition)
                                {
                                    m_state = m_animator.PlayAnimationFromSet(m_startTalkingAnim);
                                    m_state.Events.OnEnd = () =>
                                    {
                                        m_stateMouth = m_animator.PlayAnimationFromSet(m_talkMuthAnim, 1);
                                        m_state = m_animator.PlayAnimationFromSet(m_talkingAnim);
                                    };
                                }
                                else
                                {
                                    m_stateMouth = m_animator.PlayAnimationFromSet(m_talkMuthAnim, 1);
                                    m_state = m_animator.PlayAnimationFromSet(m_talkingAnim);
                                }
                            }
                            else
                            {
                                m_stateMouth = m_animator.PlayAnimationFromSet(m_talkMuthAnim, 1);
                                m_state = m_animator.PlayAnimationFromSet(m_talkingAnim);
                            }
                        }
                        else
                        {
                            if (m_useFullTransition)
                            {
                                m_animator.SetTrigger(m_startTalkingAnim);
                                m_animator.SetBool(m_talkingAnim, true);
                            }
                            else
                            {
                                m_animator.SetBool(m_talkingAnim, true);
                            }
                        }
#else
                        m_animator.SetBool(m_talkingAnim, true);
#endif
                    }

                    if (m_target != null)
                    {
                        VP_DialogManager.OnAnimationTargetAction(m_target);
                    }
                }
                else
                {
                    // TODO: What do I do if I'm already speaking
                }
            }
        }

        protected virtual void OnTextShown()
        {
            if (m_speaking)
            {
                if (m_considerEnd == EndAnimation.OnTextShown)
                    m_speaking = false;

                if (m_endMouth == EndAnimation.OnTextEnd)
                    StopMouthAnimations();

                if (m_endBody == EndAnimation.OnTextEnd)
                    StopBodyAnimations();
            }
        }

        protected virtual void OnEndText()
        {
            //	Debug.Log("end text");
            if (m_speaking)
            {
                m_speaking = false;

                if (m_endMouth == EndAnimation.OnTextEnd)
                    StopMouthAnimations();

                if (m_endBody == EndAnimation.OnTextEnd)
                    StopBodyAnimations();
            }
        }
    }

}
