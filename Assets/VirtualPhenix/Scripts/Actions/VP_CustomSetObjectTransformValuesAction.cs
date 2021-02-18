using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Actions
{
    [System.Serializable]
    public class VP_CustomSetObjectTransformValuesAction : VP_CustomActions
    {
        [Header("--Set Position--"), Space(10)]
        [SerializeField] protected Transform m_targetTrs;
        [SerializeField] protected bool m_setPosition;
        [SerializeField] protected Vector3 m_newPosition;
        [SerializeField] protected bool m_setRotation;
        [SerializeField] protected Quaternion m_newRotation;
        [SerializeField] protected bool m_setScale;
        [SerializeField] protected Vector3 m_newScale;

        public override void InitActions(System.Action _initCallback = null, System.Action _invokeCallback = null, System.Action<int> _indexCallback = null)
        {
            base.InitActions(_initCallback, _invokeCallback, _indexCallback);
            m_action = CUSTOM_GAME_ACTIONS.SET_POSITION;
        }

        public override void InvokeAction()
        {
            base.InvokeAction();
            if (m_targetTrs)
            {
                if (m_setPosition)
                    m_targetTrs.position = m_newPosition;

                if (m_setRotation)
                    m_targetTrs.rotation = m_newRotation;

                if (m_setScale)
                    m_targetTrs.localScale = m_newScale;
            }

            VP_ActionManager.Instance.DoGameplayAction();
        }
    }

}
