using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace VirtualPhenix.Dialog
{
    [NodeTint(VP_DialogSetup.NodeColors.EVENT_NODE), CreateNodeMenuAttribute("Event")]
    public class VP_DialogEvent : VP_DialogBaseNode
    {
        /// <summary>
        /// When the event is gonna be triggered
        /// </summary>
        public enum TRIGGER_TIME
        {
            ON_NODE_START,          // On Node Trigger. If you have code listening with direct message dialogue, callbacks for it will need to be tweaked
            ON_DIALOG_START,
            ON_DIALOG_TEXT_SHOWN,
            ON_DIALOG_COMPLETE,
            ON_DIALOG_END,
            ON_DIALOG_NUMBER_SHOWN,
            ON_DIALOG_NUMBER_SELECTED,
            ON_NUMBER_CANCEL,
            ON_DIALOG_ANSWER_SHOWN,
            ON_DIALOG_ANSWER_SELECTED,
            ON_SKIP,
            ON_CHARACTER_SPEAK,
            ON_AUTO_ANSWER_START,
            ON_AUTO_ANSWER_END,
            ON_INTERACT_INPUT,
            ON_CANCEL_INPUT,
            ON_UP_INPUT,
            ON_DOWN_INPUT,
            ON_RIGHT_INPUT,
            ON_LEFT_INPUT
        }

        /// <summary>
        /// Event that call a method of scriptable object or prefab
        /// </summary>
        public SerializableEvent[] trigger;
        /// <summary>
        /// VP_EventManager event name
        /// </summary>
        public string[] triggerStrings;

        public TRIGGER_TIME m_triggerTime = TRIGGER_TIME.ON_NODE_START;
        public VP_DialogCharacterData m_character = null;
        /// <summary>
        /// Choice for number and answers. As answers can't be less than 0, setting it 
        /// to -1 will be any choice.
        /// </summary>
        public int m_choice = -1;
        public bool m_sameNumberChosen = true;

        void TriggerEvent()
        {
            if (m_needToSkip)
            {
                m_needToSkip = false;
            }

            for (int i = 0; i < trigger.Length; i++)
            {
                trigger[i].Invoke();
            }

            foreach (string t in triggerStrings)
            {
                if (!string.IsNullOrEmpty(t))
                {
                    VP_EventManager.TriggerEvent(t);
                }
            }
           
        }

        #region LISTENS
        
        void OnAutoAnswerEnd()
        {
            VP_DialogManager.StopListeningToOnAutoAnswerTimeEnd(OnAutoAnswerEnd);
            TriggerEvent();
        }
        void OnAutoAnswerStart()
        {
            VP_DialogManager.StopListeningToOnAutoAnswerTimeStart(OnAutoAnswerStart);
            TriggerEvent();
        }

        void OnDialogCancel()
        {
            VP_DialogManager.StopListeningToOnDialogCancel(OnDialogCancel);
            TriggerEvent();
        }

        void OnCharacterSpeak(VP_DialogCharacterData _data)
        {
            VP_DialogManager.StopListeningToOnCharacterSpeak(OnCharacterSpeak);
            TriggerEvent();
        }

        void OnChoiceSelection(int _value)
        {
            if (m_choice > -1)
            {
                if (_value == m_choice)
                {
                    VP_DialogManager.StopListeningOnChoiceSelection(OnChoiceSelection);
                    TriggerEvent();
                }
            }
            else
            {
                VP_DialogManager.StopListeningOnChoiceSelection(OnChoiceSelection);
                TriggerEvent();
            }
        }

        void OnAnswerShow()
        {
            VP_DialogManager.StopListeningToOnAnswerShow(OnAnswerShow);
            TriggerEvent();
        }

        void OnDialogComplete()
        {
            VP_DialogManager.StopListeningToOnDialogComplete(OnDialogComplete);
            TriggerEvent();
        }

        void OnDialogEnd()
        {
            VP_DialogManager.StopListeningToOnDialogEnd(OnDialogEnd);
            TriggerEvent();
        }

        void OnNumberChooseShow()
        {
            VP_DialogManager.StopListeningToOnNumberShow(OnNumberChooseShow);
            TriggerEvent();
        }

        void OnNumberChosen(int _value)
        {
            if (m_sameNumberChosen)
            {
                if (_value == m_choice)
                {
                    VP_DialogManager.StopListeningToOnChooseNumber(OnNumberChosen);
                    TriggerEvent();
                }
            }
            else
            {
                VP_DialogManager.StopListeningToOnChooseNumber(OnNumberChosen);
                TriggerEvent();
            }
        }

        void OnNumberCancel()
        {
            VP_DialogManager.StopListeningToOnNumberCancel(OnNumberCancel);
            TriggerEvent();
        }

        void OnDialogStart()
        {
            VP_DialogManager.StopListeningToOnDialogStart(OnDialogStart);
            TriggerEvent();
        }

        void OnTextShown()
        {
            VP_DialogManager.StopListeningToOnTextShown(OnTextShown);
            TriggerEvent();
        }

        void OnDialogDown()
        {
            VP_DialogManager.StartListeningToOnDialogLeft(OnDialogDown);
            TriggerEvent();
        }

        void OnDialogUp()
        {
            VP_DialogManager.StartListeningToOnDialogLeft(OnDialogUp);
            TriggerEvent();
        }

        void OnDialogRight()
        {
            VP_DialogManager.StartListeningToOnDialogLeft(OnDialogRight);
            TriggerEvent();
        }

        void OnDialogLeft()
        {
            VP_DialogManager.StartListeningToOnDialogLeft(OnDialogLeft);
            TriggerEvent();
        }

        void OnInteract()
        {
            VP_DialogManager.StartListeningToOnDialogInteract(OnInteract);
            TriggerEvent();
        }
        #endregion

        public override void Trigger<T>(T parameter)
        {
            Trigger();
        }

        /// <summary>
        /// Trigger the event
        /// </summary>
        public override void Trigger()
	    {
		    NodePort port = GetOutputPort(VP_DialogSetup.Fields.DIALOG_NODE_OUTPUT);
		    bool hasContinuation = (port.ConnectionCount == 0);
		    
		    
		    if (!hasContinuation)
			    CheckEndSequenceNode();
			
			
		    VP_DialogManager.OnDialogCompleteAction();
        	
            switch (m_triggerTime)
            {
                case TRIGGER_TIME.ON_NODE_START:
                    TriggerEvent();
                    break;
                case TRIGGER_TIME.ON_AUTO_ANSWER_END:
                    VP_DialogManager.StartListeningToOnAutoAnswerTimeEnd(OnAutoAnswerEnd);
                    break;
                case TRIGGER_TIME.ON_AUTO_ANSWER_START:
                    VP_DialogManager.StartListeningToOnAutoAnswerTimeStart(OnAutoAnswerStart);
                    break;
                case TRIGGER_TIME.ON_CANCEL_INPUT:
                    VP_DialogManager.StartListeningToOnDialogCancel(OnDialogCancel);
                    break;
                case TRIGGER_TIME.ON_CHARACTER_SPEAK:
                    VP_DialogManager.StartListeningToOnCharacterSpeak(OnCharacterSpeak);
                    break;
                case TRIGGER_TIME.ON_DIALOG_ANSWER_SELECTED:
                    VP_DialogManager.StartListeningOnChoiceSelection(OnChoiceSelection);
                    break;
                case TRIGGER_TIME.ON_DIALOG_ANSWER_SHOWN:
                    VP_DialogManager.StartListeningToOnAnswerShow(OnAnswerShow);
                    break;
                case TRIGGER_TIME.ON_DIALOG_COMPLETE:
                    VP_DialogManager.StartListeningToOnDialogComplete(OnDialogComplete);
                    break;
                case TRIGGER_TIME.ON_DIALOG_END:
                    VP_DialogManager.StartListeningToOnDialogEnd(OnDialogEnd);
                    break;
                case TRIGGER_TIME.ON_DIALOG_NUMBER_SHOWN:
                    VP_DialogManager.StartListeningToOnNumberShow(OnNumberChooseShow);
                    break;
                case TRIGGER_TIME.ON_DIALOG_NUMBER_SELECTED:
                    VP_DialogManager.StartListeningToOnChooseNumber(OnNumberChosen);
                    break;
                case TRIGGER_TIME.ON_NUMBER_CANCEL:
                    VP_DialogManager.StartListeningToOnNumberCancel(OnNumberCancel);
                    break;
                case TRIGGER_TIME.ON_DIALOG_START:
                    VP_DialogManager.StartListeningToOnDialogStart(OnDialogStart);
                    break;
                case TRIGGER_TIME.ON_DIALOG_TEXT_SHOWN:
                    VP_DialogManager.StartListeningToOnTextShown(OnTextShown);
                    break;
                case TRIGGER_TIME.ON_DOWN_INPUT:
                    VP_DialogManager.StartListeningToOnDialogDown(OnDialogDown);
                    break;
                case TRIGGER_TIME.ON_UP_INPUT:
                    VP_DialogManager.StartListeningToOnDialogUp(OnDialogUp);
                    break;
                case TRIGGER_TIME.ON_RIGHT_INPUT:
                    VP_DialogManager.StartListeningToOnDialogRight(OnDialogRight);
                    break;
                case TRIGGER_TIME.ON_LEFT_INPUT:
                    VP_DialogManager.StartListeningToOnDialogLeft(OnDialogLeft);
                    break;
                case TRIGGER_TIME.ON_INTERACT_INPUT:
                    VP_DialogManager.StartListeningToOnDialogInteract(OnInteract);
                    break;
            }

		    if (hasContinuation)
            {
			    //CheckEndSequenceNode();
                return;
            }

            for (int i = 0; i < port.ConnectionCount; i++)
            {
                NodePort connection = port.GetConnection(i);
                (connection.node as VP_DialogBaseNode).Trigger();
            }
        }
    }
}