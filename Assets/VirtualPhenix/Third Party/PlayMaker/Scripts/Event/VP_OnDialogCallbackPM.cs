#if PLAYMAKER
using HutongGames.PlayMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Dialog
{
    [AddComponentMenu("Virtual Phenix/PlayMaker/Callback Events")]
    public class VP_OnDialogCallbackPM : MonoBehaviour
    {
        public enum ON_START_LISTENING
        {
            START,
            AWAKE,
            MANUAL,
            ON_ENABLE
        }

        public enum ON_STOP_LISTENING
        {
            DISABLE,
            DESTROY,
            MANUAL
        }

        public enum EVENT_TYPE
        {
            CUSTOM_EVENT,
            ON_START_DIALOG,
            ON_TEXT_SHOWN,
            ON_CHOICE_SELECTION,
            ON_ANSWERS_SHOWN,
            ON_END_DIALOG,
            ON_SKIP
        }

        [SerializeField] protected ON_START_LISTENING m_startListening = ON_START_LISTENING.START;
        [SerializeField] protected ON_STOP_LISTENING m_stopListening = ON_STOP_LISTENING.DESTROY;
        [SerializeField] protected List<EVENT_TYPE> m_callbacks = new List<EVENT_TYPE>() { EVENT_TYPE.ON_END_DIALOG };
        [SerializeField] protected string m_customEvent = "";

        public List<PlayMakerFSM> targetFsms = new List<PlayMakerFSM>();

        [NonSerialized]
        protected bool initialized;

        public void AddTargetFsm(PlayMakerFSM fsm)
        {
            if (!TargetsFsm(fsm))
            {
                targetFsms.Add(fsm);
            }

            Initialize();
        }

        private bool TargetsFsm(PlayMakerFSM fsm)
        {
            for (var i = 0; i < targetFsms.Count; i++)
            {
                var targetFsm = targetFsms[i];
                if (fsm == targetFsm)
                    return true;
            }

            return false;
        }

        protected void OnEnable()
        {
            Initialize();

            if (m_startListening == ON_START_LISTENING.ON_ENABLE)
                StartAllListeners();
        }

        public void PreProcess()
        {
            Initialize();
        }

        protected virtual void Initialize()
        {
            initialized = true;
        }

        protected void SendEvent(FsmEvent fsmEvent)
        {
            for (var i = 0; i < targetFsms.Count; i++)
            {
                var targetFsm = targetFsms[i];
                targetFsm.Fsm.Event(gameObject, fsmEvent);
            }
        }

        protected void Awake()
        {
            if (m_startListening == ON_START_LISTENING.AWAKE)
                StartAllListeners();
        }

        protected virtual void Start()
        { 
            if (m_startListening == ON_START_LISTENING.START)
                StartAllListeners();
        }

        protected virtual void OnDisable()
        {
            if (m_stopListening == ON_STOP_LISTENING.DISABLE)
                StopAllListeners();
        }

        protected virtual void OnDestroy()
        {
            if (m_stopListening == ON_STOP_LISTENING.DESTROY)
                StopAllListeners();
        }

        protected virtual void StartAllListeners()
        {
            foreach (EVENT_TYPE m_eventType in m_callbacks)
            {
                switch (m_eventType)
                {
                    case EVENT_TYPE.CUSTOM_EVENT:
                        VP_EventManager.StartListening(m_customEvent, () => { SendEvent(FsmEvent.GetFsmEvent("VIRTUAL PHENIX / ON CUSTOM EVENT")); });
                        break;
                    case EVENT_TYPE.ON_ANSWERS_SHOWN:
                        VP_DialogManager.StartListeningToOnAnswerShow(() => { SendEvent(FsmEvent.GetFsmEvent("VIRTUAL PHENIX / ON ANSWERS SHOWN")); });
                        break;
                    case EVENT_TYPE.ON_CHOICE_SELECTION:
                        VP_DialogManager.StartListeningOnChoiceSelection((int _value) => { SendEvent(FsmEvent.GetFsmEvent("VIRTUAL PHENIX / ON CHOICE SELECTED")); });
                        break;
                    case EVENT_TYPE.ON_END_DIALOG:
                        VP_DialogManager.StartListeningToOnDialogEnd(() => { SendEvent(FsmEvent.GetFsmEvent("VIRTUAL PHENIX / ON DIALOGUE END")); });
                        break;
                    case EVENT_TYPE.ON_START_DIALOG:
                        VP_DialogManager.StartListeningToOnDialogStart(() => { SendEvent(FsmEvent.GetFsmEvent("VIRTUAL PHENIX / ON DIALOGUE START")); });
                        break;
                    case EVENT_TYPE.ON_TEXT_SHOWN:
                        VP_DialogManager.StartListeningToOnTextShown(() => { SendEvent(FsmEvent.GetFsmEvent("VIRTUAL PHENIX / ON TEXT SHOWN")); });
                        break;
                    default:
                        VP_DialogManager.StartListeningToOnSkip(() => { SendEvent(FsmEvent.GetFsmEvent("VIRTUAL PHENIX / ON SKIP")); });
                        break;
                }
            }
            
        }

        protected virtual void StopAllListeners()
        {
            foreach (EVENT_TYPE m_eventType in m_callbacks)
            {
                switch (m_eventType)
                {
                    case EVENT_TYPE.CUSTOM_EVENT:
                        VP_EventManager.StopListening(m_customEvent, () => { SendEvent(FsmEvent.GetFsmEvent("VIRTUAL PHENIX / ON CUSTOM EVENT")); });
                        break;
                    case EVENT_TYPE.ON_ANSWERS_SHOWN:
                        VP_DialogManager.StopListeningToOnAnswerShow(() => { SendEvent(FsmEvent.GetFsmEvent("VIRTUAL PHENIX / ON ANSWERS SHOWN")); });
                        break;
                    case EVENT_TYPE.ON_CHOICE_SELECTION:
                        VP_DialogManager.StopListeningOnChoiceSelection((int _value) => { SendEvent(FsmEvent.GetFsmEvent("VIRTUAL PHENIX / ON CHOICE SELECTED")); });
                        break;
                    case EVENT_TYPE.ON_END_DIALOG:
                        VP_DialogManager.StopListeningToOnDialogEnd(() => { SendEvent(FsmEvent.GetFsmEvent("VIRTUAL PHENIX / ON DIALOGUE END")); });
                        break;
                    case EVENT_TYPE.ON_START_DIALOG:
                        VP_DialogManager.StopListeningToOnDialogStart(() => { SendEvent(FsmEvent.GetFsmEvent("VIRTUAL PHENIX / ON DIALOGUE START")); });
                        break;
                    case EVENT_TYPE.ON_TEXT_SHOWN:
                        VP_DialogManager.StopListeningToOnTextShown(() => { SendEvent(FsmEvent.GetFsmEvent("VIRTUAL PHENIX / ON TEXT SHOWN")); });
                        break;
                    default:
                        VP_DialogManager.StopListeningToOnSkip(() => { SendEvent(FsmEvent.GetFsmEvent("VIRTUAL PHENIX / ON SKIP")); });
                        break;
                }
            }
        }
    }
}
#endif