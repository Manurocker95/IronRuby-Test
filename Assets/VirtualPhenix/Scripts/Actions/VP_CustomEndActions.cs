using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Actions
{
    public class VP_CustomEndActions : VP_CustomActions
    {
        public override void InvokeAction()
        {
            base.InvokeAction();
            VP_ActionManager.Instance.ClearPendingActions();
        }
    }

}
