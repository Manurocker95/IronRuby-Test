using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix
{
    public class VP_PoolTimeWheel<T, T0, T1> : VP_SingletonMonobehaviour<T> where T : VP_Monobehaviour
    {
        public enum UpdateSlots
        {
            None = -1,
            Update,
            LateUpdate,
            FixedUpdate,
            Manually
        }

        // Object T and its TTL
        protected List<Item>[] m_slots;
        protected int m_lastIndex = -1;
        [SerializeField] protected int m_poolSize = 128;
        [SerializeField] protected float m_granularity = 0.02f; // Seconds per step
        [SerializeField] protected UpdateSlots m_updateMode = UpdateSlots.None;


        public delegate void TimeOutDelegated(Item data);
        private TimeOutDelegated m_timeoutFunc;

        public virtual TimeOutDelegated TimeOutCallback { get { return m_timeoutFunc; } }


        public VP_PoolTimeWheel(TimeOutDelegated timeoutFunc)
        {
            m_slots = new List<Item>[m_poolSize]; // slots for 2" aprox
            for (int i = 0; i < m_poolSize; i++)
            {
                m_slots[i] = new List<Item>();
            }
            m_timeoutFunc = timeoutFunc;
        }

        /// <summary>
        /// Override this to specify how to create a resource.
        /// </summary>
        /// <returns>Newly created resource</returns>
        protected virtual T1 CreateResource()
        {
            return default(T1);
        }

        /// <summary>
        /// Override this to specify how to destroy a resource.
        /// </summary>
        /// <param name="resource">Resource to destroy</param>
        protected virtual void DestroyResource(T1 resource)
        {

        }

        public void Release(Item item)
        {
            item.m_used = false;
        }

        public void ReleaseAll()
        {
            foreach (List<Item> itemList in m_slots)
            {
                foreach (var item in itemList)
                {
                    Release(item);
                }
            }
        }

        protected virtual void Update()
        {
            if (m_updateMode == UpdateSlots.Update)
            {
                UpdatePool(Time.deltaTime);
            }
        }

        protected virtual void LateUpdate()
        {
            if (m_updateMode == UpdateSlots.LateUpdate)
            {
                UpdatePool(Time.deltaTime);
            }
        }

        protected virtual void FixedUpdate()
        {
            if (m_updateMode == UpdateSlots.FixedUpdate)
            {
                UpdatePool(Time.fixedDeltaTime);
            }
        }

        protected override void OnDestroy()
        {
            foreach (List<Item> itemList in m_slots)
            {
                foreach (var item in itemList)
                {
                    DestroyResource(item.m_resource);
                }
            } 
        }

        public virtual void UpdatePool(float _deltaTime)
        {
            foreach (List<Item> itemList in m_slots)
            {
                foreach (var item in itemList)
                {
                    if (item.m_used)
                        item.Update();
                }
            }
        }

        public virtual void Insert(Item item, int ttl)
        {
            int index = (m_lastIndex + (int)(ttl * m_granularity)) & (m_poolSize-1); // (time of death) & (size-1)
            m_slots[index].Add(item);
        }

        public virtual void Step()
        {
            m_lastIndex = (m_lastIndex + 1) & (m_poolSize - 1);
            ExpireSlot(m_lastIndex);
        }

        public virtual void ExpireSlot(int idx)
        {
            List<Item> slot = m_slots[idx];
            foreach (Item item in slot)
            {
                m_timeoutFunc(item);
                // NOTE: In a better/more generic Time Wheel, the entries' TTLs can be updated,
                //       so we should check it and move those entries to the corresponding slot.
                //       Here, for simplicity we don't enven store the TTLs, since we won't
                //       refresh the particles' TTL once emitted.
            }
            slot.Clear();
        }

        private Item Create(T0 _id, bool used = false, int ttl = 0)
        {
            // Create new pool item and add to pool

            var item = new Item() {m_id = _id, m_resource = CreateResource(), m_used = used };
            Insert(item, ttl);


            return item;
        }

        [System.Serializable]
        public class Item
        {
            public T0 m_id;
            public T1 m_resource;
            public bool m_used;

            public virtual void Update()
            {
      
            }
        }

    }
}
