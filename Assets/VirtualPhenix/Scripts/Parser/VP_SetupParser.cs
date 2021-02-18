// http://wiki.unity3d.com/index.php/CSVReader
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System.Linq;
using VirtualPhenix.Localization;
using System.IO;
using System.Text;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace VirtualPhenix
{
    /// <summary>
    /// CSV Parser
    /// </summary>
    public static class VP_SetupParser
    {
        /// <summary>
        /// If the phrase is valid or not
        /// </summary>
        /// <param name="_phrase"></param>
        /// <param name="_separator"></param>
        /// <returns></returns>
        private static bool IsValidLine(string _phrase, char _separator)
        {
            return !string.IsNullOrEmpty(_phrase) && _phrase.IndexOf(_separator) != -1;
        }

#if UNITY_EDITOR
        public static void RemoveFromSetup(string _scriptFile, string className, string varToAdd)
        {
            string path = _scriptFile;
            bool found = false;
            if (File.Exists(path))
            {
                Debug.Log("audio setup script exists");

                string[] arrLine = File.ReadAllLines(path, Encoding.UTF8);
                List<string> newArr = new List<string>();

                int line_to_edit = 0;
                foreach (string str in arrLine)
                {
                    if (str.Contains(varToAdd + " ="))
                    {
                        found = true;
                        break;
                    }

                    line_to_edit++;
                }

                if (found)
                {
                    int counter = 0;
                    foreach (string str in arrLine)
                    {
                        if (counter != line_to_edit)
                            newArr.Add(str);

                        counter++;
                    }

                    Debug.Log("Key Removed successfully");

                    File.WriteAllLines(path, newArr.ToArray(), Encoding.UTF8);
                    AssetDatabase.Refresh();
                }

            }
            else
            {
                Debug.LogError("FILE DOESNT EXIST: " + path);
            }
        }

        public static bool IsKeyInSetup(string _script, string _key, string[] arrLine)
        {
            string path = _script;
    
            List<string> KeyList = new List<string>();
            foreach (string str in arrLine)
            {
                if (str.Contains("= " + _key))
                {

                    int pFrom = str.IndexOf("public const string ") + 20;
                    int pTo = str.LastIndexOf("=");

                    string result = str.Substring(pFrom, pTo - pFrom);

                    pFrom = str.IndexOf("= ");
                    pTo = str.LastIndexOf(";");
                    result = str.Substring(pFrom, pTo - pFrom);
                    KeyList.Add(result);
                    break;
                }
            }

            return KeyList.Contains(_key);
        }

        public static bool AddToSetup(string _scriptFile, string className, string varToAdd, string keyToAdd)
        {
            string path = _scriptFile;

            if (!File.Exists(path))
            {
                Debug.LogError("The file doesn't exist");
                return false;
            }

            string[] arrLine = File.ReadAllLines(path, Encoding.UTF8);

            if (IsKeyInSetup(path, keyToAdd, arrLine))
            {
                return false;
            }

            string[] arrLineNew = new string[arrLine.Length + 1];
            int counter = 0;
            int line_to_edit = 0;

            if (string.IsNullOrEmpty(className))
                className = "General";

            bool found = false;
            Debug.Log("Looking for class " + className);
            foreach (string str in arrLine)
            {
                if (str.Contains("class " + className))
                {
                    Debug.Log("Found class " + className);
                    line_to_edit = counter + 2;
                    found = true;
                    break;
                }

                counter++;

            }

            if (found)
            {
                bool IsAlreadyThere = false;

                foreach (string str in arrLine)
                {
                    if (str.Contains(varToAdd) && str.Contains(keyToAdd))
                    {
                        Debug.Log(keyToAdd + " is already in setup with path "+path);
                        IsAlreadyThere = true;
                        break;
                    }
                }

                if (IsAlreadyThere)
                    return true;

                for (int i = 0; i < line_to_edit; i++)
                {
                    arrLineNew[i] = arrLine[i];
                }

                string varName = varToAdd;
                if (string.IsNullOrEmpty(varToAdd))
                {
                    varName = keyToAdd.ToUpper();
                }
                
                arrLineNew[line_to_edit] = "     public const string " + varToAdd + " =" + "\"" + keyToAdd + "\"" + ";";
                Debug.Log("Audio with key " + keyToAdd + " added to script with path"+path);
                for (int j = line_to_edit; j < arrLine.Length; j++)
                {
                    arrLineNew[j + 1] = arrLine[j];
                }

                Debug.Log("Added to AudioSetup.cs");
                File.WriteAllLines(path, arrLineNew, Encoding.UTF8);
                AssetDatabase.Refresh();
            }
            else
            {
                arrLineNew = File.ReadAllLines(path, Encoding.UTF8);

                string[] newArr = new string[arrLineNew.Length + 4];
                int cc = 0;
                foreach (string str in arrLineNew)
                {
                    newArr[cc] = str;
                    cc++;
                }

                className = "General";
                string[] classArr = new string[6];
                classArr[0] = "        public static class " + className;
                classArr[1] = "        {";
                classArr[2] = "            public const string " + varToAdd + " = " + "\"" + keyToAdd + "\";";
                classArr[3] = "        }";
                classArr[4] = "    }";
                classArr[5] = "}";
                int _c = 0;
                newArr[newArr.Length - 7] = "";
                for (int i = newArr.Length - 6; i < newArr.Length; i++)
                {
                    newArr[i] = classArr[_c];
                    _c++;
                }

                Debug.Log("Created class " + className);

                File.WriteAllLines(path, newArr, Encoding.UTF8);
                AssetDatabase.Refresh();
            }

            return true;
        }

        public static void ReplaceTextInSetup(string _scriptFile, string className, string varToAdd, string keyToAdd)
        {
            string path = _scriptFile;
            bool found = false;
            if (File.Exists(path))
            {
                Debug.Log("script exists");

                string[] arrLine = File.ReadAllLines(path, Encoding.UTF8);

                int line_to_edit = 0;
                foreach (string str in arrLine)
                {
                    if (str.Contains(varToAdd + " ="))
                    {
                        found = true;
                        arrLine[line_to_edit] = "           public const string " + varToAdd + " =" + "\"" + keyToAdd + "\"" + ";";
                        Debug.Log("Audio with key " + keyToAdd + " added to VP_audioSetup.cs");
                        break;
                    }

                    line_to_edit++;
                }

                if (found)
                {
                    File.WriteAllLines(path, arrLine, Encoding.UTF8);
                    AssetDatabase.Refresh();
                }

            }
            else
            {
                Debug.LogError("FILE DOESNT EXIST: " + path);
            }
        }

        public static Dictionary<string, string> ParseSetup(string script)
        {
            if (script == null)
            {
                Debug.LogError("CSV ERROR: NULL");
                return null;
            }

            string path = script;
     
            if (!File.Exists(path))
            {
                return null;
            }

            Dictionary<string, string> m_vTempDictionary = new Dictionary<string, string>();
            string[] arrLine = File.ReadAllLines(path, Encoding.UTF8);
            string varName = "";
            string keyValue = "";
            foreach (string str in arrLine)
            {
                if (str.Contains("public const string"))
                {
                    int pFrom = str.IndexOf("public const string ") + 20;
                    int pTo = str.LastIndexOf("=");
                    string result = str.Substring(pFrom, pTo - pFrom);
                    varName = result;

                    pFrom = str.IndexOf("=") + 1;
                    pTo = str.LastIndexOf(";");
                    result = str.Substring(pFrom, pTo - pFrom);
 
                    result = result.Replace('"', ' ');
                    result = System.Text.RegularExpressions.Regex.Replace(result, " ", "");
                    keyValue = result;

                    m_vTempDictionary.Add(varName, keyValue);
                }
            }

            return m_vTempDictionary;
        }

        public static VirtualPhenix.Dialog.VP_DialogSetupData LoadDataFromKey(string script, string _key)
        {
            if (script == null)
            {
                Debug.LogError("CSV ERROR: NULL");
                return null;
            }

            string path = script;

            if (!File.Exists(path))
            {
                return null;
            }

            VirtualPhenix.Dialog.VP_DialogSetupData data = new VirtualPhenix.Dialog.VP_DialogSetupData();
            string[] arrLine = File.ReadAllLines(path, Encoding.UTF8);
            string varName = "";
            string keyValue = "";
            string className = "";
            foreach (string str in arrLine)
            {
                if (str.Contains("class"))
                {
                    int pFrom = str.IndexOf("class ") + 6;
                    int pTo = str.LastIndexOf("") + 1;
                    string result = str.Substring(pFrom, pTo - pFrom);
                    className = result;
                }

                if (str.Contains("public const string"))
                {
                    int pFrom = str.IndexOf("public const string ") + 20;
                    int pTo = str.LastIndexOf("=");
                    string result = str.Substring(pFrom, pTo - pFrom);
                    varName = result;

                    pFrom = str.IndexOf("=") + 1;
                    pTo = str.LastIndexOf(";");
                    result = str.Substring(pFrom, pTo - pFrom);

                    result = result.Replace('"', ' ');
                    result = System.Text.RegularExpressions.Regex.Replace(result, " ", "");
                    keyValue = result;

                    if (keyValue == _key)
                    {
                        data.SetData(className, varName, keyValue);
                    }
                }
            }

            return data;
        }

        public static List<VirtualPhenix.Dialog.VP_DialogSetupData> ParseSetupInList(string script)
        {
            if (script == null)
            {
                Debug.LogError("CSV ERROR: NULL");
                return null;
            }

            string path = script;

            if (!File.Exists(path))
            {
                return null;
            }

            List <VirtualPhenix.Dialog.VP_DialogSetupData> m_vTempDictionary = new List<VirtualPhenix.Dialog.VP_DialogSetupData>();
            string[] arrLine = File.ReadAllLines(path, Encoding.UTF8);
            string varName = "";
            string keyValue = "";
            string className = "";
            foreach (string str in arrLine)
            {
                if (str.Contains("class"))
                {
                    int pFrom = str.IndexOf("class ") + 6;
                    int pTo = str.LastIndexOf("") + 1;
                    string result = str.Substring(pFrom, pTo - pFrom);
                    className = result;
                }

                if (str.Contains("public const string"))
                {
                    int pFrom = str.IndexOf("public const string ") + 20;
                    int pTo = str.LastIndexOf("=");
                    string result = str.Substring(pFrom, pTo - pFrom);
                    varName = result;

                    pFrom = str.IndexOf("=") + 1;
                    pTo = str.LastIndexOf(";");
                    result = str.Substring(pFrom, pTo - pFrom);

                    result = result.Replace('"', ' ');
                    result = System.Text.RegularExpressions.Regex.Replace(result, " ", "");
                    keyValue = result;

                    VirtualPhenix.Dialog.VP_DialogSetupData data = new VirtualPhenix.Dialog.VP_DialogSetupData(className, varName, keyValue);
                    m_vTempDictionary.Add(data);
                }
            }

            return m_vTempDictionary;
        }
#endif

    }
}
