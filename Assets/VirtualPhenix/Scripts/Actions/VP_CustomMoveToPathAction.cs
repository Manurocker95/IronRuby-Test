using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Actions
{
    [System.Serializable]
    public class VP_CustomMoveToPathAction : VP_CustomMovementAction
    {
        public override void InitActions(System.Action _initCallback = null, System.Action _invokeCallback = null, System.Action<int> _indexCallback = null)
        {
            base.InitActions(_initCallback, _invokeCallback, _indexCallback);
            m_action = CUSTOM_GAME_ACTIONS.MOVE_TO;
        }

        public override void InvokeAction()
        {
            base.InvokeAction();

            if (m_movementObjects.Length == 0)
            {
                VP_ActionManager.Instance.DoGameplayAction();
                return;
            }

            m_movableCompleted = 0;
            for (int i = 0; i < m_movementObjects.Length; i++)
            {
                m_movementObjects[i].SetFollowActionPath(CheckMovementAllFinished);
            }

        }
    }

}
