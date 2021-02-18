using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

using VirtualPhenix.Controllers.Components;

namespace VirtualPhenix.Controllers
{
    [AddComponentMenu("")]
    public class VP_CharacterController : VP_MonoBehaviour
    {
        [Header("Components"), Space]
        [SerializeField] protected bool m_fillComponentsOnInit = false;
        [SerializeField] protected List<VP_CharacterComponent> m_characterComponents = new List<VP_CharacterComponent>();

        [Header("Character - Animator"),Space]
        [SerializeField] protected Transform m_mainObject;
	    [SerializeField] protected bool m_playIdleAnimationOnEnable = false;
	    [SerializeField] protected string m_idleAnimation = "Idle";
	    [SerializeField] protected VP_LookAnimator m_lookAnimator;
        [SerializeField] protected VP_Animator m_animator;

        [Header("Config"), Space]
	    public bool m_blocked = false;	
	    
	    [Header("Character Control Abilities"), Space]
	    [SerializeField] protected bool m_canCharacterMove = true;
	    [SerializeField] protected bool m_canJumpOnLand = false;
	    [SerializeField] protected bool m_canMoveOnJump = true;
	    [SerializeField] protected bool m_canRotateOnLand = false;
	    [SerializeField] protected bool m_canMoveOnLand = false;
	    [SerializeField] protected bool m_canButtAttack = false;
	    [SerializeField] protected bool m_canInteract = true;
	    [SerializeField] protected bool m_canJump = false;
	    [SerializeField] protected bool m_canSprint = false;
	    [SerializeField] protected bool m_walkSpeedBasedOnInput = true;
	    
	    [Header("States Based on Controls"), Space]
        [SerializeField] protected bool m_jumping = false;
	    [SerializeField] protected bool m_falling = false;
        [SerializeField] protected bool m_landing = false;
        [SerializeField] protected bool m_buttAttack = false;
	    [SerializeField] protected bool m_sprinting = false;
	    [SerializeField] protected bool m_isWalking = false;
	    [SerializeField] protected bool m_isRunning = false;
        [SerializeField] protected bool m_interacting = false;
        [SerializeField] protected bool m_forcingAnimation = false;

	    protected bool m_startJump = false;

	    [Header("Floor"), Space]
	    [SerializeField] protected LayerMask m_floorMask = 0;
	    [SerializeField] protected bool m_isGrounded;
	    [SerializeField] protected float m_raycastHeight = 1f;
	    [SerializeField] protected Vector3 m_upVector = Vector3.up;
	    [SerializeField] protected Transform m_floorCheckerTrs;
	    [SerializeField] protected float m_floorYPos = 0f;
	    [SerializeField] protected bool m_setFloorOnCheck = true;
        [SerializeField] protected float m_fallingThreshold = 1f;

        [Header("Interact"), Space]
        [SerializeField] protected Transform m_interactPoint;
        [SerializeField] protected LayerMask m_interactMask;
        [SerializeField] protected float m_interactDistance = 2f;

        [Header("Speed Values"), Space]
        [SerializeField] protected float m_currentSpeed;

        [SerializeField] protected float m_movementSpeed = 9.0f;
        [SerializeField] protected float m_runningSpeed = 14.0f;
        [SerializeField] protected float m_rotationSpeed = 240.0f;
	    [SerializeField] protected float m_runningThreshold = 0.59f;
        
        [Header("Jump"), Space]
        [SerializeField] protected float m_jumpMaxHeight = 3f;
        [SerializeField] protected float m_jumpSpeed = 8.0f;
        [SerializeField] protected float m_gravity = 20f;
	    [SerializeField] protected float m_jumpForce = 1.5f; // THis is Actually Jump Force
        [SerializeField] protected float m_forceFallValue = 10f;
        [SerializeField] protected AudioClip m_jumpClip;

        [Header("Particles"), Space]
        [SerializeField] protected VP_VFXDefaultDatabase m_vfxResources;

        [Header("Move direction-> useful for CC"), Space]
        [SerializeField] protected Vector3 m_moveDir = Vector3.zero;
	    [SerializeField] protected float m_verticalMovement;


        [Header("Camera"), Space]
	    [SerializeField] protected bool m_basedOnCamera = true;
	    [SerializeField] protected Camera m_camera;

	    [Header("Invert H and V values"), Space]
	    [SerializeField] protected bool m_invertXZ = true;

	    [Header("Events"), Space]
	    public bool m_useEvents = true;
	    public UnityEvent<VP_CharacterController> OnButtSmashEvent;
	    public UnityEvent<VP_CharacterController> OnJumpEvent;
	    public UnityEvent<VP_CharacterController> OnFlyingActionEvent;
	    public UnityEvent<VP_CharacterController> OnLandEvent;
        public UnityEvent<VP_CharacterController, VP_IActionable> OnCharacterInteract;


        public virtual VP_LookAnimator LookAnimator
        {
            get
            {
                return m_lookAnimator;
            }
        }
        
        public virtual bool IsGrounded
        {
            get
            {
	            return m_isGrounded;
            }
        }
        
	    public virtual bool CanJump
        {
            get
            {
	            return m_canJump;
            }
        }

        public virtual bool MoveByBehaviorTree()
        {
            return true;
        }

        protected virtual void Reset()
	    {
            m_characterComponents = GetComponentsInChildren<VP_CharacterComponent>().ToList();
		    
		    m_idleAnimation = VP_AnimationSetup.Characters.IDLE;
		    
            m_mainObject = this.transform;
		    m_camera = Camera.main;
            
            m_animator = GetComponentInChildren<VP_Animator>();
	        m_lookAnimator = GetComponentInChildren<VP_LookAnimator>();

	        m_floorCheckerTrs = transform;
		    m_interactPoint = transform;
        }

        protected override void Initialize()
        {
            base.Initialize();

            if (m_animator == null)
            {
                transform.TryGetComponentInChildren<VP_Animator>(out m_animator);
            }

            if (m_interactPoint == null)
                m_interactPoint = transform;

	        if (!m_mainObject)
		        m_mainObject =  transform;				

	        if (m_floorCheckerTrs == null)
		        m_floorCheckerTrs = transform;
		      
	        if (m_playIdleAnimationOnEnable)
	        	PlayInitAnimation();
		      
	        if (!m_camera)
		        m_camera = Camera.main;

            if (m_fillComponentsOnInit)
            {
                m_characterComponents = GetComponentsInChildren<VP_CharacterComponent>().ToList();
            }

	        CheckNullComponents();
           
        }

        public virtual void CheckNullComponents()
        {
            List<VP_CharacterComponent> cc = new List<VP_CharacterComponent>(m_characterComponents);
            foreach (VP_CharacterComponent component in cc)
            {
                if (component == null)
                {
                    m_characterComponents.Remove(component);
                }
            }
        }

        public virtual bool TryToAddComponent(VP_CharacterComponent _component)
        {
            if (!m_characterComponents.Contains(_component))
            {
                m_characterComponents.Add(_component);
                CheckMainComponents(_component);
                return true;
            }

            return false;
        }

        public virtual void CheckMainComponents(VP_CharacterComponent _component)
        {
#if !USE_ANIMANCER
            if (_component is VP_Animator && m_animator == null)
            {
                m_animator = (VP_Animator)_component;
                return;
            }
#endif
            if (_component is VP_LookAnimator && m_lookAnimator == null)
            {
                m_lookAnimator = (VP_LookAnimator)_component;
                return;
            }
        }

        public virtual List<T> GetCharacterComponentsOfType<T>() where T : VP_CharacterComponent
        {
            List<T> list = new List<T>();

            foreach (VP_CharacterComponent cc in m_characterComponents)
            {
                if (cc is T)
                {
                    list.Add((T)cc);
                }
            }

            return list;
        }

        public virtual T GetCharacterComponentOfType<T>() where T : VP_CharacterComponent
        {
            foreach (VP_CharacterComponent cc in m_characterComponents)
            {
                if (cc is T)
                {
                    return (T)cc;
                }
            }

            return null;
        }

        public virtual bool TryGetCharacterComponentsOfType<T>(out List<T> _components) where T : VP_CharacterComponent
        {
            bool _found = false;
            _components = new List<T>();
            foreach (VP_CharacterComponent cc in m_characterComponents)
            {
                if (cc is T)
                {
                    if (!_found)
                        _found = true;

                    _components.Add((T)cc);
                }
            }

            return _found;
        }

        public virtual bool TryGetCharacterComponentOfType<T>(out T _component) where T : VP_CharacterComponent
        {
            _component = null;
            foreach (VP_CharacterComponent cc in m_characterComponents)
            {
                if (cc is T)
                {
                    _component = (T)cc;
                    return true;
                }
            }

            return false;
        }

        protected virtual void OnDrawGizmos()
	    {
		    Gizmos.color = Color.blue;
		    Gizmos.DrawRay(m_floorCheckerTrs != null ? m_floorCheckerTrs.position : transform.position, m_upVector * -1 * m_raycastHeight);
	    }

        protected override void StartAllListeners()
        {
	        if (m_useEvents)
	        {
		        OnJumpEvent.AddListener(OnJumpActionPerformed);
		        OnFlyingActionEvent.AddListener(OnFlyActionPerformed);
		        OnButtSmashEvent.AddListener(OnButtSmashPerformed);
		        OnLandEvent.AddListener(OnRegularLandPerformed);
	        }
        }

        protected override void StopAllListeners()
        {
            if (!VP_MonobehaviourSettings.Quiting)
            {
	            OnJumpEvent.RemoveAllListeners();
	            OnFlyingActionEvent.RemoveAllListeners();
	            OnButtSmashEvent.RemoveAllListeners();
	            OnLandEvent.RemoveAllListeners();
            }
         
        }

        protected override void OnEnable()
        {
	        base.OnEnable();
            
	        if (m_initialized && m_playIdleAnimationOnEnable)
                PlayInitAnimation();
        }

        protected virtual void PlayInitAnimation()
	    {
		    Debug.Log(gameObject.name + "m_playIdleAnimationOnEnable");
	        PlayAnimation(m_idleAnimation);
        }

	    public virtual void OnJumpActionPerformed(VP_CharacterController _performer)
	    {
		    
	    }

	    public virtual void OnFlyActionPerformed(VP_CharacterController _performer)
	    {
		   
	    }
		
	    public virtual void OnButtSmashPerformed(VP_CharacterController _performer)
	    {
		    
	    }

	    public virtual void OnRegularLandPerformed(VP_CharacterController _performer)
	    {
		    
	    }

	    public virtual bool IsUsingPathfindingForMovement()
	    {
	    	return false;	
	    }
	    
	    public virtual void SetPlayerMove(bool _value)
	    {
            m_canCharacterMove = _value;
	    }

	    protected virtual void OnFloorDetected(RaycastHit hit)
	    {
	    	if (m_setFloorOnCheck)
		    	SetFloor(hit.transform.position.y);
	    }

	    public virtual bool CheckIsGrounded()
        {
	        RaycastHit hit;
	        // Does the ray intersect any objects excluding the player layer
	        if (Physics.Raycast(m_floorCheckerTrs.position, m_upVector * -1, out hit, m_raycastHeight, m_floorMask))
	        {
		        m_isGrounded = true;
		        OnFloorDetected(hit);
	        }
	        else
	        {
		        m_isGrounded = false;
	        }
	        
	        return m_isGrounded;
        }

        protected virtual void SetDirection(Quaternion _rotation)
        {

        }

	    protected virtual bool CanHandleMovementAnimation()
	    {
	    	return (!m_jumping && !m_landing && !m_forcingAnimation && !m_falling);
	    }

        protected virtual void HandleMovementAnimation(float _speed, bool _sprinting)
        {
	        if (CanHandleMovementAnimation())
            {
	            string anim = _speed > 0 ? (_sprinting ? VP_AnimationSetup.Characters.SPRINT : (_speed > m_runningThreshold ? VP_AnimationSetup.Characters.RUN : VP_AnimationSetup.Characters.WALK)) : VP_AnimationSetup.Characters.IDLE;
                
	            if (m_walkSpeedBasedOnInput && _speed <= m_runningThreshold)
	            {
		            PlayAnimation(anim, null, (_speed / m_runningThreshold));
		            return;
	            }
	 
	            PlayAnimation(anim);
            }
        }

	    protected virtual bool ButtSmashWasPressed()
	    {
		    return false;
	    }   
        
	    protected virtual bool ButtSmashIsPressed()
	    {
		    return false;
	    }


        protected virtual bool JumpWasPressed()
        {
	        return false;
        }   
        
        protected virtual bool JumpIsPressed()
        {
            return false;
        }

        protected virtual bool InteractWasPressed()
        {
            return false;
        }

        protected virtual bool IsSprinting()
        {
            return false;
        }

        protected virtual float GetVerticalValue()
        {
            return m_canCharacterMove ? 1 : 0f;
        }

        protected virtual float GetHorizontalValue()
        {
            return m_canCharacterMove ? 1 : 0f;
        }

        protected virtual bool CanCharacterMove()
        {
            return m_canCharacterMove;
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            CheckIsGrounded();

            CheckInteract(InteractWasPressed());

            bool canMove = CanCharacterMove();
            float h = canMove ? GetHorizontalValue() : 0f;
            float v = canMove ? GetVerticalValue() : 0f;

	        m_sprinting = m_canSprint && IsSprinting();

            ApplyMovement(h, v);
        }

        /// <summary>
        /// If raycast is not always desired, we can swap in children the _trying condition with raycast one
        /// </summary>
        /// <param name="_trying">Pressed input for Interaction?</param>

        protected virtual void CheckInteract(bool _trying)
        { 
	        if (!m_canInteract || m_blocked || !m_interactPoint)
                return;

            RaycastHit hit;
            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(m_interactPoint.position, m_interactPoint.forward, out hit, m_interactDistance, m_interactMask))
            {
#if UNITY_EDITOR
                Debug.DrawRay(m_interactPoint.position, m_interactPoint.forward * m_interactDistance, Color.red);
#endif
                Transform trs = hit.transform;
                if (CanObjectBeInteracted(trs, out VP_IActionable _actionable))                 
                {
                    CheckLookAtInteractableObject(trs);

                    if (_trying)
                    {
                        TriggerActionable(ref _actionable);
                    }
                }
            }
            else
            {
#if UNITY_EDITOR
                Debug.DrawRay(m_interactPoint.position, m_interactPoint.forward * m_interactDistance, Color.yellow);
#endif
            }
        }

        protected virtual void CheckLookAtInteractableObject(Transform _whereToLookAt)
        {
            if (m_lookAnimator && m_lookAnimator.m_canLookAt)
            {
                m_lookAnimator.SetTarget(_whereToLookAt);
            }
        }

        protected virtual void TriggerActionable(ref VP_IActionable _actionable)
        {          
            _actionable.OnAction(this.transform);
            OnCharacterInteract.Invoke(this, _actionable);
        }
        protected virtual bool CanObjectBeInteracted(Transform _object, out VP_IActionable _actionable)
        {
            if (_object.TryGetComponentInChildren<VP_IActionable>(out _actionable))
            {
                //VP_Debug.Log("Found Actionable");
                return true;
            }

            return false;
        }

	    protected virtual float CalculateCurrentSpeed()
	    {
	    	return  m_landing && !m_canMoveOnLand ? 0f : (m_sprinting && m_canSprint ? m_runningSpeed : m_movementSpeed);
	    }

        protected virtual void ApplyMovement(float h, float v)
        {
	        CheckJump(IsGrounded);
        }

        protected virtual void EndButtAttack()
	    {
		    if (m_useEvents)
			    OnButtSmashEvent.Invoke(this);
	        	
            m_buttAttack = false;
        }

	    protected virtual void RegularLand()
	    {
		    if (m_useEvents)
			    OnLandEvent.Invoke(this);
	    }
		
	    protected virtual bool CheckJumpToIdleConditions(bool _grounded)
	    {
	    	return _grounded;
	    }

	   

        protected virtual void CheckJump(bool _grounded)
        {
          //  Debug.LogFormat("Grounded{0}, _grounded{1}", IsGrounded, _grounded);

	        if (_grounded)
	        {
	        	if (m_falling && !m_jumping)
		        	EndFalling();
	        	
	        	if (m_jumping)
	        	{
		        	m_jumping = false;
		        	HandleJumpToIdleAnimation();
		        	if (m_buttAttack)
		        	{
			        	EndButtAttack();
			        	return;
		        	}
	            
		        	RegularLand();
	        	}
	        }
            

            //JUMP LOGIC:  https://docs.unity3d.com/ScriptReference/CharacterController.Move.html?_ga=2.147233271.1936709409.1605611681-1524690815.1604470128

            // Changes the height position of the player..
	        if (m_canJump && JumpWasPressed())
            {
                if (_grounded)
                {
                    if (CheckJumpConditions())
                    {
                        PerformJump();
                    }
                }
            }

	        if (m_canButtAttack && ButtSmashWasPressed())
	        {
	        	Debug.Log("Butt Smash");
		        if(!_grounded && !m_landing)
		        {
			        PerformJumpFlyingAction();
		        }
	        }

	        ClampVerticalMovement(_grounded);
            
	        if (IsFallingFromHeight(_grounded))
	        {
	        	SetFallingFromHeight();
	        }
        }

	    public virtual void EndFalling()
	    {
	    	m_falling = false;
	    	HandleJumpToIdleAnimation();
	    }

	    public virtual void SetFallingFromHeight()
	    {
	    	m_falling = true;
		    HandleFallingAnimation();
	    }

	    public virtual void ClampVerticalMovement(bool _grounded)
	    {
		    if (_grounded && m_verticalMovement < m_floorYPos)
		    {
			    m_verticalMovement = 0f;//m_floorYPos;
		    }
	    }

        public virtual bool CheckJumpConditions()
        {
            return (!m_landing || (m_canJumpOnLand && m_landing)) && !m_blocked && !m_interacting;
        }

        public virtual bool IsFallingFromHeight(bool _grounded)
        {
            if (!m_jumping && !_grounded && !m_landing && !m_falling)
            {
                RaycastHit hit;
                if (Physics.Raycast(m_floorCheckerTrs.position, m_upVector * -1, out hit, 100f, m_floorMask))
                {
                    return hit.distance >= m_fallingThreshold;
                }
                return true;
            }

            return false;
        }

        public virtual void PerformJump()
	    {
	    	
            VP_AudioManager.PlaySFXOneShot(m_jumpClip);
            m_jumping = true;
		    HandleStartJumpAnimation();
		    
		    if (m_useEvents)
		    	OnJumpEvent.Invoke(this);

            m_verticalMovement += Mathf.Sqrt(m_jumpForce * m_jumpMaxHeight * Mathf.Abs(GetGravityUpValue()));
		    Debug.Log("Performed Jump at Y "+m_mainObject.position.y);
        }

	    public virtual void SetFloor(float _floorYPos)
	    {
	    	m_floorYPos = _floorYPos;
	    }

	    public virtual void UpdateCharacterMovementExternally()
	    {
	    	
	    }

	    public virtual bool CheckIsRunning(float _speed, bool _setIsRunning = true)
	    {
	    	if (_setIsRunning)
	    	{
		    	m_isRunning = _speed >= m_runningThreshold;
		    	return m_isRunning;
	    	}
		    else
		    {
		    	return _speed >= m_runningThreshold;
		    }
	    }

	    public virtual void ForceJumpAndButtAttack(float _maxHeight, UnityEngine.Events.UnityAction<VP_CharacterController> _OnJumpPerformedCallback = null, UnityEngine.Events.UnityAction<VP_CharacterController> _OnFlyPerformedCallback = null, UnityEngine.Events.UnityAction<VP_CharacterController> _buttCallback = null, UnityEngine.Events.UnityAction<VP_CharacterController> _regularLandCallback = null)
	    {
	    	
	    }

	    protected virtual Vector3 CalculateCameraBasedMoveDirection(float h, float v)
	    {
	    	return Vector3.zero;
	    }
	    
	    protected virtual Vector3 CalculateRegularMoveDirection(float h, float v)
	    {
		    return Vector3.zero;
	    }

	    public virtual bool GoToPoint(Vector3 _point, bool _unlock)
	    {
		    Debug.Log("Go To Point");
		    
	    	return true;
	    }

	    protected virtual void PlayAnimation(string _animation, UnityEngine.Events.UnityAction _callback = null, float _speed = -1, int _layer = -1, string _layerName = "Base Layer")
	    {
		    m_animator.PlayAnimatorState(_animation, _layer, _layerName);
  
	        if (_callback != null)
		        _callback.Invoke();
        }

        protected virtual void HandleJumpToIdleAnimation()
        {
            m_landing = true;
            PlayAnimation(VP_AnimationSetup.Characters.LAND, () =>
            {
                m_landing = false;
                PlayAnimation(VP_AnimationSetup.Characters.IDLE);
            });

        }

	    protected virtual void HandleFallingAnimation()
	    {
	    	m_falling = true;
	    	PlayAnimation(VP_AnimationSetup.Characters.FALL);
	    }
	    
        protected virtual void HandleStartJumpAnimation()
	    {
		    m_startJump = true;
            PlayAnimation(VP_AnimationSetup.Characters.JUMP, () =>
            {
	            m_startJump = false;
            	
                if (!IsGrounded)
                {
                	PlayAnimation(VP_AnimationSetup.Characters.FALL);
                }
                else
                {
                	Debug.Log("land directly bc grounded");
                	PlayAnimation(VP_AnimationSetup.Characters.LAND);
                }
            });
        }

        protected virtual void PerformJumpFlyingAction()
        {
            if (m_canButtAttack)
            {
            	if (m_useEvents)
	            	OnFlyingActionEvent.Invoke(this);
	            	
                m_buttAttack = true;
                //VP_Debug.Log("Performed Flying Action with Value: " + Mathf.Sqrt(m_jumpForce * m_jumpMaxHeight * Mathf.Abs(m_forceFallValue)));
                m_verticalMovement -= Mathf.Sqrt(m_jumpForce * m_jumpMaxHeight * Mathf.Abs(m_forceFallValue));
	            HandleFallingAnimation();
            }
        }

	    public virtual bool SamplePosition(Vector3 position)
	    {
	    	return true;
	    }
	    
        public virtual float GetGravityUpValue()
        {
            return m_gravity;
        }

	    public virtual void UpdateRotation(bool update)
	    {

	    }

        public virtual Vector3 GetUpVector()
        {
            return Vector3.up;
        }

	    public virtual Vector3 Velocity()
	    {
		    return m_moveDir.normalized * m_movementSpeed;
	    }

        protected virtual bool ResolveCanMove()
        {
            return m_canCharacterMove;
        }

        /// <summary>
        /// Mainly for AI NPC 
        /// </summary>
        /// <returns></returns>
        public virtual bool HasArrivedToTarget()
	    {
	    	return false;
	    }
	    
	    public virtual bool HasPath()
	    {
	    	return false;
	    }
	    
	    public virtual void OnEndPath(bool _stop)
	    {

	    }
	    
	    public virtual void StopAIMovement()
	    {

	    }
	    
	    public virtual void SetAgentSpeed()
	    {

	    }

        public virtual bool UseVPCharacterControllerToHandleAIMovement()
        {
            return false;
        }
    }

}
