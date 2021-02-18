using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Dialog.Demo
{
    public class VP_DialogTest : MonoBehaviour
    {
        private string m_startMessage = "";

        // Start is called before the first frame update
        void Start()
        {
            m_startMessage = VP_DialogSetup.InitEvents.TEST_INIT_EVENT_DEMO3;
        }

        private void Update()
        {
            ///Start the dialog when pressing return key
            if (Input.GetKeyDown(KeyCode.Return) && !VP_DialogManager.IsSpeaking)
            {
                VP_DialogManager.Instance._SendMessage(m_startMessage);
            }
        }
    }
}
