#if EASY_MOBILE
using EasyMobile;
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Privacy
{
    [System.Serializable]
    public class VP_AppConsent
    {
        #region 3rd-party Services Consent

#if EASY_MOBILE
        // The consent for the whole Advertising module.
        // (we could have had different consents for individual ad networks, but for
        // the sake of simplicity in this demo, we'll ask the user a single consent
        // for the whole module and use it for all ad networks).
        public ConsentStatus m_advertisingConsent = ConsentStatus.Unknown;

        public ConsentStatus m_ageConsent = ConsentStatus.Unknown;

        public ConsentStatus m_analyticsConsent = ConsentStatus.Unknown;

        // The consent for the whole Notifications module.
        // Note that data consent is only applicable to push notifications,
        // local notifications don't require any consent.
        public ConsentStatus m_notificationConsent = ConsentStatus.Unknown;

#endif
        /// <summary>
        /// To JSON string.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="EasyMobile.Demo.AppConsent"/>.</returns>
        public override string ToString()
        {
            return JsonUtility.ToJson(this);
        }

        #endregion
    }

}
