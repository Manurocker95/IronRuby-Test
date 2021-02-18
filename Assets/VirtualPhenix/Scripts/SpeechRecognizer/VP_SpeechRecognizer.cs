using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace VirtualPhenix.SpeechRecognition
{
    [System.Serializable]
    public class VP_SpeechEvent : UnityEvent<string>
    {

    }
    [DefaultExecutionOrder(VP_ExecutingOrderSetup.SPEECH_RECOGNIZER),AddComponentMenu(VP_PrefixSetup.MAIN_PREFIX+"/Speech Recognition/Speech Recognizer")]
    public class VP_SpeechRecognizer : VP_MonoBehaviour
    {
        [Header("Phrases"),Space]
        [SerializeField] protected List<VP_SpeechPhrase> m_phrases = new List<VP_SpeechPhrase>();
        [Header("Event Triggered on recognized"), Space]
        [SerializeField] protected VP_SpeechEvent OnPhraseRecognized = new VP_SpeechEvent();
        [Header("Config"), Space]
        [SerializeField] protected bool m_useEventManager = true;
        protected virtual void Reset()
        {
            m_initializationTime = InitializationTime.OnAwake;
            m_startListeningTime = StartListenTime.OnEnable;
            m_stopListeningTime = StopListenTime.OnDisable;
        }

        protected override void StartAllListeners()
        {
            base.StartAllListeners();
            VP_EventManager.StartListening<string>(VP_EventSetup.SpeechRecognizer.SPEECH_RECOGNIZED, DebugPhrase);
            InitSpeechRecognizer();
        }

        protected override void StopAllListeners()
        {
            base.StopAllListeners();
            VP_EventManager.StopListening<string>(VP_EventSetup.SpeechRecognizer.SPEECH_RECOGNIZED, DebugPhrase);
            StopSpeechRecognizer();
        }

        public virtual void InitSpeechRecognizer()
        {

        }

        public virtual void StopSpeechRecognizer()
        {

        }

        public virtual void OnEventTrigger(string _text)
        {
            OnPhraseRecognized.Invoke(_text);

            VP_EventManager.TriggerEvent(VP_EventSetup.SpeechRecognizer.SPEECH_RECOGNIZED, _text);
        }

        public virtual void DebugPhrase(string _text)
        {

        }

        public virtual string[] ConvertToStringArray(List<VP_SpeechPhrase> _phrases)
        {
            string[] result = new string[_phrases.Count];

            for (int i = 0; i < _phrases.Count; i++)
            {
                result[i] = _phrases[i].Text;
            }

            return result;
        }
    }
}
