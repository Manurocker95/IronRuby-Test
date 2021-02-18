#if USE_PERMISSIONS && USE_MICROPHONE
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Permissions
{
    public class VP_MicrophonePermission : VP_Permission
    {
        public override string PermissionName
        {
            get
            {
#if UNITY_ANDROID
                return UnityEngine.Android.Permission.Microphone;
#else
                return VP_PermissionSetup.MICROPHONE_PERMISSION_NAME;
#endif

            }
        }

       
        public override UserAuthorization UserAuthorization
        {
            get
            {
                return UserAuthorization.Microphone;
            }
        }
    }
}
#endif