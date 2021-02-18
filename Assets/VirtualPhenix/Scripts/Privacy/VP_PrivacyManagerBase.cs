using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix.Localization;
using UnityEngine.Events;

#if USE_MONETIZATION
using UnityEngine.Analytics;
#endif

using System.Text;

#if EASY_MOBILE
using EasyMobile;
#endif

using VirtualPhenix.Formatter;

namespace VirtualPhenix.Privacy
{
	[DefaultExecutionOrder(VP_ExecutingOrderSetup.PRIVACY_MANAGER), AddComponentMenu("")]
    public class VP_PrivacyManagerBase : VP_SingletonMonobehaviour<VP_PrivacyManagerBase>
    {

        [Header("Formatting"),Space]
        [SerializeField] protected FORMATTER_METHOD m_formatter = FORMATTER_METHOD.JSONUTILITY_RESOLVER;
#if ODIN_INSPECTOR
        [SerializeField] protected Sirenix.Serialization.DataFormat m_dataFormat = Sirenix.Serialization.DataFormat.Binary;
#endif

        [Header("Privacy"), Space]
        [SerializeField] protected bool m_isInEEARegion = false;
        [SerializeField] protected bool m_createConsentDialogue = true;
        [SerializeField] protected bool m_isConsentDissmisible = true;
        [SerializeField] protected string m_adsToggleId = "em-demo-consent-toggle-ads";
        [SerializeField] protected string m_ageToggleId = "age-consent-toggle-ads";
        [SerializeField] protected string m_notifsToggleId = "em-demo-consent-toggle-notifs";
        [SerializeField] protected string m_unityAnalyticsToggleId = "em-demo-consent-toggle-unity-analytics";
        [SerializeField] protected string m_acceptButtonId = "em-demo-consent-button-ok";

        [SerializeField] protected string m_consentStorageKey = "VP_AppConsent";

#if EASY_MOBILE
        protected ConsentDialog m_consentDialog;
#endif
        [SerializeField] protected UnityEvent m_onConsentAccepted;

        protected override void StopAllListeners()
        {
            base.StopAllListeners();
#if EASY_MOBILE
            UnSubscribeConsentDialogEvents(ref m_consentDialog);
#endif
        }

        public virtual void ShowConsentDialog()
        {
  #if EASY_MOBILE
            if (m_consentDialog == null)
            {
                m_consentDialog = CreateConsentDialog();

                SubscribeConsentDialogEvents(ref m_consentDialog);
            }

            if (m_consentDialog != null)
                m_consentDialog.Show(m_isConsentDissmisible);
#endif
        }

#if EASY_MOBILE
        protected virtual void SubscribeConsentDialogEvents(ref ConsentDialog dialog)
        {
            if (dialog == null)
                return;

            dialog.Dismissed += DialogDismissed;
            dialog.Completed += DialogCompleted;
            dialog.ToggleStateUpdated += DialogToggleStateUpdated;
        }

        protected virtual void UnSubscribeConsentDialogEvents(ref ConsentDialog dialog)
        {
            if (dialog == null)
                return;

            dialog.Dismissed -= DialogDismissed;
            dialog.Completed -= DialogCompleted;
            dialog.ToggleStateUpdated -= DialogToggleStateUpdated;
        }

        protected virtual void DialogDismissed(ConsentDialog dialog)
        {
            VP_Debug.Log("Demo consent dialog was dismissed.");
            Application.Quit();
        }

        protected virtual void DialogCompleted(ConsentDialog dialog, ConsentDialog.CompletedResults results)
        {
           
        }

        protected virtual void DialogToggleStateUpdated(ConsentDialog dialog, string toggleId, bool isOn)
        {
          
        }

        public virtual ConsentDialog CreateConsentDialog()
        {
            var lm = VP_LocalizationManager.Instance;

            // First create a new consent dialog.
            ConsentDialog dialog = new ConsentDialog();

            CreateTitle(lm, ref dialog);
            CreateDisclaimerText(lm, ref dialog);

#if USE_MONETIZATION
            CreateAdsToggle(lm, ref dialog);
#endif

#if USE_NOTIFICATIONS
            CreateNotificationToggle(lm, ref dialog);
#endif

#if USE_MONETIZATION
            CreateAnalyticsToggle(lm, ref dialog);
#endif
            CreateAgeToggle(lm, ref dialog);

            CreateAccept(lm, ref dialog);

            return dialog;
        }

        public virtual void CreateAccept(VP_LocalizationManager lm, ref ConsentDialog dialog)
        {

        }

        public virtual void CreateTitle(VP_LocalizationManager lm, ref ConsentDialog dialog)
        {
            dialog.Title = lm.GetTextTranslated(VP_TextSetup.Privacy.PRIVACY_TITLE);
        }

        public virtual void CreateDisclaimerText(VP_LocalizationManager lm, ref ConsentDialog dialog)
        {
            dialog.AppendText(lm.GetTextTranslatedINTL(VP_TextSetup.Privacy.PRE_DISCLAIMER_TEXT, "", Application.productName));
            dialog.AppendText(lm.GetTextTranslatedINTL(VP_TextSetup.Privacy.DISCLAIMER_TEXT, "", VP_AppInfo.VIRTUAL_PHENIX_POLICY_URL));
        }


        public virtual void CreateAgeToggle(VP_LocalizationManager lm, ref ConsentDialog dialog)
        {

        }

        public virtual void CreateAdsToggle(VP_LocalizationManager lm, ref ConsentDialog dialog)
        {

     
        }

        public virtual void CreateNotificationToggle(VP_LocalizationManager lm, ref ConsentDialog dialog)
        {

            
        }

        public virtual void CreateAnalyticsToggle(VP_LocalizationManager lm, ref ConsentDialog dialog)
        {
            
            
        }

#endif
      
    }
}
