
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix;

namespace VirtualPhenix.Actions
{
    public class VP_CustomEventAction : VP_CustomActions
    {

        [Header("--Custom Events--"), Space(10)]
        [SerializeField] protected string m_customEvent;
        [SerializeField] protected bool m_usePendingActionAsParameter;

        public override void InitActions(System.Action _initCallback = null, System.Action _invokeCallback = null, System.Action<int> _indexCallback = null)
        {
            base.InitActions(_initCallback, _invokeCallback, _indexCallback);
            m_action = CUSTOM_GAME_ACTIONS.CUSTOM_EVENTS;
        }


        public override void InvokeAction()
        {
            base.InvokeAction();
            if (m_usePendingActionAsParameter)
            {
                VP_EventManager.TriggerEvent(m_customEvent, new Action(VP_ActionManager.Instance.DoGameplayAction));
            }
            else
            {
                VP_EventManager.TriggerEvent(m_customEvent);
                VP_ActionManager.Instance.DoGameplayAction();
            }

        }
    }

}
