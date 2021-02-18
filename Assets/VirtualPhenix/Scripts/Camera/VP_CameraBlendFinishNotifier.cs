using UnityEngine;
#if USE_CINEMACHINE
using Cinemachine;
#endif
using UnityEngine.Events;
using System;

namespace VirtualPhenix
{
    [AddComponentMenu(VP_PrefixSetup.MAIN_PREFIX + "/Camera/CameraBlend Finish Notifier")]
    public class VP_CameraBlendFinishNotifier : VP_MonoBehaviour
    {
#if USE_CINEMACHINE
        [SerializeField] CinemachineVirtualCameraBase vcamBase;
        [SerializeField] CinemachineBrain m_brain;

        [Serializable] public class BlendFinishedEvent : UnityEvent<CinemachineVirtualCameraBase> { }
        [Serializable] public class BlendInitEvent : UnityEvent<ICinemachineCamera, ICinemachineCamera> { }
        [Serializable] public class GenericBlendFinishedEvent : UnityEvent { }

        public BlendFinishedEvent OnBlendFinished;
        public BlendInitEvent OnBlendStart;
        public GenericBlendFinishedEvent OnGenericBlendFinished;
        public GenericBlendFinishedEvent OnGenericBlendStart;

        public bool m_disableOnStart = true;
        public bool m_logs = true;



        protected override void Awake()
        {
            base.Awake();

            if (!vcamBase)
                vcamBase = GetComponent<CinemachineVirtualCameraBase>();
        }

        protected override void Start()
        {
            base.Start();

            ConnectToVcam(true);

            if (m_disableOnStart)
                enabled = false;
        }

        void ConnectToVcam(bool connect)
        {
            m_brain = CinemachineCore.Instance.FindPotentialTargetBrain(vcamBase);
            var vcam = vcamBase as CinemachineVirtualCamera;
            if (vcam != null)
            {
                vcam.m_Transitions.m_OnCameraLive.RemoveListener(OnCameraLive);
                if (connect)
                    vcam.m_Transitions.m_OnCameraLive.AddListener(OnCameraLive);
            }
            var freeLook = vcamBase as CinemachineFreeLook;
            if (freeLook != null)
            {
                freeLook.m_Transitions.m_OnCameraLive.RemoveListener(OnCameraLive);
                if (connect)
                    freeLook.m_Transitions.m_OnCameraLive.AddListener(OnCameraLive);
            }
        }

        void OnCameraLive(ICinemachineCamera vcamIn, ICinemachineCamera vcamOut)
        {
            if (vcamIn != null && vcamOut != null)
            {
                if (m_logs)
                    VP_Debug.Log("On Camera live with In: " + vcamIn.VirtualCameraGameObject.name + " and out: " + vcamOut.VirtualCameraGameObject.name);
                enabled = true;
                OnBlendStart.Invoke(vcamIn, vcamOut);
                OnGenericBlendStart.Invoke();
            }
        }

        void Update()
        {
            if (!m_brain)
            {
                m_brain = CinemachineCore.Instance.FindPotentialTargetBrain(vcamBase);
                if (m_brain == null)
                {
                    if (m_logs)
                        VP_Debug.LogError("Brain is null");
                    enabled = false;
                }
            }
           

            if (!m_brain.IsBlending)
            {
                if (m_brain.IsLive(vcamBase))
                {
                    OnBlendFinished.Invoke(vcamBase);
                    OnGenericBlendFinished.Invoke();
                }
                else
                {
                    if (OnBlendFinished.GetPersistentEventCount() > 0)
                    {
                        OnBlendFinished.Invoke(vcamBase);
                    }

                    if (OnGenericBlendFinished.GetPersistentEventCount() > 0)
                    {
                        OnGenericBlendFinished.Invoke();
                    }
                }
                if (m_logs)
                    VP_Debug.Log("Deactivating Camera Blend finisher in object "+name);
                enabled = false;
            }
        }
#endif
    }
}
