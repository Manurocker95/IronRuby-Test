using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEditor.Animations;
using System;
using VirtualPhenix.Localization;
using UnityEditor.SceneManagement;
using System.Text;
using System.Reflection;
using VirtualPhenix.Serialization;
using VirtualPhenix.Actions;
using VirtualPhenix.Dialog;
using VirtualPhenix.Interaction;
using VirtualPhenix.Variables;
using System.Net;
using VirtualPhenix.Fade;
using VirtualPhenix.Settings;
using VirtualPhenix.Save;
#if DOTWEEN
using DG.Tweening;
#endif

namespace VirtualPhenix.Debugger
{
    public enum DEBUG_TABS
    {
        SETTINGS,
        BUILD,
	    LOCALIZATION,
	    GAME,
        MISCELLANEOUS,
        AUDIO,
        DATABASE,
        AI,
    }

    /// <summary>
    /// JUst for running any scene without needing to initialize the initializing
    /// </summary>
    public class VP_Debugger : VP_EditorWindow<VP_Debugger>
    {


        #region Variables
        
        private DEBUG_TABS m_currentTab;
        private int m_toolbarInt;

	    private string[] m_toolbarStrings = new string[] { "SETTINGS", "BUILD", "LOCALIZATION", "GAME", "MISCELLANEOUS", "AUDIO", "DATABASE","AI"};
        [SerializeField] private string m_projectName;
        [SerializeField] private string m_companyName;
        [SerializeField] private string m_bundleVersion;
        [SerializeField] private string m_codeVersion;
        [SerializeField] private string m_scriptName;
        [SerializeField] private string m_buildExtension;
        [SerializeField] private string m_applicationID;
        [SerializeField] private BuildTarget m_currentTarget;

        [SerializeField] private bool m_loadClip;
        [SerializeField] private bool m_playAudioOnInit;
        [SerializeField] private bool m_clipInLoop;
        [SerializeField] private bool m_overrideOnPlay;
        [SerializeField] private float m_audioVolume;
        [SerializeField] private string m_audioVarName;
        [SerializeField] private string m_audioClass;
        [SerializeField] private string m_audioKey;
        [SerializeField] private VirtualPhenix.VP_AudioSetup.AUDIO_TYPE m_audioType;

        [SerializeField] private LANGUAGE_PARSER m_languageParser;
        [SerializeField] private string m_localizationKey;
        private Dictionary<string, TextAsset> languageFiles;
        [SerializeField] private List<SystemLanguage> m_activeLanguages;
        [SerializeField] private List<string> m_localizationTexts;

        [SerializeField] public VP_DialogKey m_dialogKey;
        [TextArea, SerializeField] public string m_dialogKeyStr;
        public string m_ID;

        private SystemLanguage m_languageToAdd;
        private SystemLanguage m_autoTranslateOriginal;
        private LANGUAGE_PARSER m_oldLanguageParser;
        private string m_keyToDelete = "";
        private float m_delayIfNoClip;
        private string currentScene = "";
        private bool m_forceAddAuto = false;

	    [SerializeField] public static int m_screenshotID;

	    string[] sceneList; // list of all scenes in build
	    int sceneIndex;
	    bool noScenesInBuild;

	    private int m_indexList = 0;
	    private CUSTOM_GAME_ACTIONS m_actionToAdd;
	    private GENERAL_FIELD_TYPES m_variableTypeToAdd;
	    private string m_variableName;
	    private int m_iVal;
	    private float m_fVal;
	    private double m_dVal;
	    private bool m_bVal;
	    private string m_sVal;
	    private GameObject m_goVal;
	    private UnityEngine.Object m_actionTest;
	    private ScriptableObject m_gameVariableDB;
	    private Vector2 m_scrollPos;
	    private Vector2 m_scrollPosH;
        private string m_defineSymbols;
        private bool m_setupDefine;
        private bool m_refreshSymbols;

        public override string WindowName => "VP Editor";
        #endregion

        #region shortcut
        [MenuItem("Virtual Phenix/Pause Program #%F1", false, 0)]
        public static void PauseProgram()
        {
            Debug.Break();
        }

        [MenuItem("Virtual Phenix/Init from Initializer %F3", false, 0)]
        static void InitFromInitializer()
        {
            string _scene = EditorBuildSettings.scenes[0].path;
            EditorSceneManager.OpenScene(_scene, OpenSceneMode.Single);
            EditorApplication.EnterPlaymode();
        }

#if PHOENIX_URP_BLIT_PASS && USE_FADE
        [MenuItem("Virtual Phenix/URP/Set Current PP Effect", false, 1)]
        static void UpdateCurrentPPEffectToURP()
        {
            VP_FadePostProcess m_pp = FindObjectOfType<VP_FadePostProcess>();
            if (m_pp != null)
            {
                m_pp.SetCurrentURPPass();
            }
        }
#endif


        [UnityEditor.MenuItem("Virtual Phenix/Localization/Parse Texts From CSV")]
        protected static void ParseTextsFromCSV()
        {
            if (UnityEditor.Selection.activeObject is VP_LocalizationData)
            {
                VP_LocalizationData data = UnityEditor.Selection.activeObject as VP_LocalizationData;               
                bool parsed = data.ParseTexts(LANGUAGE_PARSER.CSV);
                
                if (parsed)
                {
                    VP_Debug.LogError("Texts were parsed");
                    UnityEditor.AssetDatabase.SaveAssets();
                    UnityEditor.AssetDatabase.Refresh();
                    UnityEditor.EditorUtility.FocusProjectWindow();
                }
                else
                {
                    VP_Debug.LogError("Texts couldn't be parsed");
                }

            }
            else
            {
                VP_Debug.LogError("Localization Data was not selected. Select Localization Data.");
            }
        }
        [UnityEditor.MenuItem("Virtual Phenix/Localization/Parse Texts From XML")]
        protected static void ParseTextsFromXML()
        {
            if (UnityEditor.Selection.activeObject is VP_LocalizationData)
            {
                VP_LocalizationData data = UnityEditor.Selection.activeObject as VP_LocalizationData;               
                bool parsed = data.ParseTexts(LANGUAGE_PARSER.XML);
                
                if (parsed)
                {
                    UnityEditor.AssetDatabase.SaveAssets();
                    UnityEditor.AssetDatabase.Refresh();
                    UnityEditor.EditorUtility.FocusProjectWindow();
                }
            }
        }
        [UnityEditor.MenuItem("Virtual Phenix/Localization/Parse Texts From JSON")]
        protected static void ParseTextsFromJSON()
        {
            if (UnityEditor.Selection.activeObject is VP_LocalizationData)
            {
                VP_LocalizationData data = UnityEditor.Selection.activeObject as VP_LocalizationData;               
                bool parsed = data.ParseTexts(LANGUAGE_PARSER.JSON);
                
                if (parsed)
                {
                    UnityEditor.AssetDatabase.SaveAssets();
                    UnityEditor.AssetDatabase.Refresh();
                    UnityEditor.EditorUtility.FocusProjectWindow();
                }
            }
        }
        
        [MenuItem("Virtual Phenix/Troubleshooting/Look For Broken SO", false, 0)]
        static void LookForNullDBAssets()
        {
            List<ScriptableObject> obj = VP_Utils.FindAssetsByType<ScriptableObject>();
            VP_Debug.Log("Number of objects: " + obj.Count);

            foreach (UnityEngine.Object o in obj)
            {
                if (o == null)
                {
                    VP_Debug.Log("NULL ");
                }
                else
                {
                    if (o is VP_VFXDefaultDatabase)
                    {
                        VP_Debug.Log("VFX Database " + o.name);
                        if ((o as VP_VFXDefaultDatabase).Resources == null)
                        {
                            VP_Debug.Log("Dictionary is null in "+o.name);
                        }
                    }

                    if (o is VP_DialogShakeLibrary)
                    {
                        VP_Debug.Log("VP_DialogShakeLibrary " + o.name);
                        if ((o as VP_DialogShakeLibrary).ShakePresets == null)
                        {
                            VP_Debug.Log("ShakePresets is null in " + o.name);
                        }
                    }
                }
            }
        }
        public void LocateScene()
        {
            UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(EditorBuildSettings.scenes[sceneIndex].path, typeof(UnityEngine.Object));
            Selection.activeObject = obj;
            EditorGUIUtility.PingObject(obj);
        }


        [MenuItem("Virtual Phenix/Troubleshooting/Look For Broken .Assets", false, 0)]
        static void LookForNullAssets()
        {
            string[] aMaterialFiles = Directory.GetFiles(Application.dataPath, "*.asset", SearchOption.AllDirectories);
            foreach (string matFile in aMaterialFiles)
            {
                string assetPath = "Assets" + matFile.Replace(Application.dataPath, "").Replace('\\', '/');
                UnityEngine.Object sourceObj = (UnityEngine.Object)AssetDatabase.LoadAssetAtPath(assetPath, typeof(UnityEngine.Object));
                if (sourceObj == null)
                {
                    VP_Debug.Log("Broken at "+assetPath);
                }
            }
        }

        [MenuItem("Virtual Phenix/Troubleshooting/Look For Broken Resources", false, 0)]
        static void LookForNullResourcesAssets()
        {
            UnityEngine.Object[] res = Resources.LoadAll<UnityEngine.Object>("Database");
            foreach (UnityEngine.Object obj in res)
            {
                if (obj == null)
                {
                    VP_Debug.Log("Broken");
                }
                else
                {
                    VP_Debug.Log(obj+" Not Broken");
                }
            }
        }

        public static new VP_Debugger Instance
        {
            get
            {
                return (VP_Debugger)m_instance;
            }
        }

        [MenuItem("Virtual Phenix/Folders/Create Project Template Folders", false, 0)]
        static void CreateProjectTemplateFolders()
        {
            var inst = Instance;

            if (inst == null)
            {
                ShowWindow();
            }

            if (!inst)
                return;

            string projectName = inst.m_projectName;
            if (!string.IsNullOrEmpty(projectName))
            {
                string baseFolder = Application.dataPath + "/" + projectName;
                string graphics = baseFolder + "/" + "Graphics/";
                string audio = baseFolder + "/" + "Audio/";
                string resources = baseFolder + "/" + "Resources/";
                string prefabs = baseFolder + "/" + "Prefabs/";
                string Scripts = baseFolder + "/" + "Scripts/";
                string Scenes = baseFolder + "/" + "Scenes/";

                if (!Directory.Exists(graphics))
                    Directory.CreateDirectory(graphics);

                if (!Directory.Exists(audio))
                    Directory.CreateDirectory(audio);

                if (!Directory.Exists(resources))
                    Directory.CreateDirectory(resources);

                if (!Directory.Exists(prefabs))
                    Directory.CreateDirectory(prefabs);

                if (!Directory.Exists(Scripts))
                    Directory.CreateDirectory(Scripts);

                if (!Directory.Exists(Scenes))
                    Directory.CreateDirectory(Scenes);

                string resPrefab = resources + "/" + "Prefabs/";
    
                if (!Directory.Exists(resPrefab))
                    Directory.CreateDirectory(resPrefab);


                string sfx = audio + "/" + "SFX/";
                string bgm = audio + "/" + "BGM/";
                string voice = audio + "/" + "VOICE/";

                if (!Directory.Exists(sfx))
                    Directory.CreateDirectory(sfx);

                if (!Directory.Exists(bgm))
                    Directory.CreateDirectory(bgm);

                if (!Directory.Exists(voice))
                    Directory.CreateDirectory(voice);

                VP_Debug.Log("Creating folder at " + baseFolder);
            }
        }

        #endregion

        #region Main
        /// <summary>
        /// Creates the custom window :D
        /// </summary>
        [MenuItem("Virtual Phenix/Window/VP Editor %F1")]
        public static void ShowWindow()
        {
            m_instance = GetWindow<VP_Debugger>("VP Editor");
        }

        private void Awake()
        {
            m_projectName = PlayerSettings.productName;
            m_companyName = PlayerSettings.companyName;
            m_bundleVersion = PlayerSettings.bundleVersion;
            m_codeVersion = "1";
            m_currentTarget = EditorUserBuildSettings.activeBuildTarget;
            m_toolbarInt = 0;
            m_currentTab = DEBUG_TABS.SETTINGS;
            m_applicationID = PlayerSettings.GetApplicationIdentifier(EditorUserBuildSettings.selectedBuildTargetGroup);
            m_delayIfNoClip = 1.0f;
            m_audioVolume = 1.0f;

            if (m_currentTarget == BuildTarget.StandaloneWindows || m_currentTarget == BuildTarget.StandaloneWindows64)
            {
                m_buildExtension = ".exe";
                m_codeVersion = "1";
            }
            else if (m_currentTarget == BuildTarget.StandaloneOSX)
            {
                m_buildExtension = ".app";
                m_codeVersion = "1";
            }
            else if (m_currentTarget == BuildTarget.Android)
            {
                m_buildExtension = ".apk";
                m_codeVersion = PlayerSettings.Android.bundleVersionCode.ToString();
            }
            else if (m_currentTarget == BuildTarget.iOS)
            {
                m_buildExtension = ".ipa";
                m_codeVersion = PlayerSettings.iOS.buildNumber.ToString();
            }

            int lastParser = PlayerPrefs.GetInt(VP_GameSetup.PlayerPrefs.LOCALIZATION_PARSER, -1);
            m_languageParser = lastParser == -1 ? LANGUAGE_PARSER.CSV : (LANGUAGE_PARSER)lastParser;
            m_oldLanguageParser = m_languageParser;

            m_activeLanguages = new List<SystemLanguage>();
            m_localizationTexts = new List<string>();
            m_autoTranslateOriginal = SystemLanguage.English;

	        LoadLocalizationFiles();
            
	        string[] guids = AssetDatabase.FindAssets("t:VP_GameVariables");

	        foreach (string typeObj in guids)
	        {
		        string path = AssetDatabase.GUIDToAssetPath(typeObj);

		        string parsedPath = path;
		        int lastSlash = path.LastIndexOf('/');
		        parsedPath = (lastSlash > -1) ? parsedPath.Substring(0, lastSlash) : parsedPath;

		        m_gameVariableDB = AssetDatabase.LoadAssetAtPath(path, typeof(VP_GameVariables)) as VP_GameVariables;
		        if (m_gameVariableDB != null)
		        {
			        break;
		        }
	        }
	        ReloadSceneList();
            RefreshInitKey();
        }

        public void CheckFieldActions()
        {
           
        }

        public void CheckLoopActions()
        {
       
        }

        public void ResetFieldActions()
        {
  
        }

        public void ContinueFieldAction()
        {

        }

        public void ResetLoopActions()
        {
          
        }

        public void ContinueLoopAction()
        {
           
        }

        public void ContinueSelectionAction(int index)
        {
         
        }

        public void ContinueBattleAction()
        {
           
        }

        public void ResetBattleAction()
        {
         
        }


        public void RefreshInitKey()
        {
            if (m_dialogKey == null)
            {
                m_dialogKey = Resources.Load<VP_DialogKey>("Debugger/KeySetupData");
                m_dialogKey.key = "";
            }

            if (m_dialogKey.list != null && !m_dialogKey.list.ContainsKey(m_ID))
                m_dialogKey.list.Add(m_ID, "");
        }

       

	    // create a list of all scenes in build
	    void ReloadSceneList()
	    {
		    int sceneCount = EditorBuildSettings.scenes.Length;

		    if (sceneCount > 0)
		    { // if there are any scenes to load
			    noScenesInBuild = false;
			    sceneList = new string[sceneCount];
			    int i = 0;
			    foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
			    {
				    sceneList[i] = VP_Utils.ExtractSceneNameFromPath(scene.path);
				    i++;
			    }
		    }
		    else
		    { // if there are no scenes, default to something
			    noScenesInBuild = true;
			    sceneList = new string[1];
			    sceneList[0] = "";
			    //			Debug.LogWarning("ScenePicker could not list scenes because no scenes are added to the build. Please add at least one scene in the build settings.");
		    }
	    }

        public override void CreateHowToText()
        {
            if (m_howToTexts == null)
                InitHowToTextList();

            CreateHowToTitle(ref m_howToTexts, "VIRTUAL PHENIX GENERAL EDITOR");

            m_howToTexts.Add(new VP_HTUText()
            {
                Text = "We have here everything needed for helping us to go faster.",
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

	        EditorGUILayout.BeginHorizontal();
	        if (GUILayout.Button("Play from Initialize"))
	        {
		        currentScene = EditorSceneManager.GetActiveScene().name;
		        // Enum.TryParse(EditorSceneManager.GetActiveScene().name, out m_sceneToLoad);
		        string _scene = EditorBuildSettings.scenes[0].path;
		        EditorSceneManager.OpenScene(_scene, OpenSceneMode.Single);
		        EditorApplication.EnterPlaymode();
	        }
	        if (GUILayout.Button("Exit Playmode"))
	        {
		        string _scene = Application.dataPath + "/Scenes/" + currentScene + ".unity";
		        EditorApplication.ExitPlaymode();
		        // EditorSceneManager.OpenScene(_scene, OpenSceneMode.Single);
	        }

	        EditorGUILayout.EndHorizontal();
	        EditorGUILayout.Space();
	        EditorGUILayout.BeginHorizontal();
	        if (!noScenesInBuild)
	        {
                EditorGUILayout.BeginHorizontal();
                sceneIndex = EditorGUILayout.Popup("Load Scene", sceneIndex, sceneList);

		        if (GUILayout.Button("Open Scene"))
		        {
			        EditorSceneManager.OpenScene(EditorBuildSettings.scenes[sceneIndex].path, OpenSceneMode.Single);
		        }

		        if (GUILayout.Button("Open Scene Additive"))
		        {
			        EditorSceneManager.OpenScene(EditorBuildSettings.scenes[sceneIndex].path, OpenSceneMode.Additive);
		        }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Refresh Scene List"))
		        {
			        ReloadSceneList();
		        }

                if (GUILayout.Button("Locate Scene"))
                {
                    LocateScene();
                }
                EditorGUILayout.EndHorizontal();
            }
	        else
	        {

		        EditorGUILayout.HelpBox("No scenes found. Please add at least one scene in the build settings.", MessageType.Warning);
		        if (GUILayout.Button("Refresh Scene List"))
		        {
			        ReloadSceneList();
		        }

	        }

	        EditorGUILayout.EndHorizontal();
	        EditorGUILayout.Space();

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("--- VIRTUAL PHENIX TOOLS --- ", EditorStyles.centeredGreyMiniLabel);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Console"))
            {
                EditorApplication.ExecuteMenuItem("Virtual Phenix/Window/Console");
      
            }
            if (GUILayout.Button("Dialog Editor"))
            {
                EditorApplication.ExecuteMenuItem("Virtual Phenix/Window/Dialogue Editor");

            }
            GUILayout.EndHorizontal();

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("--- OTHER TOOLS --- ", EditorStyles.centeredGreyMiniLabel);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.Space();
	        m_scrollPosH = EditorGUILayout.BeginScrollView(m_scrollPosH, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
	        m_toolbarInt = GUILayout.Toolbar(m_toolbarInt, m_toolbarStrings);
	        m_currentTab = (DEBUG_TABS)m_toolbarInt;
	        EditorGUILayout.EndScrollView();
	        m_scrollPos = EditorGUILayout.BeginScrollView(m_scrollPos, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            switch (m_currentTab)
            {
                case DEBUG_TABS.SETTINGS:
                    EditorGUILayout.HelpBox("This section has everything related to project settings.", MessageType.Info);
                    EditorGUILayout.Space();

                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                    GUILayout.Label("--- PROJECT PROPERTIES --- ", EditorStyles.centeredGreyMiniLabel);
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                    EditorGUILayout.Space();
                    EditorGUILayout.BeginVertical();
                    
                    m_currentTarget = (BuildTarget)EditorGUILayout.EnumPopup("Current Target:", m_currentTarget);
                    m_projectName = EditorGUILayout.TextField("Project Name:", m_projectName);
                    m_companyName = EditorGUILayout.TextField("Company Name:", m_companyName);
                    m_bundleVersion = EditorGUILayout.TextField("Bundle Version:", m_bundleVersion);
                    m_codeVersion = EditorGUILayout.TextField("Code Version:", m_codeVersion);
                    m_applicationID = EditorGUILayout.TextField("Application identifier:", m_applicationID);
                    EditorGUILayout.Space();
                    if (GUILayout.Button("Set Application ID"))
                    {
                  
                        SetupApplicationID();
                    }
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Refresh properties"))
                    {
                        RefreshProperties();
                    }
                    if (GUILayout.Button("Apply properties"))
                    {
                        ApplySettings();
                    }
                    
                    GUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                    this.Repaint();


                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                    GUILayout.Label("--- DEFINE SYMBOLS --- ", EditorStyles.centeredGreyMiniLabel);
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                    m_defineSymbols = EditorGUILayout.TextField("Define:", m_defineSymbols);

                    EditorGUILayout.BeginHorizontal();

                    if (GUILayout.Button("Add Define symbols"))
                    {
                        AddDefineSymbols(); 
                    }

                    if (GUILayout.Button("Refresh Define symbols"))
                    {
                        RefreshDefineSymbols();
                    } 
                    
                    if (GUILayout.Button("Remove Define symbols"))
                    {
                        RemoveDefineSymbols();
                    }

                   
                    EditorGUILayout.EndHorizontal();


                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                    GUILayout.Label("--- OPEN TABS --- ", EditorStyles.centeredGreyMiniLabel);
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

                    EditorGUILayout.BeginHorizontal();

                    if (GUILayout.Button("Open Project Settings"))
                    {
                        EditorApplication.ExecuteMenuItem("Edit/Project Settings...");
                    }

                    if (GUILayout.Button("Open Tags and layers"))
                    {
                        EditorApplication.ExecuteMenuItem("Edit/Project Settings/Tags and Layers");
                    }

                    if (GUILayout.Button("Open Save file folder"))
                    {
                        string m_dataFolder = "";
                        var saveManager = GameObject.FindObjectOfType<VP_SaveManagerBase>();
                        if (saveManager != null)
                        {
                            m_dataFolder = saveManager.GetSaveLocation();
                        }
                        else
                        {
                            GameObject sm = GameObject.Find("SaveManager");
                            if (sm != null)
                            {
                                
                                foreach (Component c in sm.GetComponents<Component>())
                                {
                                    
                                    if (c is VP_SaveManagerBase) //IsSameOrSubclassOf
                                    {
	                                    m_dataFolder = (c as VP_SaveManagerBase).GetSaveLocation();
                                        break;
                                    }
                                }
                                
                                if (string.IsNullOrEmpty(m_dataFolder))
                                {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
                                    m_dataFolder = Application.persistentDataPath;//System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "\\" + VP_SaveSetup.SAVE_PATH + "\\" + m_projectName + "\\";
#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
             m_dataFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "/" + VP_SaveSetup.SAVE_PATH+"/"+m_projectName+"/";
#elif UNITY_ANDROID
            m_dataFolder = Application.persistentDataPath + "/" + VP_SaveSetup.SAVE_PATH + "/";
#endif
                                }
                            }
                            else
                            {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
                                m_dataFolder = Application.persistentDataPath;//System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "\\" + VP_SaveSetup.SAVE_PATH + "\\" + m_projectName + "\\";
#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
             m_dataFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "/" + VP_SaveSetup.SAVE_PATH+"/"+m_projectName+"/";
#elif UNITY_ANDROID
            m_dataFolder = Application.persistentDataPath + "/" + VP_SaveSetup.SAVE_PATH + "/";
#endif
                            }
                        }

                        if (!System.IO.Directory.Exists(m_dataFolder))
                            Directory.CreateDirectory(m_dataFolder);

                        System.Diagnostics.Process.Start(Path.GetFullPath(m_dataFolder));


                    }
                    GUILayout.EndHorizontal();
                    break;


                case DEBUG_TABS.LOCALIZATION:
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                    GUILayout.Label("--- FOLDER --- ", EditorStyles.centeredGreyMiniLabel);
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                    EditorGUILayout.Space();
                    if (GUILayout.Button("Open Localization Folder"))
                    {
                        OpenLocalizationFolder();
                    }
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
                            m_localizationTexts[i] = EditorGUILayout.TextField(m_activeLanguages[i].ToString()+" text", m_localizationTexts[i]);
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
                    break;
                case DEBUG_TABS.GAME:
	                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
	                GUILayout.Label("--- ADD CUSTOM ACTIONS --- ", EditorStyles.centeredGreyMiniLabel);
	                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
	                EditorGUILayout.Space();
	                m_actionToAdd = (CUSTOM_GAME_ACTIONS)EditorGUILayout.EnumPopup("Action Type", m_actionToAdd);
	                m_indexList = (int)EditorGUILayout.IntField("Action List", m_indexList);
	                m_actionTest = EditorGUILayout.ObjectField(m_actionTest, typeof(UnityEngine.Object), true);
	                if (GUILayout.Button("Add Action1"))
	                {
		                AddActionToGameObject(m_actionToAdd);
	                }
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                    GUILayout.Label("--- HANDLE CUSTOM ACTIONS --- ", EditorStyles.centeredGreyMiniLabel);
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                    EditorGUILayout.Space();
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Continue Field Action"))
                    {
                        ContinueFieldAction();
                    }
                    if (GUILayout.Button("Continue Loop Action"))
                    {
                        ContinueLoopAction();
                    }
                    if (GUILayout.Button("Continue Selection Action"))
                    {
                        ContinueSelectionAction(m_indexList);
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Reset field Actions"))
                    {
                        ResetFieldActions();
                    }
                    if (GUILayout.Button("Reset Loop Actions"))
                    {
                        ResetLoopActions();
                    }
                    
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Check field Actions"))
                    {
                        CheckFieldActions();
                    }
                    if (GUILayout.Button("Check Loop Actions"))
                    {
                        CheckLoopActions();
                    }

                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                    GUILayout.Label("--- BATTLE ACTIONS --- ", EditorStyles.centeredGreyMiniLabel);
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                    EditorGUILayout.Space();
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Continue Battle Action"))
                    {
                        ContinueBattleAction();
                    }
                    if (GUILayout.Button("Reset Battle Action"))
                    {
                        ResetBattleAction();
                    }

                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
	                GUILayout.Label("--- GAME VARIABLES --- ", EditorStyles.centeredGreyMiniLabel);
	                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
	                EditorGUILayout.Space();
	                m_gameVariableDB = (ScriptableObject)EditorGUILayout.ObjectField(m_gameVariableDB, typeof(UnityEngine.Object), true);

	                m_variableTypeToAdd = (GENERAL_FIELD_TYPES)EditorGUILayout.EnumPopup("Variable Type", m_variableTypeToAdd);
	                m_variableName = (string)EditorGUILayout.TextField("Variable Name", m_variableName);

	                if (!UnityEngine.Application.isPlaying)
	                {
		                switch (m_variableTypeToAdd)
		                {
		                case GENERAL_FIELD_TYPES.INT:
			                m_iVal = (int)EditorGUILayout.IntField("Value", m_iVal);

			                EditorGUILayout.BeginHorizontal();
			                if (GUILayout.Button("Add Variable"))
			                {
				                if (m_gameVariableDB == null)
				                {
					                VP_Debug.LogError("There is no database selected.");
				                }
				                else if (!string.IsNullOrEmpty(m_variableName))
				                {
					                VP_GameVariables gv = (VP_GameVariables)m_gameVariableDB;
					                gv.Data.AddVariable(new Field<int>(m_variableName, m_iVal));
					                EditorUtility.SetDirty(gv);
					                AssetDatabase.SaveAssets();
				                }
			                }
			                if (GUILayout.Button("Replace Variable Value"))
			                {
				                if (m_gameVariableDB == null)
				                {
					                VP_Debug.LogError("There is no database selected.");
				                }
				                else if (!string.IsNullOrEmpty(m_variableName))
				                {
					                VP_GameVariables gv = (VP_GameVariables)m_gameVariableDB;
					                gv.Data.ReplaceVariableValue(new Field<int>(m_variableName, m_iVal));
					                EditorUtility.SetDirty(gv);
					                AssetDatabase.SaveAssets();
				                }
			                }
			                if (GUILayout.Button("Remove Variable"))
			                {
				                if (m_gameVariableDB == null)
				                {
					                VP_Debug.LogError("There is no database selected.");
				                }
				                else if (!string.IsNullOrEmpty(m_variableName))
				                {
					                VP_GameVariables gv = (VP_GameVariables)m_gameVariableDB;
					                gv.Data.RemoveVariableValue(m_variableName);
					                EditorUtility.SetDirty(gv);
					                AssetDatabase.SaveAssets();
				                }
			                }
			                if (GUILayout.Button("Check Value"))
			                {
				                if (m_gameVariableDB == null)
				                {
					                VP_Debug.LogError("There is no database selected.");
				                }
				                else if (!string.IsNullOrEmpty(m_variableName))
				                {
					                VP_GameVariables gv = (VP_GameVariables)m_gameVariableDB;
					                Field<int> i_f = (Field<int>)gv.Data.GetVariableValue<int>(m_variableName);
					                m_iVal = i_f.Value;
					                EditorUtility.SetDirty(gv);
					                AssetDatabase.SaveAssets();
				                }
			                }
			                break;
		                case GENERAL_FIELD_TYPES.FLOAT:
			                m_fVal = (float)EditorGUILayout.FloatField("Value", m_fVal);
			                EditorGUILayout.BeginHorizontal();
			                if (GUILayout.Button("Add Variable"))
			                {
				                if (m_gameVariableDB == null)
				                {
					                VP_Debug.LogError("There is no database selected.");
				                }
				                else if (!string.IsNullOrEmpty(m_variableName))
				                {
					                VP_GameVariables gv = (VP_GameVariables)m_gameVariableDB;
					                gv.Data.AddVariable(new Field<float>(m_variableName, m_fVal));
					                EditorUtility.SetDirty(gv);
					                AssetDatabase.SaveAssets();
				                }
			                }
			                if (GUILayout.Button("Replace Variable Value"))
			                {
				                if (m_gameVariableDB == null)
				                {
					                Debug.LogError("There is no database selected.");
				                }
				                else if (!string.IsNullOrEmpty(m_variableName))
				                {
					                VP_GameVariables gv = (VP_GameVariables)m_gameVariableDB;
					                gv.Data.ReplaceVariableValue(new Field<float>(m_variableName, m_fVal));
					                EditorUtility.SetDirty(gv);
					                AssetDatabase.SaveAssets();
				                }
			                }
			                if (GUILayout.Button("Remove Variable"))
			                {
				                if (m_gameVariableDB == null)
				                {
					                Debug.LogError("There is no database selected.");
				                }
				                else if (!string.IsNullOrEmpty(m_variableName))
				                {
					                VP_GameVariables gv = (VP_GameVariables)m_gameVariableDB;
					                gv.Data.RemoveVariableValue(m_variableName);
					                EditorUtility.SetDirty(gv);
					                AssetDatabase.SaveAssets();
				                }
			                }
			                if (GUILayout.Button("Check Value"))
			                {
				                if (m_gameVariableDB == null)
				                {
					                Debug.LogError("There is no database selected.");
				                }
				                else if (!string.IsNullOrEmpty(m_variableName))
				                {
					                VP_GameVariables gv = (VP_GameVariables)m_gameVariableDB;
					                Field<float> f_f = (Field<float>)gv.Data.GetVariableValue<float>(m_variableName);
					                m_fVal = f_f.Value;
					                EditorUtility.SetDirty(gv);
					                AssetDatabase.SaveAssets();
				                }
			                }
			                break;
		                case GENERAL_FIELD_TYPES.DOUBLE:
			                m_dVal = (double)EditorGUILayout.DoubleField("Value", m_dVal);
			                EditorGUILayout.BeginHorizontal();
			                if (GUILayout.Button("Add Variable"))
			                {
				                if (m_gameVariableDB == null)
				                {
					                Debug.LogError("There is no database selected.");
				                }
				                else if (!string.IsNullOrEmpty(m_variableName))
				                {
					                VP_GameVariables gv = (VP_GameVariables)m_gameVariableDB;
					                gv.Data.AddVariable(new Field<double>(m_variableName, m_dVal));
					                EditorUtility.SetDirty(gv);
					                AssetDatabase.SaveAssets();
				                }
			                }
			                if (GUILayout.Button("Replace Variable Value"))
			                {
				                if (m_gameVariableDB == null)
				                {
					                Debug.LogError("There is no database selected.");
				                }
				                else if (!string.IsNullOrEmpty(m_variableName))
				                {
					                VP_GameVariables gv = (VP_GameVariables)m_gameVariableDB;
					                gv.Data.ReplaceVariableValue(new Field<double>(m_variableName, m_dVal));
					                EditorUtility.SetDirty(gv);
					                AssetDatabase.SaveAssets();
				                }
			                }
			                if (GUILayout.Button("Remove Variable"))
			                {
				                if (m_gameVariableDB == null)
				                {
					                Debug.LogError("There is no database selected.");
				                }
				                else if (!string.IsNullOrEmpty(m_variableName))
				                {
					                VP_GameVariables gv = (VP_GameVariables)m_gameVariableDB;
					                gv.Data.RemoveVariableValue(m_variableName);
					                EditorUtility.SetDirty(gv);
					                AssetDatabase.SaveAssets();
				                }
			                }
			                if (GUILayout.Button("Check Value"))
			                {
				                if (m_gameVariableDB == null)
				                {
					                Debug.LogError("There is no database selected.");
				                }
				                else if (!string.IsNullOrEmpty(m_variableName))
				                {
					                VP_GameVariables gv = (VP_GameVariables)m_gameVariableDB;
					                Field<double> d_f = (Field<double>)gv.Data.GetVariableValue<double>(m_variableName);
					                m_dVal = d_f.Value;
					                EditorUtility.SetDirty(gv);
					                AssetDatabase.SaveAssets();
				                }
			                }
			                break;
		                case GENERAL_FIELD_TYPES.STRING:
			                m_sVal = (string)EditorGUILayout.TextField("Value", m_sVal);
			                EditorGUILayout.BeginHorizontal();
			                if (GUILayout.Button("Add Variable"))
			                {
				                if (m_gameVariableDB == null)
				                {
					                Debug.LogError("There is no database selected.");
				                }
				                else if (!string.IsNullOrEmpty(m_variableName))
				                {
					                VP_GameVariables gv = (VP_GameVariables)m_gameVariableDB;
					                gv.Data.AddVariable(new Field<string>(m_variableName, m_sVal));
					                EditorUtility.SetDirty(gv);
					                AssetDatabase.SaveAssets();
				                }
			                }
			                if (GUILayout.Button("Replace Variable Value"))
			                {
				                if (m_gameVariableDB == null)
				                {
					                Debug.LogError("There is no database selected.");
				                }
				                else if (!string.IsNullOrEmpty(m_variableName))
				                {
					                VP_GameVariables gv = (VP_GameVariables)m_gameVariableDB;
					                gv.Data.ReplaceVariableValue(new Field<string>(m_variableName, m_sVal));
					                EditorUtility.SetDirty(gv);
					                AssetDatabase.SaveAssets();
				                }
			                }
			                if (GUILayout.Button("Remove Variable"))
			                {
				                if (m_gameVariableDB == null)
				                {
					                Debug.LogError("There is no database selected.");
				                }
				                else if (!string.IsNullOrEmpty(m_variableName))
				                {
					                VP_GameVariables gv = (VP_GameVariables)m_gameVariableDB;
					                gv.Data.RemoveVariableValue(m_variableName);
					                EditorUtility.SetDirty(gv);
					                AssetDatabase.SaveAssets();
				                }
			                }
			                if (GUILayout.Button("Check Value"))
			                {
				                if (m_gameVariableDB == null)
				                {
					                Debug.LogError("There is no database selected.");
				                }
				                else if (!string.IsNullOrEmpty(m_variableName))
				                {
					                VP_GameVariables gv = (VP_GameVariables)m_gameVariableDB;
					                Field<string> s_f = (Field<string>)gv.Data.GetVariableValue<string>(m_variableName);
					                m_sVal = s_f.Value;
					                EditorUtility.SetDirty(gv);
					                AssetDatabase.SaveAssets();
				                }
			                }
			                break;
		                case GENERAL_FIELD_TYPES.BOOL:
			                m_bVal = (bool)EditorGUILayout.Toggle("Value", m_bVal);
			                EditorGUILayout.BeginHorizontal();
			                if (GUILayout.Button("Add Variable"))
			                {
				                if (m_gameVariableDB == null)
				                {
					                Debug.LogError("There is no database selected.");
				                }
				                else if (!string.IsNullOrEmpty(m_variableName))
				                {
					                VP_GameVariables gv = (VP_GameVariables)m_gameVariableDB;
					                gv.Data.AddVariable(new Field<bool>(m_variableName, m_bVal));
					                EditorUtility.SetDirty(gv);
					                AssetDatabase.SaveAssets();
				                }
			                }
			                if (GUILayout.Button("Replace Variable Value"))
			                {
				                if (m_gameVariableDB == null)
				                {
					                Debug.LogError("There is no database selected.");
				                }
				                else if (!string.IsNullOrEmpty(m_variableName))
				                {
					                VP_GameVariables gv = (VP_GameVariables)m_gameVariableDB;
					                gv.Data.ReplaceVariableValue(new Field<bool>(m_variableName, m_bVal));
					                EditorUtility.SetDirty(gv);
					                AssetDatabase.SaveAssets();
				                }
			                }
			                if (GUILayout.Button("Remove Variable"))
			                {
				                if (m_gameVariableDB == null)
				                {
					                Debug.LogError("There is no database selected.");
				                }
				                else if (!string.IsNullOrEmpty(m_variableName))
				                {
					                VP_GameVariables gv = (VP_GameVariables)m_gameVariableDB;
					                gv.Data.RemoveVariableValue(m_variableName);
					                EditorUtility.SetDirty(gv);
					                AssetDatabase.SaveAssets();
				                }
			                }
			                if (GUILayout.Button("Check Value"))
			                {
				                if (m_gameVariableDB == null)
				                {
					                Debug.LogError("There is no database selected.");
				                }
				                else if (!string.IsNullOrEmpty(m_variableName))
				                {
					                VP_GameVariables gv = (VP_GameVariables)m_gameVariableDB;
					                Field<bool> b_f = (Field<bool>)gv.Data.GetVariableValue<bool>(m_variableName);
					                m_bVal = b_f.Value;
					                EditorUtility.SetDirty(gv);
					                AssetDatabase.SaveAssets();
				                }
			                }
			                break;
		                case GENERAL_FIELD_TYPES.GAMEOBJECT:
			                m_goVal = (GameObject)EditorGUILayout.ObjectField(m_goVal, typeof(GameObject), true);
			                EditorGUILayout.BeginHorizontal();
			                if (GUILayout.Button("Add Variable"))
			                {
				                if (m_gameVariableDB == null)
				                {
					                Debug.LogError("There is no database selected.");
				                }
				                else if (!string.IsNullOrEmpty(m_variableName))
				                {
					                VP_GameVariables gv = (VP_GameVariables)m_gameVariableDB;
					                gv.Data.AddVariable(new Field<GameObject>(m_variableName, m_goVal));
					                EditorUtility.SetDirty(gv);
					                AssetDatabase.SaveAssets();
				                }
			                }
			                if (GUILayout.Button("Replace Variable Value"))
			                {
				                if (m_gameVariableDB == null)
				                {
					                Debug.LogError("There is no database selected.");
				                }
				                else if (!string.IsNullOrEmpty(m_variableName))
				                {
					                VP_GameVariables gv = (VP_GameVariables)m_gameVariableDB;
					                gv.Data.ReplaceVariableValue(new Field<GameObject>(m_variableName, m_goVal));
					                EditorUtility.SetDirty(gv);
					                AssetDatabase.SaveAssets();
				                }
			                }
			                if (GUILayout.Button("Remove Variable"))
			                {
				                if (m_gameVariableDB == null)
				                {
					                Debug.LogError("There is no database selected.");
				                }
				                else if (!string.IsNullOrEmpty(m_variableName))
				                {
					                VP_GameVariables gv = (VP_GameVariables)m_gameVariableDB;
					                gv.Data.RemoveVariableValue(m_variableName);
					                EditorUtility.SetDirty(gv);
					                AssetDatabase.SaveAssets();
				                }
			                }
			                if (GUILayout.Button("Check Value"))
			                {
				                if (m_gameVariableDB == null)
				                {
					                Debug.LogError("There is no database selected.");
				                }
				                else if (!string.IsNullOrEmpty(m_variableName))
				                {
					                VP_GameVariables gv = (VP_GameVariables)m_gameVariableDB;
					                Field<GameObject> go_f = (Field<GameObject>)gv.Data.GetVariableValue<GameObject>(m_variableName);
					                m_goVal = go_f.Value;
					                EditorUtility.SetDirty(gv);
					                AssetDatabase.SaveAssets();
				                }
			                }
			                break;
		                }

	                }
	                else // If we are playing we set the values to the Game manager too
	                {
		                switch (m_variableTypeToAdd)
		                {
		                case GENERAL_FIELD_TYPES.INT:
			                m_iVal = (int)EditorGUILayout.IntField("Value", m_iVal);

			                EditorGUILayout.BeginHorizontal();
			                if (GUILayout.Button("Add Variable"))
			                {
				                if (m_gameVariableDB == null)
				                {
					                Debug.LogError("There is no database selected.");
				                }
				                else if (!string.IsNullOrEmpty(m_variableName))
				                {
					                VP_GameVariables gv = (VP_GameVariables)m_gameVariableDB;
					                gv.Data.AddVariable(new Field<int>(m_variableName, m_iVal));
					                EditorUtility.SetDirty(gv);
					                AssetDatabase.SaveAssets();
				                }
			                }
			                if (GUILayout.Button("Replace Variable Value"))
			                {
				                if (m_gameVariableDB == null)
				                {
					                Debug.LogError("There is no database selected.");
				                }
				                else if (!string.IsNullOrEmpty(m_variableName))
				                {
					                VP_GameVariables gv = (VP_GameVariables)m_gameVariableDB;
					                gv.Data.ReplaceVariableValue(new Field<int>(m_variableName, m_iVal));
					                EditorUtility.SetDirty(gv);
					                AssetDatabase.SaveAssets();
				                }
			                }
			                if (GUILayout.Button("Remove Variable"))
			                {
				                if (m_gameVariableDB == null)
				                {
					                Debug.LogError("There is no database selected.");
				                }
				                else if (!string.IsNullOrEmpty(m_variableName))
				                {
					                VP_GameVariables gv = (VP_GameVariables)m_gameVariableDB;
					                gv.Data.RemoveVariableValue(m_variableName);
					                EditorUtility.SetDirty(gv);
					                AssetDatabase.SaveAssets();
				                }
			                }
			                if (GUILayout.Button("Check Value"))
			                {
				                if (m_gameVariableDB == null)
				                {
					                Debug.LogError("There is no database selected.");
				                }
				                else if (!string.IsNullOrEmpty(m_variableName))
				                {
					                VP_GameVariables gv = (VP_GameVariables)m_gameVariableDB;
					                Field<int> i_f = (Field<int>)gv.Data.GetVariableValue<int>(m_variableName);
					                m_iVal = i_f.Value;
					                EditorUtility.SetDirty(gv);
					                AssetDatabase.SaveAssets();
				                }
			                }
			                break;
		                case GENERAL_FIELD_TYPES.FLOAT:
			                m_fVal = (float)EditorGUILayout.FloatField("Value", m_fVal);
			                EditorGUILayout.BeginHorizontal();
			                if (GUILayout.Button("Add Variable"))
			                {
				                if (m_gameVariableDB == null)
				                {
					                Debug.LogError("There is no database selected.");
				                }
				                else if (!string.IsNullOrEmpty(m_variableName))
				                {
					                VP_GameVariables gv = (VP_GameVariables)m_gameVariableDB;
					                gv.Data.AddVariable(new Field<float>(m_variableName, m_fVal));
					                EditorUtility.SetDirty(gv);
					                AssetDatabase.SaveAssets();
				                }
			                }
			                if (GUILayout.Button("Replace Variable Value"))
			                {
				                if (m_gameVariableDB == null)
				                {
					                Debug.LogError("There is no database selected.");
				                }
				                else if (!string.IsNullOrEmpty(m_variableName))
				                {
					                VP_GameVariables gv = (VP_GameVariables)m_gameVariableDB;
					                gv.Data.ReplaceVariableValue(new Field<float>(m_variableName, m_fVal));
					                EditorUtility.SetDirty(gv);
					                AssetDatabase.SaveAssets();
				                }
			                }
			                if (GUILayout.Button("Remove Variable"))
			                {
				                if (m_gameVariableDB == null)
				                {
					                Debug.LogError("There is no database selected.");
				                }
				                else if (!string.IsNullOrEmpty(m_variableName))
				                {
					                VP_GameVariables gv = (VP_GameVariables)m_gameVariableDB;
					                gv.Data.RemoveVariableValue(m_variableName);
					                EditorUtility.SetDirty(gv);
					                AssetDatabase.SaveAssets();
				                }
			                }
			                if (GUILayout.Button("Check Value"))
			                {
				                if (m_gameVariableDB == null)
				                {
					                Debug.LogError("There is no database selected.");
				                }
				                else if (!string.IsNullOrEmpty(m_variableName))
				                {
					                VP_GameVariables gv = (VP_GameVariables)m_gameVariableDB;
					                Field<float> f_f = (Field<float>)gv.Data.GetVariableValue<float>(m_variableName);
					                m_fVal = f_f.Value;
					                EditorUtility.SetDirty(gv);
					                AssetDatabase.SaveAssets();
				                }
			                }
			                break;
		                case GENERAL_FIELD_TYPES.DOUBLE:
			                m_dVal = (double)EditorGUILayout.DoubleField("Value", m_dVal);
			                EditorGUILayout.BeginHorizontal();
			                if (GUILayout.Button("Add Variable"))
			                {
				                if (m_gameVariableDB == null)
				                {
					                Debug.LogError("There is no database selected.");
				                }
				                else if (!string.IsNullOrEmpty(m_variableName))
				                {
					                VP_GameVariables gv = (VP_GameVariables)m_gameVariableDB;
					                gv.Data.AddVariable(new Field<double>(m_variableName, m_dVal));
					                EditorUtility.SetDirty(gv);
					                AssetDatabase.SaveAssets();
				                }
			                }
			                if (GUILayout.Button("Replace Variable Value"))
			                {
				                if (m_gameVariableDB == null)
				                {
					                Debug.LogError("There is no database selected.");
				                }
				                else if (!string.IsNullOrEmpty(m_variableName))
				                {
					                VP_GameVariables gv = (VP_GameVariables)m_gameVariableDB;
					                gv.Data.ReplaceVariableValue(new Field<double>(m_variableName, m_dVal));
					                EditorUtility.SetDirty(gv);
					                AssetDatabase.SaveAssets();
				                }
			                }
			                if (GUILayout.Button("Remove Variable"))
			                {
				                if (m_gameVariableDB == null)
				                {
					                Debug.LogError("There is no database selected.");
				                }
				                else if (!string.IsNullOrEmpty(m_variableName))
				                {
					                VP_GameVariables gv = (VP_GameVariables)m_gameVariableDB;
					                gv.Data.RemoveVariableValue(m_variableName);
					                EditorUtility.SetDirty(gv);
					                AssetDatabase.SaveAssets();
				                }
			                }
			                if (GUILayout.Button("Check Value"))
			                {
				                if (m_gameVariableDB == null)
				                {
					                Debug.LogError("There is no database selected.");
				                }
				                else if (!string.IsNullOrEmpty(m_variableName))
				                {
					                VP_GameVariables gv = (VP_GameVariables)m_gameVariableDB;
					                Field<double> d_f = (Field<double>)gv.Data.GetVariableValue<double>(m_variableName);
					                m_dVal = d_f.Value;
					                EditorUtility.SetDirty(gv);
					                AssetDatabase.SaveAssets();
				                }
			                }
			                break;
		                case GENERAL_FIELD_TYPES.STRING:
			                m_sVal = (string)EditorGUILayout.TextField("Value", m_sVal);
			                EditorGUILayout.BeginHorizontal();
			                if (GUILayout.Button("Add Variable"))
			                {
				                if (m_gameVariableDB == null)
				                {
					                Debug.LogError("There is no database selected.");
				                }
				                else if (!string.IsNullOrEmpty(m_variableName))
				                {
					                VP_GameVariables gv = (VP_GameVariables)m_gameVariableDB;
					                gv.Data.AddVariable(new Field<string>(m_variableName, m_sVal));
					                EditorUtility.SetDirty(gv);
					                AssetDatabase.SaveAssets();
				                }
			                }
			                if (GUILayout.Button("Replace Variable Value"))
			                {
				                if (m_gameVariableDB == null)
				                {
					                Debug.LogError("There is no database selected.");
				                }
				                else if (!string.IsNullOrEmpty(m_variableName))
				                {
					                VP_GameVariables gv = (VP_GameVariables)m_gameVariableDB;
					                gv.Data.ReplaceVariableValue(new Field<string>(m_variableName, m_sVal));
					                EditorUtility.SetDirty(gv);
					                AssetDatabase.SaveAssets();
				                }
			                }
			                if (GUILayout.Button("Remove Variable"))
			                {
				                if (m_gameVariableDB == null)
				                {
					                Debug.LogError("There is no database selected.");
				                }
				                else if (!string.IsNullOrEmpty(m_variableName))
				                {
					                VP_GameVariables gv = (VP_GameVariables)m_gameVariableDB;
					                gv.Data.RemoveVariableValue(m_variableName);
					                EditorUtility.SetDirty(gv);
					                AssetDatabase.SaveAssets();
				                }
			                }
			                if (GUILayout.Button("Check Value"))
			                {
				                if (m_gameVariableDB == null)
				                {
					                Debug.LogError("There is no database selected.");
				                }
				                else if (!string.IsNullOrEmpty(m_variableName))
				                {
					                VP_GameVariables gv = (VP_GameVariables)m_gameVariableDB;
					                Field<string> s_f = (Field<string>)gv.Data.GetVariableValue<string>(m_variableName);
					                m_sVal = s_f.Value;
					                EditorUtility.SetDirty(gv);
					                AssetDatabase.SaveAssets();
				                }
			                }
			                break;
		                case GENERAL_FIELD_TYPES.BOOL:
			                m_bVal = (bool)EditorGUILayout.Toggle("Value", m_bVal);
			                EditorGUILayout.BeginHorizontal();
			                if (GUILayout.Button("Add Variable"))
			                {
				                if (m_gameVariableDB == null)
				                {
					                Debug.LogError("There is no database selected.");
				                }
				                else if (!string.IsNullOrEmpty(m_variableName))
				                {
					                VP_GameVariables gv = (VP_GameVariables)m_gameVariableDB;
					                gv.Data.AddVariable(new Field<bool>(m_variableName, m_bVal));
					                EditorUtility.SetDirty(gv);
					                AssetDatabase.SaveAssets();
				                }
			                }
			                if (GUILayout.Button("Replace Variable Value"))
			                {
				                if (m_gameVariableDB == null)
				                {
					                Debug.LogError("There is no database selected.");
				                }
				                else if (!string.IsNullOrEmpty(m_variableName))
				                {
					                VP_GameVariables gv = (VP_GameVariables)m_gameVariableDB;
					                gv.Data.ReplaceVariableValue(new Field<bool>(m_variableName, m_bVal));
					                EditorUtility.SetDirty(gv);
					                AssetDatabase.SaveAssets();
				                }
			                }
			                if (GUILayout.Button("Remove Variable"))
			                {
				                if (m_gameVariableDB == null)
				                {
					                Debug.LogError("There is no database selected.");
				                }
				                else if (!string.IsNullOrEmpty(m_variableName))
				                {
					                VP_GameVariables gv = (VP_GameVariables)m_gameVariableDB;
					                gv.Data.RemoveVariableValue(m_variableName);
					                EditorUtility.SetDirty(gv);
					                AssetDatabase.SaveAssets();
				                }
			                }
			                if (GUILayout.Button("Check Value"))
			                {
				                if (m_gameVariableDB == null)
				                {
					                Debug.LogError("There is no database selected.");
				                }
				                else if (!string.IsNullOrEmpty(m_variableName))
				                {
					                VP_GameVariables gv = (VP_GameVariables)m_gameVariableDB;
					                Field<bool> b_f = (Field<bool>)gv.Data.GetVariableValue<bool>(m_variableName);
					                m_bVal = b_f.Value;
					                EditorUtility.SetDirty(gv);
					                AssetDatabase.SaveAssets();
				                }
			                }
			                break;
		                case GENERAL_FIELD_TYPES.GAMEOBJECT:
			                m_goVal = (GameObject)EditorGUILayout.ObjectField(m_goVal, typeof(GameObject), true);
			                EditorGUILayout.BeginHorizontal();
			                if (GUILayout.Button("Add Variable"))
			                {
				                if (m_gameVariableDB == null)
				                {
					                Debug.LogError("There is no database selected.");
				                }
				                else if (!string.IsNullOrEmpty(m_variableName))
				                {
					                VP_GameVariables gv = (VP_GameVariables)m_gameVariableDB;
					                gv.Data.AddVariable(new Field<GameObject>(m_variableName, m_goVal));
					                EditorUtility.SetDirty(gv);
					                AssetDatabase.SaveAssets();
				                }
			                }
			                if (GUILayout.Button("Replace Variable Value"))
			                {
				                if (m_gameVariableDB == null)
				                {
					                VP_Debug.LogError("There is no database selected.");
				                }
				                else if (!string.IsNullOrEmpty(m_variableName))
				                {
					                VP_GameVariables gv = (VP_GameVariables)m_gameVariableDB;
					                gv.Data.ReplaceVariableValue(new Field<GameObject>(m_variableName, m_goVal));
					                EditorUtility.SetDirty(gv);
					                AssetDatabase.SaveAssets();
				                }
			                }
			                if (GUILayout.Button("Remove Variable"))
			                {
				                if (m_gameVariableDB == null)
				                {
					                VP_Debug.LogError("There is no database selected.");
				                }
				                else if (!string.IsNullOrEmpty(m_variableName))
				                {
					                VP_GameVariables gv = (VP_GameVariables)m_gameVariableDB;
					                gv.Data.RemoveVariableValue(m_variableName);
					                EditorUtility.SetDirty(gv);
					                AssetDatabase.SaveAssets();
				                }
			                }
			                if (GUILayout.Button("Check Value"))
			                {
				                if (m_gameVariableDB == null)
				                {
					                VP_Debug.LogError("There is no database selected.");
				                }
				                else if (!string.IsNullOrEmpty(m_variableName))
				                {
					                VP_GameVariables gv = (VP_GameVariables)m_gameVariableDB;
					                Field<GameObject> go_f = (Field<GameObject>)gv.Data.GetVariableValue<GameObject>(m_variableName);
					                m_goVal = go_f.Value;
					                EditorUtility.SetDirty(gv);
					                AssetDatabase.SaveAssets();
				                }
			                }
			                break;
		                }

	                }

	                EditorGUILayout.EndHorizontal();
	                break;
                case DEBUG_TABS.AUDIO:

                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                    GUILayout.Label("--- ADD AUDIO NEEDED COMPONENTS --- ", EditorStyles.centeredGreyMiniLabel);
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                    EditorGUILayout.Space();



                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.BeginVertical();
                    m_audioKey = EditorGUILayout.TextField("Audio Key:", m_audioKey);          
                    m_audioVarName = EditorGUILayout.TextField("Setup Var:", m_audioVarName);   
                    m_audioClass = EditorGUILayout.TextField("Audio Class:", m_audioClass);
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Load Data By Key"))
                    {
                        LoadKeyDataInSetup(true);
                    }

                    if (GUILayout.Button("Load Data By Var"))
                    {
                        LoadKeyDataInSetup(false);
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                    GUILayout.Label("--- VP_Audio Setup --- ", EditorStyles.centeredGreyMiniLabel);
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                    EditorGUILayout.Space();
                    EditorGUILayout.BeginHorizontal();
                    m_audioType = (VP_AudioSetup.AUDIO_TYPE)EditorGUILayout.EnumPopup("Audio type:", m_audioType); //TODO
                    m_playAudioOnInit = EditorGUILayout.Toggle("Play on Init:", m_playAudioOnInit);
                    m_clipInLoop = EditorGUILayout.Toggle("Loop:", m_clipInLoop);
                    m_overrideOnPlay = EditorGUILayout.Toggle("Override Clip on Play:", m_overrideOnPlay);
                    GUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    m_audioVolume = EditorGUILayout.Slider(m_audioVolume, 0.0f, 1.0f);
                    GUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    m_loadClip = EditorGUILayout.Toggle("Add Clip to source:", m_loadClip);
                    GUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Add Component"))
                    {
                        AddAudioToSelection();
                    }
                    
                    EditorGUILayout.Space();
                    if (GUILayout.Button("Modify Component"))
                    {
                        ModifyAudio();
                    }
                    EditorGUILayout.Space();

                    if (GUILayout.Button("Romove Component"))
                    {
                        RemoveAudio();
                    }
                    GUILayout.EndHorizontal();
                    EditorGUILayout.Space();

                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                    GUILayout.Label("--- ADD AUDIO TO MANAGER--- ", EditorStyles.centeredGreyMiniLabel);
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                    EditorGUILayout.Space();

                    EditorGUILayout.BeginVertical();
                    if (GUILayout.Button("Add predefined to audio manager"))
                    {
                        AddToAudioManager();
                    }
                    GUILayout.EndVertical();
                    break;
                case DEBUG_TABS.DATABASE:
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                    GUILayout.Label("--- Data Bases --- ", EditorStyles.centeredGreyMiniLabel);
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                    EditorGUILayout.Space();
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Scan for DB"))
                    {
                        Debug.Log("Gonna scan databases...");
                        EditorApplication.ExecuteMenuItem("Window/ScriptableObject Database/Scan For Databases");
                    }
                    if (GUILayout.Button("Open DB Editor"))
                    {
                        EditorApplication.ExecuteMenuItem("Window/ScriptableObject Database/Editor Window");
                    }
                    if (GUILayout.Button("Create DB"))
                    {
                        EditorApplication.ExecuteMenuItem("Window/ScriptableObject Database/Database Wizard");
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                    GUILayout.Label("--- Parse Data --- ", EditorStyles.centeredGreyMiniLabel);
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                    EditorGUILayout.Space();
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Full Data"))
                    {
                        Debug.Log("PARSE FULL DATA...");
                        EditorApplication.ExecuteMenuItem("Lets Go Unity/Create/All Data");
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space();
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Pokémon Data (Need Types, Moves and Abilities)"))
                    {
                        EditorApplication.ExecuteMenuItem("Lets Go Unity/Create/Data/Pokémon Data (NEED MOVE AND ABILITY)");
                    }    
                    
                    if (GUILayout.Button("Pokémon Icons and Cries"))
                    {
                        EditorApplication.ExecuteMenuItem("Lets Go Unity/Create/Data/Pokémon Unity Reference Data");
                    }   
                    
                    if (GUILayout.Button("Pokémon Abilities"))
                    {
                        EditorApplication.ExecuteMenuItem("Lets Go Unity/Create/Data/Ability Data");
                    }

                    if (GUILayout.Button("Pokémon Types"))
                    {
                        EditorApplication.ExecuteMenuItem("Lets Go Unity/Create/Data/Pokémon Type Data");
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space();
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Item Data"))
                    {
                        EditorApplication.ExecuteMenuItem("Lets Go Unity/Create/Data/Item Data");
                    } 
                    if (GUILayout.Button("Move Data"))
                    {
                        EditorApplication.ExecuteMenuItem("Lets Go Unity/Create/Data/Move Data");
                    }
                    if (GUILayout.Button("TM Data"))
                    {
                        EditorApplication.ExecuteMenuItem("Lets Go Unity/Create/Data/TM Data");
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space();

                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Shadow Moves (need moves and Pokemon)"))
                    {
                        EditorApplication.ExecuteMenuItem("Lets Go Unity/Create/Data/Shadow Moves Data (need moves and Pokemon)");
                    }
                    if (GUILayout.Button("Pokemon Forms Data"))
                    {
                        EditorApplication.ExecuteMenuItem("Lets Go Unity/Create/Data/Pokemon Forms Data (need Pokemon Data)");
                    }
    
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.Space();
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Trainer Types"))
                    {
                        EditorApplication.ExecuteMenuItem("Lets Go Unity/Create/Data/Trainer Type Data");
                    }

                    if (GUILayout.Button("Trainers (Need Everything)"))
                    {
                        EditorApplication.ExecuteMenuItem("Lets Go Unity/Create/Data/Trainer List Data (Need Everything)");
                    }

                    if (GUILayout.Button("PokéDex"))
                    {
                        EditorApplication.ExecuteMenuItem("Lets Go Unity/Create/Data/Pokedex Data");
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                    GUILayout.Label("--- Refresh Data --- ", EditorStyles.centeredGreyMiniLabel);
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                    EditorGUILayout.Space();
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Refresh Moves ID"))
                    {
                        Debug.Log("Refreshing Moves...");
                        EditorApplication.ExecuteMenuItem("Lets Go Unity/Refresh Data/Parse Move IDs (Need Moves)");
                    }
                    
                    if (GUILayout.Button("Refresh Moves Targets"))
                    {
                        Debug.Log("Refreshing Moves...");
                        EditorApplication.ExecuteMenuItem("Lets Go Unity/Refresh Data/Parse Move Targets (Need Moves)");
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space();
                    break;
                case DEBUG_TABS.MISCELLANEOUS:
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
	                GUILayout.Label("--- SCREENSHOT --- ", EditorStyles.centeredGreyMiniLabel);
	                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
	                EditorGUILayout.Space();
	                EditorGUILayout.BeginHorizontal();
	                if (GUILayout.Button("Game Screenshot"))
	                {
		                TakeScreenshot();
	                }
	                if (GUILayout.Button("Open Screenshot Folder"))
	                {
                        OpenScreenshotFolder();
	                }
	                EditorGUILayout.EndHorizontal();
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                    GUILayout.Label("--- CONSOLE --- ", EditorStyles.centeredGreyMiniLabel);
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                    EditorGUILayout.Space();
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Clean Console"))
                    {
                        ClearConsole();
                    }

                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                    GUILayout.Label("--- Game Object --- ", EditorStyles.centeredGreyMiniLabel);
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                    EditorGUILayout.Space();
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Set Active"))
                    {
                        foreach (GameObject go in Selection.gameObjects)
                        {
                            go.SetActive(true);
                            EditorUtility.SetDirty(go);
                        }
                    }
                    if (GUILayout.Button("Set Active False"))
                    {
                        foreach (GameObject go in Selection.gameObjects)
                        {
                            go.SetActive(false);
                            EditorUtility.SetDirty(go);
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                    GUILayout.Label("--- PLAYER PREFS --- ", EditorStyles.centeredGreyMiniLabel);
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                    EditorGUILayout.Space();
                    EditorGUILayout.BeginVertical();
                    GUILayout.BeginHorizontal();
                    m_keyToDelete = EditorGUILayout.TextField("Playerpref Key", m_keyToDelete);
                    if (GUILayout.Button("Delete player pref"))
                    {
                        if (!string.IsNullOrEmpty(m_keyToDelete))
                        {
                            PlayerPrefs.DeleteKey(m_keyToDelete);
                            PlayerPrefs.Save();
                        }
                    }
                    GUILayout.EndHorizontal();
                    if (GUILayout.Button("Delete all player prefs"))
                    {
                        PlayerPrefs.DeleteAll();
                        PlayerPrefs.Save();
                    }
                    GUILayout.EndVertical();
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                    GUILayout.Label("--- SNAP ANCHORS --- ", EditorStyles.centeredGreyMiniLabel);
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                    EditorGUILayout.Space();
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Snap GameObject and Children"))
                    {
                        foreach (GameObject go in Selection.gameObjects)
                        {
                            VP_Debug.Log("Snaping anchors of ''" + Selection.activeTransform.gameObject.name + "'' and its children.");
                            StaticSweepingSnapAnchors(go);
                        }
                    }

                    if (GUILayout.Button("Snap GameObject"))
                    {
                        foreach (GameObject go in Selection.gameObjects)
                        {
                            VP_Debug.Log("Snaping anchors of ''" + go.name + ".");
                            StaticSnapAnchors(go);
                        }
                    }
                    GUILayout.EndHorizontal();
                    break;

                case DEBUG_TABS.BUILD:
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                    GUILayout.Label("--- BUILD --- ", EditorStyles.centeredGreyMiniLabel);
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                    EditorGUILayout.Space();
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Build " + m_buildExtension))
                    {
                        UnityEngine.Debug.Log("Building " + m_buildExtension + " for " + m_projectName);
                        BuildGame();
                    }

                    if (GUILayout.Button("Build And run "))
                    {
                        UnityEngine.Debug.Log("Building " + m_buildExtension + " for " + m_projectName);
                        BuildGame(true);
                    }
                    GUILayout.EndHorizontal();

                    // installer
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                    GUILayout.Label("--- INSTALLER --- ", EditorStyles.centeredGreyMiniLabel);
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                    EditorGUILayout.Space();
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Create installer"))
                    {
                        UnityEngine.Debug.Log("Creating installer for Color Picker Gun");
                        CreateInstaller();
                    }

                    if (GUILayout.Button("Build and Installer "))
                    {
                        UnityEngine.Debug.Log("Building .exe for Color Picker Gun and creating installer");
                        BuildAndCreateInstaller();
                    }
                    GUILayout.EndHorizontal();
                    break;
                case DEBUG_TABS.AI:
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                    GUILayout.Label("--- AI --- ", EditorStyles.centeredGreyMiniLabel);
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                    EditorGUILayout.Space();
                    EditorGUILayout.HelpBox("Convert Animator Controller to State Machine script.", MessageType.Info);
                    GUILayout.BeginVertical();
                    m_scriptName = EditorGUILayout.TextField("Script name: ", m_scriptName);
                    m_delayIfNoClip = EditorGUILayout.FloatField("Delay if no params nor clip: ", m_delayIfNoClip);
                    if (GUILayout.Button("Convert Animator"))
                    {
                        ParseAnimator();
                    }
                    GUILayout.EndVertical();
                    break;
                
            }
            
	        EditorGUILayout.EndScrollView();
        }
#endregion

#region SETTINGS
        void RefreshProperties()
        {
            m_currentTarget = EditorUserBuildSettings.activeBuildTarget;
            UnityEngine.Debug.Log("target: " + m_currentTarget.ToString());

            m_projectName = PlayerSettings.productName;
            m_companyName = PlayerSettings.companyName;
            m_bundleVersion = PlayerSettings.bundleVersion;
            m_codeVersion = "1";
            m_applicationID = PlayerSettings.GetApplicationIdentifier(EditorUserBuildSettings.selectedBuildTargetGroup);

            if (m_currentTarget == BuildTarget.StandaloneWindows || m_currentTarget == BuildTarget.StandaloneWindows64)
            {
                m_buildExtension = ".exe";
                m_codeVersion = "1";
            }
            else if (m_currentTarget == BuildTarget.StandaloneOSX)
            {
                m_buildExtension = ".app";
                m_codeVersion = "1";
            }
            else if (m_currentTarget == BuildTarget.Android)
            {
                m_buildExtension = ".apk";
                m_codeVersion = PlayerSettings.Android.bundleVersionCode.ToString();
            }
            else if (m_currentTarget == BuildTarget.iOS)
            {
                m_buildExtension = ".ipa";
                m_codeVersion = PlayerSettings.iOS.buildNumber.ToString();
            }
        }


	    void AddActionToGameObject(CUSTOM_GAME_ACTIONS _action)
	    {
		    foreach (GameObject go in Selection.gameObjects)
		    {
			    VP_InteractableObject io = go.GetComponent<VP_InteractableObject>();
			    if (io)
			    {
				    switch (_action)
				    {
				    case CUSTOM_GAME_ACTIONS.CALL_METHOD:
					    io.ActionList[m_indexList].Add(new VP_CustomCallMethodAction());
					    break;
				    case CUSTOM_GAME_ACTIONS.CHANGE_SCENE:
					    io.ActionList[m_indexList].Add(new VP_CustomChangeSceneAction());
					    break;
				    case CUSTOM_GAME_ACTIONS.CHANGE_SCENE_WITH_FADE:
					    io.ActionList[m_indexList].Add(new VP_CustomChangeSceneWithFadeAction());
					    break;
				    case CUSTOM_GAME_ACTIONS.CUSTOM_EVENTS:
					    io.ActionList[m_indexList].Add(new VP_CustomEventAction());
					    break;
				    case CUSTOM_GAME_ACTIONS.FADE_EFFECT:
					    io.ActionList[m_indexList].Add(new VP_CustomFadeEffectAction());
					    break;
				    case CUSTOM_GAME_ACTIONS.INIT_OTHER_NPC_INTERACTION:
					    io.ActionList[m_indexList].Add(new VP_CustomInitOtherNPCAction());
					    break;
				    case CUSTOM_GAME_ACTIONS.LOAD_ADDITIVE_SCENE:
					    io.ActionList[m_indexList].Add(new VP_CustomChangeSceneAdditiveAction());
					    break;
				    case CUSTOM_GAME_ACTIONS.MOVE_TO:
					    io.ActionList[m_indexList].Add(new VP_CustomMoveToPathAction());
					    break;
				    case CUSTOM_GAME_ACTIONS.PLAY_ANIMATION:
					    io.ActionList[m_indexList].Add(new VP_CustomPlayAnimation());
					    break;
				    case CUSTOM_GAME_ACTIONS.PLAY_AUDIO:
					    io.ActionList[m_indexList].Add(new VP_CustomPlayAudioAction());
					    break;
				    case CUSTOM_GAME_ACTIONS.PLAY_VFX:
					    io.ActionList[m_indexList].Add(new VP_CustomPlayVFXAction());
					    break;
				    case CUSTOM_GAME_ACTIONS.ROTATE_TO:
					    io.ActionList[m_indexList].Add(new VP_CustomSetObjectTransformValuesAction());
					    break;
				    case CUSTOM_GAME_ACTIONS.ROTATE_TO_PLAYER:
					    io.ActionList[m_indexList].Add(new VP_CustomSetObjectTransformValuesAction());
					    break;
				    case CUSTOM_GAME_ACTIONS.SET_VARIABLE:
					    io.ActionList[m_indexList].Add(new VP_CustomSetVariableAction());
					    break;
				    case CUSTOM_GAME_ACTIONS.SET_LIST_INDEX:
					    io.ActionListIndex = m_indexList;
					    break;
				    case CUSTOM_GAME_ACTIONS.SET_POSITION:
					    io.ActionList[m_indexList].Add(new VP_CustomSetObjectTransformValuesAction());
					    break;
				    case CUSTOM_GAME_ACTIONS.STOP_AUDIO:
					    io.ActionList[m_indexList].Add(new VP_CustomStopAudioAction());
					    break;
				    case CUSTOM_GAME_ACTIONS.STOP_NAVMESH_MOVEMENT:
					    io.ActionList[m_indexList].Add(new VP_CustomStopNavmeshAction());
					    break;
				    case CUSTOM_GAME_ACTIONS.UNITY_EVENTS:
					    io.ActionList[m_indexList].Add(new VP_CustomUnityEventAction());
					    break;
				    case CUSTOM_GAME_ACTIONS.WAIT:
					    io.ActionList[m_indexList].Add(new VP_CustomWaitTimeAction());
					    break;
				    default:
					    break;

				    }
			    }
		    }
	    }


        void SetupApplicationID()
        {
            string projectName = m_projectName;
            m_projectName = projectName.Replace(' ', '_');

            string companyName = m_companyName;
            m_companyName = companyName.Replace(' ', '_');
            string appID = "com." + m_companyName + "." + m_projectName;

            m_applicationID = appID;
            ApplySettings();
        }

        void ApplySettings()
        {
            m_currentTarget = EditorUserBuildSettings.activeBuildTarget;
            UnityEngine.Debug.Log("Setting Name, company, version, application identifier, and bundle version");
            PlayerSettings.productName = m_projectName;
            PlayerSettings.companyName = m_companyName;
            PlayerSettings.bundleVersion = m_bundleVersion;

            // Replace spaces with _ for avoiding blank space errors
            m_applicationID = m_applicationID.Replace(' ', '_');

            if (m_currentTarget == BuildTarget.Android)
            {
                int value = 1;
                int.TryParse(m_codeVersion, out value);
                PlayerSettings.Android.bundleVersionCode = value;
                PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, m_applicationID);
            }
            else if (m_currentTarget == BuildTarget.iOS)
            {
                PlayerSettings.iOS.buildNumber = m_codeVersion;
                PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, m_applicationID);
            }
            else if (m_currentTarget == BuildTarget.StandaloneLinux64 || m_currentTarget == BuildTarget.StandaloneOSX || m_currentTarget == BuildTarget.StandaloneWindows)
            {
                PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Standalone, m_applicationID);
            }

        }
#endregion

#region ANIMATOR TO FSM
        private void ParseAnimator()
        {
            UnityEngine.Debug.ClearDeveloperConsole();

            string path = EditorUtility.OpenFilePanel("choose the animator controller", Application.dataPath, "controller");

            if (string.IsNullOrEmpty(path))
                return;

            path = "Assets/" + VP_Utils.GetStringBetweenStrings(path, "/Assets/", ".controller") + ".controller";

            UnityEditor.Animations.AnimatorController animator = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEditor.Animations.AnimatorController>(path); //UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath("Assets/test.controller");//UnityEditor.AssetDatabase.LoadAssetAtPath < UnityEditor.Animations.AnimatorController > (path);

            if (animator == null)
            {
                UnityEngine.Debug.LogError("The animator in path " + path + " is null");
                return;
            }

            VP_FinitStateMachine fsm = new VP_FinitStateMachine(animator);
            WriteScript(fsm);
        }

        void WriteScript(VP_FinitStateMachine _fsm)
        {
            // Get filename.
            string folderPath = EditorUtility.SaveFolderPanel("Choose script folder", Application.dataPath, "Scripts");
            string extension = ".cs";

            if (string.IsNullOrEmpty(m_scriptName))
                m_scriptName = "NoNameScript";

            string path = folderPath + "/VP_" + m_scriptName + extension;

            if (File.Exists(path))
            {
                string newDir = Application.dataPath + "/BackUpScripts";
                if (!Directory.Exists(newDir))
                {
                    Directory.CreateDirectory(newDir);
                }

                string newPath = newDir + "/" + m_scriptName + "_backup" + extension;
                File.Move(path, newPath);
                UnityEngine.Debug.LogWarning("The script already existed, creating a backup of previous script at path: " + newPath);
            }

            UnityEngine.Debug.Log("Creating Script at path: " + path);

            // Create a file to write to.
            using (StreamWriter sw = File.CreateText(path))
            {
                string tdName = "";
                float stateTime = m_delayIfNoClip;
                bool hasConditions = true;

                sw.WriteLine("using System.Collections;");
                sw.WriteLine("using System.Collections.Generic;");
                sw.WriteLine("using UnityEngine;");
                sw.WriteLine("");
                sw.WriteLine(" // Script made with Animator to FSM Conversor");
                sw.WriteLine("namespace VirtualPhenix");
                sw.WriteLine("{");
                sw.WriteLine("      public class VP_" + m_scriptName + " : MonoBehaviour");
                sw.WriteLine("      {");
                sw.WriteLine("");
                sw.WriteLine("          // States in every layer ");
                sw.WriteLine("          public enum STATES");
                sw.WriteLine("          {");

                for (int i = 0; i < _fsm.m_states.Count; i++)
                {
                    for (int j = 0; j < _fsm.m_states[i].Count; j++)
                    {
                        sw.WriteLine("              " + _fsm.GetStateName(i, j) + " = " + j + ",");
                    }
                }

                sw.WriteLine("          }");
                sw.WriteLine("");
                sw.WriteLine("          // First state, first layer");
                sw.WriteLine("          [SerializeField] private STATES m_currentState = STATES." + _fsm.GetStateName(0, 0) + ";");
                sw.WriteLine("");
                sw.WriteLine("          private delegate void TransitionDetectionDelegate();");
                sw.WriteLine("          private TransitionDetectionDelegate m_TD_State;");
                sw.WriteLine("");
                sw.WriteLine("          private delegate void UpdateStateDelegate();");
                sw.WriteLine("          private UpdateStateDelegate m_US_State;");
                sw.WriteLine("");

                hasConditions = _fsm.HasCondition(0, 0);
                sw.WriteLine("          private float m_timer = 0.0f;");
                sw.WriteLine("          private float m_stateTime = " + stateTime + "f;");

                sw.WriteLine("");

                sw.WriteLine(Append_AnimatorParamVariables(_fsm));


                sw.WriteLine("          void Awake()");
                sw.WriteLine("          {");
                sw.WriteLine("");
                sw.WriteLine("          }");
                sw.WriteLine("");
                sw.WriteLine("          void Start()");
                sw.WriteLine("          {");
                sw.WriteLine("");
                sw.WriteLine("              m_timer = 0;");
                sw.WriteLine("");

                tdName = _fsm.GetStateName(0, 0);

                sw.WriteLine("             m_currentState = STATES." + tdName + ";");
                sw.WriteLine("             m_TD_State = TransitionDetection_" + tdName + ";");
                sw.WriteLine("             m_US_State = UpdateState" + tdName + "; ");
                sw.WriteLine("");
                sw.WriteLine("          }");
                sw.WriteLine("");
                sw.WriteLine("          void Update()");
                sw.WriteLine("          {");
                sw.WriteLine("");
                sw.WriteLine("              m_TD_State();"); //transition Detections
                sw.WriteLine("              m_US_State();"); //updateStates
                sw.WriteLine("              UpdateParams();");       //update params
                sw.WriteLine("");
                sw.WriteLine("          }");
                sw.WriteLine("");


                //Each frame, pass the variables from the FSM to the animator for animations to work too.
                sw.WriteLine(Append_PassAnimatorParamsToAnimator(_fsm));

                sw.WriteLine("          ///Pass the param values from the FSM->AnimController");
                sw.WriteLine("          void UpdateParams()");
                sw.WriteLine("          {");
                sw.WriteLine("               if(m_PassAnimatorParamsToAnimator && m_animator != null )");
                sw.WriteLine("               {");
                sw.WriteLine("                   PassAnimatorParamsToAnimator();");
                sw.WriteLine("               }");
                sw.WriteLine("          }");

                sw.WriteLine("");

                string A;
                string B;
                string s = "";
                for (int i = 0; i < _fsm.m_states.Count; i++)
                {
                    for (int j = 0; j < _fsm.m_states[i].Count; j++)
                    {
                        //origen state A
                        A = _fsm.GetStateName(i, j);

                        //update A
                        sw.WriteLine("");
                        sw.WriteLine("          void UpdateState" + A + "()");
                        sw.WriteLine("          {");
                        sw.WriteLine("              // TODO: UPDATE THIS STATE");
                        sw.WriteLine("          }");

                        for (int k = 0; k < _fsm.m_states[i][j].transitioNNames.Count; k++)
                        {
                            //the destination state name B
                            B = _fsm.m_states[i][j].transitioNNames[k];

                            //A->B Transition Detection
                            //sw.WriteLine(Append_TransitionDetectionAtoB(A,B));
                            sw.WriteLine(Append_TransitionDetectionAtoB(A, B, _fsm, _fsm.m_states[i][j].stateMachineTransitionsDictionary_Miki[A]));

                            //A->B OnTransition
                            sw.WriteLine(Append_TransitonFromAtoB(A, B, _fsm));

                            sw.WriteLine(s);

                        }
                    }
                }

                //Automatic Transition Detection for all states
                sw.WriteLine(Append_TransitionDetectionA(_fsm));

                sw.WriteLine("      }");
                sw.WriteLine("}");
            }

            AssetDatabase.Refresh();
        }



        /// <summary>
        /// Creates a string that includes the function for transitioning from A->B
        /// </summary>
        /// <returns>The TD etect ato b.</returns>
        /// <param name="stateA">State a.</param>
        /// <param name="stateB">State b.</param>
        string Append_TransitonFromAtoB(string stateA, string stateB, VP_FinitStateMachine _fsm)
        {
            string s = "";
            s += AppendLine("");
            s += AppendLine("          void TransitionFrom" + stateA + "To" + stateB + "()");
            s += AppendLine("          {");
            s += AppendLine("              m_currentState = STATES." + stateB + ";");
            s += AppendLine("              m_TD_State = TransitionDetection_" + stateB + ";");
            s += AppendLine("              m_US_State = UpdateState" + stateB + ";");
            s += AppendLine("              m_timer = 0.0f;");

            bool hasCondition = _fsm.HasConditionByName(stateB);
            float newTime = m_delayIfNoClip;
            if (!hasCondition && _fsm.HasClipByName(stateB))
            {
                s += AppendLine("              // CLIP NAME:" + _fsm.GetStateByName(stateB).animation.clipName + " WITH DURATION: " + _fsm.GetStateByName(stateB).animation.clipLength + " AND SPEED: " + _fsm.GetStateByName(stateB).animation.clipSpeed);
                newTime = _fsm.GetStateByName(stateB).animation.clipLength;
            }
            s += AppendLine("              m_stateTime = " + newTime + "f;");
            s += AppendLine("");
            s += AppendLine("              // TODO ANY OTHER CHANGES WHEN GOING FROM " + stateA + " TO " + stateB);
            s += AppendLine("          }");
            s += AppendLine("");


            return s;
        }

        /// <summary>
        /// Creates a string that includes the function for transition DETECTION from A->B
        /// </summary>
        /// <returns>The transition detection ato b.</returns>
        /// <param name="stateA">State a.</param>
        /// <param name="stateB">State b.</param>
        string Append_TransitionDetectionAtoB(string stateA, string stateB)
        {
            string s = "";
            s += AppendLine("");
            s += AppendLine("          bool TD_From" + stateA + "To" + stateB + "()");
            s += AppendLine("          {");
            s += AppendLine("              // TODO CHECK IF " + stateA + " GOES TO " + stateB);
            s += AppendLine("              return false;");
            s += AppendLine("          }");
            s += AppendLine("");
            return s;
        }

        string Append_TransitionDetectionAtoB(string stateA, string stateB, VP_FinitStateMachine fsm, AnimatorStateTransition[] transitions)
        {
            string s = "";
            s += AppendLine("");
            s += AppendLine("          bool TD_From" + stateA + "To" + stateB + "()");
            s += AppendLine("          {");
            s += AppendLine("              // TODO CHECK IF " + stateA + " GOES TO " + stateB);

            string paramKey;
            AnimatorCondition condition;
            for (int i = 0; i < transitions.Length; i++)
            {
                //only if transition goes to B
                if (transitions[i].destinationState.name == stateB)
                {
                    if (transitions[i].conditions.Length == 0)
                    {
                        s += AppendLine("");
                        s += AppendLine("            m_timer += Time.deltaTime;");
                        s += AppendLine("            return (m_timer >= m_stateTime)");
                    }
                    else
                    {

                        //loop through conditions for this transition
                        for (int j = 0; j < transitions[i].conditions.Length; j++)
                        {
                            //multiple conditions liked by or's
                            if (j > 0)
                            {
                                s += " && ";
                            }
                            else
                            {
                                s += "\n                return ";
                            }

                            //dictionary paramKey
                            condition = transitions[i].conditions[j];
                            paramKey = condition.parameter.Replace(" ", string.Empty);
                            string s1 = "";

                            switch (condition.mode)
                            {
                                case AnimatorConditionMode.If:
                                    s1 += "                 ap_" + paramKey;
                                    break;
                                case AnimatorConditionMode.IfNot:
                                    s1 += "                 !ap_" + paramKey;
                                    break;

                                case AnimatorConditionMode.Equals:
                                    s1 += "                 ap_" + paramKey + "==" + condition.threshold;
                                    break;
                                case AnimatorConditionMode.NotEqual:
                                    s1 += "                 ap_" + paramKey + "!=" + condition.threshold;
                                    break;
                                case AnimatorConditionMode.Greater:
                                    s1 += "                 ap_" + paramKey + ">" + condition.threshold;
                                    break;
                                case AnimatorConditionMode.Less:
                                    s1 += "                 ap_" + paramKey + "<" + condition.threshold;
                                    break;

                                default:
                                    break;
                            }
                            s += AppendLine(s1);

                        }
                    }
                }
            }
            s += ";"; //closes condition


            //s += AppendLine("              return false;");
            s += AppendLine("          }");
            s += AppendLine("");
            return s;
        }

        /// <summary>
        /// simply concatenates \n before a string and returns it.
        /// </summary>
        /// <returns>The line.</returns>
        /// <param name="s">S.</param>
        string AppendLine(string s)
        {
            return "\n" + s;
        }


        /// <summary>
        /// A function to create a string that outputs all the transition detection logic automatically
        /// </summary>
        /// <returns>The transition detection a.</returns>
        /// <param name="_fsm">Fsm.</param>
        string Append_TransitionDetectionA(VP_FinitStateMachine _fsm)
        {
            string s = "";

            string stateA;
            string stateB;
            for (int i = 0; i < _fsm.m_states.Count; i++)
            {
                for (int j = 0; j < _fsm.m_states[i].Count; j++)
                {
                    //origen state A
                    stateA = _fsm.GetStateName(i, j);


                    s += AppendLine("");
                    s += AppendLine("          void TransitionDetection_" + stateA + "()");
                    s += AppendLine("          {");


                    for (int k = 0; k < _fsm.m_states[i][j].transitioNNames.Count; k++)
                    {
                        //the destination state name B
                        stateB = _fsm.m_states[i][j].transitioNNames[k];


                        s += AppendLine("              // CHECK IF " + stateA + " GOES TO " + stateB);

                        //first time, if
                        if (k == 0)
                        {
                            s += AppendLine("              if(TD_From" + stateA + "To" + stateB + "()" + ")");
                        }
                        else
                        { //rest use else if
                            s += AppendLine("              else if(TD_From" + stateA + "To" + stateB + "()" + ")");
                        }

                        s += AppendLine("                  {TransitionFrom" + stateA + "To" + stateB + "();}");

                    }
                    s += AppendLine("          }");


                }
            }

            s += AppendLine("");



            return s;
        }

        /// <summary>
        /// Creates all the Animator Params as class variables
        /// </summary>
        /// <returns>The animator parameter variables.</returns>
        /// <param name="_fsm">Fsm.</param>
        string Append_AnimatorParamVariables(VP_FinitStateMachine _fsm)
        {
            string s = "         // Animator Params ap_...";
            s += AppendLine("          [SerializeField] Animator m_animator;");
            s += AppendLine("          [SerializeField] bool m_PassAnimatorParamsToAnimator=true;");


            foreach (string key in _fsm.m_paramAndTypes.Keys)
            {
                switch (_fsm.m_paramAndTypes[key])
                {
                    case AnimatorControllerParameterType.Bool:
                        UnityEngine.Debug.Log("Bool Param!!!");
                        s += AppendLine("          [SerializeField] bool ap_" + key.Replace(" ", string.Empty) + ";");
                        break;
                    case AnimatorControllerParameterType.Float:
                        UnityEngine.Debug.Log("Float Param!!");
                        s += AppendLine("          [SerializeField] float ap_" + key.Replace(" ", string.Empty) + ";");
                        break;
                    case AnimatorControllerParameterType.Int:
                        UnityEngine.Debug.Log("Int Param!!!");
                        s += AppendLine("          [SerializeField] int ap_" + key.Replace(" ", string.Empty) + ";");
                        break;
                    case AnimatorControllerParameterType.Trigger:
                        UnityEngine.Debug.Log("Trigger Param!!!");
                        s += AppendLine("          [SerializeField] bool ap_" + key.Replace(" ", string.Empty) + ";");
                        break;
                    default:
                        UnityEngine.Debug.Log("Other Param!!!");

                        break;
                }
            }
            s += AppendLine("");
            return s;
        }

        private string Append_PassAnimatorParamsToAnimator(VP_FinitStateMachine fsm)
        {

            string s = "";
            s += AppendLine("");
            s += AppendLine("          void PassAnimatorParamsToAnimator()");
            s += AppendLine("          {");

            string spacelessKey;
            string s1, s2;
            foreach (string key in fsm.m_paramAndTypes.Keys)
            {
                s1 = "              m_animator.Set";
                s2 = "";
                //remove spaces from key
                spacelessKey = key.Replace(" ", string.Empty);
                switch (fsm.m_paramAndTypes[key])
                {
                    case AnimatorControllerParameterType.Bool:
                        s1 += "Bool";
                        break;
                    case AnimatorControllerParameterType.Float:
                        s1 += "Float";
                        break;
                    case AnimatorControllerParameterType.Int:
                        s1 += "Integer";
                        break;
                    case AnimatorControllerParameterType.Trigger:
                        s2 += "              if(ap_" + spacelessKey + "){\n                 m_animator.SetTrigger(\"" + spacelessKey + "\");";
                        s2 += "\n               ap_" + spacelessKey + "=false;";
                        s2 += "\n            }";
                        s += AppendLine(s2);
                        continue;

                }
                s1 += "(\"" + spacelessKey + "\", ap_" + spacelessKey + "); ";
                s += AppendLine(s1);


            }

            s += AppendLine("          }");
            s += AppendLine("");


            return s;
        }


#endregion

#region BUILD AND INSTALLER
        void ClearConsole()
        {
            UnityEngine.Debug.ClearDeveloperConsole();
        }

        private string BuildGame(bool runIt = false)
        {
            UnityEngine.Debug.ClearDeveloperConsole();

            // Get filename.
            string path = EditorUtility.SaveFolderPanel("Elige la carpeta para buildear", "", "Build");

            if (string.IsNullOrEmpty(path))
                return "";

            SetupApplicationID();

            string[] levels = UnityEditor.EditorBuildSettingsScene.GetActiveSceneList(EditorBuildSettings.scenes);

            foreach (string lv in levels)
            {
                VP_Debug.Log("Saving: " + lv);
            }

            string fullPath = path + "/" + m_projectName + "-" + m_bundleVersion + "-" + m_codeVersion + m_buildExtension;

            // Build player.
            BuildPipeline.BuildPlayer(levels, fullPath, EditorUserBuildSettings.activeBuildTarget, BuildOptions.None);

	        System.Diagnostics.Process proc = new  System.Diagnostics.Process();
            proc.StartInfo.FileName = Path.GetFullPath(path);
            proc.Start();

            if (runIt && (m_currentTarget == BuildTarget.StandaloneWindows || m_currentTarget == BuildTarget.StandaloneWindows64))
            {
                // Run the game (Process class from System.Diagnostics).
                 System.Diagnostics.Process openproc = new  System.Diagnostics.Process();
                openproc.StartInfo.FileName = Path.GetFullPath(fullPath);
                openproc.Start();
            }
            
            return path;
        }
        // Remember to change the script install.bat in "Executable" to build from here with INNO Setup
        private void CreateInstaller(string buildPath = "")
        {
            if (string.IsNullOrEmpty(buildPath))
                buildPath = EditorUtility.SaveFolderPanel("Choose build folder", "", "Build");

            string name = PlayerSettings.productName;

            if (!Directory.Exists(Path.GetFullPath(buildPath + name + "_Data")))
            {
                UnityEngine.Debug.LogWarning("The game build doesn't exist. The game will be built before creating the installer.");
                BuildGame();
            }

            if (m_currentTarget == BuildTarget.StandaloneWindows || m_currentTarget == BuildTarget.StandaloneWindows64)
            {

                if (File.Exists(Path.GetFullPath(buildPath + name + ".exe")))
                {
                    UnityEngine.Debug.Log("Moving " + name + ".exe");
                    FileUtil.ReplaceFile(buildPath + name + ".exe", System.IO.Directory.GetParent(buildPath) + "/" + name + ".exe");
                }

                string batFile = "Executable/install.bat";

                if (File.Exists(batFile))
                {
                    System.Diagnostics.Process process = new System.Diagnostics.Process();
                    System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                    startInfo.FileName = Path.GetFullPath(batFile);
                    process.StartInfo = startInfo;
                    process.Start();
                    process.WaitForExit();
                }
                else
                {
                    UnityEngine.Debug.LogError("Bat file couldn't be loaded");
                }

                string installerDir = "Installer/Output";
                if (Directory.Exists(installerDir))
                    System.Diagnostics.Process.Start(Path.GetFullPath(installerDir));
            }
            else
            {
                UnityEngine.Debug.LogError("The installer is ONLY supported for Windows.");
            }
        }

        private void BuildAndCreateInstaller()
        {
            string _path = BuildGame();
            CreateInstaller(_path);
        }

#endregion

#region MISCELLANEOUS
        // Add a menu item named "Do Something with a Shortcut Key" to MyMenu in the menu bar
        // and give it a shortcut (ctrl-g on Windows, cmd-g on macOS).
        [MenuItem("Virtual Phenix/Tools/Screenshot %#J")]
        static void TakeScreenshot()
        {
            string mode = Application.isPlaying ? "Runtime\\" : "Editor\\";
#if !UNITY_EDITOR_OSX
            string path = System.IO.Directory.GetCurrentDirectory() + "\\Screenshots\\" + mode + System.DateTime.Now.ToShortDateString();      
             path = path.Replace("/", "\\");            
#else
            string path = "/tmp/Screenshots/"+ mode + System.DateTime.Now.ToShortDateString();
             path = path.Replace("\\", "/");  
#endif  
           
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            } 

#if !UNITY_EDITOR_OSX            
            ScreenCapture.CaptureScreenshot(path + "\\screenshot_" + m_screenshotID + ".png");  
#else
            ScreenCapture.CaptureScreenshot(path + "/screenshot_" + m_screenshotID + ".png");  
#endif
            UnityEngine.Debug.Log("Saved screenshot in: " + path + "\\screenshot_" + m_screenshotID);
            m_screenshotID++;

        }


        [MenuItem("Virtual Phenix/Tools/Open Screenshot Folder %#K")]
        static void OpenScreenshotFolder()
        {
            string mode = Application.isPlaying ? "Runtime\\" : "Editor\\";
#if !UNITY_EDITOR_OSX            
            string path = System.IO.Directory.GetCurrentDirectory() + "\\Screenshots\\" + mode + System.DateTime.Now.ToShortDateString();
            path = path.Replace("/", "\\");
#else
             string path = "/tmp/Screenshots/"+ mode+ System.DateTime.Now.ToShortDateString();
             path = path.Replace("\\", "/");  
#endif   
            
            if (System.IO.Directory.Exists(path))
            {
                System.Diagnostics.Process.Start(Path.GetFullPath(path));
            }
            else
            {
                System.IO.Directory.CreateDirectory(path);
                System.Diagnostics.Process.Start(Path.GetFullPath(path));
            }           
        }


        void RefreshDefineSymbols()
        {
            VP_AddDefineSymbols.RemoveDefine();
            VP_AddDefineSymbols.AddDefine();
        }        
        
        void AddDefineSymbols()
        {
            VP_AddDefineSymbols.AddDefine();
        }

        void RemoveDefineSymbols()
        {
            VP_AddDefineSymbols.RemoveDefine();
        }

        void CheckMonetization()
        {
            string monetization = "USE_MONETIZATION";
            string currentDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);

            if (!currentDefines.Contains(monetization))
            {
                string packagePath = Path.GetFullPath(new Uri(Path.Combine(Path.GetDirectoryName(Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path)), "../../Packages")).AbsolutePath);

                string jsonPath = packagePath.Replace("%20", " ") + @"\manifest.json";

                if (File.Exists(jsonPath))
                {
                    string jsonString = VP_ComplexFormatter.LoadJSONStringFromJSONFile(jsonPath, Encoding.UTF8);
                    string addPackage = "com.unity.ads";
                    if (jsonString.Contains(addPackage))
                    {
                        string[] lines = VP_ComplexFormatter.LoadAllLinesFromJSONFile(jsonPath, Encoding.UTF8);
                        foreach (string line in lines)
                        {
                            if (line.Contains(addPackage))
                            {
                                string versionStr = VP_Utils.GetStringBetweenStrings(line, ":", ",");
                                versionStr = versionStr.Replace(" ", "");
                                versionStr = versionStr.Replace("\"", "");

                                if (VP_Utils.IsStringVersionHigher("3.1.1", versionStr, 3))
                                {
                                    VP_Debug.Log("The needed version is 3.1.1. This package will be installed.");
                                    AddPackage(addPackage);

                                }
                                else
                                {
                                    PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, currentDefines + "; " + monetization);
                                }

                                break;
                            }
                        }
                    }
                    else
                    {
                        VP_Debug.Log("The needed version of Ads package is 3.1.1. This package will be installed.");
                        AddPackage(addPackage);
                    }
                }

            }
        }

        void AddPackage(string _packageID)
        {
            UnityEditor.PackageManager.Client.Add(_packageID);
        }

        public static void StaticSnapAnchors(GameObject o)
        {

            RectTransform recTransform = null;
            RectTransform parentTransform = null;

            if (o.transform.parent != null)
            {
                if (o.gameObject.tag != "IgnoreSnapAnchors")
                {
                    if (o.GetComponent<RectTransform>() != null)
                        recTransform = o.GetComponent<RectTransform>();
                    else
                    {
                        VP_Debug.LogError(o.name + " Doesn't have RectTransform. SnapAnchors must be used only with UI objects. Please select a objet with RectTransform. Returning function.");
                        return;
                    }

                    if (parentTransform == null)
                    {
                        parentTransform = o.transform.parent.GetComponent<RectTransform>();
                    }
                    Undo.RecordObject(recTransform, "Snap Anchors");

                    Vector2 offsetMin = recTransform.offsetMin;
                    Vector2 offsetMax = recTransform.offsetMax;

                    Vector2 anchorMin = recTransform.anchorMin;
                    Vector2 anchorMax = recTransform.anchorMax;

                    Vector2 parent_scale = new Vector2(parentTransform.rect.width, parentTransform.rect.height);


                    recTransform.anchorMin = new Vector2(
                        anchorMin.x + (offsetMin.x / parent_scale.x),
                        anchorMin.y + (offsetMin.y / parent_scale.y));

                    recTransform.anchorMax = new Vector2(
                        anchorMax.x + (offsetMax.x / parent_scale.x),
                        anchorMax.y + (offsetMax.y / parent_scale.y));

                    recTransform.offsetMin = Vector2.zero;
                    recTransform.offsetMax = Vector2.zero;
                }
            }

        }

        public static void StaticSweepingSnapAnchors(GameObject o)
        {
            StaticSnapAnchors(o);
            for (int i = 0; i < o.transform.childCount; i++)
            {
                StaticSweepingSnapAnchors(o.transform.GetChild(i).gameObject);
            }
        }


       // [MenuItem("Virtual Phenix/Tools/Snap Selected UI objects")]
        public static void SnapSelectedObjects()
        {
            foreach (GameObject go in Selection.gameObjects)
            {
                VP_Debug.Log("Snaping anchors of ''" + Selection.activeTransform.gameObject.name + "''");
                StaticSnapAnchors(go);
            }
        }

        //[MenuItem("Virtual Phenix/Tools/Snap Objects and its Children")]
        public static void SnapObjectAndChildren()
        {
            foreach (GameObject go in Selection.gameObjects)
            {
                VP_Debug.Log("Snaping anchors of ''" + Selection.activeTransform.gameObject.name + "'' and its children.");
                StaticSweepingSnapAnchors(go);
            }
        }

#endregion

#region AUDIO

        void AddToAudioManager()
        {
            VP_AudioManager manager = FindObjectOfType<VP_AudioManager>();

            if (manager)
            {
                if (!ReplaceKeyInSetup())
                {
                    if (!AddKeyToSetup())
                    {
                        return;
                    }
                }


                foreach (GameObject g in Selection.gameObjects)
                {
                    VP_Audio item = g.GetComponent<VP_Audio>();

                    if (item)
                    {
                        manager.AddToPredefined(item.Audio);
                    }
                }
            }
            else
            {
                UnityEngine.Debug.ClearDeveloperConsole();
                UnityEngine.Debug.LogWarning("Audio manager is not in the scene. Add one before trying to add an item.");
            }
        }

        AudioClip LoadClip()
        {
            string path = EditorUtility.OpenFilePanel("choose the Audio Clip", Application.dataPath, "wav");

            if (!string.IsNullOrEmpty(path))
            {
                string extension = Path.GetExtension(path);

                // UnityEngine.Debug.Log("Extension: " + extension);

                if (string.IsNullOrEmpty(extension))
                    extension = ".wav";

                path = "Assets/" + VP_Utils.GetStringBetweenStrings(path, "/Assets/", extension) + extension;

                return UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>(path);
            }

            return null;
        }

        void ModifyAudio()
        {
            if (string.IsNullOrEmpty(m_audioKey))
            {
                VP_Debug.LogError("NO AUDIO KEY");
                return;
            }

            if (!ReplaceKeyInSetup())
            {
                if (!AddKeyToSetup())
                {
                    return;
                }
            }

            AudioClip _clip = null;

            if (m_loadClip)
                _clip = LoadClip();

            foreach (GameObject g in Selection.gameObjects)
            {
                VP_Audio item = g.GetComponent<VP_Audio>();
                AudioSource source = g.GetComponent<AudioSource>();

                if (item)
                {
                    if (!item.Audio.m_source)
                    {
                        if (!source)
                            item.Audio.m_source = g.AddComponent<AudioSource>();
                        else
                            item.Audio.m_source = source;
                    }

                    SetAudioValues(item, _clip);
                }
            }
        }

        void SetAudioValues(VP_Audio _audio, AudioClip _clip)
        {
            VP_AudioItem item = _audio.Audio;

            item.m_originalVolume = m_audioVolume;
            item.m_inLoop = m_clipInLoop;
            item.m_override = m_overrideOnPlay;
            item.m_playOnAwake = m_playAudioOnInit;

            item.m_source.playOnAwake = m_playAudioOnInit;
            item.m_source.loop = m_clipInLoop;
            item.m_source.volume = m_audioVolume;
            item.m_clip = _clip;
            item.m_type = m_audioType;
            item.m_clip = _clip;

            if (m_loadClip && _clip)
                item.m_source.clip = _clip;

            if (!string.IsNullOrEmpty(m_audioKey))
                item.m_key = m_audioKey;

        }

        void RemoveAudio()
        {
            foreach (GameObject g in Selection.gameObjects)
            {
                VP_Audio item = g.GetComponent<VP_Audio>();
                AudioSource source = g.GetComponent<AudioSource>();

                if (item)
                    DestroyImmediate(item);

                if (source)
                    DestroyImmediate(source);
            }
        }

        void LoadKeyDataInSetup(bool _byKey)
        {
            string path = Application.dataPath + "/VirtualPhenix/Scripts/Setup/VP_AudioSetup.cs";
            bool found = false;
            if (File.Exists(path))
            {
                VP_Debug.Log("audio setup script exists");

                string[] arrLine = File.ReadAllLines(path, Encoding.UTF8);
                VP_Debug.Log("Trying to load var " + m_audioVarName);

                foreach (string str in arrLine)
                {
                    if (str.Contains("class"))
                    {
                        int pFrom = str.IndexOf("class ")+6;
                        int pTo = str.LastIndexOf("")+1;
                        string result = str.Substring(pFrom, pTo - pFrom);
                        m_audioClass = result;
                    }

                    if (_byKey)
                    {
                        if (str.Contains("= " + "\"" + m_audioKey + "\"") || str.Contains("=" + "\"" + m_audioKey + "\""))
                        {
                            VP_Debug.Log("found key " + m_audioKey);
                            int pFrom = str.IndexOf("public const string ") + 20;
                            int pTo = str.LastIndexOf("=");
                            string result = str.Substring(pFrom, pTo - pFrom);
                            found = true;
                            m_audioVarName = result;

                            break;
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(m_audioVarName))
                        {

                            if (str.Contains(m_audioVarName + "=") || str.Contains(m_audioVarName + " ="))
                            {
   
                                int pFrom = str.IndexOf("=") + 1;
                                int pTo = str.LastIndexOf(";");
                                string result = str.Substring(pFrom, pTo - pFrom);
                                found = true;
                                result = result.Replace('"',' ');
                                result = System.Text.RegularExpressions.Regex.Replace(result, " ", "");
                                m_audioKey = result;
                                VP_Debug.Log("found Variable " + m_audioVarName + " with key "+m_audioKey);
                                break;
                            }
                           
                        }
                    }
                }
            }
            else
            {
                VP_Debug.LogError("FILE DOESNT EXIST: " + path);
            }
            if (!found)
            {
                if (_byKey)
                    VP_Debug.LogError("Couldn't find: " + m_audioKey + " in AudioSetup.");
                else
                    VP_Debug.LogError("Not found " + m_audioVarName + " in AudioSetup.");
            }
     
        }

        bool ReplaceKeyInSetup()
        {
            string path = Application.dataPath + "/VirtualPhenix/Scripts/Setup/VP_AudioSetup.cs";
            bool found = false;
            if (File.Exists(path))
            {
                VP_Debug.Log("audio setup script exists");

                string[] arrLine = File.ReadAllLines(path, Encoding.UTF8);

                int line_to_edit = 0;
                foreach (string str in arrLine)
                {
                    if (str.Contains("= " + m_audioVarName))
                    { 
                        found = true;
                        arrLine[line_to_edit] = "     public const string " + m_audioVarName + " =" + "\""+ m_audioKey + "\""+ ";";
                        VP_Debug.Log("Audio with key " + m_audioKey + " added to VP_audioSetup.cs");
                        break;
                    }

                    line_to_edit++;
                }

                if (found)
                {
                    File.WriteAllLines(path, arrLine, Encoding.UTF8);
                    AssetDatabase.Refresh();
                }
             
            }
            else
            {
                VP_Debug.LogError("FILE DOESNT EXIST: " + path);
            }
            return found;
        }

        bool AddKeyToSetup()
        {
            string path = Application.dataPath + "/VirtualPhenix/Scripts/Setup/VP_AudioSetup.cs";

            if (File.Exists(path))
            {
                VP_Debug.Log("audio setup script exists");

                string[] arrLine = File.ReadAllLines(path, Encoding.UTF8);

                if (IsInSetup())
                {
                    return true;
                }

                string[] arrLineNew = new string[arrLine.Length + 1];
                int counter = 0;
                int line_to_edit = 0;

                if (string.IsNullOrEmpty(m_audioClass))
                    m_audioClass = "General";

                bool found = false;
                VP_Debug.Log("Looking for class " + m_audioClass);
                foreach (string str in arrLine)
                {
                    if (str.Contains("class " + m_audioClass))
                    {
                        VP_Debug.Log("Found class " + m_audioClass);
                        line_to_edit = counter + 2;
                        found = true;
                        break;
                    }

                    counter++;

                }

                if (found)
                {
                    bool IsAlreadyThere = false;

                    foreach (string str in arrLine)
                    {
                        if (str.Contains(m_audioVarName) && str.Contains(m_audioKey))
                        {
                            VP_Debug.Log(m_audioKey + " is already in VP_AudioSetup.cs");
                            IsAlreadyThere = true;
                            break;
                        }
                    }

                    if (IsAlreadyThere)
                        return true;

                    for (int i = 0; i < line_to_edit; i++)
                    {
                        arrLineNew[i] = arrLine[i];
                    }

                    string varName = m_audioVarName;
                    if (string.IsNullOrEmpty(m_audioVarName))
                    {
                        varName = m_audioKey.ToUpper();
                    }

                    arrLineNew[line_to_edit] = "     public const string " +m_audioVarName + " =" + "\"" + m_audioKey + "\"" + ";";
                    VP_Debug.Log("Audio with key " + m_audioKey + " added to VP_AudioSetup.cs");
                    for (int j = line_to_edit; j < arrLine.Length; j++)
                    {
                        arrLineNew[j + 1] = arrLine[j];
                    }

                    VP_Debug.Log("Added to AudioSetup.cs");
                    File.WriteAllLines(path, arrLineNew, Encoding.UTF8);
                    AssetDatabase.Refresh();
                }
                else
                {
                    arrLineNew = File.ReadAllLines(path, Encoding.UTF8);

                    string[] newArr = new string[arrLineNew.Length+4];
                    int cc = 0;
                    foreach (string str in arrLineNew)
                    {
                        newArr[cc] = str;
                        cc++;
                    }

                    m_audioClass = "General";
                    string[] classArr = new string[6];
                    classArr[0] = "        public static class " + m_audioClass;
                    classArr[1] = "        {";
                    classArr[2] = "            public const string " + m_audioVarName + " = " + "\""+ m_audioKey+ "\";";
                    classArr[3] = "        }";
                    classArr[4] = "    }";
                    classArr[5] = "}";
                    int _c = 0;
                    newArr[newArr.Length - 7] = "";
                    for (int i = newArr.Length - 6; i < newArr.Length;i++)
                    {
                        newArr[i] = classArr[_c];
                        _c++;
                    }

                    VP_Debug.Log("Created class " + m_audioClass);

                    File.WriteAllLines(path, newArr, Encoding.UTF8);
                    AssetDatabase.Refresh();
                }

                return true;
            }
            else
            {
                VP_Debug.LogError("FILE DOESNT EXIST: " + path);
            }
            return true;
        }

        bool IsInSetup()
        {
            string path = Application.dataPath + "/VirtualPhenix/Scripts/Setup/VP_AudioSetup.cs";
            string[] arrLine = File.ReadAllLines(path, Encoding.UTF8);

            List<string> KeyList = new List<string>();
            foreach (string str in arrLine)
            {
                if (str.Contains("= " + m_audioKey))
                {

                    int pFrom = str.IndexOf("public const string ") + 20;
                    int pTo = str.LastIndexOf("=");

                    string result = str.Substring(pFrom, pTo - pFrom);

                    pFrom = str.IndexOf("= ");
                    pTo = str.LastIndexOf(";");
                    result = str.Substring(pFrom, pTo - pFrom);
                    KeyList.Add(result);
                    break;
                }
            }

            return KeyList.Contains(m_audioKey);
        }

        void AddAudioToSelection()
        {
            if (string.IsNullOrEmpty(m_audioKey))
            {
                VP_Debug.LogError("NO AUDIO KEY");
                return;
            }

            if (!AddKeyToSetup())
            {
                return;
            }
           
            AudioClip _clip = null;

            if (m_loadClip)
                _clip = LoadClip();

            foreach (GameObject g in Selection.gameObjects)
            {
                VP_Audio item = g.GetComponent<VP_Audio>();
                AudioSource source = g.GetComponent<AudioSource>();
                if (item)
                {
                    if (!item.Audio.m_source)
                    {
                        if (!source)
                            item.Audio.m_source = g.AddComponent<AudioSource>();
                        else
                            item.Audio.m_source = source;
                    }
                }
                else
                {
                    item = g.AddComponent<VP_Audio>();
                    item.CreateAudio();

                    if (!source)
                        item.Audio.m_source = g.AddComponent<AudioSource>();
                    else
                        item.Audio.m_source = source;
                }

                SetAudioValues(item, _clip);
            }
        }

#endregion

#region LOCALIZATION

        public void DeleteLanguageFile(int _index)
        {
            SystemLanguage language = m_activeLanguages[_index];

            string folder = Application.dataPath + "/VirtualPhenix/Resources/";

            string extension = "";

            switch (m_languageParser)
            {
                case LANGUAGE_PARSER.CSV:
                    extension = VP_LocalizationSetup.Extension.CSV;
                    folder += VP_LocalizationSetup.Folder.LOCALIZATION_FOLDER_CSV;
                    break;
                case LANGUAGE_PARSER.JSON:
                    extension = VP_LocalizationSetup.Extension.JSON;
                    folder+= VP_LocalizationSetup.Folder.LOCALIZATION_FOLDER_JSON;
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
                    VP_Debug.Log("The language " + assetToDelete + " was deleted. ");
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
                VP_Debug.LogError("Key is null. It can't be added.");
                return;
            }

            if (m_activeLanguages.Count == 0)
            {
                VP_Debug.LogError("There are no languages availables in " + m_languageParser.ToString() + " format. Please, create at least one.");
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
                VP_Debug.LogError("Key is null. It can't be added.");
                return;
            }

            if (m_activeLanguages.Count == 0)
            {
                VP_Debug.LogError("There are no languages availables in " + m_languageParser.ToString() + " format. Please, create at least one.");
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
                VP_Debug.LogError("There are no text availables.");
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
                    Debug.Log("Original Text: "+ text+ ", Language: " + activeLang.ToString() + " has text: " + result + " code: "+ language2 + " and language 1 was " + language1);
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
                VP_Debug.LogError("Key is null. It can't be added.");
                return;
            }

            if (m_activeLanguages.Count == 0)
            {
                VP_Debug.LogError("There are no languages availables in "+m_languageParser.ToString()+" format. Please, create at least one.");
                return;
            }
            if (languageFiles == null)
            {
                string folder = Application.dataPath + "/VirtualPhenix/Resources/";
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

                            VP_Debug.Log("Replaced " + m_localizationKey + " in " + asset.name);
                        }
                        break;
                    }
                }

                if (!_found)
                {
                    if (_forceAdd)
                        VP_Debug.Log("Key " + m_localizationKey + " not found in " + asset.name+". it will be added instead.");

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

                    VP_Debug.Log("Added " + m_localizationKey + " to " + asset.name);
                }

                counter++;
            }

            AssetDatabase.Refresh();
        }

        public void LoadTextFromKey()
        {
            if (string.IsNullOrEmpty(m_localizationKey))
            {
                VP_Debug.LogError("Key is null. Texts can't be loaded.");
                return;
            }

            if (m_activeLanguages.Count == 0)
            {
                VP_Debug.LogError("There are no languages availables in " + m_languageParser.ToString() + " format. Please, create at least one.");
                return;
            }

            string folder = "";// Application.dataPath + "/VirtualPhenix/Resources/";
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
                    Debug.LogError("localization text not found for language "+ asset.name);
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
                    VP_Debug.LogError("Key is null. It can't be added.");
                    return;
                }

                if (m_activeLanguages.Count == 0)
                {
                    VP_Debug.LogError("There are no languages availables in " + m_languageParser.ToString() + " format. Please, create at least one.");
                    return;
                }
                if (languageFiles == null)
                {
                    string folder = Application.dataPath + "/VirtualPhenix/Resources/";
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

                                VP_Debug.Log("Replaced " + _key + " in " + asset.name);
                            }
                            break;
                        }
                    }

                    if (!_found)
                    {
                        if (_forceAdd)
                            VP_Debug.Log("Key " + _key + " not found in " + asset.name + ". it will be added instead.");

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

                        VP_Debug.Log("Added " + _key + " to " + asset.name);
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
                VP_Debug.LogError("Key is null. It can't be added.");
                return;
            }

            if (m_activeLanguages.Count == 0)
            {
                VP_Debug.LogError("There are no languages availables in " + m_languageParser.ToString() + " format. Please, create at least one.");
                return;
            }

            if (languageFiles == null)
            {
                string folder = Application.dataPath + "/VirtualPhenix/Resources/";
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

                        VP_Debug.Log("Removed "+m_localizationKey + " from " + asset.name);
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
            string fullPath = Application.dataPath + "/VirtualPhenix/Resources/" + folder + m_languageToAdd.ToString() + extension;
            if (m_default != null)
            {
                if (!File.Exists(fullPath))
                {
                    VP_Debug.Log("Created language file in" + extension + " extension at " + fullPath + " with default english data. You should modify everything to " + m_languageToAdd.ToString() + ".");
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
            VP_Debug.CleanConsole();
            VP_Debug.Log("Refreshing languages...");
            LoadLocalizationFiles();
        }

        public void OpenLocalizationFolder()
        {
            string folder = Application.dataPath + "/VirtualPhenix/Resources/";

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
          
            VP_Debug.Log("Open folder: " + folder);
       
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
