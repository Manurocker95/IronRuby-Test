using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix;
using VirtualPhenix.Dialog;

namespace VirtualPhenix.Actions
{
    public class VP_MessageOption
    {
        public VP_Dialog.Answer m_answer;
        public Action m_callback;
    }


    [System.Serializable]
    public class VP_CustomDialogWithOptionsAction : VP_CustomDialogAction
    {
        [Header("-- Options --"), Space(10)]
        [SerializeField] protected List<VP_MessageOption> m_options;

        public override void InitActions(System.Action _initCallback = null, System.Action _invokeCallback = null, System.Action<int> _indexCallback = null)
        {          
            base.InitActions(_initCallback, _invokeCallback, _indexCallback);
            m_action = CUSTOM_GAME_ACTIONS.DIALOG_WITH_OPTIONS;
        }

        public override void InvokeAction()
        {
           
            if (m_onStartCallback != null)
                m_onStartCallback.Invoke();

            if (m_useDialgueSystemGraph)
            {
                if (!string.IsNullOrEmpty(m_regularMessage))
                    VP_DialogManager.SendDialogMessage(m_regularMessage);
            }
            else
            {

                int counter = 0;
                List<VP_Dialog.Answer> answers = new List<VP_Dialog.Answer>();
                List<Action> actions = new List<Action>();
                foreach (VP_MessageOption answer in m_options)
                {
                    answers.Add(answer.m_answer);
                    actions.Add(new Action(() => { if (m_onEndCallback != null) m_onEndCallback.Invoke(); VP_ActionManager.Instance.DoActionBasedOnSelection(counter); }));
                    counter++;
                }


                VP_DialogManager.ShowDirectMessageAndOptions(m_regularMessage, null, DIALOG_TYPE.REGULAR, answers, null, true, -1, m_useTranslation, false, true, m_customMessage);
            }
        }


    }

}
