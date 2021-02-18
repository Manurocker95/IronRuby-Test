using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix
{
    public static partial class VP_Utils
    {
        public static class Mobile
        {
            public static void RateApp()
            {
                switch (Application.platform)
                {
                    case RuntimePlatform.IPhonePlayer:
                        Application.OpenURL(VP_AppInfo.APPSTORE_LINK);
                        break;

                    case RuntimePlatform.Android:
                        Application.OpenURL(VP_AppInfo.PLAYSTORE_LINK);
                        break;
                }
            }


            public static void OpenFacebookPage()
            {
                Application.OpenURL(VP_AppInfo.FACEBOOK_LINK);
            }

            public static void OpenCustomPage()
            {
                Application.OpenURL(VP_AppInfo.CUSTOM_WEB_LINK);
            }

            public static void OpenTwitterPage()
            {
                Application.OpenURL(VP_AppInfo.TWITTER_LINK);
            }

            public static void ContactUs()
            {
                string email = VP_AppInfo.SUPPORT_EMAIL;
                string subject = EscapeURL(VP_AppInfo.APP_NAME + " [" + Application.version + "] Support");
                string body = EscapeURL("");
                Application.OpenURL("mailto:" + email + "?subject=" + subject + "&body=" + body);
            }

            public static string EscapeURL(string url)
            {
                return UnityEngine.Networking.UnityWebRequest.EscapeURL(url).Replace("+", "%20");
            }

        }
    }

}
