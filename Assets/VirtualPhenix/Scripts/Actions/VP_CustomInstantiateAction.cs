using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Actions
{
    public class VP_CustomInstantiateAction : VP_CustomActions
    {

        [SerializeField] private GameObject m_objectToInstance = null;
        [SerializeField] private Transform m_parent = null;
        [SerializeField] private Vector3 m_position = Vector3.zero;
        [SerializeField] private Quaternion m_rotation = Quaternion.identity;
        [SerializeField] private Vector3 m_scale = Vector3.one;

        public override void InitActions(Action _initCallback = null, Action _invokeCallback = null, Action<int> _indexCallback = null)
        {
            base.InitActions(_initCallback, _invokeCallback, _indexCallback);
        }

        public override void InvokeAction()
        {
            base.InvokeAction();

            if (m_objectToInstance)
            {
                GameObject obj = null;
                if (m_parent)
                {
                    obj = UnityEngine.Object.Instantiate(m_objectToInstance, m_parent);
                    obj.transform.localPosition = m_position;
                    obj.transform.localRotation = m_rotation;
                    obj.transform.localScale = m_scale;
                }
                else
                {
                    obj = UnityEngine.Object.Instantiate(m_objectToInstance, m_parent);
                    obj.transform.position = m_position;
                    obj.transform.rotation = m_rotation;
                    obj.transform.localScale = m_scale;
                }
            }
        }
    }
}

