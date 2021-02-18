﻿using UnityEditor;
using UnityEngine;

namespace VirtualPhenix.Dialog
{
    // prefab override logic works on the entire property.
    [CustomPropertyDrawer(typeof(VP_DialogPositionData))]
    public class VP_DialogPositionDrawer : PropertyDrawer
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

            string buttonLabel = "Select";
            VP_DialogPositionData currentCharInfo = property.objectReferenceValue as VP_DialogPositionData;


            if (currentCharInfo != null)
                buttonLabel = currentCharInfo.name;

            if (GUI.Button(buttonRect, buttonLabel))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("None"), currentCharInfo == null, () => SelectMatInfo(property, null));
                string[] guids = AssetDatabase.FindAssets("t:VP_DialogPositionData");
                for (int i = 0; i < guids.Length; i++)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                    VP_DialogPositionData matInfo = AssetDatabase.LoadAssetAtPath(path, typeof(VP_DialogPositionData)) as VP_DialogPositionData;
                    if (matInfo != null)
                    {
                        GUIContent content = new GUIContent(matInfo.name);
                        string[] nameParts = matInfo.name.Split(' ');
                        if (nameParts.Length > 1) content.text = nameParts[0] + "/" + matInfo.name.Substring(nameParts[0].Length + 1);
                        menu.AddItem(content, matInfo == currentCharInfo, () => SelectMatInfo(property, matInfo));
                    }
                }
                menu.ShowAsContext();
            }

            position.x += buttonRect.width + 4;
            position.width -= buttonRect.width + 4;
            EditorGUI.ObjectField(position, property, typeof(VP_DialogPositionData), GUIContent.none);

            if (EditorGUI.EndChangeCheck())
                property.serializedObject.ApplyModifiedProperties();

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }

        private void SelectMatInfo(SerializedProperty property, VP_DialogPositionData charInfo)
        {
            property.objectReferenceValue = charInfo;
            property.serializedObject.ApplyModifiedProperties();
            property.serializedObject.Update();
        }
    }
}