using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix.Localization;
using VirtualPhenix.ResourceReference;

namespace VirtualPhenix
{
	[CreateAssetMenu(fileName = "Localization Resources", menuName = "Virtual Phenix/Localization/Localization Resources", order = 1)]
	public class VP_LocalizationResources : VP_ResourceReferencer<SystemLanguage, VirtualPhenix.Localization.VP_LocalizationData>
	{
		public VP_SerializableDictionary<SystemLanguage, VirtualPhenix.Localization.VP_LocalizationData> LocalizationData { get { return m_resources; } }
		
		public List<SystemLanguage> GetLanguages()
		{
			List<SystemLanguage> languages = new List<SystemLanguage>();
			foreach (SystemLanguage l in m_resources.Keys)
				languages.Add(l);
				
			return languages;
		}
		
		public VP_LocalizationData GetData(SystemLanguage _language = SystemLanguage.English)
		{
			return m_resources.ContainsKey(_language) ? m_resources[_language] : UnityEngine.Resources.Load<VP_LocalizationData>("Localization/English_Data");
		}
		
		public string GetText(SystemLanguage _language, string _key, string _fallback="")
		{
			if (string.IsNullOrEmpty(_fallback))
				_fallback = _key;
			
			return m_resources.ContainsKey(_language) ? m_resources[_language].GetText(_key, _fallback) : _fallback;
		}
		
		
		public virtual bool CanTranslateText(SystemLanguage lang, string _key)
		{
			return  m_resources.ContainsKey(lang) ? m_resources[lang].CanTranslateText(_key) : false;
		}

		public virtual bool HasLanguage(SystemLanguage _language)
		{
			var langs = GetLanguages();
			if (langs == null || langs.Count == 0)
				return false;
			
			return langs.Contains(_language);
		}
		
		public virtual bool HasLanguage(string _language)
		{
			SystemLanguage lang = SystemLanguage.Unknown;
			
			System.Enum.TryParse(_language, out lang);
			var langs = GetLanguages();
			if (langs == null || langs.Count == 0)
				return false;
			
			return lang != SystemLanguage.Unknown ? langs.Contains(lang) : false;
		}
	}

}