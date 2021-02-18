using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix
{
    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = true)]
    public class VP_EnumFlagsAttribute : PropertyAttribute
    {

        public string m_label;
        public GUIStyle m_labelStyle;
        public float m_width;


        public VP_EnumFlagsAttribute() 
        {

        }

        public VP_EnumFlagsAttribute(string _label = "Flag")
        {
            m_label = _label;
            m_labelStyle = GUI.skin.GetStyle("miniLabel");
            m_width = m_labelStyle.CalcSize(new GUIContent(m_label)).x;
        }
    }
}
