
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix;

namespace VirtualPhenix.Actions
{
    public class VP_CustomStopAudioAction : VP_CustomAudioAction
    {
        [Header("--Stop Audio--"), Space(10)]
        [SerializeField] protected bool m_stopByKey;
        [SerializeField] protected bool m_stopAllFromAudioType;


        public override void InitActions(System.Action _initCallback = null, System.Action _invokeCallback = null, System.Action<int> _indexCallback = null)
        {
            base.InitActions(_initCallback, _invokeCallback, _indexCallback);
            m_action = CUSTOM_GAME_ACTIONS.STOP_AUDIO;
        }

        public override void InvokeAction()
        {
            base.InvokeAction();

            if (m_stopByKey)
            {
                VP_AudioManager.StopAudioItembyKey(m_audioKey);
            }
            else if (m_stopAllFromAudioType)
            {
                VP_AudioManager.StopAllAudiosByType(m_audioType);
            }

            VP_ActionManager.Instance.DoGameplayAction();
        }
    }

}
