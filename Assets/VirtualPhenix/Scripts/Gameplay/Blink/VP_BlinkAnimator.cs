using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#if USE_MORE_EFFECTIVE_COROUTINES
using MEC;
#endif

namespace VirtualPhenix.Gameplay
{
	public class VP_BlinkAnimator : VP_MonoBehaviour
	{
		[Header("Blink Animator")]
		[SerializeField] protected bool m_blinkOnEnable = false;
		[SerializeField] protected float m_timeBetweenBlinks = 3.75f;
		[SerializeField] protected float m_timeBlinking = .075f;
		[SerializeField] protected bool m_randomTimeBetweenBlinks = true;
		[SerializeField] protected float m_minTimeBetweenBlinks = 2.5f;
		[SerializeField] protected float m_maxTimeBetweenBlinks = 5f;

		public bool m_blockingBlink = false;
#if USE_MORE_EFFECTIVE_COROUTINES
		protected CoroutineHandle m_coroutine;
#else
		protected Coroutine m_coroutine;
#endif

		protected virtual void Reset()
        {
			m_initializationTime = InitializationTime.OnAwake;
			m_startListeningTime = StartListenTime.None;
			m_stopListeningTime = StopListenTime.None;
        }


		// Start is called before the first frame update
		protected override void OnEnable()
		{			
			base.OnEnable();

			if (m_blinkOnEnable)
				StartWaitForBlink();
		}

		public virtual void StartWaitForBlink()
		{
			StartWaitForBlinkSetup();

			float time = m_randomTimeBetweenBlinks ? UnityEngine.Random.Range(m_minTimeBetweenBlinks, m_maxTimeBetweenBlinks) : m_timeBetweenBlinks;
#if USE_MORE_EFFECTIVE_COROUTINES
			m_coroutine = Timing.RunCoroutine(Wait(time, Blink).CancelWith(gameObject));
#else
			m_coroutine = StartCoroutine(Wait(time, Blink));
#endif
		}



#if USE_MORE_EFFECTIVE_COROUTINES
		protected virtual IEnumerator<float> Wait(float _time, System.Action _callback)
		{
			float time = 0f;

			while (time < _time || m_blockingBlink)
			{

				time += Timing.DeltaTime;
				yield return Timing.WaitForOneFrame;
			}

			if (_callback != null)
				_callback.Invoke();
		}
#else
		protected virtual IEnumerator Wait(float _time, UnityAction _callback = null)
		{
			float time = 0f;

			while (time < _time || m_blockingBlink)
			{
				time += Time.deltaTime;
				yield return null;
			}

			if (_callback != null)
				_callback.Invoke();
		}
#endif

		protected virtual void BlinkSetup()
        {

        }		
		
		protected virtual void StartWaitForBlinkSetup()
        {

        }

		public virtual void Blink()
		{
			BlinkSetup();

#if USE_MORE_EFFECTIVE_COROUTINES
			m_coroutine = Timing.RunCoroutine(Wait(m_timeBlinking, StartWaitForBlink).CancelWith(gameObject));
#else
			m_coroutine = StartCoroutine(Wait(m_timeBlinking, StartWaitForBlink));
#endif
		}

	}
}
