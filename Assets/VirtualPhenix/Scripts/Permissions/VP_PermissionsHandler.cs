#if USE_PERMISSIONS
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Permissions
{
    [
        DefaultExecutionOrder(VP_ExecutingOrderSetup.PERMISSION_HANDLER),
        AddComponentMenu(VP_PrefixSetup.MAIN_PREFIX+"/Permissions/VP Permission Handler")
    ]
    public class VP_PermissionsHandler : VP_Monobehaviour
    {
#if  USE_MICROPHONE
        [Header("Microphone Permission"), Space]
        [SerializeField] protected VP_MicrophonePermission m_microphonePermission;
        [SerializeField] protected UnityEngine.Events.UnityEvent m_onMicRequest = new UnityEngine.Events.UnityEvent();
#endif
        protected override void Initialize()
        {
            base.Initialize();
#if USE_MICROPHONE
            if (m_microphonePermission == null)
                m_microphonePermission = new VP_MicrophonePermission();

            if (m_microphonePermission.AutoInit && !m_microphonePermission.HasPermission())
            {
                AskMicrophonePermission();
             
            }
#endif
        }

        public virtual void AskMicrophonePermission()
        {
#if USE_MICROPHONE
            m_microphonePermission?.AskForPermission(m_onMicRequest.Invoke);
#endif
        }

        public virtual bool HasMicrophonePermission()
        {
#if USE_MICROPHONE
            return m_microphonePermission.HasPermission();
#else
            return false;
#endif
        }
    }
}
#endif