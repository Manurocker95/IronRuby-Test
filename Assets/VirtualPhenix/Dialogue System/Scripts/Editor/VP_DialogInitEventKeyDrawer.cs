using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace VirtualPhenix.Dialog
{
    [CustomPropertyDrawer(typeof(VP_DialogInitEventKey))]
    public class VP_DialogInitEventKeyDrawer : PropertyDrawer
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
            VP_DialogInitEventKey currentKeyTest = property.objectReferenceValue as VP_DialogInitEventKey;

            if (currentKeyTest == null)
            {
                return;
            }

            position.x += buttonRect.width + 4;
            position.width -= buttonRect.width + 4;
            EditorGUI.ObjectField(position, property, typeof(VP_DialogInitEventKey), GUIContent.none);

            if (GUI.Button(buttonRect, buttonLabel))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("None"), currentKeyTest == null, () => SelectStringInfo(property, null));

                string path = currentKeyTest.m_keySetup; 

                if (string.IsNullOrEmpty(path))
                {
                    path = Application.dataPath + "/VirtualPhenix/Dialogue System/Scripts/Setup/VP_DialogSetup.cs";
                    currentKeyTest.m_keySetup = path;
                    AssetDatabase.SaveAssets();
                }

                List <VP_DialogSetupData> setupTexts = VP_SetupParser.ParseSetupInList(path);

                if (setupTexts != null)
                {
                    foreach (VP_DialogSetupData data in setupTexts)
                    {
                        GUIContent content = new GUIContent(data.className + "/" + data.variableName);
                        VP_DialogInitEventKey newKey = ScriptableObject.CreateInstance<VP_DialogInitEventKey>();
                        newKey.key = data.keyName;
                        menu.AddItem(content, newKey.key == currentKeyTest.key, () => SelectStringInfo(property, newKey));
                    }

                }

                /*
                TextAsset asset = Resources.Load<TextAsset>(Localization.VP_LocalizationSetup.Folder.LOCALIZATION_FOLDER_CSV + "English");
                Dictionary<string, Localization.VP_TextItem> m_texts = VP_CSVParser.ParseCSV(asset);
                foreach (string key in m_texts.Keys)
                {
                    GUIContent content = new GUIContent(key);
                    VP_DialogInitEventKey newKey = ScriptableObject.CreateInstance<VP_DialogInitEventKey>();
                    newKey.key = key;
                    menu.AddItem(content, newKey.key == currentKeyTest.key, () => SelectStringInfo(property, newKey));
                }
                */
                menu.ShowAsContext();

            }

            if (EditorGUI.EndChangeCheck())
                property.serializedObject.ApplyModifiedProperties();

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }

        private void SelectStringInfo(SerializedProperty property, VP_DialogInitEventKey charInfo)
        {
            VP_DialogInitEventKey currentKeyTest = property.objectReferenceValue as VP_DialogInitEventKey;

            if (currentKeyTest == null)
            {
                return;
            }

            currentKeyTest.key = (charInfo != null) ? charInfo.key : "";

            if (property.serializedObject.targetObject is VP_Dialog)
            {
                (property.serializedObject.targetObject as VP_Dialog).key = currentKeyTest.key;
            }

            else if (property.serializedObject.targetObject is VP_DialogInitEvent)
            {                
                (property.serializedObject.targetObject as VP_DialogInitEvent).startEvent = currentKeyTest.key;
            }
            else if (property.serializedObject.targetObject is VP_DialogInitializer)
            {
                (property.serializedObject.targetObject as VP_DialogInitializer).m_key = currentKeyTest.key;
            }
            else if (property.serializedObject.targetObject is VP_SimpleTalker)
            {
                (property.serializedObject.targetObject as VP_SimpleTalker).m_key = currentKeyTest.key;
            }
            property.serializedObject.ApplyModifiedProperties();
            property.serializedObject.Update();
        }
    }

}
