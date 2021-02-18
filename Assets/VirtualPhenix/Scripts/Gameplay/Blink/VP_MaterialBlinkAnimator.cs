using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace VirtualPhenix.Gameplay
{
    public class VP_MaterialBlinkAnimator<T> : VP_BlinkAnimator
    {
		[Header("Material Setup")]
		[SerializeField] protected bool m_autoInitIfNull= true;
		[SerializeField] protected Renderer[] m_renderers = new Renderer[0];		
		[SerializeField] protected string[] m_eyeTextureIDs = new string[] { "_MainTex" /*, "_STexture"*/ };

		[Header("Material Offsets")]
		[SerializeField] protected T m_openEyeOffset;
		[SerializeField] protected T m_closeEyeOffset;

		[Header("Material Index")]
		[SerializeField] protected int[] m_renderIndex = new int[] { -1 };
	    [SerializeField] protected int[] m_materialIndex = new int[] { -1 };
		[SerializeField] protected int[] m_idIndex = new int[] { -1 };

		[Header("Event Trigger")]
		[SerializeField] protected bool m_triggerEventOnBlink = false;
		[SerializeField] protected UnityEvent m_onBlink = new UnityEvent();

		public UnityEvent OnBlink
        {
			get
            {
				return m_onBlink;
            }
        }

		protected override void Initialize()
        {
            base.Initialize();
			if (m_autoInitIfNull && m_renderers.Length == 0)
            {
				m_renderers = GetComponentsInChildren<Renderer>();
			}
        }

		protected override void BlinkSetup()
		{
			TextureSetup(m_closeEyeOffset);
			base.BlinkSetup();
		}

		protected override void StartWaitForBlinkSetup()
		{
			TextureSetup(m_openEyeOffset);
			base.StartWaitForBlinkSetup();
		}


		protected virtual void TextureSetup(T eyeOffset)
        {
			
		}
    }

}