using UnityEngine;
using System.Collections;

namespace VirtualPhenix
{
    public static partial class VP_AppInfo 
    {
        // App-specific metadata
        public const string APP_NAME = "Coco Flip";

        public const string CUSTOM_WEB_LINK = "https://virtualphenix.com/";

        public const string APPSTORE_ID = "[YOUR_APP_ID]";

        public const string UNITY_ADS_PLAYSTORE_ID = "3757417";
        // App Store id

        public const string BUNDLE_ID = "[YOUR_BUNDLE_ID]";
        // app bundle id

        public const string APPSTORE_LINK = "itms-apps://itunes.apple.com/app/id";
        // App Store link

        public const string PLAYSTORE_LINK = "market://details?id=";
        // Google Play store link

        public const string APPSTORE_SHARE_LINK = "https://itunes.apple.com/app/id";
        // App Store link

        public const string PLAYSTORE_SHARE_LINK = "https://play.google.com/store/apps/details?id=com.VirtualPhenix.CocoFlip";
        // Google Play store link

        // Publisher links
        public const string APPSTORE_HOMEPAGE = "[YOUR_APPSTORE_PUBLISHER_LINK]";
        // e.g itms-apps://itunes.apple.com/artist/[publisher-name]/[publisher-id]

        public const string PLAYSTORE_HOMEPAGE = "[YOUR_GOOGLEPLAY_PUBLISHER_NAME]";
        // e.g https://play.google.com/store/apps/developer?id=[PUBLISHER_NAME]

        public const string FACEBOOK_ID = "[YOUR_FACEBOOK_PAGE_ID]";

        public const string TWITTER_NAME = "[YOUR_TWITTER_PAGE_NAME]";

        public const string SUPPORT_EMAIL = "virtualphenixgames@gmail.com";

        public const string FACEBOOK_LINK = "https://facebook.com/";

        public const string TWITTER_LINK = "https://twitter.com/VirtualPhenix";
        
        //Unity ads for test mode (in editor)
        public const bool ADS_TEST_MODE = false;

        // 1 minute waiting for 
        public const float MAX_WAIT_FOR_AD = 60;

        public const string VIRTUAL_PHENIX_POLICY_URL = "https://virtualphenix.com/privacy/";

        public const string UNITY_ADS_POLICY_URL = "https://unity3d.com/legal/privacy-policy";

        // The opt-out URL for Unity Analytics must be fetched at runtime, so we use a placeholder
        // which we will replace with the actual URL once we fetched it.
        // https://assetstore.unity.com/packages/add-ons/services/unity-data-privacy-plug-in-118922
         
        public const string UNITY_ANALYTICS_OPTOUTPUT_URL = "https://virtualphenix.com/privacy/cocoflip";



    }

}