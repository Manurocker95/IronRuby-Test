using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.SpeechRecognition
{
    [System.Serializable]
    public class VP_SpeechPhrase
    {
        [SerializeField] private string m_phraseText;

        public string Text {  get { return m_phraseText; } }

        public VP_SpeechPhrase()
        {

        }

        public VP_SpeechPhrase(string _target)
        {
            m_phraseText = _target;
        }
    }
}
