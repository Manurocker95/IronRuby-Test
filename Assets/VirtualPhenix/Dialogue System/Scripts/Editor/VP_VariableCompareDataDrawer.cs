using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace VirtualPhenix.Dialog
{
    [CustomPropertyDrawer(typeof(VP_VariableCompareData))]
    public class VP_VariableCompareDataDrawer : PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
           // position.height+=50;
            label = EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginChangeCheck();

            // Store old indent level and set it to 0, the PrefixLabel takes care of it
            position = EditorGUI.PrefixLabel(position, label);

            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var varRect = new Rect(position.x - 80, position.y, 70, position.height);
            var typeRect = new Rect(position.x-5, position.y, 65, position.height);          
            var valueRect = new Rect(position.x+60, position.y, 40, position.height);
            var compRect = new Rect(position.x+110, position.y, 45, position.height);
          
            EditorGUI.PropertyField(typeRect, property.FindPropertyRelative("VarType"), GUIContent.none);
            EditorGUI.PropertyField(varRect, property.FindPropertyRelative("varName"), GUIContent.none);
            EditorGUI.PropertyField(compRect, property.FindPropertyRelative("variablecomparison"), GUIContent.none);
            
            var type = property.FindPropertyRelative("VarType");

            switch ((VariableTypes)type.intValue)
            {
                case VariableTypes.Bool:
                    EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("boolvalue"), GUIContent.none);
                    break;
                case VariableTypes.Int:
                    EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("intvalue"), GUIContent.none);
                    break;
                case VariableTypes.Float:
                    EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("floatvalue"), GUIContent.none);
                    break;
                case VariableTypes.Double:
                    EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("doublevalue"), GUIContent.none);
                    break;
                case VariableTypes.String:
                    EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("stringvalue"), GUIContent.none);
                    break;
                case VariableTypes.UnityObject:
                    EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("unityobjectvalue"), GUIContent.none);
                    break;
                case VariableTypes.GameObject:
                    EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("gameobjectvalue"), GUIContent.none);
                    break;
            }

            if (EditorGUI.EndChangeCheck())
                property.serializedObject.ApplyModifiedProperties();

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }

        private void SelectMatInfo(SerializedProperty property, VP_DialogCharacterData charInfo)
        {
            property.objectReferenceValue = charInfo;
            property.serializedObject.ApplyModifiedProperties();
            property.serializedObject.Update();
        }
    }

}
