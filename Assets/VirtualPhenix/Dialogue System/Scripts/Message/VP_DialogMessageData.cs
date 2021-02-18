using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace VirtualPhenix.Dialog
{
    public enum DIALOG_TYPE
    {
        REGULAR,
        MUGSHOT,
        REGULAR_NAME,
        MUGSHOT_AND_NAME,
        NO_BACKGROUND,
        NONE
    }

    public enum DISPLAY_MODE
    {
        CHARACTER_BY_CHARACTER,
        ALL_DIRECTLY
    }

    [System.Serializable]
    public class VP_DialogMessageData 
    {
        /// <summary>
        /// Stored data explained in the README
        /// </summary>
        [SerializeField] public bool m_overrideColor = true; 
        [SerializeField] public Color m_color = Color.black;
        [SerializeField] public TMPro.TMP_FontAsset m_font;
        [SerializeField] public AudioClip m_clipDialogue;
        [SerializeField] public DIALOG_TYPE m_dialogType;
        [SerializeField] public VP_DialogCharacterData m_characterData;
        [SerializeField] public string m_text;
        [SerializeField] public bool m_showDirectly;
        [SerializeField] public bool m_canSkipWithInput;
        [SerializeField] public float m_automaticScreenTime;
        [SerializeField] public bool m_fadeInOut;
        [SerializeField] public bool m_lastMessage;
        [SerializeField] public bool m_waitForAudioEnd;
        [SerializeField] public bool m_waitForInput;
        [SerializeField] public bool m_answersAtSameTime;
        [SerializeField] public float m_textSpeed;
        [SerializeField] public float m_fadeDuration;
        [SerializeField] public int m_numberOfAnswers;
	    [SerializeField] public float m_fontSize;
	    [SerializeField] public int m_autoAnswer;
        [SerializeField] public List<VP_Dialog.Answer> m_answers;
        [SerializeField] public VP_DialogPositionData m_positionData;
        [SerializeField] public float m_timeForAnswer = 5f;
        [SerializeField] public bool m_chooseNumber;
        [SerializeField] public bool m_canCancel;
        [SerializeField] public bool m_soundOnContinue;
        [SerializeField] public Vector3 m_numberParameters;

        public UnityAction m_onDialogCompleteCallback = null;
        public UnityAction m_onDialogEndCallback = null;
        public UnityAction m_onDialogStartCallback = null;
        public UnityAction m_onTextShownCallback = null;
        public UnityAction<int> m_onAnswerCallback = null;
        public UnityAction<int> m_chooseNumberCallback = null;
        public UnityAction m_cancelNumberCallback = null;

        public VP_DialogMessageData(AudioClip _clip, DIALOG_TYPE _dialogType, string _parsedText, VP_DialogCharacterData data, 
            bool last, bool _fade, bool _canSkip, bool _showDirectly, float _speed, float _fadeDuration, bool _waitForAudioEnd, List<VP_Dialog.Answer> answers, bool _answersAtSameTime,
	        int _autoAnswer, VP_DialogPositionData _overridedPos, Color _color, TMPro.TMP_FontAsset _font = null, bool _waitForInput = true, float _fontSize = 45f,
            float _timeForAnswer = 5f, bool _chooseNumber = false, Vector3 _parameters = default(Vector3), bool _canCancel = false, bool _soundOnContinue = true, bool _overrideColor = true,
            float _automaticScreenTime = 1f, UnityAction _onDialogCompleteCallback = null, UnityAction m_onDialogEndCallback = null, UnityAction m_onDialogStartCallback = null,
             UnityAction m_onTextShownCallback = null, UnityAction<int> _onAnswerCallback = null, UnityAction<int> _chooseNumberCallback = null, UnityAction _cancelNumberCallback = null)
        {
            this.m_overrideColor = _overrideColor;
            this.m_dialogType = _dialogType;
            this.m_characterData = data;
            this.m_text = _parsedText;
            this.m_clipDialogue = _clip;
            this.m_fadeDuration = _fadeDuration;
            this.m_textSpeed = _speed;
            this.m_lastMessage = last;
            this.m_fadeInOut = _fade;
            this.m_canSkipWithInput = _canSkip;
            this.m_showDirectly = _showDirectly;
            this.m_waitForAudioEnd = _waitForAudioEnd;
            this.m_answers = answers;
            this.m_numberOfAnswers = answers != null ? answers.Count : 0;
            this.m_color = _color;
            this.m_font = _font;
            this.m_waitForInput = _waitForInput;
            this.m_positionData = _overridedPos;
	        this.m_fontSize = _fontSize;
            this.m_answersAtSameTime = _answersAtSameTime;
            this.m_autoAnswer = _autoAnswer;
            this.m_timeForAnswer = _timeForAnswer;
            this.m_chooseNumber = _chooseNumber;
            this.m_numberParameters = _parameters;
            this.m_canCancel = _canCancel;
            this.m_soundOnContinue = _soundOnContinue;
            this.m_automaticScreenTime = _automaticScreenTime;

            this.m_onDialogCompleteCallback = _onDialogCompleteCallback;
            this.m_onDialogEndCallback = m_onDialogEndCallback;
            this.m_onDialogStartCallback = m_onDialogStartCallback;
            this.m_onTextShownCallback = m_onTextShownCallback;
            this.m_onAnswerCallback = _onAnswerCallback;
            this.m_chooseNumberCallback = _chooseNumberCallback;
            this.m_cancelNumberCallback = _cancelNumberCallback;
        }
    }

}
