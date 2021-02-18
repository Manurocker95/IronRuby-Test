using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace VirtualPhenix.InApp
{
    [System.Serializable]
    public class VP_InAppPack<T>
    {
        /// <summary>
        /// Android/IOS developer console ID
        /// </summary>
        public string m_id;
        /// <summary>
        /// Translated product Text
        /// </summary>
        public string m_productName;
        /// <summary>
        /// Translated Price Text
        /// </summary>
        public string m_priceString;
        /// <summary>
        /// Value received after this pack is purchased
        /// </summary>
        public T m_value;
    }
}
