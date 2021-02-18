using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Dialog.Demo
{
    public enum KEY_TYPE
    {
        DOWN,
        CONTINUOUS,
        UP
    }

    public class VP_DemoInputManager : MonoBehaviour
    {
        private static VP_DemoInputManager m_instance;
        public static VP_DemoInputManager Instance { get { return m_instance; } }

        [Header("Inputs")]
        [SerializeField] private KeyCode[] m_interactKeys = new KeyCode[] { KeyCode.Space, KeyCode.Joystick1Button0 };
        [SerializeField] private KeyCode m_upKey = KeyCode.W;
        [SerializeField] private KeyCode m_downKey = KeyCode.S;
        [SerializeField] private KeyCode m_rightKey = KeyCode.D;
        [SerializeField] private KeyCode m_leftKey = KeyCode.A;

        [Header("Availability")]
        [SerializeField] private bool m_canMove = true;
        [SerializeField] private bool m_availableInput = false;

        private void Awake()
        {
            if (!m_instance)
            {
                m_instance = this;
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        private void OnEnable()
        {
            VP_DialogManager.StartListeningToOnDialogStart(DisableMovement);
            VP_DialogManager.StartListeningToOnDialogEnd(AbleMovement);
        }

        private void OnDisable()
        {
            VP_DialogManager.StopListeningToOnDialogStart(DisableMovement);
            VP_DialogManager.StopListeningToOnDialogEnd(AbleMovement);
        }

        // Start is called before the first frame update
        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        void AbleMovement()
        {
            m_canMove = true;
        }

        void DisableMovement()
        {
            m_canMove = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

            if (m_availableInput)
            {
                foreach (KeyCode k in m_interactKeys)
                {
                    if (Input.GetKeyDown(k))
                        VP_EventManager.TriggerEvent(VP_EventSetup.Input.INTERACT_BUTTON);
                }

            }

            if (Input.GetKeyDown(m_upKey))
                VP_EventManager.TriggerEvent(VP_EventSetup.Input.UP_ANSWER);

            if (Input.GetKeyDown(m_downKey))
                VP_EventManager.TriggerEvent(VP_EventSetup.Input.DOWN_ANSWER);

            if (Input.GetKeyDown(m_rightKey))
                VP_EventManager.TriggerEvent(VP_EventSetup.Input.RIGHT_ANSWER);


            if (Input.GetKeyDown(m_leftKey))
                VP_EventManager.TriggerEvent(VP_EventSetup.Input.LEFT_ANSWER);
        }

        public static bool PressedInteract(KEY_TYPE _type = KEY_TYPE.DOWN)
        {
            if (!m_instance)
            {
                Debug.LogError("No Input manager in the current scene.");
                return false;
            }

            foreach (KeyCode k in m_instance.m_interactKeys)
            {
                if (m_instance.PressedButton(k, _type))
                {
                    return true;
                }
            }

            return false;
        }

        public static float HorizontalPlayerMovement()
        {
            if (!m_instance)
            {
                Debug.LogError("No Input manager in the current scene.");
                return 0f;
            }

            return m_instance.GetAxis("Horizontal") * (m_instance.m_canMove ? 1f : 0f);
        }

        public static float VerticalPlayerMovement()
        {
            if (!m_instance)
            {
                Debug.LogError("No Input manager in the current scene.");
                return 0f;
            }

            return m_instance.GetAxis("Vertical") * (m_instance.m_canMove ? 1f : 0f);
        }

        public bool PressedButton(KeyCode _keyCode, KEY_TYPE _type)
        {
            switch (_type)
            {
                case KEY_TYPE.DOWN:
                    return Input.GetKeyDown(_keyCode);
                case KEY_TYPE.CONTINUOUS:
                    return Input.GetKey(_keyCode);
                case KEY_TYPE.UP:
                    return Input.GetKeyUp(_keyCode);
                default:
                    return Input.GetKeyDown(_keyCode);
            }
        }

        public float GetAxis(string _axisName)
        {
            return Input.GetAxis(_axisName);
        }

        
    }

}
