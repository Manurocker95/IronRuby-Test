using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace VirtualPhenix.Dialog
{
    [NodeTint(VP_DialogSetup.NodeColors.LOG_NODE), CreateNodeMenuAttribute("Debug Log")]
    public class VP_DialogLogNode : VP_DialogBaseNode
    {
        public enum LOG_TYPE
        {
            LOG,
            WARNING,
            ERROR
        }

        [TextArea, SerializeField] private string m_message = "";
        [SerializeField] private LOG_TYPE m_logType = LOG_TYPE.LOG;

        private string finalText;


        public void ReplaceVariables()
        {
          
            var textAsSymbolList = VP_Utils.DialogUtils.CreateSymbolListFromText(m_message);
            finalText = m_message;
            int printedCharCount = 0;
            int skipCounter = 0;
            foreach (var symbol in textAsSymbolList)
            {
                if (skipCounter > 0)
                {
                    skipCounter--;
                    continue;
                }

                if (symbol.IsTag)
                {
                    if (symbol.Tag.TagType == VP_DialogSetup.Tags.VARIABLE)
                    {
                        if (symbol.Tag.IsOpeningTag)
                        {
                            
                            var txt = VP_DialogManager.Instance.GetVariableStringValueFromStringType(symbol.Tag.Parameter, symbol.Tag.m_middleText);
                            skipCounter += symbol.Tag.m_fullText.Length;
                            printedCharCount += txt.Length;
                            finalText = finalText.Replace(symbol.Tag.m_fullText, txt);
                        }
                    }
                    else if (symbol.Tag.TagType == VP_DialogSetup.Tags.GRAPH_VARIABLE)
                    {
                        if (symbol.Tag.IsOpeningTag)
                        {
                            var txt = VP_DialogManager.Instance.GetGraphVariableStringValueFromStringType(symbol.Tag.Parameter, symbol.Tag.m_middleText);
                            skipCounter += symbol.Tag.m_fullText.Length;
                            printedCharCount += txt.Length;
                            finalText = finalText.Replace(symbol.Tag.m_fullText, txt);
                        }
                    }
                    else
                    {
                        // Unrecognized CustomTag Type. Should we error here?
                    }
                }
                else
                {
                    printedCharCount++;
                }
            }
        }

        public override void Trigger<T>(T parameter)
        {
            if (parameter != null)
            {
                ReplaceVariables();
                finalText = finalText.Replace("{parameter}", parameter.ToString());

                switch (m_logType)
                {
                    case LOG_TYPE.LOG:
                        Debug.Log(finalText);
                        break;
                    case LOG_TYPE.WARNING:
                        Debug.LogWarning(finalText);
                        break;
                    case LOG_TYPE.ERROR:
                        Debug.LogError(finalText);
                        break;
                    default:
                        Debug.Log(finalText);
                        break;
                }


                NodePort port = GetOutputPort(VP_DialogSetup.Fields.DIALOG_NODE_OUTPUT);

                if (port.ConnectionCount > 0)
                {
                    for (int i = 0; i < port.ConnectionCount; i++)
                    {
                        NodePort connection = port.GetConnection(i);
                        (connection.node as VP_DialogBaseNode).Trigger(finalText);
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
            ReplaceVariables();
            finalText = finalText.Replace("{parameter}", "[null parameter]");

            switch (m_logType)
            {
                case LOG_TYPE.LOG:
                    Debug.Log($"{finalText}");
                    break;
                case LOG_TYPE.WARNING:
                    Debug.LogWarning($"{finalText}");
                    break;
                case LOG_TYPE.ERROR:
                    Debug.LogError($"{finalText}");
                    break;
                default:
                    Debug.Log($"{finalText}");
                    break;
            }
          

            NodePort port = GetOutputPort(VP_DialogSetup.Fields.DIALOG_NODE_OUTPUT);

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
                return;
            }
        }
    }

}
