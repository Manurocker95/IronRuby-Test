using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Actions
{
    public class VP_CustomMovementAction : VP_CustomActions
    {
        [Header("-- Movement path --"), Space(10)]
        [SerializeField] protected VP_MovementActionPath[] m_movementObjects;
        protected int m_movableCompleted = 0;
        public override void InvokeAction()
        {
            base.InvokeAction();
        }

        public virtual void CheckMovementAllFinished()
        {
            Debug.Log("All finished!");
            m_movableCompleted++;
            if (m_movableCompleted >= m_movementObjects.Length)
            {
                VP_ActionManager.Instance.DoGameplayAction();
            }
        }
    }
}
