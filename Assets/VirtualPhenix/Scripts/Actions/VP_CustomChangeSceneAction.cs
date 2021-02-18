

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix;

namespace VirtualPhenix.Actions
{
    public class VP_CustomChangeSceneAction : VP_CustomSceneChangeAction
    {
        public override void InitActions(System.Action _initCallback = null, System.Action _invokeCallback = null, System.Action<int> _indexCallback = null)
        {
            m_action = CUSTOM_GAME_ACTIONS.CHANGE_SCENE;
            base.InitActions(_initCallback, _invokeCallback, _indexCallback);
        }

        public override void InvokeAction()
        {
            base.InvokeAction();
            VP_ActionManager.Instance.ClearPendingActions();
            if (m_playChangeSceneAudio)
                VP_AudioManager.Instance.PlayOneShot(m_transitionAudio, VP_AudioSetup.AUDIO_TYPE.SFX, 1);

            
            // TODO
            VP_SceneManager.Instance.LoadSceneAsync(m_sceneToChange, 1f, m_onloadEvent);

        }
    }

}
