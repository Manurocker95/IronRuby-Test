using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Text.RegularExpressions;

namespace VirtualPhenix
{

    public class VP_ConsoleWindow : VP_EditorWindow<VP_ConsoleWindow>
    {
        private Rect m_upperPanel;
        private Rect m_lowerPanel;
        private Rect m_resizer;
        private Rect m_menuBar;

        private float m_sizeRatio = 0.5f;
        private bool m_isResizing;

        private float m_resizerHeight = 5f;
        private float m_menuBarHeight = 20f;

        private bool m_collapse = false;
        private bool m_clearOnPlay = false;
        private bool m_errorPause = false;
        private bool m_showCustom = false;
        private bool m_showLog = false;
        private bool m_showWarnings = false;
        private bool m_showErrors = false;
        private bool m_customBars = false;

        private Vector2 m_upperPanelScroll;
        private Vector2 m_lowerPanelScroll;

        private GUIStyle m_resizerStyle;
        private GUIStyle m_boxStyle;
        private GUIStyle m_textAreaStyle;

        private Texture2D m_boxBgOdd;
        private Texture2D m_boxBgEven;
        private Texture2D m_boxBgSelected;
        private Texture2D m_boxBgLog;
        private Texture2D m_boxBgWarning;
        private Texture2D m_boxBgError;
        private Texture2D m_boxBgCustom;
        private Texture2D m_icon;
        private Texture2D m_errorIcon;
        private Texture2D m_errorIconSmall;
        private Texture2D m_warningIcon;
        private Texture2D m_warningIconSmall;
        private Texture2D m_infoIcon;
        private Texture2D m_infoIconSmall;
        private Texture2D m_customInfoSmall;
        private Texture2D m_customIcon;

        private List<Log> m_logs;
        private Log m_selectedLog;

        private int m_customCount = 0;
        private int m_logCount = 0;
        private int m_warningCount = 0;
        private int m_errorCount = 0;

        public override string WindowName => "VP Console";

        [MenuItem("Virtual Phenix/Window/VP Console %F6")]
        private static void OpenWindow()
        {
            m_instance = GetWindow<VP_ConsoleWindow>();
            m_instance.titleContent = new GUIContent("Virtual Phenix Console");
        }

        private void OnEnable()
        {
            m_errorCount = m_warningCount = m_logCount = m_customCount = 0;

            m_customIcon = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/VirtualPhenix/Graphics/Icons/Icons/customIcon.png", typeof(Texture2D)); //EditorGUIUtility.Load("icons/VirtualPhenix/customIcon.png") as Texture2D;
            m_customInfoSmall = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/VirtualPhenix/Graphics/Icons/Icons/customIconSmall.png", typeof(Texture2D)); //EditorGUIUtility.Load("icons/VirtualPhenix/customIcon.png") as Texture2D;

            m_errorIcon = EditorGUIUtility.Load("icons/console.erroricon.png") as Texture2D;
            m_warningIcon = EditorGUIUtility.Load("icons/console.warnicon.png") as Texture2D;
            m_infoIcon = EditorGUIUtility.Load("icons/console.infoicon.png") as Texture2D;

            m_errorIconSmall = EditorGUIUtility.Load("icons/console.erroricon.sml.png") as Texture2D;
            m_warningIconSmall = EditorGUIUtility.Load("icons/console.warnicon.sml.png") as Texture2D;
            m_infoIconSmall = EditorGUIUtility.Load("icons/console.infoicon.sml.png") as Texture2D;

            m_resizerStyle = new GUIStyle();
            m_resizerStyle.normal.background = EditorGUIUtility.Load("icons/d_AvatarBlendBackground.png") as Texture2D;

            m_boxStyle = new GUIStyle();
            m_boxStyle.normal.textColor = new Color(0.7f, 0.7f, 0.7f);

            m_boxBgOdd = EditorGUIUtility.Load("builtin skins/darkskin/images/cn entrybackodd.png") as Texture2D;
            m_boxBgEven = EditorGUIUtility.Load("builtin skins/darkskin/images/cnentrybackeven.png") as Texture2D;
            m_boxBgSelected = EditorGUIUtility.Load("builtin skins/darkskin/images/menuitemhover.png") as Texture2D;

            m_boxBgLog = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/VirtualPhenix/Graphics/Icons/Toolbar/logBar.png", typeof(Texture2D));
            m_boxBgWarning = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/VirtualPhenix/Graphics/Icons/Toolbar/warningBar.png", typeof(Texture2D));
            m_boxBgError = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/VirtualPhenix/Graphics/Icons/Toolbar/errorBar.png", typeof(Texture2D));
            m_boxBgCustom = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/VirtualPhenix/Graphics/Icons/Toolbar/customBar.png", typeof(Texture2D));
            
            m_textAreaStyle = new GUIStyle();
            m_textAreaStyle.normal.textColor = new Color(0.9f, 0.9f, 0.9f);
            m_textAreaStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/projectbrowsericonareabg.png") as Texture2D;

            m_logs = new List<Log>();
            m_selectedLog = null;

            Application.logMessageReceived += LogMessageReceived;
        }

        private void OnDisable()
        {
            Application.logMessageReceived -= LogMessageReceived;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Application.logMessageReceived -= LogMessageReceived;
        }

        public override void CreateHowToText()
        {
            if (m_howToTexts == null)
                InitHowToTextList();

            CreateHowToTitle(ref m_howToTexts, "VIRTUAL PHENIX CONSOLE");
            
            m_howToTexts.Add(new VP_HTUText()
            {
                Text = "This window is a clone of Unity's console but with more features.\n" +
                "VP Console catches regular Debug.Logs but if you want to personalize your debug logs\n"+
                "You better use VP_Debug.Log which has these parameters:\n\n"+
                "- Text: Text you want to display\n\n"+
                "- Color: You can set a color directly, no need to use RichText\n\n" +
                "- Custom Tag: you can filter with Tags. By default: [Virtual Phenix]:\n\n" +
                "- Use small icon in the log: If you need to have a more personalized log",
              
                m_labelType = VP_EditorWindowStyleSetup.RegularTextStyle,
                m_spaces = new KeyValuePair<bool, int>(true, 2)
            });

            VP_HowToUseWindow.ShowWindow(HowToWindowWidth, HowToWindowHeight, m_howToTexts.ToArray());
        }

        protected override void OnGUI()
        {
            base.OnGUI();
            EditorGUILayout.Space();

            DrawMenuBar();
            DrawUpperPanel();
            DrawLowerPanel();
            DrawResizer();
            ProcessEvents(Event.current);
            Repaint();
        }

        private void DrawMenuBar()
        {
            m_menuBar = new Rect(0, 0, position.width, m_menuBarHeight);

            GUILayout.BeginArea(m_menuBar, EditorStyles.toolbar);
            GUILayout.BeginHorizontal();

            if (GUILayout.Button(new GUIContent("Clear"), EditorStyles.toolbarButton, GUILayout.Width(80)))
            {
                ClearLogs();
            }
            GUILayout.Space(5);

            m_collapse = GUILayout.Toggle(m_collapse, new GUIContent("Collapse"), EditorStyles.toolbarButton, GUILayout.Width(80));
            m_clearOnPlay = GUILayout.Toggle(m_clearOnPlay, new GUIContent("Clear On Play"), EditorStyles.toolbarButton, GUILayout.Width(80));
            m_errorPause = GUILayout.Toggle(m_errorPause, new GUIContent("Error Pause"), EditorStyles.toolbarButton, GUILayout.Width(80));
            m_customBars = GUILayout.Toggle(m_customBars, new GUIContent("Custom Bars"), EditorStyles.toolbarButton, GUILayout.Width(100));

            if (GUILayout.Button(new GUIContent("Editor"), EditorStyles.toolbarButton, GUILayout.Width(80)))
            {
                EditorApplication.ExecuteMenuItem("Virtual Phenix/Editor");
            }
            GUILayout.FlexibleSpace();

            m_showCustom = GUILayout.Toggle(m_showCustom, new GUIContent("Custom ("+m_customCount+")", m_customInfoSmall), EditorStyles.toolbarButton, GUILayout.Width(110));
            m_showLog = GUILayout.Toggle(m_showLog, new GUIContent("Logs ("+m_logCount+")", m_infoIconSmall), EditorStyles.toolbarButton, GUILayout.Width(100));
            m_showWarnings = GUILayout.Toggle(m_showWarnings, new GUIContent("Warnings (" + m_warningCount + ")", m_warningIconSmall), EditorStyles.toolbarButton, GUILayout.Width(100));
            m_showErrors = GUILayout.Toggle(m_showErrors, new GUIContent("Errors(" + m_errorCount + ")", m_errorIconSmall), EditorStyles.toolbarButton, GUILayout.Width(100));

            ShowLogsByFilter(m_showLog, m_showWarnings, m_showErrors);
            Collapse(!m_collapse);

            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        private void ClearLogs()
        {
            var logEntries = Type.GetType("UnityEditor.LogEntries, UnityEditor.dll");

            var clearMethod = logEntries.GetMethod("Clear", BindingFlags.Static | BindingFlags.Public);

            clearMethod.Invoke(null, null);
            m_logs.Clear();
            m_selectedLog = null;

            m_errorCount = m_warningCount = m_logCount = m_customCount = 0;
     
            Repaint();
        }

        

        private void DrawUpperPanel()
        {
            m_upperPanel = new Rect(0, m_menuBarHeight, position.width, (position.height * m_sizeRatio) - m_menuBarHeight);

            GUILayout.BeginArea(m_upperPanel);
            m_upperPanelScroll = GUILayout.BeginScrollView(m_upperPanelScroll);

            for (int i = 0; i < m_logs.Count; i++)
            {
                if (m_logs[i].m_shown)
                {
                    if (DrawBox(m_logs[i].m_info, m_logs[i].m_type, i % 2 == 0, m_logs[i].m_isSelected, m_logs[i].m_collapsedNum, m_logs[i].m_useCustomIcon))
                    {
                        if (m_selectedLog != null)
                        {
                            m_selectedLog.m_isSelected = false;
                        }

                        m_logs[i].m_isSelected = true;
                        m_selectedLog = m_logs[i];
                        Repaint();
                    }
                }
            }

            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        private void DrawLowerPanel()
        {
            m_lowerPanel = new Rect(0, (position.height * m_sizeRatio) + m_resizerHeight, position.width, (position.height * (1 - m_sizeRatio)) - m_resizerHeight);

            GUILayout.BeginArea(m_lowerPanel);
            m_lowerPanelScroll = GUILayout.BeginScrollView(m_lowerPanelScroll);

            if (m_selectedLog != null)
            {
                GUILayout.TextArea(m_selectedLog.m_message, m_textAreaStyle);
            }

            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        private void DrawResizer()
        {
            m_resizer = new Rect(0, (position.height * m_sizeRatio) - m_resizerHeight, position.width, m_resizerHeight * 2);

            GUILayout.BeginArea(new Rect(m_resizer.position + (Vector2.up * m_resizerHeight), new Vector2(position.width, 2)), m_resizerStyle);
            GUILayout.EndArea();

            EditorGUIUtility.AddCursorRect(m_resizer, MouseCursor.ResizeVertical);
        }

        private bool DrawBox(string content, LogType boxType, bool isOdd, bool isSelected, int _collapseNum, bool _customIcon)
        {
            m_boxStyle.normal.textColor = Color.white;

            if (isSelected)
            {
                m_boxStyle.normal.background = m_boxBgSelected;
            }
            else
            {
                if (!m_customBars)
                {
                    if (isOdd)
                    {
                        m_boxStyle.normal.background = m_boxBgOdd;
                    }
                    else
                    {
                        m_boxStyle.normal.background = m_boxBgEven;
                    }

                    switch (boxType)
                    {
                        case LogType.Error: m_icon = m_errorIcon;  break;
                        case LogType.Exception: m_icon = m_errorIcon; break;
                        case LogType.Assert: m_icon = m_errorIcon; break;
                        case LogType.Warning: m_icon = m_warningIcon; break;
                        case LogType.Log: m_icon = m_infoIcon; break;
                    }
                }
                else
                {
                    switch (boxType)
                    {
                        case LogType.Error: m_icon = m_errorIcon; m_boxStyle.normal.background = m_boxBgError; m_boxStyle.normal.textColor = Color.white; break;
                        case LogType.Exception: m_icon = m_errorIcon; break;
                        case LogType.Assert: m_icon = m_errorIcon; break;
                        case LogType.Warning: m_icon = m_warningIcon; m_boxStyle.normal.background = m_boxBgWarning; m_boxStyle.normal.textColor = Color.black; break;
                        case LogType.Log: m_icon = m_infoIcon; m_boxStyle.normal.background = m_boxBgLog; m_boxStyle.normal.textColor = Color.black; break;
                    }

                    if (_customIcon)
                    {
                        m_icon = m_customIcon;
                        m_boxStyle.normal.background = m_boxBgCustom;
                        m_boxStyle.normal.textColor = Color.white;
                    }
                }
            }

            string _num = m_collapse && _collapseNum > 1 ? "(" + _collapseNum + ")    " : "";

            return GUILayout.Button(new GUIContent(_num + content, m_icon), m_boxStyle, GUILayout.ExpandWidth(true), GUILayout.Height(30));
        }

        private void ProcessEvents(Event e)
        {
            switch (e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 0 && m_resizer.Contains(e.mousePosition))
                    {
                        m_isResizing = true;
                    }
                    break;

                case EventType.MouseUp:
                    m_isResizing = false;
                    break;
            }

            Resize(e);
        }

        private void Resize(Event e)
        {
            if (m_isResizing && position.height > 100)
            {
                m_sizeRatio = e.mousePosition.y / position.height;
                //VP_Debug.Log("SIZE RATIO:" + sizeRatio + " MOUSE POS: " + e.mousePosition.y + " POSITION HEIGHT: " + position.height);

                Repaint();
            }
        }

        private void CheckErrorPause(Log _log)
        {
            if (m_errorPause && _log.m_type == LogType.Error)
                UnityEngine.Debug.Break();
        }

        private void CheckErrorPauseInAll(bool _pause)
        {
            if (!_pause)
                return;

            bool needToBreak = false;
            if (Application.isPlaying && !needToBreak)
            {
                foreach (Log log in m_logs)
                {            
                    if (log.m_shown && log.m_type == LogType.Error)
                    {
                        needToBreak = true;
                        break;
                    }
                }

                if (needToBreak)
                    UnityEngine.Debug.Break();
            }
            
        }

        private void ShowLogsByFilter(bool _log, bool _warning, bool _error)
        {
            foreach (Log log in m_logs)
            {
                ShowLogByFilter(log, m_showLog, m_showWarnings, m_showErrors);
            }
        }


        private void ShowLogByFilter(Log log, bool _log, bool _warning, bool _error)
        {
            log.m_shown = ((log.m_useCustomIcon && m_showCustom) || !log.m_useCustomIcon) && ((log.m_type == LogType.Error && m_showErrors) || (log.m_type == LogType.Warning && m_showWarnings) || (log.m_type == LogType.Log && m_showLog) || log.m_type == LogType.Assert || log.m_type == LogType.Exception);
        }

        private void Collapse(bool _showAll)
        {
            if (_showAll)
            {
                foreach (Log log in m_logs)
                {
                    ShowLogByFilter(log, m_showLog, m_showWarnings, m_showErrors);
                    log.m_collapsedNum = 0;
                }
            }
            else
            {
                List<Log> m_collapse = new List<Log>();
                foreach (Log log in m_logs)
                {
                    log.m_collapsedNum = 0;

                    bool col = false;
                    foreach (Log logToCompare in m_collapse)
                    {
                        if (log != logToCompare && log.m_info == logToCompare.m_info && log.m_type == logToCompare.m_type)
                        {
                            col = true;
                            log.m_shown = false;
                            logToCompare.m_collapsedNum++;
                            break;
                        }
                    }

                    if (!col)
                    {
                        log.m_shown = (log.m_type == LogType.Error && m_showErrors || log.m_type == LogType.Warning && m_showWarnings || log.m_type == LogType.Log && m_showLog);
                        if (!m_collapse.Contains(log))
                        {
                            log.m_collapsedNum++;
                            m_collapse.Add(log);
                        }                            
                    }

                }
            }
        }

        private void LogMessageReceived(string condition, string stackTrace, LogType type)
        {
            bool _show = (type == LogType.Error && m_showErrors || type == LogType.Warning && m_showWarnings || type == LogType.Log && m_showLog);

            bool hasCustom = (condition.Contains("[CustomIcon]"));

            if (hasCustom)
            {
                condition = condition.Replace("[CustomIcon]", "");
                m_customCount++;
            }
            else
            {
                if (type == LogType.Error)
                    m_errorCount++;
                else if (type == LogType.Warning)
                    m_warningCount++;
                else if (type == LogType.Log)
                    m_logCount++;
            }

            Log l = new Log(false, condition, stackTrace, type, _show, hasCustom);
            m_logs.Add(l);

            CheckErrorPause(l);
            ShowLogsByFilter(m_showLog, m_showWarnings, m_showErrors);
            Repaint();
        }
    }

    public class Log
    {
        /// <summary>
        /// If it is shown in the console
        /// </summary>
        public bool m_shown;
        /// <summary>
        /// If it is selected
        /// </summary>
        public bool m_isSelected;
        /// <summary>
        /// The message shown in the console (the log)
        /// </summary>
        public string m_info;
        /// <summary>
        /// The event that displayed this log
        /// </summary>
        public string m_message;
        /// <summary>
        /// Type of log
        /// </summary>
        public LogType m_type;
        public int m_collapsedNum;
        public bool m_useCustomIcon; 

        public Log(bool _isSelected, string _info, string _message, LogType _type, bool _shown, bool _customIcon)
        {
            this.m_collapsedNum = 0;
            this.m_shown = _shown;
            this.m_isSelected = _isSelected;
            this.m_info = _info;
            this.m_message = _message;
            this.m_type = _type;
            this.m_useCustomIcon = _customIcon;
        }
    }
}