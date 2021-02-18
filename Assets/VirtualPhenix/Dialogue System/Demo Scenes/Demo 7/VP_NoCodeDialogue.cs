using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix
{
    public class VP_NoCodeDialogue : MonoBehaviour
    {
      
        // Start is called before the first frame update
        void Start()
        {
            VP_DialogManager.ShowDirectMessage("Hello, I am just showing this by code instead of graph!");
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}
