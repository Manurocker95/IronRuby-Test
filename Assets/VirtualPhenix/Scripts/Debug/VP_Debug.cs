using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix
{
    /// <summary>
    /// Type of message. Custom may show an icon
    /// </summary>
    public enum DEBUG_TYPE
    {
        REGULAR,
        WARNING,
        ERROR,
        EXCEPTION,
        CUSTOM
    }
    /// <summary>
    /// All colors for <color=
    /// </summary>
    public enum DEBUG_COLOR
    {
        NONE,
        AQUA,
        BLACK,
        BLUE,
        BROWN,
        CYAN,
        DARKBLUE,
        FUCHSIA,
        GREEN,
        GREY,
        LIGHTBLUE,
        LIME,
        MAGENTA,
        MAROON,
        NAVY,
        OLIVE,
        ORANGE,
        PURPLE,
        RED,
        SILVER,
        TEAL,
        WHITE,
        YELLOW
    }   

    public class VP_Debug
    {
        private static ILogger logger = Debug.unityLogger;
        private static string m_logTag = "[VirtualPhenix]";
        public static bool DebugInBuild = true;
        public static bool m_useTag = false;

        public static void CleanConsole()
        {
#if !UNITY_EDITOR
            if (!DebugInBuild)
                return;
#endif

            UnityEngine.Debug.ClearDeveloperConsole();
        }

        /// <summary>
        /// Debug.Log
        /// </summary>
        /// <param name="_text"></param>
        /// <param name="_color"></param>
        /// <param name="_context"></param>
        public static void Log(object _text, DEBUG_COLOR _color = DEBUG_COLOR.NONE, string customTag = "", bool _customIcon = false, UnityEngine.Object _context = null)
        {
#if !UNITY_EDITOR
            if (!DebugInBuild)
                return;
#endif
            string tag = m_useTag ? (string.IsNullOrEmpty(customTag) ? m_logTag : customTag) : "";
            ShowLog(_text, _color, DEBUG_TYPE.REGULAR, tag, _customIcon, false, false, _context);
        }

        /// <summary>
        /// Log Warning
        /// </summary>
        /// <param name="_text"></param>
        /// <param name="_color"></param>
        /// <param name="_context"></param>
        public static void LogWarning(object _text, DEBUG_COLOR _color = DEBUG_COLOR.NONE, string customTag = "", bool _customIcon = false, UnityEngine.Object _context = null)
        {
#if !UNITY_EDITOR
            if (!DebugInBuild)
                return;
#endif
            string tag = m_useTag ? (string.IsNullOrEmpty(customTag) ? m_logTag : customTag) : "";
            ShowLog(_text, _color, DEBUG_TYPE.WARNING, tag, _customIcon, false, false, _context);
        }

        /// <summary>
        /// Log error
        /// </summary>
        /// <param name="_text"></param>
        /// <param name="_color"></param>
        /// <param name="_context"></param>
        public static void LogError(object _text, DEBUG_COLOR _color = DEBUG_COLOR.NONE, string customTag = "", bool _customIcon = false, UnityEngine.Object _context = null)
        {
#if !UNITY_EDITOR
            if (!DebugInBuild)
                return;
#endif
            string tag = m_useTag ? string.IsNullOrEmpty(customTag) ? m_logTag : customTag : "";
            ShowLog(_text, _color, DEBUG_TYPE.ERROR, tag, _customIcon, false, false, _context);
        }

        public static void LogCustom(object _text, DEBUG_COLOR _color = DEBUG_COLOR.NONE, string customTag = "", DEBUG_TYPE _type = DEBUG_TYPE.CUSTOM, UnityEngine.Object _context = null)
        {
#if !UNITY_EDITOR
            if (!DebugInBuild)
                return;
#endif
            ShowLog(_text, _color, _type, customTag, true);
        }

        /// <summary>
        /// Debug log with color
        /// </summary>
        /// <param name="_text"></param>
        public static void ShowLog(object _text, DEBUG_COLOR _color = DEBUG_COLOR.WHITE, DEBUG_TYPE _type = DEBUG_TYPE.REGULAR, string _customTag = "", bool _customIcon = false, bool _black = false, bool _italic = false, UnityEngine.Object _context = null)
        {
#if !UNITY_EDITOR
            if (!DebugInBuild)
                return;
#endif
            string customTag = _customTag;
            string colorStr = _color != DEBUG_COLOR.NONE ? "<color=" + _color.ToString().ToLower() + ">" : "";
            string endColor = _color != DEBUG_COLOR.NONE ? "</color>" : "";
            string black = _black ? "<b>" : "";
            string endBlack = _black ? "<b>" : "";
            string italic = _italic ? "<i>" : "";
            string endItalic = _italic ? "<i>" : "";

            switch (_type)
            {
                case DEBUG_TYPE.REGULAR:
                    
                    if (_context != null)
                        logger.Log(customTag, colorStr + black + italic + _text + endItalic + endBlack + endColor, _context);
                    else
                        logger.Log(customTag, colorStr + black + italic + _text + endItalic + endBlack + endColor);
                    break;
                case DEBUG_TYPE.WARNING:
                    if (_context != null)
                        logger.LogWarning(customTag, colorStr + black + italic + _text + endItalic + endBlack + endColor, _context);
                    else
                        logger.LogWarning(customTag, colorStr + black + italic + _text + endItalic + endBlack + endColor);
                    break;
                case DEBUG_TYPE.ERROR:
                    if (_context != null)
                        logger.LogError(customTag, colorStr + black + italic + _text + endItalic + endBlack + endColor, _context);
                    else
                        logger.LogError(customTag, colorStr + black + italic + _text + endItalic + endBlack + endColor);
                    break;
                case DEBUG_TYPE.EXCEPTION:
                    if (_context != null)
                        logger.LogException(new System.Exception(colorStr + black + italic + _text + endItalic + endBlack + endColor), _context);
                    else
                        logger.LogException(new System.Exception(colorStr + black + italic + _text + endItalic + endBlack + endColor));
                    break;
                case DEBUG_TYPE.CUSTOM:
                    if (_context != null)
                        logger.Log("[CustomIcon]"+ customTag, colorStr + black + italic + _text + endItalic + endBlack + endColor, _context);
                    else
                        logger.Log("[CustomIcon]" + customTag, colorStr + black + italic + _text + endItalic + endBlack + endColor);
                    break;
            }
        }
    }
}
