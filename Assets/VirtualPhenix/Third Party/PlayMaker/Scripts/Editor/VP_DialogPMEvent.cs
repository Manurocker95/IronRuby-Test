#if PLAYMAKER
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using HutongGames.PlayMaker;
using HutongGames.PlayMakerEditor;

namespace VirtualPhenix.Dialog.PlayMaker
{
    public class VP_DialogPMEvent : MonoBehaviour
    {
        [MenuItem("PlayMaker/Addons/Virtual Phenix/Dialogue Callback Events &b")]
        static void SetupCustomDialogEvents()
        {
            FsmEvent dialogueStartCallback = new FsmEvent("VIRTUAL PHENIX / ON DIALOGUE START");
            FsmEvent dialogueEndCallback = new FsmEvent("VIRTUAL PHENIX / ON DIALOGUE END");
            FsmEvent dialogueTextShownCallback = new FsmEvent("VIRTUAL PHENIX / ON TEXT SHOWN");
            FsmEvent dialogueAnswersShownCallback = new FsmEvent("VIRTUAL PHENIX / ON ANSWERS SHOWN");
            FsmEvent dialogueAnswerSelectedCallback = new FsmEvent("VIRTUAL PHENIX / ON CHOICE SELECTED");
            FsmEvent dialogueSkipCallback = new FsmEvent("VIRTUAL PHENIX / ON SKIP");
            FsmEvent dialogueCustomEvent = new FsmEvent("VIRTUAL PHENIX / ON CUSTOM EVENT");


            if (FsmEvent.GetFsmEvent(dialogueStartCallback) == null)
                FsmEvent.AddFsmEvent(dialogueStartCallback);

            if (FsmEvent.GetFsmEvent(dialogueEndCallback) == null)
                FsmEvent.AddFsmEvent(dialogueEndCallback);

            if (FsmEvent.GetFsmEvent(dialogueTextShownCallback) == null)
                FsmEvent.AddFsmEvent(dialogueTextShownCallback);

            if (FsmEvent.GetFsmEvent(dialogueAnswersShownCallback) == null)
                FsmEvent.AddFsmEvent(dialogueAnswersShownCallback);

            if (FsmEvent.GetFsmEvent(dialogueAnswerSelectedCallback) == null)
                FsmEvent.AddFsmEvent(dialogueAnswerSelectedCallback);

            if (FsmEvent.GetFsmEvent(dialogueSkipCallback) == null)
                FsmEvent.AddFsmEvent(dialogueSkipCallback);

            if (FsmEvent.GetFsmEvent(dialogueCustomEvent) == null)
                FsmEvent.AddFsmEvent(dialogueCustomEvent);
        }
    }
}
#endif