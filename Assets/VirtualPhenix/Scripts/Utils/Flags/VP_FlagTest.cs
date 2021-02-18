using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Testing
{
    public class VP_FlagTest : VP_MonoBehaviour
    {
        [System.Flags]
        public enum AttackType
        {
            //               // Binary  // Dec
            None = 0,      // 000000  0
            Melee = 1 << 0, // 000001  1
            Fire = 1 << 1, // 000010  2
            Ice = 1 << 2, // 000100  4
            Poison = 1 << 3, // 001000  8
        }


        [SerializeField] public AttackType m_type = AttackType.None;

        protected override void Initialize()
        {
            base.Initialize();
            Debug.Log("Flag: " + m_type);
            m_type = m_type.SetFlag(AttackType.Ice | AttackType.Poison);
            Debug.Log("Flag: " + m_type);
            m_type = m_type.UnsetFlag(AttackType.Ice);
            Debug.Log("Flag: " + m_type);
        }
    }

}