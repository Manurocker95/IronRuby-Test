using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Dialog
{
    public class VP_SimpleTalker : VP_DialogAnimator
    {
        [Header("Interaction")]
        protected bool m_showInteractBubble = true;
        protected bool m_interacting = true;
        [SerializeField] protected bool m_canBeInteracted = true;

        [Header("Type of message sent to dialogue manager"), Space()]
        [SerializeField] private VP_DialogInitializer.TALK_TYPE m_talkType = VP_DialogInitializer.TALK_TYPE.KEY;
        public VP_DialogInitEventKey m_initKey = null;
        public string m_key = "";

        protected override void Start()
        {
            base.Start();

        }

        protected override void OnEndText()
        {
            base.OnEndText();
            OnEndInteraction();
        }
        /// <summary>
        /// Call this method from anywhere to interact with this
        /// </summary>
        public virtual void Interact()
        {
            m_interacting = true;

            if (m_animationCheck == ANIMATION_BASE.ANIMATE_WITHOUT_CHARACTER)
                m_speakRefresh = true;

            if (m_initKey != null && string.IsNullOrEmpty(m_key))
                m_key = m_initKey.key;

            if (!string.IsNullOrEmpty(m_key))
            {
                if (m_talkType == VP_DialogInitializer.TALK_TYPE.KEY)
                    VP_DialogManager.SendDialogMessage(m_key);
                else
                    VP_DialogManager.ShowDirectMessage(m_key);
            }

            m_canBeInteracted = false;
        }

        public virtual void OnEndInteraction()
        {
            m_interacting = false;
            m_speakRefresh = false;
            m_canBeInteracted = true;
        }
    }

}
