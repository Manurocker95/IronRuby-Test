using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Inventory
{
    [CreateAssetMenu(menuName = VP_PrefixSetup.MAIN_PREFIX+"/Inventory/Inventory Item")]
    public class VP_InventoryItemData : VP_ScriptableObject
    {
        [SerializeField] protected string m_internalName;
        [SerializeField] protected string m_displayName;
        [SerializeField] protected Sprite m_icon;
        [SerializeField] protected bool m_isInfinite = false;

        public string InternalName
        {
            get
            {
                return m_internalName;
            }
        }

        public bool IsInfinite
        {
            get
            {
                return m_isInfinite;
            }
        }

        public string DisplayName
        {
            get
            {
                return m_displayName;
            }
        }

        public Sprite Icon
        {
            get
            {
                return m_icon;
            }
        }
    }
}