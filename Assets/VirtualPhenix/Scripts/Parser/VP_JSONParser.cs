using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix.Localization;
using VirtualPhenix.SimpleJSON;
using System.Linq;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace VirtualPhenix
{

    public static class VP_JSONParser
    {
#if UNITY_EDITOR
        public static void RemoveFromJSON(TextAsset json, string _key)
        {
            JSONNode info = JSON.Parse(json.text);
            info.Remove(_key);
            info.SaveToFile(AssetDatabase.GetAssetPath(json));
        }
        public static void AddToJSON(TextAsset json, string _key, string _text)
        {
            JSONNode info = JSON.Parse(json.text);

            string rootKey = VP_LocalizationSetup.JsonParser.JSON_ROOT;
            string keyKey = VP_LocalizationSetup.JsonParser.JSON_KEY;
            string textKey = VP_LocalizationSetup.JsonParser.JSON_TEXT;

            info[rootKey][info[rootKey].Count][keyKey] = _key;
            info[rootKey][info[rootKey].Count][textKey] = _text;
            info.SaveToFile(AssetDatabase.GetAssetPath(json));
        }
        public static void ReplaceInJSON(TextAsset json,string _key, string _newText)
        {
            if (json == null)
            {
                Debug.LogError("JSON ERROR: NULL");
                return;
            }

            Dictionary<string, VP_TextItem> tempDictionary = new Dictionary<string, VP_TextItem>();
            JSONNode info = JSON.Parse(json.text);

            string rootKey = VP_LocalizationSetup.JsonParser.JSON_ROOT;
            string keyKey = VP_LocalizationSetup.JsonParser.JSON_KEY;
            string textKey = VP_LocalizationSetup.JsonParser.JSON_TEXT;

            for (int i = 0; i < info[rootKey].Count; i++)
            {
                if (!string.IsNullOrEmpty(_key) && !string.IsNullOrEmpty(_newText))
                {
                    if (_key == info[rootKey][i][keyKey])
                    {
                        info[rootKey][i][textKey] = _newText;
                        break;
                    }
                }
            }

            info.SaveToFile(AssetDatabase.GetAssetPath(json));
        }
#endif

        public static VP_TextItemDictionary ParseTextItemJSON(TextAsset json)
        {
            return new VP_TextItemDictionary(ParseJSON(json));
        }

        public static Dictionary<string, VP_TextItem> ParseJSON(TextAsset json)
        {
            if (json == null)
            {
                Debug.LogError("JSON ERROR: NULL");
                return null;
            }

            Dictionary<string, VP_TextItem> tempDictionary = new Dictionary<string, VP_TextItem>();
            JSONNode info = JSON.Parse(json.text);
            VP_TextItem item = null;
            string key = "";
            string text = "";

            string rootKey = VP_LocalizationSetup.JsonParser.JSON_ROOT;
            string keyKey = VP_LocalizationSetup.JsonParser.JSON_KEY;
            string textKey = VP_LocalizationSetup.JsonParser.JSON_TEXT;

            for (int i = 0; i < info[rootKey].Count; i++)
            {
                key = info[rootKey][i][keyKey];
                text = info[rootKey][i][textKey];

                if (tempDictionary.ContainsKey(key))
                {
                    Debug.LogError("[Json Parse] Error! the key must be unique.");
                    Debug.LogError("Name: " + key);
                    continue;
                }

                item = new VP_TextItem(key, text);
                tempDictionary.Add(key, item);
            }

            return tempDictionary;
        }
    }
}

