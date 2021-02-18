using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

#if USE_MORE_EFFECTIVE_COROUTINES
using MEC;
#endif

namespace VirtualPhenix
{
    [DefaultExecutionOrder(VP_ExecutingOrderSetup.TIMER), AddComponentMenu(VP_PrefixSetup.MAIN_PREFIX + "/Time/Timer")]
    public class VP_Timer : VP_MonoBehaviour
    {
        [Header("Text"),Space]
        public bool m_setToText;
        [SerializeField] protected TMP_Text m_text;
        
        [Header("Time"), Space]
        protected float m_currentTime = 0f;
        [SerializeField] protected float m_time;
        [SerializeField] protected bool m_isRunning;

        [Header("Event"), Space]
        [SerializeField] protected UnityEvent m_onTimerEnd;

#if USE_MORE_EFFECTIVE_COROUTINES
        protected CoroutineHandle m_coroutine;
#else
        protected Coroutine m_coroutine;
#endif

        public UnityEvent OnTimerEnd { get { return m_onTimerEnd; } }
        public bool IsRunning { get { return m_isRunning; } }

        protected virtual void Reset()
        {
            m_text = GetComponent<TMP_Text>();
            m_setToText = m_text != null;

            m_initializationTime = InitializationTime.OnAwake;
            m_startListeningTime = StartListenTime.OnEnable;
            m_stopListeningTime = StopListenTime.OnDisable;
        }

        protected override void Awake()
        {
            base.Awake();

            if (!m_text)
                m_text = GetComponent<TMP_Text>();
        }

        public virtual void StartTimer(float _time, bool _up, bool _reset = true, UnityAction _callback = null)
        {
            if (_reset || !m_isRunning)
            {
                StopTimer();

                m_isRunning = true;

                if (_up)
                {
#if USE_MORE_EFFECTIVE_COROUTINES
                    m_coroutine = Timing.RunCoroutine(WaitUpTime(_time, _callback).CancelWith(gameObject));
#else
                    m_coroutine = StartCoroutine(WaitUpTime(_time, _callback));
#endif
                }
                else
                {
#if USE_MORE_EFFECTIVE_COROUTINES
                    m_coroutine = Timing.RunCoroutine(WaitDownTime(_time, _callback).CancelWith(gameObject));
#else
                    m_coroutine = StartCoroutine(WaitDownTime(_time, _callback));
#endif
                }
            }
        }

        public virtual void StopTimer()
        {
            if (m_isRunning)
            {
#if USE_MORE_EFFECTIVE_COROUTINES
                Timing.KillCoroutines(m_coroutine);
#else
                StopCoroutine(m_coroutine);
#endif

                m_isRunning = false;
                SetTimeText(0f);
            }
        }

	    public virtual void SetTimeText(float _time)
        {
            if (m_text != null)
            {
                float minutesLeft = Mathf.Floor(_time / 60);
                float secondsLeft = Mathf.Floor(_time % 60);
                var minZero = minutesLeft < 10 ? "0" : "";
                var secZero = secondsLeft < 10 ? "0" : "";
                m_text.text = GetTranslatedTime()+": " + minZero + Mathf.Round(minutesLeft).ToString() + ":" + secZero + Mathf.Round(secondsLeft).ToString();
            }
        }

        protected virtual string GetTranslatedTime()
        {
            return "Time";
        }

#if USE_MORE_EFFECTIVE_COROUTINES
        protected virtual IEnumerator<float> WaitUpTime(float _time, UnityAction _callback = null)
        {
            m_currentTime = 0f;

            while(m_currentTime < _time)
            {
                m_currentTime += Timing.DeltaTime;

                if (m_setToText)
                    SetTimeText(m_currentTime);

                yield return Timing.WaitForOneFrame;
            }

            m_isRunning = false;
            SetTimeText(_time);
            m_onTimerEnd.Invoke();

            if (_callback != null)
                _callback.Invoke();
        }

        protected virtual IEnumerator<float> WaitDownTime(float _time, UnityAction _callback = null)
        {
            m_currentTime = _time;

            while (m_currentTime > 0f)
            {
                m_currentTime -= Timing.DeltaTime;

                if (m_setToText)
                    SetTimeText(m_currentTime);

                yield return Timing.WaitForOneFrame;
            }

            m_isRunning = false;
            SetTimeText(0f);
            m_onTimerEnd.Invoke();

            if (_callback != null)
                _callback.Invoke();
        }
#else
        protected virtual IEnumerator WaitUpTime(float _time, UnityAction _callback = null)
        {
            m_currentTime = 0f;

            while(m_currentTime < _time)
            {
                m_currentTime+=Time.deltaTime;

                if (m_setToText)
                    SetTimeText(m_currentTime);

                yield return null;
            }

            m_isRunning = false;
            SetTimeText(_time);
            m_onTimerEnd.Invoke();

            if (_callback != null)
                _callback.Invoke();
        }

        protected virtual IEnumerator WaitDownTime(float _time, UnityAction _callback = null)
        {
            m_currentTime = _time;

            while(m_currentTime > 0)
            {
                m_currentTime-=Time.deltaTime;

                if (m_setToText)
                    SetTimeText(m_currentTime);

                yield return null;
            }
            
             m_isRunning = false;
             SetTimeText(0f);
            m_onTimerEnd.Invoke();

            if (_callback != null)
                _callback.Invoke();
        }
#endif
    }
}