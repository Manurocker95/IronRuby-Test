using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace VirtualPhenix
{
	public class VP_DefineSymbolEditor : VP_EditorWindow<VP_DefineSymbolEditor>
	{
		protected int m_welcomeWindowWidth = 488;
		protected int m_welcomeWindowHeight = 600;

		protected Vector2 m_scrollPos;
		
		/// <summary>
		/// Symbols that will be added to the editor
		/// </summary>
		protected Dictionary<string, bool> m_symbols;
		protected int m_lastIndex = 7;
		protected List<string>  m_avoidSymbols;
		protected VP_DefineSymbolsReferencer  m_symbolDictionary;

		protected string m_defineSymbolsPath = "";
		public const string DEFINE_SYMBOL_ASSET_NAME = "VP_DefineSymbols.asset";
		public const string DEFINE_SYMBOL_ASSET_DEFAULT_PATH = "\\Assets\\VirtualPhenix\\Resources\\Editor\\Define Symbols\\";
		public const string DEFINE_SYMBOL_TXT_NAME = "VP_DefineSymbol_Config.txt";
		public const string DEFINE_SYMBOL_TXT_DEFAULT_PATH = "\\Assets\\Resources\\Editor\\Define Symbols\\";

        public override string WindowName => "Define Symbol Editor";


        [UnityEditor.MenuItem("Virtual Phenix/Window/Define Symbol Editor %F2")]
		public static void ShowWindow()
		{
			m_instance = GetWindow<VP_DefineSymbolEditor>("VP Define Symbol Editor");
			Instance.m_lastIndex = VP_AddDefineSymbols.Symbols.Length;
		}
		
		protected virtual void Awake()
		{
			LoadMainDictionary();
		}

		protected virtual void LoadMainDictionary()
        {
			string path = GetSaveFilePath();
			m_defineSymbolsPath = (System.IO.File.Exists(path)) ? System.IO.File.ReadAllText(path) : CreateConfig();

			VP_Utils.GetObjectOfTypeInProject(out m_symbolDictionary, "VP_DefineSymbolsReferencer", CheckPath);

			if (m_symbolDictionary == null)
			{
				m_symbolDictionary = Resources.Load<VP_DefineSymbolsReferencer>("Editor/" + DEFINE_SYMBOL_ASSET_NAME);
			}

			if (m_symbolDictionary != null)
			{
				RefreshLists();
			}
		}

		protected virtual string DefineSymbolAssetPath()
        {
			string path = System.IO.Directory.GetCurrentDirectory() + DEFINE_SYMBOL_ASSET_DEFAULT_PATH + DEFINE_SYMBOL_ASSET_NAME;
#if UNITY_EDITOR_OSX
			path = path.Replace("\\", "/");
#endif
			return path;
		}

		protected virtual string CreateConfig()
		{
			string txt =GetSaveFilePath();
			string path = DefineSymbolAssetPath();

			System.IO.File.WriteAllText(txt, path);
			return txt;
		}

		public virtual string GetSaveFilePath()
		{
			string p = GetCurrentDirectory() + DEFINE_SYMBOL_TXT_DEFAULT_PATH;

#if UNITY_EDITOR_OSX
			p = p.Replace("\\", "/");
#endif

			if (!System.IO.Directory.Exists(p))
			{
				Debug.Log("Creating " + p);
				System.IO.Directory.CreateDirectory(p);
			}

			string path = p + DEFINE_SYMBOL_TXT_NAME;
#if UNITY_EDITOR_OSX
			path = path.Replace("\\", "/");
#endif
			return path;
		}

		protected virtual bool CheckPath(VP_DefineSymbolsReferencer _symbol)
		{
			return VP_Utils.CheckPathOfDefineSymbol(_symbol, m_defineSymbolsPath);
		}

		protected virtual bool CheckInternalName(VP_DefineSymbolsReferencer _symbol)
		{
			return _symbol.InternalName == Application.productName;
		}
		
		public virtual string GetCurrentDirectory()
        {
			return System.IO.Directory.GetCurrentDirectory();
		}

		public virtual void RefreshLists()
		{
			RefreshAvoidSymbols();
			RefreshSymbols();
		}

		public virtual void ResetLists()
        {
			RefreshAvoidSymbols(true);
			RefreshSymbols();		
		}

		public virtual void RefreshSymbols()
		{
			m_lastIndex = VP_AddDefineSymbols.Symbols.Length-1;
		
			m_symbols = new Dictionary<string, bool>();
			foreach (string str in VP_AddDefineSymbols.Symbols)
			{
				bool active = !m_avoidSymbols.Contains(str);
				m_symbols.Add(str, active);
			}
		}


		public virtual void RefreshAvoidSymbols(bool _default = false)
		{
			m_avoidSymbols = (m_symbolDictionary == null || _default) ? new List<string>(VP_AddDefineSymbols.DefaultAvoidSymbols) : new List<string>(m_symbolDictionary.GetAvoidSymbols());
		}

		public virtual void SetupDefineSymbols(bool _remove = true)
		{
			if (m_symbolDictionary)
			{
				m_symbolDictionary.SetAvoidSymbols(m_avoidSymbols);

				if (_remove)
					VP_AddDefineSymbols.RefreshDefine(m_symbolDictionary);
				else
					VP_AddDefineSymbols.AddDefine(m_symbolDictionary);
			}
		}

		protected virtual void SaveSO()
		{
			if (m_symbolDictionary)
			{
				m_symbolDictionary.SetDefaultSymbols(m_symbols);
				UnityEditor.EditorUtility.SetDirty(m_symbolDictionary);		
				UnityEditor.AssetDatabase.SaveAssets();
				UnityEditor.AssetDatabase.Refresh();
				UnityEditor.EditorUtility.FocusProjectWindow();
			}
		}

		public virtual void ResetSymbolDictionary()
		{
			ResetLists();

			if (m_symbolDictionary != null)
            {
				m_symbolDictionary.ClearResources();
				m_symbolDictionary.SetDefaultSymbols(m_symbols);
				m_symbolDictionary.SetAvoidSymbols(m_avoidSymbols);
			}
		}

		public virtual void SetCurrentDictionaryAsMain()
        {
			if (m_symbolDictionary != null)
            {
				string whereToSaveAll = UnityEditor.AssetDatabase.GetAssetPath(m_symbolDictionary);

				string assetsStr = "Assets";
				int index = VirtualPhenix.VP_Utils.GetWordIndexInString(whereToSaveAll, assetsStr) - assetsStr.Length;
				whereToSaveAll = whereToSaveAll.Substring(index, whereToSaveAll.Length - index);


				System.IO.File.WriteAllText(GetSaveFilePath(), whereToSaveAll);
			}
			else
            {
				Debug.LogError("Can't set null dictionary");
            }

		}
		
		public virtual void CreateNewSymbolDictionary()
		{
			string whereToSaveAll = EditorUtility.SaveFolderPanel("Choose the folder to save Define Symbol Dictionary", "Assets/VirtualPhenix/Resources/Editor/", "Define Symbols");
			if (string.IsNullOrEmpty(whereToSaveAll))
			{
				return;
			}

			string assetsStr = "Assets";
			int index = VirtualPhenix.VP_Utils.GetWordIndexInString(whereToSaveAll, assetsStr) - assetsStr.Length;
			whereToSaveAll = whereToSaveAll.Substring(index, whereToSaveAll.Length - index);
			string fullpath = whereToSaveAll + "/" + DEFINE_SYMBOL_ASSET_NAME;
			if (whereToSaveAll.IsNotNullNorEmpty())
			{
				ResetLists();
				
				VP_DefineSymbolsReferencer asset = ScriptableObject.CreateInstance<VP_DefineSymbolsReferencer>();
				asset.SetDefaultSymbols(m_symbols);
				asset.SetAvoidSymbols(m_avoidSymbols);
				
				string assetPathAndName = UnityEditor.AssetDatabase.GenerateUniqueAssetPath(fullpath);
				UnityEditor.AssetDatabase.CreateAsset(asset, assetPathAndName);

				m_symbolDictionary = asset;

				UnityEditor.AssetDatabase.SaveAssets();
				UnityEditor.AssetDatabase.Refresh();
				UnityEditor.EditorUtility.FocusProjectWindow();	
			}
		}

		public override void CreateHowToText()
        {
			if (m_howToTexts == null)
				InitHowToTextList();

			CreateHowToTitle(ref m_howToTexts, "DEFINE SYMBOL EDITOR");

			m_howToTexts.Add(new VP_HTUText() 
			{
				Text = "This window can configure all the needed Define Symbols for Virtual Phenix Framework.\n\n" +
						"This needs an scriptable object of the class VP_DefineSymbolReferencer which can be created manually by clicking on \n" +
						"Right Click (project window) > Virtual Phenix > Resource Dictionary > Define Symbol > Referencer. \n\n" +
						"Define Symbol Config object can be created clicking on \"Create New\" button in this Window.", 
				m_labelType = VP_EditorWindowStyleSetup.RegularTextStyle, 
				m_spaces = new KeyValuePair<bool, int>(true, 2) 
			});

			m_howToTexts.Add(new VP_HTUText()
			{
				Text = "The main configuration file path will be saved in " + DEFINE_SYMBOL_TXT_DEFAULT_PATH + 
				". The linked config .asset will be" +
				"loaded when \"Load Main\" button is pressed\n for easy access. If you want to change the main, load it by dragging it in the object field" +
				" and click on \"Set as current\". This system can have multiple configurations and swap them when desired.",
				m_labelType = VP_EditorWindowStyleSetup.RegularTextStyle,
				m_spaces = new KeyValuePair<bool, int>(true, 2)
			});

			m_howToTexts.Add(new VP_HTUText()
			{
				Text = "When you are done modifying the config, save it by clicking on \"Save ScriptableObject\". \n\nIf you want to setup the Define Symbols of the loaded" +
				"config, click on the button \"Setup\" to set them in Player Settings. If you need to deserialize and re-serialize, use \"Remove and Add Button\". \n\n" +
				"Friendly reminder: You will need the needed packages already installed in your project.",
				m_labelType = VP_EditorWindowStyleSetup.RegularTextStyle,
				m_spaces = new KeyValuePair<bool, int>(true, 2)
			});

			m_howToTexts.Add(new VP_HTUText()
			{
				Text = "* If your config is loaded but the list is not displayed, click on \"Refresh List\" Button.",
				m_labelType = VP_EditorWindowStyleSetup.RegularTextStyle,
				m_spaces = new KeyValuePair<bool, int>(true, 1)
			});

			m_howToTexts.Add(new VP_HTUText()
			{
				Text = "* If you want to load the default settings, click on \"Reset\" Button.",
				m_labelType = VP_EditorWindowStyleSetup.RegularTextStyle,
				m_spaces = new KeyValuePair<bool, int>(true, 1)
			});

			VP_HowToUseWindow.ShowWindow(HowToWindowWidth, HowToWindowHeight, m_howToTexts.ToArray());
		}

		protected override void OnGUI()
		{
			base.OnGUI();
			EditorGUILayout.Space();
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Symbol Dictionary", EditorStyles.boldLabel);
			m_symbolDictionary = EditorGUILayout.ObjectField(m_symbolDictionary, typeof(VP_DefineSymbolsReferencer), true) as VP_DefineSymbolsReferencer;

			var oldColor = GUI.backgroundColor;
			var style = new GUIStyle(GUI.skin.button);
			style.normal.textColor = Color.white;

			DrawButtonWithColor("Create New", CreateNewSymbolDictionary, Color.green, Color.white);

			if (GUILayout.Button("Load Main"))
			{
				LoadMainDictionary();
			}

			if (m_symbolDictionary != null)
			{
				DrawButtonWithColor("Reset", ResetSymbolDictionary, Color.red, Color.white);
			}
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Refresh the list inside Symbol Dictionary", EditorStyles.boldLabel);
			if (GUILayout.Button("Refresh List"))
			{
				RefreshLists();
			}
			EditorGUILayout.EndHorizontal();
			
			if (m_symbolDictionary != null && m_symbols != null)
			{
				EditorGUILayout.BeginVertical();
				m_scrollPos = EditorGUILayout.BeginScrollView(m_scrollPos, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
		
				EditorGUILayout.Space();
				EditorGUILayout.LabelField("Define Symbols", EditorStyles.boldLabel);
				for(int i = 0; i < m_symbols.Count; i++)
				{
					var el = m_symbols.ElementAt(i);
					m_symbols[el.Key] = EditorGUILayout.Toggle(el.Key, el.Value, GUILayout.ExpandWidth(true));
					EditorGUILayout.Space();
				}
		
				EditorGUILayout.EndScrollView();
				EditorGUILayout.EndVertical();
			
				EditorGUILayout.Space();
				EditorGUILayout.Space();
				EditorGUILayout.Space();
				EditorGUILayout.BeginHorizontal();
				if (GUILayout.Button("Save ScriptableObject"))
				{
					SaveSO();
				}
				
				if (GUILayout.Button("Set as Current"))
				{
					SetCurrentDictionaryAsMain();
				}
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.Space();

				// Buttons to Set the symbols
				EditorGUILayout.BeginHorizontal();
				GUI.backgroundColor = Color.green;
				if (GUILayout.Button("Setup Define Symbols"))
				{
					SetupDefineSymbols(false);
				}
				GUI.backgroundColor = oldColor;

				GUI.backgroundColor = Color.red;
				if (GUILayout.Button("Remove and Re-Add Define Symbols",style))
				{
					SetupDefineSymbols();
				}
				GUI.backgroundColor = oldColor;
				EditorGUILayout.EndHorizontal();
			}
		}
	}
}
