using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Localization
{
    public enum LANGUAGE_PARSER
    {
        CSV,
        XML,
        JSON
    }

    public static partial class VP_LocalizationSetup
    {
        public const string DEFAULT_FILE_NAME = "default";
        public const string FILE_PATH = "/VirtualPhenix/Dialogue System/Resources/";
	    public const string LOCALIZATION_CONFIG_FILE_NAME = "locConfig.VPData";

        public static class JsonParser
        {
            public const string JSON_ROOT = "root";
            public const string JSON_KEY = "key";
            public const string JSON_TEXT = "text";
        }

        public static class XMLParser
        {
            public const string TABLE_NAME = "Table1";
            public const string NODE_KEY = "key";
            public const string NODE_TEXT = "text";
        }


        public static class Folder
        {
            public const string LOCALIZATION_FOLDER_CSV = "Files/Localization/CSV/";
            public const string LOCALIZATION_FOLDER_XML = "Files/Localization/XML/";
            public const string LOCALIZATION_FOLDER_JSON = "Files/Localization/JSON/";
        }

        public static class Extension
        {
            public const string JSON = ".json";
            public const string CSV = ".csv";
            public const string XML = ".xml";
        }

        public static class PlayerPrefs
        {
            public const string LAST_LANGUAGE = "LastLanguage";
            public const string LOCALIZATION_PARSER = "LocalizationParser";
        }

    }

}
