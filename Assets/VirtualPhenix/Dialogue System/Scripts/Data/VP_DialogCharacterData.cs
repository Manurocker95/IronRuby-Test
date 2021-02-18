using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using VirtualPhenix.ResourceReference;

namespace VirtualPhenix.Dialog
{
    public enum DIALOG_VOICE_TYPE
    {
        NO_SOUND,
        SOUND_EVERY_CHARACTER,
        SOUND_VOCALS,
        SOUND_EVERY_CHARACTER_DIFFERENT_SOUND,
        SOUND_EVERY_WORD
    }

    public enum CHARACTER_GENDER
    {
        MALE,
        FEMALE,
        OTHER
    }

    [CreateAssetMenu(menuName = "Virtual Phenix/Dialogue System/Characters/CharacterData")]
	public class VP_DialogCharacterData : VP_ScriptableObject
	{
		/// <summary>
		/// Node Color
		/// </summary>
        public Color color;
        public Sprite image;
        public string characterName;

        public CHARACTER_GENDER gender;

#if USE_ANIMANCER
		public VP_AnimationTransitionResources m_animationResources;

#endif		

        public RuntimeAnimatorController animator;
		public string idleAnim = "IDLE";
		public string speakingAnim = "TALK";
		public string startSpeakingAnim = "START_TALK";
		public string endSpeakingAnim= "END_TALK";

		
        public DIALOG_VOICE_TYPE m_dialogVoiceType;
        public AudioClip m_dialogVoiceClip;
        
        //public VP_DialogPositionData m_characterPositionData;

        public TMP_FontAsset m_customFont;

        public float m_timeBetweenCharacters = 0.1f;

		public bool m_overrideTextColor = true;
        
		public float m_characterDefaultFontSize = 45f;
		public bool m_overrideFontSize = true;
		
		public Color m_characterDefaultNameTextColor;
		public bool m_overrideTextNameColor = true;
		
        /// <summary>
        /// Text default color for the character
        /// </summary>
        public Color m_characterDefaultTextColor;

        protected virtual void Awake()
        {
            if (m_dialogVoiceClip == null)
                m_dialogVoiceClip = Resources.Load<AudioClip>("Dialogue/Letters/UITextDisplay");

            m_characterDefaultNameTextColor.a = 1f;
            m_characterDefaultTextColor.a = 1f;
        }

    }
}