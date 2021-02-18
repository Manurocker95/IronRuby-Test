using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace VirtualPhenix.Inventory
{
    [System.Serializable]
    public class VP_InventoryPocket
    {
        [Header("Inventory Pocket"), Space]
        [SerializeField] protected List<VP_InventoryItemData> m_allowedItemsInPocket;
        [SerializeField] protected List<VP_InventoryStack> m_stacks;
        [SerializeField] protected string m_pocketName;
        [SerializeField] protected bool m_createNewStackOnExcence = true;
        [SerializeField] protected bool m_removeStackOnZero = true;

	    protected VP_Inventory m_inventory;

	    public VP_Inventory Owner
	    {
		    get
		    {
		    	return m_inventory;
		    }
	    }

        public List<VP_InventoryStack> Stacks
        {
            get
            {
                return m_stacks;
            }
        }
        
        public List<VP_InventoryItemData> AllowedItemsInPocket
        {
            get
            {
                return m_allowedItemsInPocket;
            }
        }

        protected virtual void InitStacks(int _numberOfStacks, List<VP_InventoryItemData> _items, List<int> _quantities)
        {
            _numberOfStacks = Mathf.Clamp(_numberOfStacks, 0, VP_InventorySetup.MAX_STACKS_PER_POCKET);

            m_stacks = new List<VP_InventoryStack>();

            for(int i = 0; i < _numberOfStacks; i++)
            {
                CreateNewStack(_items[i], _quantities[i]);
            }
        }

        public virtual void OnStackClear(VP_InventoryStack _stack)
        {
            // Do Sth with the cleared stack
        }

        public virtual bool IsItemInPocket(VP_InventoryItemData _data)
        {
            if (_data == null)
                return false;

            return m_allowedItemsInPocket.Contains(_data);
        }

        public virtual void TryCreateNewStack(VP_InventoryItemData _data, int _quantity)
        {
            if (m_createNewStackOnExcence && m_stacks.Count < VP_InventorySetup.MAX_STACKS_PER_POCKET)
            {
                CreateNewStack(_data, _quantity);
            }
        }

        public virtual void TryRemoveStack(VP_InventoryStack _stack)
        {
            if (m_removeStackOnZero)
            {
                RemoveStack(_stack);
            }
        }

        public int GetQuantityOfItem(VP_InventoryItemData _item)
        {
            int qty = 0;
            foreach (VP_InventoryStack stack in m_stacks)
            {
                if (stack.ItemData == (_item))
                {
                    qty += stack.Quantity;
                }
            }

            return qty;
        }

        public virtual void CreateNewStack(VP_InventoryItemData _data, int _quantity)
        {
            if (m_stacks == null)
                m_stacks = new List<VP_InventoryStack>();

            var stack = new VP_InventoryStack(_data, _quantity);
            
            stack.AddCallbacks(TryCreateNewStack, TryRemoveStack, OnStackClear);

            m_stacks.Add(stack);
            m_inventory.OnStackCreation.Invoke(stack);
        }

        public virtual void RemoveStack(VP_InventoryStack _stack)
        {
            if (m_stacks == null)
                m_stacks = new List<VP_InventoryStack>();

            if (m_stacks.Contains(_stack))
            {
                m_stacks.Remove(_stack);
                m_inventory.OnStackRemoval.Invoke(_stack);
            }
        }

        public virtual void AddItem(VP_InventoryItemData _data, int _quantity)
        {
            if (_data == null)
                return;

            foreach (VP_InventoryStack stack in m_stacks)
            {
                if (stack.ItemData.Equals(_data))
                {
                    stack.AddItem(_data, _quantity);
                }
            }
        }

        public virtual void UseItem(VP_InventoryItemData _data)
        {
            if (_data == null)
                return;

            bool used = false;
            foreach (VP_InventoryStack stack in m_stacks)
            {
                if (stack.ItemData.Equals(_data))
                {
                    stack.Use();
                    used = true;
                }
            }

            if (used)
                m_inventory.OnItemUse.Invoke(_data);
        }
    }
}