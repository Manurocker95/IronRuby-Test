#if USE_PERMISSIONS
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Permissions
{
    [System.Serializable]
    public class VP_Permission
    {
        [SerializeField] protected bool m_initAutomatically = true;

        public virtual bool AutoInit
        {
            get
            {
                return m_initAutomatically;
            }
        }

        public virtual string PermissionName
        {
            get
            {
                return "";
            }
        }

        public virtual UserAuthorization UserAuthorization
        {
            get
            {
                return UserAuthorization.Microphone;
            }
        }

        public virtual void AskForPermission(UnityEngine.Events.UnityAction _callback = null)
        {
#if UNITY_ANDROID
            UnityEngine.Android.Permission.RequestUserPermission(PermissionName);
#endif

            if (_callback != null)
                _callback.Invoke();
        }

        public virtual bool HasPermission()
        {
#if UNITY_ANDROID
            return UnityEngine.Android.Permission.HasUserAuthorizedPermission(PermissionName);
#else
                return Application.HasUserAuthorization(UserAuthorization);
#endif
        }
    }
}
#endif