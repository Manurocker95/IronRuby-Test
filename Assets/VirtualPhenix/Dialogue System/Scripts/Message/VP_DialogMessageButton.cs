using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace VirtualPhenix.Dialog
{
    [System.Serializable]
    public class VP_DialogMessageButton : VP_MonoBehaviour
    {
        [SerializeField] private VP_DialogMessage m_message;
        [SerializeField] private int m_index;
        [SerializeField] private TMP_Text m_text;
        [SerializeField] public bool m_selected;
        [SerializeField] public bool m_hasSelectedIcon = false;
        [SerializeField] public Image m_selectedIcon = null;

        public int Index { get { return m_index; } }
        public TMP_Text ButtonText { get { return m_text; } }

        protected override void Awake()
        {
            base.Awake();
            if (m_text == null)
            {
                m_text = GetComponentInChildren<TMP_Text>();
            }

            if (m_hasSelectedIcon)
            {
                if (!m_selectedIcon)
                {
                    m_selectedIcon = GetComponentInChildren<Image>();
                }
            }
        }

        public void SetData(VP_DialogMessage msg, int index, string btnText)
        {
            m_message = msg;
            m_index = index;
            m_text.text = btnText;
            m_selected = (m_index == 0);
            m_selectedIcon?.gameObject.SetActive(m_selected);
        }

        public void Select()
        {
            m_selected = true;
            m_selectedIcon?.gameObject.SetActive(m_selected);
        }

        public void SelectThis()
        {
            Select();
            m_message.Answer(m_index);
        }
    }

}
