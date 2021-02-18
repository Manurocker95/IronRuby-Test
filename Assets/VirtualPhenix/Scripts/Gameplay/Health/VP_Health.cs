using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VirtualPhenix.Controllers.Components;
using VirtualPhenix;

namespace VirtualPhenix.Gameplay
{
    public class VP_Health : VP_CharacterComponent
    {

        [Header("Health"),Space]
        [SerializeField] protected bool m_setHPOnInit = true;

        [SerializeField] public virtual float HP { get; protected set; }
        [SerializeField] public virtual float MaxHP { get; protected set; }

        [Header("Events"), Space]

        [SerializeField] protected bool m_useEvents = false;
	    [SerializeField] protected bool m_useEventManagerEvent = false;

        [SerializeField] public UnityEvent<float> OnHPSet { get; protected set; }
        [SerializeField] public UnityEvent<float> OnMaxHPSet { get; protected set; }
        [SerializeField] public UnityEvent<float> OnHeal { get; protected set; }
        [SerializeField] public UnityEvent<float> OnDamage { get; protected set; }
        [SerializeField] public UnityEvent<VP_Health> OnHealthEnd { get; protected set; }

        public bool IsAlive
        {
            get
            {
                return HP > 0;
            }
        }

        public bool IsFullHP
        {
            get
            {
                return HP == MaxHP;
            }
        }

        protected override void Initialize()
        {
            base.Initialize();

            if (m_setHPOnInit)
            {
                SetHP(MaxHP);
            }
        }

        public virtual void SetHP(float _newHP, bool _useEvent = false)
        {
            _newHP = Mathf.Clamp(_newHP, 0, MaxHP);
            HP = _newHP;
            if (_useEvent)
            {
                OnHPSet?.Invoke(HP);
                if (HP == 0)
                {
	                if (m_attachedController && m_useEventManagerEvent)
                        VP_EventManager.TriggerEvent(VP_EventSetup.Controllers.CHARACTER_DIE, m_attachedController);

                    OnHealthEnd?.Invoke(this);
                }
            }
        }
        protected virtual void SetMaxHP(float _newMaxHP)
        {
            MaxHP = _newMaxHP;
            if (m_useEvents)
                OnMaxHPSet?.Invoke(MaxHP);
        }

        public virtual void AddHealth(float _hp)
        {
            SetHP(HP + _hp, m_useEvents);

            if (m_useEvents)
                OnDamage?.Invoke(_hp);
        }

        public virtual void RemoveHealth(float _hp)
	    {
		    if (IsAlive)
		    {
			    SetHP(HP - _hp, m_useEvents);
			    if (m_useEvents)
				    OnDamage?.Invoke(_hp);
		    }
        }
    }
}