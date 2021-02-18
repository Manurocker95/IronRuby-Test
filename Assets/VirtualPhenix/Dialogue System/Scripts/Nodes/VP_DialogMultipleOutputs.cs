using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace VirtualPhenix.Dialog
{
    [NodeTint(VP_DialogSetup.NodeColors.SEQUENCE_NODE), CreateNodeMenuAttribute("Sequence")]
    public class VP_DialogMultipleOutputs : VP_DialogBaseNode
    {
        [Output(backingValue = ShowBackingValue.Unconnected)]
        public List<VP_DialogBaseNode> outputs;

        public int m_currentIndex = 0;
        public VP_DialogMultipleOutputs m_previousSequence;
       

        protected override void OnEnable()
        {
            base.OnEnable();

            if (outputs == null)
                outputs = new List<VP_DialogBaseNode>();

            //m_previousSequence = null;
        }

        // Use this for initialization
        protected override void Init()
        {
            base.Init();
            m_currentIndex = 0;
            m_previousSequence = null;
        }

        public override void OnCreateConnection(NodePort from, NodePort to)
        {
            base.OnCreateConnection(from, to);

            for(int i = 0; i < outputs.Count; i++)
            {
                if (from == GetOutputPort(VP_DialogSetup.Fields.DIALOG_NODE_MULTIPLE_OUTPUT + " " + i))
                {
                    outputs[i] = to.node as VP_DialogBaseNode;
                    break;
                }
            }
        }

        public override void OnRemoveConnection(NodePort port)
        {
            base.OnRemoveConnection(port);

            for (int i = 0; i < outputs.Count; i++)
            {
                if (port == GetOutputPort(VP_DialogSetup.Fields.DIALOG_NODE_MULTIPLE_OUTPUT + " " + i))
                {
                    outputs[i] = null;
                    break;
                }
            }
        }

        public override void Trigger()
        {
            NodePort port = null;
            VP_DialogManager.OnDialogCompleteAction();
            if (outputs.Count == 0) // nothing connected. We check the main port
            {
                if (port == null)
                {
                    port = GetOutputPort(VP_DialogSetup.Fields.DIALOG_NODE_OUTPUT);
                    if (port.ConnectionCount == 0)
                    {
                        if (m_previousSequence == null)
                        {
                            VP_DialogManager.OnDialogEndAction();
                        }
                        else
                        {
                            VP_DialogManager.StopListeningToOnDialogCompleteForOutput(Trigger);
                            VP_DialogManager.StartListeningToOnDialogCompleteForOutput(m_previousSequence.Trigger);
                            m_previousSequence = null;
                            // The previous will continue for us
                            VP_DialogManager.OnDialogCompleteOutputAction();
                        }
                    }
                    else
                    {
                        for (int i = 0; i < port.ConnectionCount; i++)
                        {
                            NodePort connection = port.GetConnection(i);
                            (connection.node as VP_DialogBaseNode).Trigger();
                        }
                    }
                }
            }
            else 
            {
                // current line of the sequence
                if (m_currentIndex < outputs.Count)
                {
                    // There was no previous line 
                    if (VP_DialogManager.OnDialogCompleteForOutput == null)
                    {
                        // we listen to the event
                        VP_DialogManager.StartListeningToOnDialogCompleteForOutput(Trigger);
                    }
                    else if (!m_previousSequence)
                    {
                        m_previousSequence = VP_DialogManager.OnDialogCompleteForOutput.Target as VP_DialogMultipleOutputs;
                        if (m_previousSequence != this)
                        {

                            // We delete all previous listeners
                            VP_DialogManager.StopListeningToOnDialogCompleteForOutput(m_previousSequence.Trigger);
                            // we listen to the event
                            VP_DialogManager.StartListeningToOnDialogCompleteForOutput(Trigger);                        
                        }
                        else
                        {
                            m_previousSequence = null;
                        }
                    }

                    // The node is null -> We continue to the next
                    if (outputs[m_currentIndex] == null)
                    {

                        m_currentIndex++;
                        // We trigger this. As we will be the only listeners, we can continue 
                        VP_DialogManager.OnDialogCompleteOutputAction();

                    }
                    else
                    {
                        // we have a node! :D 
                        NodePort outputport = GetOutputPort(VP_DialogSetup.Fields.DIALOG_NODE_MULTIPLE_OUTPUT + " " + m_currentIndex);
                        for (int i = 0; i < outputport.ConnectionCount; i++)
                        {
                            NodePort connection = outputport.GetConnection(i);
                            (connection.node as VP_DialogBaseNode).Trigger();
                        }

                        m_currentIndex++;
                    }
                }
                else
                {
                    // End of the first sequence in the tree (root one)
                    if (m_previousSequence == null)
                    {
                        VP_DialogManager.OnDialogCompleteForOutput = null;
                    }
                    else
                    {
                        VP_DialogManager.StopListeningToOnDialogCompleteForOutput(Trigger);
                        VP_DialogManager.StartListeningToOnDialogCompleteForOutput(m_previousSequence.Trigger);
                        m_previousSequence = null;
                    }

                    if (m_needToSkip)
                    {
                        m_needToSkip = false;
                    }

                    // Ended the sequence!
                    // We check if there is a previous sequence
                    CheckEndSequenceNode();
                }
            }
        }
    }
}

