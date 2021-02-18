using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Inputs.VR
{
    public class VP_VRInputManager<T,T0> : VP_InputManager<T, T0> where T : VP_VRPlayerActions where T0 : VP_VRInputKeyData
    {
        [Header("Block if controller not available")]
        [SerializeField] protected bool m_blockInputsIfControllerNotAvailable = true;

        protected override T0 CreateDataFromCopy(VP_InputKeyData _keyData)
        {
            return DefaultInputKeyValue();
        }

        public override T DefaultInputActionValue()
        {
            return default(T);
        }

        public override void CreateInputActions(string _key, out T _actions, T0 _keyData)
        {
            _actions = DefaultInputActionValue();
            _actions.SetupInputs(_keyData, m_blockInputsIfControllerNotAvailable);

            if (!m_createdActions.ContainsKey(_key))
                m_createdActions.Add(_key, _actions);
            else
                m_createdActions[_key] = _actions;
        }

        public override T GetCastedActionSet(string _key, int _idx = -1, bool _blockIfNull = false)
        {

            var ia = m_createdActions.ContainsKey(_key) && m_createdActions[_key] is T ? m_createdActions[_key] as T : PlayerActions as T;

            if (_idx >= 0)
            {
                ia.RemapToIndex(_idx, _blockIfNull);
            }

            return ia;
        }
    }
}