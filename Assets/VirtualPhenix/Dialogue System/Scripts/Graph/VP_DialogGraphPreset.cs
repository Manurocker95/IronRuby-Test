using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace VirtualPhenix.Dialog
{
    [CreateAssetMenu(fileName = "New Dialog Preset", menuName = "Virtual Phenix/Dialogue System/Dialog Preset", order = 0)]
    public class VP_DialogGraphPreset : VP_ScriptableObject
    {
        public DIALOG_TYPE m_dialogueType = DIALOG_TYPE.REGULAR;
        public VP_DialogCharacterData m_characterData;
        public VP_DialogPositionData m_positionData;
        public bool m_localization = false;
        public bool m_waitForInput = true;
        public bool m_canSkipWithInput = true;
        public bool m_waitForAudioEnd = false;
        public TMP_FontAsset m_font;
        public float m_fontSize = 45f;
        public Color m_textColor = Color.black;
        public bool m_showDirectly = false;
        public bool m_fadeInOut = true;
        public float m_textSpeed = 1f;
        public float m_fadeDuration = 1f;
        public float m_automaticScreenTime = 1f;
        public float m_timeForAnswer = 5f;
        public bool m_registerDialog = true;
        public bool m_soundOnContinue = true;

        public void OnEnable()
        {
            if (m_font == null)
            {
                m_font = Resources.Load<TMPro.TMP_FontAsset>("Dialogue/Fonts/DialogueSystemDefaultFont");
            }
        }
    }
}
