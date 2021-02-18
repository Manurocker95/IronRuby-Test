using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Dialog
{
    [CreateAssetMenu(menuName = "Virtual Phenix/Dialogue System/Conditions/String Comparison")]
    public class VP_DialogStringConditionData : VP_DialogConditionData<string, string>
    {
        public override bool IsDifferent()
        {
            string v1 = (string)var1;
            string v2 = (string)var2;

            return (v1 != v2);
        }

        public override bool IsEqual()
        {
            string v1 = (string)var1;
            string v2 = (string)var2;

            return (v1 == v2);
        }


    }

}
