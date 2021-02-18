using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#if USE_CINEMACHINE
using Cinemachine;
#endif


#if USE_MORE_EFFECTIVE_COROUTINES
using MEC;
#endif

namespace VirtualPhenix
{
    [AddComponentMenu(VP_PrefixSetup.MAIN_PREFIX + "/Camera/Cinemachine Camera Shake")]
    public class VP_CinemachineCameraShake : VP_MonoBehaviour
    {
#if USE_CINEMACHINE
        [SerializeField] protected CinemachineVirtualCamera m_virtualCamera;
#endif
        protected float m_shakeTimerTotal;
        protected float m_shakeTimer;
        protected float m_startingIntensity;

        public UnityEvent OnShake;

        protected virtual void Reset()
        {
#if USE_CINEMACHINE
            m_virtualCamera = GetComponent<CinemachineVirtualCamera>();
#endif
        }

        protected override void Initialize()
        {
            base.Initialize();
#if USE_CINEMACHINE
            if (!m_virtualCamera)
                m_virtualCamera = GetComponent<CinemachineVirtualCamera>();
#endif
        }

        public void Shake(float _intensity, float _time, float _frequencyGrain = 2f, UnityAction _callback = null)
        {
            m_startingIntensity = _intensity;
#if USE_CINEMACHINE
            var prl = m_virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
             prl.m_FrequencyGain = _frequencyGrain;
#if USE_MORE_EFFECTIVE_COROUTINES
            Timing.RunCoroutine(DownCounter(prl, _time, _callback).CancelWith(gameObject));
#else
            StartCoroutine(DownCounter(prl, _time,_callback));
#endif
#endif
        }

#if USE_CINEMACHINE
#if USE_MORE_EFFECTIVE_COROUTINES
        IEnumerator<float> DownCounter(CinemachineBasicMultiChannelPerlin prl, float _time, UnityAction _callback = null)
        {
            m_shakeTimerTotal = _time;
            m_shakeTimer = _time;
            while (m_shakeTimer > 0)
            {
                m_shakeTimer -= Time.deltaTime;
                prl.m_AmplitudeGain = Mathf.Lerp(m_startingIntensity, 0f, 1 - (m_shakeTimer / m_shakeTimerTotal));
                yield return Timing.WaitForOneFrame;
            }
            prl.m_AmplitudeGain = 0;

            OnShake.Invoke();

            if (_callback != null)
                _callback.Invoke();
        }
#else
        IEnumerator DownCounter(CinemachineBasicMultiChannelPerlin prl, float _time, UnityAction _callback = null)
        {
            m_shakeTimerTotal = _time;
            m_shakeTimer = _time;
            while (m_shakeTimer > 0)
            {
                m_shakeTimer -= Time.deltaTime;
                prl.m_AmplitudeGain = Mathf.Lerp(m_startingIntensity, 0f, 1 - (m_shakeTimer / m_shakeTimerTotal));
                yield return null;
            }
            prl.m_AmplitudeGain = 0;

            OnShake.Invoke();

            if (_callback != null)
                _callback.Invoke();
        }
#endif
#endif
    }

}