using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix
{
    [System.Serializable]
    public class VP_AudioSettings 
    {
        public float m_bgmVolume = 1f;
        public float m_sfxVolume = 1f;
        public float m_voiceVolume = 1f;
        public float m_masterVolume = 1f;

        public VP_AudioSettings()
        {

        }

        public VP_AudioSettings(float _bgm, float _sfx, float _voice)
        {
            m_bgmVolume = _bgm;
            m_sfxVolume = _sfx;
            m_voiceVolume = _voice;
        }
    }

}
