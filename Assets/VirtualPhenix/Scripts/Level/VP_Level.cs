using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Level
{
    [System.Serializable]
    public class VP_Level<T> where T : VP_LevelData
    {
        [SerializeField] protected T m_LevelData;

        [SerializeField] protected bool m_completed;

        public virtual T Data { get { return m_LevelData; } }
        public virtual int Index { get { return m_LevelData.m_index; } }
        public virtual int Number { get { return m_LevelData.m_index + 1; } }
        public virtual string SceneName { get { return m_LevelData.SceneName; } }
        public virtual bool Completed { get { return m_completed; } set { m_completed = value; } }
    }
}
