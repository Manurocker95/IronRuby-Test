using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.LogCatcher
{
    [System.Serializable]
    public class VP_RuntimeLog
    {
        public LogType m_type;
        public string m_logString;
        public string m_stackTrace;
    }

    public class VP_RuntimeLogCatcher : VP_MonoBehaviour
    {
        public enum PathType
        {
            PersistentDatapath,
            DataPath,
            Custom
        }

        [SerializeField] protected List<VP_RuntimeLog> m_logs = new List<VP_RuntimeLog>();
        [SerializeField] protected PathType m_pathType;
        [SerializeField] protected string m_savePath;
        [SerializeField] protected string m_saveFullPath;
        [SerializeField] protected bool m_saveOnDestroy = true;

        protected override void Awake()
        {
            base.Awake();

#if UNITY_ANDROID
            m_pathType = PathType.PersistentDatapath;
#endif

            string path = "";

            switch (m_pathType)
            {
                case PathType.PersistentDatapath:
                    path = Application.persistentDataPath;
                    break;
                case PathType.DataPath:
                    path = Application.dataPath;
                    break;
                case PathType.Custom:
                    path = "";
                    break;
            }

            m_savePath = path + " /Logs/";
            m_saveFullPath = m_savePath+"logs.vplogs";

        }

        protected override void StartAllListeners()
        {
            base.StartAllListeners();
            Application.logMessageReceived += LogReceived;
        }

        protected override void StopAllListeners()
        {
            base.StopAllListeners();
            Application.logMessageReceived -= LogReceived;
        }

        protected virtual void LogReceived(string logString, string stackTrace, LogType type)
        {
            m_logs.Add(new VP_RuntimeLog() { m_type = type, m_logString = logString, m_stackTrace = stackTrace });
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (m_saveOnDestroy)
                SaveLogs();
        }

        public virtual void SaveLogs()
        {
            if (m_savePath.IsNotNullNorEmpty())
            {
                
                List<string> lines = new List<string>();

                lines.Add("------------------------------------");
                lines.Add("Logs " + System.DateTime.Now.ToShortDateString());
                lines.Add("------------------------------------");

                foreach (VP_RuntimeLog log in m_logs)
                {
                    if (log == null)
                        continue;

                    lines.Add("------------------------------------");
                    lines.Add("Type: " + log.m_type);
                    lines.Add(" ");
                    lines.Add("Log Message: " + log.m_logString);
                    lines.Add(" ");
                    lines.Add("Stack Trace: " + log.m_stackTrace);
                    lines.Add("------------------------------------");
                    lines.Add(" ");
                }

                if (!System.IO.Directory.Exists(m_savePath))
                {
                    System.IO.Directory.CreateDirectory(m_savePath);
                }

                if (!System.IO.File.Exists(m_saveFullPath))
                    System.IO.File.WriteAllLines(m_saveFullPath, lines.ToArray());
                else
                    System.IO.File.AppendAllLines(m_saveFullPath, lines.ToArray());
            }
        }
    }
}
