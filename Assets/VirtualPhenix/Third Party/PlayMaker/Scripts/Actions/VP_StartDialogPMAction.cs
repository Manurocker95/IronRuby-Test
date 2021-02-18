#if PLAYMAKER
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HutongGames.PlayMaker.Actions;
using HutongGames.PlayMaker;

namespace VirtualPhenix.Dialog
{
    [ActionCategory(ActionCategory.GameLogic)]
    [HutongGames.PlayMaker.Tooltip("Starts dialog calling VP_DialogManager"), Title("Phoenix Dialogue")]
    public class VP_StartDialogPMAction : FsmStateAction
    {
        [RequiredField]
        [HutongGames.PlayMaker.Tooltip("Key used in the dialogue graph for starting dialogue.")]
        public FsmString initKeyForGraph;

        [HutongGames.PlayMaker.Tooltip("Is is not key, it will be considered direct message")]
        public bool isKey = true;

        [HutongGames.PlayMaker.Tooltip("Is is not key, it will translate to the current language")]
        public bool translate = false;
        [HutongGames.PlayMaker.Tooltip("Is is not key, it will show directly without fading")]
        public bool showDirectly = false;
        [HutongGames.PlayMaker.Tooltip("Is is not key, Custom message added?")]
        public VP_DialogMessage customMessage = null;

        [HutongGames.PlayMaker.Tooltip("Do you have a character??")]
        public VP_DialogCharacterData characterData = null;

        [HutongGames.PlayMaker.Tooltip("Specific Position data??")]
        public VP_DialogPositionData positionData = null;

        [HutongGames.PlayMaker.Tooltip("Type of Dialog")]
        public DIALOG_TYPE dialogType = DIALOG_TYPE.REGULAR;

        public override void Reset()
        {

        }

        public override void OnEnter()
        {
            VP_DialogManager.StartListeningToOnDialogEnd(DialogEnded);

            if (isKey)
                VP_DialogManager.SendDialogMessage(initKeyForGraph.Value);
            else
                VP_DialogManager.ShowDirectMessage(initKeyForGraph.Value, null, dialogType, translate, showDirectly, true, customMessage, null, null,null, null, positionData, true, true,0.5f, true, characterData);
        }

        public override void OnUpdate()
        {
            // update time
        }

        void DialogEnded()
        {
            VP_DialogManager.StartListeningToOnDialogEnd(DialogEnded);

            OnExit();
        }

    }
}
#endif