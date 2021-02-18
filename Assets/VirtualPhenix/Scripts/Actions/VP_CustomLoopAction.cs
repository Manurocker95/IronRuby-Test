using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Actions
{
    public class VP_CustomLoopAction : VP_CustomActions
    {
        [Header("-- Loop Action --"), Space(10)]
        [SerializeField] protected bool m_forever;       
        [SerializeField] protected int m_times;
        [SerializeField] protected List<VP_CustomActions> m_actions;

        protected int m_timesPlayed = 0;

        public override void InitActions(Action _initCallback = null, Action _invokeCallback = null, Action<int> _indexCallback = null)
        {
            base.InitActions(_initCallback, _invokeCallback, _indexCallback);
            foreach (VP_CustomActions action in m_actions)
            {
                VP_ActionManager.Instance.AddLoopAction(action.InvokeAction, this);
            }
        }
        

        public override void InvokeAction()
        {
            base.InvokeAction();
            if (m_actions.Count > 0)
                VP_ActionManager.Instance.DoLoopAction();
        }

        public virtual void CheckAllLoopAction()
        {
            Debug.Log("All finished!");
            if (m_forever)
            {
                foreach (VP_CustomActions action in m_actions)
                {
                    VP_ActionManager.Instance.AddLoopAction(action.InvokeAction, this);
                }
            }
            else
            {
                m_timesPlayed++;
                if (m_timesPlayed >= m_times)
                {
                    VP_ActionManager.Instance.DoGameplayAction();
                }
                else
                {
                    foreach (VP_CustomActions action in m_actions)
                    {
                        VP_ActionManager.Instance.AddLoopAction(action.InvokeAction, this);
                    }

                    VP_ActionManager.Instance.DoLoopAction();
                }
            }
        }

        public virtual void CancelActions()
        {
            m_forever = false;
            m_timesPlayed = m_times;
            VP_ActionManager.Instance.CancelLoopActions();
        }
    }
}
