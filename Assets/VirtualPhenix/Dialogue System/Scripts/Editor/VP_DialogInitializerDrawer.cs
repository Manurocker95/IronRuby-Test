using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace VirtualPhenix.Dialog
{
   
    [CustomEditor(typeof(VP_DialogInitializer))]
    public class VP_DialogInitializerDrawer : Editor
    {
        private void OnEnable()
        {
            VP_DialogInitializer init = target as VP_DialogInitializer;
            if (init.m_initKey == null)
            {
                init.m_initKey = Resources.Load<VP_DialogInitEventKey>("Dialogue/Data/defaultInitEvents");
                init.m_initKey.key = "";
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            VP_DialogInitializer init = target as VP_DialogInitializer;

            List<VP_DialogInitializer.INIT_ACTION> list = init.Actions;

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            SerializedProperty keyp = serializedObject.FindProperty("m_initKey");
            EditorGUILayout.PropertyField(keyp, true);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_key"), true);
            EditorGUILayout.Space();
            init.m_key = init.m_initKey.key;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_actions"), true);
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            if (list.Contains(VP_DialogInitializer.INIT_ACTION.ON_COLLISION_ENTER) ||
               list.Contains(VP_DialogInitializer.INIT_ACTION.ON_COLLISION_ENTER_2D) ||
               list.Contains(VP_DialogInitializer.INIT_ACTION.ON_COLLISION_EXIT) ||
               list.Contains(VP_DialogInitializer.INIT_ACTION.ON_COLLISION_EXIT_2D) ||

               list.Contains(VP_DialogInitializer.INIT_ACTION.ON_TRIGGER_ENTER) ||
               list.Contains(VP_DialogInitializer.INIT_ACTION.ON_TRIGGER_ENTER_2D) ||
               list.Contains(VP_DialogInitializer.INIT_ACTION.ON_TRIGGER_EXIT) ||
               list.Contains(VP_DialogInitializer.INIT_ACTION.ON_TRIGGER_EXIT_2D) ||

               list.Contains(VP_DialogInitializer.INIT_ACTION.ON_COLLISION_ENTER))
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_triggerTag"), true);
                EditorGUILayout.Space();
            }

            if (list.Contains(VP_DialogInitializer.INIT_ACTION.ON_KEY_DOWN) ||
                list.Contains(VP_DialogInitializer.INIT_ACTION.ON_KEY_UP))
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_triggerKey"), true);
                EditorGUILayout.Space();
            }

            if (list.Contains(VP_DialogInitializer.INIT_ACTION.ON_BUTTON_DOWN) ||
                list.Contains(VP_DialogInitializer.INIT_ACTION.ON_BUTTON_UP))
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_buttonName"), true);
                EditorGUILayout.Space();
            }

            if (list.Contains(VP_DialogInitializer.INIT_ACTION.ON_CUSTOM_EVENT))
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_startListeningTime"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_stopListeningTime"), true);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_customEvent"), true);
            }


            serializedObject.ApplyModifiedProperties();
        }
    }

}
