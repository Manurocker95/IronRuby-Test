using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace VirtualPhenix.Interaction
{
    public enum NPC_MOVEMENT_TYPE
    {
        IDLE,
        RANDOM_MOVEMENT,
        PATH_MOVEMENT,
        PATH_MOVEMENT_LOOP,
        PATH_MOVEMENT_LOOP_REVERSE
    }

    public enum ACTION_AFTER_ACTION_PATH
    {
        DISAPPEAR,
        STOP,
        ROTATE_TOWARDS_PLAYER,
        BACK_TO_MOVEMENT,
        SET_ACTION_INDEX
    }

    public class VP_MovableObject : VP_InteractableObject
    {
        [Header("-- Movement --"), Space(10)]
        [SerializeField] protected NPC_MOVEMENT_TYPE m_movementType = NPC_MOVEMENT_TYPE.IDLE;
        [SerializeField] protected Transform[] m_originalPath;

        [Header("-- Components --"), Space(10)]
        [SerializeField] protected CharacterController m_characterController;
        [SerializeField] protected NavMeshAgent m_navmeshAgent;

        [Header("-- Action movement --"), Space(10)]
        protected Transform[] m_path;
        protected ACTION_AFTER_ACTION_PATH m_whatToDoAfterMovement;
        protected bool m_actionMoving = false;
        [SerializeField] protected string m_movingTrigger = "moving";
        [SerializeField] protected string m_runningTrigger = "running";
        [SerializeField] protected float m_speedToRun = 5f;
        protected int m_waypointIndex = 0;
        [SerializeField] protected float m_waypointDistance = 0.6f;
        [SerializeField] protected float m_currentDistance = 0.1f;
        protected System.Action m_callingMethodAfterActionMoving;

        protected override void Start()
        {
            if (!m_characterController)
                m_characterController = GetComponent<CharacterController>();

            if (!m_navmeshAgent)
                m_navmeshAgent = GetComponent<NavMeshAgent>();
        }

        void BackToRegularMovement(System.Action _callback)
        {

            if (_callback != null)
                _callback.Invoke();
        }

	    protected override void Update()
        {
            if (m_actionMoving)
            {
                m_currentDistance = Vector3.Distance(m_path[m_waypointIndex].transform.position, this.transform.position);
                if (m_currentDistance <= m_waypointDistance)
                {
                    m_waypointIndex++;
                    if (m_waypointIndex >= m_path.Length)
                    {
                        StopMovement();

                        switch (m_whatToDoAfterMovement)
                        {
                            case ACTION_AFTER_ACTION_PATH.DISAPPEAR:

                                if (m_callingMethodAfterActionMoving != null)
                                    m_callingMethodAfterActionMoving.Invoke();

                                this.gameObject.SetActive(false);
                                break;
                            case ACTION_AFTER_ACTION_PATH.SET_ACTION_INDEX:
                                SetListIndex(m_savedData.m_currentActionList++);

                                if (m_callingMethodAfterActionMoving != null)
                                    m_callingMethodAfterActionMoving.Invoke();
                                break;
                            case ACTION_AFTER_ACTION_PATH.BACK_TO_MOVEMENT:
                                BackToRegularMovement(m_callingMethodAfterActionMoving);
                                break;
                            case ACTION_AFTER_ACTION_PATH.ROTATE_TOWARDS_PLAYER:
	                            RotateToPlayer(Vector3.zero, m_callingMethodAfterActionMoving);
                                break;
                        }

                        return;
                    }
                }

                if (m_navmeshAgent.speed >= m_speedToRun)
                {
                    m_animator.SetBool(m_runningTrigger, true);
                    m_animator.SetBool(m_movingTrigger, true);
                }
                else
                {
                    m_animator.SetBool(m_runningTrigger, false);
                    m_animator.SetBool(m_movingTrigger, true);
                }

                m_navmeshAgent.SetDestination(m_path[m_waypointIndex].position);
            }
        }

        public override void OnInteract(Vector3 _playerPos)
        {
            base.OnInteract(_playerPos);
        }

        protected override void OnDialogStart()
        {
            if (m_animator)
                m_animator.SetBool(m_talkingAnimatorBool, true);
        }

        protected override void OnDialogComplete()
        {
            if (m_animator)
                m_animator.SetBool(m_talkingAnimatorBool, false);
        }

        public virtual void SetFollowActionPath(Transform[] _path, float _speed = 3.5f, ACTION_AFTER_ACTION_PATH _afterAction = ACTION_AFTER_ACTION_PATH.DISAPPEAR, System.Action _callback = null)
        {
            if (m_navmeshAgent == null || !m_navmeshAgent.isOnNavMesh || _path.Length == 0)
                Debug.LogError("CANT SET PATH FOR: " + this.name);

            StopMovement();
            m_actionMoving = true;
            m_path = _path;
            m_whatToDoAfterMovement = _afterAction;
            m_navmeshAgent.speed = _speed;
            m_navmeshAgent.isStopped = false;
            m_waypointIndex = 0;
            m_navmeshAgent.SetDestination(_path[0].position);
            m_callingMethodAfterActionMoving = _callback;
            m_animator.SetBool(m_runningTrigger, false);
            m_animator.SetBool(m_movingTrigger, true);
        }

        public virtual void StopMovement()
        {
            m_actionMoving = false;
            m_navmeshAgent.isStopped = true;
            m_navmeshAgent.enabled = false;
            m_navmeshAgent.enabled = true;
            m_animator.SetBool(m_runningTrigger, false);
            m_animator.SetBool(m_movingTrigger, false);
        }
    }
}
