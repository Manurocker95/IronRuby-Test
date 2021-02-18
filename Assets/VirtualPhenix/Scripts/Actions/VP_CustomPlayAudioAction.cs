using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix;

namespace VirtualPhenix.Actions
{
    public class VP_CustomPlayAudioAction : VP_CustomAudioAction
    {
        public override void InitActions(System.Action _initCallback = null, System.Action _invokeCallback = null, System.Action<int> _indexCallback = null)
        {
            base.InitActions(_initCallback, _invokeCallback, _indexCallback);
            m_action = CUSTOM_GAME_ACTIONS.PLAY_AUDIO;
        }

        public override void InvokeAction()
        {
            base.InvokeAction();
            if (m_previousAudio)
            {
                if (!string.IsNullOrEmpty(m_audioKey))
                {
                    if (m_oneShot)
                        VP_AudioManager.PlayAudioItemOneShotbyKey(m_audioKey);
                    else
                        VP_AudioManager.PlayAudioItemByKey(m_audioKey);
                }
                else
                {
                    Debug.LogError("NO AUDIO KEY TO PLAY");
                }
            }
            else if (m_addItForLater)
            {
                if (!string.IsNullOrEmpty(m_audioKey) && m_audioToPlay)
                {
                    if (m_oneShot)
                        VP_AudioManager.PlayOneShot(m_audioToPlay, m_audioType, true, m_audioVolume, m_audioKey, m_addItForLater);
                    else
                        VP_AudioManager.PlayNewAudio(m_audioToPlay, m_audioType, true, m_audioVolume, m_audioKey, m_addItForLater);
                }
                else
                {
                    Debug.LogError("NO AUDIO KEY TO PLAY");
                }
            }
            else
            {
                if (m_audioToPlay)
                {
                    if (m_oneShot)
                        VP_AudioManager.PlayOneShot(m_audioToPlay, m_audioType, true, m_audioVolume, m_audioKey, m_addItForLater);
                    else
                        VP_AudioManager.PlayNewAudio(m_audioToPlay, m_audioType, true, m_audioVolume, m_audioKey, m_addItForLater);
                }
                else
                {
                    Debug.LogError("NO AUDIO TO PLAY");
                }

            }

            if (m_waitForAudio && m_audioToPlay)
            {
                Debug.LogError("Audio with clip " + m_audioToPlay.name + " played with waiting.");
                VP_ActionManager.Instance.StartWaitTime(m_audioToPlay.length, VP_ActionManager.Instance.DoGameplayAction);
            }
            else
            {
                Debug.LogError("Audio with clip " + m_audioToPlay.name + " played but no wait");
                VP_ActionManager.Instance.DoGameplayAction();
            }
          
        }
    }

}
