using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VirtualPhenix.Dialog
{
    [System.Serializable]
    public class VP_DialogSetupData
    {
        public string className = "";
        public string variableName = "";
        public string keyName = "";

        public VP_DialogSetupData()
        {

        }

        public VP_DialogSetupData(string _class, string _var, string _key)
        {
            this.className = _class;
            this.variableName = _var;
            this.keyName = _key;
        }

        public void SetData(string _class, string _var, string _key)
        {
            this.className = _class;
            this.variableName = _var;
            this.keyName = _key;
        }
    }

    public static class VP_DialogSetup
    {
        public const string WINDOW_NAME = "Dialog Editor";

        public static class Log
        {
            public const string PREFAB_PATH = "Dialogue/Prefabs/Log/LogObject";
        }         
        
        public static class DialogNodes
        {
            public const string AUDIO_NODE_KEY_PATH = "Dialogue/Data/audioKeyData";
        }    

        public static class NodeColors
        {
            public const string DIALOG_NODE = "#767676";
            public const string LOG_NODE = "#767676";
            public const string BRANCH_NODE = "#175C9D";
            public const string EVENT_NODE = "#870068";
            public const string INIT_EVENT_NODE = "#FF001D";
            public const string SEQUENCE_NODE = "#870068";
            public const string SET_POSITION_NODE = "#004801";
            public const string WAIT_NODE = "#FF2BAE";
            public const string SET_VARIABLE = "#FF2BAE";
            public const string AUDIO_NODE = "#612FEE";
            public const string CHOOSE_NUMBER = "#612FEE";
            public const string RANDOM_NUMBER_NODE = "#612FEE";
        }

        public static class Animations
        {
            public const string FADE_OUT = "fadeOut";
            public const string FADE_IN = "fadeIn";
            public const string IDLE_SHOWN = "idle";
            public const string SHOW_DIRECTLY = "showDirectly";
            public const string HIDE_DIRECTLY = "hideDirectly";
        }


        /// <summary>
        /// The user must only modify this class to add custom events
        /// 
        /// See demo scenes
        /// </summary>
        public static class InitEvents
        {
            public const string TEST_INIT_EVENT_DEMO3 = "start3";
            public const string TEST_INIT_EVENT_DEMO4 = "start4";
            public const string INIT_DEMO_8_NPC_DIALOG = "demoNPC";
            public const string PLAY_MAKER_INIT = "PlayMakerExample";
            public const string SIMPLE_INITIALIZER = "SimpleInitializer";
        }


        public static class Tags
        {
            public const string DELAY = "delay";
            public const string ANIM = "anim";
            public const string ANIMATION = "animation";
            public const string VARIABLE = "var";
            public const string GRAPH_VARIABLE = "graphvar";
            public const string EMOTION = "emotion";

            public const string BLACK = "b";
            public const string ITALIC = "i";
            public const string SIZE = "size";
            public const string COLOR = "color";
            public const string STYLE = "style";
            public const string SPRITE = "sprite";

            public static readonly string[] UNITY_TAGS = new string[] 
            {
                Tags.BLACK,
                Tags.ITALIC,
                Tags.SIZE,
                Tags.COLOR,
                Tags.STYLE,
                Tags.SPRITE
            };

            public static readonly string[] CUSTOM_TAGS = new string[]
            {
                Tags.DELAY,
                Tags.ANIM,
                Tags.ANIMATION,
                Tags.VARIABLE,
                Tags.GRAPH_VARIABLE,
                Tags.EMOTION
            };
        }
        public static class Fields
        {
            // NAMES OF VARIABLES AS THEY ARE SERIALIZED WITH SYSTEM.REFLECTION
            public const string DIALOG_NODE_INPUT = "input";
            public const string DIALOG_NODE_OUTPUT = "output";
            public const string DIALOG_NODE_MULTIPLE_OUTPUT = "outputs";
            public const string DIALOG_NODE_CONDITION_TRUE = "isTrue";
            public const string DIALOG_NODE_CONDITION_FALSE = "isFalse";
            public const string DIALOG_NODE_CONDITIONS = "conditions";
            public const string DIALOG_NODE_ANSWERS = "answers";
            public const string DIALOG_NODE_KEY = "key";
            public const string DIALOG_NODE_AUDIOCLIP = "clip";
            public const string DIALOG_NODE_HIDE_SHOW_INFO = "m_hideInfoInGraph";
            public const string DIALOG_NODE_DIALOG_TYPE = "dialogType";
            public const string DIALOG_NODE_CHARACTER = "characterData";
            public const string DIALOG_NODE_TRIGGER = "trigger";
            public const string DIALOG_NODE_TRIGGER_TIME = "m_triggerTime";
            public const string DIALOG_NODE_TRIGGER_CHARACTER = "m_character";
            public const string DIALOG_NODE_TRIGGER_CHOICE = "m_choice";
            public const string DIALOG_NODE_TRIGGER_SAME_CHOICE = "m_sameNumberChosen";
            public const string DIALOG_NODE_TRIGGER_UNITY = "unityTrigger";
            public const string DIALOG_NODE_TRIGGER_STRING = "triggerStrings";
            public const string DIALOG_NODE_TRIGGER_LIST = "triggerList";
            public const string DIALOG_NODE_START_EVENT_SEL = "keyTest";
            public const string DIALOG_NODE_START_EVENT= "startEvent";
            public const string DIALOG_NODE_ON_START_EVENT= "onStart";
            public const string DIALOG_NODE_WAIT_AUDIO_END= "waitForAudioEnd";
            public const string DIALOG_NODE_TEXT_AND_ANSWERS = "m_answerAtTheSameTime";
            public const string DIALOG_NODE_TEST_KEY= "keyT";
            public const string DIALOG_NODE_WAIT_FOR_INPUT= "waitForInput";
            public const string DIALOG_NODE_SHOW_DIRECTLY = "m_showDirectly";
            public const string DIALOG_NODE_DEFAULT_ANSWER= "m_autoAnswer";
            public const string DIALOG_NODE_TIME_TO_ANSWER = "m_timeForAnswer";
            public const string CHOOSE_NUMBER_NODE_MIN = "m_min";
            public const string CHOOSE_NUMBER_NODE_MAX = "m_max";
            public const string CHOOSE_NUMBER_NODE_NUMBER = "m_number";
            public const string CHOOSE_NUMBER_NODE_COMPARISON = "m_comparison";
            public const string CHOOSE_NUMBER_NODE_MESSAGE = "m_message";
            public const string CHOOSE_NUMBER_NODE_TRANSLATE = "m_translate";
            public const string CHOOSE_NUMBER_NODE_CANCANCEL = "m_canCancel";
            public const string CHOOSE_NUMBER_NODE_CANCELOUTPUT = "ifCancel";
            public const string DEBUG_LOG_NODE_TYPE = "m_logType";

            public const string DIALOG_NODE_AUDIO_ACTION= "m_audioAction";
            public const string DIALOG_NODE_AUDIO_TYPE= "m_audioType";
            public const string DIALOG_NODE_AUDIO_CLIP= "m_audioClip";
            public const string DIALOG_NODE_AUDIO_KEY= "m_audioKey";
            public const string DIALOG_NODE_AUDIO_ADD= "m_addAudio";
            public const string DIALOG_NODE_AUDIO_REMOVE = "m_removeAudio";
            public const string DIALOG_NODE_AUDIO_VOLUME= "m_audioVolume";
            public const string DIALOG_NODE_AUDIO_KEY_DATA= "m_AudiokeyData";
            public const string DIALOG_NODE_AUDIO_LOOP= "m_audioInLoop";
            public const string DIALOG_NODE_AUDIO_PREV_KEY= "m_stopAllPrevious";
            public const string DIALOG_NODE_WAIT_TIME= "m_waitTime";

            public const string DIALOG_NODE_POSITION_DATA= "m_positionData";
            public const string DIALOG_NODE_POSITION_NEW_VALUE = "m_position";
            public const string DIALOG_NODE_USE_DEFAULT_POS = "m_useDefaultPosition";
            public const string DIALOG_NODE_SET_TEXT_POS = "m_setTextPosition";
            public const string DIALOG_NODE_TOP_BOT_MUG = "m_textTopBottomMugshot";
            public const string DIALOG_NODE_LEFT_RIGHT_MUG = "m_textLeftRightMugshot";
            public const string DIALOG_NODE_TOP_BOT_REG = "m_textTopBottomRegular";
            public const string DIALOG_NODE_LEFT_RIGHT_REG = "m_textLeftRightRegular";
            public const string DIALOG_NODE_TOP_BOT_BG = "m_bgTopBottom";
            public const string DIALOG_NODE_LEFT_RIGHT_BG = "m_bgLeftRight";
            public const string DIALOG_NODE_SET_BG_POS = "m_setBGPosition";
            public const string DIALOG_NODE_USE_LOCALIZATION = "m_useLocalization";
            public const string DIALOG_NODE_USE_KEY_FOR_LOCALIZATION = "m_useKey";
            public const string DIALOG_NODE_SET_GO_POS = "m_useGameObjectPosition";
            public const string DIALOG_NODE_SET_GO_NAME = "m_gameObjectName";
            public const string VARIABLE_LIST = "graphVariableList";
            public const string GLOBAL_VARIABLE_LIST = "globalVariableList";
            public const string SET_VARIABLE_VARIABLES = "m_variables";
            public const string SAVE_VARIABLES_AS_PLAYERPREFS = "m_saveAsPlayerPrefs";




        }

    }
}
