using UnityEngine;

namespace VirtualPhenix.Localization
{
    /// <summary>
    /// Text item saved in a dictionary in TextManager. Has key and text for access every text in every language.
    /// </summary>
    [System.Serializable]
    public class VP_TextItem
    {
        /// <summary>
        /// Variables - Parsed Text
        /// </summary>
        [SerializeField] protected string m_text = "";
        /// <summary>
        /// Key in the dictionary for accesing the parsed text
        /// </summary>
        [SerializeField] protected string m_key = "";
        [SerializeField] protected string m_class = "";
        /// <summary>
        /// properties
        /// </summary>
        public string Text { get { return m_text; } }
        public string Key { get { return m_key; } }
        public string Class { get { return m_class; } }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="key"></param>
        /// <param name="text"></param>
        public VP_TextItem(string key, string text)
        {
            m_key = key;
            m_text = text;
            m_class = "Unknown";
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="key"></param>
        /// <param name="text"></param>
        public VP_TextItem(string key, string text, string _class)
        {
            m_key = key;
            m_text = text;
            m_class = _class;
        }
    }
}
