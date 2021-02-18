using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if USE_MORE_EFFECTIVE_COROUTINES
using MEC;
#endif
using UnityEngine.SceneManagement;
using VirtualPhenix;
using TMPro;
using VirtualPhenix.Localization;
using UnityEngine.Events;

namespace VirtualPhenix
{
	[DefaultExecutionOrder(VP_ExecutingOrderSetup.SCENE_MANAGER),AddComponentMenu(VP_PrefixSetup.MAIN_PREFIX+"/Scenes/Scene Manager")]
	public class VP_SceneManager : VP_SingletonMonobehaviour<VP_SceneManager>
	{
		[Header("Scene Manager"), Space]
		public bool m_hasLoadingScreen = true;
		[SerializeField] protected GameObject m_loadingGroup;
		[SerializeField] protected TMP_Text m_loadingText;
		[SerializeField] protected Image m_progressBar;
		[SerializeField] protected float m_progress;

		[Header("Scene Database"), Space]
		[SerializeField] protected bool m_useDatabase;
#if ODIN_INSPECTOR		
		[Sirenix.Serialization.OdinSerialize] protected VP_SceneDatabase m_database;
#else
		[SerializeField] protected VP_SceneDatabase m_database;
#endif

		protected Queue<UnityEngine.Events.UnityAction> m_loadUnloadQueue;
		[HideInInspector] public UnityAction m_onEndQueue;

		public UnityEvent m_onStartLoad;
		public UnityEvent m_onEndLoad;

#if !USE_MORE_EFFECTIVE_COROUTINES
		protected Coroutine m_loadCoroutine;
#else
		protected CoroutineHandle m_loadCoroutine;
#endif

		protected override void Reset()
		{
			base.Reset();

			if (!m_database)
			{
				m_database = Resources.Load<VP_SceneDatabase>(VP_SceneSetup.DATABASE_PATH);
			}
		}

		public virtual void StartListeningOnStartLoad(UnityAction _callback)
		{
			if (_callback == null)
				return;

			m_onStartLoad.AddListener(_callback);
		}

		public virtual void StopListeningOnStartLoad(UnityAction _callback)
		{
			if (_callback == null)
				return;

			m_onStartLoad.RemoveListener(_callback);
		}

		public virtual void InvokeOnStartLoad()
		{
			if (m_onStartLoad != null)
				m_onStartLoad.Invoke();
		}

		public virtual void StartListeningOnEndLoad(UnityAction _callback)
		{
			if (_callback == null)
				return;

			m_onStartLoad.AddListener(_callback);
            
		}

		public virtual void StopListeningOnEndLoad(UnityAction _callback)
		{
			if (_callback == null)
				return;

			m_onStartLoad.RemoveListener(_callback);
		}

		public virtual void InvokeOnEndLoad()
		{
			if (m_onEndLoad != null)
				m_onEndLoad.Invoke();
		}

		// Start is called before the first frame update
		protected override void Start()
		{
			base.Start();


			m_loadUnloadQueue = new Queue<UnityEngine.Events.UnityAction>();
		}

		public virtual void SceneQueueLoad()
		{
			if (m_loadUnloadQueue.Count > 0)
			{
				m_loadUnloadQueue.Dequeue().Invoke();
			}
			else
			{
				if (m_onEndQueue != null)
				{
					m_onEndQueue.Invoke();
					m_onEndQueue = null;
				}
			}
		}

		public virtual void AddOnEndQueueCallback(UnityAction _callback)
		{
			m_onEndQueue += _callback;
		}

		public virtual void LoadSceneToQueue(string _scene, bool load = true, float delay = 0f, bool _blockInput = false, bool _loadingScreen = false, bool _unlockAfterLoad = true, UnityEngine.Events.UnityAction _callback = null)
		{
			if (m_useDatabase)
			{
				string s = m_database.GetSceneName(_scene);
				if (!string.IsNullOrEmpty(s))
					_scene = s;
			}

			m_loadUnloadQueue.Enqueue(() =>
			{
				if (load)
					LoadSceneAdditiveWithCallback(_scene, SceneQueueLoad, delay, _blockInput, _loadingScreen, _unlockAfterLoad, _callback);
				else
					UnloadAdditiveSceneWithCallback(_scene, SceneQueueLoad, delay, "", _blockInput, _loadingScreen, _unlockAfterLoad, _callback);
			});
		}

		public virtual void LoadSceneAdditive(string _scene, float _delayAfterLoading = 1f, string _eventName = "", bool _blockInputs = true, bool _showLoadingScreen = true, bool _unblockInputs = true, UnityEngine.Events.UnityAction _callback = null)
		{
			if (m_useDatabase)
			{
				string s = m_database.GetSceneName(_scene);
				if (!string.IsNullOrEmpty(s))
					_scene = s;
			}

#if USE_MORE_EFFECTIVE_COROUTINES
			if (!SceneManager.GetSceneByName(_scene).isLoaded)
				m_loadCoroutine =Timing.RunCoroutine(LoadingScreen(_scene, _delayAfterLoading, _eventName, LoadSceneMode.Additive, _blockInputs, _showLoadingScreen, _unblockInputs, _callback));
#else
			if (!SceneManager.GetSceneByName(_scene).isLoaded)
			m_loadCoroutine = StartCoroutine(LoadingScreen(_scene, _delayAfterLoading, _eventName, LoadSceneMode.Additive, _blockInputs, _showLoadingScreen, _unblockInputs, _callback));
#endif
		}

		public virtual void LoadSceneAdditiveWithParameter<T>(string _scene, T parameter, float _delayAfterLoading = 1f, string _eventName = "", bool _blockInputs = true, bool _showLoadingScreen = true, bool _unblockInputs = true, UnityEngine.Events.UnityAction _callback = null)
		{
			if (m_useDatabase)
			{
				string s = m_database.GetSceneName(_scene);
				if (!string.IsNullOrEmpty(s))
					_scene = s;
			}

#if USE_MORE_EFFECTIVE_COROUTINES
			if (!SceneManager.GetSceneByName(_scene).isLoaded)
				m_loadCoroutine = Timing.RunCoroutine(LoadingScreenWithParameter(_scene, parameter, _delayAfterLoading, _eventName, LoadSceneMode.Additive, _blockInputs, _showLoadingScreen, _unblockInputs, _callback));
#else
			if (!SceneManager.GetSceneByName(_scene).isLoaded)
			m_loadCoroutine = StartCoroutine(LoadingScreenWithParameter(_scene, parameter, _delayAfterLoading, _eventName, LoadSceneMode.Additive, _blockInputs, _showLoadingScreen, _unblockInputs, _callback));
#endif
		}
		public virtual void LoadSceneAdditiveWithCallback(string _scene, UnityEngine.Events.UnityAction _callback, float _delayAfterLoading = 1f, bool _blockInputs = true, bool _showLoadingScreen = true, bool _unblockInputs = true, UnityEngine.Events.UnityAction _secondcallback = null)
		{
			if (m_useDatabase)
			{
				string s = m_database.GetSceneName(_scene);
				if (!string.IsNullOrEmpty(s))
					_scene = s;
			}

#if USE_MORE_EFFECTIVE_COROUTINES
			if (!SceneManager.GetSceneByName(_scene).isLoaded)
				m_loadCoroutine =Timing.RunCoroutine(LoadingSceneWithCallback(_scene, _callback, _delayAfterLoading, LoadSceneMode.Additive, _blockInputs, _showLoadingScreen, _unblockInputs, _secondcallback));
			else
				_callback?.Invoke();
#else
			if (!SceneManager.GetSceneByName(_scene).isLoaded)
			m_loadCoroutine = StartCoroutine(LoadingSceneWithCallback(_scene, _callback, _delayAfterLoading, LoadSceneMode.Additive, _blockInputs, _showLoadingScreen, _unblockInputs, _secondcallback));
			else
			_callback?.Invoke();
#endif
		}
		public virtual void UnloadAdditiveScene(string _scene, float _delayAfterLoading = 1f, string _eventName = "", bool _blockInputs = true, bool _showLoadingScreen = true, bool _unblockInputs = true)
		{
			if (m_useDatabase)
			{
				string s = m_database.GetSceneName(_scene);
				if (!string.IsNullOrEmpty(s))
					_scene = s;
			}

#if USE_MORE_EFFECTIVE_COROUTINES
			if (SceneManager.GetSceneByName(_scene).isLoaded)
				m_loadCoroutine =Timing.RunCoroutine(UnLoadingScreen(_scene, _delayAfterLoading, _eventName, _blockInputs, _showLoadingScreen, _unblockInputs));
#else
			if (SceneManager.GetSceneByName(_scene).isLoaded)
			m_loadCoroutine = StartCoroutine(UnLoadingScreen(_scene, _delayAfterLoading, _eventName, _blockInputs, _showLoadingScreen, _unblockInputs));
#endif
		}

		public virtual void UnloadAdditiveSceneWithCallback(string _scene, UnityEngine.Events.UnityAction _callback = null, float _delayAfterLoading = 1f, string _eventName = "", bool _blockInputs = true, bool _showLoadingScreen = true, bool _unblockInputs = true, UnityEngine.Events.UnityAction _seccallback = null)
		{
			if (m_useDatabase)
			{
				string s = m_database.GetSceneName(_scene);
				if (!string.IsNullOrEmpty(s))
					_scene = s;
			}

#if USE_MORE_EFFECTIVE_COROUTINES
			if (SceneManager.GetSceneByName(_scene).isLoaded)
				m_loadCoroutine =Timing.RunCoroutine(UnLoadingScreenWithCallback(_scene, _callback, _delayAfterLoading, _eventName, _blockInputs, _showLoadingScreen, _unblockInputs, _seccallback));
			else
				_callback?.Invoke();
#else
			if (SceneManager.GetSceneByName(_scene).isLoaded)
			m_loadCoroutine = StartCoroutine(UnLoadingScreenWithCallback(_scene, _callback, _delayAfterLoading, _eventName, _blockInputs, _showLoadingScreen, _unblockInputs, _seccallback));
			else
			_callback?.Invoke();
#endif
		}

		public virtual void LoadSceneAdditive(int _scene, float _delayAfterLoading = 1f, string _eventName = "", bool _blockInputs = true, bool _showLoadingScreen = true, bool _unblockInputs = true, UnityEngine.Events.UnityAction _callback = null)
		{

#if USE_MORE_EFFECTIVE_COROUTINES
			if (!SceneManager.GetSceneByBuildIndex(_scene).isLoaded)
				m_loadCoroutine =Timing.RunCoroutine(LoadingScreen(_scene, _delayAfterLoading, _eventName, LoadSceneMode.Additive, _blockInputs, _showLoadingScreen, _unblockInputs, _callback));
#else
			if (!SceneManager.GetSceneByBuildIndex(_scene).isLoaded)
			m_loadCoroutine = StartCoroutine(LoadingScreen(_scene, _delayAfterLoading, _eventName, LoadSceneMode.Additive, _blockInputs, _showLoadingScreen, _unblockInputs, _callback));

#endif
		}

		public virtual void LoadSceneAdditiveWithParameter<T>(int _scene, T parameter, float _delayAfterLoading = 1f, string _eventName = "", bool _blockInputs = true, bool _showLoadingScreen = true, bool _unblockInputs = true, UnityEngine.Events.UnityAction _callback = null)
		{
#if USE_MORE_EFFECTIVE_COROUTINES
			if (!SceneManager.GetSceneByBuildIndex(_scene).isLoaded)
				m_loadCoroutine = Timing.RunCoroutine(LoadingScreenWithParameter(_scene, parameter, _delayAfterLoading, _eventName, LoadSceneMode.Additive, _blockInputs, _showLoadingScreen, _unblockInputs, _callback));
#else
			if (!SceneManager.GetSceneByBuildIndex(_scene).isLoaded)
			m_loadCoroutine = StartCoroutine(LoadingScreenWithParameter(_scene, parameter, _delayAfterLoading, _eventName, LoadSceneMode.Additive, _blockInputs, _showLoadingScreen, _unblockInputs, _callback));

#endif
		}

		public virtual void UnloadAdditiveScene(int _scene, float _delayAfterLoading = 1f, string _eventName = "", bool _blockInputs = true, bool _showLoadingScreen = true, bool _unblockInputs = true, UnityEngine.Events.UnityAction _callback = null)
		{
#if USE_MORE_EFFECTIVE_COROUTINES
			if (SceneManager.GetSceneByBuildIndex(_scene).isLoaded)
				m_loadCoroutine = Timing.RunCoroutine(UnLoadingScreen(_scene, _delayAfterLoading, _eventName, _blockInputs, _showLoadingScreen, _unblockInputs, _callback));
#else
			if (SceneManager.GetSceneByBuildIndex(_scene).isLoaded)
			m_loadCoroutine = StartCoroutine(UnLoadingScreen(_scene, _delayAfterLoading, _eventName, _blockInputs, _showLoadingScreen, _unblockInputs, _callback));
#endif
		}
		/// <summary>
		/// Load Level Async so we can show the 
		/// </summary>
		/// <param name="_scene"></param>
		/// <param name="_delayAfterLoading"></param>
		/// <param name="_eventName"></param>
		public virtual void LoadSceneAsync(int _scene, float _delayAfterLoading = 1f, string _eventName = "", bool _blockInputs = true, bool _showLoadingScreen = true, bool _unblockInputs = true, UnityEngine.Events.UnityAction _callback = null)
		{
#if USE_MORE_EFFECTIVE_COROUTINES
			if (!SceneManager.GetSceneByBuildIndex(_scene).isLoaded)
				m_loadCoroutine = Timing.RunCoroutine(LoadingScreen(_scene, _delayAfterLoading, _eventName, LoadSceneMode.Single, _blockInputs, _showLoadingScreen, _unblockInputs, _callback));
#else
			if (!SceneManager.GetSceneByBuildIndex(_scene).isLoaded)
			m_loadCoroutine = StartCoroutine(LoadingScreen(_scene, _delayAfterLoading, _eventName, LoadSceneMode.Single, _blockInputs, _showLoadingScreen, _unblockInputs, _callback));
#endif
		}

		public virtual void LoadSceneAsync(string _scene, float _delayAfterLoading = 1f, string _eventName = "", bool _blockInputs = true, bool _showLoadingScreen = true, bool _unblockInputs = true, UnityEngine.Events.UnityAction _callback = null)
		{
			if (m_useDatabase)
			{
				string s = m_database.GetSceneName(_scene);
				if (!string.IsNullOrEmpty(s))
					_scene = s;
			}

#if USE_MORE_EFFECTIVE_COROUTINES
			if (!SceneManager.GetSceneByName(_scene).isLoaded)
				m_loadCoroutine = Timing.RunCoroutine(LoadingScreen(_scene, _delayAfterLoading, _eventName, LoadSceneMode.Single, _blockInputs, _showLoadingScreen, _unblockInputs, _callback));
#else
			if (!SceneManager.GetSceneByName(_scene).isLoaded)
			m_loadCoroutine = StartCoroutine(LoadingScreen(_scene, _delayAfterLoading, _eventName, LoadSceneMode.Single, _blockInputs, _showLoadingScreen, _unblockInputs, _callback));
#endif
		}

		/// <summary>
		/// Load Level Async so we can show the 
		/// </summary>
		/// <param name="_scene"></param>
		/// <param name="_delayAfterLoading"></param>
		/// <param name="_eventName"></param>
		public virtual void LoadSceneAsyncWithParameter<T>(string _scene, T parameter, float _delayAfterLoading = 1f, string _eventName = "", bool _blockInputs = true, bool _showLoadingScreen = true, bool _unblockInputs = true, UnityEngine.Events.UnityAction _callback = null)
		{
			if (m_useDatabase)
			{
				string s = m_database.GetSceneName(_scene);
				if (!string.IsNullOrEmpty(s))
					_scene = s;
			}


#if USE_MORE_EFFECTIVE_COROUTINES
			if (!SceneManager.GetSceneByName(_scene).isLoaded)
				m_loadCoroutine = Timing.RunCoroutine(LoadingScreenWithParameter(_scene, parameter, _delayAfterLoading, _eventName, LoadSceneMode.Single, _blockInputs, _showLoadingScreen, _unblockInputs, _callback));
#else
			if (!SceneManager.GetSceneByName(_scene).isLoaded)
			m_loadCoroutine = StartCoroutine(LoadingScreenWithParameter(_scene, parameter, _delayAfterLoading, _eventName, LoadSceneMode.Single, _blockInputs, _showLoadingScreen, _unblockInputs, _callback));
#endif
		}

		/// <summary>
		/// Load Level Async so we can show the 
		/// </summary>
		/// <param name="_scene"></param>
		/// <param name="_delayAfterLoading"></param>
		/// <param name="_eventName"></param>
		public virtual void LoadFieldAsyncScene(string _scene, UnityEngine.Events.UnityAction _callback, float _delayAfterLoading = 1f, bool _blockInputs = true, bool _showLoadingScreen = true, bool _unblockInputs = true, UnityEngine.Events.UnityAction _secondcallback = null)
		{
			if (m_useDatabase)
			{
				string s = m_database.GetSceneName(_scene);
				if (!string.IsNullOrEmpty(s))
					_scene = s;
			}

#if USE_MORE_EFFECTIVE_COROUTINES
			if (!SceneManager.GetSceneByName(_scene).isLoaded)
				m_loadCoroutine = Timing.RunCoroutine(LoadingSceneWithCallback(_scene, _callback, _delayAfterLoading, LoadSceneMode.Single, _blockInputs, _showLoadingScreen, _unblockInputs, _secondcallback));
#else
			if (!SceneManager.GetSceneByName(_scene).isLoaded)
			m_loadCoroutine = StartCoroutine(LoadingSceneWithCallback(_scene, _callback, _delayAfterLoading, LoadSceneMode.Single, _blockInputs, _showLoadingScreen, _unblockInputs, _secondcallback));

#endif
		}

		public virtual void UnloadSceneWithParameter<T>(string _scene, T parameter, float _delayAfterLoading = 1f, string _eventName = "", bool _blockInputs = true, bool _showLoadingScreen = true, bool _unblockInputs = true, UnityEngine.Events.UnityAction _callback = null)
		{
			if (m_useDatabase)
			{
				string s = m_database.GetSceneName(_scene);
				if (!string.IsNullOrEmpty(s))
					_scene = s;
			}

#if USE_MORE_EFFECTIVE_COROUTINES
			if (SceneManager.GetSceneByName(_scene).isLoaded)
				m_loadCoroutine = Timing.RunCoroutine(UnLoadingScreenWithParameter(_scene, parameter, _delayAfterLoading, _eventName, _blockInputs, _showLoadingScreen, _unblockInputs, _callback));
#else
			if (SceneManager.GetSceneByName(_scene).isLoaded)
			m_loadCoroutine = StartCoroutine(UnLoadingScreenWithParameter(_scene, parameter, _delayAfterLoading, _eventName, _blockInputs, _showLoadingScreen, _unblockInputs, _callback));
#endif
		}

		/// <summary>
		/// Load Level Async so we can show the 
		/// </summary>
		/// <param name="_scene"></param>
		/// <param name="_delayAfterLoading"></param>
		/// <param name="_eventName"></param>
		public virtual void LoadSceneAsyncWithParameter<T>(int _scene, T parameter, float _delayAfterLoading = 1f, string _eventName = "", bool _blockInputs = true, bool _showLoadingScreen = true, bool _unblockInputs = true, UnityEngine.Events.UnityAction _callback = null)
		{
#if USE_MORE_EFFECTIVE_COROUTINES
			if (!SceneManager.GetSceneByBuildIndex(_scene).isLoaded)
				m_loadCoroutine = Timing.RunCoroutine(LoadingScreenWithParameter(_scene, parameter, _delayAfterLoading, _eventName, LoadSceneMode.Single, _blockInputs, _showLoadingScreen, _unblockInputs, _callback));
#else
			if (!SceneManager.GetSceneByBuildIndex(_scene).isLoaded)
			m_loadCoroutine = StartCoroutine(LoadingScreenWithParameter(_scene, parameter, _delayAfterLoading, _eventName, LoadSceneMode.Single, _blockInputs, _showLoadingScreen, _unblockInputs, _callback));

#endif
		}

		public virtual void UnloadSceneWithParameter<T>(int _scene, T parameter, float _delayAfterLoading = 1f, string _eventName = "", bool _blockInputs = true, bool _showLoadingScreen = true, bool _unblockInputs = true, UnityEngine.Events.UnityAction _callback = null)
		{
#if USE_MORE_EFFECTIVE_COROUTINES
			if (SceneManager.GetSceneByBuildIndex(_scene).isLoaded)
				m_loadCoroutine = Timing.RunCoroutine(UnLoadingScreenWithParameter(_scene, parameter, _delayAfterLoading, _eventName, _blockInputs, _showLoadingScreen, _unblockInputs, _callback));
#else
			if (SceneManager.GetSceneByBuildIndex(_scene).isLoaded)
			m_loadCoroutine = StartCoroutine(UnLoadingScreenWithParameter(_scene, parameter, _delayAfterLoading, _eventName, _blockInputs, _showLoadingScreen, _unblockInputs, _callback));
#endif
		}

		public virtual void UnloadAdditiveSceneWithParameter<T>(int _scene, T parameter, float _delayAfterLoading = 1f, string _eventName = "", bool _blockInputs = true, bool _showLoadingScreen = true, bool _unblockInputs = true, UnityEngine.Events.UnityAction _callback = null)
		{
#if USE_MORE_EFFECTIVE_COROUTINES
			if (SceneManager.GetSceneByBuildIndex(_scene).isLoaded)
				m_loadCoroutine = Timing.RunCoroutine(UnLoadingScreenWithParameter(_scene, parameter, _delayAfterLoading, _eventName, _blockInputs, _showLoadingScreen, _unblockInputs, _callback));
#else
			if (SceneManager.GetSceneByBuildIndex(_scene).isLoaded)
			m_loadCoroutine = StartCoroutine(UnLoadingScreenWithParameter(_scene, parameter, _delayAfterLoading, _eventName, _blockInputs, _showLoadingScreen, _unblockInputs, _callback));
#endif
		}

		public virtual void UnloadAdditiveSceneWithParameter<T>(string _scene, T parameter, float _delayAfterLoading = 1f, string _eventName = "", bool _blockInputs = true, bool _showLoadingScreen = true, bool _unblockInputs = true, UnityEngine.Events.UnityAction _callback = null)
		{
			if (m_useDatabase)
			{
				string s = m_database.GetSceneName(_scene);
				if (!string.IsNullOrEmpty(s))
					_scene = s;
			}

#if USE_MORE_EFFECTIVE_COROUTINES
			if (SceneManager.GetSceneByName(_scene).isLoaded)
				m_loadCoroutine = Timing.RunCoroutine(UnLoadingScreenWithParameter(_scene, parameter, _delayAfterLoading, _eventName, _blockInputs, _showLoadingScreen, _unblockInputs, _callback));
#else
			if (SceneManager.GetSceneByName(_scene).isLoaded)
			m_loadCoroutine = StartCoroutine(UnLoadingScreenWithParameter(_scene, parameter, _delayAfterLoading, _eventName, _blockInputs, _showLoadingScreen, _unblockInputs, _callback));
#endif
		}

		public virtual void ShowHideLoadingScreenAfterTime(bool _from, bool _to, float _time = 2f, UnityEngine.Events.UnityAction _callback = null)
		{
			if (m_hasLoadingScreen && m_loadingGroup != null)
				m_loadingGroup.SetActive(_from);
                
			WaitTime(_time, () =>
			{
				ShowHideLoadingScreen(_to);
                
				if (_callback != null)
					_callback.Invoke();
			});

		}

		public virtual void ShowHideLoadingScreen(bool show, string _event = "", UnityEngine.Events.UnityAction _callback = null)
		{
			if (m_hasLoadingScreen)
			{
				if (m_loadingGroup != null)
					m_loadingGroup.SetActive(show);

				if (show && m_loadingText != null)
					m_loadingText.text = VP_LocalizationManager.GetText(VP_TextSetup.LoadingScreen.LOADING);

			}

			if (!string.IsNullOrEmpty(_event))
			{
				VP_EventManager.TriggerEvent(_event);
			}

			if (_callback != null)
				_callback.Invoke();
		}

#if USE_MORE_EFFECTIVE_COROUTINES
		
		public virtual void LoadCurrentSceneAgain(float _delayAfterLoading, bool _blockInputs, bool _showLoadingScreen, bool _unblockInputs = true, UnityEngine.Events.UnityAction _callback = null)
		{
			Timing.RunCoroutine(LoadingScreen(SceneManager.GetActiveScene().buildIndex, _delayAfterLoading, VP_EventSetup.Scene.RELOAD_CURRENT_SCENE, LoadSceneMode.Single, _blockInputs, _showLoadingScreen, _unblockInputs, _callback));
		}

		/// <summary>
		/// Coroutine called for loading the next scene
		/// </summary>
		/// <param name="_index"></param>
		/// <param name="_delayAfterLoading"></param>
		/// <param name="_eventName"></param>
		/// <returns></returns>
		protected virtual IEnumerator<float> LoadingSceneWithCallback(string _scene, UnityEngine.Events.UnityAction _callback, float _delayAfterLoading = 1f, LoadSceneMode _mode = LoadSceneMode.Single, bool _blockInputs = true, bool _showLoadingScreen = true, bool _unblockInputs = true, UnityEngine.Events.UnityAction _seccallback = null)
		{
			InvokeOnStartLoad();

			if (_showLoadingScreen)
				ShowHideLoadingScreen(true);

			Application.backgroundLoadingPriority = ThreadPriority.Low;

			AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(_scene, _mode);
			m_progress = 0f;

			// Wait until the asynchronous scene fully loads
			while (!asyncLoad.isDone)
			{

				m_progress = asyncLoad.progress;
				if (m_progressBar)
					m_progressBar.fillAmount = m_progress;


				yield return Timing.WaitForOneFrame;
			}
			InvokeOnEndLoad();
			if (_delayAfterLoading > 0)
				yield return Timing.WaitForSeconds(_delayAfterLoading);

			if (_showLoadingScreen)
				ShowHideLoadingScreen(false);


			if (_callback != null)
				_callback.Invoke();

			if (_seccallback != null)
				_seccallback.Invoke();



			VP_EventManager.TriggerEvent(VP_EventSetup.Audio.CLEAR_NULL_AUDIO);
		}

		/// <summary>
		/// Coroutine called for loading the next scene
		/// </summary>
		/// <param name="_index"></param>
		/// <param name="_delayAfterLoading"></param>
		/// <param name="_eventName"></param>
		/// <returns></returns>
		protected virtual IEnumerator<float> LoadingScreenWithParameter<T>(int _index, T parameter, float _delayAfterLoading = 1f, string _eventName = "", LoadSceneMode _mode = LoadSceneMode.Single, bool _blockInputs = true, bool _showLoadingScreen = true, bool _unblockInputs = true, UnityEngine.Events.UnityAction _callback = null)
		{
			InvokeOnStartLoad();

			if (_showLoadingScreen)
				ShowHideLoadingScreen(true);

			AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(_index, _mode);
			m_progress = 0f;

			// Wait until the asynchronous scene fully loads
			while (!asyncLoad.isDone)
			{
				m_progress = asyncLoad.progress;
				if (m_progressBar)
					m_progressBar.fillAmount = m_progress;
				yield return Timing.WaitForOneFrame;
			}
			InvokeOnEndLoad();
			yield return Timing.WaitForSeconds(_delayAfterLoading);
			if (_showLoadingScreen)
				ShowHideLoadingScreen(false);

			if (!string.IsNullOrEmpty(_eventName))
			{
				//Debug.Log(_eventName + " is being called");

				VP_EventManager.TriggerEvent(_eventName, parameter);
			}

			if (_callback != null)
			{
				_callback.Invoke();
			}
            
      

			VP_EventManager.TriggerEvent(VP_EventSetup.Audio.CLEAR_NULL_AUDIO);
		}

		protected virtual IEnumerator<float> UnLoadingScreenWithParameter<T>(int _index, T parameter, float _delayAfterLoading = 1f, string _eventName = "", bool _blockInputs = true, bool _showLoadingScreen = true, bool _unblockInputs = true, UnityEngine.Events.UnityAction _callback = null)
		{
			InvokeOnStartLoad();

			if (_showLoadingScreen)
			{

				ShowHideLoadingScreen(true);
			}

			m_progress = 0f;

			AsyncOperation asyncLoad = SceneManager.UnloadSceneAsync(_index);

			// Wait until the asynchronous scene fully loads
			while (!asyncLoad.isDone)
			{
				m_progress = asyncLoad.progress;
				if (m_progressBar)
					m_progressBar.fillAmount = m_progress;
				yield return Timing.WaitForOneFrame;
			}
			InvokeOnEndLoad();
			yield return Timing.WaitForSeconds(_delayAfterLoading);

			if (_showLoadingScreen)
				ShowHideLoadingScreen(false);

			if (!string.IsNullOrEmpty(_eventName))
			{
				//  Debug.Log(_eventName + " is being called");
				VP_EventManager.TriggerEvent(_eventName, parameter);
			}

			if (_callback != null)
			{
				_callback.Invoke();
			}
			VP_EventManager.TriggerEvent(VP_EventSetup.Audio.CLEAR_NULL_AUDIO);
		}
		/// <summary>
		/// Coroutine called for loading the next scene
		/// </summary>
		/// <param name="_index"></param>
		/// <param name="_delayAfterLoading"></param>
		/// <param name="_eventName"></param>
		/// <returns></returns>
		protected virtual IEnumerator<float> LoadingScreenWithParameter<T>(string _index, T parameter, float _delayAfterLoading = 1f, string _eventName = "", LoadSceneMode _mode = LoadSceneMode.Single, bool _blockInputs = true, bool _showLoadingScreen = true, bool _unblockInputs = true, UnityEngine.Events.UnityAction _callback = null)
		{
			InvokeOnStartLoad();

			if (_showLoadingScreen)
				ShowHideLoadingScreen(true);

			AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(_index, _mode);

			// Wait until the asynchronous scene fully loads
			while (!asyncLoad.isDone)
			{
				yield return Timing.WaitForOneFrame;
			}
			InvokeOnEndLoad();
			yield return Timing.WaitForSeconds(_delayAfterLoading);
			if (_showLoadingScreen)
				ShowHideLoadingScreen(false);

			if (!string.IsNullOrEmpty(_eventName))
			{
				//Debug.Log(_eventName + " is being called");

				VP_EventManager.TriggerEvent(_eventName, parameter);
			}

			if (_callback != null)
			{
				_callback.Invoke();
			}

     

			VP_EventManager.TriggerEvent(VP_EventSetup.Audio.CLEAR_NULL_AUDIO);
		}

		protected virtual IEnumerator<float> UnLoadingScreenWithParameter<T>(string _index, T parameter, float _delayAfterLoading = 1f, string _eventName = "", bool _blockInputs = true, bool _showLoadingScreen = true, bool _unblockInputs = true, UnityEngine.Events.UnityAction _callback = null)
		{
			InvokeOnStartLoad();

			if (_showLoadingScreen)
			{

				ShowHideLoadingScreen(true);
			}

			m_progress = 0f;

			AsyncOperation asyncLoad = SceneManager.UnloadSceneAsync(_index);

			// Wait until the asynchronous scene fully loads
			while (!asyncLoad.isDone)
			{
				m_progress = asyncLoad.progress;
				if (m_progressBar)
					m_progressBar.fillAmount = m_progress;
				yield return Timing.WaitForOneFrame;
			}
			InvokeOnEndLoad();
			yield return Timing.WaitForSeconds(_delayAfterLoading);

			if (_showLoadingScreen)
				ShowHideLoadingScreen(false);

			if (!string.IsNullOrEmpty(_eventName))
			{
				//  Debug.Log(_eventName + " is being called");
				VP_EventManager.TriggerEvent(_eventName, parameter);
			}

			if (_callback != null)
			{
				_callback.Invoke();
			}



			VP_EventManager.TriggerEvent(VP_EventSetup.Audio.CLEAR_NULL_AUDIO);
		}
		/// <summary>
		/// Coroutine called for loading the next scene
		/// </summary>
		/// <param name="_index"></param>
		/// <param name="_delayAfterLoading"></param>
		/// <param name="_eventName"></param>
		/// <returns></returns>
		protected virtual IEnumerator<float> LoadingScreen(Scene _scene, float _delayAfterLoading = 1f, string _eventName = "", LoadSceneMode _mode = LoadSceneMode.Single, bool _blockInputs = true, bool _showLoadingScreen = true, bool _unblockInputs = true, UnityEngine.Events.UnityAction _callback = null)
		{
			InvokeOnStartLoad();

			if (_showLoadingScreen)
				ShowHideLoadingScreen(true);
			AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(_scene.buildIndex, _mode);
			m_progress = 0f;

			// Wait until the asynchronous scene fully loads
			while (!asyncLoad.isDone)
			{
				m_progress = asyncLoad.progress;
				if (m_progressBar)
					m_progressBar.fillAmount = m_progress;
				yield return Timing.WaitForOneFrame;
			}
			InvokeOnEndLoad();
			yield return Timing.WaitForSeconds(_delayAfterLoading);

			if (_showLoadingScreen)
				ShowHideLoadingScreen(false);

			if (!string.IsNullOrEmpty(_eventName))
			{
				VP_EventManager.TriggerEvent(_eventName);
			}

			if (_callback != null)
			{
				_callback.Invoke();
			}
            
    

			VP_EventManager.TriggerEvent(VP_EventSetup.Audio.CLEAR_NULL_AUDIO);
		}


		/// <summary>
		/// Coroutine called for loading the next scene
		/// </summary>
		/// <param name="_index"></param>
		/// <param name="_delayAfterLoading"></param>
		/// <param name="_eventName"></param>
		/// <returns></returns>
		protected virtual IEnumerator<float> LoadingScreen(int _index, float _delayAfterLoading = 1f, string _eventName = "", LoadSceneMode _mode = LoadSceneMode.Single, bool _blockInputs = true, bool _showLoadingScreen = true, bool _unblockInputs = true, UnityEngine.Events.UnityAction _callback = null)
		{
			InvokeOnStartLoad();

			if (_showLoadingScreen)
				ShowHideLoadingScreen(true);
			AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(_index, _mode);
			m_progress = 0f;

			// Wait until the asynchronous scene fully loads
			while (!asyncLoad.isDone)
			{
				m_progress = asyncLoad.progress;
				if (m_progressBar)
					m_progressBar.fillAmount = m_progress;
				yield return Timing.WaitForOneFrame;
			}
			InvokeOnEndLoad();
			yield return Timing.WaitForSeconds(_delayAfterLoading);

			if (_showLoadingScreen)
				ShowHideLoadingScreen(false);

			if (!string.IsNullOrEmpty(_eventName))
			{
				VP_EventManager.TriggerEvent(_eventName);
			}

			if (_callback != null)
			{
				_callback.Invoke();
			}
            
  

			VP_EventManager.TriggerEvent(VP_EventSetup.Audio.CLEAR_NULL_AUDIO);
		}

		protected virtual IEnumerator<float> LoadingScreen(string _index, float _delayAfterLoading = 1f, string _eventName = "", LoadSceneMode _mode = LoadSceneMode.Single, bool _blockInputs = true, bool _showLoadingScreen = true, bool _unblockInputs = true, UnityEngine.Events.UnityAction _callback = null)
		{
			InvokeOnStartLoad();

			if (_showLoadingScreen)
				ShowHideLoadingScreen(true);
			AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(_index, _mode);
			m_progress = 0f;


			// Wait until the asynchronous scene fully loads
			while (!asyncLoad.isDone)
			{
				m_progress = asyncLoad.progress;
				if (m_progressBar)
					m_progressBar.fillAmount = m_progress;
				yield return Timing.WaitForOneFrame;
			}
			InvokeOnEndLoad();
			yield return Timing.WaitForSeconds(_delayAfterLoading);

			if (_showLoadingScreen)
				ShowHideLoadingScreen(false);


			if (!string.IsNullOrEmpty(_eventName))
			{
				VP_EventManager.TriggerEvent(_eventName);
			}

			if (_callback != null)
			{
				_callback.Invoke();
			}



			VP_EventManager.TriggerEvent(VP_EventSetup.Audio.CLEAR_NULL_AUDIO);
		}

		protected virtual IEnumerator<float> UnLoadingScreen(int _index, float _delayAfterLoading = 1f, string _eventName = "", bool _blockInputs = true, bool _showLoadingScreen = true, bool _unblockInputs = true, UnityEngine.Events.UnityAction _callback = null)
		{
			InvokeOnStartLoad();

			if (_showLoadingScreen)
				ShowHideLoadingScreen(false);


			AsyncOperation asyncLoad = SceneManager.UnloadSceneAsync(_index);
			m_progress = 0f;

			// Wait until the asynchronous scene fully loads
			while (!asyncLoad.isDone)
			{
				m_progress = asyncLoad.progress;
				if (m_progressBar)
					m_progressBar.fillAmount = m_progress;
				yield return Timing.WaitForOneFrame;
			}

			InvokeOnEndLoad();

			yield return Timing.WaitForSeconds(_delayAfterLoading);

			if (_showLoadingScreen)
				ShowHideLoadingScreen(false);


			if (!string.IsNullOrEmpty(_eventName))
			{
				VP_EventManager.TriggerEvent(_eventName);
			}

			if (_callback != null)
			{
				_callback.Invoke();
			}

     

			VP_EventManager.TriggerEvent(VP_EventSetup.Audio.CLEAR_NULL_AUDIO);
		}

		protected virtual IEnumerator<float> UnLoadingScreen(string _index, float _delayAfterLoading = 1f, string _eventName = "", bool _blockInputs = true, bool _showLoadingScreen = true, bool _unblockInputs = true, UnityEngine.Events.UnityAction _callback = null)
		{
			InvokeOnStartLoad();

			if (_showLoadingScreen)
				ShowHideLoadingScreen(false);


			AsyncOperation asyncLoad = SceneManager.UnloadSceneAsync(_index);
			m_progress = 0f;

			// Wait until the asynchronous scene fully loads
			while (!asyncLoad.isDone)
			{
				m_progress = asyncLoad.progress;
				if (m_progressBar)
					m_progressBar.fillAmount = m_progress;
				yield return Timing.WaitForOneFrame;
			}

			InvokeOnEndLoad();

			yield return Timing.WaitForSeconds(_delayAfterLoading);

			if (_showLoadingScreen)
				ShowHideLoadingScreen(false);


			if (!string.IsNullOrEmpty(_eventName))
			{
				VP_EventManager.TriggerEvent(_eventName);
			}
			if (_callback != null)
			{
				_callback.Invoke();
			}
          
			VP_EventManager.TriggerEvent(VP_EventSetup.Audio.CLEAR_NULL_AUDIO);
		}

		protected virtual IEnumerator<float> UnLoadingScreenWithCallback(string _index, UnityEngine.Events.UnityAction _callback = null, float _delayAfterLoading = 1f, string _eventName = "", bool _blockInputs = true, bool _showLoadingScreen = true, bool _unblockInputs = true, UnityEngine.Events.UnityAction _seccallback = null)
		{
			InvokeOnStartLoad();

			if (_showLoadingScreen)
				ShowHideLoadingScreen(false);


			AsyncOperation asyncLoad = SceneManager.UnloadSceneAsync(_index);
			m_progress = 0f;

			// Wait until the asynchronous scene fully loads
			while (!asyncLoad.isDone)
			{
				m_progress = asyncLoad.progress;
				if (m_progressBar)
					m_progressBar.fillAmount = m_progress;

				yield return Timing.WaitForOneFrame;
			}

			InvokeOnEndLoad();

			if (_delayAfterLoading > 0)
				yield return Timing.WaitForSeconds(_delayAfterLoading);

			if (_showLoadingScreen)
				ShowHideLoadingScreen(false);


			if (!string.IsNullOrEmpty(_eventName))
			{
				VP_EventManager.TriggerEvent(_eventName);
			}

			if (_callback != null)
				_callback.Invoke();

			if (_seccallback != null)
				_seccallback.Invoke();

			VP_EventManager.TriggerEvent(VP_EventSetup.Audio.CLEAR_NULL_AUDIO);
		}

#else

		protected virtual IEnumerator WaitTime(float _time, UnityEngine.Events.UnityAction _callback = null)
		{
			float timer = 0f;
			m_progress = 0f;

			while (timer < _time)
			{

				timer += Time.deltaTime;

				m_progress = timer * 100 / _time;

				if (m_progressBar)
					m_progressBar.fillAmount = m_progress;

				yield return null;

			}

			if (_callback != null)
				_callback.Invoke();
		}

		public virtual void LoadCurrentSceneAgain(float _delayAfterLoading, bool _blockInputs, bool _showLoadingScreen, bool _unblockInputs = true, UnityEngine.Events.UnityAction _callback = null)
		{
#if USE_MORE_EFFECTIVE_COROUTINES
		m_loadCoroutine = Timing.RunCoroutine(LoadingScreen(SceneManager.GetActiveScene().buildIndex, _delayAfterLoading, VP_EventSetup.Scene.RELOAD_CURRENT_SCENE, LoadSceneMode.Single, _blockInputs, _showLoadingScreen, _unblockInputs, _callback));
#else
		m_loadCoroutine = StartCoroutine(LoadingScreen(SceneManager.GetActiveScene().buildIndex, _delayAfterLoading, VP_EventSetup.Scene.RELOAD_CURRENT_SCENE, LoadSceneMode.Single, _blockInputs, _showLoadingScreen, _unblockInputs, _callback));

#endif
		}

		/// <summary>
		/// Coroutine called for loading the next scene
		/// </summary>
		/// <param name="_index"></param>
		/// <param name="_delayAfterLoading"></param>
		/// <param name="_eventName"></param>
		/// <returns></returns>
		protected virtual IEnumerator LoadingSceneWithCallback(string _scene, UnityEngine.Events.UnityAction _callback, float _delayAfterLoading = 1f, LoadSceneMode _mode = LoadSceneMode.Single, bool _blockInputs = true, bool _showLoadingScreen = true, bool _unblockInputs = true, UnityEngine.Events.UnityAction _seccallback = null)
		{
			InvokeOnStartLoad();

			if (_showLoadingScreen)
				ShowHideLoadingScreen(true);

			Application.backgroundLoadingPriority = ThreadPriority.Low;

			AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(_scene, _mode);
			m_progress = 0f;

			// Wait until the asynchronous scene fully loads
			while (!asyncLoad.isDone)
			{
				m_progress = asyncLoad.progress;
				if (m_progressBar)
					m_progressBar.fillAmount = m_progress;

				yield return null;
			}
			InvokeOnEndLoad();
			if (_delayAfterLoading > 0)
				yield return new WaitForSeconds(_delayAfterLoading);

			if (_showLoadingScreen)
				ShowHideLoadingScreen(false);


			if (_callback != null)
				_callback.Invoke();

			if (_seccallback != null)
				_seccallback.Invoke();

			VP_EventManager.TriggerEvent(VP_EventSetup.Audio.CLEAR_NULL_AUDIO);
		}

		/// <summary>
		/// Coroutine called for loading the next scene
		/// </summary>
		/// <param name="_index"></param>
		/// <param name="_delayAfterLoading"></param>
		/// <param name="_eventName"></param>
		/// <returns></returns>
		protected virtual IEnumerator LoadingScreenWithParameter<T>(int _index, T parameter, float _delayAfterLoading = 1f, string _eventName = "", LoadSceneMode _mode = LoadSceneMode.Single, bool _blockInputs = true, bool _showLoadingScreen = true, bool _unblockInputs = true, UnityEngine.Events.UnityAction _callback = null)
		{
			InvokeOnStartLoad();

			if (_showLoadingScreen)
				ShowHideLoadingScreen(true);

			AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(_index, _mode);
			m_progress = 0f;

			// Wait until the asynchronous scene fully loads
			while (!asyncLoad.isDone)
			{
				m_progress = asyncLoad.progress;
				if (m_progressBar)
					m_progressBar.fillAmount = m_progress;
				yield return null;
			}
			InvokeOnEndLoad();
			yield return new WaitForSeconds(_delayAfterLoading);
			if (_showLoadingScreen)
				ShowHideLoadingScreen(false);

			if (!string.IsNullOrEmpty(_eventName))
			{
				//Debug.Log(_eventName + " is being called");
				VP_EventManager.TriggerEvent(_eventName, parameter);
			}

			if (_callback != null)
			{
				_callback.Invoke();
			}

			VP_EventManager.TriggerEvent(VP_EventSetup.Audio.CLEAR_NULL_AUDIO);
		}

		protected virtual IEnumerator UnLoadingScreenWithParameter<T>(int _index, T parameter, float _delayAfterLoading = 1f, string _eventName = "", bool _blockInputs = true, bool _showLoadingScreen = true, bool _unblockInputs = true, UnityEngine.Events.UnityAction _callback = null)
		{
			InvokeOnStartLoad();

			if (_showLoadingScreen)
			{
				ShowHideLoadingScreen(true);
			}

			m_progress = 0f;

			AsyncOperation asyncLoad = SceneManager.UnloadSceneAsync(_index);

			// Wait until the asynchronous scene fully loads
			while (!asyncLoad.isDone)
			{
				m_progress = asyncLoad.progress;
				if (m_progressBar)
				m_progressBar.fillAmount = m_progress;
				yield return null;
			}

			InvokeOnEndLoad();
			yield return new WaitForSeconds(_delayAfterLoading);

			if (_showLoadingScreen)
				ShowHideLoadingScreen(false);

			if (!string.IsNullOrEmpty(_eventName))
			{
				//  Debug.Log(_eventName + " is being called");
				VP_EventManager.TriggerEvent(_eventName, parameter);
			}

			if (_callback != null)
			{
				_callback.Invoke();
			}
			VP_EventManager.TriggerEvent(VP_EventSetup.Audio.CLEAR_NULL_AUDIO);
		}
		/// <summary>
		/// Coroutine called for loading the next scene
		/// </summary>
		/// <param name="_index"></param>
		/// <param name="_delayAfterLoading"></param>
		/// <param name="_eventName"></param>
		/// <returns></returns>
		protected virtual IEnumerator LoadingScreenWithParameter<T>(string _index, T parameter, float _delayAfterLoading = 1f, string _eventName = "", LoadSceneMode _mode = LoadSceneMode.Single, bool _blockInputs = true, bool _showLoadingScreen = true, bool _unblockInputs = true, UnityEngine.Events.UnityAction _callback = null)
		{
			InvokeOnStartLoad();

			if (_showLoadingScreen)
				ShowHideLoadingScreen(true);

			AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(_index, _mode);

			// Wait until the asynchronous scene fully loads
			while (!asyncLoad.isDone)
			{
				m_progress = asyncLoad.progress;
				if (m_progressBar)
					m_progressBar.fillAmount = m_progress;

				yield return null;
			}
			InvokeOnEndLoad();
			yield return new WaitForSeconds(_delayAfterLoading);
			if (_showLoadingScreen)
				ShowHideLoadingScreen(false);

			if (!string.IsNullOrEmpty(_eventName))
			{
				//Debug.Log(_eventName + " is being called");
				VP_EventManager.TriggerEvent(_eventName, parameter);
			}

			if (_callback != null)
			{
				_callback.Invoke();
			}

			VP_EventManager.TriggerEvent(VP_EventSetup.Audio.CLEAR_NULL_AUDIO);
		}

		protected virtual IEnumerator UnLoadingScreenWithParameter<T>(string _index, T parameter, float _delayAfterLoading = 1f, string _eventName = "", bool _blockInputs = true, bool _showLoadingScreen = true, bool _unblockInputs = true, UnityEngine.Events.UnityAction _callback = null)
		{
		InvokeOnStartLoad();

		if (_showLoadingScreen)
		{

		ShowHideLoadingScreen(true);
		}

		m_progress = 0f;

		AsyncOperation asyncLoad = SceneManager.UnloadSceneAsync(_index);

		// Wait until the asynchronous scene fully loads
		while (!asyncLoad.isDone)
		{
		m_progress = asyncLoad.progress;
		if (m_progressBar)
		m_progressBar.fillAmount = m_progress;
		yield return null;
		}
		InvokeOnEndLoad();
		yield return new WaitForSeconds(_delayAfterLoading);

		if (_showLoadingScreen)
		ShowHideLoadingScreen(false);

		if (!string.IsNullOrEmpty(_eventName))
		{
		//  Debug.Log(_eventName + " is being called");
		VP_EventManager.TriggerEvent(_eventName, parameter);
		}

		if (_callback != null)
		{
		_callback.Invoke();
		}



		VP_EventManager.TriggerEvent(VP_EventSetup.Audio.CLEAR_NULL_AUDIO);
		}
		/// <summary>
		/// Coroutine called for loading the next scene
		/// </summary>
		/// <param name="_index"></param>
		/// <param name="_delayAfterLoading"></param>
		/// <param name="_eventName"></param>
		/// <returns></returns>
		protected virtual IEnumerator LoadingScreen(Scene _scene, float _delayAfterLoading = 1f, string _eventName = "", LoadSceneMode _mode = LoadSceneMode.Single, bool _blockInputs = true, bool _showLoadingScreen = true, bool _unblockInputs = true, UnityEngine.Events.UnityAction _callback = null)
		{
		InvokeOnStartLoad();

		if (_showLoadingScreen)
		ShowHideLoadingScreen(true);
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(_scene.buildIndex, _mode);
		m_progress = 0f;

		// Wait until the asynchronous scene fully loads
		while (!asyncLoad.isDone)
		{
		m_progress = asyncLoad.progress;
		if (m_progressBar)
		m_progressBar.fillAmount = m_progress;
		yield return null;
		}
		InvokeOnEndLoad();
		yield return new WaitForSeconds(_delayAfterLoading);

		if (_showLoadingScreen)
		ShowHideLoadingScreen(false);

		if (!string.IsNullOrEmpty(_eventName))
		{
		VP_EventManager.TriggerEvent(_eventName);
		}

		if (_callback != null)
		{
		_callback.Invoke();
		}



		VP_EventManager.TriggerEvent(VP_EventSetup.Audio.CLEAR_NULL_AUDIO);
		}


		/// <summary>
		/// Coroutine called for loading the next scene
		/// </summary>
		/// <param name="_index"></param>
		/// <param name="_delayAfterLoading"></param>
		/// <param name="_eventName"></param>
		/// <returns></returns>
		protected virtual IEnumerator LoadingScreen(int _index, float _delayAfterLoading = 1f, string _eventName = "", LoadSceneMode _mode = LoadSceneMode.Single, bool _blockInputs = true, bool _showLoadingScreen = true, bool _unblockInputs = true, UnityEngine.Events.UnityAction _callback = null)
		{
		InvokeOnStartLoad();

		if (_showLoadingScreen)
		ShowHideLoadingScreen(true);
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(_index, _mode);
		m_progress = 0f;

		// Wait until the asynchronous scene fully loads
		while (!asyncLoad.isDone)
		{
		m_progress = asyncLoad.progress;
		if (m_progressBar)
		m_progressBar.fillAmount = m_progress;
		yield return null;
		}
		InvokeOnEndLoad();
		yield return new WaitForSeconds(_delayAfterLoading);

		if (_showLoadingScreen)
		ShowHideLoadingScreen(false);

		if (!string.IsNullOrEmpty(_eventName))
		{
		VP_EventManager.TriggerEvent(_eventName);
		}

		if (_callback != null)
		{
		_callback.Invoke();
		}



		VP_EventManager.TriggerEvent(VP_EventSetup.Audio.CLEAR_NULL_AUDIO);
		}

		protected virtual IEnumerator LoadingScreen(string _index, float _delayAfterLoading = 1f, string _eventName = "", LoadSceneMode _mode = LoadSceneMode.Single, bool _blockInputs = true, bool _showLoadingScreen = true, bool _unblockInputs = true, UnityEngine.Events.UnityAction _callback = null)
		{
		InvokeOnStartLoad();

		if (_showLoadingScreen)
		ShowHideLoadingScreen(true);
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(_index, _mode);
		m_progress = 0f;


		// Wait until the asynchronous scene fully loads
		while (!asyncLoad.isDone)
		{
		m_progress = asyncLoad.progress;
		if (m_progressBar)
		m_progressBar.fillAmount = m_progress;
		yield return null;
		}
		InvokeOnEndLoad();
		yield return new WaitForSeconds(_delayAfterLoading);

		if (_showLoadingScreen)
		ShowHideLoadingScreen(false);


		if (!string.IsNullOrEmpty(_eventName))
		{
		VP_EventManager.TriggerEvent(_eventName);
		}

		if (_callback != null)
		{
		_callback.Invoke();
		}



		VP_EventManager.TriggerEvent(VP_EventSetup.Audio.CLEAR_NULL_AUDIO);
		}

		protected virtual IEnumerator UnLoadingScreen(int _index, float _delayAfterLoading = 1f, string _eventName = "", bool _blockInputs = true, bool _showLoadingScreen = true, bool _unblockInputs = true, UnityEngine.Events.UnityAction _callback = null)
		{
		InvokeOnStartLoad();

		if (_showLoadingScreen)
		ShowHideLoadingScreen(false);


		AsyncOperation asyncLoad = SceneManager.UnloadSceneAsync(_index);
		m_progress = 0f;

		// Wait until the asynchronous scene fully loads
		while (!asyncLoad.isDone)
		{
		m_progress = asyncLoad.progress;
		if (m_progressBar)
		m_progressBar.fillAmount = m_progress;
		yield return null;
		}

		InvokeOnEndLoad();

		yield return new WaitForSeconds(_delayAfterLoading);

		if (_showLoadingScreen)
		ShowHideLoadingScreen(false);


		if (!string.IsNullOrEmpty(_eventName))
		{
		VP_EventManager.TriggerEvent(_eventName);
		}

		if (_callback != null)
		{
		_callback.Invoke();
		}



		VP_EventManager.TriggerEvent(VP_EventSetup.Audio.CLEAR_NULL_AUDIO);
		}

		protected virtual IEnumerator UnLoadingScreen(string _index, float _delayAfterLoading = 1f, string _eventName = "", bool _blockInputs = true, bool _showLoadingScreen = true, bool _unblockInputs = true, UnityEngine.Events.UnityAction _callback = null)
		{
		InvokeOnStartLoad();

		if (_showLoadingScreen)
		ShowHideLoadingScreen(false);


		AsyncOperation asyncLoad = SceneManager.UnloadSceneAsync(_index);
		m_progress = 0f;

		// Wait until the asynchronous scene fully loads
		while (!asyncLoad.isDone)
		{
		m_progress = asyncLoad.progress;
		if (m_progressBar)
		m_progressBar.fillAmount = m_progress;
		yield return null;
		}

		InvokeOnEndLoad();

		yield return new WaitForSeconds(_delayAfterLoading);

		if (_showLoadingScreen)
		ShowHideLoadingScreen(false);


		if (!string.IsNullOrEmpty(_eventName))
		{
		VP_EventManager.TriggerEvent(_eventName);
		}
		if (_callback != null)
		{
		_callback.Invoke();
		}

		VP_EventManager.TriggerEvent(VP_EventSetup.Audio.CLEAR_NULL_AUDIO);
		}

		protected virtual IEnumerator UnLoadingScreenWithCallback(string _index, UnityEngine.Events.UnityAction _callback = null, float _delayAfterLoading = 1f, string _eventName = "", bool _blockInputs = true, bool _showLoadingScreen = true, bool _unblockInputs = true, UnityEngine.Events.UnityAction _seccallback = null)
		{
		InvokeOnStartLoad();

		if (_showLoadingScreen)
		ShowHideLoadingScreen(false);


		AsyncOperation asyncLoad = SceneManager.UnloadSceneAsync(_index);
		m_progress = 0f;

		// Wait until the asynchronous scene fully loads
		while (!asyncLoad.isDone)
		{
		m_progress = asyncLoad.progress;
		if (m_progressBar)
		m_progressBar.fillAmount = m_progress;

		yield return null;
		}

		InvokeOnEndLoad();

		if (_delayAfterLoading > 0)
		yield return new WaitForSeconds(_delayAfterLoading);

		if (_showLoadingScreen)
		ShowHideLoadingScreen(false);


		if (!string.IsNullOrEmpty(_eventName))
		{
		VP_EventManager.TriggerEvent(_eventName);
		}

		if (_callback != null)
		_callback.Invoke();

		if (_seccallback != null)
		_seccallback.Invoke();

		VP_EventManager.TriggerEvent(VP_EventSetup.Audio.CLEAR_NULL_AUDIO);
		}
#endif

		protected virtual void _HidePanel()
		{
			if (m_loadingGroup != null)
				m_loadingGroup.SetActive(false);
		}

		public static void LoadScene(int _sceneIndex, float _delayAfterLoading = 1f, string _eventName = "", bool _blockInputs = true, bool _showLoadingScreen = true, bool _unblockInputs = true, UnityEngine.Events.UnityAction _callback = null)
		{
			Instance?.LoadSceneAsync(_sceneIndex, _delayAfterLoading, _eventName, _blockInputs, _showLoadingScreen, _unblockInputs, _callback);
		}
		public static void LoadScene(string _scene, float _delayAfterLoading = 1f, string _eventName = "", bool _blockInputs = true, bool _showLoadingScreen = true, bool _unblockInputs = true, UnityEngine.Events.UnityAction _callback = null)
		{
			Instance?.LoadSceneAsync(_scene, _delayAfterLoading, _eventName, _blockInputs, _showLoadingScreen, _unblockInputs, _callback);
		}

		public static void LoadTrainerBattle<T>(string _scene, T parameter, float _delayAfterLoading = 1f, string _eventName = "", bool _blockInputs = true, bool _showLoadingScreen = true, bool _unblockInputs = true, UnityEngine.Events.UnityAction _callback = null)
		{
			Instance?.LoadSceneAsyncWithParameter(_scene, parameter, _delayAfterLoading, _eventName, _blockInputs, _showLoadingScreen, _unblockInputs, _callback);
		}
		public static void LoadFieldScene(string _scene, UnityEngine.Events.UnityAction _callback, float _delayAfterLoading = 1f, bool _blockInputs = true, bool _showLoadingScreen = true, bool _unblockInputs = true, UnityEngine.Events.UnityAction _seccallback = null)
		{
			Instance?.LoadFieldAsyncScene(_scene, _callback, _delayAfterLoading, _blockInputs, _showLoadingScreen, _unblockInputs, _seccallback);
		}

		public static void LoadSceneWithParameter<T>(string _scene, T parameter, float _delayAfterLoading = 1f, string _eventName = "", bool _blockInputs = true, bool _showLoadingScreen = true, bool _unblockInputs = true, UnityEngine.Events.UnityAction _callback = null)
		{
			Instance?.LoadSceneAsyncWithParameter(_scene, parameter, _delayAfterLoading, _eventName, _blockInputs, _showLoadingScreen, _unblockInputs, _callback);
		}

		public static void LoadAdditiveScene(string _scene, float _delayAfterLoading = 1f, string _eventName = "", bool _blockInputs = true, bool _showLoadingScreen = true, bool _unblockInputs = true, UnityEngine.Events.UnityAction _callback = null)
		{
			Instance?.LoadSceneAdditive(_scene, _delayAfterLoading, _eventName, _blockInputs, _showLoadingScreen, _unblockInputs, _callback);
		}

		public static void LoadSceneWithParameter<T>(int _sceneIndex, T parameter, float _delayAfterLoading = 1f, string _eventName = "", bool _blockInputs = true, bool _showLoadingScreen = true, bool _unblockInputs = true, UnityEngine.Events.UnityAction _callback = null)
		{
			Instance?.LoadSceneAsyncWithParameter(_sceneIndex, parameter, _delayAfterLoading, _eventName, _blockInputs, _showLoadingScreen, _unblockInputs, _callback);
		}

		public static void LoadAdditiveScene(int _sceneIndex, float _delayAfterLoading = 1f, string _eventName = "", bool _blockInputs = true, bool _showLoadingScreen = true, bool _unblockInputs = true, UnityEngine.Events.UnityAction _callback = null)
		{
			Instance?.LoadSceneAdditive(_sceneIndex, _delayAfterLoading, _eventName, _blockInputs, _showLoadingScreen, _unblockInputs, _callback);
		}

		public static void LoadSceneWithParameterAdditive<T>(int _sceneIndex, T parameter, float _delayAfterLoading = 1f, string _eventName = "", bool _blockInputs = true, bool _showLoadingScreen = true, bool _unblockInputs = true, UnityEngine.Events.UnityAction _callback = null)
		{
			Instance?.LoadSceneAdditiveWithParameter(_sceneIndex, parameter, _delayAfterLoading, _eventName, _blockInputs, _showLoadingScreen, _unblockInputs, _callback);
		}
		public static void LoadSceneWithParameterAdditive<T>(string _sceneIndex, T parameter, float _delayAfterLoading = 1f, string _eventName = "", bool _blockInputs = true, bool _showLoadingScreen = true, bool _unblockInputs = true, UnityEngine.Events.UnityAction _callback = null)
		{
			Instance?.LoadSceneAdditiveWithParameter(_sceneIndex, parameter, _delayAfterLoading, _eventName, _blockInputs, _showLoadingScreen, _unblockInputs, _callback);
		}

		public static void HidePanel()
		{
			Instance?._HidePanel();
		}

		public static void ReloadCurrentSceneAfterTime(float _timeToWait, float _delayAfterLoading = 1f, bool _blockInputs = true, bool _showLoadingScreen = true, bool _unblockInputs = true, UnityEngine.Events.UnityAction _callback = null)
        {
			Instance?.WaitTime(_timeToWait, () =>
			{
				Instance.LoadCurrentSceneAgain(_delayAfterLoading, _blockInputs, _showLoadingScreen, _unblockInputs, _callback);
			});
		}

		public static void ReloadCurrentScene(float _delayAfterLoading = 1f, bool _blockInputs = true, bool _showLoadingScreen = true, bool _unblockInputs = true, UnityEngine.Events.UnityAction _callback = null)
		{
			
				Instance?.LoadCurrentSceneAgain(_delayAfterLoading, _blockInputs, _showLoadingScreen, _unblockInputs, _callback);
		}
	}

}
