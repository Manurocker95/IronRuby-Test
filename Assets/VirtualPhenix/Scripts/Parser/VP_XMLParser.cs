using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;
using VirtualPhenix.Localization;
using System.Text;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace VirtualPhenix
{
    public static class VP_XMLParser
    {
#if UNITY_EDITOR
        public static void RemoveFromXML(TextAsset _xml, string _key)
        {
            if (_xml == null)
            {
                Debug.LogError("XML ERROR: NULL");
                return;
            }

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(_xml.text);

            string Tablename = "Table1";

            XmlNodeList globalTable = xmlDoc.GetElementsByTagName(Tablename);

            foreach (XmlNode TableNode in globalTable)
            {
                if (TableNode.ChildNodes[0].InnerText == _key)
                {
                    TableNode.ParentNode.RemoveChild(TableNode);
                    break;
                }
              
            }
        }
        public static void AddToXML(TextAsset _xml, string _key, string _text)
        {
            if (_xml == null)
            {
                Debug.LogError("XML ERROR: NULL");
                return;
            }

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(_xml.text);
            string Tablename = VP_LocalizationSetup.XMLParser.TABLE_NAME;
            XmlElement newSlot = xmlDoc.CreateElement(Tablename);
            XmlElement key = xmlDoc.CreateElement(VP_LocalizationSetup.XMLParser.NODE_KEY);
            XmlElement text = xmlDoc.CreateElement(VP_LocalizationSetup.XMLParser.NODE_TEXT);

            key.InnerText = _key;
            text.InnerText = _text;

            newSlot.AppendChild(key);
            newSlot.AppendChild(text);

            xmlDoc.DocumentElement.AppendChild(newSlot);

            string path = AssetDatabase.GetAssetPath(_xml);
            xmlDoc.Save(path);
        }

        public static void ReplaceInXML(TextAsset _xml, string _key, string _text)
        {
            if (_xml == null)
            {
                Debug.LogError("XML ERROR: NULL");
                return;
            }

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(_xml.text);

            string Tablename = VP_LocalizationSetup.XMLParser.TABLE_NAME;

            XmlNodeList globalTable = xmlDoc.GetElementsByTagName(Tablename);

            foreach (XmlNode TableNode in globalTable)
            {
                if (TableNode.ChildNodes[0].InnerText == _key)
                {
                    TableNode.ChildNodes[1].InnerText = _text;
                    break;
                }

            }
            string path = AssetDatabase.GetAssetPath(_xml);
            xmlDoc.Save(path);
        }
#endif

        public static VP_TextItemDictionary ParseTextItemXML(TextAsset _xml)
        {
            return new VP_TextItemDictionary(ParseXML(_xml));
        }
        public static Dictionary<string, VP_TextItem> ParseXML (TextAsset _xml)
        {
            if (_xml == null)
            {
                Debug.LogError("XML ERROR: NULL");
                return null;
            }

            Dictionary<string, VP_TextItem> tempDictionary = new Dictionary<string, VP_TextItem>();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(_xml.text);

            string Tablename = VP_LocalizationSetup.XMLParser.TABLE_NAME;

            XmlNodeList globalTable = xmlDoc.GetElementsByTagName(Tablename);

            foreach (XmlNode TableNode in globalTable)
            {
                string newItemContext = TableNode.ChildNodes[0].InnerText;
                if (newItemContext != "")
                {
                    if (tempDictionary.ContainsKey(newItemContext))
                    {
                        Debug.LogError("[XMLParse] Error! The context should always be unique (Script line " + TableNode.ChildNodes[0] + ")");
                        Debug.LogError("Name: " + newItemContext);
                        continue;
                    }
                    else
                    {
                        VP_TextItem newInfoText = null;
                        //(string _key, string _text, string _audioName, bool _hasOptions, bool _showOptions)  
                        newInfoText = new VP_TextItem(newItemContext, TableNode.ChildNodes[1].InnerText);
                        tempDictionary.Add(newItemContext, newInfoText);
                    }
                }
            }

            return tempDictionary;
        }
    }
}
