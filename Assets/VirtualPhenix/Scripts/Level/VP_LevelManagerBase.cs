using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace VirtualPhenix.Level
{

    [System.Serializable]
    public class VP_LevelCompleteEvent : UnityEvent<bool>
    {

    }

    [DefaultExecutionOrder(VP_ExecutingOrderSetup.LEVEL_MANAGER), AddComponentMenu("")]
    public class VP_LevelManagerBase : VP_SingletonMonobehaviour<VP_LevelManagerBase>, VP_ITranslatable
    {
        public enum LevelUIButtons
        {
            Exit
        }

        [Header("UI"), Space]
        [SerializeField] protected bool m_canInteract;
        [SerializeField] protected LevelUIButtons m_levelUI;

        [Header("Events"), Space]
        public VP_LevelCompleteEvent m_onLevelComplete;
        public UnityEvent m_onLevelLoad;
        public UnityEvent m_onLevelStart;
        public UnityEvent m_onLevelReset;
        public UnityEvent m_onLevelExit;


        public bool CanInteract { get { return m_canInteract; } set { m_canInteract = value; } }

        protected override void StartAllListeners()
        {
            base.StartAllListeners();

            m_onLevelLoad.AddListener(OnLevelLoad);
            m_onLevelStart.AddListener(OnLevelStart);
            m_onLevelReset.AddListener(OnLevelReset);
            m_onLevelComplete.AddListener(OnLevelComplete);
            m_onLevelExit.AddListener(OnLevelExit);

            VP_EventManager.StartListening(VP_EventSetup.Localization.TRANSLATE_TEXTS, TranslateTexts);
            StartListenToLoadLevel();
        }

        protected override void StopAllListeners()
        {
            base.StopAllListeners();

            m_onLevelLoad.RemoveListener(OnLevelLoad);
            m_onLevelStart.RemoveListener(OnLevelStart);
            m_onLevelReset.RemoveListener(OnLevelReset);
            m_onLevelComplete.RemoveListener(OnLevelComplete);
            m_onLevelExit.RemoveListener(OnLevelExit);

            VP_EventManager.StopListening(VP_EventSetup.Localization.TRANSLATE_TEXTS, TranslateTexts);
            StopListenToLoadLevel();
        }

        protected virtual void StartListenToLoadLevel()
        {

        }

        protected virtual void StopListenToLoadLevel()
        {

        }


        public virtual void PlaySoundButton()
        {
            VP_AudioManager.PlayAudioItemOneShotbyKey(VP_AudioSetup.UI.CONFIRM);
        }

        protected override void OnApplicationQuit()
        {
            base.OnApplicationQuit();

            ExitLevel();
        }

        public virtual void LevelLoad()
        {
            m_onLevelLoad.Invoke();
        }


        public virtual void LevelStart()
        {
            m_onLevelStart.Invoke();
        }

        public virtual void LevelReset()
        {
            m_onLevelReset.Invoke();
        }

        public virtual void LevelCompleted(bool _completed)
        {
            m_onLevelComplete.Invoke(_completed);
        }


        public virtual void ExitLevel()
        {
            m_onLevelExit.Invoke();
        }

        protected virtual void OnLevelComplete(bool _completed)
        {
            VP_Debug.Log("Level completed");
        }

        protected virtual void OnLevelStart()
        {
            VP_Debug.Log("Level started");
        }

        protected virtual void OnLevelReset()
        {
            VP_Debug.Log("Level reset");
        }

        protected virtual void OnLevelExit()
        {
            VP_Debug.Log("Exiting Level");
        }

        protected virtual void OnLevelLoad()
        {
            VP_Debug.Log("Level Load");
        }

        public virtual void TranslateTexts()
        {

        }
    }
}