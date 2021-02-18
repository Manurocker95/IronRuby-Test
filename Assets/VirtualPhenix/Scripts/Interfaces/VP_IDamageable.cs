using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix.Gameplay;

namespace VirtualPhenix
{
    public interface VP_IDamageable
    {
        void TakeDamage(VP_DamageData _data);
    }
}