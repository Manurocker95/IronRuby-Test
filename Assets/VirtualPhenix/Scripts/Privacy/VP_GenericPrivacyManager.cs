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
    [DefaultExecutionOrder(VP_ExecutingOrderSetup.PRIVACY_MANAGER),AddComponentMenu("")]
    public class VP_GenericPrivacyManager<T> : VP_PrivacyManagerBase where T : VP_AppConsent
    {
        [Header("Privacy"), Space]
        [SerializeField] protected T m_appConsent;

        protected override void Initialize()
        {
            base.Initialize();

#if (!UNITY_ANDROID && !UNITY_IOS && !UNITY_EDITOR)
            this.gameObject.SetActive(false);
#else

#if UNITY_IOS
            if (!m_createConsentDialogue)
            {
                m_onConsentAccepted.Invoke();
            }
#else
            try
            {
#if EASY_MOBILE
              
                // Checks if we're in EEA region.
                EasyMobile.Privacy.IsInEEARegion(result =>
                {
                    m_isInEEARegion = result == EEARegionStatus.InEEA;
                });
#else
                m_isInEEARegion = false;
#endif

                // First check if there's any consent saved previously.
                // If there is, we will set the 'isOn' state of our toggles
                // according to the saved consent to reflect the current consent
                // status on the consent dialog.
                m_appConsent = LoadAppConsent();

                if (m_appConsent == null)
                {
                    ShowConsentDialog();
                }
                else
                {
                    m_onConsentAccepted.Invoke();
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("Something in App consent crashed!");
                Debug.LogError("Error: " + e.Message + " with trace:" + e.StackTrace);
                //Save(m_consentStorageKey, null);
                m_onConsentAccepted.Invoke();
            }

#endif

#endif
        }

#if EASY_MOBILE
        protected override void DialogCompleted(ConsentDialog dialog, ConsentDialog.CompletedResults results)
        {
            VP_Debug.Log("Demo consent dialog completed with button ID: " + results.buttonId);

            // Construct the new consent.
            if (m_appConsent == null)
                m_appConsent = ResetAppConsent();

            if (results.toggleValues != null)
            {
                VP_Debug.Log("Consent toggles:");
                foreach (KeyValuePair<string, bool> t in results.toggleValues)
                {
                    string toggleId = t.Key;
                    bool toggleValue = t.Value;
                    VP_Debug.Log("Toggle ID: " + toggleId + "; Value: " + toggleValue);

                    if (toggleId == m_adsToggleId)
                    {
                        // Whether the Advertising module is given consent.
                        m_appConsent.m_advertisingConsent = toggleValue ? ConsentStatus.Granted : ConsentStatus.Revoked;
                    }
                    else if (toggleId == m_notifsToggleId)
                    {
                        // Whether the Notifications module is given consent.
                        m_appConsent.m_notificationConsent = toggleValue ? ConsentStatus.Granted : ConsentStatus.Revoked;
                    }
                    else if (toggleId == m_unityAnalyticsToggleId)
                    {
                        m_appConsent.m_analyticsConsent = toggleValue ? ConsentStatus.Granted : ConsentStatus.Revoked;
#if USE_MONETIZATION
                        Analytics.enabled = m_appConsent.m_analyticsConsent == ConsentStatus.Granted;
#endif
                        // We don't store the UnityAnalytics consent ourselves as it is managed
                        // by the Unity Data Privacy plugin.
                    }
                    else if (toggleId == m_ageToggleId)
                    {
                        m_appConsent.m_ageConsent = toggleValue ? ConsentStatus.Granted : ConsentStatus.Revoked;
                    }
                    else
                    {
                        // Unrecognized toggle ID.
                    }
                }
            }

            VP_Debug.Log("Now forwarding new consent to relevant modules and then store it...");

            // Forward the consent to relevant modules.
            ApplyAppConsent(m_appConsent);

            // Store the new consent.
            Save(m_consentStorageKey, m_appConsent);

            // Here you can forward the consent to other relevant 3rd-party services if needed...
            m_onConsentAccepted.Invoke();
        }

        protected override void DialogToggleStateUpdated(ConsentDialog dialog, string toggleId, bool isOn)
        {
            VP_Debug.Log("ToggleStateUpdated. ID: " + toggleId + "; new value: " + isOn);

            if (m_appConsent.m_ageConsent == ConsentStatus.Granted && 
                m_appConsent.m_advertisingConsent == ConsentStatus.Granted && 
                m_appConsent.m_analyticsConsent == ConsentStatus.Granted && 
                m_appConsent.m_notificationConsent == ConsentStatus.Granted)
            {
                dialog.SetButtonInteractable(m_acceptButtonId, isOn);
            }
        }

        public override void CreateAccept(VP_LocalizationManager lm, ref ConsentDialog dialog)
        {
            dialog.AppendText(lm.GetTextTranslated(VP_TextSetup.Privacy.ACCEPT_TITLE));

            // Build and append the accept button.
            // A consent dialog should always have at least one button!
            ConsentDialog.Button okButton = new ConsentDialog.Button(m_acceptButtonId);
            okButton.Title = lm.GetTextTranslated(VP_TextSetup.Privacy.ACCEPT);
            okButton.TitleColor = Color.white;
            okButton.BodyColor = new Color(66 / 255f, 179 / 255f, 1);
            okButton.IsInteractable = m_appConsent != null ? (m_appConsent.m_ageConsent == ConsentStatus.Granted) : false;
            dialog.AppendButton(okButton);
        }

        public override void CreateAgeToggle(VP_LocalizationManager lm, ref ConsentDialog dialog)
        {
            ConsentDialog.Toggle ageToggle = new ConsentDialog.Toggle(m_ageToggleId);

            ageToggle.Title = lm.GetTextTranslated(VP_TextSetup.Privacy.AGE_TOGGLE_TITLE);
            ageToggle.OnDescription = lm.GetTextTranslated(VP_TextSetup.Privacy.AGE_TOGGLE_ON_DESCRIPTION);
            ageToggle.OffDescription = lm.GetTextTranslated(VP_TextSetup.Privacy.AGE_TOGGLE_OFF_DESCRIPTION);
            ageToggle.ShouldToggleDescription = true;   // make the description change with the toggle state.
            ageToggle.IsOn = m_appConsent != null ? m_appConsent.m_ageConsent == ConsentStatus.Granted : false;     // reflect previous ads consent if any
            dialog.AppendToggle(ageToggle);
        }

        public override void CreateAdsToggle(VP_LocalizationManager lm, ref ConsentDialog dialog)
        {

            ConsentDialog.Toggle adsToggle = new ConsentDialog.Toggle(m_adsToggleId);
        
            adsToggle.Title = lm.GetTextTranslated(VP_TextSetup.Privacy.ADS_TOGGLE_TITLE);
            adsToggle.OnDescription = lm.GetTextTranslatedINTL(VP_TextSetup.Privacy.ADS_TOGGLE_ON_DESCRIPTION, "", VP_AppInfo.VIRTUAL_PHENIX_POLICY_URL, VP_AppInfo.UNITY_ADS_POLICY_URL);
            adsToggle.OffDescription = lm.GetTextTranslatedINTL(VP_TextSetup.Privacy.ADS_TOGGLE_OFF_DESCRIPTION, "", VP_AppInfo.VIRTUAL_PHENIX_POLICY_URL, VP_AppInfo.UNITY_ADS_POLICY_URL);

            adsToggle.ShouldToggleDescription = true;   // make the description change with the toggle state.
            adsToggle.IsOn = m_appConsent != null ? m_appConsent.m_advertisingConsent == ConsentStatus.Granted : false;     // reflect previous ads consent if any
            dialog.AppendToggle(adsToggle);
        }

        public override void CreateNotificationToggle(VP_LocalizationManager lm, ref ConsentDialog dialog)
        {
            
            ConsentDialog.Toggle notificationToggle = new ConsentDialog.Toggle(m_notifsToggleId);

            notificationToggle.Title = lm.GetTextTranslated(VP_TextSetup.Privacy.NOTIFICATIONS_TOGGLE_TITLE);
            notificationToggle.OnDescription = lm.GetTextTranslated(VP_TextSetup.Privacy.NOTIFICATIONS_TOGGLE_ON_DESCRIPTION);
            notificationToggle.OffDescription = lm.GetTextTranslated(VP_TextSetup.Privacy.NOTIFICATIONS_TOGGLE_OFF_DESCRIPTION);

            notificationToggle.ShouldToggleDescription = true;   // make the description change with the toggle state.
            notificationToggle.IsOn = m_appConsent != null ? m_appConsent.m_notificationConsent == ConsentStatus.Granted : false;     // reflect previous ads consent if any
            dialog.AppendToggle(notificationToggle);
            
        }

        public override void CreateAnalyticsToggle(VP_LocalizationManager lm, ref ConsentDialog dialog)
        {
            
            ConsentDialog.Toggle analyticsToggle = new ConsentDialog.Toggle(m_unityAnalyticsToggleId);

            analyticsToggle.Title = lm.GetTextTranslated(VP_TextSetup.Privacy.ANALYTICS_TOGGLE_TITLE);
            analyticsToggle.OnDescription = lm.GetTextTranslatedINTL(VP_TextSetup.Privacy.ANALYTICS_TOGGLE_ON_DESCRIPTION, "", VP_AppInfo.UNITY_ANALYTICS_OPTOUTPUT_URL);
            analyticsToggle.OffDescription = lm.GetTextTranslated(VP_TextSetup.Privacy.ANALYTICS_TOGGLE_OFF_DESCRIPTION);

            analyticsToggle.ShouldToggleDescription = true;   // make the description change with the toggle state.
            analyticsToggle.IsOn = m_appConsent != null ? m_appConsent.m_analyticsConsent == ConsentStatus.Granted : false;     // reflect previous ads consent if any
            dialog.AppendToggle(analyticsToggle);
            
        }

        /// <summary>
        /// Forwards the consent to relevant modules of EM.
        /// </summary>
        /// <param name="consent">Consent.</param>
        /// <remarks>
        /// In a real-world app, you'd want to write similar method
        /// to forward the obtained consent not only to relevant EM modules
        /// and services, but also to other relevant 3rd-party SDKs in your app.
        public virtual void ApplyAppConsent(T consent)
        {
            // Forward the consent to the Advertising module.
            if (consent.m_advertisingConsent == ConsentStatus.Granted)
                Advertising.GrantDataPrivacyConsent();
            else if (consent.m_advertisingConsent == ConsentStatus.Revoked)
                Advertising.RevokeDataPrivacyConsent();

            // Forward the consent to the Notifications module.
            if (consent.m_notificationConsent == ConsentStatus.Granted)
                Notifications.GrantDataPrivacyConsent();
            else if (consent.m_notificationConsent == ConsentStatus.Revoked)
                Notifications.RevokeDataPrivacyConsent();

        }

#endif
        /// <summary>
        /// Converts this object to JSON and stores in PlayerPrefs with the provided key.
        /// </summary>
        /// <param name="key">Key.</param>
        public virtual void Save(string key, T consent)
        {
            string json = "";
            if (key.IsNullOrEmpty())
                key = "VP_AppConsent";


            switch (m_formatter)
            {
                case FORMATTER_METHOD.JSONUTILITY_RESOLVER:
                    json = JsonUtility.ToJson(consent);
                    break;
                case FORMATTER_METHOD.COMPLEX_FORMATTER:
                    json = Serialization.VP_ComplexFormatter.SerializeToJSON<T>(consent);
                    break;
#if ODIN_INSPECTOR
                case FORMATTER_METHOD.ADDRESSABLE_RESOLVER:
                    var bytes = VP_Formatter.GetBytesWithAddressableContext(consent, m_dataFormat);
                    json = Encoding.UTF8.GetString(bytes);
                    break;
                case FORMATTER_METHOD.DATABASE_RESOLVER:
                    bytes = VP_Formatter.GetBytesWithDatabaseContext(consent, m_dataFormat);
                    json = Encoding.UTF8.GetString(bytes);
                    break;
                case FORMATTER_METHOD.GUID_REFERENCE_RESOLVER:
                    bytes = VP_Formatter.GetBytesWithGUIDContext(consent, m_dataFormat);
                    json = Encoding.UTF8.GetString(bytes);
                    break;
                case FORMATTER_METHOD.INDEX_REFERENCE_RESOLVER:
                    List<Object> ob = new List<Object>();
                    bytes = VP_Formatter.GetBytesWithIndexContext(consent, out ob, m_dataFormat);
                    json = Encoding.UTF8.GetString(bytes);
                    break;
                case FORMATTER_METHOD.STRING_REFERENCE_RESOLVER:
                    bytes = VP_Formatter.GetBytesWithStringContext(consent, m_dataFormat);
                    json = Encoding.UTF8.GetString(bytes);
                    break;
                case FORMATTER_METHOD.NO_CONTEXT:
                    bytes = VP_Formatter.GetBytesWithoutContext(consent, m_dataFormat);
                    json = Encoding.UTF8.GetString(bytes);
                    break;
#endif
                default:
                    json = JsonUtility.ToJson(consent);
                    break;
            }

            if (json.IsNullOrEmpty())
            {
                json = JsonUtility.ToJson(ResetAppConsent());
            }

            PlayerPrefs.SetString(key, json);
        }

        /// <summary>
        /// Saves the give app consent to PlayerPrefs as JSON using the demo storage key.
        /// </summary>
        /// <param name="consent">Consent.</param>
        public virtual void SaveDemoAppConsent(T consent)
        {
            if (consent != null)
                Save(m_consentStorageKey, consent);
        }

        /// <summary>
        /// Loads the demo app consent from PlayerPrefs, returns null if nothing stored previously.
        /// </summary>
        /// <returns>The demo app consent.</returns>
        public virtual T LoadAppConsent()
        {
            if (m_consentStorageKey.IsNullOrEmpty())
                m_consentStorageKey = "VP_AppConsent";

            string json = PlayerPrefs.GetString(m_consentStorageKey, "");

            if (!string.IsNullOrEmpty(json))
            {
                switch (m_formatter)
                {
                    case FORMATTER_METHOD.JSONUTILITY_RESOLVER:
                        return JsonUtility.FromJson<T>(json);
                    case FORMATTER_METHOD.COMPLEX_FORMATTER:
                        return Serialization.VP_ComplexFormatter.DeserializeFromJSON<T>(json);
#if ODIN_INSPECTOR
                    case FORMATTER_METHOD.ADDRESSABLE_RESOLVER:
                        return VP_Formatter.LoadWithAddressableContext<T>(Encoding.UTF8.GetBytes(json), m_dataFormat);
                    case FORMATTER_METHOD.DATABASE_RESOLVER:
                        return VP_Formatter.LoadWithDataBaseContext<T>(Encoding.UTF8.GetBytes(json), m_dataFormat);
                    case FORMATTER_METHOD.GUID_REFERENCE_RESOLVER:
                        return VP_Formatter.LoadWithGUIDContext<T>(Encoding.UTF8.GetBytes(json), m_dataFormat);
                    case FORMATTER_METHOD.INDEX_REFERENCE_RESOLVER:
                        List<Object> ob = new List<Object>();
                        return VP_Formatter.LoadWithIndexContext<T>(Encoding.UTF8.GetBytes(json), ob, m_dataFormat);
                    case FORMATTER_METHOD.STRING_REFERENCE_RESOLVER:
                        return VP_Formatter.LoadWithStringContext<T>(Encoding.UTF8.GetBytes(json), m_dataFormat);
                    case FORMATTER_METHOD.NO_CONTEXT:
                        return VP_Formatter.LoadWithNoContext<T>(Encoding.UTF8.GetBytes(json), m_dataFormat);
#endif
                    default:
                        return JsonUtility.FromJson<T>(json);
                }
            }
            else
            {
                return null;
            }
        }

        public virtual T ResetAppConsent()
        {
            return default(T);
        }
    }
}
