using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix.Level;

namespace VirtualPhenix.Example.Level
{
    [
        DefaultExecutionOrder(VP_ExecutingOrderSetup.LEVEL_MANAGER),
        AddComponentMenu(VP_PrefixSetup.MAIN_PREFIX + "/Example/Example Level Manager")
    ]
    public class VP_ExampleLevelManager : VP_GenericLevelManager<VP_ExampleLevel, VP_ExampleLevelData>
    {
        [SerializeField] private VP_ExampleLevel[] m_levels;
        [SerializeField] private TMPro.TMP_Text m_currentLevelTxt;
        public void AddLevel(int _index)
        {
            if (m_levels.Length > _index && m_levels.Length > 0)
            {
                SetCurrentLevel(m_levels[_index]);
                m_currentLevelTxt.text = "Level: "+ m_currentLevel.Level;
            }
        }

        protected override void Start()
        {
            base.Start();
            m_currentLevelTxt.text = "Level: " + m_currentLevel.Level;
        }
    }
}
