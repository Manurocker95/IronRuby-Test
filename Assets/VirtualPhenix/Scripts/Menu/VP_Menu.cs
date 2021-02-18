using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VirtualPhenix.Settings;

namespace VirtualPhenix
{
    [DefaultExecutionOrder(VP_ExecutingOrderSetup.MENU), AddComponentMenu(VP_PrefixSetup.MAIN_PREFIX + "/Menu/Menu")]
    public class VP_Menu : VP_Monobehaviour
    {
        [Header("Menu Properties"), Space]
        [SerializeField] protected bool m_canInteract = false;

        [Header("Main Group"), Space]
        [SerializeField] protected bool m_useUI = true;
        [SerializeField] protected GameObject m_mainGroup;
        [SerializeField] protected Button[] m_buttons;
        [SerializeField] protected int m_currentButtonIndex = 0;

        [Header("Settings Group"), Space]
        [SerializeField] protected GameObject m_settingsGroup;

	    protected virtual void Reset()
        {
	        m_initializationTime = InitializationTime.OnAwake;
	        m_startListeningTime = StartListenTime.OnAwake;
	        m_stopListeningTime = StopListenTime.OnDestroy;
        }

	    protected override void Initialize()
	    {
	    	base.Initialize();
	    	
	    	if (m_useUI)
	    	{
		    	if (m_mainGroup)
			    	m_mainGroup.SetActive(true);
			    
		    	if (m_settingsGroup)
			    	m_settingsGroup.SetActive(false);
	    	}

	    }

        // Update is called once per frame
        protected virtual void Update()
        {

        }

        public virtual void PlaySoundButton()
        {
            VP_AudioManager.PlayAudioItemOneShotbyKey(VP_AudioSetup.UI.CONFIRM);
        }

        public virtual void Play()
        {

        }       
        
        public virtual void InteractWithButton(int _index)
        {

        }

        public virtual void HideMainGroup(bool _force = false)
        {
	        if (m_useUI && m_mainGroup)
                 m_mainGroup.SetActive(false);
        }

        public virtual void ShowMainGroup(bool _force = false)
	    {
		    if (m_useUI)
		    {
			    if (m_mainGroup)
				    m_mainGroup.SetActive(true);
			    if (m_settingsGroup)
				    m_settingsGroup.SetActive(false);
		    }

        }

        public virtual void BackFromSettings()
        {

        }

        public virtual void ShowSettings(bool _force = false)
	    {
		    if (m_useUI)
		    {
			    if (!m_canInteract && !_force)
				    return;
			    if (m_mainGroup)
				    m_mainGroup.SetActive(false);
			    if (m_settingsGroup)
				    m_settingsGroup.SetActive(true);
		    }

        }

        public virtual void TranslateTexts()
        {
            
        }

        protected override void StartAllListeners()
        {
            base.StartAllListeners();

            VP_EventManager.StartListening(VP_EventSetup.Localization.TRANSLATE_TEXTS, TranslateTexts);
        }

        protected override void StopAllListeners()
        {
            base.StopAllListeners();

            VP_EventManager.StopListening(VP_EventSetup.Localization.TRANSLATE_TEXTS, TranslateTexts);
        }

        public virtual void ShowRewardUI(int reward)
        {

        }


        public virtual void WatchRewardedAd()
        {

        }    
        
        protected virtual void OnCompleteRewardedAdToEarnCoins()
        {

        }

        protected virtual void ShowWatchForCoinsBtn()
        {

        }
    }

}
