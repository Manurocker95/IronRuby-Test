using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#if USE_MORE_EFFECTIVE_COROUTINES
using MEC;
#endif

namespace VirtualPhenix
{
	public class VP_DissolveModel : VP_MonoBehaviour
	{
		[Header("Disolve Material and Renderer")]
		[SerializeField] protected MeshRenderer m_meshToDissolve;
		[SerializeField] protected Material m_disolveMaterial;
		[SerializeField]protected string m_disolveID = "_DissolveAmount";
		[SerializeField]protected bool m_blockPreviousCoroutine = false;
	
			
		[Header("Events")]
		[SerializeField] protected UnityEvent m_preDisolve = new UnityEvent();
		[SerializeField] protected UnityEvent m_postDisolve = new UnityEvent();
	
		protected Material m_copyOfMaterial;
		protected bool m_disolving = false;

		public bool IsDisolving 
		{
			get
			{
				return m_disolving;
			}
		}
		
		public UnityEvent PreDisolveEvent
		{
			get
			{
				return m_preDisolve;
			}
		}
			
			
		public UnityEvent PostDisolveEvent
		{
			get
			{
				return m_postDisolve;
			}
		}
			
#if USE_MORE_EFFECTIVE_COROUTINES
		CoroutineHandle m_coroutine;
#else
		Coroutine m_coroutine;
#endif

		public virtual void Dissolve(float dissolveTime, MeshRenderer _renderer, Material dissolveMat, UnityEngine.Events.UnityAction _callback)
		{
			m_disolveMaterial = dissolveMat;
			
			m_meshToDissolve = _renderer;
			
			//Make a local copy of dissolve material
			m_copyOfMaterial = Instantiate(m_disolveMaterial);
			m_meshToDissolve.material = m_copyOfMaterial;
			
			m_preDisolve.Invoke();
			
#if USE_MORE_EFFECTIVE_COROUTINES

			if (m_blockPreviousCoroutine)
			{
				Timing.KillCoroutines(m_coroutine);
			}


			m_coroutine = Timing.RunCoroutine(Dissolving(dissolveTime,_callback));
#else
			if (m_blockPreviousCoroutine)
			{
			StopCoroutine(m_coroutine);
			}


			m_coroutine = StartCoroutine(Dissolving(dissolveTime,_callback));
#endif
		}

		public virtual void StartDissolving(float dissolveTime)
		{
			//Make a local copy of dissolve material
			m_copyOfMaterial = Instantiate(m_disolveMaterial);
			m_meshToDissolve.material = m_copyOfMaterial;
			
			m_preDisolve.Invoke();
			
#if USE_MORE_EFFECTIVE_COROUTINES

			if (m_blockPreviousCoroutine)
			{
				Timing.KillCoroutines(m_coroutine);
			}


			m_coroutine = Timing.RunCoroutine(Dissolving(dissolveTime));
#else
			if (m_blockPreviousCoroutine)
			{
				StopCoroutine(m_coroutine);
			}


			m_coroutine = StartCoroutine(Dissolving(dissolveTime));
#endif
		}
	
#if USE_MORE_EFFECTIVE_COROUTINES
		protected virtual IEnumerator<float> Dissolving(float dissolveTime, UnityEngine.Events.UnityAction _callback = null)
		{
	        float counter = 0;
	        float m_oneOverDissolveTime = 1f/dissolveTime;
			while(counter < dissolveTime)
			{
				counter += Time.deltaTime;
				m_copyOfMaterial.SetFloat(m_disolveID,counter * m_oneOverDissolveTime);
				yield return Timing.WaitForOneFrame;
			}
			
			m_postDisolve.Invoke();
			if (_callback != null)
				_callback.Invoke();
		}
#else
		protected virtual IEnumerator Dissolving(float dissolveTime, UnityEngine.Events.UnityAction _callback = null)
		{
			float counter = 0;
			float m_oneOverDissolveTime = 1f/dissolveTime;
			
			while(counter	< dissolveTime)
			{
				counter += Time.deltaTime;
				m_copyOfMaterial.SetFloat(m_disolveID,counter*m_oneOverDissolveTime);
				yield return new WaitForEndOfFrame();
			}
			m_postDisolve.Invoke();
			
			if (_callback != null)
				_callback.Invoke();
		}
#endif
	}

}
