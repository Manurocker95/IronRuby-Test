using UnityEngine;
using UnityEditor;

namespace Helpers.HierarchyIcons
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(HierarchyIcon))]
    public class HierarchyIconEditor : Editor
    {
        SerializedProperty m_iconProperty;
        SerializedProperty m_tooltipProperty;
        SerializedProperty m_positionProperty;
        SerializedProperty m_showInfoProperty;

        const float k_IconButtonSize = 28;

        void OnEnable()
        {
            m_iconProperty = serializedObject.FindProperty("icon");
            m_tooltipProperty = serializedObject.FindProperty("tooltip");
            m_positionProperty = serializedObject.FindProperty("m_iconPosition");
            m_showInfoProperty = serializedObject.FindProperty("showInfo");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            HierarchyIcon hierarchyIcon = (HierarchyIcon)target as HierarchyIcon;
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_showInfoProperty, true);
            if (EditorGUI.EndChangeCheck())
            {
                // repaint the hierarchy
                EditorApplication.RepaintHierarchyWindow();
            }

            if (hierarchyIcon.showIcon)
            {
                // draw the script header
                {
                    GUI.enabled = false;
                    DrawPropertiesExcluding(serializedObject, m_iconProperty.name, m_tooltipProperty.name, m_positionProperty.name);
                    GUI.enabled = true;
                }

                // Update the serializedProperty - always do this in the beginning of OnInspectorGUI.

                // draw the pick icon button
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.PrefixLabel("Icon");

                    Rect rectButton = EditorGUILayout.GetControlRect(GUILayout.Width(k_IconButtonSize), GUILayout.Height(k_IconButtonSize));

                    if (!m_iconProperty.hasMultipleDifferentValues)
                    {
                        if (GUI.Button(rectButton, hierarchyIcon.icon))
                        {
                            PopupWindow.Show(rectButton, new PickIconWindow(m_iconProperty));
                        }
                    }
                    else
                    {
                        if (GUI.Button(rectButton, "_"))
                        {
                            PopupWindow.Show(rectButton, new PickIconWindow(m_iconProperty));
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();
                // draw the position
                {
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(m_positionProperty, true);
                    if (EditorGUI.EndChangeCheck())
                    {
                        // repaint the hierarchy
                        EditorApplication.RepaintHierarchyWindow();
                    }
                }

                // draw the tooltip
                EditorGUILayout.PropertyField(m_tooltipProperty, true);
            }
      
            // Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
            serializedObject.ApplyModifiedProperties();
        }

    }

}