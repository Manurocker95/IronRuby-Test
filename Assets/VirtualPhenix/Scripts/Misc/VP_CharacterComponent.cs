using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix;

namespace VirtualPhenix.Controllers.Components
{
    /// <summary>
    /// Player will look for all character components
    /// </summary>
    public class VP_CharacterComponent : VP_MonoBehaviour
    {
        protected bool m_attached = false;
        protected VP_CharacterController m_attachedController;

        public VP_CharacterController CharacterController
        {
            get
            {
                return m_attachedController;
            }
        }


        protected virtual void Reset()
        {
            m_attached = TryToAttachToCharacter();
        }

        protected virtual void OnValidate()
        {
            if (!m_attached)
            {
                m_attached = TryToAttachToCharacter();
            }
        }

        protected virtual bool TryToAttachToCharacter()
        {
            if (transform.TryGetComponentInParent<VP_CharacterController>(out VP_CharacterController chara))
            {
                m_attachedController = chara;
                chara.TryToAddComponent(this);
            }

            return false;
        }
    }
}