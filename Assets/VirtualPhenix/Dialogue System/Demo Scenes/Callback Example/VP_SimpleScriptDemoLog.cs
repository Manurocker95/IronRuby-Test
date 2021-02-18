using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Dialog.Demo
{
    public class VP_SimpleScriptDemoLog : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        public void ShowLog(string _log = "")
        {
            Debug.Log("LISTENER: "+name+ " AND LOG: "+ _log);
        }

        public void TriggerEvent(string _event)
        {
            VP_EventManager.TriggerEvent(_event);
        }
    }

}
