using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix
{
    [CreateAssetMenu(fileName = "DefaultAudioDatabase", menuName = "Virtual Phenix/Scriptable Objects/Audio/Default", order = 1)]
    public class VP_DefaultAudioDatabase : VP_ScriptableObject
    {
        [SerializeField] protected VP_AudioItemDictionary m_audios;

        public virtual VP_AudioItemDictionary Audios { get { return m_audios; } }

        public virtual void Add(string _key, VP_AudioItem _value, bool _replace = false)
        {
            if (!Contains(_key))
                m_audios.Add(_key, _value);
            else if (_replace)
                m_audios[_key] = _value;
        }


        public virtual void Remove(string _key)
        {
            if (Contains(_key))
                m_audios.Remove(_key);
        }

        public virtual VP_AudioItem GetAudioItem(string _key)
        {
            if (Contains(_key))
            {
                return m_audios[_key];
            }

            return GetDefault();
        }

        public virtual bool Contains(VP_AudioItem _key)
        {
            if (m_audios.ContainsValueInDictionary(_key))
            {
                return true;
            }

            return false;

        }

        public virtual bool Contains(string _key)
        {
            if (m_audios.Contains(_key))
            {
                return true;
            }

            return false;

        }

        public virtual VP_AudioItem GetDefault()
        {
            return null;
        }
    }

    [System.Serializable]
    public class VP_AudioDatabase<T> : VP_ScriptableObject where T : VP_AudioItem
    {
        [SerializeField] protected VP_AudioDictionary<T> m_audios;

        public virtual VP_AudioDictionary<T> Audios { get { return m_audios; } }

        public virtual void Add(string _key, T _value, bool _replace = false)
        {
            if (!Contains(_key))
                m_audios.Add(_key, _value);
            else if (_replace)
                m_audios[_key] = _value;
        }


        public virtual void Remove(string _key)
        {
            if (Contains(_key))
                m_audios.Remove(_key);
        }

        public virtual T GetAudioItem(string _key)
        {
            if (Contains(_key))
            {
                return m_audios[_key];
            }

            return GetDefault();
        }

        public virtual bool Contains(T _key)
        {
            if (m_audios.ContainsValueInDictionary(_key))
            {
                return true;
            }

            return false;

        }

        public virtual bool Contains(string _key)
        {
            if (m_audios.Contains(_key))
            {
                return true;
            }

            return false;

        }

        public virtual T GetDefault()
        {
            return default(T);
        }
    }

    [System.Serializable]
    public class VP_AudioDictionary<T> : VP_SerializableDictionary<string, T> where T : VP_AudioItem
    {

    }

    [System.Serializable]
    public class VP_AudioItemDictionary: VP_SerializableDictionary<string, VP_AudioItem>
    {

    }
}
