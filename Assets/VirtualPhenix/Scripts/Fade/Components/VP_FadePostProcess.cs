#if USE_MORE_EFFECTIVE_COROUTINES
using MEC;
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
#if PHOENIX_URP_BLIT_PASS
using VirtualPhenix.URP;
#endif
namespace VirtualPhenix.Fade 
{
	[ExecuteInEditMode]
	[ImageEffectAllowedInSceneView]
	[AddComponentMenu(VP_PrefixSetup.MAIN_PREFIX+"/Fade/Fade Post Process")]
	[DefaultExecutionOrder(VP_ExecutingOrderSetup.FADE_POST_PROCESS)]
	public class VP_FadePostProcess : VP_Monobehaviour 
	{
		//-----------------------------------------------------------------------------------------
		// Constants:
		//-----------------------------------------------------------------------------------------

		protected const string ALPHA = "_Alpha";
		protected const string DELTA = "_Delta";

		//-----------------------------------------------------------------------------------------
		// Delegates:
		//-----------------------------------------------------------------------------------------

		public delegate void FadeEvent();
		public delegate void EffectEvent(VP_FadeEffect effect);

		//-----------------------------------------------------------------------------------------
		// Events:
		//-----------------------------------------------------------------------------------------

		public event FadeEvent FadedUp;
		public event FadeEvent FadedDown;
		public event EffectEvent EffectChanged;

		//-----------------------------------------------------------------------------------------
		// Inspector Variables:
		//-----------------------------------------------------------------------------------------

		[Header("Effect")]
		[SerializeField] protected VirtualPhenix.ResourceReference.VP_FadeEffectResources m_fadeEffects;
		[SerializeField] protected VP_FadeEffect m_defaultFadeEffect;

		public float m_effectDuration = 1;

		[SerializeField] protected AnimationCurve m_effectEasing;

		[Header("Events")]
		[SerializeField] protected UnityEvent m_fadedUpUnityEvent;

		[SerializeField] protected UnityEvent m_fadedDownUnityEvent;

		[Header("GamePlay")]
		[SerializeField] protected bool m_setAutomaticallyToGM = false;
		[SerializeField] protected bool m_killPrevious = false;

		[Header("Developer")]
		[SerializeField] protected bool m_preview;
		[SerializeField] protected float m_previewAlphaPercentage;

		//-----------------------------------------------------------------------------------------
		// Backing Fields:
		//-----------------------------------------------------------------------------------------

		protected VP_FadeEffect m_currentEffect;

		//-----------------------------------------------------------------------------------------
		// Private Fields:
		//-----------------------------------------------------------------------------------------

		protected Material m_currentEffectMaterial;

#if !USE_MORE_EFFECTIVE_COROUTINES
		protected Coroutine m_fadeUpCoroutine;
		protected Coroutine m_fadeDownCoroutine;
#else
		protected CoroutineHandle m_fadeUpCoroutine;
		protected CoroutineHandle m_fadeDownCoroutine;
#endif
		//-----------------------------------------------------------------------------------------
		// Public Properties:
		//-----------------------------------------------------------------------------------------


		public virtual VP_FadeEffect CurrentEffect 
		{
			get 
			{ 
				return m_currentEffect; 
			}
			private set 
			{
				if (m_currentEffect == value)
				{
					return;
				}
				m_currentEffect = value;
				if (EffectChanged != null)
                {
					EffectChanged(m_currentEffect);
				}
				if (m_currentEffect == null) 
				{
					m_currentEffectMaterial = null;
					return;
				}
				m_currentEffectMaterial = m_currentEffect.BaseMaterial;
			}
		}

		public float EffectDuration
        {
            get
            {
                return m_effectDuration;
            }
        }

		public void AssignEffectFromResourceLibrary(string _key)
        {
			if (m_fadeEffects != null)
            {
				if (m_fadeEffects.TryGetResource(_key, out VP_FadeEffect effect))
                {
					AssignEffect(effect);
                }
			}
        }

		protected virtual void Reset()
        {
			m_startListeningTime = StartListenTime.OnInitialization;
			m_stopListeningTime = StopListenTime.OnDestroy;
			m_fadeEffects = Resources.Load<VirtualPhenix.ResourceReference.VP_FadeEffectResources>("Database/Referencer/Fade Referencer");
        }

		//-----------------------------------------------------------------------------------------
		// Unity Lifecycle:
		//-----------------------------------------------------------------------------------------

		protected override void Initialize() 
		{
			base.Initialize();

			if (m_defaultFadeEffect != null) 
				AssignEffect(m_defaultFadeEffect);

			if (m_setAutomaticallyToGM)
            {
				if (VP_GameManager.TryGetInstance(out VP_GameManager gm))
                {
					gm.m_fadePP = this;
                }
			}
		}

        protected override void StartAllListeners()
        {
            base.StartAllListeners();

			
			EffectChanged += ChangeURPPass;
		}

        protected override void StopAllListeners()
        {
            base.StopAllListeners();

			EffectChanged -= ChangeURPPass;

		}

		public void ListenToEffectChanged(EffectEvent _callback)
        {
			if (EffectChanged == null)
            {
				EffectChanged = _callback;
            }
			else
            {
				EffectChanged += _callback;
			}
        }

		public void StopListenToEffectChanged(EffectEvent _callback)
		{
			if (EffectChanged == null)
			{
				return;
			}
			else
			{
				EffectChanged -= _callback;
			}
		}

		public void SetCurrentURPPass()
        {
#if PHOENIX_URP_BLIT_PASS && USE_FADE
			VP_Utils.RenderPipelineUtils.SetFadeEffectToURPPass(m_currentEffect);
#endif
		}

		public void ChangeURPPass(VP_FadeEffect _newEffect)
        {
#if PHOENIX_URP_BLIT_PASS && USE_FADE
			VP_Utils.RenderPipelineUtils.SetFadeEffectToURPPass(_newEffect);
#endif
		}

		protected virtual void OnRenderImage(RenderTexture source, RenderTexture destination) 
		{
			if (m_currentEffectMaterial == null || (!Application.isPlaying && !m_preview) || GetEffectAlpha() == 0) 
			{
				Graphics.Blit(source, destination);
				return;
			}

			Graphics.Blit(source, destination, m_currentEffectMaterial);
		}

		protected virtual void Update() 
		{
			if (!Application.isPlaying) 
			{
				if (CurrentEffect != m_defaultFadeEffect)
                {
					AssignEffect(m_defaultFadeEffect);
				}
				if (m_preview) 
				{
					SetEffectAlpha(m_previewAlphaPercentage * 0.01f);
				}
			}
		}

		public virtual void AssignEffect(VP_FadeEffect fadeEffect) 
		{
			float currentAlpha = GetEffectAlpha();
			CurrentEffect = fadeEffect;
			SetEffectAlpha(currentAlpha);
		}

		//-----------------------------------------------------------------------------------------
		// Public Methods:
		//-----------------------------------------------------------------------------------------

		/// <summary>
		/// Fade down then back up again immediately.
		/// </summary>
		public virtual void Dip() 
		{
#if !USE_MORE_EFFECTIVE_COROUTINES
			if (m_killPrevious)
			StopAllCoroutines();
				
			m_fadeUpCoroutine = StartCoroutine(FadeUpCoroutine(() => FadeUp()));
#else
			if (m_killPrevious)
			{
				Timing.KillCoroutines(m_fadeUpCoroutine);
				Timing.KillCoroutines(m_fadeDownCoroutine);
			}

			Timing.RunCoroutine(FadeUpCoroutine(() => FadeUp()));
#endif
		}

		/// <summary>
		/// Fades 'Down' (Displays the fade effect).
		/// </summary>
		public virtual void FadeDown(bool instant = false, Action onComplete=null, params CanvasGroup[] _canvasGroup)
        {
#if !USE_MORE_EFFECTIVE_COROUTINES
	        if (m_killPrevious)
				StopAllCoroutines();
#else
	        if (m_killPrevious)
	        {
		        Timing.KillCoroutines(m_fadeUpCoroutine);
		        Timing.KillCoroutines(m_fadeDownCoroutine);
	        }

#endif
			if (instant) 
			{
				SetEffectAlpha(1);
				
				foreach (CanvasGroup cg in _canvasGroup)
					cg.alpha = 0f;
				
				if (onComplete != null) onComplete();
				return;
			}
#if !USE_MORE_EFFECTIVE_COROUTINES
			m_fadeDownCoroutine = StartCoroutine(FadeDownCoroutine(onComplete, _canvasGroup));
#else
			m_fadeDownCoroutine = Timing.RunCoroutine(FadeDownCoroutine(onComplete, _canvasGroup));
#endif
		}

		/// <summary>
		/// Fades 'Up' (Hides the fade effect).
		/// </summary>
		public virtual void FadeUp(bool instant = false, Action onComplete = null, params CanvasGroup[] _canvasGroup) {
#if !USE_MORE_EFFECTIVE_COROUTINES
			if (m_killPrevious)
				StopAllCoroutines();
#else
			if (m_killPrevious)
			{
				Timing.KillCoroutines(m_fadeUpCoroutine);
				Timing.KillCoroutines(m_fadeDownCoroutine);
			}
#endif
			if (instant) 
			{
				SetEffectAlpha(0);
				
				foreach (CanvasGroup cg in _canvasGroup)
					cg.alpha = 1f;
				
				if (onComplete != null) onComplete();
				return;
			}
#if !USE_MORE_EFFECTIVE_COROUTINES
			m_fadeUpCoroutine = StartCoroutine(FadeUpCoroutine(onComplete, _canvasGroup));
#else
			m_fadeUpCoroutine = Timing.RunCoroutine(FadeUpCoroutine(onComplete, _canvasGroup));
#endif
		}

		//-----------------------------------------------------------------------------------------
		// Protected Methods:
		//-----------------------------------------------------------------------------------------

		protected virtual void InvokeFadedUp() 
		{
			if (FadedUp != null) 
				FadedUp.Invoke();

			m_fadedUpUnityEvent.Invoke();
		}

		protected virtual void InvokeFadedDown() 
		{
			if (FadedDown != null) 
				FadedDown.Invoke();

			m_fadedDownUnityEvent.Invoke();
		}

		public virtual float GetEffectAlpha() 
		{
			if (m_currentEffectMaterial == null) 
				return 0;

			return m_currentEffectMaterial.GetFloat(ALPHA);
		}

		public virtual void SetEffectAlpha(float alpha) 
		{
			if (m_currentEffectMaterial == null) 
				return;

			m_currentEffectMaterial.SetFloat(ALPHA, alpha);

			float delta = m_effectEasing == null ? alpha : m_effectEasing.Evaluate(alpha);

			m_currentEffectMaterial.SetFloat(DELTA, delta);
		}

		//-----------------------------------------------------------------------------------------
		// Private Methods:
		//-----------------------------------------------------------------------------------------

#if !USE_MORE_EFFECTIVE_COROUTINES
		protected virtual IEnumerator FadeDownCoroutine(Action onComplete, params CanvasGroup[] _canvasGroup)
		{
			float alpha = 0; //GetEffectAlpha();
			float canvasalpha = 1; 
            bool updateCanvas = _canvasGroup.Length > 0;


            while (true) {
				float step = Time.unscaledDeltaTime / m_effectDuration;
				alpha += step;
				canvasalpha -= step;
				
				if (alpha >= 1) {
					alpha = 1;
				}

                if (updateCanvas)
                {
					foreach (CanvasGroup cg in _canvasGroup)
						cg.alpha = canvasalpha;
				}


                SetEffectAlpha(alpha);

				if (alpha == 1) {
					yield return new WaitForEndOfFrame();

					InvokeFadedDown();
					if (onComplete != null) onComplete.Invoke();
					break;
				}

				yield return null;
			}
		}

		protected virtual IEnumerator FadeUpCoroutine(Action onComplete, params CanvasGroup[] _canvasGroup) 
		{
			float alpha = 1; //GetEffectAlpha();
		float canvasalpha = 0;
			bool updateCanvas = _canvasGroup.Length > 0;

		while (true) 
		{
				float step = Time.unscaledDeltaTime / m_effectDuration;
				alpha -= step;
				canvasalpha += step;

                if (alpha <= 0) {
					alpha = 0;
				}

				if (updateCanvas)
				{
					foreach (CanvasGroup cg in _canvasGroup)
						cg.alpha = canvasalpha;
				}

				SetEffectAlpha(alpha);

				if (alpha == 0) {
					yield return new WaitForEndOfFrame();

					InvokeFadedUp();
					if (onComplete != null) onComplete.Invoke();
					break;
				}

				yield return null;
			}
		}
#else
		protected virtual IEnumerator<float> FadeDownCoroutine(Action onComplete, params CanvasGroup[] _canvasGroup)
		{
			float alpha = 0; //GetEffectAlpha();
			float canvasalpha = 1;
            bool updateCanvas = _canvasGroup.Length > 0;


			while (true)
			{
				float step = Time.unscaledDeltaTime / m_effectDuration;
				alpha += step;
				canvasalpha -= step;

				if (alpha >= 1)
				{
					alpha = 1;
				}

				if (updateCanvas)
                {
					foreach (CanvasGroup cg in _canvasGroup)
						cg.alpha = canvasalpha;
				}


				SetEffectAlpha(alpha);

				if (alpha == 1)
				{
					yield return Timing.WaitForOneFrame;

					InvokeFadedDown();
					if (onComplete != null) onComplete.Invoke();
					break;
				}

				yield return Timing.WaitForOneFrame;
			}
		}

		protected virtual IEnumerator<float> FadeUpCoroutine(Action onComplete, params CanvasGroup[] _canvasGroup)
		{
			float alpha = 1; //GetEffectAlpha();
			float canvasalpha = 0;
            bool updateCanvas = _canvasGroup.Length > 0;

			while (true)
			{
				float step = Time.unscaledDeltaTime / m_effectDuration;
				alpha -= step;
				canvasalpha += step;

				if (alpha <= 0)
				{
					alpha = 0;
				}

				if (updateCanvas)
                {
					foreach (CanvasGroup cg in _canvasGroup)
						cg.alpha = canvasalpha;
				}

				SetEffectAlpha(alpha);

				if (alpha == 0)
				{
					yield return Timing.WaitForOneFrame;

					InvokeFadedUp();
					if (onComplete != null) onComplete.Invoke();
					break;
				}

				yield return Timing.WaitForOneFrame;
			}
		}
#endif
	}
}