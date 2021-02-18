using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix.Interaction;

namespace VirtualPhenix.Actions
{
    [Serializable]
    public class VP_MovementActionPath
    {
        [SerializeField] protected float m_movementSpeed;
        [SerializeField] protected VP_MovableObject m_movableObject;
        [SerializeField] protected Transform[] m_waypoints;
        [SerializeField] protected ACTION_AFTER_ACTION_PATH m_afterMovementAction;

        public float MovementSpeed { get { return m_movementSpeed; } }
        public VP_MovableObject MovementObject { get { return m_movableObject; } }
        public Transform[] Path { get { return m_waypoints; } }
        public ACTION_AFTER_ACTION_PATH AfterMovement { get { return m_afterMovementAction; } }

        public void SetFollowActionPath(Action _callback)
        {
            m_movableObject.SetFollowActionPath(m_waypoints, m_movementSpeed, m_afterMovementAction, _callback);
        }

        public void StopMovement()
        {
            m_movableObject.StopMovement();
        }

        public void RotateTo(Vector3 pos, Action _callback)
        {
            m_movableObject.RotateToPlayer(pos, _callback);
        }
    }

}