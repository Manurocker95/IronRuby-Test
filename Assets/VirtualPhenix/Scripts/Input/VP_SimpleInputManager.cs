using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix.Inputs;

namespace VirtualPhenix
{
	[DefaultExecutionOrder(VP_ExecutingOrderSetup.INPUT_MANAGER),AddComponentMenu(VP_PrefixSetup.MAIN_PREFIX+"/Inputs/VP Input Manager")]
    public class VP_SimpleInputManager : VP_InputManager<VP_PlayerActions, VP_InputKeyData>
    {
        public override VP_PlayerActions DefaultInputActionValue()
        {
            return new VP_PlayerActions();
        }

        public override VP_PlayerActions GetCastedActionSet(string _key, int _idx = -1, bool _blockIfNull = false)
        {

            var ia = m_createdActions.ContainsKey(_key) && m_createdActions[_key] is VP_PlayerActions ? m_createdActions[_key] as VP_PlayerActions : PlayerActions as VP_PlayerActions;

            if (_idx >= 0)
            {
                ia.RemapToIndex(_idx, _blockIfNull);
            }

            return ia;
        }

        public override void CreateInputActions(string _key, out VP_PlayerActions _actions, VP_InputKeyData keys)
        {
            _actions = DefaultInputActionValue();
            _actions.SetupInputs(keys);


            if (!m_createdActions.ContainsKey(_key))
                m_createdActions.Add(_key, _actions);
            else
                m_createdActions[_key] = _actions;
        }
    }
}