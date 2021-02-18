using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Net;
using System;
using VirtualPhenix.Dialog;

namespace VirtualPhenix.Localization
{
    public class VP_LocalizationWindowEditor : VP_EditorWindow<VP_LocalizationWindowEditor>
    {
        private LANGUAGE_PARSER m_languageParser = LANGUAGE_PARSER.CSV;
        private string m_localizationKey;
        private string m_localizationVar;
        private string m_localizationKeyClass;
        private Dictionary<string, TextAsset> languageFiles;
        private List<SystemLanguage> m_activeLanguages;
        private List<string> m_localizationTexts;
        private SystemLanguage m_languageToAdd = SystemLanguage.English;
        private LANGUAGE_PARSER m_oldLanguageParser = LANGUAGE_PARSER.CSV;
        private Vector2 m_scrollPos;
        private SystemLanguage m_autoTranslateOriginal = SystemLanguage.English;
        private VP_DialogKey m_dialogKey;
        private string m_dialogKeyStr;
        private bool m_forceAddAuto = false;
        private string m_ID;

        public override string WindowName => "Localization Editor";
        
        /// <summary>
        /// Creates the custom window :D
        /// </summary>
        [MenuItem("Virtual Phenix/Window/Localization Editor %F3")]
        public static void ShowWindow()
        {
            GetWindow<VP_LocalizationWindowEditor>("Localization Editor");
        }

        private void Awake()
        {
            int lastParser = PlayerPrefs.GetInt(VP_LocalizationSetup.PlayerPrefs.LOCALIZATION_PARSER, -1);
            m_languageParser = lastParser == -1 ? LANGUAGE_PARSER.CSV : (LANGUAGE_PARSER)lastParser;
            m_oldLanguageParser = m_languageParser;

            m_activeLanguages = new List<SystemLanguage>();
            m_localizationTexts = new List<string>();
            LoadLocalizationFiles();
        }

        public override void CreateHowToText()
        {
            if (m_howToTexts == null)
                InitHowToTextList();

            CreateHowToTitle(ref m_howToTexts, "LOCALIZATION EDITOR");

            m_howToTexts.Add(new VP_HTUText()
            {
                Text = "This window will help you to localize easier than modifying files by hand.",
                m_labelType = VP_EditorWindowStyleSetup.RegularTextStyle,
                m_spaces = new KeyValuePair<bool, int>(true, 2)
            });

            VP_HowToUseWindow.ShowWindow(HowToWindowWidth, HowToWindowHeight, m_howToTexts.ToArray());
        }


        /// <summary>
        /// On GUI: Paint gui stuff
        /// </summary>
        protected override void OnGUI()
        {
            base.OnGUI();
            EditorGUILayout.Space();

            GUILayout.Label("Localization Editor", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("", MessageType.Info);
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            if (VP_DialogueGraphEditor.GetID != 0)
            {
                if (GUILayout.Button("Dialog Editor"))
                {
                    EditorApplication.ExecuteMenuItem("Virtual Phenix/Dialogue Editor");
                }
            }

            if (GUILayout.Button("Open Localization Folder"))
            {
                OpenLocalizationFolder();
            }
            EditorGUILayout.EndHorizontal();
#if UNITY_EDITOR_WIN
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Open Text Setup Script"))
            {
                System.Diagnostics.Process.Start(Application.dataPath + "/VirtualPhenix/Scripts/Setup/VP_TextSetup.cs");
            }
            if (GUILayout.Button("Open Dialog Setup Script"))
            {
                System.Diagnostics.Process.Start(Application.dataPath + "/VirtualPhenix/Dialogue System/Scripts/Setup/VP_DialogSetup.cs");
               
            }
            if (GUILayout.Button("Open Event Setup Script"))
            {
                System.Diagnostics.Process.Start(Application.dataPath + "/VirtualPhenix/Scripts/Setup/VP_EventSetup.cs");
            }
            EditorGUILayout.EndHorizontal();
#endif
            m_scrollPos = EditorGUILayout.BeginScrollView(m_scrollPos, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("--- LOCALIZATION SETTINGS --- ", EditorStyles.centeredGreyMiniLabel);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.Space();
            m_languageParser = (LANGUAGE_PARSER)EditorGUILayout.EnumPopup("Parser type:", m_languageParser);

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("--- LANGUAGE --- ", EditorStyles.centeredGreyMiniLabel);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();

            m_languageToAdd = (SystemLanguage)EditorGUILayout.EnumPopup("New Language:", m_languageToAdd);

            if (m_languageParser != m_oldLanguageParser)
            {
                OnLanguageParserChange();
            }


            if (GUILayout.Button("Add Language"))
            {
                AddLanguage();
            }

            GUILayout.EndHorizontal();


            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("--- ADD TO FILES--- ", EditorStyles.centeredGreyMiniLabel);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.Space();
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            m_localizationKey = EditorGUILayout.TextField("Localization key:", m_localizationKey);
            //---------------
            if (m_dialogKey == null)
            {
                Debug.Log("No current Key Test");
                RefreshInitKey();
            }

            if (GUILayout.Button("Select Key"))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("None"), m_dialogKey == null, () => { m_localizationKey = ""; });

                string path = Application.dataPath + "/VirtualPhenix/Scripts/Setup/VP_TextSetup.cs";

                List<VP_DialogSetupData> setupTexts = VP_SetupParser.ParseSetupInList(path);

                if (setupTexts != null)
                {
                    foreach (VP_DialogSetupData data in setupTexts)
                    {
                        GUIContent content = new GUIContent(data.className + "/" + data.variableName);
                        VP_DialogKey newKey = ScriptableObject.CreateInstance<VP_DialogKey>();
                        newKey.key = data.keyName;
                        menu.AddItem(content, newKey.key == m_dialogKey.key, () => { m_localizationKey = newKey.key; });
                    }

                }
                menu.ShowAsContext();
            }

            //---------------
            GUILayout.EndHorizontal();
            if (m_activeLanguages.Count > 0 && m_localizationTexts.Count > 0)
            {
                for (int i = 0; i < m_activeLanguages.Count; i++)
                {
                    GUILayout.BeginVertical();
                    GUILayout.BeginHorizontal();
                    m_localizationTexts[i] = EditorGUILayout.TextField(m_activeLanguages[i].ToString() + " text", m_localizationTexts[i]);
                    if (GUILayout.Button("Delete language"))
                    {
                        DeleteLanguageFile(i);
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                }
            }

            GUILayout.EndVertical();
            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();
            m_autoTranslateOriginal = (SystemLanguage)EditorGUILayout.EnumPopup("Original Auto-Lang:", m_autoTranslateOriginal);
            if (GUILayout.Button("Auto-Translate Key"))
            {
                AutoTranslateKeyToAvailableLanguages(m_forceAddAuto);
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            m_forceAddAuto = EditorGUILayout.Toggle("Force Add", m_forceAddAuto);
            if (GUILayout.Button("Auto-Translate Language to the rest"))
            {
                AutoTranslateFirstLanguageToAvailableLanguages(m_forceAddAuto);
            }
            GUILayout.EndHorizontal();
            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Text"))
            {
                AddKeyToAllLanguages(false);
            }
            if (GUILayout.Button("Replace Text"))
            {
                AddKeyToAllLanguages(true);
            }
            if (GUILayout.Button("Remove Text"))
            {
                RemoveKeyFromAllLanguages();
            }
            if (GUILayout.Button("Load Texts"))
            {
                LoadTextFromKey();
            }
            GUILayout.EndHorizontal();
            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add GameObject name as Key"))
            {
                AddSelectionAsKeys(false);
            }
            if (GUILayout.Button("Replace GameObject name as Key"))
            {
                AddSelectionAsKeys(true);
            }
            GUILayout.EndHorizontal();
            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Refresh Languages"))
            {
                RefreshLanguages();
            }

            GUILayout.EndHorizontal();
            EditorGUILayout.EndScrollView();
        }

        #region LOCALIZATION

        public void RefreshInitKey()
        {
            if (string.IsNullOrEmpty(m_ID))
                m_ID = VP_Utils.CreateID();

            if (m_dialogKey == null)
            {
                m_dialogKey = Resources.Load<VP_DialogKey>("Dialogue/KeySetupData");
                m_dialogKey.key = "";
            }

            if (m_dialogKey.list != null && !m_dialogKey.list.ContainsKey(m_ID))
                m_dialogKey.list.Add(m_ID, "");
        }

        public void DeleteLanguageFile(int _index)
        {
            SystemLanguage language = m_activeLanguages[_index];

            string folder = Application.dataPath + VP_LocalizationSetup.FILE_PATH;

            string extension = "";

            switch (m_languageParser)
            {
                case LANGUAGE_PARSER.CSV:
                    extension = VP_LocalizationSetup.Extension.CSV;
                    folder += VP_LocalizationSetup.Folder.LOCALIZATION_FOLDER_CSV;
                    break;
                case LANGUAGE_PARSER.JSON:
                    extension = VP_LocalizationSetup.Extension.JSON;
                    folder += VP_LocalizationSetup.Folder.LOCALIZATION_FOLDER_JSON;
                    break;
                case LANGUAGE_PARSER.XML:
                    extension = VP_LocalizationSetup.Extension.XML;
                    folder += VP_LocalizationSetup.Folder.LOCALIZATION_FOLDER_XML;
                    break;
            }

            if (languageFiles == null)
            {
                languageFiles = SetAllLanguages(extension, folder);
            }

            bool _canDelete = false;
            string assetToDelete = "";
            foreach (TextAsset asset in languageFiles.Values)
            {
                if (asset.name == language.ToString())
                {
                    string filePath = folder + asset.name + extension;
                    _canDelete = true;
                    assetToDelete = asset.name;
                    Debug.Log("The language " + assetToDelete + " was deleted. ");
                    File.Delete(filePath);
                    break;
                }
            }

            if (_canDelete)
            {
                //  m_activeLanguages.Remove(language);
                //  m_localizationTexts.RemoveAt(_index);
                //  languageFiles.Remove(assetToDelete);
                AssetDatabase.Refresh();
                RefreshLanguages();
            }
        }
        // 
        public void AutoTranslateKeyToAvailableLanguages(bool _forceAdd = false)
        {
            if (string.IsNullOrEmpty(m_localizationKey))
            {
                Debug.LogError("Key is null. It can't be added.");
                return;
            }

            if (m_activeLanguages.Count == 0)
            {
                Debug.LogError("There are no languages availables in " + m_languageParser.ToString() + " format. Please, create at least one.");
                return;
            }

            Dictionary<SystemLanguage, string> languagesCodes = new Dictionary<SystemLanguage, string>()
            {
                { SystemLanguage.Spanish, "es" },
                { SystemLanguage.English, "en" },
                { SystemLanguage.French, "fr" },
                { SystemLanguage.German, "de" }

            };

            string text = m_localizationKey;
            string language1 = languagesCodes[m_autoTranslateOriginal];
            int counter = 0;

            foreach (SystemLanguage activeLang in m_activeLanguages)
            {
                string language2 = languagesCodes[activeLang];
                var url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl={language1}&tl={language2}&dt=t&q={Uri.EscapeUriString(text)}";

                var webClient = new WebClient
                {
                    Encoding = System.Text.Encoding.UTF8
                };
                var result = webClient.DownloadString(url);
                try
                {
                    result = result.Substring(4, result.IndexOf("\"", 4, StringComparison.Ordinal) - 4);
                    m_localizationTexts[counter] = result;
                }
                catch
                {
                    m_localizationTexts[counter] = "Error Google Translation not found";
                }

                counter++;
            }

            if (_forceAdd)
            {
                AddKeyToAllLanguages(true);
            }
        }

        public void AutoTranslateFirstLanguageToAvailableLanguages(bool _forceAdd = false)
        {
            if (string.IsNullOrEmpty(m_localizationKey))
            {
                Debug.LogError("Key is null. It can't be added.");
                return;
            }

            if (m_activeLanguages.Count == 0)
            {
                Debug.LogError("There are no languages availables in " + m_languageParser.ToString() + " format. Please, create at least one.");
                return;
            }

            Dictionary<SystemLanguage, string> languagesCodes = new Dictionary<SystemLanguage, string>()
            {
                { SystemLanguage.Spanish, "es" },
                { SystemLanguage.English, "en" },
                { SystemLanguage.French, "fr" },
                { SystemLanguage.German, "de" }

            };

            string language1 = languagesCodes[m_autoTranslateOriginal];
            string text = "";
            int counter = 0;
            bool found = false;
            if (m_localizationTexts.Count == 0)
            {
                Debug.LogError("There are no text availables.");
                return;
            }
            foreach (SystemLanguage activeLang in m_activeLanguages)
            {
                if (activeLang == m_autoTranslateOriginal)
                {
                    found = true;
                    text = m_localizationTexts[counter];
                    break;
                }
                counter++;
            }

            if (!found || string.IsNullOrEmpty(m_localizationTexts[counter]))
            {
                Debug.LogError("Lets add key to all languages");
                AddKeyToAllLanguages(_forceAdd);
                return;
            }

            counter = 0;
            foreach (SystemLanguage activeLang in m_activeLanguages)
            {
                string language2 = languagesCodes[activeLang];
                var url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl={language1}&tl={language2}&dt=t&q={Uri.EscapeUriString(text)}";

                var webClient = new WebClient
                {
                    Encoding = System.Text.Encoding.UTF8
                };
                var result = webClient.DownloadString(url);
                try
                {
                    result = result.Substring(4, result.IndexOf("\"", 4, StringComparison.Ordinal) - 4);
                    m_localizationTexts[counter] = result;
                    Debug.Log("Original Text: " + text + ", Language: " + activeLang.ToString() + " has text: " + result + " code: " + language2 + " and language 1 was " + language1);
                }
                catch
                {
                    m_localizationTexts[counter] = "Error Google Translation not found";
                }

                counter++;
            }

            if (_forceAdd)
            {
                AddKeyToAllLanguages(true);
            }
        }

        public void AddKeyToAllLanguages(bool _forceAdd)
        {
            if (string.IsNullOrEmpty(m_localizationKey))
            {
                Debug.LogError("Key is null. It can't be added.");
                return;
            }

            if (m_activeLanguages.Count == 0)
            {
                Debug.LogError("There are no languages availables in " + m_languageParser.ToString() + " format. Please, create at least one.");
                return;
            }
            if (languageFiles == null)
            {
                string folder = Application.dataPath + VP_LocalizationSetup.FILE_PATH;
                string extension = "";

                switch (m_languageParser)
                {
                    case LANGUAGE_PARSER.CSV:
                        extension = VP_LocalizationSetup.Extension.CSV;
                        folder += VP_LocalizationSetup.Folder.LOCALIZATION_FOLDER_CSV;
                        break;
                    case LANGUAGE_PARSER.JSON:
                        extension = VP_LocalizationSetup.Extension.JSON;
                        folder += VP_LocalizationSetup.Folder.LOCALIZATION_FOLDER_JSON;
                        break;
                    case LANGUAGE_PARSER.XML:
                        extension = VP_LocalizationSetup.Extension.XML;
                        folder += VP_LocalizationSetup.Folder.LOCALIZATION_FOLDER_XML;
                        break;
                }


                languageFiles = SetAllLanguages(extension, folder);
            }

            Dictionary<string, VP_TextItem> m_texts = new Dictionary<string, VP_TextItem>();
            int counter = 0;
            foreach (TextAsset asset in languageFiles.Values)
            {
                switch (m_languageParser)
                {
                    case LANGUAGE_PARSER.CSV:
                        m_texts = VP_CSVParser.ParseCSV(asset);
                        break;
                    case LANGUAGE_PARSER.JSON:
                        m_texts = VP_JSONParser.ParseJSON(asset);
                        break;
                    case LANGUAGE_PARSER.XML:
                        m_texts = VP_XMLParser.ParseXML(asset);
                        break;
                }

                bool _found = false;

                foreach (string k in m_texts.Keys)
                {
                    if (string.IsNullOrEmpty(m_localizationTexts[counter]))
                    {
                        m_localizationTexts[counter] = "TEXT NOT ADDED FROM INSPECTOR";
                    }

                    if (k == (m_localizationKey))
                    {
                        _found = true;
                        if (_forceAdd)
                        {
                            switch (m_languageParser)
                            {
                                case LANGUAGE_PARSER.CSV:
                                    VP_CSVParser.ReplaceTextInCSV(asset, m_localizationKey, m_localizationTexts[counter]);
                                    break;
                                case LANGUAGE_PARSER.JSON:
                                    VP_JSONParser.ReplaceInJSON(asset, m_localizationKey, m_localizationTexts[counter]);
                                    break;
                                case LANGUAGE_PARSER.XML:
                                    VP_XMLParser.ReplaceInXML(asset, m_localizationKey, m_localizationTexts[counter]);
                                    break;
                            }

                            Debug.Log("Replaced " + m_localizationKey + " in " + asset.name);
                        }
                        break;
                    }
                }

                if (!_found)
                {
                    if (_forceAdd)
                        Debug.Log("Key " + m_localizationKey + " not found in " + asset.name + ". it will be added instead.");

                    switch (m_languageParser)
                    {
                        case LANGUAGE_PARSER.CSV:
                            VP_CSVParser.AddToCSV(asset, m_localizationKey, m_localizationTexts[counter]);
                            break;
                        case LANGUAGE_PARSER.JSON:
                            VP_JSONParser.AddToJSON(asset, m_localizationKey, m_localizationTexts[counter]);
                            break;
                        case LANGUAGE_PARSER.XML:
                            VP_XMLParser.AddToXML(asset, m_localizationKey, m_localizationTexts[counter]);
                            break;
                    }

                    Debug.Log("Added " + m_localizationKey + " to " + asset.name);
                }

                counter++;
            }

            AssetDatabase.Refresh();
        }

        public void LoadTextFromKey()
        {
            RefreshLanguages();

            if (string.IsNullOrEmpty(m_localizationKey))
            {
                Debug.LogError("Key is null. Texts can't be loaded.");
                return;
            }

            if (m_activeLanguages.Count == 0)
            {
                Debug.LogError("There are no languages availables in " + m_languageParser.ToString() + " format. Please, create at least one.");
                return;
            }
           
            string folder = "";;
            string extension = "";

            switch (m_languageParser)
            {
                case LANGUAGE_PARSER.CSV:
                    extension = VP_LocalizationSetup.Extension.CSV;
                    folder += VP_LocalizationSetup.Folder.LOCALIZATION_FOLDER_CSV;
                    break;
                case LANGUAGE_PARSER.JSON:
                    extension = VP_LocalizationSetup.Extension.JSON;
                    folder += VP_LocalizationSetup.Folder.LOCALIZATION_FOLDER_JSON;
                    break;
                case LANGUAGE_PARSER.XML:
                    extension = VP_LocalizationSetup.Extension.XML;
                    folder += VP_LocalizationSetup.Folder.LOCALIZATION_FOLDER_XML;
                    break;
            }

            if (languageFiles == null || languageFiles.Count == 0)
                languageFiles = SetAllLanguages(extension, folder);

            Dictionary<string, VP_TextItem> m_texts = new Dictionary<string, VP_TextItem>();
            int counter = 0;
            bool found = false;
            foreach (TextAsset asset in languageFiles.Values)
            {
                switch (m_languageParser)
                {
                    case LANGUAGE_PARSER.CSV:
                        m_texts = VP_CSVParser.ParseCSV(asset);
                        break;
                    case LANGUAGE_PARSER.JSON:
                        m_texts = VP_JSONParser.ParseJSON(asset);
                        break;
                    case LANGUAGE_PARSER.XML:
                        m_texts = VP_XMLParser.ParseXML(asset);
                        break;
                }

                foreach (string k in m_texts.Keys)
                {
                    if (k == (m_localizationKey))
                    {
                        m_localizationTexts[counter] = m_texts[k].Text;
                        //Debug.Log("localization text: " + m_localizationTexts[counter]);
                        Repaint();
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    Debug.LogError("localization text not found for language " + asset.name);
                    m_localizationTexts[counter] = m_localizationKey + " not found";
                    AutoTranslateFirstLanguageToAvailableLanguages(m_forceAddAuto);
                }

                counter++;
            }

        }

        public void AddSelectionAsKeys(bool _forceAdd)
        {
            foreach (GameObject go in Selection.gameObjects)
            {
                string _key = go.name;

                if (string.IsNullOrEmpty(_key))
                {
                    Debug.LogError("Key is null. It can't be added.");
                    return;
                }

                if (m_activeLanguages.Count == 0)
                {
                    Debug.LogError("There are no languages availables in " + m_languageParser.ToString() + " format. Please, create at least one.");
                    return;
                }
                if (languageFiles == null)
                {
                    string folder = Application.dataPath + VP_LocalizationSetup.FILE_PATH;
                    string extension = "";

                    switch (m_languageParser)
                    {
                        case LANGUAGE_PARSER.CSV:
                            extension = VP_LocalizationSetup.Extension.CSV;
                            folder += VP_LocalizationSetup.Folder.LOCALIZATION_FOLDER_CSV;
                            break;
                        case LANGUAGE_PARSER.JSON:
                            extension = VP_LocalizationSetup.Extension.JSON;
                            folder += VP_LocalizationSetup.Folder.LOCALIZATION_FOLDER_JSON;
                            break;
                        case LANGUAGE_PARSER.XML:
                            extension = VP_LocalizationSetup.Extension.XML;
                            folder += VP_LocalizationSetup.Folder.LOCALIZATION_FOLDER_XML;
                            break;
                    }


                    languageFiles = SetAllLanguages(extension, folder);
                }

                Dictionary<string, VP_TextItem> m_texts = new Dictionary<string, VP_TextItem>();
                int counter = 0;
                foreach (TextAsset asset in languageFiles.Values)
                {
                    switch (m_languageParser)
                    {
                        case LANGUAGE_PARSER.CSV:
                            m_texts = VP_CSVParser.ParseCSV(asset);
                            break;
                        case LANGUAGE_PARSER.JSON:
                            m_texts = VP_JSONParser.ParseJSON(asset);
                            break;
                        case LANGUAGE_PARSER.XML:
                            m_texts = VP_XMLParser.ParseXML(asset);
                            break;
                    }

                    bool _found = false;

                    foreach (VP_TextItem item in m_texts.Values)
                    {
                        if (item.Text == (_key))
                        {
                            _found = true;
                            if (_forceAdd)
                            {
                                switch (m_languageParser)
                                {
                                    case LANGUAGE_PARSER.CSV:
                                        VP_CSVParser.ReplaceTextInCSV(asset, _key, m_localizationTexts[counter]);
                                        break;
                                    case LANGUAGE_PARSER.JSON:
                                        VP_JSONParser.ReplaceInJSON(asset, _key, m_localizationTexts[counter]);
                                        break;
                                    case LANGUAGE_PARSER.XML:
                                        VP_XMLParser.ReplaceInXML(asset, _key, m_localizationTexts[counter]);
                                        break;
                                }

                                Debug.Log("Replaced " + _key + " in " + asset.name);
                            }
                            break;
                        }
                    }

                    if (!_found)
                    {
                        if (_forceAdd)
                            Debug.Log("Key " + _key + " not found in " + asset.name + ". it will be added instead.");

                        switch (m_languageParser)
                        {
                            case LANGUAGE_PARSER.CSV:
                                VP_CSVParser.AddToCSV(asset, _key, m_localizationTexts[counter]);
                                break;
                            case LANGUAGE_PARSER.JSON:
                                VP_JSONParser.AddToJSON(asset, _key, m_localizationTexts[counter]);
                                break;
                            case LANGUAGE_PARSER.XML:
                                VP_XMLParser.AddToXML(asset, _key, m_localizationTexts[counter]);
                                break;
                        }

                        Debug.Log("Added " + _key + " to " + asset.name);
                    }

                    counter++;
                }

            }

            AssetDatabase.Refresh();
        }

        public void RemoveKeyFromAllLanguages()
        {
            if (string.IsNullOrEmpty(m_localizationKey))
            {
                Debug.LogError("Key is null. It can't be added.");
                return;
            }

            if (m_activeLanguages.Count == 0)
            {
                Debug.LogError("There are no languages availables in " + m_languageParser.ToString() + " format. Please, create at least one.");
                return;
            }

            if (languageFiles == null)
            {
                string folder = Application.dataPath + VP_LocalizationSetup.FILE_PATH;
                string extension = "";

                switch (m_languageParser)
                {
                    case LANGUAGE_PARSER.CSV:
                        extension = VP_LocalizationSetup.Extension.CSV;
                        folder += VP_LocalizationSetup.Folder.LOCALIZATION_FOLDER_CSV;
                        break;
                    case LANGUAGE_PARSER.JSON:
                        extension = VP_LocalizationSetup.Extension.JSON;
                        folder += VP_LocalizationSetup.Folder.LOCALIZATION_FOLDER_JSON;
                        break;
                    case LANGUAGE_PARSER.XML:
                        extension = VP_LocalizationSetup.Extension.XML;
                        folder += VP_LocalizationSetup.Folder.LOCALIZATION_FOLDER_XML;
                        break;
                }

                languageFiles = SetAllLanguages(extension, folder);
            }

            Dictionary<string, VP_TextItem> m_texts = new Dictionary<string, VP_TextItem>();
            int counter = 0;
            foreach (TextAsset asset in languageFiles.Values)
            {
                switch (m_languageParser)
                {
                    case LANGUAGE_PARSER.CSV:
                        m_texts = VP_CSVParser.ParseCSV(asset);
                        break;
                    case LANGUAGE_PARSER.JSON:
                        m_texts = VP_JSONParser.ParseJSON(asset);
                        break;
                    case LANGUAGE_PARSER.XML:
                        m_texts = VP_XMLParser.ParseXML(asset);
                        break;
                }

                foreach (VP_TextItem item in m_texts.Values)
                {
                    if (item.Key == (m_localizationKey))
                    {
                        switch (m_languageParser)
                        {
                            case LANGUAGE_PARSER.CSV:
                                VP_CSVParser.RemoveFromCSV(asset, m_localizationKey);
                                break;
                            case LANGUAGE_PARSER.JSON:
                                VP_JSONParser.RemoveFromJSON(asset, m_localizationKey);
                                break;
                            case LANGUAGE_PARSER.XML:
                                VP_XMLParser.RemoveFromXML(asset, m_localizationKey);
                                break;
                        }

                        Debug.Log("Removed " + m_localizationKey + " from " + asset.name);
                        break;
                    }
                }

                counter++;
            }
            AssetDatabase.Refresh();
        }

        public void AddLanguage()
        {
            string extension = "";
            string folder = "";

            switch (m_languageParser)
            {
                case LANGUAGE_PARSER.CSV:
                    extension = VP_LocalizationSetup.Extension.CSV;
                    folder = VP_LocalizationSetup.Folder.LOCALIZATION_FOLDER_CSV;
                    break;
                case LANGUAGE_PARSER.JSON:
                    extension = VP_LocalizationSetup.Extension.JSON;
                    folder = VP_LocalizationSetup.Folder.LOCALIZATION_FOLDER_JSON;
                    break;
                case LANGUAGE_PARSER.XML:
                    extension = VP_LocalizationSetup.Extension.XML;
                    folder = VP_LocalizationSetup.Folder.LOCALIZATION_FOLDER_XML;
                    break;
            }

            TextAsset m_default = Resources.Load<TextAsset>(folder + VP_LocalizationSetup.DEFAULT_FILE_NAME);
            string fullPath = Application.dataPath + VP_LocalizationSetup.FILE_PATH + folder + m_languageToAdd.ToString() + extension;
            if (m_default != null)
            {
                if (!File.Exists(fullPath))
                {
                    Debug.Log("Created language file in" + extension + " extension at " + fullPath + " with default english data. You should modify everything to " + m_languageToAdd.ToString() + ".");
                    File.WriteAllText(fullPath, m_default.text);
                    AssetDatabase.Refresh();
                    RefreshLanguages();
                }
                else
                {
                    Debug.Log("The language already exists. Gonna refresh languages.");
                    RefreshLanguages();
                }
            }

        }

        public void RefreshLanguages()
        {
            Debug.ClearDeveloperConsole();
            Debug.Log("Refreshing languages...");
            LoadLocalizationFiles();
        }

        public void OpenLocalizationFolder()
        {
            string folder = Application.dataPath + VP_LocalizationSetup.FILE_PATH;

            switch (m_languageParser)
            {
                case LANGUAGE_PARSER.CSV:
                    folder += VP_LocalizationSetup.Folder.LOCALIZATION_FOLDER_CSV;
                    break;
                case LANGUAGE_PARSER.JSON:
                    folder += VP_LocalizationSetup.Folder.LOCALIZATION_FOLDER_JSON;
                    break;
                case LANGUAGE_PARSER.XML:
                    folder += VP_LocalizationSetup.Folder.LOCALIZATION_FOLDER_XML;
                    break;
            }

            Debug.Log("Open folder: " + folder);

            System.Diagnostics.Process.Start(Path.GetFullPath(folder));
        }

        public void LoadLocalizationFiles()
        {
            if (m_localizationTexts == null)
            {
                m_localizationTexts = new List<string>();
            }

            if (languageFiles == null)
            {
                languageFiles = new Dictionary<string, TextAsset>();
            }

            if (m_activeLanguages == null)
            {
                m_activeLanguages = new List<SystemLanguage>();
            }

            languageFiles.Clear();
            m_activeLanguages.Clear();
            m_localizationTexts.Clear();

            string extension = "";
            string folder = "";

            switch (m_languageParser)
            {
                case LANGUAGE_PARSER.CSV:
                    extension = VP_LocalizationSetup.Extension.CSV;
                    folder = VP_LocalizationSetup.Folder.LOCALIZATION_FOLDER_CSV;
                    break;
                case LANGUAGE_PARSER.JSON:
                    extension = VP_LocalizationSetup.Extension.JSON;
                    folder = VP_LocalizationSetup.Folder.LOCALIZATION_FOLDER_JSON;
                    break;
                case LANGUAGE_PARSER.XML:
                    extension = VP_LocalizationSetup.Extension.XML;
                    folder = VP_LocalizationSetup.Folder.LOCALIZATION_FOLDER_XML;
                    break;
            }

            languageFiles = SetAllLanguages(extension, folder);
        }


        Dictionary<string, TextAsset> SetAllLanguages(string _extension, string folder)
        {
            Dictionary<string, TextAsset> languageAssets = new Dictionary<string, TextAsset>();
            TextAsset[] assets = Resources.LoadAll<TextAsset>(folder);
            foreach (TextAsset asset in assets)
            {
                if (asset.name != VP_LocalizationSetup.DEFAULT_FILE_NAME)
                {
                    if (!languageAssets.ContainsKey(asset.name))
                        languageAssets.Add(asset.name, asset);
                    else
                        languageAssets[asset.name] = asset;

                    SystemLanguage lang;
                    if (VP_CustomParser.TryParse(asset.name, out lang))
                    {
                        m_activeLanguages.Add(lang);
                        m_localizationTexts.Add("");
                    }
                }
            }

            if (m_activeLanguages.Count == 0 && assets.Length > 0)
            {
                // Only default
                foreach (TextAsset asset in assets)
                {
                    if (asset.name == VP_LocalizationSetup.DEFAULT_FILE_NAME)
                    {
                        languageAssets.Add("English", asset);
                        SystemLanguage lang = SystemLanguage.English;
                        m_activeLanguages.Add(lang);
                    }
                }
            }

            return languageAssets;
        }

        void OnLanguageParserChange()
        {
            m_oldLanguageParser = m_languageParser;

            RefreshLanguages();
        }
        #endregion
    }

}
