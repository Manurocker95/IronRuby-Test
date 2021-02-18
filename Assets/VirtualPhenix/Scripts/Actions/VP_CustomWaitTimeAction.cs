
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Actions
{
    public class VP_CustomWaitTimeAction : VP_CustomActions
    {
        [Header("-- Wait time --"), Space(10)]
        protected float m_timeToWait;
        [SerializeField] protected float m_minTime;
        [SerializeField] protected float m_maxTime;


        public override void InitActions(System.Action _initCallback = null, System.Action _invokeCallback = null, System.Action<int> _indexCallback = null)
        {
            base.InitActions(_initCallback, _invokeCallback, _indexCallback);
            m_action = CUSTOM_GAME_ACTIONS.WAIT;
        }

        public override void InvokeAction()
        {
            base.InvokeAction();

            m_timeToWait = UnityEngine.Random.Range(m_minTime, m_maxTime);

            VP_ActionManager.Instance.StartWaitTime(m_timeToWait, VP_ActionManager.Instance.DoGameplayAction);

        }
    }

}
