#if USE_PERMISSIONS
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Permissions
{
    public class VP_WebcamPermissions : VP_Permission
    {
        public override string PermissionName
        {
            get
            {
#if UNITY_ANDROID
                return UnityEngine.Android.Permission.Camera;
#else
                return VP_PermissionSetup.CAMERA_PERMISSION_NAME;
#endif

            }
        }

        public override UserAuthorization UserAuthorization
        {
            get
            {
                return UserAuthorization.WebCam;
            }
        }

    }
}
#endif
