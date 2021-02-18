using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace VirtualPhenix.Gameplay
{
	[AddComponentMenu(VP_PrefixSetup.MAIN_PREFIX+"/Gameplay/UI/Health Bar")]
	public class VP_Healthbar : VP_MonoBehaviour
	{
		[SerializeField] protected VP_Health m_health;
		[SerializeField] protected float m_currentValue;

		public virtual float MaxHealth
        {
			get
            {
				return m_health ? m_health.MaxHP : 1f;
            }
        }

		public virtual float MinHealth
		{
			get
			{
				return 0f;
			}
		}

		public virtual float CurrentHealth
        {
			get
            {
				return m_currentValue;
            }
        }

		// Start is called before the first frame update
		protected override void Initialize()
		{
			base.Initialize();

			if (!m_health)
				m_health = GetComponent<VP_Health>();
					
			UpdateHealthBar();
		}

		protected override void StartAllListeners()
		{
			if (m_health)
            {
				m_health.OnDamage.AddListener(RecieveDamage);
				m_health.OnHeal.AddListener(Heal);
			}
		}

		protected override void StopAllListeners()
		{
			if (m_health)
			{
				m_health.OnDamage.RemoveListener(RecieveDamage);
				m_health.OnHeal.RemoveListener(Heal);
			}
		}

		public virtual void RecieveDamage(float damageTaken)
		{
			UpdateHealthBar();
		}
		
		public virtual void Heal(float ammount)
		{
			// We could make an animation before updating values, fpr example
			UpdateHealthBar();
		}

		public virtual void UpdateHealthBar()
		{
			if (m_health)
				m_currentValue = m_health.HP / m_health.MaxHP;

		
		}
	}
}