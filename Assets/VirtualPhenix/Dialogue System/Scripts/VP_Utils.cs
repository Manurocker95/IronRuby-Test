using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix.Dialog;

namespace VirtualPhenix
{
    public static partial class VP_Utils 
    {
        public static class DialogUtils
        {
            public static string AddZerosToCharacterNumber(int number, int maxCharacter = 3)
            {
                string ret = number.ToString().PadLeft(maxCharacter, '0');



                return ret;
            }

            public static List<VP_DialogTextSymbol> CreateSymbolListFromText(string text)
            {
                var symbolList = new List<VP_DialogTextSymbol>();
                int parsedCharacters = 0;
                while (parsedCharacters < text.Length)
                {
                    VP_DialogTextSymbol symbol = null;

                    // Check for tags
                    var remainingText = text.Substring(parsedCharacters, text.Length - parsedCharacters);
                    if (VP_DialogRichTextTag.StringStartsWithTag(remainingText))
                    {
                        var tag = VP_DialogRichTextTag.ParseNextWithMiddleText(remainingText);
                        symbol = new VP_DialogTextSymbol(tag);
                    }
                    else
                    {
                        symbol = new VP_DialogTextSymbol(remainingText.Substring(0, 1));
                    }

                    parsedCharacters += symbol.Length;
                    symbolList.Add(symbol);
                }

                return symbolList;
            }

            public static void SetVariableToDatabase<T>(string _name, T value, VP_VariableDataBase database)
            {
                if (typeof(T).Equals(typeof(string)))
                {
                    string _val = (string)System.Convert.ChangeType(value, typeof(string));
                    database.AddStringVariable(_name, _val);
                }
                else if (typeof(T).Equals(typeof(int)))
                {
                    int _val = (int)System.Convert.ChangeType(value, typeof(int));
                    database.AddIntVariable(_name, _val);
                }
                else if (typeof(T).Equals(typeof(float)))
                {
                    float _val = (float)System.Convert.ChangeType(value, typeof(float));
                    database.AddFloatVariable(_name, _val);
                }
                else if (typeof(T).Equals(typeof(double)))
                {
                    double _val = (double)System.Convert.ChangeType(value, typeof(double));
                    database.AddDoubleVariable(_name, _val);
                }
                else if (typeof(T).Equals(typeof(bool)))
                {
                    bool _val = (bool)System.Convert.ChangeType(value, typeof(bool));
                    database.AddBoolVariable(_name, _val);
                }
                else if (typeof(T).Equals(typeof(UnityEngine.GameObject)))
                {
                    UnityEngine.GameObject _val = (UnityEngine.GameObject)System.Convert.ChangeType(value, typeof(UnityEngine.GameObject));
                    database.AddGameObjectVariable(_name, _val);
                }
                else if (typeof(T).Equals(typeof(UnityEngine.Object)))
                {
                    UnityEngine.Object _val = (UnityEngine.Object)System.Convert.ChangeType(value, typeof(UnityEngine.Object));
                    database.AddUnityObjectVariable(_name, _val);
                }
                else
                {
                    Debug.LogError("Unknown or not accepted variable type in variable with name " + _name);
                }
            }

            public static FieldData GetVariableValueFromDatabase<T>(string _variableName, T _type, VP_VariableDataBase database)
            {
                return database?.GetVariableValue(_variableName, _type);
            }

            public static FieldData GetVariableValueStrTypeFromDatabase(string _variableName, string _type, VP_VariableDataBase database)
            {
                return database?.GetVariableValueStrType(_variableName, _type);
            }

            public static string GetVariableValueStringFromStrTypeFromDatabase(string _variableName, string _type, VP_VariableDataBase database)
            {
                return GetVariableFieldDataTextValue(database?.GetVariableValueStrType(_variableName, _type));
            }

            public static string GetVariableValueStringFromDatabase<T>(string _variableName, T _type, VP_VariableDataBase database)
            {
                return GetVariableFieldDataTextValue(database?.GetVariableValue(_variableName, _type));
            }

            public static string GetVariableFieldDataTextValue(FieldData _data)
            {
                if (_data == null)
                    return "";

                if (_data is Field<string>)
                {
                    return (_data as Field<string>).Value;
                }
                else if (_data is Field<bool>)
                {
                    return (_data as Field<bool>).Value.ToString();
                }
                else if (_data is Field<int>)
                {
                    return (_data as Field<int>).Value.ToString();
                }
                else if (_data is Field<float>)
                {
                    return (_data as Field<float>).Value.ToString();
                }
                else if (_data is Field<double>)
                {
                    return (_data as Field<double>).Value.ToString();
                }
                else if (_data is Field<GameObject>)
                {
                    return (_data as Field<GameObject>).Value.name;
                }
                else if (_data is Field<UnityEngine.Object>)
                {
                    return (_data as Field<UnityEngine.Object>).Value.name;
                }
                else
                {
                    return "";
                }
            }

            public static string RemoveAllTags(string textWithTags)
            {
                string textWithoutTags = textWithTags;
                textWithoutTags = RemoveUnityTags(textWithoutTags);
                textWithoutTags = RemoveCustomTags(textWithoutTags);

                return textWithoutTags;
            }

            public static string RemoveCustomTags(string textWithTags)
            {
                return RemoveTags(textWithTags, VP_DialogSetup.Tags.CUSTOM_TAGS);
            }

            public static string RemoveUnityTags(string textWithTags)
            {
                return RemoveTags(textWithTags, VP_DialogSetup.Tags.UNITY_TAGS);
            }

            private static string RemoveTags(string textWithTags, params string[] tags)
            {
                string textWithoutTags = textWithTags;
                foreach (var tag in tags)
                {
                    textWithoutTags = VP_DialogRichTextTag.RemoveTagsFromString(textWithoutTags, tag);
                }

                return textWithoutTags;
            }
        }
    }
}
