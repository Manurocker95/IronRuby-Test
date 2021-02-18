using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if ODIN_INSPECTOR
using Sirenix.Serialization;
using Sirenix.OdinInspector;
#endif

namespace VirtualPhenix.Level
{

    [CreateAssetMenu(fileName = "Level Data", menuName = "Virtual Phenix/Levels/Level Data", order = 1)]
#if ODIN_INSPECTOR
    public class VP_LevelData : SerializedScriptableObject
#else
   public class VP_LevelData : ScriptableObject
#endif
    {
        // Level Data
        public int m_index;
        public VP_SceneReference m_scene;

        public string SceneName { get { return m_scene != null ? m_scene.SceneName : "Level " + m_index; } }
    }
}
