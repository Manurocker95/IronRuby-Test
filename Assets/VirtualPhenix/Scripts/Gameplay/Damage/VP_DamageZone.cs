using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Gameplay
{
    [AddComponentMenu(VP_PrefixSetup.MAIN_PREFIX+"/Gameplay/Damage Zone")]
    public class VP_DamageZone : VP_MonoBehaviour, VP_IDamageable
    {
        [Header("Damage Zone"),Space]
        [SerializeField] protected VP_Health m_health;
        [SerializeField] protected bool m_useEvents = true;

        [SerializeField] public UnityEngine.Events.UnityEvent<VP_DamageData> OnDamage { get; protected set; }

        public VP_Health Health
        {
            get
            {
                return m_health;
            }
        }

        /// <summary>
        /// Override with specific damage calculations in inherited scripts
        /// </summary>
        /// <param name="_data"></param>
        public virtual void TakeDamage(VP_DamageData _data)
        {
            if (m_useEvents)
                OnDamage.Invoke(_data);

            if (m_health)
                m_health.RemoveHealth(_data.Damage);

            if (_data.Callback != null)
                _data.Callback.Invoke();
        }
    }
}