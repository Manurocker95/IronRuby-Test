using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix.Controllers.Components;

namespace VirtualPhenix
{
    [DefaultExecutionOrder(VP_ExecutingOrderSetup.LOOK_ANIMATOR), AddComponentMenu(VP_PrefixSetup.MAIN_PREFIX + "/IK/Aim IK")]
    public class VP_LookAnimator : VP_CharacterComponent, VP_IFaceable
    {
        [Header("Look Animator"), Space]
        [SerializeField] protected Vector3 m_facePoint;
        [SerializeField] protected Transform m_faceTrs;
        [SerializeField] protected Transform m_lookAtTrs;
        public bool m_canLookAt = true;
        public bool m_canbeLookedAt = true;

        public virtual Transform Target { get { return m_lookAtTrs; } }
        public virtual Transform FaceTransform { get { return m_faceTrs; } }
        public virtual Vector3 FacePoint { get { return m_facePoint; } }

        protected override void Reset()
        {
            base.Reset();
            m_canLookAt = true;
            SetFace(this.transform);
        }

        public virtual void SetTarget(Transform _target, bool _saveIt = true)
        {
            if (_saveIt)
                m_lookAtTrs = _target;
        }

        public virtual void SetPreviousTarget()
        {

        }   
        
        public virtual void DisableAimIK()
        {

        }

        public virtual void EnableAimIK()
        {

        }

        public bool CanBeFaced()
        {
            return m_canbeLookedAt;
        }

        public void SetCanBeFaced(bool _canBeFaced)
        {
            m_canbeLookedAt = _canBeFaced;
        }

        public Vector3 GetFace()
        {
            return m_faceTrs.position;
        }

        public void SetFace(Transform _newFace)
        {
            m_faceTrs = _newFace;

            if (m_faceTrs != null)
                SetFacePosition(m_faceTrs.position);
        }

        public void SetFacePosition(Vector3 _newFacePoint)
        {
            m_facePoint = _newFacePoint;
        }
    }
}
