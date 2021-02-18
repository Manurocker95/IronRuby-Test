using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Dialog.Demo
{
    public class VP_TestCallbackListener : MonoBehaviour
    {
        void OnEnable()
        {
            VP_DialogManager.StartListeningToOnDialogStart(DialogStart);
            VP_DialogManager.StartListeningToOnTextShown(DialogTextShown);
            VP_DialogManager.StartListeningToOnDialogComplete(DialogCompleted);
            VP_DialogManager.StartListeningToOnDialogEnd(DialogEnd);
        }

        private void OnDisable()
        {
            VP_DialogManager.StopListeningToOnDialogStart(DialogStart);
            VP_DialogManager.StopListeningToOnTextShown(DialogTextShown);
            VP_DialogManager.StopListeningToOnDialogComplete(DialogCompleted);
            VP_DialogManager.StopListeningToOnDialogEnd(DialogEnd);
        }
        public void DialogTextShown()
        {
            Debug.Log("Text Shown");
        }
        public void DialogEnd()
        {
            Debug.Log("Dialog End");
        }
        public void DialogCompleted()
        {
            Debug.Log("Dialog Completed");
        }
        public void DialogStart()
        {
            Debug.Log("Dialog Start");
        }

    }

}
