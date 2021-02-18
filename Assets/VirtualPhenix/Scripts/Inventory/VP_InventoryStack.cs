using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace VirtualPhenix.Inventory
{
    [System.Serializable]
    public class VP_InventoryStack : VP_ITossable, VP_IUsable
    {
        [SerializeField] protected int m_quantity = 0;
        [SerializeField] protected VP_InventoryItem m_item;
        
        public UnityEvent<VP_InventoryItemData, int> OnTryToCreateStack;
        public UnityEvent<VP_InventoryStack> OnTryToRemoveStack;
        public UnityEvent<VP_InventoryStack> OnClearStack;

        public int Quantity
        {
            get
            {
                return m_quantity;
            }
        }

        public VP_InventoryItemData ItemData
        {
            get
            {
                return m_item.Data;
            }
        }

        public VP_InventoryItem InventoryItem
        {
            get
            {
                return m_item;
            }
        }

        public Sprite Icon
        {
            get
            {
                return m_item != null ? m_item.Icon : null;
            }
        }

        public VP_InventoryStack()
        {

        }

        public VP_InventoryStack(VP_InventoryItemData _item, int _quantity = 1)
        {

            AddItem(_item, _quantity, true);
        }

        public virtual void AddCallbacks(UnityAction<VP_InventoryItemData, int> _createCallback = null, UnityAction<VP_InventoryStack> _removeCallback = null, UnityAction<VP_InventoryStack> _clearCallback = null)
        {
            if (_createCallback != null && OnTryToCreateStack != null)
            {
                OnTryToCreateStack.AddListener(_createCallback);
            }

            if (_removeCallback != null && OnTryToRemoveStack != null)
            {
                OnTryToRemoveStack.AddListener(_removeCallback);
            }

            if (_clearCallback != null && OnClearStack != null)
            {
                OnClearStack.AddListener(_clearCallback);
            }
        }

        public virtual void AddItem(VP_InventoryItemData _item, int _quantity = 1, bool _force = false)
        {
            if (m_item.Data == _item || _force)
            {
                m_quantity += _quantity;

                int max = VP_InventorySetup.MAX_QUANTITY_PER_STACK;
                if (m_quantity > max && OnTryToCreateStack != null)
                {
                    OnTryToCreateStack.Invoke(_item, m_quantity - max);
                }

                m_quantity = Mathf.Clamp(m_quantity, 0, max);
            }
        }

        public virtual void RemoveItem(VP_InventoryItemData _item, int _quantity = 1)
        {
            if (m_item.Data == _item)
            {
                m_quantity -= _quantity;

                m_quantity = Mathf.Clamp(m_quantity, 0, m_quantity);
            }
        }
            
        public virtual void Toss(int _quantity = 1)
        {
            m_quantity -= _quantity;
            m_quantity = Mathf.Clamp(m_quantity, 0, m_quantity);

            if (m_quantity <= 0 && OnTryToRemoveStack != null)
            {
                OnTryToRemoveStack.Invoke(this);
            }
        }

        public virtual void Use()
        {
            if (m_quantity > 0)
            {

            }
        }

        public virtual void ClearStack()
        {
            m_quantity = 0;

            if (OnClearStack != null)
                OnClearStack.Invoke(this);
        }
    }
}