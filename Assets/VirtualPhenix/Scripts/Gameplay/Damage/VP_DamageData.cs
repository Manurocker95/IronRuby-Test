using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Gameplay
{
    [System.Serializable]
    public class VP_DamageData 
    {
        public float Damage;
        public RaycastHit HitInfo;
        public VP_DamageSource Source;
        public UnityEngine.Events.UnityAction Callback;
    }
}