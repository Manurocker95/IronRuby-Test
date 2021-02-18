using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix.Inputs;

#if USE_VRIF

using BNG;
using VirtualPhenix.Inputs.VR;

namespace VirtualPhenix.Integrations.Inputs.VR
{
    [DefaultExecutionOrder(VP_ExecutingOrderSetup.VR_INPUT_MANAGER), AddComponentMenu(VP_PrefixSetup.MAIN_PREFIX + "/Inputs/VP Input Manager for VRIF")]
    public class VP_VRIFInputManager : VP_VRInputManager<VP_VRIFPlayerAction, VP_VRIFInputKeyData>
    {
        [SerializeField] protected LocomotionManager m_locomotionManager;
        [SerializeField] protected SmoothLocomotion m_smoothLocomotion;
        [SerializeField] protected PlayerTeleport m_playerTP;

        public static new VP_VRIFInputManager Instance
        {
            get
            {
                return (VP_VRIFInputManager)m_instance;
            }
        }

        public LocomotionManager LocomotionManager
        {
            get
            {
                return m_locomotionManager;
            }
        }

        public SmoothLocomotion SmoothLocomotion
        {
            get
            {
                return m_smoothLocomotion;
            }
        }

        public PlayerTeleport PlayerTeleport
        {
            get
            {
                return m_playerTP;
            }
        }


        protected override void Reset()
        {
            base.Reset();
            m_smoothLocomotion = FindObjectOfType<SmoothLocomotion>();
            m_locomotionManager = FindObjectOfType<LocomotionManager>();
            m_playerTP = FindObjectOfType<PlayerTeleport>();
            m_defaultData = new VP_VRIFInputKeyData();
        }

        protected override void Initialize()
        {
            base.Initialize();

            if (!m_smoothLocomotion)
                m_smoothLocomotion = FindObjectOfType<SmoothLocomotion>();

            if (!m_locomotionManager)
                m_locomotionManager = FindObjectOfType<LocomotionManager>();            
            
            if (!m_playerTP)
                m_playerTP = FindObjectOfType<PlayerTeleport>();
        }


        public override VP_VRIFPlayerAction DefaultInputActionValue()
        {
            return new VP_VRIFPlayerAction();
        }

        public override VP_VRIFInputKeyData DefaultInputKeyValue()
        {
            return new VP_VRIFInputKeyData(m_defaultData);
        }

        protected override VP_VRIFPlayerAction CreatePlayerInputs()
        {
            var m_playerActions = DefaultInputActionValue();
            CreateInputActions(VP_InputSetup.PLAYER_ID, out m_playerActions, DefaultInputKeyValue());
            return m_playerActions;
        }

        protected override VP_VRIFInputKeyData CreateDataFromCopy(VP_InputKeyData _keyData)
        {
            return new VP_VRIFInputKeyData(_keyData);
        }

        public override void CreateInputActions(string _key, out VP_VRIFPlayerAction _actions, VP_VRIFInputKeyData _keyData)
        {
            _actions = DefaultInputActionValue();
            _actions.SetupInputs(_keyData, m_blockInputsIfControllerNotAvailable);

            if (!m_createdActions.ContainsKey(_key))
                m_createdActions.Add(_key, _actions);
            else
                m_createdActions[_key] = _actions;
        }


        protected override void CreateDefaultActionFromData(string k, VP_InputKeyData _keyData, out VP_VRIFPlayerAction ip)
        {
            CreateInputActions(k, out ip, _keyData is VP_VRIFInputKeyData ? (VP_VRIFInputKeyData)_keyData : CreateDataFromCopy(_keyData));
        }

        public override VP_VRIFPlayerAction GetCastedActionSet(string _key, int _idx = -1, bool _blockIfNull = false)
        {

            var ia = m_createdActions.ContainsKey(_key) && m_createdActions[_key] is VP_VRIFPlayerAction ? m_createdActions[_key] as VP_VRIFPlayerAction : PlayerActions as VP_VRIFPlayerAction;

            if (_idx >= 0)
            {
                ia.RemapToIndex(_idx, _blockIfNull);
            }

            return ia;
        }
    }
}
#endif