using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Dialog.Demo
{
    public class VP_DEMO_DirectMessages : MonoBehaviour
    {
        [SerializeField] List<string> m_listOfMessages = new List<string>();

        // Start is called before the first frame update
        void Start()
        {
            DirectMessages();
        }

        void DirectMessages()
        {
            foreach (string msg in m_listOfMessages)
                VP_DialogManager.ShowDirectMessage(msg);
        }
    }

}
