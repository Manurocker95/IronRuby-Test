using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace VirtualPhenix.Dialog
{
    // prefab override logic works on the entire property.
    [CustomPropertyDrawer(typeof(VP_DialogStringConditionData))]
    public class VP_DialogStringConditionDataDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label = EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginChangeCheck();

            // Store old indent level and set it to 0, the PrefixLabel takes care of it

            position = EditorGUI.PrefixLabel(position, label);

            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            Rect buttonRect = position;
            buttonRect.width = 80;

            string buttonLabel = "Set";
            VP_DialogStringConditionData currentInfo = property.objectReferenceValue as VP_DialogStringConditionData;

            if (currentInfo == null)
                return;

            Rect txt1Rect = position;
            Rect txt2Rect = position;

            currentInfo.var1 = EditorGUI.TextField(txt1Rect, currentInfo.var1);
            currentInfo.var2 = EditorGUI.TextField(txt2Rect, currentInfo.var2);

            if (GUI.Button(buttonRect, buttonLabel))
            {
                SelectMatInfo(property, currentInfo);
            }

            position.x += buttonRect.width + 4;
            position.width -= buttonRect.width + 4;

            if (EditorGUI.EndChangeCheck())
                property.serializedObject.ApplyModifiedProperties();

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }

        private void SelectMatInfo(SerializedProperty property, VP_DialogStringConditionData info)
        {
            property.objectReferenceValue = info;
            property.serializedObject.ApplyModifiedProperties();
            property.serializedObject.Update();
        }
    }
}