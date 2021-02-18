#if USE_GRID_SYSTEM
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.GridEngine
{
    public class VP_GridAttack : MonoBehaviour
    {

        public delegate void AttackStartedHandler(VP_GridAttack attack);
        public delegate void AttackFinishedHandler(VP_GridAttack attack);

        public event AttackStartedHandler OnAttackStarted;
        public event AttackFinishedHandler OnAttackFinished;

        [HideInInspector]
        public HashSet<VP_GridAttackTrigger> m_AttackTriggers = new HashSet<VP_GridAttackTrigger>();

        [Header("Settings")]
        public int m_Damage = 1;
        public bool m_IgnoresInvicibility;
        public bool m_InProgress { get; private set; }
        public bool m_CanAttack => !m_InProgress && !m_Cooldown.InProgress;

        [Header("CoolDown")]
        [SerializeField] protected VP_GridCooldown _cooldown;
        public VP_GridCooldown m_Cooldown { get { return _cooldown; } }
        public bool m_CausesMovementCooldown = true;

        [Header("Delay")]
        public bool m_DelayBeforeDamaging = false;
        public float m_DelayTime = 0f;

        [Header("Animation")]
        public Animator m_Animator;
        public string m_AttackAnimationTriggerName = "Attack";

        [Header("References")]
        public VP_GridObject m_GridObject;
        public VP_GridMovement m_GridMovement;
        public VP_GridHealth m_OwnerHealth;

        protected virtual void Reset()
        {
            Awake();
        }

        protected virtual void Awake()
        {
            if (m_GridObject == null)
            {
                m_GridObject = GetComponent<VP_GridObject>();
                if (m_GridObject == null)
                {
                    m_GridObject = GetComponentInParent<VP_GridObject>();
                }
            }
            if (m_GridMovement == null)
            {
                m_GridMovement = GetComponent<VP_GridMovement>();
                if (m_GridMovement == null)
                {
                    m_GridMovement = GetComponentInParent<VP_GridMovement>();
                }
            }
            if (m_OwnerHealth == null)
            {
                m_OwnerHealth = GetComponent<VP_GridHealth>();
                if (m_OwnerHealth == null)
                {
                    m_OwnerHealth = GetComponentInParent<VP_GridHealth>();
                }
            }

            // Finds and adds the attack triggers for this attack
            VP_GridAttackTrigger[] componentsInChildren = GetComponentsInChildren<VP_GridAttackTrigger>();
            foreach (VP_GridAttackTrigger item in componentsInChildren)
            {
                if (!m_AttackTriggers.Contains(item))
                {
                    m_AttackTriggers.Add(item);
                    item.m_Attack = this;
                }
            }
        }


        // Update is called once per frame
        protected virtual void Update()
        {
            _cooldown.Update();
        }

        public virtual AttackResult TryAttack(Vector2Int? targetPosition = null, bool triggersCooldown = true)
        {

            // The failed attack result will be used later when the attack system is expanded
            AttackResult attackResult = AttackResult.Failed;

            // Check if the attack is already in progress and if it is on cooldown
            if (!m_CanAttack)
            {
                return AttackResult.Cooldown;
            }

            // Triggers the movement cooldown
            if (m_CausesMovementCooldown && m_GridObject && m_GridMovement)
            {
                m_GridMovement.Cooldown.Reset(null);
            }

            // Trigger the cooldown
            if (triggersCooldown)
            {
                ResetCooldown();
            }

            DoAttack(targetPosition.HasValue ? targetPosition.Value : targetPosition);
            attackResult = AttackResult.Success;
            return attackResult;
        }

        public virtual void DoAttack(Vector2Int? targetPosition = null)
        {
            // Start the attack
            AttackStarted();

            // Get the victim(s) list from trigger(s)
            var victims = targetPosition.HasValue ? GetVictimsFromTriggerAtPosition(targetPosition.Value) : GetVictimsFromTriggers();
            // Damage the health(s)
            foreach (VP_GridHealth item in victims)
            {
                if (m_DelayBeforeDamaging)
                {
                    StartCoroutine(DamageHealth(item, m_DelayTime));
                }
                else
                {
                    DamageHealth(item);
                }
            }

            // End the attack
            AttackFinished();
        }

        public virtual List<VP_GridHealth> GetVictimsFromTriggerAtPosition(Vector2Int position)
        {
            var healths = new List<VP_GridHealth>();

            if (m_AttackTriggers.Count > 0)
            {
                foreach (VP_GridAttackTrigger attackTrigger in m_AttackTriggers)
                {
                    if (m_GridObject)
                    {
                        attackTrigger.TrySetAttackDirection(m_GridObject.FacingDirection);
                    }

                    if (attackTrigger.HasVictimAtPosition(position))
                    {
                        foreach (VP_GridHealth item in attackTrigger.GetVictims())
                        {
                            if (item.CanBeAttackedBy(this))
                            {
                                healths.Add(item);
                            }
                        }
                    }
                }
            }

            return healths;
        }

        public virtual List<VP_GridHealth> GetVictimsFromTriggers()
        {
            var healths = new List<VP_GridHealth>();

            if (m_AttackTriggers.Count > 0)
            {
                foreach (VP_GridAttackTrigger attackTrigger in m_AttackTriggers)
                {
                    if (m_GridObject)
                    {
                        attackTrigger.TrySetAttackDirection(m_GridObject.FacingDirection);
                    }
                    foreach (VP_GridHealth item in attackTrigger.GetVictims())
                    {
                        if (item.CanBeAttackedBy(this))
                        {
                            healths.Add(item);
                        }
                    }
                }
            }

            return healths;
        }

        public virtual List<Vector2Int> GetAllTargetPositions()
        {
            var positions = new List<Vector2Int>();

            if (m_AttackTriggers.Count > 0)
            {
                foreach (VP_GridAttackTrigger attackTrigger in m_AttackTriggers)
                {
                    if (m_GridObject)
                    {
                        attackTrigger.TrySetAttackDirection(m_GridObject.FacingDirection);
                    }
                    var tempPositions = attackTrigger.GetTargetPositions();
                    if (tempPositions.Count > 0)
                    {
                        positions.AddRange(tempPositions);
                    }
                }
            }

            return positions;
        }

        public virtual List<VP_GridTile> GetAllTargetTiles()
        {
            var positions = GetAllTargetPositions();
            var tiles = new List<VP_GridTile>();

            foreach (Vector2Int pos in positions)
            {
                var tileAtPosition = VP_GridManager.Instance.GetGridTileAtPosition(pos);
                if (tileAtPosition && !tiles.Contains(tileAtPosition))
                {
                    tiles.Add(tileAtPosition);
                }
            }

            return tiles;
        }

        public virtual void DamageHealth(VP_GridHealth health)
        {
            health.DamageHealth(this, m_Damage, m_IgnoresInvicibility);
        }

        public virtual IEnumerator DamageHealth(VP_GridHealth health, float delay)
        {
            yield return new WaitForSeconds(delay);
            DamageHealth(health);
        }

        protected virtual void AttackStarted()
        {
            if (this.OnAttackStarted != null)
            {
                this.OnAttackStarted(this);
            }
            m_InProgress = true;

            if (m_Animator != null)
            {
                m_Animator.SetTriggerIfExists(m_AttackAnimationTriggerName);
            }
        }

        protected virtual void AttackFinished()
        {
            if (this.OnAttackFinished != null)
            {
                this.OnAttackFinished(this);
            }
            m_InProgress = false;
        }

        public void ResetCooldown(float? duration = default(float?))
        {
            m_Cooldown.Reset(duration);
        }
    }

}
#endif