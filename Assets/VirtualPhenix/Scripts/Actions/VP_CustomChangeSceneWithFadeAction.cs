using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix;

namespace VirtualPhenix.Actions
{
    public class VP_CustomChangeSceneWithFadeAction : VP_CustomSceneChangeAction
    {
        public override void InitActions(System.Action _initCallback = null, System.Action _invokeCallback = null, System.Action<int> _indexCallback = null)
        {
            m_action = CUSTOM_GAME_ACTIONS.CHANGE_SCENE_WITH_FADE;
           
            base.InitActions(_initCallback, _invokeCallback, _indexCallback);
        }

        public override void InvokeAction()
        {
            base.InvokeAction();
            VP_ActionManager.Instance.ClearPendingActions();
#if USE_FADE
            if (m_fadeEffect != null && m_fadePP != null)
            {
                if (m_modifyFadeDuration)
                    m_fadePP.m_effectDuration = m_newFadeDuration;

                m_fadePP.AssignEffect(m_fadeEffect);

                if (m_playChangeSceneAudio)
                    VP_AudioManager.Instance.PlayOneShot(m_transitionAudio, VP_AudioSetup.AUDIO_TYPE.SFX, 1f);

                if (m_fadeType == FADE_EFFECT_TYPE.DOWN)
                {
                    m_fadePP.FadeDown(m_fadeDirectly, () =>
                    {
                        VP_SceneManager.Instance.LoadSceneAsync(m_sceneToChange, 1f, m_onloadEvent, true, true, true);
                    });
                }
                else
                {
                    m_fadePP.FadeUp(m_fadeDirectly, () =>
                    {
                        VP_SceneManager.Instance.LoadSceneAsync(m_sceneToChange, 1f, m_onloadEvent, true, true, true);
                    });
                }
            }
            else
            {
                Debug.LogError("NO FADE EFFECT, WILL CHANGE WITHOUT IT");
                if (m_playChangeSceneAudio)
                    VP_AudioManager.Instance.PlayOneShot(m_transitionAudio, VP_AudioSetup.AUDIO_TYPE.SFX, 1f);

                VP_SceneManager.Instance.LoadSceneAsync(m_sceneToChange, 1f, m_onloadEvent, true, true, true);
            }
#else
            Debug.LogError("NO FADE EFFECT, WILL CHANGE WITHOUT IT");
            if (m_playChangeSceneAudio)
                VP_AudioManager.Instance.PlayOneShot(m_transitionAudio, VP_AudioSetup.AUDIO_TYPE.SFX, 1f);

            VP_SceneManager.Instance.LoadSceneAsync(m_sceneToChange, 1f, m_onloadEvent, true, true, true);
#endif
        }
    }

}
