using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix.Level;

namespace VirtualPhenix.Example.Level
{
    [CreateAssetMenu(fileName = "Level Data", menuName = "Virtual Phenix/Example/Levels/Level Data", order = 1)]
    public class VP_ExampleLevelData : VP_LevelData
    {
        [Header("Example"),Space]
        public int m_levelIndex = 100;
    }
}
