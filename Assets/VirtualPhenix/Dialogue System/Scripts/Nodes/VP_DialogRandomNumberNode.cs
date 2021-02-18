using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace VirtualPhenix.Dialog
{
    [NodeTint(VP_DialogSetup.NodeColors.RANDOM_NUMBER_NODE), CreateNodeMenuAttribute("Random Number")]
    public class VP_DialogRandomNumberNode : VP_DialogBaseNode
    {
        public enum NumberType
        {
            Int,
            Float
        }

        [SerializeField] private NumberType m_numberType = NumberType.Int;
        [SerializeField] private bool m_minByVariable = false;
        [SerializeField] private bool m_maxByVariable = false;
        [SerializeField] private string m_minVariable = "";
        [SerializeField] private string m_maxVariable = "";
        [SerializeField] private float m_min = 0;
        [SerializeField] private float m_max = 100;

        public bool MinByVariable { get { return m_minByVariable; } }
        public bool MaxByVariable { get { return m_maxByVariable; } }
        public string MinVariable { get { return m_minVariable; } }
        public string MaxVariable { get { return m_maxVariable; } }


        public override void Trigger<T>(T parameter)
        {
            if (parameter != null)
            {
     
                NodePort port = GetOutputPort(VP_DialogSetup.Fields.DIALOG_NODE_OUTPUT);

                if (port.ConnectionCount > 0)
                {
                    switch (m_numberType)
                    {
                        case NumberType.Int:
                            int nint = UnityEngine.Random.Range((int)m_min, (int)m_max);
                            VP_DialogManager.Instance.SetVariable("randomnumber", nint.ToString());
                            for (int i = 0; i < port.ConnectionCount; i++)
                            {
                                NodePort connection = port.GetConnection(i);
                                (connection.node as VP_DialogBaseNode).Trigger(nint);
                            }
                            break;
                        case NumberType.Float:
                            float number = UnityEngine.Random.Range(m_min, m_max);
                            VP_DialogManager.Instance.SetVariable("randomnumber", number.ToString());
                            for (int i = 0; i < port.ConnectionCount; i++)
                            {
                                NodePort connection = port.GetConnection(i);
                                (connection.node as VP_DialogBaseNode).Trigger(number);
                            }
                            break;
                        default:
                            number = UnityEngine.Random.Range(m_min, m_max);
                            VP_DialogManager.Instance.SetVariable("randomnumber", number.ToString());
                            for (int i = 0; i < port.ConnectionCount; i++)
                            {
                                NodePort connection = port.GetConnection(i);
                                (connection.node as VP_DialogBaseNode).Trigger(number);
                            }
                            break;
                    }

                }
                else
                {
                    CheckEndSequenceNode();
                    return;
                }
            }
            else
            {

                Trigger();
            }
        }

        public override void Trigger()
        {
            NodePort port = GetOutputPort(VP_DialogSetup.Fields.DIALOG_NODE_OUTPUT);

            if (port.ConnectionCount > 0)
            {
                switch (m_numberType)
                {
                    case NumberType.Int:
                        int nint = UnityEngine.Random.Range((int)m_min, (int)m_max);
                        VP_DialogManager.Instance.SetVariable("randomnumber", nint.ToString());
                        for (int i = 0; i < port.ConnectionCount; i++)
                        {
                            NodePort connection = port.GetConnection(i);
                            (connection.node as VP_DialogBaseNode).Trigger(nint);
                        }
                        break;
                    case NumberType.Float:
                        float number = UnityEngine.Random.Range(m_min, m_max);
                        VP_DialogManager.Instance.SetVariable("randomnumber", number.ToString());
                        for (int i = 0; i < port.ConnectionCount; i++)
                        {
                            NodePort connection = port.GetConnection(i);
                            (connection.node as VP_DialogBaseNode).Trigger(number);
                        }
                        break;
                    default:
                        number = UnityEngine.Random.Range(m_min, m_max);
                        VP_DialogManager.Instance.SetVariable("randomnumber", number.ToString());
                        for (int i = 0; i < port.ConnectionCount; i++)
                        {
                            NodePort connection = port.GetConnection(i);
                            (connection.node as VP_DialogBaseNode).Trigger(number);
                        }
                        break;
                }
            }
            else
            {
                CheckEndSequenceNode();
                return;
            }
        }
    }

}
