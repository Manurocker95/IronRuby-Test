using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_STANDALONE_WIN || UNITY_EDITOR
using UnityEngine.Windows.Speech;
#endif


namespace VirtualPhenix.SpeechRecognition
{
    [DefaultExecutionOrder(VP_ExecutingOrderSetup.SPEECH_RECOGNIZER), AddComponentMenu(VP_PrefixSetup.MAIN_PREFIX + "/Speech Recognition/Windows Speech Recognizer")]
    public class VP_WindowsSpeechRecognizer : VP_SpeechRecognizer
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
        protected PhraseRecognizer m_phraseRecognizer;
        [Header("Config"), Space]
        [SerializeField, Tooltip("Accuracy in the speech recognition")] protected ConfidenceLevel m_confidenceLevel = ConfidenceLevel.Low;
        [SerializeField, Tooltip("Phrase Recognizer initialized")] protected bool m_recognizerStarted = false;
#endif

        public override void InitSpeechRecognizer()
        {
            if (m_phrases != null)
            {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
                m_phraseRecognizer = new KeywordRecognizer(ConvertToStringArray(m_phrases), m_confidenceLevel);
                m_phraseRecognizer.OnPhraseRecognized += OnSpeechRecognizedEvent;
                m_phraseRecognizer.Start();
                m_recognizerStarted = true;
#endif
            }
        }

        public override void StopSpeechRecognizer()
        {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
            if (m_phraseRecognizer != null && m_phraseRecognizer.IsRunning)
            {
                m_phraseRecognizer.OnPhraseRecognized -= OnSpeechRecognizedEvent;
                m_phraseRecognizer.Stop();
                m_phraseRecognizer.Dispose();
            }
#endif
        }


#if UNITY_STANDALONE_WIN || UNITY_EDITOR
        protected virtual void OnSpeechRecognizedEvent(PhraseRecognizedEventArgs args)
        {
            string word = args.text;
            OnPhraseRecognized.Invoke(word);
        }
#endif
    }
}