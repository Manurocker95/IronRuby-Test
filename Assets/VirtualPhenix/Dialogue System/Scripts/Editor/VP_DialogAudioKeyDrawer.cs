using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace VirtualPhenix.Dialog
{
    [CustomPropertyDrawer(typeof(VP_DialogAudioKey))]
    public class VP_DialogAudioKeyDrawer : PropertyDrawer
    {
        Vector2 scroll;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label = EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginChangeCheck();

            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            Rect buttonRect = position;
            buttonRect.width = 120;

            string buttonLabel = "Select Key";
            VP_DialogAudioKey currentKeyTest = property.objectReferenceValue as VP_DialogAudioKey;

            if (currentKeyTest == null)
            {
                return;
            }

            position.x += buttonRect.width + 4;
            position.width -= buttonRect.width + 4;
            //EditorGUI.ObjectField(position, property, typeof(VP_DialogAudioKey), GUIContent.none);

            if (GUI.Button(buttonRect, buttonLabel))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("None"), currentKeyTest == null, () => SelectStringInfo(property, null));

                string path = Application.dataPath + "/VirtualPhenix/Scripts/Setup/VP_AudioSetup.cs";

                List<VP_DialogSetupData> setupTexts = VP_SetupParser.ParseSetupInList(path);

                if (setupTexts != null)
                {
                    foreach (VP_DialogSetupData data in setupTexts)
                    {
                        GUIContent content = new GUIContent(data.className + "/" + data.variableName);
                        VP_DialogAudioKey newKey = ScriptableObject.CreateInstance<VP_DialogAudioKey>();
                        newKey.key = data.keyName;
                        menu.AddItem(content, newKey.key == currentKeyTest.key, () => SelectStringInfo(property, newKey));
                    }

                }

                menu.ShowAsContext();

            }
    
            if (EditorGUI.EndChangeCheck())
                property.serializedObject.ApplyModifiedProperties();

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }

        private void SelectStringInfo(SerializedProperty property, VP_DialogAudioKey charInfo)
        {
            VP_DialogAudioKey currentKeyTest = property.objectReferenceValue as VP_DialogAudioKey;

            if (currentKeyTest == null)
            {
                return;
            }

            currentKeyTest.key = (charInfo != null) ? charInfo.key : "";

            (property.serializedObject.targetObject as VP_DialogAudioNode).m_audioKey = charInfo.key;
            property.serializedObject.ApplyModifiedProperties();
            property.serializedObject.Update();
        }
    }

}
