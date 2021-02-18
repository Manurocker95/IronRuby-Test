using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Dialog
{
    [CreateAssetMenu(menuName = "Virtual Phenix/Dialogue System/Conditions/GameObject Comparison")]
    public class VP_DialogGameObjectConditionData : VP_DialogConditionData<GameObject, GameObject>
    {
        public override bool IsDifferent()
        {
            GameObject v1 = (GameObject)var1 as GameObject;
            GameObject v2 = (GameObject)var2 as GameObject;

            return (v1 != v2);
        }

        public override bool IsEqual()
        {
            GameObject v1 = (GameObject)var1 as GameObject;
            GameObject v2 = (GameObject)var2 as GameObject;

            return (v1 == v2);
        }

        public override bool IsActive(int index)
        {
            return (index == 0) ? var1.activeSelf : var2.activeSelf;
        }

        public override bool IsTag(string tag, int index)
        {
            return (index == 0) ? var1.CompareTag(tag) : var2.CompareTag(tag);
        }
    }
}