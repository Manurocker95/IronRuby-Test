using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Actions
{
    public class VP_CustomSetListIndex : VP_CustomActions
    {
        [Header("-- List Index --"), Space(10)]
        [SerializeField] protected int m_actionListIndex = 0;

        public override void InitActions(System.Action _initCallback = null, System.Action _invokeCallback = null, System.Action<int> _indexCallback = null)
        {
            base.InitActions(_initCallback, _invokeCallback, _indexCallback);
            m_action = CUSTOM_GAME_ACTIONS.SET_LIST_INDEX;
        }

        public override void InvokeAction()
        {
            base.InvokeAction();

            m_onListCallback.Invoke(m_actionListIndex);

            VP_ActionManager.Instance.DoGameplayAction();
        }
    }

}
