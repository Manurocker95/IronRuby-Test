#if USE_MORE_EFFECTIVE_COROUTINES
using MEC;
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VirtualPhenix.Save;
using VirtualPhenix.Fade;

namespace VirtualPhenix
{
	[DefaultExecutionOrder(VP_ExecutingOrderSetup.GAME_MANAGER)]
	public class VP_GameManager : VP_SingletonMonobehaviour<VP_GameManager>
    {
		[Header("Game Manager")]
		[SerializeField] protected GameObject m_canvas;
		[SerializeField] protected bool m_useImage = false;
		[SerializeField] protected CanvasGroup m_fadeImage;
		[SerializeField] protected bool m_useFadePostProcessing = true;
		public VP_FadePostProcess m_fadePP;

		[SerializeField] protected bool m_paused = false;
		[SerializeField] protected bool m_inGame = false;
		[SerializeField] protected bool m_canInteract = false;

#if !USE_MORE_EFFECTIVE_COROUTINES
		protected Coroutine m_fadeDownCoroutine;
		protected Coroutine m_fadeUpCoroutine;
#else
		protected CoroutineHandle m_fadeDownCoroutine;
		protected CoroutineHandle m_fadeUpCoroutine;
#endif

		public UnityEvent<bool> m_onGamePaused;

		public bool CanInteract { get { return m_canInteract; } set { m_canInteract = value; } }

		public virtual void InitGame()
        {
			m_inGame = true;
			m_paused = false;
			m_canInteract = true;
		}

        public virtual void EndGame()
        {

        }

		public virtual void PauseGame()
        {
			m_paused = !m_paused;
			m_onGamePaused.Invoke(m_paused);
        }

		public virtual void SetPause(bool _value)
		{
			m_paused = _value;
			m_onGamePaused.Invoke(m_paused);
		}

		protected override void StartAllListeners()
		{

			base.StartAllListeners();

			VP_EventManager.StartListening(VP_EventSetup.Init.AFTER_INIT, () => { m_inGame = false; });
			VP_EventManager.StartListening(VP_EventSetup.Game.START_GAME, InitGame);
		}

		protected override void StopAllListeners()
		{
			base.StopAllListeners();

			VP_EventManager.StopListening(VP_EventSetup.Init.AFTER_INIT, () => { m_inGame = false; });
			VP_EventManager.StopListening(VP_EventSetup.Game.START_GAME, InitGame);
		}


		protected virtual void Update()
		{

			if (!m_paused && m_canInteract)
			{
				if (Input.GetKeyDown(KeyCode.Escape))
				{
					CheckExit();
				}
			}

		}

		public virtual void ExitGameFromMenu()
		{
			FadeDownAfterUp(false, 1f, () =>
			{
#if UNITY_EDITOR
				UnityEditor.EditorApplication.isPlaying = false;
#else
				Application.Quit();
#endif
			});

		}

		public virtual void GoBackToMenu()
		{
			FadeDownAfterUp(false, 1f, () =>
			{
				VP_SceneManager.LoadScene(VP_SceneSetup.MENU, 0f, VP_EventSetup.Init.AFTER_INIT, true, true, true, ()=> { m_canInteract = true; m_paused = false; } );
			});
		}

		protected virtual void CheckExit()
		{
			m_canInteract = false;
			m_paused = true;
			if (!m_inGame)
			{
				VP_AlertManager.ShowConfirm("Do you really want to exit?", "Yes", "No", () =>
				{
					ExitGameFromMenu();
				}, () => { m_paused = false; m_canInteract = true; });
			}
			else
			{
				VP_AlertManager.ShowConfirm("Do you really want to go back to menu?", "Yes", "No", () =>
				{
					GoBackToMenu();
				}, () => { m_paused = false; m_canInteract = true; });
			}
		}

		protected virtual void SetEffectAlpha(float _alpha)
		{
			if (m_fadeImage)
				m_fadeImage.alpha = _alpha;
		}

#if USE_MORE_EFFECTIVE_COROUTINES
		/// <summary>
		/// Fades 'Down' (Displays the fade effect) after Up.
		/// </summary>
		public virtual void FadeDownAfterUp(bool instant = false, float effectDuration = 1f, System.Action onComplete = null, params CanvasGroup[] _canvasGroup)
		{
			if (!m_useFadePostProcessing)
			{
				FadeUp(true, 1f, () =>
				{
					Timing.KillCoroutines(m_fadeDownCoroutine);
					Timing.KillCoroutines(m_fadeUpCoroutine);
					if (instant)
					{
						SetEffectAlpha(1);
						if (onComplete != null)
							onComplete();
						return;
					}
					m_fadeDownCoroutine = Timing.RunCoroutine(FadeDownCoroutine(effectDuration, onComplete, _canvasGroup), "FadeDown");
				});
			}
			else
            {
				m_fadePP.FadeUp(true, () =>
				{
					m_fadePP.m_effectDuration = effectDuration;
					m_fadePP.FadeDown(instant, onComplete, _canvasGroup);
				}, _canvasGroup);

            }
		}

		/// <summary>
		/// Fades 'Down' (Displays the fade effect).
		/// </summary>
		public virtual void FadeDown(bool instant = false, float effectDuration = 1f, System.Action onComplete = null, params CanvasGroup[] _canvasGroup)
		{
			if (!m_useFadePostProcessing)
			{
				Timing.KillCoroutines(m_fadeDownCoroutine);
				Timing.KillCoroutines(m_fadeUpCoroutine);
				if (instant)
				{
					SetEffectAlpha(1);
					if (onComplete != null)
						onComplete();
					return;
				}
				m_fadeDownCoroutine = Timing.RunCoroutine(FadeDownCoroutine(effectDuration, onComplete, _canvasGroup), "FadeDown");
			}
			else
			{
				m_fadePP.m_effectDuration = effectDuration;
				m_fadePP.FadeDown(instant, onComplete, _canvasGroup);
			}

		}

		/// <summary>
		/// Fades 'Up' (Hides the fade effect).
		/// </summary>
		public virtual void FadeUpAfterDown(bool instant = false, float effectDuration = 1f, System.Action onComplete = null, params CanvasGroup[] _canvasGroup)
		{
			if (!m_useFadePostProcessing)
			{
				FadeDown(true, 1f, () =>
				{
					Timing.KillCoroutines(m_fadeUpCoroutine);
					Timing.KillCoroutines(m_fadeDownCoroutine);
					if (instant)
					{
						SetEffectAlpha(0);
						if (onComplete != null) onComplete();
						return;
					}
					m_fadeUpCoroutine = Timing.RunCoroutine(FadeUpCoroutine(effectDuration, onComplete, _canvasGroup), "FadeUp");
				});
			}
			else
			{
				m_fadePP.FadeDown(true, () =>
				{
					m_fadePP.m_effectDuration = effectDuration;
					m_fadePP.FadeUp(instant, onComplete, _canvasGroup);
				}, _canvasGroup);
			}
		}

		/// <summary>
		/// Fades 'Up' (Hides the fade effect).
		/// </summary>
		public virtual void FadeUp(bool instant = false, float effectDuration = 1f, System.Action onComplete = null, params CanvasGroup[] _canvasGroup)
		{
			if (!m_useFadePostProcessing)
			{

				Timing.KillCoroutines(m_fadeUpCoroutine);
				Timing.KillCoroutines(m_fadeDownCoroutine);
				if (instant)
				{
					SetEffectAlpha(0);
					if (onComplete != null) onComplete();
					return;
				}
				m_fadeUpCoroutine = Timing.RunCoroutine(FadeUpCoroutine(effectDuration, onComplete, _canvasGroup), "FadeUp");
			}
			else
			{
				m_fadePP.m_effectDuration = effectDuration;
				m_fadePP.FadeUp(instant, onComplete, _canvasGroup);
			}
		}

		protected virtual IEnumerator<float> FadeDownCoroutine(float effectDuration, System.Action onComplete, params CanvasGroup[] _canvasGroup)
		{
			float alpha = 0;
			float canvasalpha = 1;
			bool updateCanvas = _canvasGroup.Length > 0;
			m_canvas.SetActive(true);

			while (true)
			{
				alpha += Time.unscaledDeltaTime / effectDuration;
				canvasalpha -= Time.unscaledDeltaTime / effectDuration;

				CheckUpdateCanvas(alpha, updateCanvas, canvasalpha, _canvasGroup);

				if (alpha >= 1)
				{
					break;
				}

				yield return Timing.WaitForOneFrame;
			}

			SetEffectAlpha(1);
			m_canvas.SetActive(false);

			if (onComplete != null)
				onComplete.Invoke();
		}

		protected virtual IEnumerator<float> FadeUpCoroutine(float effectDuration, System.Action onComplete, params CanvasGroup[] _canvasGroup)
		{
			float alpha = 1; 
			float canvasalpha = 1;
			bool updateCanvas = _canvasGroup != null;
			m_canvas.SetActive(true);

			while (true)
			{
				alpha -= Time.unscaledDeltaTime / effectDuration;
				canvasalpha += Time.unscaledDeltaTime / effectDuration;

				CheckUpdateCanvas(alpha, updateCanvas, canvasalpha, _canvasGroup);

				if (alpha <= 0)
				{
					break;
				}

				yield return Timing.WaitForOneFrame;
			}

			SetEffectAlpha(0);
			m_canvas.SetActive(false);

			if (onComplete != null)
				onComplete.Invoke();
		}
#else
		/// <summary>
		/// Fades 'Down' (Displays the fade effect) after Up.
		/// </summary>
		public virtual void FadeDownAfterUp(bool instant = false, float effectDuration = 1f, System.Action onComplete = null, params CanvasGroup[] _canvasGroup)
		{
			if (!m_useFadePostProcessing)
            {
				FadeUp(true, 1f, () =>
				{
					StopCoroutine(m_fadeDownCoroutine);
					StopCoroutine(m_fadeUpCoroutine);
					if (instant)
					{
						SetEffectAlpha(1);
						if (onComplete != null)
							onComplete();
						return;
					}
					m_fadeDownCoroutine = StartCoroutine(FadeDownCoroutine(effectDuration, onComplete, _canvasGroup));
				}, _canvasGroup);
			}
			else
            {
				m_fadePP.FadeUp(true, () =>
				{
					m_fadePP.m_effectDuration = effectDuration;
					m_fadePP.FadeDown(instant, onComplete, _canvasGroup);
				}, _canvasGroup);

            }
		}

		/// <summary>
		/// Fades 'Down' (Displays the fade effect).
		/// </summary>
		public virtual void FadeDown(bool instant = false, float effectDuration = 1f, System.Action onComplete = null, params CanvasGroup[] _canvasGroup)
		{
			if (!m_useFadePostProcessing)
            {
				StopCoroutine(m_fadeDownCoroutine);
				StopCoroutine(m_fadeUpCoroutine);
				if (instant)
				{
					SetEffectAlpha(1);
					if (onComplete != null)
						onComplete();
					return;
				}
				m_fadeDownCoroutine = StartCoroutine(FadeDownCoroutine(effectDuration, onComplete, _canvasGroup));
			}
			else
            {
				m_fadePP.m_effectDuration = effectDuration;
				m_fadePP.FadeDown(instant, onComplete, _canvasGroup);
			}
		}

		/// <summary>
		/// Fades 'Up' (Hides the fade effect).
		/// </summary>
		public virtual void FadeUpAfterDown(bool instant = false, float effectDuration = 1f, System.Action onComplete = null, params CanvasGroup[] _canvasGroup)
		{
			if (!m_useFadePostProcessing)
            {
				FadeDown(true, 1f, () =>
				{
					StopCoroutine(m_fadeDownCoroutine);
					StopCoroutine(m_fadeUpCoroutine);
					if (instant)
					{
						SetEffectAlpha(0);
						if (onComplete != null) onComplete();
						return;
					}
					m_fadeUpCoroutine = StartCoroutine(FadeUpCoroutine(effectDuration, onComplete, _canvasGroup));
				}, _canvasGroup);
			}
			else
            {
				m_fadePP.FadeDown(true, () =>
				{
					m_fadePP.m_effectDuration = effectDuration;
					m_fadePP.FadeUp(instant, onComplete, _canvasGroup);
				}, _canvasGroup);
			}
				
		}

		/// <summary>
		/// Fades 'Up' (Hides the fade effect).
		/// </summary>
		public virtual void FadeUp(bool instant = false, float effectDuration = 1f, System.Action onComplete = null, params CanvasGroup[] _canvasGroup)
		{
			if (!m_useFadePostProcessing)
            {
				StopCoroutine(m_fadeDownCoroutine);
				StopCoroutine(m_fadeUpCoroutine);
				if (instant)
				{
					SetEffectAlpha(0);
					if (onComplete != null) onComplete();
					return;
				}
				m_fadeUpCoroutine = StartCoroutine(FadeUpCoroutine(effectDuration, onComplete, _canvasGroup));
			}
			else
            {
				m_fadePP.m_effectDuration = effectDuration;
				m_fadePP.FadeUp(instant, onComplete, _canvasGroup);
			}

		}

		protected virtual IEnumerator FadeDownCoroutine(float effectDuration, System.Action onComplete, params CanvasGroup[] _canvasGroup)
		{
			float alpha = 0;
			float canvasalpha = 1;
			bool updateCanvas = _canvasGroup.Length > 0;
			m_canvas.SetActive(true);

			while (true)
			{
				alpha += Time.unscaledDeltaTime / effectDuration;
				canvasalpha -= Time.unscaledDeltaTime / effectDuration;

				CheckUpdateCanvas(alpha, updateCanvas, canvasalpha, _canvasGroup);

				if (alpha >= 1)
				{
					break;
				}

				yield return new WaitForEndOfFrame();
			}

			SetEffectAlpha(1);
			m_canvas.SetActive(false);

			if (onComplete != null)
				onComplete.Invoke();
		}

		protected virtual IEnumerator FadeUpCoroutine(float effectDuration, System.Action onComplete, params CanvasGroup[] _canvasGroup)
		{
			float alpha = 1; 
			float canvasalpha = 1;
			bool updateCanvas = _canvasGroup != null;
			m_canvas.SetActive(true);

			while (true)
			{
				alpha -= Time.unscaledDeltaTime / effectDuration;
				canvasalpha += Time.unscaledDeltaTime / effectDuration;

				CheckUpdateCanvas(alpha, updateCanvas, canvasalpha, _canvasGroup);

				if (alpha <= 0)
				{
					break;
				}

				yield return new WaitForEndOfFrame();
			}

			SetEffectAlpha(0);
			m_canvas.SetActive(false);

			if (onComplete != null)
				onComplete.Invoke();
		}
#endif


		protected virtual void CheckUpdateCanvas(float _effectAlpha, bool _updateCanvas, float _canvasAlpha, params CanvasGroup[] _canvasGroup)
        {
			SetEffectAlpha(_effectAlpha);
			if (_updateCanvas)
			{
				foreach (CanvasGroup c in _canvasGroup)
				{
					if (c != null)
						c.alpha = _canvasAlpha;
				}
			}
		}
	}

}
