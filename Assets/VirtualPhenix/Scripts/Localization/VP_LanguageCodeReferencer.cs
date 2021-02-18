using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix.ResourceReference;

namespace VirtualPhenix.Localization
{
	[CreateAssetMenu(fileName = "Localization Codes", menuName = "Virtual Phenix/Localization/Localization Code Resources", order = 1)]
	public class VP_LanguageCodeReferencer : VP_ResourceReferencer<SystemLanguage, string>
	{
		public VP_LanguageCodeReferencer()
		{
			m_resources = new VP_SerializableDictionary<SystemLanguage, string>()
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
				{ SystemLanguage.Dutch, "da" },
			};
		}
		
		public virtual string LanguageCode(SystemLanguage _language)
		{
			if (m_resources.Contains(_language))
				return m_resources[_language];
				
			return "en";
		}
		
		public virtual string LanguageCode(string _langStr)
		{
			SystemLanguage _language = SystemLanguage.English;
			System.Enum.TryParse(_langStr, out _language);
			
			if (m_resources.Contains(_language))
				return m_resources[_language];
				
			return "en";
		}
	}

}