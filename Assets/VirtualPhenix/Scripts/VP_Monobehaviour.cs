
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if USE_MORE_EFFECTIVE_COROUTINES
using MEC;
#endif

namespace VirtualPhenix
{
    public class VP_MonoBehaviour : VP_Monobehaviour
    {

    }

    public class VP_Monobehaviour : Helpers.HierarchyIcon
    {
        public enum StartListenTime
        {
            None,
            OnAwake,
            OnStart,
            OnEnable,
            OnInitialization,
        }

        public enum StopListenTime
        {
            None,
            OnDisable,
            OnDestroy
        }

        public enum InitializationTime
        {
            None,
            Singleton,
            OnAwake,
            OnStart,
            OnEnable
        }

        [SerializeField] protected StartListenTime m_startListeningTime = StartListenTime.None;
        [SerializeField] protected StopListenTime m_stopListeningTime = StopListenTime.None;
        [SerializeField] protected InitializationTime m_initializationTime = InitializationTime.OnAwake;

	    protected bool m_initialized = false;

#if USE_MORE_EFFECTIVE_COROUTINES
        protected CoroutineHandle m_waitCoroutine;
#endif
        protected Coroutine m_regulaWaitCoroutine;

        public virtual bool IsInitialized
	    {
		    get
		    {
		    	return m_initialized;
		    }
	    }
	    

        protected virtual void Awake()
	    {
		    if (m_startListeningTime == StartListenTime.OnAwake)
		    {
			    StartAllListeners();
		    }
		    
            if (m_initializationTime == InitializationTime.OnAwake)
            {
                Initialize();
            }
        }

        protected virtual void Start()
	    {
		    if (m_startListeningTime == StartListenTime.OnStart)
		    {
			    StartAllListeners();
		    }
		    
            if (m_initializationTime == InitializationTime.OnStart)
            {
                Initialize();
            }
        }

        protected virtual void OnEnable()
        {
            if (m_startListeningTime == StartListenTime.OnEnable)
            {
                StartAllListeners();
            }
            
	        if (m_initializationTime == InitializationTime.OnEnable)
	        {
		        Initialize();
	        }
        }

        protected virtual void StartAllListeners()
        {
            // do stuff

        }

        protected virtual void StopAllListeners()
        {
            // do stuff
        }

        protected virtual void OnDestroy()
        {
            if (m_stopListeningTime == StopListenTime.OnDestroy)
                StopAllListeners();
        }

        protected virtual void OnDisable()
        {
            if (m_stopListeningTime == StopListenTime.OnDisable)
                StopAllListeners();
        }

        protected virtual void Initialize()
	    {
		    m_initialized = true;
		    
            // Only used in managers
            if (m_startListeningTime == StartListenTime.OnInitialization)
            {
                StartAllListeners();
            }
        }

        protected virtual float Randf(float max, float min = 0)
        {
            return Random.Range(min, max);
        }

        protected virtual int Rand(int max, int min = 0)
        {
            return Random.Range(min, max);
        }

        protected virtual string _INTL(string _originalStr, params string[] _args)
        {
            return VP_INTL.INTL(_originalStr, _args);
        }

#if USE_MORE_EFFECTIVE_COROUTINES
        public virtual void WaitTime(float _time, UnityEngine.Events.UnityAction _callback = null, Segment _segment = Segment.Update, string _coroutineName = "", bool _killPrevious = false, bool _saveIt = false)
        {
            if (_killPrevious && !string.IsNullOrEmpty(_coroutineName))
                Timing.KillCoroutines(_coroutineName);

            if (_saveIt)
               m_waitCoroutine = Timing.RunCoroutine(_WaitTime(_time, _callback).CancelWith(gameObject), _segment, _coroutineName);
            else
                Timing.RunCoroutine(_WaitTime(_time, _callback).CancelWith(gameObject), _segment, _coroutineName);
        }

        protected virtual IEnumerator<float> _WaitTime(float _time, UnityEngine.Events.UnityAction _callback = null)
        {
            yield return Timing.WaitForSeconds(_time);
            if (_callback != null)
                _callback.Invoke();
        }
        
	    public virtual void WaitTime(float _time, System.Func<bool> _exitCallback, UnityEngine.Events.UnityAction _callback = null, Segment _segment = Segment.Update, string _coroutineName = "", bool _killPrevious = false, bool _saveIt = false)
        {
            if (_killPrevious && !string.IsNullOrEmpty(_coroutineName))
                Timing.KillCoroutines(_coroutineName);

            if (_saveIt)
	            m_waitCoroutine = Timing.RunCoroutine(_WaitTime(_time, _exitCallback, _callback).CancelWith(gameObject), _segment, _coroutineName);
            else
	            Timing.RunCoroutine(_WaitTime(_time, _exitCallback, _callback).CancelWith(gameObject), _segment, _coroutineName);
        }

	    protected virtual IEnumerator<float> _WaitTime(float _time, System.Func<bool> _exitCallback, UnityEngine.Events.UnityAction _callback = null)
        {
	        float timer = 0;
	        
	        while (timer < _time && !_exitCallback.Invoke())
	        {
	        	timer+=Timing.DeltaTime;
	        	yield return Timing.WaitForOneFrame;
	        }
	        
            if (_callback != null)
                _callback.Invoke();
        }
#else
        public virtual void WaitTime(float _time, UnityEngine.Events.UnityAction _callback = null, bool _killPrevious = false, bool _saveIt = false)
        {
            if (_killPrevious && m_regulaWaitCoroutine != null)
                StopCoroutine(m_regulaWaitCoroutine);

            if (_saveIt)
                m_regulaWaitCoroutine = StartCoroutine(WaitTimeRegular(_time, _callback));
            else
                StartCoroutine(WaitTimeRegular(_time, _callback));
        }

#endif
        protected virtual IEnumerator WaitTimeRegular(float _time, UnityEngine.Events.UnityAction _callback = null)
        {
            yield return new WaitForSeconds(_time);
            if (_callback != null)
                _callback.Invoke();
        }
    }
}
