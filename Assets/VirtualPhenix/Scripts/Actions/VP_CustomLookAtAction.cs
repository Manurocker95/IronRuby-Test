using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Actions
{
    public class VP_CustomLookAtAction : VP_CustomMovementAction
    {
        [SerializeField] protected Transform m_whereToLookAt;

        public override void InitActions(System.Action _initCallback = null, System.Action _invokeCallback = null, System.Action<int> _indexCallback = null)
        {
            base.InitActions(_initCallback, _invokeCallback, _indexCallback);
            m_action = CUSTOM_GAME_ACTIONS.ROTATE_TO;
        }

        public override void InvokeAction()
        {
            base.InvokeAction();

            if (m_movementObjects.Length == 0)
            {
                VP_ActionManager.Instance.DoGameplayAction();
                return;
            }

            for (int i = 0; i < m_movementObjects.Length; i++)
            {
                m_movementObjects[i].RotateTo(m_whereToLookAt.position, null);
            }

            VP_ActionManager.Instance.DoGameplayAction();
        }
    }

}
