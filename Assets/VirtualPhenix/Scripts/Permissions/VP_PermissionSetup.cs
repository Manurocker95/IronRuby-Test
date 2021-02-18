#if USE_PERMISSIONS
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Permissions
{
    public static class VP_PermissionSetup
    {
        public const string MICROPHONE_PERMISSION_NAME = "android.permission.RECORD_AUDIO";
        public const string CAMERA_PERMISSION_NAME = "android.permission.CAMERA";
    }
}
#endif