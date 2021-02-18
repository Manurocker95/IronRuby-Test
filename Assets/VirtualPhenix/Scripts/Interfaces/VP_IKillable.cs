using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix
{
    public interface VP_IKillable
    {
        GameObject gameObject { get; }

        void Damage(float _damage);
        void Recover(float _hp);
        
        void SetHP(float _hp);
        float GetHP();

        void SetMaxHP(float _hp);
        float GetMaxHP();

        void Kill(UnityEngine.Events.UnityAction _onKilled);
        void TriggerOnKill();
    }
}