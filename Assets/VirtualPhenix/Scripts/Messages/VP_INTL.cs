using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix.Localization;

namespace VirtualPhenix
{
    public static class VP_INTL
    {
        public static string ParseString(string _originalStr, params string[] _args)
        {
            string finalStr = _originalStr;

            try
            {
                for (int i = 0; i < _args.Length; i++)
                {
                    finalStr = _args[i] != null ? finalStr.Replace("{" + (i) + "}", $"{_args[i]}") : finalStr.Replace("{" + (i) + "}", "");
                }

                return finalStr;
            }
            catch(System.Exception e)
            {
                Debug.LogError("INTL ERROR: " + e.StackTrace);
                return finalStr;
            }
        }

        public static string TranslateParseString(string _originalStr, params string[] _args)
        {
            return ParseString(VP_LocalizationManager.GetText(_originalStr), _args);
        }

        public static string INTL(string _originalStr, params string[] _args)
        {
            return ParseString(VP_LocalizationManager.GetText(_originalStr), _args);
        } 
        
        public static string ParseString(this VP_Monobehaviour target, string _originalStr, params string[] _args)
        {
            return ParseString(VP_LocalizationManager.GetText(_originalStr), _args);
        }

        public static string TranslateParseString(this VP_Monobehaviour target, string _originalStr, params string[] _args)
        {
            return ParseString(VP_LocalizationManager.GetText(_originalStr), _args);
        }

        public static string INTL(this VP_Monobehaviour target, string _originalStr, params string[] _args)
        {
            return ParseString(target, VP_LocalizationManager.GetText(_originalStr), _args);
        }
    }
}
