using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix
{
    public class VP_SingletonScriptableObject<T> : VP_ScriptableObject where T : VP_ScriptableObject
    {
        private static T m_instance;
        public static T Instance { get { return m_instance; } }

        protected virtual string GetFilePath()
        {
            return "";
        }

        public virtual void CreateInstance()
        {

        }

        protected virtual void Save(bool saveAsText)
        {

        }
    }
}