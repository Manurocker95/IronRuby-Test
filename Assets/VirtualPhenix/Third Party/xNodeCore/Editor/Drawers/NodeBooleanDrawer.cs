using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XNode;
using XNodeEditor;

namespace XNodeEditor
{
    [CustomPropertyDrawer(typeof(NodeBooleanAttribute))]
    public class NodeBooleanDrawer : PropertyDrawer
    {
        private bool _value;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // Throw error on wrong type
            if (property.propertyType != SerializedPropertyType.Boolean)
            {
                throw new ArgumentException("Parameter selected must be of type System.Boolean");
            }

            // Add label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // Get current enum name
            string boolName = property.name;

            property.boolValue = EditorGUI.Toggle(position, "", property.boolValue);

            EditorGUI.EndProperty();
        }

        private void ShowContextMenuAtMouse(SerializedProperty property)
        {
            SetBoolean(property, !property.boolValue);
        }

        private void SetBoolean(SerializedProperty property, bool newVal)
        {
            property.boolValue = newVal;
            property.serializedObject.ApplyModifiedProperties();
            property.serializedObject.Update();
        }
    }
}