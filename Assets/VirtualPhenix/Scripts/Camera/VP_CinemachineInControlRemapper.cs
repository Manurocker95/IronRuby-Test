using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if USE_INCONTROL
using InControl;
#endif

namespace VirtualPhenix
{
#if USE_INCONTROL
    [System.Serializable]
    public class VP_CinemachineInControlAction : PlayerActionSet
    {
        public readonly PlayerTwoAxisAction RemappedControl;
        
        public readonly PlayerAction Right;
        public readonly PlayerAction Up;
        public readonly PlayerAction Down;
        public readonly PlayerAction Left;

        public VP_CinemachineInControlAction()
        {
            Up = CreatePlayerAction("RemappedCinemachineYPos");
            Down = CreatePlayerAction("RemappedCinemachineYNeg");
            Right = CreatePlayerAction("RemappedCinemachineXPos");
            Left = CreatePlayerAction("RemappedCinemachineXNeg");

            // Dpad in Axis
            RemappedControl = CreateTwoAxisPlayerAction(Left, Right, Down, Up);
        }

        public static VP_CinemachineInControlAction CreateWithDefaultBindings(InputControlType[] _upKeys, InputControlType[] _leftKeys, InputControlType[]_rightKeys, InputControlType[] _downKeys,
            Key[] _upKeyss, Key[] _leftKeyss, Key[] _rightKeyss, Key[] _downKeyss)
        {
            var playerActions = new VP_CinemachineInControlAction();

            foreach (InputControlType keys in _upKeys)
                playerActions.Up.AddDefaultBinding(keys);

            foreach (InputControlType keys in _leftKeys)
                playerActions.Left.AddDefaultBinding(keys);

            foreach (InputControlType keys in _rightKeys)
                playerActions.Right.AddDefaultBinding(keys);

            foreach (InputControlType keys in _downKeys)
                playerActions.Down.AddDefaultBinding(keys);

            foreach (Key keys in _upKeyss)
                playerActions.Up.AddDefaultBinding(keys);

            foreach (Key keys in _leftKeyss)
                playerActions.Left.AddDefaultBinding(keys);

            foreach (Key keys in _rightKeyss)
                playerActions.Right.AddDefaultBinding(keys);

            foreach (Key keys in _downKeyss)
                playerActions.Down.AddDefaultBinding(keys);


            playerActions.ListenOptions.IncludeUnknownControllers = true;
            playerActions.ListenOptions.MaxAllowedBindings = 4;

            playerActions.ListenOptions.UnsetDuplicateBindingsOnSet = true;

            return playerActions;
        }
    }
#endif

    public class VP_CinemachineInControlRemapper : VP_CinemachineControlRemapper
    {
#if USE_INCONTROL
        private VP_CinemachineInControlAction m_remappedControls;

        [Header("Game Pad"),Space]
        [SerializeField] private InputControlType[] m_upKeys = new InputControlType[] { InputControlType.RightStickUp };
        [SerializeField] private InputControlType[] m_leftKeys = new InputControlType[] { InputControlType.RightStickLeft };
        [SerializeField] private InputControlType[] m_rightKeys = new InputControlType[] { InputControlType.RightStickRight };
        [SerializeField] private InputControlType[] m_downKeys = new InputControlType[] { InputControlType.RightStickDown };

        [Header("KeyBoard"), Space]
        [SerializeField] private Key[] m_upKeyss = new Key[0];
        [SerializeField] private Key[] m_leftKeyss = new Key[0];
        [SerializeField] private Key[] m_rightKeyss = new Key[0];
        [SerializeField] private Key[] m_downKeyss = new Key[0];

        protected override void Initialize()
        {
            base.Initialize();
            m_remappedControls = VP_CinemachineInControlAction.CreateWithDefaultBindings
            (
                m_upKeys, m_leftKeys, m_rightKeys, m_downKeys, 
                m_upKeyss, m_leftKeyss, m_rightKeyss, m_downKeyss
            );
        }

        public override float GetAxisCustom(string axisData)
        {
            if (axisData.Equals(m_originalXAxis))
            {
                return m_remappedControls.RemappedControl.X;
            }
            else if (axisData.Equals(m_originalYAxis))
            {
                return m_remappedControls.RemappedControl.Y;
            }
            return 0f;
        }
#endif
    }
}
