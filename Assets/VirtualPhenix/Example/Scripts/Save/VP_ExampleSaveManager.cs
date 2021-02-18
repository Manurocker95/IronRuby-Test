using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix.Example.Save;
using UnityEngine.UI;
using TMPro;
using VirtualPhenix.Save;

namespace VirtualPhenix.Example
{
    [
         DefaultExecutionOrder(VP_ExecutingOrderSetup.SAVE_MANAGER),
         AddComponentMenu(VP_PrefixSetup.MAIN_PREFIX + "/Example/Example Save Manager")
    ]
    public class VP_ExampleSaveManager : VP_GenericSaveManager<VP_ExampleSave>
    {
        [Header("Example Save Manager"), Space]
        [SerializeField] protected TMP_Text m_text;

        /// <summary>
        /// Important to define this. default(T) might return null
        /// </summary>
        /// <returns></returns>
        public override VP_ExampleSave DefaultValue()
        {
            return new VP_ExampleSave();
        }

        public override void InitSave()
        {
            base.InitSave();
            LoadFromButton();
        }

        public void LoadFromButton()
        {
            if (m_text != null)
            {
                m_text.text = m_save.m_customString;
            }
        }

        public void SaveFromButton()
        {
            if (m_text != null)
            {
                m_save.m_customString = m_text.text;
            }
        }
    }

}
