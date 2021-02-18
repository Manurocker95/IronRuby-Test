
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Actions
{
    public class VP_CustomFadeEffectAction : VP_CustomFadeAction
    {
        public override void InitActions(System.Action _initCallback = null, System.Action _invokeCallback = null, System.Action<int> _indexCallback = null)
        {
            base.InitActions(_initCallback, _invokeCallback, _indexCallback);
            m_action = CUSTOM_GAME_ACTIONS.FADE_EFFECT;
        }

        public override void InvokeAction()
        {
            base.InvokeAction();

#if USE_FADE
            if (m_fadeEffect != null && m_fadePP != null)
            {
                m_fadePP.AssignEffect(m_fadeEffect);
                if (m_fadeType == FADE_EFFECT_TYPE.DOWN)
                    m_fadePP.FadeDown(m_fadeDirectly, VP_ActionManager.Instance.DoGameplayAction);
                else
                    m_fadePP.FadeUp(m_fadeDirectly, VP_ActionManager.Instance.DoGameplayAction);
            }
            else
            {
                VP_ActionManager.Instance.DoGameplayAction();
            }
#else
            // TODO
            VP_ActionManager.Instance.DoGameplayAction();
#endif

        }

    }

}
