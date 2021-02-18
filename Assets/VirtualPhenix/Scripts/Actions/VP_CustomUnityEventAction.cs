
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace VirtualPhenix.Actions
{
    [System.Serializable]
    public class VP_CustomUnityEventAction : VP_CustomActions
    {
        [Header("--Unity Event Method--"), Space(10)]
        [SerializeField] protected List<UnityEvent> m_unityEvents;


        public override void InitActions(System.Action _initCallback = null, System.Action _invokeCallback = null, System.Action<int> _indexCallback = null)
        {
            base.InitActions(_initCallback, _invokeCallback, _indexCallback);
            m_action = CUSTOM_GAME_ACTIONS.UNITY_EVENTS;
        }

        public override void InvokeAction()
        {
            base.InvokeAction();

            foreach (UnityEvent m_unityEvent in m_unityEvents)
            {
                if (m_unityEvent != null)
                    m_unityEvent.Invoke();
            }

            VP_ActionManager.Instance.DoGameplayAction();
        }
    }

}
