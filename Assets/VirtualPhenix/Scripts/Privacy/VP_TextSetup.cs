using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix
{
    public static partial class VP_TextSetup
    {
        public static class Privacy
        {
            public const string PRE_DISCLAIMER_TEXT = "Predisclaimer";


            public const string DISCLAIMER_TEXT = "Disclaimer";

            public const string ADS_TOGGLE_TITLE = "Advertising";
   
            public const string ADS_TOGGLE_ON_DESCRIPTION = "AdsToggleOn";


            public const string ADS_TOGGLE_OFF_DESCRIPTION = "AdsToggleOff";

            public const string NOTIFICATIONS_TOGGLE_TITLE = "Notifications";

            public const string NOTIFICATIONS_TOGGLE_ON_DESCRIPTION = "NotificationsToggleOn";


            public const string NOTIFICATIONS_TOGGLE_OFF_DESCRIPTION = "NotificationsToggleOff";



             public const string AGE_TOGGLE_TITLE = "Accept Privacy Policy";


            public const string AGE_TOGGLE_ON_DESCRIPTION = "AgeToggleOnDesc";

            public const string AGE_TOGGLE_OFF_DESCRIPTION = "AgeToggleOffDesc";




            // The title of the toggle for Unity Analytics consent in English
            // Note that this toggle is On by default and cannot be changed by the user, because we can't opt-out
            // Unity Analytics locally.
            // Instead we use the Unity Data Privacy Plugin to fetch an opt-out URL and present it to the user.
            // https://assetstore.unity.com/packages/add-ons/services/unity-data-privacy-plug-in-118922
            public const string ANALYTICS_TOGGLE_TITLE = "Analytics*";

            // The common description (both On & Off states) of the toggle for Unity Analytics consent in English
            public const string ANALYTICS_TOGGLE_ON_DESCRIPTION = "AnalyticsText";

            // The description of the toggle for Unity Analytics consent that is used if the opt-out URL can't be fetched, in English.
            public const string ANALYTICS_TOGGLE_OFF_DESCRIPTION = "AnalyticsText";

            public const string PRIVACY_TITLE = "Data Privacy Consent";

            // The second paragraph of the dialog content in English
            public const string ACCEPT_TITLE = "ClickToConsent";


            public const string ACCEPT = "Accept";


        }
    }
}
