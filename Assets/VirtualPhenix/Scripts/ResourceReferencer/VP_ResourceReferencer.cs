using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace VirtualPhenix.ResourceReference
{
	[System.Serializable]
    public class VP_ResourceReferencer<T, T0> : VP_ResourceReferencerBase
	{
#if ODIN_INSPECTOR    	
	    [Sirenix.Serialization.OdinSerialize] protected VP_SerializableDictionary<T, T0> m_resources = new VP_SerializableDictionary<T, T0>();
#else
		[SerializeField] protected VP_SerializableDictionary<T, T0> m_resources = new VP_SerializableDictionary<T, T0>();
#endif

        public virtual VP_SerializableDictionary<T, T0> Resources { get { return m_resources; } }
		public virtual int ResourceCount { get { return m_resources.Count; } }

        public virtual T0 GetRandomResource()
        {
            return m_resources != null && m_resources.Count > 0 ? m_resources.ElementAt((int)VP_Utils.Math.RandomNumber(0, m_resources.Count)).Value : default(T0);
        }

        public virtual KeyValuePair<T,T0> GetRandomElementInResource()
        {
            return m_resources != null && m_resources.Count > 0 ? m_resources.ElementAt((int)VP_Utils.Math.RandomNumber(0, m_resources.Count)) : new KeyValuePair<T, T0>();
        }

		public virtual KeyValuePair<T,T0> GetElementInResourceAtIndex(int _idx)
		{
			return m_resources != null && m_resources.Count > 0 && _idx < m_resources.Count ? m_resources.ElementAt(_idx) : new KeyValuePair<T, T0>();
		}


        public virtual bool TryGetResource(T _id, out T0 _value)
        {
            if (m_resources.ContainsKey(_id))
            {
                _value = m_resources[_id];
                return true;
            }
            
            _value = default(T0);

            return false;
        }

        public virtual T0 GetResource(T _id)
        {
            if (m_resources == null || _id == null) 
                return default(T0);

            if (m_resources.ContainsKey(_id))
                return m_resources[_id];

            return default(T0);
        }

        public virtual List<T0> GetResourceList()
        {
            List<T0> list = new List<T0>();
            foreach (T0 t in m_resources.Values)
            {
                list.Add(t);
            }

            return list;
        } 
        
        public virtual bool ContainsKey(T _key)
        {
            return m_resources.ContainsKey(_key);
        }

        public virtual bool ContainsValue(T0 _value)
        {
            return m_resources.ContainsValueInDictionary(_value);
        }    
        
        public virtual void Replace(T _key, T0 _value)
        {
            if (m_resources.ContainsValueInDictionary(_value))
            {
                m_resources[_key] = _value;
            }
        }
    
		public virtual void ClearResources()
		{
			m_resources = new VP_SerializableDictionary<T, T0>();
		}
    
		public virtual void Add(IList<KeyValuePair<T,T0>> _kvp, bool _replace = true)
		{
			foreach (KeyValuePair<T,T0> k in _kvp)
			{
				if (!m_resources.ContainsKey(k.Key))
				{
					m_resources.Add(k.Key, k.Value);
				}
				else if (_replace)
				{
					m_resources[k.Key] =  k.Value;
				}
			}
		}

        
        public virtual void Add(T _key, T0 _value, bool _replace = true)
        {
            if (!m_resources.ContainsValueInDictionary(_value))
            {
                m_resources.Add(_key, _value);
            }
            else if (_replace)
            {
                m_resources[_key] = _value;
            }
        }

        public virtual void Remove (T _key)
        {
            if (m_resources.ContainsKey(_key))
            {
                m_resources.Remove(_key);
            }
        }

        public virtual void RemoveAt(int _idx)
        {
            if (m_resources.Count > _idx && _idx >= 0)
            {
                m_resources.Remove(m_resources.Keys.ElementAt(_idx));
            }
        }
    }

}
