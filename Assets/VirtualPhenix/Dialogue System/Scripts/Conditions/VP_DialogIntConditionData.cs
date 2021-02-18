using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Dialog
{
    [CreateAssetMenu(menuName = "Virtual Phenix/Dialogue System/Conditions/Integer Comparison")]
    public class VP_DialogIntConditionData : VP_DialogConditionData<int, int>
    {
        public override bool IsDifferent()
        {
            int v1 = (int)var1;
            int v2 = (int)var2;

            return (v1 != v2);
        }

        public override bool IsEqual()
        {
            int v1 = (int)var1;
            int v2 = (int)var2;

            return (v1 == v2);
        }

        public override bool IsGreater()
        {
            int v1 = (int)var1;
            int v2 = (int)var2;

            return v1 >= v2;
        }

        public override bool IsLower()
        {
            int v1 = (int)var1;
            int v2 = (int)var2;

            return v1 <= v2;
        }
    }

}
