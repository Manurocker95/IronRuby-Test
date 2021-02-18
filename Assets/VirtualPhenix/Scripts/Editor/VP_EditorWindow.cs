using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace VirtualPhenix
{
    public class VP_EditorWindow<T> : EditorWindow where T : EditorWindow
    {
        protected static EditorWindow m_instance;
        public static T Instance { get { return (T)m_instance; } }

        protected List<VP_HTUText> m_howToTexts;
        protected virtual int HowToWindowWidth { get { return 488; } }
        protected virtual int HowToWindowHeight { get { return 600; } }
        public virtual string WindowName { get { return "VP Window"; } }

        public virtual void CreateInstance()
        {
            m_instance = this;
        }

        public virtual void SetWindowName()
        {
            titleContent = new GUIContent(WindowName);
        }

        public virtual void InitHowToTextList()
        {
            m_howToTexts = new List<VP_HTUText>();
        }

        protected virtual void DrawButtonWithColor(string _text, UnityEngine.Events.UnityAction _callback, Color _color, Color _textColor)
        {
            var oldColor = GUI.backgroundColor;
            GUI.backgroundColor = _color;
            var style = new GUIStyle(GUI.skin.button);
            style.normal.textColor = _textColor;
            if (GUILayout.Button(_text, style))
            {
                _callback.Invoke();
            }
            GUI.backgroundColor = oldColor;
        }

        public virtual void CreateHowToText()
        {
            if (m_howToTexts == null)
                InitHowToTextList();

            CreateHowToTitle(ref m_howToTexts, "THIS WINDOW");

            m_howToTexts.Add(new VP_HTUText()
            {
                Text = "* This Window has no HOW TO USE text already defined...",
                m_labelType = VP_EditorWindowStyleSetup.RegularTextStyle,
                m_spaces = new KeyValuePair<bool, int>(true, 1)
            });

        }

        public virtual void CreateHowToTitle(ref List<VP_HTUText> m_texts, string title =" THIS WINDOW")
        {
            m_texts.Add(new VP_HTUText() { Text = "HOW TO USE " + title + " WINDOW!", m_labelType = VP_EditorWindowStyleSetup.LargeTextStyle, m_spaces = new KeyValuePair<bool, int>(true, 1) });
        }

        protected virtual void OnDestroy()
        {
            if (m_instance == this)
            {
                m_instance = null;
            }
        }

        protected virtual void OnGUI()
        {
            if (GUILayout.Button("How To Use "+WindowName))
            {
                CreateHowToText();
            }
        }
    }
}