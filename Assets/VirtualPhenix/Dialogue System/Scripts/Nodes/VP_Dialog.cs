using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using XNode;

namespace VirtualPhenix.Dialog
{
    [NodeTint(VP_DialogSetup.NodeColors.DIALOG_NODE), CreateNodeMenuAttribute("Dialog"), CreateAssetMenu(menuName = "Virtual Phenix/Dialogue System/DialogueManager Data/Custom dialog")]
    public class VP_Dialog : VP_DialogBaseNode
    {
        [SerializeField] public bool m_hideInfoInGraph = false;
        [SerializeField] public VP_DialogMessage dialog;
        [SerializeField] public VP_DialogCharacterData characterData;
        [SerializeField] public DIALOG_TYPE dialogType;
        [SerializeField] public VP_DialogPositionData m_positionData;
        [SerializeField] public bool m_useLocalization;
	    [SerializeField] public bool m_useKey = true;
        [SerializeField] public bool m_showDirectly = false;
        [SerializeField] public bool m_canSkipWithInput = true;
        [SerializeField] public bool m_fadeInOut = true;
        [SerializeField] public float m_textSpeed = 1f;
        [SerializeField] public float m_fadeDuration = 0.5f; // If not animator
        [SerializeField] public VP_DialogKey keyT;
        [TextArea, SerializeField] public string key;
        [SerializeField] public AudioClip clip;
        [SerializeField] public bool waitForAudioEnd;
        [SerializeField] public bool waitForInput = true;
        [SerializeField] public bool m_Automatic = true; // if not wait for input, if automatic, the dialog will display and then hide after 2 seconds
        [SerializeField] public float m_automaticScreenTime = 1f;
        [SerializeField] public Color color = Color.black;
        [SerializeField] public TMPro.TMP_FontAsset font;
        [SerializeField] public float m_fontSize = 45f;
        [SerializeField] public bool m_overideFontSize = true;
        [SerializeField] public float m_timeForAnswer = 5f;
        [SerializeField] public bool m_registerDialog = true;
        [SerializeField] public bool m_soundOnContinue = true;


        public UnityAction m_onDialogCompleteCallback = null;
        public UnityAction m_onDialogEndCallback = null;
        public UnityAction m_onDialogStartCallback = null;
        public UnityAction m_onTextShownCallback = null;
        public UnityAction<int> m_onAnswerCallback = null;
        public UnityAction<int> m_chooseNumberCallback = null;
        public UnityAction m_cancelNumberCallback = null;


        [Output(dynamicPortList = true)]
        public List<Answer> answers = new List<Answer>();

        [SerializeField] public bool m_answerAtTheSameTime = true;
        public int m_autoAnswer = -1;
        [SerializeField] public bool hideAfter;

        [System.Serializable]
        public class Answer
        {
            /// <summary>
            /// A not visible answer can't be selected by the player BUT we can select it from code.
            /// This is useful for auto-answer, if we want to define an answer that doesn't appear like '...',
            /// but can be auto-selected.
            /// </summary>
            public bool m_visible = true;
            /// <summary>
            /// Want to translate Answer text?
            /// </summary>
            public bool m_translate;
            /// <summary>
            /// Text or Key to translate
            /// </summary>
            public string m_answerKey;
            /// <summary>
            /// If we press cancel button, this will be selected. This is useful for yes-no questions so the user 
            /// can spam A or B
            /// </summary>
            public bool m_selectIfCancel = false;
            /// <summary>
            /// Wait for ending the voiceclip (if it's not null), before continuing our dialogues
            /// </summary>
            public bool m_waitForVoiceClip = true;
            /// <summary>
            /// Voice clip played when this answer is selected. 
            /// </summary>
            public AudioClip m_voiceClip;
            /// <summary>
            /// Custom callback
            /// </summary>
            public UnityAction m_callback;

            public Answer()
            {

            }

            public Answer(bool _translate, string _key, bool _visible = true, bool _selectIfCancel = false, bool _waitForVoiceClip = false, AudioClip _voiceClip = null, UnityAction _callback = null)
            {
                this.m_translate = _translate;
                this.m_answerKey = _key;
                this.m_visible = _visible;
                this.m_waitForVoiceClip = _waitForVoiceClip;
                this.m_voiceClip = _voiceClip;
                this.m_selectIfCancel = _selectIfCancel;
                this.m_callback = _callback;
            }
        }

        protected override void Awake()
        {
            base.Awake();

            RefreshInit();
        }

        public override void RefreshInit()
        {
            if (keyT == null)
            {
                keyT = Resources.Load<VP_DialogKey>("Dialogue/Data/defaultKeyData");
                keyT.key = "";
            }

            if (font == null)
            {
                font = Resources.Load<TMPro.TMP_FontAsset>("Dialogue/Fonts/DialogueSystemDefaultFont");
            }

            if (m_positionData == null)
            {
                m_positionData = Resources.Load<VP_DialogPositionData>("Dialogue/Data/defaultPositionData");
            }

            if (keyT.list != null && !keyT.list.ContainsKey(m_ID))
                keyT.list.Add(m_ID, "");
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            RefreshInit();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (keyT != null && keyT.list != null && keyT.list.ContainsKey(m_ID))
                keyT.list.Remove(m_ID);
        }

        public void SetValuesByPreset(VP_DialogGraphPreset _preset)
        {
            this.dialogType = _preset.m_dialogueType;
            this.characterData = _preset.m_characterData;
            this.m_positionData = _preset.m_positionData;
            this.m_useLocalization = _preset.m_localization;
            this.m_canSkipWithInput = _preset.m_canSkipWithInput;
            this.waitForInput = _preset.m_waitForInput;
            this.waitForAudioEnd = _preset.m_waitForAudioEnd;
            this.color = _preset.m_textColor;
            this.m_showDirectly = _preset.m_showDirectly;
            this.m_fadeInOut = _preset.m_fadeInOut;
            this.m_textSpeed = _preset.m_textSpeed;
            this.m_fadeDuration = _preset.m_fadeDuration;
            this.m_registerDialog = _preset.m_registerDialog;
            this.m_soundOnContinue = _preset.m_soundOnContinue;
            this.m_automaticScreenTime = _preset.m_automaticScreenTime;
        }

        public bool ContinueText(int _index)
        {
            NodePort port = null;
            if (answers.Count == 0)
            {
                port = GetOutputPort(VP_DialogSetup.Fields.DIALOG_NODE_OUTPUT);
            }
            else
            {
                if (answers.Count <= _index)
                {
                    port = GetOutputPort(VP_DialogSetup.Fields.DIALOG_NODE_OUTPUT);
                    return false;
                }

                port = GetOutputPort(VP_DialogSetup.Fields.DIALOG_NODE_ANSWERS + " " + _index);
            }

            if (port == null)
            {
                port = GetOutputPort(VP_DialogSetup.Fields.DIALOG_NODE_OUTPUT);
            }

            VP_DialogManager.OnDialogCompleteAction();
            if (port.ConnectionCount == 0)
            {
                CheckEndSequenceNode();
                return false;
            }

            for (int i = 0; i < port.ConnectionCount; i++)
            {
                NodePort connection = port.GetConnection(i);
                (connection.node as VP_DialogBaseNode).Trigger();
            }

            return true;
        }

        public override void Trigger()
        {
           
            if (m_needToSkip)
            {
                m_needToSkip = false;
                (graph as VP_DialogGraph).CallChart(this, true);
            }
            else
            {
                (graph as VP_DialogGraph).CallChart(this);
            }
        }

        public override void Trigger<T>(T parameter)
        {
            if (parameter == null)
            {
                Trigger();
                return;
            }

            key = key.Replace("{parameter}", parameter.ToString());

            if (m_needToSkip)
            {
                m_needToSkip = false;
                (graph as VP_DialogGraph).CallChart(this, true);
            }
            else
            {
                (graph as VP_DialogGraph).CallChart(this);
            }
        }

    }
}
