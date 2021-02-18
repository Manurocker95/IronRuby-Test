using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
namespace VirtualPhenix
{
    [DefaultExecutionOrder(VP_ExecutingOrderSetup.GAME_EVENT),AddComponentMenu(VP_PrefixSetup.MAIN_PREFIX+"/Events/Game Event Trigger")]
    public class VP_GameEvent : VP_Monobehaviour, VP_IActionable
    {
        [Header("Type"), Space(10)]
        /// <summary>
        /// If it uses Unity Events or Unity Send Message
        /// </summary>
        [SerializeField] protected GAME_EVENT_TYPE m_type = GAME_EVENT_TYPE.SEND_MESSAGE;

        [Header("How to trigger"), Space(10)]
        /// <summary>
        /// If the event must be triggered when an event is listened, On Action or On Trigger Enter
        /// </summary>
        [SerializeField] protected GAME_EVENT_TRIGGER m_trigger = GAME_EVENT_TRIGGER.ON_ACTION;
        /// <summary>
        /// If wanna use send message, which objects do you wanna call
        /// </summary>
        [SerializeField, Tag] protected string[] m_triggerTags = new string[] { "Player" };
        [SerializeField] protected GameObject[] m_triggerGOs;
        /// <summary>
        /// Deactivate the game event after triggering it
        /// </summary>
        [SerializeField] protected bool m_deactivateOnTrigger = true;
        [SerializeField] protected bool m_canActivate = true;

        [Header("Custom Events"), Space(10)]
        /// <summary>
        /// If wanna use send message, which methods do you wanna call. The rest uses Unity Events and only the first one in the array
        /// </summary>
        [SerializeField] protected string[] m_events;
        /// <summary>
        /// Events that this game event can be listening to
        /// </summary>
        [SerializeField] protected string[] m_listeningEvents;

        [Header("Unity Events"), Space(10)]
        [SerializeField] protected UnityEvent[] m_unityEventToCall;

        [Header("Send Messages"), Space(10)]
        [SerializeField] protected string[] m_messages;
        /// <summary>
        /// If wanna use send message, which objects do you wanna call
        /// </summary>
        [SerializeField] protected GameObject[] m_relatedGOs;

        public bool m_hasBeenUsed = false;

        public bool CanActivate { get { return m_canActivate; } set { m_canActivate = value; } }
        public bool IsFaceTrigger { get { return m_trigger == GAME_EVENT_TRIGGER.ON_PLAYER_FACE; } }

        protected override void Awake()
        {
            base.Awake();

            if (m_trigger == GAME_EVENT_TRIGGER.ON_AWAKE)
            {
                DoEvent();
            }

        }

        // Use this for initialization
        protected override void Start()
        {
            base.Start();

            if (m_trigger == GAME_EVENT_TRIGGER.ON_START)
            {
                DoEvent();
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (m_trigger == GAME_EVENT_TRIGGER.ON_ENABLE)
            {
                DoEvent();
            }

            if (m_trigger == GAME_EVENT_TRIGGER.ON_EVENT)
            {
                foreach (string ev in m_listeningEvents)
                {
                    VP_EventManager.StartListening(ev, DoEvent);
                }
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (m_trigger == GAME_EVENT_TRIGGER.ON_EVENT)
            {
                foreach (string ev in m_listeningEvents)
                {
                    VP_EventManager.StopListening(ev, DoEvent);
                }
            }

            if (m_trigger == GAME_EVENT_TRIGGER.ON_DISABLE)
            {
                DoEvent();
            }
        }

        protected virtual void OnDrawGizmos()
        {
            // Draws the Light bulb icon at position of the object.
            // Because we draw it inside OnDrawGizmos the icon is also pickable
            // in the scene view.

            Gizmos.DrawIcon(transform.position, "Game Event Gizmo.png", true);
        }

        public virtual bool CanBeInteracted()
        {
            return m_canActivate;
        }

        public virtual void SetCanBeInteracted(bool _value)
        {
            m_canActivate = _value;
        }

        public virtual void OnAction()
        {
            if (m_trigger == GAME_EVENT_TRIGGER.ON_ACTION)
            {
                DoEvent();
            }
        }

        public virtual void DoEvent()
        {
            if (!m_canActivate)
            {
                Debug.LogError("Game Event in gameobject " + this.gameObject.name + " can't be activated");
                return;
            }

            m_hasBeenUsed = true;

            switch (m_type)
            {
                case GAME_EVENT_TYPE.CUSTOM_EVENT:
                    foreach (string _event in m_events)
                    {
                        VP_EventManager.TriggerEvent(_event);
                    }

                    break;
                case GAME_EVENT_TYPE.UNITY_EVENT:
                    foreach (UnityEvent ev in m_unityEventToCall)
                    {
                        ev.Invoke();
                    }
                    break;
                case GAME_EVENT_TYPE.SEND_MESSAGE:
                    int index = 0;
                    foreach (GameObject go in m_relatedGOs)
                    {
                        go.SendMessage(m_messages[index]);
                        index++;
                    }
                    break;
            }

            if (m_deactivateOnTrigger)
            {
                m_canActivate = false;
            }
        }

        protected virtual void OnTriggerEnter(Collider collision)
        {
            if (!m_canActivate)
                return;

            if (m_trigger == GAME_EVENT_TRIGGER.ON_ENTER)
            {
                if (m_triggerGOs != null && m_triggerGOs.Length > 0)
                {
                    foreach (GameObject go in m_triggerGOs)
                    {
                        if (collision.gameObject == go)
                        {
                            DoEvent();
                            return;
                        }
                    }
                }

                if (m_triggerTags != null && m_triggerTags.Length > 0)
                {
                    foreach (string go in m_triggerTags)
                    {
                        if (collision.gameObject.CompareTag(go))
                        {
                            DoEvent();
                            break;
                        }
                    }
                }
            }
        }


        protected virtual void OnTriggerExit(Collider collision)
        {
            if (!m_canActivate)
                return;

            if (m_trigger == GAME_EVENT_TRIGGER.ON_EXIT)
            {
                foreach (GameObject go in m_triggerGOs)
                {
                    if (collision.gameObject == go)
                    {
                        DoEvent();
                        return;
                    }
                }

                foreach (string go in m_triggerTags)
                {
                    if (collision.gameObject.CompareTag(go))
                    {
                        DoEvent();
                        break;
                    }
                }
            }
        }

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if (!m_canActivate)
                return;

            if (m_trigger == GAME_EVENT_TRIGGER.ON_ENTER)
            {
                foreach (string go in m_triggerTags)
                {
                    if (collision.gameObject.CompareTag(go))
                    {
                        DoEvent();
                        break;
                    }
                }
            }
        }

        public void OnAction(Transform trs)
        {

        }
    }

    public enum GAME_EVENT_TYPE
    {
        CUSTOM_EVENT,
        UNITY_EVENT,
        SEND_MESSAGE,
        SERIALIZED_CALLBACK
    }

    public enum GAME_EVENT_TRIGGER
    {
        ON_ENTER,
        ON_EXIT,
        ON_ACTION,
        ON_EVENT,
        ON_AWAKE,
        ON_START,
        ON_ENABLE,
        ON_DISABLE,
        ON_PLAYER_FACE
    }
}
