using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if USE_FLOOK_ANIMATOR
using FIMSpace.FLook;
#endif

namespace VirtualPhenix.Integrations.LookAnimator
{
#if USE_FLOOK_ANIMATOR
    [DefaultExecutionOrder(VP_ExecutingOrderSetup.LOOK_ANIMATOR), AddComponentMenu(VP_PrefixSetup.MAIN_PREFIX + "/IK/Aim IK - Look Animator")]
    public class VP_FLookAnimator : VP_LookAnimator
    {
        [SerializeField] protected FLookAnimator m_flookAnimator;

        public FLookAnimator LookAnimator {  get { return m_flookAnimator; } }
        public bool IsActive {  get { return m_flookAnimator != null; } }

		protected override void Reset()
		{
            base.Reset();
			m_flookAnimator = GetComponentInChildren<FLookAnimator>();
		}

        public override void SetPreviousTarget()
        {
            if (IsActive)
                m_flookAnimator.SetLookTarget(m_lookAtTrs);
        }

        public override void SetTarget(Transform _target, bool _saveIt = true)
        {
            base.SetTarget(_target, _saveIt);
            if (IsActive)
                m_flookAnimator.SetLookTarget(_target);
        }

        public override void DisableAimIK()
        {
            if (IsActive)
                m_flookAnimator.enabled = false;
        }

        public override void EnableAimIK()
        {
            if (IsActive)
                m_flookAnimator.enabled = true;
        }
    }
#endif
}