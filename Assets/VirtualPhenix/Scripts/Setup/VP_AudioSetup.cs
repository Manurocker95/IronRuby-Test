using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix
{
    public static partial class VP_AudioSetup
    {
        //Resources/Audio/
        public const string AUDIO_PATH = "/Audio/";
        public const string PREDEFINED_AUDIO_PATH = "/Database/Audio/DefaultAudioDatabase";
        public enum AUDIO_TYPE
        {
            BGM,
            SFX,
            VOICE
        }

	    public static partial class UI
        {
            public const string BUTTON_HOVER = "buttonOver";
            public const string BUTTON_SELECT = "buttonSelect";
            public const string CONFIRM = "confirm";
            public const string WARNING = "warning";
            public const string TYPE = "type";
            public const string ALERT_SHOW = "alertShow";
        }

        public static partial class General
        {
            public const string BGM_TEST = "BGM_TEST_KEY";
        }     
        
        public static partial class Dialog
        {
            public const string ANSWER_CLIP = "answerClip";
            public const string DIALOGUE_CLIP = "dialogueClip";
        }

	    public static partial class DefaultNames
	    {
	    	public const string BGM_SOURCE_NAME = "BGM Source";
	    	public const string SFX_SOURCE_NAME = "SFX Source";
	    	public const string VOICE_SOURCE_NAME = "Voice Source";
	    	public const string ONE_SHOT_SOURCE_NAME = "One Shot Source";
	    }

    }
}
