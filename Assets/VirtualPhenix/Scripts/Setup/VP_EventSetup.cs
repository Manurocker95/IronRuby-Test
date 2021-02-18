using UnityEngine;

namespace VirtualPhenix
{
    public static partial class VP_EventSetup
    {
        public static class HealthBar
        {
            public const string HEAL_PLAYER = "HEAL_PLAYER";
            public const string DAMAGE_PLAYER = "DAMAGE_PLAYER";
        }

        public static class Localization
        {
            public const string TRANSLATE_TEXTS = "TranslateTextsEvent";
        }
        public static class Reward
        {
            public const string STORE_NEXT_REWARD_TIME = "STORE_NEXT_REWARD_TIME";
        }
       
        public static class Input
        {
            public const string CAN_USE_INPUT = "CanUseInputEvent";
            public const string INTERACT_BUTTON = "INTERACT_BUTTON";
            public const string UP_ANSWER = "UP_ANSWER";
            public const string DOWN_ANSWER = "DOWN_ANSWER";
            public const string RIGHT_ANSWER = "RIGHT_ANSWER";
            public const string LEFT_ANSWER = "LEFT_ANSWER";
            public const string CANCEL_BUTTON = "CANCEL_BUTTON";
        }

        public static class Privacy
        {
            public const string WEBSITE = "http://manuelrodriguezmatesanz.com";
        } 
                
        public static class Init
        {
            public const string AFTER_INIT = "AFTER_INITm";
        } 
        
        public static class Dialog
        {
            public const string TRIGGER_EMOTION = "TriggerEmotionDialog";
        }

        public static class Camera
        {
            public const string TAKE_PICTURE = "takePicEv";
            public const string LOAD_PICTURE = "loadPicEv";
            public const string SET_AVERAGE_COLOR = "SetAvColEv";
            public const string SHAKE_CAMERA = "ShakeRegularCam";
	        public const string SET_STACK_TO_CAMERAS = "SetCameraToStack";
        }


        public static class Menu
        {
            public const string EXIT = "ExitGameEvent";
            public const string SHOW_MENU = "ShowMenuEv";
            public const string SELECT_CARD = "selectCardEv";
            public const string RESET_COUNTER_TITLE = "resetTimesCounter";
            public const string SHOW_INIT_DIALOG = "showInitDialog";
        }

        public static class Dialogs
        {
            public const string SHOW_HAND_MAIN = "showHandInMain";
            public const string SHOW_HAND_SHARE = "showShare";
            public const string SHOW_HAND_PROF = "showProf";
            public const string SHOW_PROGRESS = "showProgress";
            public const string SHOW_TAKE_PICS = "showTakePictures";
            public const string SHOW_GALLERY = "showGallery";
            public const string SHOW_BUTTONS_PROF = "showDownBtns";
            public const string SHOW_FIRST_BUTTONS = "showButtonsInMain";
            public const string SHOW_BLOCKER = "showBlocker";
            public const string HIDE_BLOCKER = "hideBlocker";
            public const string HIDE_TUTORIAL = "hideTutorial";

        }

        public static class Tutorial
        {
            public const string CHECK_TUTORIAL = "CHECK_TUTORIAL_STAGE_SEL";
        }

        public static class Scene
        {
            public const string LOAD_SCENE = "LoadSceneEvent";
            public const string RELOAD_CURRENT_SCENE = "ReloadSceneEvent";
            public const string RELOADED_CURRENT_SCENE = "ReloadedSceneEvent";

        }

        public static class Save
        {
            public const string SAVE_GAME = "SaveGameEvent";
            public const string SET_FIRST_TIME = "SetFirstTimeEv";
            public const string LOAD_GAME = "LoadGameEvent";
            public const string DATA_LOADED = "DataLoadedEvent";
            public const string DELETE_SAVE_FILE = "deleteSaveFile";

        }

        public static class Game
        {
            public const string START_GAME = "StartGameEvent";

            public const string ADD_SCORE = "AddScoreEvent";
            public const string START_COUNTDOWN = "startCountdownEv";
            public const string GAME_OVER = "GAME_OVEREv";

            public const string UPDATE_SCORE_UI = "UPDATE_SCORE_UIEvent";
            public const string UPDATE_BESTSCORE_UI = "UPDATE_BESTSCORE_UIEvent";
            public const string LOAD_LEVEL = "LOAD_LEVEL";
            public const string LEVEL_EXIT = "LEVEL_EXIT";
            public const string SAVE_LEVEL = "SAVE_LEVEL";
        }

        public static class Settings
        {
            public const string SET_SETTINGS = "SetSettingsEvent";
            public const string LANGUAGE = "SetlanguageSettingsEvent";
            public const string LOAD_SAVED_SETTINGS = "LoadSavedSettingsEvent";
        }

        public static class Audio
        {
            public const string PLAY_AUDIO = "PlayAudioEvent";
            public const string PLAY_AUDIO_ITEM = "PlayAudioItemEvent";
            public const string PLAY_AUDIO_ITEM_BY_KEY = "PlayAudioItemByKeyEvent";
            public const string STOP_AUDIO_ITEM = "StopAudioItemEvent";
            public const string PLAY_ONE_SHOT = "PlayOneShotEvent";
            public const string CLEAR_NULL_AUDIO = "ClearNullAudiosEvent";
            public const string REMOVE_ITEM = "RemoveAudioItemEvent";
        }

        public static class Monetization
        {
            public const string REWARDED_AD_IS_READY_TO_SHOW = "REWARDED_AD_IS_READY_TO_SHOW";
            public const string REWARDED_AD_IS_STARTING_TO_LOAD = "REWARDED_AD_IS_STARTING_TO_LOAD";

        }
        public static class Store
        {
            public const string UPDATE_COINS_UI = "UPDATE_COINS_UI";
            public const string SAVE_STORE_VALUES = "SAVE_STORE_VALUES";
            public const string REGISTER_DISPLAY_ONCLICKHANDLER = "REGISTER_DISPLAY_ONCLICKHANDLER";
            public const string REFRESH_UNLOCKABLE_UI_DISPLAYS = "REFRESH_UNLOCKABLE_UI_DISPLAYS";
            public const string UNLOCKED_ELEMENT = "UNLOCKED_ELEMENT";
            public const string ENTERED_STORE = "ENTERED_STORE";

        }
    }
}
