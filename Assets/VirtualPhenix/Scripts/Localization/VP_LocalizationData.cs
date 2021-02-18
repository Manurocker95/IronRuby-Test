using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Localization
{
	[CreateAssetMenu(fileName = "Localization Data", menuName = "Virtual Phenix/Localization/Localization Data", order = 1)]
	public class VP_LocalizationData : VP_ScriptableObject
	{
		[SerializeField] protected SystemLanguage m_language = SystemLanguage.English;
		#if ODIN_INSPECTOR
		[Sirenix.Serialization.OdinSerialize] protected VP_TextAssetDictionary m_textAssets = new VP_TextAssetDictionary();
		[Sirenix.Serialization.OdinSerialize] protected VP_TextItemDictionary m_texts = new VP_TextItemDictionary();
		#else
		[SerializeField] protected VP_TextAssetDictionary m_textAssets = new VP_TextAssetDictionary();
		[SerializeField] protected VP_TextItemDictionary m_texts = new VP_TextItemDictionary();
		#endif
		public virtual SystemLanguage Language { get { return m_language; } }
		public virtual VP_TextItemDictionary Texts { get { return m_texts; } }
		
		public virtual string GetText(string _key, string _fallback="")
		{
			if (string.IsNullOrEmpty(_fallback))
				_fallback = _key;
			
			return m_texts.ContainsKey(_key) ? m_texts[_key].Text : _fallback;
		}
		
		public virtual bool ParseTexts(LANGUAGE_PARSER _parser = LANGUAGE_PARSER.CSV)
		{
			bool parsed = false;
			if (m_textAssets != null && m_textAssets.ContainsKey(_parser))
			{
				switch (_parser)
				{
				case LANGUAGE_PARSER.CSV:
					parsed = ParseCSV(m_textAssets[_parser]);
					break;
				case LANGUAGE_PARSER.JSON:
					parsed = ParseJSON(m_textAssets[_parser]);
					break;
				case LANGUAGE_PARSER.XML:
					parsed = ParseXML(m_textAssets[_parser]);
					break;
				}
			}
			return parsed;
		}
		
		
		public virtual bool CanTranslateText(string _key)
		{
			return (!string.IsNullOrEmpty(_key) && m_texts != null && m_texts.ContainsKey(_key));
		}

		protected virtual bool ParseCSV(TextAsset csv)
		{
			var dic = VP_CSVParser.ParseTextItemCSV(csv);

			m_texts = new VP_TextItemDictionary();

			if (dic == null || dic.Count == 0)
				return false;

			foreach (var it in dic.Keys)
				m_texts.Add(it, dic[it]);

			return (m_texts.Count > 0);
		}

		protected virtual bool ParseXML(TextAsset xml)
		{
			m_texts = VP_XMLParser.ParseTextItemXML(xml);
			return (m_texts.Count > 0);
		}

		protected virtual bool ParseJSON(TextAsset json)
		{
			m_texts = VP_JSONParser.ParseTextItemJSON(json);
			return (m_texts.Count > 0);
		}

	}
}
