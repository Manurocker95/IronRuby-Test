using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace VirtualPhenix
{
    public class VP_PlayerPrefsEditor : VP_EditorWindow<VP_PlayerPrefsEditor>
    {
        public enum PLAYER_PREF_TYPE
        {
            STRING,
            INT,
            FLOAT
        }

        protected PLAYER_PREF_TYPE m_fieldType = PLAYER_PREF_TYPE.STRING;
        protected string m_setKey = "";
        protected string m_setVal = "";
        protected string m_error = null;

        public override string WindowName => "Player Prefs Editor";


        [MenuItem("Virtual Phenix/Window/Player Prefs Editor %F7")]
        public static void OpenWindow()
        {
            m_instance = (VP_PlayerPrefsEditor)EditorWindow.GetWindow(typeof(VP_PlayerPrefsEditor));
            Instance.SetWindowName();
            m_instance.Show();
        }

        public virtual void SetPlayerPref(PLAYER_PREF_TYPE _type, string _key, string _value)
        {
            switch (_type)
            {
                case PLAYER_PREF_TYPE.INT:
                    int result;
                    if (!int.TryParse(_value, out result))
                    {

                        m_error = "Invalid input \"" + _value + "\"";
                        return;

                    }

                    PlayerPrefs.SetInt(_key, result);
                    break;
                case PLAYER_PREF_TYPE.FLOAT:
                    float resultf;
                    if (!float.TryParse(_value, out resultf))
                    {

                        m_error = "Invalid input \"" + _value + "\"";
                        return;

                    }

                    PlayerPrefs.SetFloat(_key, resultf);
                    break;
                default:
                    PlayerPrefs.SetString(_key, _value);
                    break;
            }

            
            PlayerPrefs.Save();
        }

        public virtual void GetPlayerPref(PLAYER_PREF_TYPE _type, string _key, out string _value)
        {
            switch (_type)
            {
                case PLAYER_PREF_TYPE.INT:
                    _value = PlayerPrefs.GetInt(_key).ToString();
                    break;
                case PLAYER_PREF_TYPE.FLOAT:
                    _value = PlayerPrefs.GetFloat(_key).ToString();
                    break;
                default:
                    _value = PlayerPrefs.GetString(_key);
                    break;
            }
        }

        protected virtual void DeleteAllPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
        }

        protected virtual void DeletePlayerPref(string _key)
        {
            PlayerPrefs.DeleteKey(_key);
            PlayerPrefs.Save();
        }


        public override void CreateHowToText()
        {
            if (m_howToTexts == null)
                InitHowToTextList();

            CreateHowToTitle(ref m_howToTexts, "PLAYER PREFS EDITOR");

            m_howToTexts.Add(new VP_HTUText()
            {
                Text = "This window can configure Player Prefs for Virtual Phenix Framework.\n\n" +
                        "You only need to define the string key and the value you want to set. \n\n" +
                        "If you need to debug a PlayerPref value, click on Get PlayerPref and. \n\n" +
                        "the value will appear in SetVal field.",
                m_labelType = VP_EditorWindowStyleSetup.RegularTextStyle,
                m_spaces = new KeyValuePair<bool, int>(true, 2)
            });

            m_howToTexts.Add(new VP_HTUText()
            {
                Text = "You can delete Player Prefs from your computer by clicking the Delete buttons.",
                m_labelType = VP_EditorWindowStyleSetup.RegularTextStyle,
                m_spaces = new KeyValuePair<bool, int>(true, 2)
            });

            VP_HowToUseWindow.ShowWindow(HowToWindowWidth, HowToWindowHeight, m_howToTexts.ToArray());
        }


        protected override void OnGUI()
        {
            base.OnGUI();

            EditorGUILayout.Space();

            m_fieldType = (PLAYER_PREF_TYPE)EditorGUILayout.EnumPopup("Key Type", m_fieldType);
            m_setKey = EditorGUILayout.TextField("Key to Set", m_setKey);
            m_setVal = EditorGUILayout.TextField("Value to Set", m_setVal);

            if (m_error.IsNotNullNorEmpty())
            {
                EditorGUILayout.HelpBox(m_error, MessageType.Error);
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Set Key"))
            {
                SetPlayerPref(m_fieldType, m_setKey, m_setVal);
                m_error = null;
            }
          
            if (GUILayout.Button("Get Key"))
            {
                GetPlayerPref(m_fieldType, m_setKey, out m_setVal);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();

            var oldColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.red;
            var style = new GUIStyle(GUI.skin.button);
            style.normal.textColor = Color.white;

            if (GUILayout.Button("Delete Key", style))
            {
                DeletePlayerPref(m_setKey);
            }

            if (GUILayout.Button("Delete All Keys", style))
            {
                DeleteAllPlayerPrefs();
            }
            EditorGUILayout.EndHorizontal();
            GUI.backgroundColor = oldColor;
        }
    }
}
