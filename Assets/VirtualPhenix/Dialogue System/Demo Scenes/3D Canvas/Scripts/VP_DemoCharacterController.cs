using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Dialog.Demo
{
    public class VP_DemoCharacterController : MonoBehaviour
    {
        [Header("Component References"), Space(10)]
        [SerializeField] private Animator m_animator;
        [SerializeField] private CharacterController m_characterController;

        [Header("Interaction Properties"), Space(10)]
        [SerializeField] private Transform m_detectionTrs;
        [SerializeField] private LayerMask m_interactionLayers;
        [SerializeField] private float m_detectionDistance = 2f;


        [Header("Movement Properties"), Space(10)]
        [SerializeField] private float m_movementSpeed = 5.0f;
        [SerializeField] private float m_runningSpeed = 7.0f;
        [SerializeField] private float m_rotationSpeed = 240.0f;

        [Header("Interact Tex"), Space(10)]
        [SerializeField] private Texture2D m_interactTexture;
        private Vector3 m_screenPos;

        private float m_gravity = 20.0f;
        private Vector3 m_moveDir = Vector3.zero;
        public bool m_canInteract = true;
        public bool m_canNowInteract = false;
        public bool m_interactBubble = false;

        private float m_screenWidth;
        private float m_screenHeight;

        public Camera m_camera;
        [SerializeField] private Rigidbody m_rb;

        void Awake()
        {
            if (!m_animator)
                m_animator = GetComponent<Animator>();

            if (!m_characterController)
                m_characterController = GetComponent<CharacterController>();

            if (!m_detectionTrs)
                m_detectionTrs = this.transform;

            m_screenWidth = Screen.width;
            m_screenHeight = Screen.height;

            if (!m_interactTexture)
                m_interactTexture = null;

            if (!m_rb)
                m_rb = GetComponent<Rigidbody>();

            if (m_movementSpeed == 0.0f)
                m_movementSpeed = 5.0f;

            if (m_rotationSpeed == 0.0f)
                m_rotationSpeed = 240.0f;

            if (m_runningSpeed == 0.0f)
                m_runningSpeed = 7.0f;

            if (!m_camera)
                m_camera = Camera.main;

#if USE_CINEMACHINE
            Cinemachine.CinemachineCore.GetInputAxis = HandleAxisInputDelegate;
#endif
        }

        private void Start()
        {
            m_canInteract = true;
        }

#if USE_CINEMACHINE
        float HandleAxisInputDelegate(string axisName)
        {
            switch (axisName)
            {

                case "Mouse X":
                    return Input.GetAxis(axisName) + (VP_Utils.CheckIfInputAxisExists("GamepadRightStickHorizontal") ? Input.GetAxis("GamepadRightStickHorizontal") : 0f);

                case "Mouse Y":
                    return Input.GetAxis(axisName) + (VP_Utils.CheckIfInputAxisExists("GamepadRightStickVertical") ? Input.GetAxis("GamepadRightStickVertical") : 0f);

                default:
                    Debug.LogError("Input <" + axisName + "> not recognyzed.", this);
                    break;
            }

            return 0;
        }

#endif

            public void SetPhysics(bool _value)
        {
            m_rb.isKinematic = !_value;
            m_rb.useGravity = _value;
        }

        void StartAllListeners()
        {
            VP_DialogManager.StartListeningToOnDialogEnd(StartCanInteractDelay);
            VP_DialogManager.StartListeningToOnDialogStart(() => { m_canInteract = false; });
        }

        void StopAllListeners()
        {
            VP_DialogManager.StopListeningToOnDialogEnd(StartCanInteractDelay);
            VP_DialogManager.StopListeningToOnDialogStart(() => { m_canInteract = false; });
        }

        private void OnEnable()
        {
            StartAllListeners();
        }

        private void OnDisable()
        {
            StopAllListeners();
        }

        public void StartCanInteractDelay()
        {
            StartCoroutine(SmallInteractDelay());
        }

        IEnumerator SmallInteractDelay(float _delay = 0.35f)
        {
            m_canInteract = false;
            float timer = 0;
            while (timer < _delay)
            {
                timer += Time.deltaTime;
                yield return null;
            }
            m_canInteract = true;
        }

        private void LateUpdate()
        {
            if (!m_canInteract)
                return;

            RaycastHit hit;
            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(m_detectionTrs.position, m_detectionTrs.TransformDirection(Vector3.forward), out hit, m_detectionDistance, m_interactionLayers.value))
            {
#if UNITY_EDITOR
                Debug.DrawRay(m_detectionTrs.position, m_detectionTrs.forward * m_detectionDistance, Color.red);
#endif
                VP_DemoInteraction npc = hit.transform.GetComponent<VP_DemoInteraction>();
                if (npc.CanBeInteracted)
                {
                    m_canNowInteract = true;
                    m_interactBubble = npc.ShowInteractBubble;
                    m_screenPos = Camera.main.WorldToScreenPoint(npc.UIRefPos.position);

                    if (VP_DemoInputManager.PressedInteract() && m_canInteract)
                    {
                        m_canInteract = false;
                        npc.OnInteraction(this);
                    }
                }
                else
                {
                    m_interactBubble = false;
                }
            }
            else
            {
                m_canNowInteract = false;
#if UNITY_EDITOR
                Debug.DrawRay(m_detectionTrs.position, m_detectionTrs.forward * m_detectionDistance, Color.yellow);
#endif
            }
        }

        void OnGUI()
        {
            if (m_canNowInteract && m_canInteract && m_interactBubble)
            {
                GUI.DrawTexture(new Rect(m_screenPos.x - 50, Screen.height - m_screenPos.y - 50, 100, 100), m_interactTexture);
            }
        }

        private void Update()
        {
            if (!m_camera)
                return;

            float h = (VP_DemoInputManager.HorizontalPlayerMovement());
            float v = (VP_DemoInputManager.VerticalPlayerMovement());

            // Calculate the forward vector
            Vector3 camForward_Dir = Vector3.Scale(m_camera.transform.forward, new Vector3(1, 0, 1)).normalized;
            Vector3 move = v * camForward_Dir + h * m_camera.transform.right;

            if (move.magnitude > 1f) move.Normalize();

            // Calculate the rotation for the player
            move = transform.InverseTransformDirection(move);

            // Get Euler angles
            float turnAmount = Mathf.Atan2(move.x, move.z);

            transform.Rotate(0, turnAmount * m_rotationSpeed * Time.deltaTime, 0);

            if (m_characterController.isGrounded)
            {
                m_animator.SetBool("isFooting", move.magnitude > 0);
                float _speed = Mathf.Max(Mathf.Abs(h), Mathf.Abs(v));
                m_animator.SetFloat("speed", _speed);
                m_moveDir = transform.forward * move.magnitude;

                m_moveDir *= m_movementSpeed;

            }

            m_moveDir.y -= m_gravity * Time.deltaTime;

            m_characterController.Move(m_moveDir * Time.deltaTime);


        }
    }

}
