#if USE_BEHAVIOR_DESIGNER
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VirtualPhenix;
using VirtualPhenix.Dialog;
using VirtualPhenix.Dialog.Demo;

namespace VirtualPhenix.Integrations.BehaviorDesignerTree
{
	[TaskDescription("Triggers dialogue using Phoenix Dialogue System.")]
	[TaskCategory(VP_PrefixSetup.MAIN_PREFIX+"/Dialogue")]
	[TaskIcon("Assets/VirtualPhenix/Editor/Icons/Behavior Designer/DialogueSystemIcon.png")]
    public class Dialog : Action
    {
        [BehaviorDesigner.Runtime.Tasks.Tooltip("The conversation to start")]
        [TextArea] public SharedString keyOrText;
        [BehaviorDesigner.Runtime.Tasks.Tooltip("Is key or direct message")]
        public SharedBool isKey;

	    public SharedVPDialogChart m_chart;
	    public SharedAudioClip2 m_audioClip;
	    public SharedVPDialogMessage m_customMessage;
	    public SharedDialogAnswerList m_answers;
	    public SharedDialogType m_dialogType;
	    public SharedBool m_isAutomatic = true;
	    public SharedBool m_skippable = true;
	    public SharedBool m_soundOnContinue = true;
	    public SharedBool m_waitForInput = true;
	    public SharedBool m_waitForAudioEnd = true;
	    public SharedFloat m_textSpeed = 5f;
	    public SharedFloat m_fadeDuration = 0.25f;
	    public SharedFloat m_fontSize = 45f;
	    public SharedFloat m_timeForAutoAnswer = 5f;
	    public SharedFloat m_automaticScreenTime = 1f;
	    public SharedBool m_useTranslation = false;
	    public SharedBool m_showDirectly = false;
	    public SharedBool m_fadeInOut = true;
	    public SharedBool m_showAnswersAtTheSameTime = true;
	    public SharedBool m_canCancel = true;
	    public SharedBool m_setFontSize = false;
	    public SharedBool m_overrideTextColor = false;
	    public SharedBool m_chooseNumber = false;
	    public SharedColor m_newTextColor;
	    public SharedInt m_autoAnswer = -1;
	    public SharedVector3 m_parameters;
	    public SharedTMPFont m_newFont;
	    public SharedVPDialogPositionData m_positionData;
	    public SharedVPCharacterData m_characterData;
	    public SharedVPAnimator m_animator;
	    
        // The return status of the conversation after it has finished executing.
        protected TaskStatus status;

	    [BehaviorDesigner.Runtime.Tasks.Tooltip("Use Animations")]
	    public SharedBool m_useAnimations = true;
		
#if USE_ANIMANCER		
	    [BehaviorDesigner.Runtime.Tasks.Tooltip("Use Animancer")]
	    public SharedBool m_useAnimancer = true;
		
	    [BehaviorDesigner.Runtime.Tasks.Tooltip("Animation Set")]
	    public SharedString m_animationSet = "Default";
#endif


	    public SharedBool m_useAllTransition = true;
	    public SharedString m_startTalkingAnim = "START_TALK";
	    public SharedString m_talkingAnim = "TALK";
	    public SharedString m_endTalkingAnim = "END_TALK";
	    public SharedString m_idleAnim = "IDLE";
	    public SharedString m_mouthIdle = "MOUTH_IDLE";
	    public SharedString m_mouthTalk = "MOUTH_TALK";
	    public SharedBool m_useMouth = false;
	    public SharedInt m_mouthLayer = 1;
	    public SharedInt m_faceLayer = 0;

        public override void OnStart()
        {
	        SetAnimatorParameter(m_idleAnim.Value, "", m_faceLayer.Value);
	        
	        if (m_useMouth.Value)
	        {
		        SetAnimatorParameter(m_mouthIdle.Value, "", m_mouthLayer.Value);
	        }
	        
	        status = TaskStatus.Running;
            
            if (keyOrText.Value == null)
                keyOrText.Value = string.Empty;

            if (isKey.Value)
            {
	            VP_DialogManager.SendDialogMessage(keyOrText.Value, m_chart.Value, OnDialogEnd, m_customMessage.Value);
            }
            else
            {
	            VP_DialogManager.ShowDirectMessage(keyOrText.Value, m_audioClip.Value, m_dialogType.Value, m_useTranslation.Value, m_showDirectly.Value, m_fadeInOut.Value,m_customMessage.Value,
		            OnComplete,OnDialogStart,OnDialogEnd,OnTextShown, m_positionData.Value, m_skippable.Value, m_waitForInput.Value, m_fadeDuration.Value, m_soundOnContinue.Value, m_characterData.Value,
		            m_textSpeed.Value,m_waitForAudioEnd.Value,m_answers.Value, OnAnswerChosen, m_showAnswersAtTheSameTime.Value, m_autoAnswer.Value, m_overrideTextColor.Value, m_newTextColor.Value,m_newFont.Value,
		            m_fontSize.Value, m_timeForAutoAnswer.Value, m_chooseNumber.Value, m_parameters.Value,m_canCancel.Value, m_automaticScreenTime.Value, OnCommandChosen,OnCancel);
	            
            }
        }
        
	    protected virtual void OnCommandChosen(int _index)
	    {
	    	
	    }
        
	    protected virtual void OnAnswerChosen(int _index)
	    {
	    	
	    }
        
        
	    protected virtual void OnCancel()
	    {
	    	
	    }
	    
	    protected virtual void OnTextShown()
	    {
	    	
	    }
        
	    protected virtual void OnComplete()
	    {
	    	
	    }
        
	    protected virtual void StopAnimation()
	    {
	    	if (!m_useAllTransition.Value)
	    	{
		    	SetAnimatorParameter(m_idleAnim.Value, "", m_faceLayer.Value);
	        
		    	if (m_useMouth.Value)
		    	{
			    	SetAnimatorParameter(m_mouthIdle.Value, "", m_mouthLayer.Value);
		    	}
	    	}
	    	else
	    	{
		    	if (m_useMouth.Value)
		    	{
			    	SetAnimatorParameter(m_mouthIdle.Value, "", m_mouthLayer.Value);
		    	}
		    	
		    	SetAnimatorParameter(m_endTalkingAnim.Value, "", m_faceLayer.Value, ()=>
		    	{
			    	SetAnimatorParameter(m_idleAnim.Value, "", m_faceLayer.Value);
		    	});
	    	}

	    }
        
	    protected virtual void OnDialogStart()
	    {
	    	if (m_useAllTransition.Value)
	    	{
		    	SetAnimatorParameter(m_startTalkingAnim.Value, "", m_faceLayer.Value, ()=>
		    	{
		    		SetAnimatorParameter(m_talkingAnim.Value, "", m_faceLayer.Value);
	    		
			    	if (m_useMouth.Value)
			    	{
				    	SetAnimatorParameter(m_mouthTalk.Value, "", m_mouthLayer.Value);
			    	}
		    	});
	    	}
	    	else
	    	{
	    		SetAnimatorParameter(m_talkingAnim.Value, "", m_faceLayer.Value);
	    		
	    		if (m_useMouth.Value)
	    		{
		    		SetAnimatorParameter(m_mouthTalk.Value, "", m_mouthLayer.Value);
	    		}
	    	}
	    }

	    protected virtual void SetAnimatorParameter<T0>(string _name, T0 _value, int _layer, UnityAction _callback = null)
	    {
		    if (m_useAnimations.Value)
		    {

			    if (m_animator.Value != null)
			    {
#if USE_ANIMANCER
				    if (!m_useAnimancer.Value)
				    {

					    m_animator.Value.SetParameter(_name, _value);
					    
					    if (_callback != null)
						    _callback.Invoke();
						    
					    return;
				    }	

				    Animancer.AnimancerState m_state;
				    if (_value is bool)
				    {
					    m_state = m_animator.Value.PlayAnimationFromSet(_name, _layer, 0, m_animationSet.Value);
				    }
				    else if (_value is string)
				    {
					    m_state = m_animator.Value.PlayAnimationFromSet(_name, _layer, 0, m_animationSet.Value);
				    }
				    else if (_value is int)
				    {
					    m_state = m_animator.Value.PlayAnimationFromSet(_name, _layer, (int)System.Convert.ChangeType(_value, typeof(int)), m_animationSet.Value);
				    }
				    else
				    {
					    float f = (float)System.Convert.ChangeType(_value, typeof(float));
					    m_state = m_animator.Value.PlayAnimationFromSet(_name, _layer, (int)f, m_animationSet.Value);
				    }
				    
				    if (m_state != null && _callback != null)
				    {
				    	m_state.Events.OnEnd = ()=>_callback.Invoke();
				    }
				    
#endif
				}
			}
	    }

        public override TaskStatus OnUpdate()
        {
            // We are returning the same status until we hear otherwise.
            return status;
        }

        // ConversationComplete will be called after the Dialogue System finishes its conversation. 
        public void OnDialogEnd()
	    {
		    StopAnimation();
        	
            // Update the status when the Dialogue System completes
            status = TaskStatus.Success;
        }

        public override void OnReset()
        {
            keyOrText = string.Empty;
        }
    }
}

#endif
