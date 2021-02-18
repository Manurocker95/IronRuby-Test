using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix;

namespace VirtualPhenix.Actions
{
    public class VP_CustomGameEventAction : VP_CustomActions
    {
        [SerializeField] private List<VP_GameEvent> m_gameEvents = new List<VP_GameEvent>();

        public override void InvokeAction()
        {
            base.InvokeAction();
            foreach (VP_GameEvent ge in m_gameEvents)
            {
                ge.DoEvent();
            }

            VP_ActionManager.Instance.DoGameplayAction();
        }
    }

}
