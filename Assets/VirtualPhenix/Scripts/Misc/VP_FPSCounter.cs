using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace VirtualPhenix.Misc
{
	[AddComponentMenu(VP_PrefixSetup.MAIN_PREFIX+"/Misc/FPS Counter")]
    public class VP_FPSCounter : VP_MonoBehaviour
    {
		[Header("FPS Counter"),Space]
		[SerializeField] protected TMP_Text m_text;
		[SerializeField] protected float m_frequency = 0.5f;
		[SerializeField] protected bool m_visible;
		[SerializeField] protected bool m_startOnInit = true;
	    
	    protected Coroutine m_coroutine;
	    
		public int FPSCount { get; protected set; }

		public bool Visible
		{
			get
			{
				return m_visible;
			}
			set
			{
				m_visible = value;
				if (m_text)
					m_text.enabled = value;
			}
		}

		protected virtual void Reset()
        {
			m_text = GetComponent<TMP_Text>();
		}

        protected override void Initialize()
        {
            base.Initialize();

			if (!m_text)
				m_text = GetComponent<TMP_Text>();
		}

        protected override void OnEnable()
		{
			base.OnEnable();

			if (!m_text)
				m_text = GetComponent<TMP_Text>();

			if (m_startOnInit)
            {
				Visible = true;
            }

			StartFPSCoroutine();
		}

        protected override void OnDisable()
        {
            base.OnDisable();
			StopFPSCoroutine();
		}

		public virtual void StartFPSCoroutine()
        {
			m_coroutine = base.StartCoroutine(this.FPS());
		}

		public virtual void StopFPSCoroutine()
		{
			if (m_coroutine != null)
				base.StopCoroutine(m_coroutine);
		}
		protected virtual IEnumerator FPS()
		{
			for (; ; )
			{
				int lastFrameCount = Time.frameCount;
				float lastTime = Time.realtimeSinceStartup;
				yield return new WaitForSeconds(this.m_frequency);
				float num = Time.realtimeSinceStartup - lastTime;
				int num2 = Time.frameCount - lastFrameCount;
				FPSCount = Mathf.RoundToInt((float)num2 / num);
				if (this.Visible && m_text != null)
				{
					m_text.text = string.Format("{0} fps", FPSCount);
				}
				yield return null;
			}
		}
	}
}