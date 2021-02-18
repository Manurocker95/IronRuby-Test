using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Dialog
{
    [CreateAssetMenu(menuName = "Virtual Phenix/Dialogue System/Conditions/Float Comparison")]
    public class VP_DialogFloatConditionData : VP_DialogConditionData<float, float>
    {
        public override bool IsDifferent()
        {
            float v1 = (float)var1;
            float v2 = (float)var2;

            return (v1 != v2);
        }

        public override bool IsEqual()
        {
            float v1 = (float)var1;
            float v2 = (float)var2;

            return (v1 == v2);
        }

        public override bool IsGreater()
        {
            float v1 = (float)var1;
            float v2 = (float)var2;

            return v1 >= v2;
        }

        public override bool IsLower()
        {
            float v1 = (float)var1;
            float v2 = (float)var2;

            return v1 <= v2;
        }
    }

}
