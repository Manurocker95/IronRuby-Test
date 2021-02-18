using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VirtualPhenix.Localization;
using UnityEngine.Events;
#if DOTWEEN
using DG.Tweening;
#endif
using UnityEngine.EventSystems;

namespace VirtualPhenix
{
	[DefaultExecutionOrder(VP_ExecutingOrderSetup.ALERT_MANAGER),AddComponentMenu(VP_PrefixSetup.MAIN_PREFIX+"/Alert/Alert Manager")]
    public class VP_AlertManager : VP_SingletonMonobehaviour<VP_AlertManager>
    {
        [Header("Alert"),Space]
        [SerializeField] protected GameObject m_alertGroup;
        [SerializeField] protected Transform m_alertPanel;
        [SerializeField] protected TMP_Text m_text;

        [Header("Single Button")]
        [SerializeField] protected GameObject m_singleBtnGroup;
        [SerializeField] protected TMP_Text m_singleBtnText;
        [SerializeField] protected Button m_singleBtn;

        UnityAction m_singleUnityAction;

        [Header("Yes No Button")]
        [SerializeField] protected GameObject m_yesNoGroup;
        [SerializeField] protected TMP_Text m_yesBtnText;
        [SerializeField] protected TMP_Text m_noBtnText;
        [SerializeField] protected Button m_yesBtn;
        [SerializeField] protected Button m_noBtn;

        UnityAction m_yesUnityAction;
        UnityAction m_noUnityAction;

        [Header("Speed"), Space]
        [SerializeField] protected float m_scale = 1f;
        [SerializeField] protected float m_duration = 0.3f;
        [SerializeField] protected float m_lastDuration = 0.3f;

        GameObject m_previousGOInES;

        protected override void Initialize()
        {
            base.Initialize();

            m_alertGroup.SetActive(false);
            m_yesNoGroup.SetActive(false);
        }

        public virtual void PlaySoundButton()
        {
            VP_AudioManager.PlayAudioItemOneShotbyKey(VP_AudioSetup.UI.CONFIRM);
        }

        public virtual void PlayAlertShow()
        {
            VP_AudioManager.PlayAudioItemOneShotbyKey(VP_AudioSetup.UI.ALERT_SHOW);
        }

        public virtual void PlayAlertHide()
        {
            //VP_AudioManager.PlayAudioItemOneShotbyKey(VP_AudioSetup.UI.ALERT_SHOW);
        }

        public static void Alert(string _text, string _buttonText = "Ok", UnityAction _callback = null, bool _closeOnClick = true, float _duration = -1f, bool _translateText = true, bool _translateButton = true)
        {
            var ins = nullableInstance;
            if (ins)
            {
                ins.ShowAlert(_text, _buttonText, _callback, _closeOnClick, _duration, _translateText, _translateButton);
            }
        }

        public virtual void ShowAlert(string _text, string _buttonText = "Ok", UnityAction _callback = null, bool _closeOnClick = true, float _duration = -1f, bool _translateText = true, bool _translateButton = true)
        {
            m_alertGroup.SetActive(true);
            m_yesNoGroup.SetActive(false);
            m_singleBtnGroup.SetActive(true);

            m_lastDuration = _duration > 0 ? _duration : m_duration;

            m_text.text = (_translateText) ? VP_LocalizationManager.GetText(_text) : _text;
            m_singleBtnText.text = (_translateButton) ? VP_LocalizationManager.GetText(_buttonText) : _buttonText;

            m_singleUnityAction = () => { PlaySoundButton(); HideAlert(_callback, _closeOnClick); };
#if DOTWEEN
            m_alertPanel.DOScale(m_scale, m_lastDuration).From(.1f).OnComplete(()=>
            {
                EventSystem es = EventSystem.current;
                if (es)
                {
                    m_previousGOInES = es.currentSelectedGameObject;
                    es.SetSelectedGameObject(m_singleBtn.gameObject);
                }
   
                PlayAlertShow();
                m_singleBtn.onClick.AddListener(m_singleUnityAction);
            });
#else
            m_alertPanel.localScale = new Vector3(m_scale, m_scale, m_scale);
            EventSystem es = EventSystem.current;
            if (es)
            {
                m_previousGOInES = es.currentSelectedGameObject;
                es.SetSelectedGameObject(m_singleBtn.gameObject);
            }

            PlayAlertShow();
            m_singleBtn.onClick.AddListener(m_singleUnityAction);
#endif
        }

        public virtual void HideAlert(UnityAction _action = null, bool _closeOnClick = true)
        {
            if (_action != null)
                _action.Invoke();

            if (m_singleUnityAction != null)
            {
                m_singleBtn.onClick.RemoveListener(m_singleUnityAction);
                m_singleUnityAction = null;
            }

            if (m_yesUnityAction != null)
            {
                m_yesBtn.onClick.RemoveListener(m_yesUnityAction);
                m_yesUnityAction = null;
            }

            if (m_noUnityAction != null)
            {
                m_noBtn.onClick.RemoveListener(m_noUnityAction);
                m_noUnityAction = null;
            }

            if (_closeOnClick)
            {
#if DOTWEEN
                m_alertPanel.DOScale(.1f, m_lastDuration).From(1f).OnComplete(() =>
                {
                    EventSystem es = EventSystem.current;
                    if (es)
                    {
                        es.SetSelectedGameObject(m_previousGOInES);
                    }

                    PlayAlertHide();
                    m_alertGroup.SetActive(false);
                });
#else
                m_alertPanel.localScale = new Vector3(.1f, .1f, .1f);
                EventSystem es = EventSystem.current;
                if (es)
                {
                    es.SetSelectedGameObject(m_previousGOInES);
                }

                PlayAlertHide();
                m_alertGroup.SetActive(false);
#endif
            }
        }

        public static void ShowConfirm(string _text, string _yesButtonText = "Yes", string _noButtonText = "No", UnityAction _yesCallback = null, UnityAction _noCallback = null, bool _closeOnClick = true, float _duration = -1f, bool _translateText = true, bool _translateButton = true)
        {
            var ins = nullableInstance;
            if (ins)
            {
                ins.ShowCondition(_text, _yesButtonText, _noButtonText, _yesCallback, _noCallback, _closeOnClick, _duration, _translateText, _translateButton);
            }
        }

        public virtual void ShowCondition(string _text, string _yesButtonText = "Ok", string _noButtonText = "Ok", UnityAction _yesCallback = null, UnityAction _noCallback = null, bool _closeOnClick = true, float _duration = -1f, bool _translateText = true, bool _translateButton = true)
        {
            m_alertGroup.SetActive(true);
            m_yesNoGroup.SetActive(true);
            m_singleBtnGroup.SetActive(false);

            m_lastDuration = _duration > 0 ? _duration : m_duration;
            m_text.text = (_translateText) ? VP_LocalizationManager.GetText(_text) : _text;

            if (_translateButton)
            {              
                m_yesBtnText.text = VP_LocalizationManager.GetText(_yesButtonText);
                m_noBtnText.text = VP_LocalizationManager.GetText(_noButtonText);
            }
            else
            {
                m_yesBtnText.text = _yesButtonText;
                m_noBtnText.text = _noButtonText;
            }

            m_yesUnityAction = () => { PlaySoundButton(); HideAlert( _yesCallback, _closeOnClick); };
            m_noUnityAction = () => { PlaySoundButton(); HideAlert(_noCallback, _closeOnClick); };
#if DOTWEEN
            m_alertPanel.DOScale(m_scale, m_lastDuration).From(.1f).OnComplete(() =>
            {
                PlayAlertShow();

                EventSystem es = EventSystem.current;
                if (es)
                {
                    m_previousGOInES = es.currentSelectedGameObject;
                    es.SetSelectedGameObject(m_yesBtn.gameObject);
                }


                m_yesBtn.onClick.AddListener(m_yesUnityAction);
                m_noBtn.onClick.AddListener(m_noUnityAction);
            });
#else
            m_alertPanel.localScale = new Vector3(.1f, .1f, .1f);
            PlayAlertShow();

            EventSystem es = EventSystem.current;
            if (es)
            {
                m_previousGOInES = es.currentSelectedGameObject;
                es.SetSelectedGameObject(m_yesBtn.gameObject);
            }


            m_yesBtn.onClick.AddListener(m_yesUnityAction);
            m_noBtn.onClick.AddListener(m_noUnityAction);
#endif
        }
    }

}
