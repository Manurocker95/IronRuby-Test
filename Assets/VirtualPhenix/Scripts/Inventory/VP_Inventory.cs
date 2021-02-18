using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VirtualPhenix.Controllers.Components;

namespace VirtualPhenix.Inventory
{
    [AddComponentMenu(VP_PrefixSetup.MAIN_PREFIX + "/Inventory/Inventory")]
    public class VP_Inventory : VP_CharacterComponent
    {
        [SerializeField] protected VP_InventoryPocket[] m_pockets;

        public UnityEvent<VP_InventoryItemData> OnItemUse;
        public UnityEvent<VP_InventoryStack> OnStackCreation;
        public UnityEvent<VP_InventoryStack> OnStackRemoval;

        public VP_InventoryPocket[] Pockets
        {
            get
            {
                return m_pockets;
            }
        }

        public int GetQuantityOfItem(VP_InventoryItemData _item)
        {
            foreach (VP_InventoryPocket pocket in m_pockets)
            {
                if (pocket.IsItemInPocket(_item))
                {
                    return pocket.GetQuantityOfItem(_item);
                }
            }

            return 0;
        }
    }
}