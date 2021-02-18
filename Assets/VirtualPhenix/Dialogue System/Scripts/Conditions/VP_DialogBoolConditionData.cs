using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Dialog
{
    [CreateAssetMenu(menuName = "Virtual Phenix/Dialogue System/Conditions/Bool Comparison")]
    public class VP_DialogBoolConditionData : VP_DialogConditionData<bool, bool>
    {
        public bool IsTrue()
        {
            var2 = true;
            return IsEqual();
        }

        public bool IsFalse()
        {
            var2 = true;
            return IsDifferent();
        }

        public override bool IsEqual()
        {
            bool v1 = (bool)var1;
            bool v2 = (bool)var2;

            return (v1 == v2);
        }
    }
}
