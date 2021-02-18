using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix.Localization;

namespace VirtualPhenix.Inventory
{
    [System.Serializable]
    public class VP_InventoryItem : VP_IUsable
    {
        [SerializeField] protected VP_InventoryItemData m_itemData;

        public VP_InventoryItemData Data
        {
            get
            {
                return m_itemData;
            }
        }

        public bool IsInfinite
        {
            get
            {
                return Data != null ? Data.IsInfinite : false;
            }
        }

        public Sprite Icon
        {
            get
            {
                return m_itemData != null ? m_itemData.Icon : null;
            }
        }

        public string InternalName
        {
            get
            {
                return m_itemData != null ? m_itemData.InternalName : VP_TextSetup.Inventory.UNKNOWN_NAME;
            }
        }

        public string DisplayName
        {
            get
            {
                return m_itemData != null ? VP_LocalizationManager.Instance.GetTextTranslated(m_itemData.DisplayName) : VP_TextSetup.Inventory.UNKNOWN_NAME;
            }
        }

        public VP_InventoryItem()
        {

        }

        public VP_InventoryItem(VP_InventoryItemData _data)
        {
            m_itemData = _data;
        }

        public virtual void Use()
        {
            
        }
    }
}