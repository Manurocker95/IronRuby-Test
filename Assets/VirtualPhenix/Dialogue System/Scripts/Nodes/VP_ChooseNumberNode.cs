using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace VirtualPhenix.Dialog
{

    [NodeTint(VP_DialogSetup.NodeColors.CHOOSE_NUMBER), CreateNodeMenuAttribute("Choose Number")]
    public class VP_ChooseNumberNode : VP_DialogBaseNode
    {
        [SerializeField] private int m_min = 0;
        [SerializeField] private int m_max = 999;
        [SerializeField] private int m_number = 0;
        [SerializeField] private bool m_translate = false;
        [SerializeField] private bool m_canCancel = true;
        [TextArea, SerializeField] private string m_message = "Choose a number";
        [SerializeField] private VariableComparison m_comparison = VariableComparison.Equal;
        [SerializeField] private VP_DialogPositionData m_positionData = null;
        [SerializeField] private VP_DialogMessage m_customMessage = null;
        [SerializeField] private AudioClip m_textAudio = null;
        [SerializeField] private DIALOG_TYPE m_dialogType = DIALOG_TYPE.REGULAR;

        public bool CanCancel { get { return m_canCancel; } }
        public VariableComparison Comparison { get { return m_comparison; } }

        /// <summary>
        /// Is true? node
        /// </summary>
        [Output] public VP_DialogBaseNode isTrue;
        /// <summary>
        /// Is false? node
        /// </summary>
        [Output] public VP_DialogBaseNode isFalse;
        [Output] public VP_DialogBaseNode ifCancel;

        // Start is called before the first frame update
        void Start()
        {

        }

        public bool Check(int _val)
        {
            switch (m_comparison)
            {
                case VariableComparison.Equal:
                    return (m_number == _val);
                case VariableComparison.Mayor:
                    return (_val > m_number);
                case VariableComparison.Minor:
                    return (_val < m_number);
                case VariableComparison.MinorEqual:
                    return (_val <= m_number);
                case VariableComparison.MayorEqual:
                    return (_val >= m_number);
                case VariableComparison.None:
                    return true;
                default:
                    return false;
            }
        }

        public override void Trigger()
        {
            VP_DialogManager.ChooseNumberWithParamsInGraph(m_message, new Vector3(m_min, m_max, m_number), WaitForInput,  m_canCancel, CancelledInput, m_textAudio, m_dialogType, m_translate, m_customMessage, m_positionData);
        }

        void CancelledInput()
        {
            if (m_needToSkip)
            {
                m_needToSkip = false;
            }
            object var = null;
            VP_DialogManager.StopListeningToOnNumberCancel(CancelledInput);
            VP_DialogManager.StopListeningToOnChooseNumber(WaitForInput);
            //Trigger next nodes
            NodePort port;
            if (m_comparison != VariableComparison.None)
            {                
                port = GetOutputPort(VP_DialogSetup.Fields.DIALOG_NODE_CONDITION_FALSE);
            }
            else
            {
                port = GetOutputPort(VP_DialogSetup.Fields.CHOOSE_NUMBER_NODE_CANCELOUTPUT);
            }

            VP_DialogManager.OnDialogCompleteAction();

            if (port.ConnectionCount > 0)
            {
                for (int i = 0; i < port.ConnectionCount; i++)
                {
                    NodePort connection = port.GetConnection(i);
                    (connection.node as VP_DialogBaseNode).Trigger(var);
                }
            }
            else
            {
                CheckEndSequenceNode();
                return;
            }
        }

        void WaitForInput(int _value)
        {
            if (m_needToSkip)
            {
                m_needToSkip = false;
            }

            VP_DialogManager.StopListeningToOnChooseNumber(WaitForInput);
            //Trigger next nodes
            NodePort port;
            if (m_comparison != VariableComparison.None)
            {
                bool _check = Check(_value);


                if (_check)
                    port = GetOutputPort(VP_DialogSetup.Fields.DIALOG_NODE_CONDITION_TRUE);
                else
                    port = GetOutputPort(VP_DialogSetup.Fields.DIALOG_NODE_CONDITION_FALSE);
            }
            else
            {
                port = GetOutputPort(VP_DialogSetup.Fields.DIALOG_NODE_OUTPUT);
            }

            VP_DialogManager.OnDialogCompleteAction();

            if (port.ConnectionCount > 0)
            {
                for (int i = 0; i < port.ConnectionCount; i++)
                {
                    NodePort connection = port.GetConnection(i);
                    (connection.node as VP_DialogBaseNode).Trigger(_value);
                }
            }
            else
            {
                CheckEndSequenceNode();
                return;
            }
        }

        public override void OnCreateConnection(NodePort from, NodePort to)
        {
            base.OnCreateConnection(from, to);
            if (m_comparison != VariableComparison.None)
            {
                if (from == GetOutputPort(VP_DialogSetup.Fields.DIALOG_NODE_OUTPUT))
                {
                    Debug.LogError("This node must be set from Is True or Is False");
                    from.Disconnect(0);
                }

                if (from == GetOutputPort(VP_DialogSetup.Fields.DIALOG_NODE_CONDITION_TRUE))
                {
                    isTrue = to.node as VP_DialogBaseNode;
                }

                if (from == GetOutputPort(VP_DialogSetup.Fields.DIALOG_NODE_CONDITION_FALSE))
                {
                    isFalse = to.node as VP_DialogBaseNode;
                }
            }
            else
            {
                if (from == GetOutputPort(VP_DialogSetup.Fields.DIALOG_NODE_OUTPUT))
                {
                    output = to.node as VP_DialogBaseNode;
                }
            }
        }

        public override void OnRemoveConnection(NodePort port)
        {
            base.OnRemoveConnection(port);

            if (port == GetOutputPort(VP_DialogSetup.Fields.DIALOG_NODE_CONDITION_TRUE))
            {
                isTrue = null;
            }

            if (port == GetOutputPort(VP_DialogSetup.Fields.DIALOG_NODE_CONDITION_FALSE))
            {
                isFalse = null;
            }

            if (port == GetOutputPort(VP_DialogSetup.Fields.DIALOG_NODE_OUTPUT))
            {
                output = null;
            }
        }
    }

}
