
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Actions
{
    public class VP_CustomCallMethodAction : VP_CustomActions
    {
        [Header("--Call Method--"), Space(10)]
        [SerializeField] protected GameObject m_targetObject;
        [SerializeField] protected string m_method;

        public override void InitActions(System.Action _initCallback = null, System.Action _invokeCallback = null, System.Action<int> _indexCallback = null)
        {
            m_action = CUSTOM_GAME_ACTIONS.CALL_METHOD;
            base.InitActions(_initCallback, _invokeCallback, _indexCallback);
        }

        public override void InvokeAction()
        {
            base.InvokeAction();

            if (m_targetObject && !string.IsNullOrEmpty(m_method))
                m_targetObject?.SendMessage(m_method);

            VP_ActionManager.Instance.DoGameplayAction();
        }
    }

}
