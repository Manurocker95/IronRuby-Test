using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Controllers
{
    [DefaultExecutionOrder(VP_ExecutingOrderSetup.CHARACTER_CONTROLLER), AddComponentMenu(VP_PrefixSetup.MAIN_PREFIX + "/Controllers/Player Controller With Character Controller")]
    public class VP_SimpleCharacterController : VP_PlayableCharacterController
    {
        [Header("Simple Character"), Space]
        [SerializeField] protected CharacterController m_characterController;


        protected override void Reset()
        {
            base.Reset();

            m_characterController = GetComponentInChildren<CharacterController>();
        }

	    protected override void Initialize()
	    {
	    	base.Initialize();
	    	
	    	if (m_camera == null && m_basedOnCamera)
	    	{
	    		m_camera = Camera.main;
	    	}
	    }

	    protected override Vector3 CalculateCameraBasedMoveDirection(float h, float v)
	    {
		    if (m_camera == null)
	    	{
	    		m_camera = Camera.main;
	    	}
        	
            Vector3 camForward_Dir = Vector3.Scale(m_camera.transform.forward, new Vector3(1, 0, 1)).normalized;
            Vector3 move = m_invertXZ ? (v * camForward_Dir + h * m_camera.transform.right) : (h * camForward_Dir + v * m_camera.transform.right);

            if (move.magnitude > 1f)
                move.Normalize();

            // Calculate the rotation for the player
            move = m_mainObject.InverseTransformDirection(move);

            // Get Euler angles
            float turnAmount = (!m_landing || (m_landing && m_canRotateOnLand)) ? Mathf.Atan2(move.x, move.z) : 0f;

            m_mainObject.Rotate(0, turnAmount * m_rotationSpeed * Time.deltaTime, 0);

            move = m_mainObject.forward * move.magnitude;
            return move;
        }

	    protected override Vector3 CalculateRegularMoveDirection(float h, float v)
        {
            Vector3 move;

            move = new Vector3(h, 0f, v);
            move = m_mainObject.TransformDirection(m_moveDir);

            // Get Euler angles
            m_mainObject.Rotate(0, h * m_rotationSpeed * Time.deltaTime, 0);

            return move;
        }

	  

        protected override void ApplyMovement(float h, float v)
        {
	        bool grounded = IsGrounded;
            
            if (grounded || (m_jumping && m_canMoveOnJump))
            {
                if (m_basedOnCamera)
                {

                    m_moveDir = CalculateCameraBasedMoveDirection(h, v);
                }
                else
                {
                    m_moveDir = CalculateRegularMoveDirection(h, v);
                }

	            m_currentSpeed = CalculateCurrentSpeed();

                float _speed = Mathf.Max(Mathf.Abs(h), Mathf.Abs(v));
	            CheckIsRunning(_speed);

                HandleMovementAnimation(_speed, m_sprinting);

                m_moveDir *= m_currentSpeed;

            }

	        m_verticalMovement -= m_gravity * Time.deltaTime;

            //Check jump AFTER gravity, because check jump clamps the vertical speed at 0 when grounded.
            CheckJump(grounded);

            if (m_characterController != null && m_characterController.enabled)
            {
                m_characterController.Move(m_moveDir * Time.deltaTime);
	            m_characterController.Move(m_upVector * m_verticalMovement * Time.deltaTime);
            }
        }

    }
}
