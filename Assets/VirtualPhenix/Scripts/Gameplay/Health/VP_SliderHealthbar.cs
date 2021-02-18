using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace VirtualPhenix.Gameplay
{
	public class VP_SliderHealthbar : VP_Healthbar
	{
		[SerializeField] protected Slider m_healthbar;
		
		protected override void Initialize()
		{
			if (m_healthbar == null)
			{
				m_healthbar = GetComponentInChildren<Slider>();
			}
			
			m_healthbar.maxValue = MaxHealth;
			
			base.Initialize();
		}
		
		public override void UpdateHealthBar()
		{
			base.UpdateHealthBar();
			m_healthbar.value = CurrentHealth;
		}
	}
}
 
