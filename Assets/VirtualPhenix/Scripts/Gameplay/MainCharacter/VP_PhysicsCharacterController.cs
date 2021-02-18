using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Controllers
{
    [DefaultExecutionOrder(VP_ExecutingOrderSetup.CHARACTER_CONTROLLER), AddComponentMenu(VP_PrefixSetup.MAIN_PREFIX + "/Controllers/Physics Player Controller")]
    public class VP_PhysicsCharacterController : VP_PlayableCharacterController, VP_IPhysicsMovable
    {
        [SerializeField] protected Collider m_collider;
		[SerializeField] protected GameObject m_forceApplyPosition;
		[SerializeField] protected Rigidbody m_rigidBody;
        [SerializeField] protected bool m_disableFriction;

		[SerializeField] protected float m_groundStickyForce = 1f;
		[SerializeField] protected float m_maxSpeed = 5f;
		[SerializeField] protected float m_turnSpeed = 1f;
		[SerializeField] protected float m_standingFriction = 1f;
		[SerializeField] protected float m_movingFriction = 0f;
		[SerializeField] protected float m_movementForce = 8000f;
		[SerializeField] protected float m_midairForce = 8000f;

		[SerializeField] protected Vector3? m_cachedDesiredMovementDirection;
		public Vector3? WalkTo { get; protected set; }

		public Rigidbody GetRigidbody()
        {
			return m_rigidBody;
        }

        protected override void Reset()
        {
            base.Reset();

			m_rigidBody = GetComponent<Rigidbody>();
			m_collider = GetComponent<Collider>();
		}

        // Update is called once per frame
        protected override void Update()
		{
			
		}

		protected virtual void FixedUpdate()
		{
			CheckIsGrounded();

			CheckInteract(InteractWasPressed());

			bool canMove = CanCharacterMove();
			float h = canMove ? GetHorizontalValue() : 0f;
			float v = canMove ? GetVerticalValue() : 0f;

			m_sprinting = m_canSprint && IsSprinting();

			ApplyMovement(h, v);
		}

        protected override void ApplyMovement(float h, float v)
        {
			Vector3 desiredMovementVector = this.GetDesiredMovementVector();
			if (this.ResolveCanMove())
			{
				Vector3 a = desiredMovementVector * this.ResolveMaximumVelocity();
				Vector3 b = base.GetComponent<Rigidbody>().velocity.SetY(0f);
				Vector3 a2 = a - b;
				float d = this.ResolveMovementForce();
				m_rigidBody.AddForceAtPosition(a2 * d * Time.fixedDeltaTime, m_forceApplyPosition.transform.position);
			}
			Vector3 desiredRotateDirection = this.GetDesiredRotateDirection();
			if (this.ResolveCanRotate() && desiredRotateDirection != Vector3.zero)
			{
				float numb = Vector3.Angle(this.GetRotationModel().transform.forward.SetY(0f), desiredRotateDirection) * Mathf.Sign(Vector3.Cross(this.GetRotationModel().transform.forward.SetY(0f), desiredRotateDirection).y);
				this.GetRotationModel().transform.Rotate(GetUpVector(), numb * this.ResolveTurnSpeed() * Time.fixedDeltaTime);
			}

			if (IsGrounded)
			{
				this.m_rigidBody.AddForce(-base.transform.up * m_groundStickyForce);
			}
			if (this.m_rigidBody.velocity.SetY(0f).sqrMagnitude <= 1.00000011E-06f)
			{
				this.m_rigidBody.velocity = this.m_rigidBody.velocity.SetX(0f).SetZ(0f);
			}
			bool flag = this.GetDesiredMovementVector() * this.ResolveMaximumVelocity() == Vector3.zero;
			float num = (IsGrounded && flag && !m_disableFriction) ? m_standingFriction : m_movingFriction;

			if (m_collider.material)
            {
				m_collider.material.dynamicFriction = num;
				m_collider.material.staticFriction = num;
			}
		}

		public virtual float ResolveMovementForce()
		{
			if (!IsGrounded)
			{
				return m_midairForce;
			}
			return m_movementForce;
		}


		public virtual Vector3 GetDesiredMovementVector()
        {
			if (m_cachedDesiredMovementDirection != null)
			{
				return m_cachedDesiredMovementDirection.Value;
			}
			if (WalkTo != null)
			{
				return (WalkTo.Value - base.transform.position).SetY(0f).normalized;
			}
			Vector3 vector = this.GetInputWorldDirection();
			m_cachedDesiredMovementDirection = new Vector3?(vector);
			return vector;
		}

		public virtual Vector3 GetInputWorldDirection()
		{
			Vector2 movement = m_inputActions != null ? m_inputActions.GetMovement() : Vector2.zero;
			Vector3 normalized = Camera.main.transform.forward.SetY(0f).normalized;
			Vector3 a = -Vector3.Cross(normalized, Vector3.up);
			return normalized * movement.y + a * movement.x;
		}

		public Vector3 GetWorldLookDirection()
		{
			Vector2 lookDirection = m_inputActions != null ? m_inputActions.GetMovement() : Vector2.zero;
			Vector3 normalized = Camera.main.transform.forward.SetY(0f).normalized;
			Vector3 a = -Vector3.Cross(normalized, Vector3.up);
			return normalized * lookDirection.y + a * lookDirection.x;
		}


		protected virtual Vector3 GetDesiredRotateDirection()
		{
			return GetDesiredMovementVector();
		}


		public virtual float ResolveMaximumVelocity()
		{
			return m_maxSpeed;
		}

		public virtual float ResolveTurnSpeed()
		{
			return m_turnSpeed;
		}

		protected virtual bool ResolveCanRotate()
		{
			return ResolveCanMove();
		}

		protected virtual GameObject GetRotationModel()
		{
			return m_mainObject.gameObject;
		}

	}
}