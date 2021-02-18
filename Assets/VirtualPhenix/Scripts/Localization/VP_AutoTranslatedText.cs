using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Localization
{
	[AddComponentMenu(VP_PrefixSetup.MAIN_PREFIX+"/UI/Auto Translated TMP Text")]
	public class VP_AutoTranslatedText : VP_MonoBehaviour
	{
		[Header("AutoTranslation Text"),Space]
		[SerializeField] protected TMPro.TMP_Text m_text;
	
		[Header("Config"),Space]
		public bool m_translateDirectly = true;
		[TextArea]public string m_key;

		protected string m_originalText;

		protected override void Initialize()
		{
			base.Initialize();
			m_originalText = m_text != null && m_key.IsNullOrEmpty() ? m_text.text : m_key;
			
			if (m_translateDirectly)
				TranslateText(SystemLanguage.English);
		
		}
		
		protected virtual void Reset()
		{
			m_text = GetComponent<TMPro.TMP_Text>();
			m_originalText = m_text.text;
			m_key = m_text.text;
		}
		
		protected override void StartAllListeners()
		{
			base.StartAllListeners();
			
					
			VP_EventManager.StartListening<SystemLanguage>(VP_EventSetup.Localization.TRANSLATE_TEXTS, TranslateText);
		}
		
		protected override void StopAllListeners()
		{
			base.StopAllListeners();
			
			VP_EventManager.StopListening<SystemLanguage>(VP_EventSetup.Localization.TRANSLATE_TEXTS, TranslateText);
		}
		

		protected virtual void TranslateText(SystemLanguage _language)
		{
			if (m_text != null)
				m_text.text = VP_LocalizationManager.GetText(m_originalText, false, true, m_originalText);
		}
	}

}