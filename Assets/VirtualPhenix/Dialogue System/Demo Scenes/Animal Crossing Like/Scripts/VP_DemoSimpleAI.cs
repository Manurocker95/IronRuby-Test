using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Dialog.Demo
{
    public class VP_DemoSimpleAI : VP_DemoNPC
    {
        [Header("Can Move")]
        [SerializeField] private bool m_canMove = true;
        [SerializeField] private bool m_moving = false;
       

        private float m_animatorMove = 5f;


        [Header("Waypoint Movement")]
        [SerializeField] private Rigidbody m_rigidBody;
        [SerializeField] private float m_speed = 3f;
        [SerializeField] private Transform[] m_waypoints = new Transform[] { };
        [SerializeField] private int m_currentIndex = 0;
        [SerializeField] private bool m_randomWaypoint = false;
        [SerializeField] private float m_waitTimeInWaypoint = 1f;
        [SerializeField] private float m_threshold = 0.5f;
        [SerializeField] private bool m_waitingForNext = false;


        [Header("Emotions")]
        [SerializeField] private VP_SerializedEmotions m_emotions = new VP_SerializedEmotions();

        // Start is called before the first frame update
        protected override void Start()
        {
            m_canMove = m_waypoints.Length > 0;
            base.Start();

            if (m_rigidBody == null)
                m_rigidBody = GetComponent<Rigidbody>();

            if (m_canMove)
            {
                if (m_randomWaypoint)
                {
                    m_currentIndex = UnityEngine.Random.Range(0, m_waypoints.Length);
                }
            }
        }

        public void CheckEmotion(KeyValuePair<VP_DialogMessage.EMOTION, string> _emotion)
        {
            if (m_emotions.ContainsKey(_emotion.Key))
            {
                string action = _emotion.Value;
                m_emotions[_emotion.Key].SetEmotionAction(action);
            }               
        }

        // Update is called once per frame
        void Update()
        {

            if (!m_speaking && m_canMove && m_moving && !m_waitingForNext && !m_interacting)
            {
                m_animatorMove = .4f;
                WaypointPath();
            }
            else
            {
                m_animatorMove = 0f;
            }

            m_animator.SetFloat("speed", m_animatorMove);
        }

        public override void OnEndInteraction()
        {
            VP_EventManager.StopListening<KeyValuePair<VP_DialogMessage.EMOTION, string>>(VP_EventSetup.Dialog.TRIGGER_EMOTION, CheckEmotion);
            base.OnEndInteraction();
        }

        public override void OnInteraction(VP_DemoCharacterController _characterController)
        {
            transform.LookAt(_characterController.transform.position);
      
            if (VP_DialogManager.Instance == null)
            {
                Debug.Log("There's no Dialog manager...");
                OnEndInteraction();
                if (m_characterController)
                    m_characterController.StartCanInteractDelay();
            }
            else
            {
                VP_EventManager.StartListening<KeyValuePair<VP_DialogMessage.EMOTION, string>>(VP_EventSetup.Dialog.TRIGGER_EMOTION, CheckEmotion);

                base.OnInteraction(_characterController);

                if (!m_discovered)
                {
                    m_discovered = true;
                }

            }

        }

        void WaypointPath()
        {
            float length = Vector3.Distance(m_waypoints[m_currentIndex].position, transform.position);
            if (length <= m_threshold)
            {
                m_moving = false;
                m_currentIndex = (m_randomWaypoint) ? UnityEngine.Random.Range(0, m_waypoints.Length) : m_currentIndex + 1;
                if (m_waypoints.Length <= m_currentIndex)
                {
                    m_currentIndex = 0;
                }

                m_rigidBody.velocity = Vector3.zero;
                m_animatorMove = 0f;
                m_animator.SetFloat("speed", m_animatorMove);
                transform.LookAt(m_waypoints[m_currentIndex]);
                StartCoroutine(WaitForNextPoint(m_waitTimeInWaypoint, () => { m_moving = true; }));
            }
            else
            {
                transform.LookAt(m_waypoints[m_currentIndex]);
                transform.position += transform.forward * m_speed * Time.deltaTime;
            }
        }

        IEnumerator WaitForNextPoint(float time = 2f, System.Action _callback = null)
        {
            m_waitingForNext = true;
            float timer = 0f;
            while (timer < time)
            {
                timer += Time.deltaTime;
                yield return null;
            }
            m_waitingForNext = false;
            if (_callback != null)
                _callback.Invoke();
        }
    }
}
