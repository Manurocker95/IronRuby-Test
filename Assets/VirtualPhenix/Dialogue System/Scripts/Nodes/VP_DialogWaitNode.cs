using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace VirtualPhenix.Dialog
{
    [NodeTint(VP_DialogSetup.NodeColors.WAIT_NODE), CreateNodeMenuAttribute("Wait")]
    public class VP_DialogWaitNode : VP_DialogBaseNode
    {
        [SerializeField] public float m_waitTime = 1f;
        [SerializeField] public bool m_stopPrevious = true;

        public override void Trigger()
        {

            if (output != null)
            {
                if (m_stopPrevious)
                    VP_DialogManager.Instance.StopCoroutine(WaitForTriggering());

                VP_DialogManager.Instance.StartCoroutine(WaitForTriggering());
            }
            else
            {
                
                if (VP_DialogManager.OnDialogCompleteForOutput == null)
                {
                    VP_DialogManager.OnDialogCompleteAction();
                    VP_DialogManager.OnDialogEndAction();
                }
                else
                {

                    if (m_stopPrevious)
                        VP_DialogManager.Instance.StopCoroutine(WaitForTriggering(true));

                    VP_DialogManager.Instance.StartCoroutine(WaitForTriggering(true));
                   
                }
            }
        }

        public IEnumerator WaitForTriggering(bool _OnComplete = false)
        {
            float timer = 0f;

            while (timer < m_waitTime && !m_needToSkip)
            {
                timer += Time.deltaTime;
                yield return null;
            }
            m_needToSkip = false;
            VP_DialogManager.OnDialogCompleteAction();
            if (!_OnComplete)
            {
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
                    VP_DialogManager.OnDialogEndAction();
                }
            }
            else
            {
                VP_DialogManager.OnDialogCompleteOutputAction();
            }
        }
    }
}
