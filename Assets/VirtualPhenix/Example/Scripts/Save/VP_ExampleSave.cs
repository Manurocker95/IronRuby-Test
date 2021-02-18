using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix.Save;
using VirtualPhenix.Example.Settings;

namespace VirtualPhenix.Example.Save
{
    public class VP_ExampleSave : VP_Save
    {
        public string m_customString = "Hello World";
        public VP_ExampleSettings m_settings;

        public VP_ExampleSave()
        {
            m_settings = new VP_ExampleSettings();
            m_customString = "Hello World";
        }

        public VP_ExampleSave(string _customString, VP_ExampleSettings _settings)
        {
            m_customString = _customString;
            m_settings = _settings;
        }
    }
}
