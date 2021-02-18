using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Dialog.Demo
{
    public class VP_DemoInteraction : VP_DialogAnimator
    {
        [SerializeField] protected bool m_canBeInteracted = true;
        [SerializeField] protected bool m_canBeInteractedOnce = false;
        [SerializeField] protected bool m_showInteractBubble = true;
        [SerializeField] protected bool m_interacting = false;
        [SerializeField] protected Transform m_UIRefPos;
       
        protected VP_DemoCharacterController m_characterController;

        public bool CanBeInteracted { get { return m_canBeInteracted; } }
        public bool ShowInteractBubble { get { return m_showInteractBubble; } }
        public Transform UIRefPos { get { return m_UIRefPos; } }

        public virtual void OnInteraction()
        {
        }

        public virtual void OnInteraction(VP_DemoCharacterController _characterController)
        {
            m_characterController = _characterController;
      
        }

        protected override void OnEndText()
        {
            base.OnEndText();
            OnEndInteraction();
        }

        public virtual void OnEndInteraction()
        {
            m_canBeInteracted = (!m_canBeInteractedOnce);
        }
    }

}
