using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace VirtualPhenix.Dialog
{
    [NodeTint(VP_DialogSetup.NodeColors.SET_POSITION_NODE), CreateNodeMenuAttribute("Position")]
    public class VP_DialogSetPositionDataNode : VP_DialogBaseNode
    {
        [SerializeField] public VP_DialogPositionData m_positionData;
        [SerializeField] public bool m_useDefaultPosition;
        [SerializeField] public Vector2 m_position;
        [SerializeField] public Vector2 m_offset;

        [SerializeField] public bool m_setTextPosition;
        [SerializeField] public Vector2 m_textTopBottomMugshot;
        [SerializeField] public Vector2 m_textLeftRightMugshot;

        [SerializeField] public Vector2 m_textTopBottomRegular;
        [SerializeField] public Vector2 m_textLeftRightRegular;

        public bool m_setBGPosition;
        public Vector2 m_bgTopBottom;
        public Vector2 m_bgLeftRight;

        public bool m_useGameObjectPosition;
        public string m_gameObjectName;

        protected override void Awake()
        {
            base.Awake();

            if (m_positionData == null)
            {
                m_positionData = Resources.Load<VP_DialogPositionData>("Dialogue/positionData");
            }
        }

        public override void Trigger()
        {
            if (m_needToSkip)
            {
                m_needToSkip = false;
            }

            m_positionData.m_useDefaultPosition = m_useDefaultPosition;

            if (!m_useDefaultPosition)
            {
                m_positionData.m_position = m_position;
            }
           
            if (m_setBGPosition)
            {
                m_positionData.m_bgTopBottom = m_bgTopBottom;
                m_positionData.m_bgLeftRight = m_bgLeftRight;
            }

            m_positionData.m_useGameObjectPosition = m_useGameObjectPosition;
            if (m_useGameObjectPosition)
            {
                m_positionData.m_useGameObjectName = m_gameObjectName;
            }

            m_positionData.m_setTextPosition = m_setTextPosition;

            if (m_setTextPosition)
            {
                m_positionData.m_textTopBottomMugshot = m_textTopBottomMugshot;
                m_positionData.m_textLeftRightMugshot = m_textLeftRightMugshot;
                m_positionData.m_textTopBottomRegular = m_textTopBottomRegular;
                m_positionData.m_textLeftRightRegular = m_textLeftRightRegular;
            }

            m_positionData.Trigger();
            VP_DialogManager.OnDialogCompleteAction();
            NodePort port = null;

            if (port == null)
            {
                port = GetOutputPort(VP_DialogSetup.Fields.DIALOG_NODE_OUTPUT);
            }

            if (port.ConnectionCount > 0)
            {
                for (int i = 0; i < port.ConnectionCount; i++)
                {
                    NodePort connection = port.GetConnection(i);
                    (connection.node as VP_DialogBaseNode).Trigger();
                }
            }
            else
            {
                CheckEndSequenceNode();
            }
        }
    }
}