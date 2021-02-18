using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix.Localization;

namespace VirtualPhenix.Example.Localization
{
    [
     DefaultExecutionOrder(VP_ExecutingOrderSetup.LOCALIZATION_MANAGER),
     AddComponentMenu(VP_PrefixSetup.MAIN_PREFIX + "/Example/Example Localization Manager")
    ]
    public class VP_ExampleLocalizationManager : VP_LocalizationManager
    {
        [Header("Example Localization Manager")]
        [SerializeField] private TMPro.TMP_Text m_text;
        [SerializeField] private string[] m_textParameters;

        private bool m_Sp = true;

        protected override void StartAllListeners()
        {
            base.StartAllListeners();

            VP_EventManager.StartListening<SystemLanguage>(VP_EventSetup.Localization.TRANSLATE_TEXTS, NewLanguage);
        }

        protected override void StopAllListeners()
        {
            base.StopAllListeners();

            if (!VP_MonobehaviourSettings.Quiting)
                VP_EventManager.StopListening<SystemLanguage>(VP_EventSetup.Localization.TRANSLATE_TEXTS, NewLanguage);
        }

        public void TranslateText()
        {
            m_text.text = GetTextTranslatedINTL(m_text.text, m_textParameters);
        }

        void NewLanguage(SystemLanguage _newLang)
        {
            TranslateText();
        }

        public void SwapLang()
        {
            m_Sp = !m_Sp;
            ChangeLanguage(m_Sp ? SystemLanguage.Spanish : SystemLanguage.English);
        }
    }

}
