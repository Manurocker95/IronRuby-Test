using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;

namespace VirtualPhenix
{

    public class VP_WelcomeWindow : VP_EditorWindow<VP_WelcomeWindow>
    {

        protected static readonly int WelcomeWindowWidth = 488;
        protected static readonly int WelcomeWindowHeight = 720;

        protected Vector2 m_scroll;

        [MenuItem("Virtual Phenix/Window/Welcome Window %F0", false, 0)]
        public static void ShowWindow()
        {
            EditorWindow editorWindow = GetWindow(typeof(VP_WelcomeWindow), false, " Welcome", true);
            editorWindow.autoRepaintOnSceneChange = true;
            editorWindow.titleContent.image = EditorGUIUtility.IconContent("_Help").image;
            editorWindow.maxSize = new Vector2(WelcomeWindowWidth, WelcomeWindowHeight);
            editorWindow.minSize = new Vector2(WelcomeWindowWidth, WelcomeWindowHeight);
            editorWindow.position = new Rect(Screen.width / 2 + WelcomeWindowWidth / 2, Screen.height / 2, WelcomeWindowWidth, WelcomeWindowHeight);
            editorWindow.Show();
        }

        protected override void OnGUI()
        {
            if (EditorApplication.isCompiling)
            {
                this.ShowNotification(new GUIContent("Compiling Scripts", EditorGUIUtility.IconContent("BuildSettings.Editor").image));
            }
            else
            {
                this.RemoveNotification();
            }
            Texture2D welcomeImage = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/VirtualPhenix/Graphics/Logo/welcome-banner-v2.png", typeof(Texture2D));
            Rect welcomeImageRect = new Rect(0, 0, 488, 325);
            if (welcomeImage != null)
                UnityEngine.GUI.DrawTexture(welcomeImageRect, welcomeImage);
            GUILayout.Space(345);

            GUILayout.BeginArea(new Rect(EditorGUILayout.GetControlRect().x + 10, 345, WelcomeWindowWidth - 20, WelcomeWindowHeight));
            EditorGUILayout.LabelField("Welcome to Virtual Phenix Framework, alias Virtual Phenix Tools!\n", VP_EditorWindowStyleSetup.LargeTextStyle);

            EditorGUILayout.LabelField("Version: " + VP_PhoenixToolsSetup.CURRENT_VERSION, VP_EditorWindowStyleSetup.RegularTextStyle);
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("GETTING STARTED", VP_EditorWindowStyleSetup.LargeTextStyle);
            EditorGUILayout.Space();
            m_scroll = EditorGUILayout.BeginScrollView(m_scroll, GUILayout.Width(WelcomeWindowWidth - 20), GUILayout.Height(WelcomeWindowHeight - 80));
            EditorGUILayout.LabelField("* Virtual Phenix Framework uses a series of Define Symbols you need to select based on the packages you import in the project.", VP_EditorWindowStyleSetup.RegularTextStyle);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("* To enable Define Symbols, open the define symbol window, select the main configuration Scriptable Object and toggle the needed ones.", VP_EditorWindowStyleSetup.RegularTextStyle);
            EditorGUILayout.LabelField("Once you are done, select on \"Setup\". If you need to delete all previous define symbols, click on \"Remove and Add\"", VP_EditorWindowStyleSetup.RegularTextStyle);
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("* Virtual Phenix Windows can be opened with the toolbar in: VirtualPhenix/Window/", VP_EditorWindowStyleSetup.RegularTextStyle);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("* VP Editor (Left Ctrl + F1) has a series of tools grouped in the same window. It's recommended to have it opened in your layout.", VP_EditorWindowStyleSetup.RegularTextStyle);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("* If you need more help, select \"How to use\" Button in every window for custom info display.", VP_EditorWindowStyleSetup.RegularTextStyle);
            EditorGUILayout.EndScrollView();

            GUILayout.EndArea();

            Rect areaRect = new Rect(0, WelcomeWindowHeight - 20, WelcomeWindowWidth, WelcomeWindowHeight - 20);
            GUILayout.BeginArea(areaRect);
            EditorGUILayout.LabelField(VP_PhoenixToolsSetup.COPYRIGHT, VP_EditorWindowStyleSetup.FooterTextStyle);
            GUILayout.EndArea();

        }

        private void OnInspectorUpdate()
        {
            Repaint();
        }
    }
}