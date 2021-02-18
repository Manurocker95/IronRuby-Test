
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Actions
{
    public class VP_CustomStopNavmeshAction : VP_CustomMovementAction
    {

        public override void InitActions(System.Action _initCallback = null, System.Action _invokeCallback = null, System.Action<int> _indexCallback = null)
        {
            base.InitActions(_initCallback, _invokeCallback, _indexCallback);
            m_action = CUSTOM_GAME_ACTIONS.STOP_NAVMESH_MOVEMENT;
        }
        public override void InvokeAction()
        {
            base.InvokeAction();
            for (int i = 0; i < m_movementObjects.Length; i++)
            {
                m_movementObjects[i].StopMovement();
            }

            VP_ActionManager.Instance.DoGameplayAction();
        }
    }

}
