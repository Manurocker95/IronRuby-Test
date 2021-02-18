using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix.FlyingSystem;

namespace VirtualPhenix.Controllers
{
    [DefaultExecutionOrder(VP_ExecutingOrderSetup.CHARACTER_CONTROLLER), AddComponentMenu(VP_PrefixSetup.MAIN_PREFIX + "/Flying System/Controllers/Flying Player Controller")]
    public class VP_FlyingCharacterController : VP_PlayableCharacterController
    {
        /*
        public enum CHARACTER_STATE
        {
            GROUNDED, //on ground
            IN_AIR, //in the air
            FLYING, //trying to fly
            STUNNED, //on a wall
            NONE,
        }

        [SerializeField] protected CHARACTER_STATE m_state = CHARACTER_STATE.GROUNDED;
        [SerializeField] protected VP_FlyingVisuals m_flyingVisuals; 
        [SerializeField] protected VP_FlyingCollisionDetector m_collisionDetector; //collision detection
        [SerializeField] protected Rigidbody m_rigidBody; //rigidbody 

        [Header("Physics")]
        [SerializeField] protected float m_handleReturnSpeed; //how quickly our handle on our character is returned to normal after a force is added (such as jumping
        protected float m_actualGravityAppliedToCharacter; //the actual gravity that is applied to our character
        protected float m_floorTimer; //how long we are on the floor
        protected float m_actionAirTimer; //the air timer counting our current actions performed in air

        [Header("Stats")]
        [SerializeField] protected float m_flyingmaxSpeed = 15f; //max speed for basic movement
        [SerializeField] protected float m_flyingSpeedClamp = 50f; //max possible speed
        protected float m_flyingActAcceleration; //our actual acceleration
        [SerializeField] protected float Acceleration = 4f; //how quickly we build speed
        [SerializeField] protected float MovementAcceleration = 20f;    //how quickly we adjust to new speeds
        [SerializeField] protected float SlowDownAcceleration = 2f; //how quickly we slow down
        [SerializeField] protected float turnSpeed = 2f; //how quickly we turn on the ground
        protected float FlownAdjustmentLerp = 1; //if we have flown this will be reset at 0, and effect turn speed on the ground
        protected Vector3 m_movepos;
        protected Vector3 m_targetDir;
        protected Vector3 m_downwardDirection; //where to move to

        [Header("Falling")]
        public float AirAcceleration = 5f;  //how quickly we adjust to new speeds when in air
        public float turnSpeedInAir = 2f;
        public float FallingDirectionSpeed = 0.5f; //how quickly we will return to a normal direction

        [Header("Flying")]
        public float FlyingDirectionSpeed = 2f; //how much influence our direction relative to the camera will influence our flying
        public float FlyingRotationSpeed = 6f; //how fast we turn in air overall
        public float FlyingUpDownSpeed = 0.1f; //how fast we rotate up and down
        public float FlyingLeftRightSpeed = 0.1f;  //how fast we rotate left and right
        public float FlyingRollSpeed = 0.1f; //how fast we roll

        public float FlyingAcceleration = 4f; //how much we accelerate to max speed
        public float FlyingDecelleration = 1f; //how quickly we slow down when flying
        public float FlyingSpeed; //our max flying speed
        public float FlyingMinSpeed; //our flying slow down speed

        public float FlyingAdjustmentSpeed; //how quickly our velocity adjusts to the flying speed
        private float FlyingAdjustmentLerp = 0; //the lerp for our adjustment amount

        [Header("Flying Physics")]
        public float FlyingGravityAmt = 2f; //how much gravity will pull us down when flying
        public float GlideGravityAmt = 4f; //how much gravity affects us when just gliding
        public float FlyingGravBuildSpeed = 3f; //how much our gravity is lerped when stopping flying

        public float FlyingVelocityGain = 2f; //how much velocity we gain for flying downwards
        public float FlyingVelocityLoss = 1f; //how much velocity we lose for flying upwards
        public float FlyingLowerLimit = -6f; //how much we fly down before a boost
        public float FlyingUpperLimit = 4f; //how much we fly up before a boost;
        public float GlideTime = 10f; //how long we glide for when not flying before we start to fall

        [Header("Jumps")]
        public float JumpAmt; //how much we jump upwards 
        private bool HasJumped; //if we have pressed jump
        public float GroundedTimerBeforeJump = 0.2f; //how long we have to be on the floor before an action can be made
        public float JumpForwardAmount = 5f; //how much our regular jumps move us forward

        [Header("Wall Impact")]
        public float SpeedLimitBeforeCrash; //how fast we have to be going to crash
        public float StunPushBack;  //how much we are pushed back
        public float StunnedTime; //how long we are stunned for
        private float StunTimer; //the in use stun timer

        [Header("Visuals")]
        private Transform HipsPos; //where our characters hips are
        private bool MirrorAnim; //if anims should be mirroed
        private float XAnimFloat; //the float for our wing turning
        private float RunTimer; //animation ctrl for running
        private float FlyingTimer; //the time before the animation stops flying

        // Start is called before the first frame update
        protected override void Initialize()
        {
            base.Initialize();

            //static until finished setup
            m_state = CHARACTER_STATE.NONE;

            if (!m_collisionDetector)
                transform.TryGetComponentInChildren<VP_FlyingCollisionDetector>(out m_collisionDetector);

            if (!m_flyingVisuals)
                m_flyingVisuals = GetComponent<VP_FlyingVisuals>();

            HipsPos = Visuals.HipsPos;


        }

        protected void Update()   //inputs and animation
        {
            //cannot function when dead
            if (States == WorldState.Static)
                return;

            //control the animator
            AnimCtrl();

            transform.position = Rigid.position;

            //check for jumping
            if (States == WorldState.Grounded)
            {
                if (FloorTimer > 0)
                    return;

                //check for ground
                bool Ground = Colli.CheckGround();

                if (!Ground)
                {
                    SetInAir();
                    return;
                }

                if (InputHand.Jump)
                {
                    //if the player can jump, isnt attacking and isnt using an item
                    if (!HasJumped)
                    {
                        if (Anim)
                        {
                            MirrorAnim = !MirrorAnim;
                            Anim.SetBool("Mirror", MirrorAnim);
                        }

                        Visuals.Jump();

                        float AddAmt = Mathf.Clamp((ActSpeed * 0.5f), -10, 16);
                        float ForwardAmt = Mathf.Clamp(ActSpeed * 4f, JumpForwardAmount, 100);

                        StopCoroutine(JumpUp(ForwardAmt, JumpAmt + AddAmt));
                        StartCoroutine(JumpUp(ForwardAmt, JumpAmt + AddAmt));

                        return;
                    }
                }
            }
            else if (States == WorldState.InAir)
            {
                if (ActionAirTimer > 0) //reduce air timer 
                    return;

                if (HasJumped) //cannot switch to flying until jump is done
                    return;

                if (InputHand.Fly)  //switch to flying
                    SetFlying();


                //check for ground
                bool Ground = Colli.CheckGround();

                if (Ground)
                {
                    SetGrounded();
                    return;
                }
            }
            else if (States == WorldState.Flying)
            {
                if (ActionAirTimer > 0) //reduce air timer 
                    return;

                //check wall collision for a crash, if this unit can crash
                bool WallHit = Colli.CheckWall();

                //if we have hit a wall
                if (WallHit)
                {
                    //if we are going fast enough to crash into a wall
                    if (ActSpeed > SpeedLimitBeforeCrash)
                    {
                        //stun character
                        Stunned(-transform.forward);
                        return;
                    }
                }

                //check for ground if we are not holding the flying button
                if (!InputHand.Fly)
                {
                    bool Ground = Colli.CheckGround();

                    if (Ground)
                    {
                        SetGrounded();
                        return;
                    }
                }
            }
        }

        // Update is called once per frame
        protected virtual void FixedUpdate()  //world movement
        {
            //tick deltatime
            float delta = Time.deltaTime;

            //get velocity to feed to the camera
            float CamVel = 0;
            if (m_rigidBody != null)
                CamVel = m_rigidBody.velocity.magnitude;
            //change the cameras fov based on speed
            m_camera.HandleFov(delta, CamVel);

            //cannot function when dead
            if (m_state == CHARACTER_STATE.NONE)
            {
                //turn off air sound
                m_flyingVisuals.WindAudioSetting(delta * 3f, 0f);

                return;
            }
            //tick any fixed camera controls
            FixedAnimCtrl(delta);

            //control our direction slightly when falling
            float _xMov = InputHand.Horizontal;
            float _zMov = InputHand.Vertical;

            //get our direction of input based on camera position
            Vector3 screenMovementForward = CamY.transform.forward;
            Vector3 screenMovementRight = CamY.transform.right;
            Vector3 screenMovementUp = CamY.transform.up;

            Vector3 h = screenMovementRight * _xMov;
            Vector3 v = screenMovementForward * _zMov;

            Vector3 moveDirection = (v + h).normalized;

            switch (m_state)
            {
                case CHARACTER_STATE.GROUNDED:
                    //turn off wind audio
                    if (Visuals.WindLerpAmt > 0)
                        Visuals.WindAudioSetting(delta * 3f, 0f);

                    float LSpeed = MaxSpeed;
                    float Accel = Acceleration;
                    float MoveAccel = MovementAcceleration;

                    //reduce floor timer
                    if (FloorTimer > 0)
                        FloorTimer -= delta;

                    if (InputHand.Horizontal == 0 && InputHand.Vertical == 0)
                    {
                        //we are not moving, lerp to a walk speed
                        LSpeed = 0f;
                        Accel = SlowDownAcceleration;
                    }
                    //lerp our current speed
                    if (ActSpeed > LSpeed - 0.5f || ActSpeed < LSpeed + 0.5f)
                        LerpSpeed(delta, LSpeed, Accel);
                    //move our character
                    MoveSelf(delta, ActSpeed, MoveAccel, moveDirection);
                    break;
                case CHARACTER_STATE.IN_AIR:
                    //reduce air timer 
                    if (ActionAirTimer > 0)
                        ActionAirTimer -= delta;

                    //falling effect
                    Visuals.FallEffectCheck(delta);

                    //falling audio
                    Visuals.WindAudioSetting(delta, Rigid.velocity.magnitude);

                    //slow our flying control if we were not
                    if (FlyingAdjustmentLerp > -.1)
                        FlyingAdjustmentLerp -= delta * (FlyingAdjustmentSpeed * 0.5f);

                    //control our character when falling
                    FallingCtrl(delta, ActSpeed, AirAcceleration, moveDirection);
                    break;
                case CHARACTER_STATE.FLYING:
                    //setup gliding
                    if (!InputHand.Fly)
                    {
                        if (FlyingTimer > 0) //reduce flying timer 
                            FlyingTimer -= delta;
                    }
                    else if (FlyingTimer < GlideTime)
                    {
                        //flapping animation
                        if (FlyingTimer < GlideTime * 0.8f)
                            Anim.SetTrigger("Flap");

                        FlyingTimer = GlideTime;
                    }

                    //reduce air timer 
                    if (ActionAirTimer > 0)
                        ActionAirTimer -= delta;

                    //falling effect
                    Visuals.FallEffectCheck(delta);

                    //falling audio
                    Visuals.WindAudioSetting(delta, Rigid.velocity.magnitude);

                    //lerp controls
                    if (FlyingAdjustmentLerp < 1.1)
                        FlyingAdjustmentLerp += delta * FlyingAdjustmentSpeed;

                    //lerp speed
                    float YAmt = Rigid.velocity.y;
                    float FlyAccel = FlyingAcceleration * FlyingAdjustmentLerp;
                    float Spd = FlyingSpeed;
                    if (!InputHand.Fly)  //we are not holding fly, slow down
                    {
                        Spd = FlyingMinSpeed;
                        if (ActSpeed > FlyingMinSpeed)
                            FlyAccel = FlyingDecelleration * FlyingAdjustmentLerp;
                    }
                    else
                    {
                        //flying effects 
                        Visuals.FlyingFxTimer(delta);
                    }

                    HandleVelocity(delta, Spd, FlyAccel, YAmt);

                    //flying controls
                    FlyingCtrl(delta, ActSpeed, _xMov, _zMov);
                    break;
                case CHARACTER_STATE.STUNNED:
                    //reduce stun timer
                    if (StunTimer > 0)
                    {
                        StunTimer -= delta;

                        if (StunTimer > StunnedTime * 0.5f)
                            return;
                    }


                    //switch to air
                    bool Ground = m_collisionDetector.CheckGround();

                    //reduce ground timer
                    if (Ground)
                    {
                        if (Anim)
                            Anim.SetBool("Stunned", false);

                        //get up from ground
                        if (StunTimer <= 0)
                        {
                            SetGrounded();
                            return;
                        }
                    }

                    //lerp mesh slower when not on ground
                    RotateSelf(DownwardDirection, delta, 8f);
                    RotateMesh(delta, transform.forward, turnSpeed);

                    //push backwards while we fall
                    Vector3 FallDir = -transform.forward * 4f;
                    FallDir.y = Rigid.velocity.y;
                    Rigid.velocity = Vector3.Lerp(Rigid.velocity, FallDir, delta * 2f);

                    //falling audio
                    Visuals.WindAudioSetting(delta, Rigid.velocity.magnitude);
                    break;
            }
        }
        //for when we return to the ground
        public void SetGrounded()
        {
            Visuals.Landing();

            //reset wind animation
            Visuals.SetFallingEffects(1.6f);

            //reset flying animation 
            FlyingTimer = 0;
            //reset flying adjustment
            FlyingAdjustmentLerp = 0;

            //reset physics and jumps
            DownwardDirection = Vector3.up;
            m_state = CHARACTER_STATE.GROUNDED; //on ground
            OnGround = true;

            //camera reset flying state
            CamFol.SetFlyingState(0);

            //turn on gravity
            if (m_rigidBody)
                m_rigidBody.useGravity = true;
        }
        //for when we are set in the air (for falling
        void SetInAir()
        {
            OnGround = false;
            FloorTimer = GroundedTimerBeforeJump;
            ActionAirTimer = 0.2f;

            m_state = CHARACTER_STATE.IN_AIR

            //camera reset flying state
            CamFol.SetFlyingState(0);

            //turn off gravity
            if (m_rigidBody)
                m_rigidBody.useGravity = true;
        }
        //for when we start to fly
        void SetFlying()
        {
            m_state = CHARACTER_STATE.FLYING;

            //set animation 
            FlyingTimer = GlideTime;

            ActGravAmt = 0f; //our gravity is returned to the flying amount

            FlownAdjustmentLerp = -1;

            //camera set flying state
            CamFol.SetFlyingState(1);

            //turn on gravity
            if (m_rigidBody != null)
                m_rigidBody.useGravity = false;
        }
        //stun our character
        public void Stunned(Vector3 PushDirection)
        {
            if (Anim)
                Anim.SetBool("Stunned", true);

            StunTimer = StunnedTime;

            //set physics
            ActSpeed = 0f;
            m_rigidBody.velocity = Vector3.zero;
            m_rigidBody.AddForce(PushDirection * StunPushBack, ForceMode.Impulse);
            DownwardDirection = Vector3.up;
            m_state = CHARACTER_STATE.STUNNED;

            //turn on gravity
            if (m_rigidBody != null)
                m_rigidBody.useGravity = true;
        }

        void AnimCtrl()
        {
            //setup the location of any velocity based animations from our hip position 
            Transform RelPos = this.transform;

            //find animations based on our hip position (for flying velocity animations
            if (HipsPos)
                RelPos = HipsPos;
            //get movement amounts in each direction
            Vector3 RelVel = RelPos.transform.InverseTransformDirection(Rigid.velocity);
            Anim.SetFloat("forwardMove", RelVel.z);
            Anim.SetFloat("sideMove", RelVel.x);
            Anim.SetFloat("upwardsMove", RelVel.y);
            //our rigidbody y amount (for upwards or downwards velocity animations
            Anim.SetFloat("YVel", Rigid.velocity.y);

            //set movement animator
            RunTimer = new Vector3(Rigid.velocity.x, 0, Rigid.velocity.z).magnitude;
            Anim.SetFloat("Moving", RunTimer);

            //set our grounded and flying animations
            Anim.SetBool("OnGround", OnGround);
            bool Fly = true;
            if (!InputHand.Fly)
                Fly = false;

            Anim.SetBool("Flying", Fly);
        }

        void FixedAnimCtrl(float D) //animations involving a timer
        {
            //setup the xinput animation for tilting our wings left and right
            float LAMT = InputHand.Horizontal;
            XAnimFloat = Mathf.Lerp(XAnimFloat, LAMT, D * 4f);
            Anim.SetFloat("XInput", XAnimFloat);
        }

        IEnumerator JumpUp(float ForwardAmt, float UpwardsAmt)
        {
            HasJumped = true;
            //kill velocity
            Rigid.velocity = Vector3.zero;
            //set to in air as we will be 
            SetInAir();
            //add force upwards
            if (UpwardsAmt != 0)
                Rigid.AddForce((Vector3.up * UpwardsAmt), ForceMode.Impulse);
            //add force forwards
            if (ForwardAmt != 0)
                Rigid.AddForce((transform.forward * ForwardAmt), ForceMode.Impulse);
            //remove any built up acceleration
            ActAccel = 0;
            //stop jump state
            yield return new WaitForSecondsRealtime(0.3f);
            HasJumped = false;
        }
        //lerp our speed over time
        void LerpSpeed(float d, float TargetSpeed, float Accel)
        {
            //if our speed is larger than our max speed, reduce it slowly 
            if (m_currentSpeed > m_flyingmaxSpeed)
            {
                m_currentSpeed = Mathf.Lerp(m_currentSpeed, TargetSpeed, d * Accel * 0.5f);
            }
            else
            {
                if (TargetSpeed > 0.5)
                {
                    //influence by x and y input 
                    float Degree = Vector3.Magnitude(new Vector3(InputHand.Horizontal, InputHand.Vertical, 0).normalized);
                    m_currentSpeed = Mathf.Lerp(m_currentSpeed, TargetSpeed, (d * Accel) * Degree);
                }
                else
                    m_currentSpeed = Mathf.Lerp(m_currentSpeed, TargetSpeed, d * Accel);


            }
            //clamp our speed
            m_currentSpeed = Mathf.Clamp(m_currentSpeed, 0, m_flyingSpeedClamp);
        }

        //handle how our speed is increased or decreased when flying
        void HandleVelocity(float d, float TargetSpeed, float Accel, float YAmt)
        {
            if (m_currentSpeed > FlyingSpeed) //we are over out max speed, slow down slower
                Accel = Accel * 0.8f;

            if (YAmt < FlyingLowerLimit) //we are flying down! boost speed
            {
                TargetSpeed = TargetSpeed + (FlyingVelocityGain * (YAmt * -0.5f));
            }
            else if (YAmt > FlyingUpperLimit) //we are flying up! reduce speed
            {
                TargetSpeed = TargetSpeed - (FlyingVelocityLoss * YAmt);
                m_currentSpeed -= (FlyingVelocityLoss * YAmt) * d;
            }
            //clamp speed
            TargetSpeed = Mathf.Clamp(TargetSpeed, -m_flyingSpeedClamp, m_flyingSpeedClamp);
            //lerp speed
            m_currentSpeed = Mathf.Lerp(m_currentSpeed, TargetSpeed, Accel * d);
        }
        //boost our speed
        public void SpeedBoost(float Amt)
        {
            m_currentSpeed += Amt;
        }

        void MoveSelf(float d, float Speed, float Accel, Vector3 moveDirection)
        {
            if (moveDirection == Vector3.zero)
            {
                m_targetDir = transform.forward;
            }
            else
            {
                m_targetDir = moveDirection;
            }

            if (m_targetDir == Vector3.zero)
            {
                m_targetDir = transform.forward;
            }
            //turn ctrl
            Quaternion lookDir = Quaternion.LookRotation(m_targetDir);
            //turn speed after flown is reduced
            if (FlownAdjustmentLerp < 1)
                FlownAdjustmentLerp += Time.deltaTime * 2f;
            //set our turn speed
            float TurnSpd = (turnSpeed + (m_currentSpeed * 0.1f)) * FlownAdjustmentLerp;
            TurnSpd = Mathf.Clamp(TurnSpd, 0, 6);

            //lerp mesh slower when not on ground
            RotateSelf(m_downwardDirection, d, 8f);
            RotateMesh(d, m_targetDir, TurnSpd);

            //move character
            float Spd = Speed;
            Vector3 curVelocity = m_rigidBody.velocity;
            Vector3 MovDirection = transform.forward;

            if (moveDirection == Vector3.zero) //if we are not pressing a move input we move towards velocity //or are crouching
            {
                Spd = Speed * 0.8f; //less speed is applied to our character
            }

            Vector3 targetVelocity = MovDirection * Spd;
            //accelerate our character
            m_flyingActAcceleration = Mathf.Lerp(m_flyingActAcceleration, Accel, m_handleReturnSpeed * d);
            //lerp our movement direction
            Vector3 dir = Vector3.Lerp(curVelocity, targetVelocity, d * m_flyingActAcceleration);
            dir.y = m_rigidBody.velocity.y;
            //set our rigibody direction
            m_rigidBody.velocity = dir;
        }

        void FallingCtrl(float d, float Speed, float Accel, Vector3 moveDirection)
        {
            if (moveDirection == Vector3.zero)
            {
                m_targetDir = transform.forward;
            }
            else
            {
                m_targetDir = moveDirection;
            }

            //rotate towards the rigid body velocity 
            Vector3 LerpDirection = m_downwardDirection;
            float FallDirSpd = FallingDirectionSpeed;

            if (m_rigidBody.velocity.y < -6) //we are going downwards
            {
                LerpDirection = Vector3.up;
                FallDirSpd = FallDirSpd * -(m_rigidBody.velocity.y * 0.2f);
            }

            m_downwardDirection = Vector3.Lerp(m_downwardDirection, LerpDirection, FallDirSpd * d);

            //lerp mesh slower when not on ground
            RotateSelf(m_downwardDirection, d, 8f);
            RotateMesh(d, transform.forward, turnSpeedInAir);

            //move character
            float Spd = Speed;
            Vector3 curVelocity = m_rigidBody.velocity;

            Vector3 targetVelocity = m_targetDir * Spd;

            //lerp our acceleration
            m_flyingActAcceleration = Mathf.Lerp(m_flyingActAcceleration, Accel, m_handleReturnSpeed * d);
            //set rigid direction
            Vector3 dir = Vector3.Lerp(curVelocity, targetVelocity, d * m_flyingActAcceleration);
            dir.y = m_rigidBody.velocity.y;
            m_rigidBody.velocity = dir;
        }

        void FlyingCtrl(float d, float Speed, float XMove, float ZMove)
        {
            //input direction 
            float InvertX = -1;
            float InvertY = -1;

            XMove = XMove * InvertX; //horizontal inputs
            ZMove = ZMove * InvertY; //vertical inputs

            //get direction to move character
            m_downwardDirection = VehicleFlyingDownwardDirection(d, ZMove);
            Vector3 SideDir = VehicleFlyingSideDirection(d, XMove);
            //get our rotation and adjustment speeds
            float rotSpd = FlyingRotationSpeed;
            float FlyLerpSpd = FlyingAdjustmentSpeed * FlyingAdjustmentLerp;

            //lerp mesh slower when not on ground
            RotateSelf(m_downwardDirection, d, rotSpd);
            RotateMesh(d, SideDir, rotSpd);

            if (FlyingTimer < GlideTime * 0.7f) //lerp to velocity if not flying
                RotateToVelocity(d, rotSpd * 0.05f);

            Vector3 targetVelocity = transform.forward * Speed;
            //push down more when not pressing fly
            if (m_inputActions.FlyIsPressed())
                m_actualGravityAppliedToCharacter = Mathf.Lerp(m_actualGravityAppliedToCharacter, FlyingGravityAmt, FlyingGravBuildSpeed * 4f * d);
            else
                m_actualGravityAppliedToCharacter = Mathf.Lerp(m_actualGravityAppliedToCharacter, GlideGravityAmt, FlyingGravBuildSpeed * 0.5f * d);

            targetVelocity -= Vector3.up * m_actualGravityAppliedToCharacter;
            //lerp velocity
            Vector3 dir = Vector3.Lerp(m_rigidBody.velocity, targetVelocity, d * FlyLerpSpd);
            m_rigidBody.velocity = dir;
        }

        Vector3 VehicleFlyingDownwardDirection(float d, float ZMove)
        {
            Vector3 VD = -transform.up;

            //up and down input = moving up and down (this effects our downward direction
            if (ZMove > 0.1) //upward tilt
            {
                VD = Vector3.Lerp(VD, -transform.forward, d * (FlyingUpDownSpeed * ZMove));
            }
            else if (ZMove < -.1) //downward tilt
            {
                VD = Vector3.Lerp(VD, transform.forward, d * (FlyingUpDownSpeed * (ZMove * -1)));
            }

            //LB and RB input = roll (this effects our downward direction
            if (m_inputActions.FlyingLeftRollWasPressed()) //left roll
            {
                VD = Vector3.Lerp(VD, -transform.right, d * FlyingRollSpeed);
            }
            else if (m_inputActions.FlyingRightRollWasPressed()) //right roll
            {
                VD = Vector3.Lerp(VD, transform.right, d * FlyingRollSpeed);
            }

            return VD;
        }

        protected virtual Vector3 VehicleFlyingSideDirection(float d, float XMove)
        {
            Vector3 RollDir = transform.forward;

            //rb lb = move left and right (this effects our target direction
            //left right input
            if (XMove > 0.1)
            {
                RollDir = Vector3.Lerp(RollDir, -m_mainObject.transform.right, d * (FlyingLeftRightSpeed * XMove));
            }
            else if (XMove < -.1)
            {
                RollDir = Vector3.Lerp(RollDir, m_mainObject.transform.right, d * (FlyingLeftRightSpeed * (XMove * -1)));
            }
            //bumper input
            if (m_inputActions.FlyingLeftRollWasPressed())
            {
                RollDir = Vector3.Lerp(RollDir, -m_mainObject.transform.right, d * FlyingLeftRightSpeed * 0.2f);
            }
            else if (m_inputActions.FlyingRightRollWasPressed())
            {
                RollDir = Vector3.Lerp(RollDir, m_mainObject.transform.right, d * FlyingLeftRightSpeed * 0.2f);
            }

            return RollDir;
        }
        //rotate our upwards direction
        protected virtual void RotateSelf(Vector3 Direction, float d, float GravitySpd)
        {
            Vector3 LerpDir = Vector3.Lerp(m_mainObject.transform.up, Direction, d * GravitySpd);
            m_mainObject.transform.rotation = Quaternion.FromToRotation(m_mainObject.transform.up, LerpDir) * transform.rotation;
        }
        //rotate our left right direction
        protected virtual void RotateMesh(float d, Vector3 LookDir, float spd)
        {
            Quaternion SlerpRot = Quaternion.LookRotation(LookDir, transform.up);
            m_mainObject.transform.rotation = Quaternion.Slerp(m_mainObject.transform.rotation, SlerpRot, spd * d);
        }
        //rotate towards the velocity direction
        protected virtual void RotateToVelocity(float d, float spd)
        {
            Quaternion SlerpRot = Quaternion.LookRotation(m_rigidBody.velocity.normalized);
            m_mainObject.transform.rotation = Quaternion.Slerp(m_mainObject.transform.rotation, SlerpRot, spd * d);
        }
        */
    }
}