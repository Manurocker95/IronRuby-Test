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
    public static class VP_CSVParser
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
        public static void RemoveFromCSV(TextAsset _csv, string _key)
        {
            if (_csv == null)
            {
                Debug.LogError("CSV ERROR: NULL");
                return;
            }

            string path = AssetDatabase.GetAssetPath(_csv);
            string[] arrLine = File.ReadAllLines(path, Encoding.UTF8);
            string[] arrLineMinusOne = new string[arrLine.Length - 1];

            for (int i = 0; i < arrLine.Length; i++)
            {
                if (arrLine[i].Contains(_key + ","))
                    continue;

                arrLineMinusOne[i] = arrLine[i];
            }

            File.WriteAllLines(path, arrLineMinusOne, Encoding.UTF8);
        }
        public static void AddToCSV(TextAsset _csv, string _key, string _text)
        {
            if (_csv == null)
            {
                Debug.LogError("CSV ERROR: NULL");
                return;
            }

            if (_text.Contains(",") && !_text.Contains("\""))
            {
                _text = "\"" + _text + "\"";
            }

            string path = AssetDatabase.GetAssetPath(_csv);

            string[] arrLine = File.ReadAllLines(path, Encoding.UTF8);
            string[] newLineArr = new string[arrLine.Length + 1];

            for (int i = 0; i < arrLine.Length; i++)
            {
                newLineArr[i] = arrLine[i];
            }
            newLineArr[newLineArr.Length - 1] = _key + "," + _text;

            File.WriteAllLines(path, newLineArr, Encoding.UTF8);
        }

        public static void ReplaceTextInCSV(TextAsset _csv, string _keyToReplace = "", string _textToReplace = "")
        {
            if (_csv == null)
            {
                Debug.LogError("CSV ERROR: NULL");
                return;
            }
            string path = AssetDatabase.GetAssetPath(_csv);
            string[] arrLine = File.ReadAllLines(path, Encoding.UTF8);

            if (_textToReplace.Contains(",") && !_textToReplace.Contains("\""))
            {
                _textToReplace = "\"" + _textToReplace + "\"";
            }

            int line_to_edit = 0;
            foreach (string str in arrLine)
            {
                if (str.Contains(_keyToReplace+","))
                {
                    arrLine[line_to_edit] = _keyToReplace+","+_textToReplace;
                    break;
                }

                line_to_edit++;
            }

            File.WriteAllLines(path, arrLine, Encoding.UTF8);

            return;
        }
#endif
        public static Dictionary<string, VP_TextItem> ParseCSV(TextAsset _csv)
        {
            if (_csv == null)
            {
                Debug.LogError("CSV ERROR: NULL");
                return null;
            }

            string[,] grid = SplitCsvGrid(_csv.text);

            if (grid == null)
            {
                Debug.LogError("GRID ERROR: NULL");
                return null;
            }

            Dictionary<string, VP_TextItem> m_vTempDictionary = new Dictionary<string, VP_TextItem>();

            string key = "";
            string text = "";

            for (int i = 0; i < grid.GetLength(1); i++)
            {
                key = grid[0, i];
                text = grid[1, i];

                if (key == null || key == "" || text == null)
                    continue;

                m_vTempDictionary.Add(key, new VP_TextItem(key, text));
            }

            return m_vTempDictionary;
        }

        public static VP_TextItemDictionary ParseTextItemCSV(TextAsset _csv)
        {
            var temp = ParseCSV(_csv);
            Debug.Log("Temp Count" + temp.Count);
            VP_TextItemDictionary ret = new VP_TextItemDictionary();
            foreach (string item in temp.Keys)
            {
                ret.Add(item, temp[item]);
            }

            return ret;
        }

        static public string[,] SplitCsvGrid(string csvText)
        {
            string[] lines = csvText.Split("\n"[0]);

            // finds the max width of row
            int width = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                string[] row = SplitCsvLine(lines[i]);
                width = Mathf.Max(width, row.Length);
            }

            // creates new 2D string grid to output to
            string[,] outputGrid = new string[width + 1, lines.Length + 1];
            for (int y = 0; y < lines.Length; y++)
            {
                string[] row = SplitCsvLine(lines[y]);
                for (int x = 0; x < row.Length; x++)
                {
                    outputGrid[x, y] = row[x];

                    // This line was to replace "" with " in my output. 
                    // Include or edit it as you wish.
                    outputGrid[x, y] = outputGrid[x, y].Replace("\"\"", "\"");
                }
            }

            return outputGrid;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        static public string[] SplitCsvLine(string line)
        {
            return (from System.Text.RegularExpressions.Match m in System.Text.RegularExpressions.Regex.Matches(line,
            @"(((?<x>(?=[,\r\n]+))|""(?<x>([^""]|"""")+)""|(?<x>[^,\r\n]+)),?)",
            System.Text.RegularExpressions.RegexOptions.ExplicitCapture)
                    select m.Groups[1].Value).ToArray();
        }


        static public void DebugOutputGrid(string[,] grid)
        {
            string textOutput = "";
            for (int y = 0; y < grid.GetUpperBound(1); y++)
            {
                for (int x = 0; x < grid.GetUpperBound(0); x++)
                {

                    textOutput = grid[x, y];
                    //Debug.Log(textOutput);
                }
                textOutput += "\n";
            }
          
        }

        static public void DebugOutputGrid2(string[,] grid)
        {
            string textOutput = "";
            for (int y = 0; y < grid.GetUpperBound(1); y++)
            {
                for (int x = 0; x < grid.GetUpperBound(0); x++)
                {

                    textOutput += grid[x, y];
                    textOutput += "|";
                }
                textOutput += "\n";
            }
            Debug.Log(textOutput);
        }
    }
}
