using UnityEngine;

namespace VirtualPhenix
{
    /// <summary>
    /// General setup 
    /// </summary>
    public static partial class VP_GameSetup
    {

        public static partial class Misc
        {
            public static partial class GameplayIds
            {
                public const string BLOOD_DECAL_ID = "BloodDecal";
            }
        }

        public static partial class General
        {
            public const bool SLEEP = false;
        }

        public static partial class Player
        {

        }

        public static partial class Animations
        {

        }

        public static partial class PlayerPrefs
        {
            public const string LAST_LANGUAGE = "PE_LastLanguagePP";
            public const string LOCALIZATION_PARSER = "LocalizationParserPP";
        }

        public static partial class Layers
        {
            public const string INTERACTABLE = "Interact";
        }

        public static partial class Points
        {

        }

        public static partial class Tags
        {
            public const string PLAYER = "Player";
            public const string WATER = "Water";
            public const string WALL = "Wall";
        }

        public static partial class GameSettings
        {
            /// <summary>
            /// We want a binary JSON? If not encrypted, can be easily modified using Visual Code
            /// </summary>
            public const bool USE_BINARY_SAVE_FILES = false;
            /// <summary>
            /// Encrypt the save file using DES Encryption? The best option on final build is 
            /// encrypt and use binary - It will be a "double trouble" file! 
            /// </summary>
            public const bool ENCRYPT_SAVE_FILES = false;

            public const int DOWN_COLLIDER_HEIGHT = -50;

            public const bool SNAP_MOTION_BLUR = false;
            public const float COUNTDOWN = 3f;
            public const int MAX_THUMBNAIL_SIZE = 1024;


        }
    }

}
