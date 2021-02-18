using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Dialog
{
    [CreateAssetMenu(menuName = "Virtual Phenix/Dialogue System/Conditions/Script")]
    public class VP_DialogMonobehaviourConditionData : VP_DialogConditionData<MonoBehaviour>
    {
        public override bool CallMethod(string methodName)
        {
            bool retVal = false;
            var1.SendMessage(methodName, retVal);

            return retVal;
        }

        public override bool IsActive()
        {
            return var1.enabled;
        }

        public override bool IsTag(string tag)
        {
            return var1.CompareTag(tag);
        }
    }
}