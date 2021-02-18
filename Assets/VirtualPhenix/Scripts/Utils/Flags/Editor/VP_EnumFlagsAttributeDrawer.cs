using UnityEditor;
using UnityEngine;

namespace VirtualPhenix
{
    [CustomPropertyDrawer(typeof(VP_EnumFlagsAttribute))]
    public class VP_EnumFlagsAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
        {

            VP_EnumFlagsAttribute labelAttribute = attribute as VP_EnumFlagsAttribute;
            /*
            EditorGUI.PropertyField(_position, _property, _label);
            GUI.Label(_position.right(labelAttribute.m_width + 2f), labelAttribute.m_label, labelAttribute.m_labelStyle);
            */

            // Change check is needed to prevent values being overwritten during multiple-selection
            UnityEditor.EditorGUI.BeginChangeCheck();
            int newValue = UnityEditor.EditorGUI.MaskField(_position, _label, _property.intValue, _property.enumNames);
            if (UnityEditor.EditorGUI.EndChangeCheck())
            {
                _property.intValue = newValue;
            }
        }
    }
}