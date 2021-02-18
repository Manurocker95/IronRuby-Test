using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Dialog
{
    [System.Serializable]
    public class VP_DialogInitializer : VP_MonoBehaviour, VP_IActionable
    {
        public enum INIT_ACTION
        {
            ON_TRIGGER_ENTER,
            ON_TRIGGER_EXIT,
            ON_TRIGGER_ENTER_2D,
            ON_TRIGGER_EXIT_2D,

            ON_COLLISION_ENTER,
            ON_COLLISION_EXIT,
            ON_COLLISION_ENTER_2D,
            ON_COLLISION_EXIT_2D,

            ON_PARTICLE_COLLISION,

            ON_MOUSE_HOVER,
            ON_MOUSE_ENTER,
            ON_MOUSE_EXIT,
            ON_MOUSE_DRAG,
            ON_MOUSE_DOWN,
            ON_MOUSE_UP,


            ON_BUTTON_DOWN,
            ON_BUTTON_UP,

            ON_KEY_DOWN,
            ON_KEY_UP,

            ON_ACTION_CALL,
            ON_CUSTOM_EVENT
        }

        public enum TALK_TYPE
        {
            KEY,
            DIRECT_TEXT
        }


        [Header("On Physics Compare Tag"), Space()]
        [SerializeField, Tag] protected string m_triggerTag = "Player";

        [Header("Keyboard Button Press"), Space()]
        [SerializeField] protected KeyCode m_triggerKey = KeyCode.Space;

        [Header("Button Press"), Space()]
        [SerializeField] protected string m_buttonName = "Fire";

        [Header("Custom Event name"), Space()]
        [SerializeField] protected string m_customEvent = "";

        [Header("Allowed Actions for Initializing (Need at least one)"), Space()]
        [SerializeField] protected List<INIT_ACTION> m_actions = new List<INIT_ACTION>();

        [Header("String key sent to Dialogue manager"), Space()]
        public VP_DialogInitEventKey m_initKey = null;
        public string m_key = "";

        [Header("Type of message sent to dialogue manager"), Space()]
        [SerializeField] private TALK_TYPE m_talkType = TALK_TYPE.KEY;
        protected bool m_useKeyDown = false;
        protected bool m_useKeyUp = false;

        protected bool m_useButtonDown = false;
        protected bool m_useButtonUp = false;

        public List<INIT_ACTION> Actions { get { return m_actions; } }

        #region Monobehaviour and Custom Event

        protected override void Awake()
        {
            base.Awake();
            m_useKeyDown = m_actions.Contains(INIT_ACTION.ON_KEY_DOWN);
            m_useKeyUp = m_actions.Contains(INIT_ACTION.ON_KEY_UP);

            m_useButtonDown = m_actions.Contains(INIT_ACTION.ON_BUTTON_DOWN);
            m_useButtonUp = m_actions.Contains(INIT_ACTION.ON_BUTTON_UP);
        }


        protected override void StartAllListeners()
        {
            if (m_actions.Contains(INIT_ACTION.ON_CUSTOM_EVENT))
            {
                base.StartAllListeners();
                VP_EventManager.StartListening(m_customEvent, Trigger);
            }

        }

        protected override void StopAllListeners()
        {
            if (m_actions.Contains(INIT_ACTION.ON_CUSTOM_EVENT))
            {
                base.StopAllListeners();
                VP_EventManager.StopListening(m_customEvent, Trigger);
            }
        }
        #endregion

        #region On Collision
        protected virtual void OnCollisionEnter(Collision collision)
        {
            if (m_actions.Contains(INIT_ACTION.ON_COLLISION_ENTER) && collision.transform.CompareTag(m_triggerTag))
            {
                Trigger();
            }
        }

        protected virtual void OnCollisionEnter2D(Collision2D collision)
        {
            if (m_actions.Contains(INIT_ACTION.ON_COLLISION_ENTER_2D) && collision.transform.CompareTag(m_triggerTag))
            {
                Trigger();
            }
        }

        protected virtual void OnCollisionExit(Collision collision)
        {
            if (m_actions.Contains(INIT_ACTION.ON_COLLISION_EXIT) && collision.transform.CompareTag(m_triggerTag))
            {
                Trigger();
            }
        }

        protected virtual void OnCollisionExit2D(Collision2D collision)
        {
            if (m_actions.Contains(INIT_ACTION.ON_COLLISION_EXIT_2D) && collision.transform.CompareTag(m_triggerTag))
            {
                Trigger();
            }
        }
        #endregion

        #region On Trigger
        protected virtual void OnTriggerEnter(Collider other)
        {
            if (m_actions.Contains(INIT_ACTION.ON_TRIGGER_ENTER) && other.transform.CompareTag(m_triggerTag))
            {
                Trigger();
            }
        }

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if (m_actions.Contains(INIT_ACTION.ON_TRIGGER_ENTER_2D) && collision.transform.CompareTag(m_triggerTag))
            {
                Trigger();
            }
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            if (m_actions.Contains(INIT_ACTION.ON_TRIGGER_EXIT) && other.transform.CompareTag(m_triggerTag))
            {
                Trigger();
            }
        }
        protected virtual void OnTriggerExit2D(Collider2D collision)
        {
            if (m_actions.Contains(INIT_ACTION.ON_TRIGGER_EXIT_2D) && collision.transform.CompareTag(m_triggerTag))
            {
                Trigger();
            }
        }
        #endregion

        #region On Particle Collision
        protected virtual void OnParticleCollision(GameObject other)
        {
            if (m_actions.Contains(INIT_ACTION.ON_PARTICLE_COLLISION) && other.CompareTag(m_triggerTag))
            {
                Trigger();
            }
        }
        #endregion

        #region Update + Key
        protected virtual void Update()
        {
            if (m_useKeyDown)
            {
                if (Input.GetKeyDown(m_triggerKey))
                {
                    Trigger();
                }
            }

            if (m_useKeyUp)
            {
                if (Input.GetKeyUp(m_triggerKey))
                {
                    Trigger();
                }
            }

            if (m_useButtonDown)
            {
                if (Input.GetButtonDown(m_buttonName))
                {
                    Trigger();
                }
            }

            if (m_useButtonUp)
            {
                if (Input.GetButtonUp(m_buttonName))
                {
                    Trigger();
                }
            }


        }
        #endregion

        #region On Action
        public virtual void OnAction()
        {
            if (m_actions.Contains(INIT_ACTION.ON_ACTION_CALL))
                Trigger();
        }
        #endregion

        #region On Mouse
        protected virtual void OnMouseDown()
        {
            if (m_actions.Contains(INIT_ACTION.ON_MOUSE_DOWN))
                Trigger();
        }

        protected virtual void OnMouseUp()
        {
            if (m_actions.Contains(INIT_ACTION.ON_MOUSE_UP))
                Trigger();
        }

        protected virtual void OnMouseOver()
        {
            if (m_actions.Contains(INIT_ACTION.ON_MOUSE_HOVER))
                Trigger();
        }

        protected virtual void OnMouseDrag()
        {
            if (m_actions.Contains(INIT_ACTION.ON_MOUSE_DRAG))
                Trigger();
        }

        protected virtual void OnMouseEnter()
        {
            if (m_actions.Contains(INIT_ACTION.ON_MOUSE_ENTER))
                Trigger();
        }

        protected virtual void OnMouseExit()
        {
            if (m_actions.Contains(INIT_ACTION.ON_MOUSE_EXIT))
                Trigger();
        }
        #endregion

        protected virtual void Trigger()
        {
            if (!string.IsNullOrEmpty(m_key))
            {
                if (m_talkType == TALK_TYPE.KEY)
                    VP_DialogManager.SendDialogMessage(m_key);
                else
                    VP_DialogManager.ShowDirectMessage(m_key);
            }
        }

        public void OnAction(Transform trs)
        {
           
        }

        public bool CanBeInteracted()
        {
            return true;
        }

        public void SetCanBeInteracted(bool _canBeInteracted)
        {

        }
    }
}
