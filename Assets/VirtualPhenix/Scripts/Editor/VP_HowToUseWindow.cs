using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;

namespace VirtualPhenix
{
    public class VP_HTUText
    {
        public string Text;
        public KeyValuePair<bool, int> m_spaces;
        public GUIStyle m_labelType;
    }

    public class VP_HowToUseWindow : VP_EditorWindow<VP_HowToUseWindow>
    {
        public static void ShowWindow(int WelcomeWindowWidth, int WelcomeWindowHeight, params VP_HTUText[] _texts)
        {
            if (m_instance == null)
            {
                EditorWindow editorWindow = GetWindow(typeof(VP_HowToUseWindow), false, " HOW TO USE", true);
                editorWindow.autoRepaintOnSceneChange = true;
                editorWindow.titleContent.image = EditorGUIUtility.IconContent("_Help").image;
                editorWindow.maxSize = new Vector2(WelcomeWindowWidth, WelcomeWindowHeight);
                editorWindow.minSize = new Vector2(WelcomeWindowWidth, WelcomeWindowHeight);
                editorWindow.position = new Rect(Screen.width / 2 + WelcomeWindowWidth / 2, Screen.height / 2, WelcomeWindowWidth, WelcomeWindowHeight);

                m_instance = (VP_HowToUseWindow)editorWindow;
            }

            m_instance.Show();
            Instance.m_howToTexts = new List<VP_HTUText>(_texts);
        }

        protected override void OnGUI()
        {
            if (m_howToTexts != null && m_howToTexts.Count > 0)
            {
                foreach (VP_HTUText t in m_howToTexts)
                {
                    EditorGUILayout.LabelField(t.Text, t.m_labelType);
                    if (t.m_spaces.Key)
                    {
                        for(int i = 0; i < t.m_spaces.Value;i++)
                        {
                            EditorGUILayout.Space();
                        }
                    }
                }
            }
        }

    }
}