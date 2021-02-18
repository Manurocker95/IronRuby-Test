using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using System.IO;
using VirtualPhenix;
using VirtualPhenix.Localization;
using System.Linq;

namespace VirtualPhenix.Localization
{
    [
     DefaultExecutionOrder(VP_ExecutingOrderSetup.LOCALIZATION_MANAGER),
     AddComponentMenu(VP_PrefixSetup.MAIN_PREFIX + "/Localization/Localization Manager")
    ]
    /// <summary>
    /// Text manager has every text from every language inside of its dictionary. This launches the event to translate every text!
    /// </summary>
    public class VP_LocalizationManager : VP_SingletonMonobehaviour<VP_LocalizationManager>
    {

        #region Variables
        [Header("Languages"), Space(10)]
        /// <summary>
        /// Current language - Will change if pressing language button
        /// </summary>
        [SerializeField] protected List<SystemLanguage> m_activeLanguages;
	   
	    
	    [Header("Config"), Space(10)]
	    [SerializeField] protected VP_LocalizationConfig m_localizationConfig;
	    [SerializeField] protected bool m_saveConfigOnNew = true;
	    [SerializeField] protected bool m_saveOnLanguageChange = true;
	    [SerializeField] protected bool m_loadConfig = true;
	    [SerializeField] protected bool m_canChangeLanguage = true;
	    [SerializeField] protected bool m_useSaveManager = false;
	    [SerializeField] protected string m_configFileName = "locConfig.VPData";
	    [SerializeField] protected SaveLocation m_configLocation = SaveLocation.PlayerPrefs;
	    [SerializeField] protected Formatter.FORMATTER_METHOD m_formatter =  Formatter.FORMATTER_METHOD.NO_CONTEXT;
	   
	    protected string fullLocation;
	    
	    [Header("Text Resources"), Space(10)]
#if ODIN_INSPECTOR
        /// <summary>
        /// Dictionary of dictionaries with every text of every language
        /// </summary>
	    [Sirenix.Serialization.OdinSerialize] protected VP_LocalizationResources m_localizationResources;
	    /// <summary>
	    /// Language codes for specific Platform. This can be used for Online auto-translation or getting the API SystemLanguage from SDKs
	    /// </summary>
	    [Sirenix.Serialization.OdinSerialize] protected VP_LanguageCodeReferencer m_languageCodes;
#else	    
	    /// <summary>
	    /// Dictionary of dictionaries with every text of every language
	    /// </summary>
	    [SerializeField] protected VP_LocalizationResources m_localizationResources;
	    [SerializeField] protected VP_LanguageCodeReferencer m_languageCodes;
#endif

        public virtual SystemLanguage[] ActiveLanguages { get { return m_activeLanguages.ToArray(); } }
	    public virtual SystemLanguage CurrentLanguage { get { return m_localizationConfig.CurrentLanguage; } }
	    public virtual bool CanChangeLanguage { get { return m_localizationConfig.CanChangeLanguage; } }
        public virtual string CurrentLanguagename { get { return CurrentLanguage.ToString(); } }
	    public virtual bool UsePlayerPrefs { get { return m_configLocation == SaveLocation.PlayerPrefs; } }
	    
        #endregion

        #region Monobehaviour and Initialization

        protected override void StartAllListeners()
        {
            VP_EventManager.StartListening<SystemLanguage, bool>(VP_EventSetup.Settings.LANGUAGE, ChangeLanguage);

        }

        protected override void StopAllListeners()
        {
            VP_EventManager.StopListening<SystemLanguage, bool>(VP_EventSetup.Settings.LANGUAGE, ChangeLanguage);
        }

	    protected override void Reset()
	    {
	    	base.Reset();
	    	m_localizationResources = Resources.Load<VP_LocalizationResources>("Database/Localization/Localization Resources");
	    	m_languageCodes = Resources.Load<VP_LanguageCodeReferencer>("Database/Localization/Localization Codes");
	    }

        /// <summary>
        /// Manager initialization and text parse. We set the (first) current language here.
        /// </summary>
        protected override void Initialize()
	    {
	    	base.Initialize();
	    	
#if UNITY_SWITCH
		    m_configLocation = SaveLocation.Console;
#endif	        
		    
		    fullLocation = GetFullLocationPath();
		    
		    LoadLanguageData(out m_activeLanguages);
            
		    if (m_loadConfig)
		    {
			    LoadConfig(out m_localizationConfig);
		    }
	        	
		    ChangeLanguage(CurrentLanguage, false);
            
        }

	    public virtual void NextLanguage()
	    {
	    	if (m_localizationConfig.CanChangeLanguage && m_activeLanguages.Count > 0)
	    	{
	    		int idx = m_activeLanguages.IndexOf(CurrentLanguage);
	    		if (idx+1 < m_activeLanguages.Count)
	    		{
	    			idx++;
	    		}
	    		else
	    		{
	    			idx = 0;
	    		}
	    		
	    		SystemLanguage lang = m_activeLanguages.ElementAt(idx);
	    		if (lang != CurrentLanguage)
	    		{
	    			ChangeLanguage(lang, true);
	    		}
	    	}
	
	    }

	    protected virtual SystemLanguage DefaultLanguage()
	    {
		    return m_localizationConfig.DefaultLanguage;
	    }

        protected virtual SystemLanguage GetCustomCurrentLanguage()
        {
	        return DefaultLanguage();
        }

	    public virtual void LoadConfig(out VP_LocalizationConfig _config)
	    {
		    SystemLanguage systemLang = Application.systemLanguage;
	    	
		    _config = new VP_LocalizationConfig() { CanChangeLanguage = m_canChangeLanguage, CurrentLanguage = systemLang };
	    	

		    if (m_canChangeLanguage)
		    {
			    if (UsePlayerPrefs)
			    {
			    	string key = VP_LocalizationSetup.PlayerPrefs.LAST_LANGUAGE + Application.productName;
				    string lastConfig = PlayerPrefs.GetString(key, "");
				    if (lastConfig.IsNullOrEmpty())
				    {
					    _config = new VP_LocalizationConfig() { CanChangeLanguage = m_canChangeLanguage, CurrentLanguage = m_localizationResources != null && m_localizationResources.HasLanguage(systemLang) ? systemLang : DefaultLanguage() };
				    }
				    else
				    {
				    	Formatter.VP_Formatter.LoadFromPlayerPrefs(key, out _config, (bool _loaded)=>
				    	{
				    		Debug.Log("Config Loaded "+_loaded);
				    	}, m_formatter);
				    	
				    		    
					    if (_config == null)
					    {
						    _config = new VP_LocalizationConfig() { CanChangeLanguage = m_loadConfig, CurrentLanguage = m_localizationResources != null && m_localizationResources.HasLanguage(systemLang) ? systemLang : DefaultLanguage() };
					    }
				    }
			    }
			    else
			    {
			    	if (ExistConfigFile())
			    	{
			    		
#if UNITY_EDITOR || !UNITY_SWITCH				    	
				    	if (m_useSaveManager)
				    	{
					    	var sm = Save.VP_SaveManagerBase.Instance;
#if ODIN_INSPECTOR
							sm.LoadFileFromPath(GetFullSaveManagerLocationPath(sm), out _config, (bool _loaded)=>
					    	{
						    	Debug.Log("Loaded config: "+_loaded);
					    	}, m_formatter);
#else
							sm.LoadFileFromPathRegular(GetFullSaveManagerLocationPath(sm), out _config, (bool _loaded) =>
							{
								Debug.Log("Loaded config: " + _loaded);
							}, m_formatter);
#endif
						}
						else
				    	{
					    	Formatter.VP_Formatter.LoadObjectFromDataFile(fullLocation, false, out _config, m_formatter, (bool _loaded)=>
					    	{
						    	Debug.Log("Loaded config: "+_loaded);
					    	});
				    	}
#else
							var sm = Save.VP_SaveManagerBase.Instance;
					    	sm.NintendoSwitchLoadGame(out _config, (bool _loaded)=>
					    	{
						    	Debug.Log("Loaded config: "+_loaded);
					    	}, fullLocation);
#endif

						if (_config == null)
				    	{
					    	_config = new VP_LocalizationConfig() { CanChangeLanguage = m_canChangeLanguage, CurrentLanguage = m_localizationResources != null && m_localizationResources.HasLanguage(systemLang) ? systemLang : DefaultLanguage() };
				    	}
			    	}
			    }
		    }
	    	else
		    {
			    _config.CurrentLanguage = (m_activeLanguages.Contains(systemLang)) ? systemLang : GetCustomCurrentLanguage();
		    }
		    
		    m_canChangeLanguage = _config.CanChangeLanguage;
		    
		    if (m_saveConfigOnNew)
		    {
			    SaveConfig();
		    }
	    }

	    public virtual bool ExistConfigFile()
	    {
		    return (File.Exists(fullLocation));
	    }

		public virtual string GetFullSaveManagerLocationPath(Save.VP_SaveManagerBase sm)
		{						
			if (m_configFileName.IsNullOrEmpty())
				m_configFileName = "locConfig.VPData";

			if (sm == null)
				return VP_Utils.GetSaveLocation(m_configLocation, CustomSaveLocation, VP_LocalizationSetup.PlayerPrefs.LAST_LANGUAGE + Application.productName);
#if !UNITY_SWITCH || UNITY_EDITOR
			return sm.GetSaveLocation(m_configLocation) + "/" + m_configFileName;
#else	
			return string.Format("{0}:/{1}", sm.GetSaveLocation(m_configLocation), m_configFileName);
#endif
		}

		public virtual string GetFullLocationPath()
	    {
	    	if (m_configFileName.IsNullOrEmpty())
		    	m_configFileName = "locConfig.VPData";
		    	
#if !UNITY_SWITCH || UNITY_EDITOR
	    	return GetLocation()+"/"+m_configFileName;
#else	
		    return string.Format("{0}:/{1}", GetLocation(), m_configFileName);
#endif	    	
	    }

	    public virtual string GetLocation()
	    {
		    Save.VP_SaveManagerBase sm = Save.VP_SaveManagerBase.Instance;
		    return m_useSaveManager && sm != null ? sm.GetSaveLocation(m_configLocation) : VP_Utils.GetSaveLocation(m_configLocation, CustomSaveLocation, VP_LocalizationSetup.PlayerPrefs.LAST_LANGUAGE + Application.productName);
	    }
	    

	    public virtual void SaveConfig()
	    {
#if UNITY_EDITOR || !UNITY_SWITCH	

		    if (m_useSaveManager)
		    {
			    var sm = Save.VP_SaveManagerBase.Instance;
			    if (sm)
			    {
#if ODIN_INSPECTOR
					sm.SaveFileAtPath(fullLocation, m_localizationConfig, (bool _saved)=>
				    {
				    	Debug.Log("Saved Config "+_saved);
				    }, m_formatter);
#else
					sm.SaveFileRegularAtPath(fullLocation, m_localizationConfig, (bool _saved) =>
					{
						Debug.Log("Saved Config " + _saved);
					}, m_formatter);
#endif
			    }
			    else
			    {
				    Formatter.VP_Formatter.SaveObjectoToDataFile(m_configLocation, fullLocation, UsePlayerPrefs, m_formatter,(bool _saved)=>
				    {
					    Debug.Log("Saved Config "+_saved);
				    });
			    }
		    }
		    else
		    {
			    Formatter.VP_Formatter.SaveObjectoToDataFile(m_configLocation, fullLocation, UsePlayerPrefs, m_formatter,(bool _saved)=>
			    {
			    	Debug.Log("Saved Config "+_saved);
			    });
		    }
#else
		    var sm = Save.VP_SaveManagerBase.Instance;
		    if (sm)
		    {
			    sm.NintendoSwitchSaveGame(m_localizationConfig, (bool _saved)=>
			    {
				    Debug.Log("Saved Config "+_saved);
			    }, fullLocation);
		    }
		    else
		    {
		    	Debug.LogError("No Save manager to handle FS");
		    }
#endif
		}

		public virtual string CustomSaveLocation()
	    {
	    	return Application.persistentDataPath;
	    }

        #endregion

        #region Language & Parse methods

        protected virtual void LoadLanguageData(out List<SystemLanguage> _languageList)
	    {        	
	        _languageList = m_localizationResources != null ? m_localizationResources.GetLanguages() : new List<SystemLanguage>();
		    
		    SystemLanguage m_defaultLanguage = DefaultLanguage();
		    	
		    if (_languageList == null)
			    _languageList = new List<SystemLanguage>() { m_defaultLanguage };
		    else if (!_languageList.Contains(m_defaultLanguage))
			    _languageList.Add(m_defaultLanguage);
		    	
		    if (m_localizationResources == null && m_localizationConfig.UseGoogleTranslation)
		    {
			    if (!_languageList.Contains(Application.systemLanguage) )
			    {
				    _languageList.Add(Application.systemLanguage);
			    }
		    	
			    if (!_languageList.Contains(SystemLanguage.English))
			    {
				    _languageList.Add(SystemLanguage.English);
			    }
		    }

	    }
	    
        protected virtual bool ExistLanguage(string _language)
        {
	        return m_localizationResources != null ? m_localizationResources.HasLanguage(_language) : false;
        }

        protected virtual bool ExistLanguageTextFile(string folder, string _extension, string _language)
        {
            return File.Exists(folder + _language + _extension);
        }

        public virtual bool CanTranslateText(string _key)
	    {
		    if (m_localizationConfig.UseGoogleTranslation && VP_Utils.HasInternetConnection())
		    {
		    	return true;
		    }
        	
		    return m_localizationResources != null ? m_localizationResources.CanTranslateText(CurrentLanguage, _key) : false;
        }

        public static SystemLanguage GetCurrentLanguage()
        {
            if (!Instance)
	            return SystemLanguage.English;

            return Instance.CurrentLanguage;
        }

        public virtual void TranslateTexts(SystemLanguage _lang)
        {
	        ChangeLanguage(_lang, true);
        }

        /// <summary>
        /// Method for changing the language. Calling this method will launch translate event so every text shows correctly
        /// </summary>
        /// <param name="language"></param>
        public virtual void ChangeLanguage(SystemLanguage language, bool translateTexts = true)
        {
	        if (!m_initialized || (language == CurrentLanguage && !translateTexts))
            {
		        return;
            }

	        if (m_canChangeLanguage)
	        {
		        if (language != CurrentLanguage)
		        {
			        if (CanUseAutoTranslation())
			        {
				        SetCurrentLanguage(language, m_saveOnLanguageChange);
			        }
			        else
			        {
				        bool parsed = m_localizationResources != null ? m_localizationResources.HasLanguage(language) : false;

				        if (parsed)
				        {
					        SetCurrentLanguage(language, m_saveOnLanguageChange);
				        }
			        }
		        }
	        }

            if (translateTexts)
            {
	            VP_Debug.Log("Current Language: " + CurrentLanguage.ToString());
                VP_EventManager.TriggerEvent(VP_EventSetup.Localization.TRANSLATE_TEXTS, CurrentLanguage);
            }
        }

	    protected virtual void SetCurrentLanguage(SystemLanguage language, bool _saveIt)
	    {
		    m_localizationConfig.CurrentLanguage = language;
		    if (_saveIt)
		    {
			    SaveConfig();
		    }
	    }
	    
        protected virtual void SaveCustomCurrentLanguage()
        {
            // Save it as you want
        }

	    public virtual bool CanUseAutoTranslation()
	    {
	    	return m_localizationConfig.UseGoogleTranslation && VP_Utils.HasInternetConnection();
	    }
	    
	    public virtual string GetPhraseFromGoogle(string url)
	    {
		    int idx = GetIndexOfEndQuote(url, 4);
		    string final = url.Substring(4, idx - 4).ReplaceUnicodeCharacters();
		    return FixRichTextFromString(final);
	    }
	    
	    public virtual int GetIndexOfEndQuote(string originalText, int currentIndex)
	    {
		    for (int i = currentIndex; i < originalText.Length; i++)
		    {
		    	if (originalText[i] == '\"' && originalText[i-1] != '\\')
		    	{
		    		return i;
		    	}
		    }
		    
		    return originalText.Length;
	    }
	    
	    public virtual string FixRichTextFromString(string originalText)
	    {
		    string finalText = originalText;
	    	try
	    	{
		    	for (int i = 0; i < originalText.Length; i++)
		    	{
			    	if (originalText[i] == '<')
			    	{
				    	int idx = originalText.IndexOf(">",i);
				    	if (idx != -1)
				    	{
					    	string toDelete = originalText.Substring(i, idx-i);
					    	string fixedStr = toDelete.Replace(" ","");
					    	finalText = finalText.Replace(toDelete, fixedStr);
				    	}
			    	}
		    	}
		    	return finalText;
	    	}
	    	catch (System.Exception e)
	    	{
	    		Debug.LogError("Error:"+e.Message+"- "+e.StackTrace);
		    	return finalText;
	    	}
	    }
	    

	    public virtual string AutoTranslate(string _text, string _fallback = "", SystemLanguage _original = SystemLanguage.English, SystemLanguage _toLanguage = SystemLanguage.English)
	    {	    	
	    	string language1 = "";
	    	string language2 = "";

	    	if (m_languageCodes == null)
	    	{
		    	Dictionary<SystemLanguage, string> languagesCodes = new Dictionary<SystemLanguage, string>()
		    	{
			    	{ SystemLanguage.Afrikaans, "af" },
			    	{ SystemLanguage.Spanish, "es" },
			    	{ SystemLanguage.English, "en" },
			    	{ SystemLanguage.French, "fr" },
			    	{ SystemLanguage.German, "de" },
			    	{ SystemLanguage.Italian, "it" },
			    	{ SystemLanguage.Catalan, "ca" },
			    	{ SystemLanguage.ChineseSimplified, "zh-CN" },
			    	{ SystemLanguage.ChineseTraditional, "zh-TW" },
			    	{ SystemLanguage.Japanese, "ja" },
			    	{ SystemLanguage.Hungarian, "hu" },
			    	{ SystemLanguage.Arabic, "ar" },
			    	{ SystemLanguage.Bulgarian, "bg" },
			    	{ SystemLanguage.Greek, "el" },
			    	{ SystemLanguage.Korean, "ko" },
			    	{ SystemLanguage.Portuguese, "pt" },
			    	{ SystemLanguage.Romanian, "ro" },
			    	{ SystemLanguage.Dutch, "da" }	
	            };
	            
		    	language1= languagesCodes.ContainsKey(_original) ? languagesCodes[_original] : "en";
		    	language2 = languagesCodes.ContainsKey(_toLanguage) ? languagesCodes[_toLanguage] : "en";
	    	}
	    	else
	    	{
	    		language1 = m_languageCodes.LanguageCode(_original);
	    		language2 = m_languageCodes.LanguageCode(_toLanguage);
	    	}
		    
		    var url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl={language1}&tl={language2}&dt=t&q={System.Uri.EscapeUriString(_text)}";

		    var webClient = new System.Net.WebClient
		    {
			    Encoding = System.Text.Encoding.UTF8
            };
            
		    var result = webClient.DownloadString(url);
		    try
		    {
			    result = GetPhraseFromGoogle(result);
			    return result;
		    }
	    	catch
	    	{
		    	return _fallback;
	    	}
	    }


	    public virtual string GetTranslatedTextInLanguage(string _text, SystemLanguage _language, SystemLanguage _originalTextLanguage = SystemLanguage.English)
	    {
		    if (CanUseAutoTranslation())
		    	return AutoTranslate(_text, _text, _originalTextLanguage, _language);
		    	
		    if (!m_localizationResources)
			    return _text;
			    
		    return m_localizationResources.GetText(_language, _text, _text); 
	    }


        /// <summary>
        /// Get the text which key is defined in pb_SetupTexts
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual string GetTextTranslated(string key, string _fallback = "")
	    {
	    	SystemLanguage m_defaultLanguage = DefaultLanguage();
	    	SystemLanguage m_currentLanguage = CurrentLanguage;
	    	
		    if (CanUseAutoTranslation())
		    	return AutoTranslate(key, _fallback, m_defaultLanguage, m_currentLanguage);
        	
	        if (!m_localizationResources)
		        return _fallback.IsNullOrEmpty() ? key : _fallback;

	        return m_localizationResources.GetText(m_currentLanguage, key, _fallback);
        }

        /// <summary>
        /// Get the text which key is defined in pb_SetupTexts
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual string GetTextTranslatedINTL(string key, string _fallback = "", params string[] _params)
	    {
		    SystemLanguage m_defaultLanguage = DefaultLanguage();
	    	SystemLanguage m_currentLanguage = CurrentLanguage;
	    	
		    if (CanUseAutoTranslation())
		    	return VP_INTL.ParseString(AutoTranslate(key, _fallback, m_defaultLanguage, m_currentLanguage), _params );
        	
		    if (!m_localizationResources)
			    return VP_INTL.ParseString(_fallback.IsNullOrEmpty() ? key : _fallback, _params);
        	
            return VP_INTL.ParseString(m_localizationResources.GetText(m_currentLanguage, key, _fallback), _params);
        }

        public virtual string GetTextTranslatedINTL(string key,params string[] _params)
	    {
		    if (!m_localizationResources)
			    return VP_INTL.ParseString(key, _params);
        	
	    	SystemLanguage m_currentLanguage = CurrentLanguage;
        	
            return VP_INTL.ParseString( m_localizationResources.GetText(m_currentLanguage, key), _params);
        }

        public static string GetTextINTL(string key, bool showLog = false, bool showkey = true, string _fallback = "", params string[] _params)
        {
            if (!Instance)
                return showkey ? key : string.IsNullOrEmpty(_fallback) ? "ERROR [404]" : _fallback;

            return Instance.GetTextTranslatedINTL(key, _fallback, _params);
        }

        public static string GetTextINTL(string key, params string[] _params)
        {
            if (!Instance)
                return key;

            return Instance.GetTextTranslatedINTL(key, "", _params);
        }

        public static string GetText(string key, bool showLog = false, bool showkey = true, string _fallback = "")
        {
            if (!Instance)
                return showkey ? key : string.IsNullOrEmpty(_fallback) ? "ERROR [404]" : _fallback;

            return Instance.GetTextTranslated(key, _fallback);
        }

	    public static string GetTextInLanguage(string text, SystemLanguage _language, SystemLanguage _originalLanguage = SystemLanguage.English)
	    {
		    if (!Instance)
			    return text;

		    return Instance.GetTranslatedTextInLanguage(text, _language, _originalLanguage);
	    }


        public static string GetText(string key, string _fallback)
        {
            if (!Instance)
                return _fallback;

            return Instance.GetTextTranslated(key, _fallback);
        }

        public static bool CanTranslate(string _key)
        {
            if (!Instance)
                return false;

            return Instance.CanTranslateText(_key);
        }

        #endregion
    }

}
