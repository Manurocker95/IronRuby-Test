using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace VirtualPhenix
{
    public static class VP_EditorWindowStyleSetup
    {
        private static GUIStyle m_largeTextStyle;
        public static GUIStyle LargeTextStyle
        {
            get
            {
                if (m_largeTextStyle == null)
                {
                    m_largeTextStyle = new GUIStyle(UnityEngine.GUI.skin.label)
                    {
                        richText = true,
                        wordWrap = true,
                        fontStyle = FontStyle.Bold,
                        fontSize = 14,
                        alignment = TextAnchor.MiddleLeft,
                        padding = new RectOffset() { left = 0, right = 0, top = 0, bottom = 0 }
                    };
                }
                return m_largeTextStyle;
            }
        }


        private static GUIStyle m_regularTextStyle;
        public static GUIStyle RegularTextStyle
        {
            get
            {
                if (m_regularTextStyle == null)
                {
                    m_regularTextStyle = new GUIStyle(UnityEngine.GUI.skin.label)
                    {
                        richText = true,
                        wordWrap = true,
                        fontStyle = FontStyle.Normal,
                        fontSize = 12,
                        alignment = TextAnchor.MiddleLeft,
                        padding = new RectOffset() { left = 0, right = 0, top = 0, bottom = 0 }
                    };
                }
                return m_regularTextStyle;
            }
        }
        private static GUIStyle m_footerTextStyle;
        public static GUIStyle FooterTextStyle
        {
            get
            {
                if (m_footerTextStyle == null)
                {
                    m_footerTextStyle = new GUIStyle(EditorStyles.centeredGreyMiniLabel)
                    {
                        alignment = TextAnchor.LowerCenter,
                        wordWrap = true,
                        fontSize = 12
                    };
                }
                return m_footerTextStyle;
            }
        }
    }
}
