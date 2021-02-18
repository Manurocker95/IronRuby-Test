using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix.Level;

namespace VirtualPhenix.Example.Level
{
    [System.Serializable]
    public class VP_ExampleLevel : VP_Level<VP_ExampleLevelData>
    {
        public int Level
        {
            get
            {
                return Data.m_levelIndex;
            }
        }
    }

}
