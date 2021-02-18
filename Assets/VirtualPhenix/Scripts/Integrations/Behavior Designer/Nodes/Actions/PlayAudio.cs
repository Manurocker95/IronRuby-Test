#if USE_BEHAVIOR_DESIGNER
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections.Generic;

namespace VirtualPhenix.Integrations.BehaviorDesignerTree
{
    [TaskDescription("Play audio with Phoenix Dialogue System's Audio Manager")]
    [TaskCategory(VP_PrefixSetup.MAIN_PREFIX+"/Audio")]
    [TaskIcon("Assets/VirtualPhenix/Editor/Icons/Behavior Designer/PlayAudioIcon.png")]
    public class PlayAudio : Action
    {
        public SharedBool m_useItemKey = true;
        public SharedBool m_playOneShot = true;
        public SharedString m_audioItemKey = "defaultSFX";
        public SharedFloat m_audioVolume = 1f;
        public SharedAudioType m_audioType = VP_AudioSetup.AUDIO_TYPE.SFX;
        public SharedAudioClip2 m_clip;
        public SharedBool m_addItForLater = true;
        public SharedBool m_overrideIfExist = false;
        public SharedString m_keyForlater = "";

        public override void OnStart()
        {
            if (m_useItemKey.Value)
            {
                if (m_playOneShot.Value)
                {
                    VP_AudioManager.PlayAudioItemOneShotbyKey(m_audioItemKey.Value);
                }
                else
                {
                    VP_AudioManager.PlayAudioItemByKey(m_audioItemKey.Value);
                }
            }
            else
            {
                if (m_playOneShot.Value)
                {
                    VP_AudioManager.PlayOneShot(m_clip.Value, m_audioType.Value, m_overrideIfExist.Value, m_audioVolume.Value, m_keyForlater.Value, m_addItForLater.Value);
                }
                else
                {
                    VP_AudioManager.PlayNewAudio(m_clip.Value, m_audioType.Value, m_overrideIfExist.Value, m_audioVolume.Value, m_keyForlater.Value, m_addItForLater.Value);
                }
            }
        }
    }

}
#endif