using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix.Interaction;

namespace VirtualPhenix.Actions
{
    public class VP_CustomInitOtherNPCAction : VP_CustomActions
    {
        [Header("--Init Other NPC Interaction--"), Space(10)]
        [SerializeField] protected VP_InteractableObject m_interactableObject;


        public override void InitActions(System.Action _initCallback = null, System.Action _invokeCallback = null, System.Action<int> _indexCallback = null)
        {
            base.InitActions(_initCallback, _invokeCallback, _indexCallback);
            m_action = CUSTOM_GAME_ACTIONS.INIT_OTHER_NPC_INTERACTION;
        }


        public override void InvokeAction()
        {
            base.InvokeAction();
            if (m_interactableObject != null)
            {
                m_interactableObject.OnInteractTrigger();
            }
            VP_ActionManager.Instance.DoGameplayAction();
        }
    }

}
