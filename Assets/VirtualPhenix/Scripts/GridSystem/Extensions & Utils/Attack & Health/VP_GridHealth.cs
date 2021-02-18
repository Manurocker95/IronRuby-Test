#if USE_GRID_SYSTEM
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace VirtualPhenix.GridEngine
{
    [RequireComponent(typeof(VP_GridObject))]
    public class VP_GridHealth : VP_MonoBehaviour
    {
        public delegate void OnHealthChangedHandler(VP_GridHealth health, VP_GridObject attacker, int healthChangedAmount);
        public event OnHealthChangedHandler onHealthChangedEvent;
        public void OnHealthChanged(VP_GridObject attacker, int healthChangedAmount) { if (onHealthChangedEvent != null) onHealthChangedEvent(this, attacker, healthChangedAmount); }

        public delegate void OnDeathHandler(VP_GridHealth health, VP_GridObject attacker);
        public event OnDeathHandler onDeathEvent;
        public void OnDeath(VP_GridObject attacker) { if (onDeathEvent != null) onDeathEvent(this, attacker); }

        [Header("Runtime Values")]
        public bool m_Dead = false;
        public bool m_CanDie = true;
        public int m_CurrentHealth = 1;
        public int m_MaxHealth = 1;
        public bool m_DestroyOnDeath = true;

        [Header("Invincibility")]
        public bool m_Invincible = false;
        public bool m_BecomeInvincibleOnDamage = false;
        [SerializeField] protected VP_GridCooldown _invincibilityTimer;
        public VP_GridCooldown m_InvincibilityTimer { get { return _invincibilityTimer; } }

        //[Tooltip("How long after this character is 'dead' will it totally despawn?")]
        //public float m_DeathWaitTime = 0.8f;
        [Tooltip("If the health collider is a very specific collider, drag it in here. Otherwise we just try the first collider we can find on this object and its children.")]
        public Collider m_HealthTrigger;
        public VP_GridObject m_GridObject;

        [Header("Animations")]
        public Animator m_Animator;
        public bool m_AnimateOnTakeDamage = false;
        public string m_TakeDamageTriggerName = "Hurt";
        public bool m_AnimateOnDeath = false;
        public string m_DeathTriggerName = "Die";

        [Header("Unity Events")]
        public UnityEvent m_OnDeath;
        public UnityEvent m_OnReceiveDamage;
        public UnityEvent m_OnBecomeInvincible;

        protected override void Initialize()
        {
            base.Initialize();

            m_CurrentHealth = m_MaxHealth;
            if (m_HealthTrigger == null)
            {
                m_HealthTrigger = GetComponentInChildren<Collider>();
                if (m_HealthTrigger == null)
                {
                    Debug.LogWarning("Health Trigger not found for " + this);
                }
            }

            m_GridObject = GetComponent<VP_GridObject>();
        }

        protected override void StartAllListeners()
        {
            base.StartAllListeners();
            _invincibilityTimer.onCooldownEnded += EndInvincibility;
        }

        protected override void StopAllListeners()
        {
            base.StopAllListeners();
            _invincibilityTimer.onCooldownEnded -= EndInvincibility;
        }

        protected virtual void Update()
        {
            m_InvincibilityTimer.Update();
        }

        public virtual void ReceiveHealth(int healAmount)
        {
            if (!m_Dead)
            {
                m_CurrentHealth = Mathf.Min(m_CurrentHealth + healAmount, m_MaxHealth);
                OnHealthChanged(null, healAmount);
            }
        }

        public virtual void DamageHealth(VP_GridAttack attack, int damage, bool ignoreInvicibility = false)
        {

            // Check if the attack was made by another grid object
            VP_GridObject attackerGridObject = null;
            if (attack)
            {
                attackerGridObject = attack.m_GridObject;
            }

            if (!m_Dead && (!m_Invincible || ignoreInvicibility))
            {
                m_CurrentHealth -= damage;
                m_CurrentHealth = Mathf.Clamp(m_CurrentHealth, 0, m_MaxHealth);
                if (m_CurrentHealth <= 0 && m_CanDie)
                {
                    // Set it to death
                    m_Dead = true;
                    // Disable the trigger/collider
                    if (m_HealthTrigger != null)
                    {
                        m_HealthTrigger.enabled = false;
                    }
                    // Death Animation
                    if (m_AnimateOnDeath && m_Animator)
                    {
                        m_Animator.SetTrigger(m_DeathTriggerName);
                    }

                    // Actually kill and remove the entity
                    Die(attackerGridObject);
                }
                else
                {
                    // Trigger the health changed event
                    OnHealthChanged(attackerGridObject, -damage);
                    m_OnReceiveDamage.Invoke();
                    // Become invincible
                    if (m_BecomeInvincibleOnDamage)
                    {
                        BecomeTemporarilyInvincible();
                    }

                    // Take Damage Animation
                    if (m_AnimateOnTakeDamage && m_Animator)
                    {
                        m_Animator.SetTrigger(m_TakeDamageTriggerName);
                    }
                }
                if (m_CurrentHealth <= 0 && !m_CanDie)
                {
                    ReceiveHealth(1 - m_CurrentHealth);
                }
            }
        }

        public virtual void DamageHealth(VP_GridObject gridObject, int damage, bool ignoreInvicibility = false)
        {

            // Check if the attack was made by another grid object
            VP_GridObject attackerGridObject = gridObject;

            if (!m_Dead && (!m_Invincible || ignoreInvicibility))
            {
                m_CurrentHealth -= damage;
                m_CurrentHealth = Mathf.Clamp(m_CurrentHealth, 0, m_MaxHealth);
                if (m_CurrentHealth <= 0 && m_CanDie)
                {
                    // Set it to death
                    m_Dead = true;
                    // Disable the trigger/collider
                    if (m_HealthTrigger != null)
                    {
                        m_HealthTrigger.enabled = false;
                    }
                    // Death Animation
                    if (m_AnimateOnDeath && m_Animator)
                    {
                        m_Animator.SetTrigger(m_DeathTriggerName);
                    }

                    // Actually kill and remove the entity
                    Die(attackerGridObject);
                }
                else
                {
                    // Trigger the health changed event
                    OnHealthChanged(attackerGridObject, -damage);
                    m_OnReceiveDamage.Invoke();
                    // Become invincible
                    if (m_BecomeInvincibleOnDamage)
                    {
                        BecomeTemporarilyInvincible();
                    }

                    // Take Damage Animation
                    if (m_AnimateOnTakeDamage && m_Animator)
                    {
                        m_Animator.SetTrigger(m_TakeDamageTriggerName);
                    }
                }
                if (m_CurrentHealth <= 0 && !m_CanDie)
                {
                    ReceiveHealth(1 - m_CurrentHealth);
                }
            }
        }

        // You can set all types of rules for wether or not this unit is damageable by another one here, this is called in the attack component
        public virtual bool CanBeAttackedBy(VP_GridAttack attack = null)
        {
            return attack.m_GridObject != m_GridObject;
        }

        public virtual bool CanBeAttackedBy(VP_GridObject gridObject)
        {
            return gridObject != m_GridObject;
        }

        protected virtual void Die(VP_GridObject attacker = null)
        {
            // Event
            OnDeath(attacker);
            // Unity Event
            m_OnDeath.Invoke();

            if (m_DestroyOnDeath)
            {
                DestroyMe();
            }
        }

        protected virtual void DestroyMe()
        {
            Destroy(gameObject);
        }
        /*
        protected virtual void HealthChanged(GridObject attacker, int healthChangedAmount)
        {
            if (this.OnHealthChanged != null)
            {
                this.OnHealthChanged(this, attacker, healthChangedAmount);
            }
        }
        */

        public virtual void BecomeTemporarilyInvincible()
        {
            m_Invincible = true;
            m_InvincibilityTimer.Reset();

            m_OnBecomeInvincible.Invoke();
        }

        public virtual void EndInvincibility()
        {
            m_Invincible = false;
        }

        public virtual void Reset()
        {

            m_startListeningTime = StartListenTime.OnEnable;
            m_stopListeningTime = StopListenTime.OnDisable;

            transform.TryGetComponentInChildren(out m_HealthTrigger);
            transform.TryGetComponentInChildren(out m_GridObject);
            m_CurrentHealth = m_MaxHealth;
            m_Dead = false;
        }
    }

}
#endif