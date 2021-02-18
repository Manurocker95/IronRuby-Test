using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix
{
    public class VP_ParticlePoolTimeWheel
    {
        // Object T and its TTL
        private List<KeyValuePair<ParticleSystem,string>>[] m_slots;
        private int m_lastIndex = -1;

        public delegate void Delegated(KeyValuePair<ParticleSystem, string> data);
        public Delegated m_timeoutFunc;
        private float m_granularity = 0.02f; // Seconds per step

        public VP_ParticlePoolTimeWheel (Delegated timeoutFunc)
        {
            m_slots = new List<KeyValuePair<ParticleSystem,string>>[128]; // slots for 2" aprox
            for (int i=0; i<128; i++) {
                m_slots[i] = new List<KeyValuePair<ParticleSystem,string>>();
            }
            m_timeoutFunc = timeoutFunc;
        }

        public void ExpireSlot(int idx)
        {
            List<KeyValuePair<ParticleSystem,string>> slot = m_slots[idx];
            foreach (KeyValuePair<ParticleSystem,string> item in slot)
            {
                m_timeoutFunc(item);
                // NOTE: In a better/more generic Time Wheel, the entries' TTLs can be updated,
                //       so we should check it and move those entries to the corresponding slot.
                //       Here, for simplicity we don't enven store the TTLs, since we won't
                //       refresh the particles' TTL once emitted.
            }
            slot.Clear();
        }

        public void Insert(KeyValuePair<ParticleSystem,string> item, int ttl)
        {
            int index = (m_lastIndex+(int)(ttl*m_granularity)) & 127; // (time of death) & (size-1)
            m_slots[index].Add(item);
        }

        public void Step()
        {
            m_lastIndex = (m_lastIndex+1) & 127;
            ExpireSlot(m_lastIndex);
        }
    }
}