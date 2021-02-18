using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Level
{
    [DefaultExecutionOrder(VP_ExecutingOrderSetup.LEVEL_MANAGER), AddComponentMenu("")]
    public class VP_GenericLevelManager<T0, T1> : VP_LevelManagerBase where T0 : VP_Level<T1> where T1 : VP_LevelData
    {
        [Header("Level"), Space]
        [SerializeField] protected T0 m_currentLevel;

        public virtual T0 Level { get { return m_currentLevel; } }

        protected override void StartListenToLoadLevel()
        {
            VP_EventManager.StartListening<T0>(VP_EventSetup.Game.LOAD_LEVEL, SetCurrentLevel);
        }

        protected override void StopListenToLoadLevel()
        {
            VP_EventManager.StartListening<T0>(VP_EventSetup.Game.LOAD_LEVEL, SetCurrentLevel);
        }
        public virtual void SetCurrentLevel(T0 _level)
        {
            this.m_currentLevel = _level;

            LevelLoad();
        }
    }
}