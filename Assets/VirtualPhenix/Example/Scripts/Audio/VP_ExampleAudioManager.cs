using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Example.Audio
{
    [
         DefaultExecutionOrder(VP_ExecutingOrderSetup.AUDIO_MANAGER),
         AddComponentMenu(VP_PrefixSetup.MAIN_PREFIX + "/Example/Example Audio Manager")
    ]
    public class VP_ExampleAudioManager : VP_AudioManager
    {
        [Header("Example Audio Manager"),Space]
        [SerializeField] private AudioClip m_Clip;
        [SerializeField] private string m_itemKey = "ItemKey";
        [SerializeField] private bool m_toggled = false;

        public void PlayFromAudioClip()
        {
            this.PlayOneShot(m_Clip, VP_AudioSetup.AUDIO_TYPE.SFX, 1f);
        }

        public void PlayFromAudioItem()
        {
            this.PlayItemOneShot(m_itemKey, 1f);
        }

        public void PlayToggleAllVolumes()
        {
            m_toggled = !m_toggled;
            float volume = m_toggled ? 1 : 0;

            SetAllTypesVolume(volume);
        }
    }
}
