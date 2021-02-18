using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Actions
{
    public class VP_CustomPlayAnimation : VP_CustomActions
    {
        public enum ANIMATION_TRIGGER
        {
            BOOL,
            FLOAT,
            TRIGGER
        }

        [SerializeField] private ANIMATION_TRIGGER m_animationTrigger = ANIMATION_TRIGGER.TRIGGER;
        [SerializeField] private Animator m_animator = null;
        [SerializeField] private string m_parameter = "";
        [SerializeField] private float m_fValue = 0f;
        [SerializeField] private bool m_bValue = false;

        public override void InitActions(Action _initCallback = null, Action _invokeCallback = null, Action<int> _indexCallback = null)
        {
            base.InitActions(_initCallback, _invokeCallback, _indexCallback);
        }

        public override void InvokeAction()
        {
            base.InvokeAction();

            if (m_animator && !string.IsNullOrEmpty(m_parameter))
            {
                switch (m_animationTrigger)
                {
                    case ANIMATION_TRIGGER.TRIGGER:
                        m_animator.SetTrigger(m_parameter);
                        break;
                    case ANIMATION_TRIGGER.FLOAT:
                        m_animator.SetFloat(m_parameter, m_fValue);
                        break;
                    case ANIMATION_TRIGGER.BOOL:
                        m_animator.SetBool(m_parameter, m_bValue);
                        break;
                }
            }
        }
    }

}
