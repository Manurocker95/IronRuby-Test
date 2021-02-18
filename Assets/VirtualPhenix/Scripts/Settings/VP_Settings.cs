using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Settings
{
    [System.Serializable]
    public class VP_Settings
    {
        [Header("Common Settings"), Space]
        public float m_bgmVolume = 1f;
        public float m_sfxVolume = 1f;
        public float m_voiceVolume = 1f;
        public SystemLanguage m_currentLanguage = SystemLanguage.English;
    }
}
