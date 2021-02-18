using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix.Fade;

namespace VirtualPhenix.Actions
{
    public class VP_CustomFadeAction : VP_CustomActions
    {
        [Header("--Fade Effect-"), Space(10)]
#if USE_FADE
        [SerializeField] protected VP_FadeEffect m_fadeEffect;
         [SerializeField]protected  VP_FadePostProcess m_fadePP;
#endif
        [SerializeField] protected FADE_EFFECT_TYPE m_fadeType = FADE_EFFECT_TYPE.DOWN;
        [SerializeField] protected bool m_fadeDirectly = false;
        [SerializeField] protected bool m_modifyFadeDuration = false;
        [SerializeField] protected float m_newFadeDuration = 1;

        public override void InitActions(Action _initCallback = null, Action _invokeCallback = null, Action<int> _indexCallback = null)
        {
            base.InitActions(_initCallback, _invokeCallback, _indexCallback);
        }

        public override void InvokeAction()
        {
            base.InvokeAction();
        }
    }

}
