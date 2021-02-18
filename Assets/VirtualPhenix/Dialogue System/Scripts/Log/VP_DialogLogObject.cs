using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace VirtualPhenix.Dialog
{
    public class VP_DialogLogObject : MonoBehaviour
    {
        [SerializeField] private TMP_Text m_characterNameText;
        [SerializeField] private TMP_Text m_dialogText;

        public void SetData(string _characterName, string _dialog)
        {
            m_characterNameText.text = _characterName;
            m_dialogText.text = _dialog;
        }

        void Awake()
        {
            if (m_characterNameText == null)
                m_characterNameText = transform.GetChild(0).GetComponent<TMP_Text>();

            if (m_dialogText == null)
                m_dialogText = transform.GetChild(1).GetComponent<TMP_Text>();
        }
    }

}
