using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace VirtualPhenix.Gameplay
{
	public class VP_ImageHealthbar : VP_Healthbar
	{
		[SerializeField] protected Image m_healthbar;
		
		protected virtual void Reset()
        {
			m_healthbar = GetComponentInChildren<Image>();
		}

		protected override void Initialize()
		{
			if (m_healthbar == null)
			{
				m_healthbar = GetComponentInChildren<Image>();
			}
			
			base.Initialize();
		}
		
		public override void UpdateHealthBar()
		{
			base.UpdateHealthBar();
			m_healthbar.fillAmount = m_currentValue;
		}
	}
}
 
