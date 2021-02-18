using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Actions
{
    public class VP_CustomPlayVFXAction : VP_CustomInstantiateAction
    {
        [SerializeField] private GameObject m_vfxPrefab = null;
        [SerializeField] private Transform m_transform = null;
        [SerializeField] private bool m_useTransform = false;
        [SerializeField] private Vector3 m_position = Vector3.zero;
        [SerializeField] private Quaternion m_rotation = Quaternion.identity;
        [SerializeField] private float m_lifeTime = 1f;

        public override void InvokeAction()
        {
            base.InvokeAction();
            if (m_useTransform)
                GameObject.Instantiate(m_vfxPrefab, m_transform);
            else
                GameObject.Instantiate(m_vfxPrefab, m_position, m_rotation);

            VP_ActionManager.Instance.StartWaitTime(m_lifeTime, VP_ActionManager.Instance.DoGameplayAction);
        }
    }
}
