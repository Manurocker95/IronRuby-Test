using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VirtualPhenix.Localization;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using VirtualPhenix.ResourceReference;

#if USE_MORE_EFFECTIVE_COROUTINES
using MEC;
#endif

#if DOTWEEN
using DG.Tweening;
#endif

#if USE_ANIMANCER
using Animancer;
#endif

namespace VirtualPhenix.Dialog
{
    public class VP_DialogMessage : VP_Monobehaviour
    {
#region enums
        public enum TEXT_TRIM_TYPE
        {
            BY_CHARACTER,
            BY_WORD
        }

        public enum EMOTION
        { 
            NONE,
            HAPPY, 
            SAD, 
            SURPRISE,
            THINKING, 
            ANGRY 
        };

        /// <summary>
        /// Animation types so the user can add more
        /// </summary>
        public enum ANIMATION_TYPE
        {
            FADE_IN_OUT,
            TWEEN,
            CUSTOM
        }
        /// <summary>
        /// States of the current message
        /// </summary>
        public enum TEXT_STATE
        {
            HIDDEN,
            FADING_ON,
            SHOWING,
            SHOWN,
            FADING_OUT
        }

        public enum FADE_ANIMATION_TYPE
        {
            ANIMATOR,
	        DOTWEEN,
	        MANUAL_FADING,
            CUSTOM
        }

#endregion

        [Header("Dialog Data"), Space(10)]
        /// <summary>
        /// Data send when triggering a dialog node
        /// </summary>
        [SerializeField]protected VP_DialogMessageData m_dialogData;
        /// <summary>
        /// current character data
        /// </summary>
	    [SerializeField] protected VP_DialogCharacterData m_characterData;
       

        [Header("Dialog State"), Space]
        /// <summary>
        /// Current state of the message. Serialized for debugging
        /// </summary>
        [SerializeField] protected TEXT_STATE m_state;

        [Header("Dialog Configurations"), Space]
        protected string m_backgroundsdPath = "Dialogue/Database/Audio/DialogBackground";
        [SerializeField] protected VP_DialogBackgrounds m_backgrounds;
        [SerializeField] protected bool m_changeBg = true;
        [SerializeField] protected bool m_setPositionData = true;
        [SerializeField] protected bool m_setSpeed = true;
        [SerializeField] protected float m_defaultSpeed = 2f;
        [SerializeField] protected float m_textSpeed = 2f;

        /// <summary>
        /// When there is no character... do you want a sound every character or not?
        /// </summary>
        [SerializeField] protected bool m_playDefaultSoundOnNoCharacter;
        /// <summary>
        /// Do you want to show a bubble arrow icon on the top of the gameobject?
        /// </summary>
        [SerializeField] protected bool m_useIconArrow;
        [SerializeField] protected bool m_setIconArrowPosition;
        [SerializeField] protected bool m_useContinueIcon = true;
        [SerializeField] protected bool m_setInsideScreen = true;
        [SerializeField] protected bool m_changeFontSize = true;
        /// <summary>
        /// If you want to display the answers with the last text or not. If false, the last text will be deleted before showing the answers
        /// </summary>
        [SerializeField] protected bool m_textAndAnswers = true;
        /// <summary>
        /// if SFX is played when interacting with the bubble or passing through dialogues
        /// </summary>
        [SerializeField] protected bool m_playSoundOnContinue = true;
        [SerializeField] protected bool m_canPlaySound = true;
        /// <summary>
        /// If the dialogue doesn't wait for user input, how much time spend with the dialogue shown
        /// </summary>
        [SerializeField] protected float m_noInputDialogWaitTime = 1f;
        /// <summary>
        /// If this message bubble listen to the PDS default callbacks or not. Untoggling this will make you listen to your own callbacks.
        /// </summary>
        [SerializeField] protected VP_SerializedCallbackListenDictionary m_listenDefaultInputCallbacks;
        [SerializeField] protected bool m_listenToEventManager = false;
        /// <summary>
        /// Delay time per character so we can customize them easily. Default is 0.1f
        /// </summary>
        protected List<float> m_characterPrintDelays;
        /// <summary>
        /// If any of the answers has Cancel input as default selection, the index is stored here
        /// </summary>
        protected int m_cancelIndex = -1;
        /// <summary>
        /// If neither number choose or answers can be cancelled
        /// </summary>
        protected bool m_canCancel = false;
        [SerializeField] protected bool m_discoverName = true;

        [Header("Dialog & Text Animations"), Space]
        /// <summary>
        ///Current appearing animation
        /// </summary>
        [SerializeField] protected ANIMATION_TYPE m_dialogAnimation;
        /// <summary>
        /// Animator for this dialog. It was made by code, but this way is easier to create appearing animations (fade in/OUT)
        /// </summary>
	    [SerializeField] protected Animator m_animator;
#if USE_ANIMANCER        
	    [SerializeField] protected bool m_useAnimancer = true;
	    [SerializeField] protected AnimancerComponent m_animancerComponent;
	    [SerializeField] protected VP_AnimationTransitionResources m_animationResources;
	    
	    protected AnimancerState m_animancerState;
#endif	  
	    [SerializeField] protected bool m_useMugshotAnimation = true;
        /// <summary>
        /// Mugshot animation reference
        /// </summary>
        [SerializeField] protected Animator m_mugshotAnimator;
        /// <summary>
        /// Presets for shake and curve libraries
        /// </summary>
        [SerializeField] protected VP_DialogShakeLibrary m_shakeLibrary;
        [SerializeField] protected VP_DialogCurveLibrary m_curveLibrary;
        /// <summary>
        /// List of animations in the current text
        /// </summary>
        protected List<VP_DialogTextAnimation> m_animations;
        protected bool m_refreshAnimations = false;
        /// <summary>
        /// Do we have animation or do we show the message directly?
        /// </summary>
        protected bool m_hasAnimations;

#if DOTWEEN
        [Header("Custom Do Tween animations"), Space]
        [SerializeField] protected DG.Tweening.DOTweenAnimation m_doTweenAnimationIn;
        [SerializeField] protected DG.Tweening.DOTweenAnimation m_doTweenAnimationOut;
#endif

        [Header("Emotion List"), Space(10)]
        [SerializeField] protected List<KeyValuePair<int, KeyValuePair<EMOTION, string>>> m_emotionList;

        [Header("Discover Character Name"), Space(10)]
        [SerializeField] protected string m_discoverVariableName = "discoveredName";
        
        [Header("Dialog UI References"), Space]
        [SerializeField] protected TMP_Text m_characterNameText;
        [SerializeField] protected TMP_Text m_mainText;
        [SerializeField] protected Image m_sprite;
        [SerializeField] protected Image m_bg;
        [SerializeField] protected Image m_iconForNext;
        [SerializeField] protected Image m_iconArrow;
        [SerializeField] protected GameObject m_mainButton;
        [SerializeField] protected GameObject m_optionGroup;
        [SerializeField] protected GameObject m_buttonPrefab;
        [SerializeField] protected RectTransform m_buttonTrs;
        [SerializeField] protected RectTransform m_dialogRect;

#if USE_TEXT_ANIMATOR
        [Header("Text animator Support"), Space]
        [SerializeField] protected bool m_useTextAnimator;
        [SerializeField] protected Febucci.UI.TextAnimatorPlayer m_textAnimatorPlayer;
#endif
        private int m_thresholdTextAnimator = 1;
        private int m_counterTextAnimator = 0;

        [Header("Event System selection")]
        [SerializeField] protected bool m_eventSystemHandling = true;
        [SerializeField] protected GameObject m_previousSelection;
        /// <summary>
        /// List of buttons created based on the options in the graph
        /// </summary>
        protected List<VP_DialogMessageButton> m_answerButtons;
        protected List<VP_Dialog.Answer> m_answers;


        protected string m_letterSoundPath = "Dialogue/Database/Audio/CharacterAudioDB";
        [Header("Character Audios"), Space]
        [SerializeField] protected VP_CharacterAudioDatabase m_characterAudios;
        /// <summary>
        /// Audio source for voice dialogues
        /// </summary>
        [SerializeField] protected AudioSource m_source;
        /// <summary>
        /// Audio source for the character sounds
        /// </summary>
        [SerializeField] protected AudioSource m_letterSource;

        protected DIALOG_VOICE_TYPE m_voiceType = DIALOG_VOICE_TYPE.NO_SOUND;


        /// <summary>
        /// Are we using AudioManager or manual audio source?
        /// </summary>
        [SerializeField] protected bool m_useAudioManager;

       
        protected bool m_waitingForAnswerAudio = false;
        /// <summary>
        /// Number of characters displayed in m_showingtext (tagless text)
        /// </summary>
        protected int m_textCharacters;
        /// <summary>
        /// Text that will be displayed with tags
        /// </summary>
        protected string m_textToShow = "";
        /// <summary>
        /// Text we are showing after removing all tags
        /// </summary>
        protected string m_showingText = "";

        /// <summary>
        /// Number of answers in a dialog node
        /// </summary>
        protected int m_numberOfAnswers;
        /// <summary>
        /// The regular delay stored in VP_DialogSetup
        /// </summary>
        protected float m_regularDelay;

        [Header("Answer"), Space]
        /// <summary>
        /// Currently selected answer index. We can change this from anywhere to automatically select an option OnAnswerShown callback
        /// </summary>
        public int m_selectedAnswerIndex = 0;
        [SerializeField] private float m_delayAfterAnswer = 0.5f;

        [Header("Choose Number"),Space]
        
        [SerializeField] private GameObject m_chooseNumberGO = null;
        [SerializeField] private Vector2 m_parameters;
        [SerializeField] private int m_currentNumber;
        [SerializeField] private bool m_chooseNumber = false;
        [SerializeField] private bool m_canChooseNumber = false;
        [SerializeField] private bool m_use0InTen = false;
        [SerializeField] private TMP_Text m_chooseNumberText = null;

        [Header("Auto-Answer"), Space]
        /// <summary>
        /// Auto-Answer (Tell Tale games)
        /// </summary>
        public int m_autoAnswer = -1;
        /// <summary>
        /// If auto answer is > 0, how much time has the player to answer. If the player doesn't select an answer, 
        /// </summary>
        public float m_autoTimeToAnswer = 5f;
        /// <summary>
        /// How much time passed. Public to modify it from anywhere
        /// </summary>
        public float m_spentAutoTime = 0f;
        /// <summary>
        /// Number of painted characters from tagless text
        /// </summary>
        protected int m_currPrintedChars = 0;
        [Header("Trim the text"), Space]
        [SerializeField] protected TEXT_TRIM_TYPE m_trimType;
        /// <summary>
        /// If the text can subdivide in sub-text chunks based on number of characters
        /// </summary>
        [SerializeField] protected bool m_queueIfMaxCharactersOrWords = false;
        /// <summary>
        /// Number of characters in which m_queueIfMaxCharacters separates the text (in cunks)
        /// </summary>
        [SerializeField] protected int m_maxCharactersOrWordsNewLine = 0;
        /// <summary>
        /// Controls for chunks
        /// </summary>
        protected bool m_needToContinueSameDialog;
        protected bool m_waitingForInputToContinueSameDialog;
        protected bool m_needLastDisplay;
        /// <summary>
        /// if you want to delete each chunk of text string after user interface
        /// </summary>
        [SerializeField] protected bool m_trimPassedDialog;
        /// <summary>
        /// Counter that goes from zero to m_maxCharactersNewLine if m_queueIfMaxCharacters is true
        /// </summary>
        protected int m_waitingChars = 0;
        /// <summary>
        /// Counter for times the user interacted with cut dialog (queue if max characters(
        /// </summary>
        protected int m_timesPerformedContinueDialog = 0;


        [Header("Fading Configuration"), Space]
        /// <summary>
        /// Type of animations for fading. if you want to do it manually, use Fade Animation.ANIMATOR. Do tween has a seamless fading tho .
        /// </summary>
        [SerializeField] protected FADE_ANIMATION_TYPE m_fadeAnimationType = FADE_ANIMATION_TYPE.DOTWEEN;

        [Header("Fading With DoTween"), Space]
        /// <summary>
        /// do we need to show answers?
        /// </summary>
        protected bool m_needToShowAnswers;
        protected float m_animatorOriginalSpeed = 1f;
        [SerializeField] protected float m_timeToFadeWithDoTween = 1f;
        /// <summary>
        /// If false, m_timeToFadeWithDoTween will be used for DoTween to all graphics in the dictionary. if true, you need to define the time to fade
        /// </summary>
        [SerializeField] protected bool m_useSameFadeTimeToAllGraphics = true;
        /// <summary>
        /// Canvas group allows you to fade ALL ui by controling just one object. However, you can add any graphic to the array
        /// </summary>
        [SerializeField] protected CanvasGroup m_dialogCanvasGroup;
        /// <summary>
        /// And it will fade one by one. This way you can fade
        /// </summary>
        [SerializeField] protected VP_GraphicFadingDictionary m_graphicsToFadeManually;

#if !USE_MORE_EFFECTIVE_COROUTINES
        /// <summary>
        /// Coroutines saved for stopping them if needed
        /// </summary>
        protected Coroutine m_displayCoroutine;
        protected Coroutine m_automaticDialog;
        protected Coroutine m_automaticSubDialog;
        protected Coroutine m_autoAnswerCoroutine;
#else
        /// <summary>
        /// Coroutines saved for stopping them if needed
        /// </summary>
        protected CoroutineHandle m_displayCoroutine;
        protected CoroutineHandle m_automaticDialog;
        protected CoroutineHandle m_automaticSubDialog;
        protected CoroutineHandle m_autoAnswerCoroutine;
#endif

        protected List<Graphic> m_fadingGraphics = new List<Graphic>();
	    protected List<CanvasGroup> m_fadingCanvasGroups = new List<CanvasGroup>();

	    [Header("Event"), Space(10)]
	    public UnityEngine.Events.UnityEvent OnDialogFadedIn;
	    public UnityEngine.Events.UnityEvent OnDialogFadedOut;
	    public UnityEngine.Events.UnityEvent OnDialogContinue;
	    
        /// <summary>
        /// Event system reference. We need to select this from inspector 
        /// </summary>
        protected EventSystem m_currentES;
        /// <summary>
        /// Selected object for current Event system
        /// </summary>
        protected GameObject m_selectedBtn;

        public virtual RectTransform MainTextRect { get { return m_mainText.rectTransform; } }
        public virtual RectTransform BGRect { get { return m_bg.rectTransform; } }
        public virtual GameObject OptionGroup { get { return m_optionGroup; } }

        protected override void OnEnable()
        {

            if (!m_currentES)
                m_currentES = EventSystem.current;

            if (m_fadeAnimationType == FADE_ANIMATION_TYPE.MANUAL_FADING || m_fadeAnimationType == FADE_ANIMATION_TYPE.DOTWEEN)
            {
                if (m_animator != null)
                    m_animator.enabled = false;
            }
            else
            {
                if (m_animator != null)
                    m_animator.enabled = true;

                m_animatorOriginalSpeed = m_animator.speed;
                m_animator.SetTrigger(VP_DialogSetup.Animations.HIDE_DIRECTLY);
            }

            if (m_answerButtons == null)
                m_answerButtons = new List<VP_DialogMessageButton>();

            if (!m_dialogCanvasGroup)
                m_dialogCanvasGroup = GetComponent<CanvasGroup>();

            if (m_bg == null)
                m_bg = GetComponent<Image>();

            if (!m_dialogRect)
                m_dialogRect = GetComponent<RectTransform>();

            if (!m_dialogCanvasGroup)
                m_dialogCanvasGroup = GetComponent<CanvasGroup>();

            if (!m_dialogCanvasGroup)
                m_dialogCanvasGroup = gameObject.AddComponent<CanvasGroup>();

            m_dialogCanvasGroup.alpha = 0f;

            m_state = TEXT_STATE.HIDDEN;

            base.OnEnable();

        }

        // Start is called before the first frame update
        protected override void Start()
        {

            if (!m_dialogCanvasGroup)
                m_dialogCanvasGroup = GetComponent<CanvasGroup>();

            if (!m_dialogCanvasGroup)
                m_dialogCanvasGroup = gameObject.AddComponent<CanvasGroup>();

            if (m_bg == null)
                m_bg = GetComponent<Image>();

            if (!m_dialogRect)
                m_dialogRect = GetComponent<RectTransform>();

            if (m_useAudioManager)
            {
                if (m_source == null)
                    m_source = VP_AudioManager.Instance.VoiceSource;

                if (m_letterSource == null)
                    m_letterSource = VP_AudioManager.Instance.SFXSource;
            }
#if DOTWEEN
            DOTween.Init();
#endif

            base.Start();

        }

        protected virtual void Update()
        {
            
            if (m_state == TEXT_STATE.SHOWN)
            {
                if (m_refreshAnimations)
                   this.UpdateMeshAndAnims();

                if (m_numberOfAnswers > 0)
                {
                    if (m_currentES != null)
                    {
                        if (m_currentES.currentSelectedGameObject != m_selectedBtn)
                        {
                            for (int i = 0; i < m_answerButtons.Count; i++)
                            {
                                if (m_currentES.currentSelectedGameObject == m_answerButtons[i].gameObject)
                                {
                                    m_selectedBtn = m_answerButtons[i].gameObject;
                                    m_selectedAnswerIndex = i;
                                    break;
                                }
                            }
                            EventSystem.current.SetSelectedGameObject(m_answerButtons[m_selectedAnswerIndex].gameObject);
                        }
                    }
                }

            }
        }

        public virtual void CheckRightInput()
        {
            if (m_state == TEXT_STATE.SHOWN)
            {
                if (m_canChooseNumber)
                {
                    if (m_currentES != null && m_currentES.currentSelectedGameObject != null)
                    {
                        m_currentES.SetSelectedGameObject(null);
    
                    }

                    m_currentNumber += 10;

                    if (m_currentNumber > m_parameters.y)
                    {
                        int v = (int)(m_currentNumber - m_parameters.y);
                        m_currentNumber = (int)m_parameters.x + v;
                    }
                    SetChooseNumberString();
                }
            }
        }

        public virtual void CheckLeftInput()
        {
            if (m_state == TEXT_STATE.SHOWN)
            {
                if (m_canChooseNumber)
                {
                    if (m_currentES != null && m_currentES.currentSelectedGameObject != null)
                    {
                        m_currentES.SetSelectedGameObject(null);
                        
                    }

                    m_currentNumber -= 10;

                    if (m_currentNumber < m_parameters.x)
                    {
                        int v = (int)Mathf.Abs(m_currentNumber - m_parameters.x);
                        m_currentNumber = (int)m_parameters.y - v;
                    }

                    SetChooseNumberString();
                }
            }
        }

        public virtual void CheckDownAnswerIndex()
        {
            if (m_state == TEXT_STATE.SHOWN)
            {
                if (m_numberOfAnswers > 0)
                {
                    m_selectedAnswerIndex++;
                    if (m_selectedAnswerIndex >= m_numberOfAnswers)
                    {
                        m_selectedAnswerIndex = 0;
                        EventSystem.current.SetSelectedGameObject(m_answerButtons[m_selectedAnswerIndex].gameObject);
                    }
                }
                else if (m_canChooseNumber)
                {
                    if (m_currentES != null && m_currentES.currentSelectedGameObject != null)
                    {
                        m_currentES.SetSelectedGameObject(null);
                        
                    }

                    m_currentNumber -= 1;

                    if (m_currentNumber < m_parameters.x)
                    {
                        int v = (int)Mathf.Abs(m_currentNumber - m_parameters.x);
                        m_currentNumber = (int)m_parameters.y - v;
                    }

                    SetChooseNumberString();
                }
            }
        }

        public virtual void CheckUpAnswerIndex()
        {
            if (m_state == TEXT_STATE.SHOWN)
            {
                if (m_numberOfAnswers > 0)
                {
                    m_selectedAnswerIndex--;
                    if (m_selectedAnswerIndex < 0)
                    {
                        m_selectedAnswerIndex = m_numberOfAnswers - 1;
                        EventSystem.current.SetSelectedGameObject(m_answerButtons[m_selectedAnswerIndex].gameObject);
                    }
                }
                else if (m_canChooseNumber)
                {
                    if (m_currentES != null && m_currentES.currentSelectedGameObject != null)
                    {
                        m_currentES.SetSelectedGameObject(null);
                    }

                    m_currentNumber += 1;
                    if (m_currentNumber > m_parameters.y)
                    {
                        int v = (int)(m_currentNumber - m_parameters.y);
                        m_currentNumber = (int)m_parameters.x + v;
                    }

                    SetChooseNumberString();
                }
            }
        }

        protected virtual void SetChooseNumberString()
        {
            if (m_use0InTen)
                m_chooseNumberText.text = VP_Utils.AddZerosToCharacterNumber(m_currentNumber);
            else
                m_chooseNumberText.text = m_currentNumber.ToString();
        }

        public virtual float AutoAnswerTimeProgress ()
        {
            return m_spentAutoTime / m_autoTimeToAnswer;
        }

        protected virtual void StartAutoAnswerTimer()
        {
            StopAutoAnswerTimer();
#if !USE_MORE_EFFECTIVE_COROUTINES
            m_autoAnswerCoroutine = StartCoroutine(WaitForAutoAnswer(m_autoTimeToAnswer));
#else
            m_autoAnswerCoroutine = Timing.RunCoroutine(WaitForAutoAnswer(m_autoTimeToAnswer));
#endif
        }

        protected virtual void StopAutoAnswerTimer()
        {
#if !USE_MORE_EFFECTIVE_COROUTINES
            if (m_autoAnswerCoroutine != null)
                StopCoroutine(m_autoAnswerCoroutine);
#else
            Timing.KillCoroutines(m_autoAnswerCoroutine);
#endif
        }

#if !USE_MORE_EFFECTIVE_COROUTINES
        protected virtual IEnumerator WaitForAutoAnswer(float _timeToAnswer)
        {
            m_spentAutoTime = 0f;

            while (m_spentAutoTime < _timeToAnswer)
            {
                m_spentAutoTime += Time.deltaTime;
                yield return null;
            }
            // if we don't answered, we answer the default one
            Answer(m_autoAnswer);
        }
#else
        protected virtual IEnumerator<float> WaitForAutoAnswer(float _timeToAnswer)
        {
            m_spentAutoTime = 0f;

            while (m_spentAutoTime < _timeToAnswer)
            {
                m_spentAutoTime += Time.deltaTime;
                yield return Timing.WaitForOneFrame;
            }
            // if we don't answered, we answer the default one
            Answer(m_autoAnswer);
        }
#endif
        protected override void StartAllListeners()
        {
            base.StartAllListeners();

            if (m_listenDefaultInputCallbacks != null)
            {
                if (m_listenDefaultInputCallbacks.ContainsKey(DIALOG_INPUT_CALLBACK.INTERACT) && m_listenDefaultInputCallbacks[DIALOG_INPUT_CALLBACK.INTERACT])
                {
                    VP_DialogManager.StartListeningToOnDialogInteract(PressedInteract);
                    if (m_listenToEventManager)
                        VP_EventManager.StartListening(VP_EventSetup.Input.INTERACT_BUTTON, PressedInteract);
                }

                if (m_listenDefaultInputCallbacks.ContainsKey(DIALOG_INPUT_CALLBACK.UP) && m_listenDefaultInputCallbacks[DIALOG_INPUT_CALLBACK.UP])
                {
                    VP_DialogManager.StartListeningToOnDialogUp(CheckUpAnswerIndex);
                    if (m_listenToEventManager)
                        VP_EventManager.StartListening(VP_EventSetup.Input.UP_ANSWER, CheckUpAnswerIndex);
                }

                if (m_listenDefaultInputCallbacks.ContainsKey(DIALOG_INPUT_CALLBACK.DOWN) && m_listenDefaultInputCallbacks[DIALOG_INPUT_CALLBACK.DOWN])
                {
                    VP_DialogManager.StartListeningToOnDialogDown(CheckDownAnswerIndex);
                    if (m_listenToEventManager)
                        VP_EventManager.StartListening(VP_EventSetup.Input.DOWN_ANSWER, CheckDownAnswerIndex);
                }

                if (m_listenDefaultInputCallbacks.ContainsKey(DIALOG_INPUT_CALLBACK.RIGHT) && m_listenDefaultInputCallbacks[DIALOG_INPUT_CALLBACK.RIGHT])
                {
                    VP_DialogManager.StartListeningToOnDialogRight(CheckRightInput);
                    if (m_listenToEventManager)
                        VP_EventManager.StartListening(VP_EventSetup.Input.RIGHT_ANSWER, CheckRightInput);
                }

                if (m_listenDefaultInputCallbacks.ContainsKey(DIALOG_INPUT_CALLBACK.LEFT) && m_listenDefaultInputCallbacks[DIALOG_INPUT_CALLBACK.LEFT])
                {
                    VP_DialogManager.StartListeningToOnDialogLeft(CheckLeftInput);
                    if (m_listenToEventManager)
                        VP_EventManager.StartListening(VP_EventSetup.Input.LEFT_ANSWER, CheckLeftInput);
                }

                if (m_listenDefaultInputCallbacks.ContainsKey(DIALOG_INPUT_CALLBACK.CANCEL) && m_listenDefaultInputCallbacks[DIALOG_INPUT_CALLBACK.CANCEL])
                {
                    VP_DialogManager.StartListeningToOnDialogCancel(CheckCancel);
                    if (m_listenToEventManager)
                        VP_EventManager.StartListening(VP_EventSetup.Input.CANCEL_BUTTON, CheckCancel);
                }
            }
     
            VP_DialogManager.StartListeningToOnExternalAnswerSelected(OnExternalAnswer);

        }

        protected override void StopAllListeners()
        {
            base.StopAllListeners();

            VP_EventManager.StopListening(VP_EventSetup.Input.INTERACT_BUTTON, PressedInteract);
            VP_EventManager.StopListening(VP_EventSetup.Input.UP_ANSWER, CheckUpAnswerIndex);
            VP_EventManager.StopListening(VP_EventSetup.Input.DOWN_ANSWER, CheckDownAnswerIndex);
            VP_EventManager.StopListening(VP_EventSetup.Input.RIGHT_ANSWER, CheckRightInput);
            VP_EventManager.StopListening(VP_EventSetup.Input.LEFT_ANSWER, CheckLeftInput);
            VP_EventManager.StopListening(VP_EventSetup.Input.CANCEL_BUTTON, CheckCancel);

            VP_DialogManager.StopListeningToOnDialogInteract(PressedInteract);
            VP_DialogManager.StopListeningToOnDialogUp(CheckUpAnswerIndex);
            VP_DialogManager.StopListeningToOnDialogDown(CheckDownAnswerIndex);
            VP_DialogManager.StopListeningToOnDialogRight(CheckRightInput);
            VP_DialogManager.StopListeningToOnDialogLeft(CheckLeftInput);
            VP_DialogManager.StopListeningToOnExternalAnswerSelected(OnExternalAnswer);
            VP_DialogManager.StopListeningToOnDialogCancel(CheckCancel);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

        }

        protected virtual void Reset()
        {
            m_startListeningTime = StartListenTime.OnEnable;
            m_stopListeningTime = StopListenTime.OnDisable;
        }

        protected virtual void OnExternalAnswer(int _answerIndex)
        {
            m_selectedAnswerIndex = _answerIndex;
            if (m_answerButtons.Count > 0)
            {
                Answer(m_answerButtons[m_selectedAnswerIndex].Index);
                return;
            }
        }

        protected virtual void ContinueSameDialog()
        {
            // Last sentence
            m_currPrintedChars = m_maxCharactersOrWordsNewLine * m_timesPerformedContinueDialog;
            
            m_iconForNext.gameObject.SetActive(false);
           
            if (m_trimPassedDialog)
            {
                var rst = m_mainText.text.Length - m_maxCharactersOrWordsNewLine;
                if (rst > 0)
                {
                    m_mainText.text = m_mainText.text.Substring(m_maxCharactersOrWordsNewLine, rst);
                }
                else
                {
                    m_needLastDisplay = true;
                }
             
                m_mainText.maxVisibleCharacters = 0;                
            }
            else
            {
                if (m_currPrintedChars >= m_textCharacters)
                {
                    m_needLastDisplay = true;
                }
                
            }

            m_waitingChars = 0;
            m_waitingForInputToContinueSameDialog = false;


            m_playSoundOnContinue = m_dialogData.m_soundOnContinue;

            if (m_playSoundOnContinue && m_canPlaySound)
            {
                PlayContinueAudio();
            }

        }

        public virtual void CheckCancel()
        {
            

            if ((m_waitingForAnswerAudio && ((!m_useAudioManager && m_source.isPlaying) || (m_useAudioManager && VP_AudioManager.Instance.VoiceSource.isPlaying))) ||
                (m_dialogData.m_waitForAudioEnd && ((!m_useAudioManager && m_source.isPlaying) || (m_useAudioManager && VP_AudioManager.Instance.VoiceSource.isPlaying))) ||
                (!m_dialogData.m_canSkipWithInput && !m_dialogData.m_waitForInput))
            {
                return;
            }

            // Fade Out if shoing msg
            if (m_state == TEXT_STATE.SHOWING)
            {
                if (m_needToContinueSameDialog)
                {
                    if (!m_waitingForInputToContinueSameDialog)
                    {
                        m_timesPerformedContinueDialog++;
                        m_currPrintedChars = m_maxCharactersOrWordsNewLine * m_timesPerformedContinueDialog;
                        m_waitingChars = m_maxCharactersOrWordsNewLine;
                        m_mainText.maxVisibleCharacters = ((!m_trimPassedDialog) ? (m_timesPerformedContinueDialog - 1) * m_maxCharactersOrWordsNewLine + m_waitingChars : m_waitingChars);
                        m_waitingForInputToContinueSameDialog = true;
                    }
                    else
                    {
                        ContinueSameDialog();
                    }

                    return;
                }

                if (m_dialogData.m_canSkipWithInput)
                {
                    EndDirectly();
                    return;
                }
            }
            else if (m_state == TEXT_STATE.SHOWN)
            {
                if (!m_canCancel)
                    return;

                if (m_needToShowAnswers)
                {
                    if (!m_textAndAnswers)
                        m_mainText.maxVisibleCharacters = 0;

                    ShowAnswers();
                    return;
                }
                
                if (m_canCancel && m_cancelIndex > 0 && m_answerButtons.Count > 0)
                {
                    Answer(m_cancelIndex);
                }
                else
                {
                    if (m_canChooseNumber)
                    {
                        CancelNumber();
                    }
                    else if (m_dialogData.m_waitForInput)
                    {
                        CheckNextDialog();
                    }
                }
            }
        }

        /// <summary>
        /// When pressing interact (if possible) we show the full message or change to the next
        /// </summary>
        public virtual void PressedInteract()
        {
            if (m_dialogData == null || (m_waitingForAnswerAudio && ((!m_useAudioManager && m_source && m_source.isPlaying) || (m_useAudioManager && VP_AudioManager.Instance.VoiceSource.isPlaying))) || 
                (m_dialogData.m_waitForAudioEnd && ((!m_useAudioManager && m_source.isPlaying) || (m_useAudioManager && VP_AudioManager.Instance.VoiceSource.isPlaying))) || 
                (!m_dialogData.m_canSkipWithInput && !m_dialogData.m_waitForInput))
            {
                return;
            }

	        //Debug.Log("Called pressed interact");

            // Fade Out if shoing msg
            if (m_state == TEXT_STATE.SHOWING)
            {
                if (m_needToContinueSameDialog)
                {
                    if (!m_waitingForInputToContinueSameDialog)
                    {
                        m_timesPerformedContinueDialog++;
                        m_currPrintedChars = m_maxCharactersOrWordsNewLine * m_timesPerformedContinueDialog;
                        m_waitingChars = m_maxCharactersOrWordsNewLine;
                        m_mainText.maxVisibleCharacters = ((!m_trimPassedDialog) ? (m_timesPerformedContinueDialog - 1) * m_maxCharactersOrWordsNewLine + m_waitingChars : m_waitingChars);
                        m_waitingForInputToContinueSameDialog = true;
                    }
                    else
                    {
                        ContinueSameDialog();
                    }

                    return;
                }

                if (m_dialogData.m_canSkipWithInput)
                {
                //	Debug.Log("End directly");
                    EndDirectly();
                    return;
                }
            }
            else if (m_state == TEXT_STATE.SHOWN)
            {
                if (m_needToShowAnswers)
                {
                    if (!m_textAndAnswers)
                        m_mainText.maxVisibleCharacters = 0;

                    ShowAnswers();
                    return;
                }

                if (m_answerButtons.Count > 0)
                {
                    Answer(m_answerButtons[m_selectedAnswerIndex].Index);
                }
                else
                {
                    if (m_canChooseNumber)
                    {
                        ChooseNumber(m_currentNumber);
                    }
                    else if (m_dialogData.m_waitForInput)
                    {
                        CheckNextDialog();
                    }
                }
            }
        }

        public virtual void Skip()
        {
            //Debug.Log("Next text");
            m_state = TEXT_STATE.HIDDEN;
            m_optionGroup.SetActive(false);
            m_iconForNext.gameObject.SetActive(false);
            m_dialogData.m_onDialogCompleteCallback?.Invoke();
	        OnDialogContinue?.Invoke();
            VP_DialogManager.Instance.ContinueText(m_dialogData.m_onDialogEndCallback);
        }

        /// <summary>
        /// Check the end of the dialog
        /// </summary>
        public virtual void CheckNextDialog()
        {
            m_playSoundOnContinue = m_dialogData.m_soundOnContinue;

            if (m_playSoundOnContinue && m_canPlaySound)
            {
                PlayContinueAudio();
            }

            if (m_numberOfAnswers == 0)
            {
                if (m_dialogData.m_fadeInOut)
                {
                    ChooseEndAnimation();
                }
                else
                {
                    NextText();
                }
            }
        }

        protected virtual IEnumerator WaitForAnswerAudioEnd(float _time, System.Action _callback)
        {
            float timer = 0f;

            while (timer < _time)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            if (_callback != null)
                _callback.Invoke();
        }

        /// <summary>
        /// Answer called from the buttons in the option group
        /// </summary>
        /// <param name="index"></param>
        public virtual void Answer(int index)
        {
            if (m_autoAnswer > 0)
            {
	            m_selectedAnswerIndex = index;
	            OnDialogContinue?.Invoke();
                VP_DialogManager.OnAutoAnswerTimeEndAction();
                StopAutoAnswerTimer();
            }
            
            m_playSoundOnContinue = m_dialogData.m_soundOnContinue;

            if (m_playSoundOnContinue && m_canPlaySound)
            {
                PlayContinueAudio();
            }

            AudioClip answerclip = m_answers[index]?.m_voiceClip;

            m_iconForNext.gameObject.SetActive(false);
            m_mainText.maxVisibleCharacters = 0;

            StartFadeOut(false);

            if (answerclip != null)
            {
                m_waitingForAnswerAudio = true;

                if (m_useAudioManager)
                    VP_AudioManager.Instance.PlayAudio(answerclip, VP_AudioSetup.AUDIO_TYPE.SFX, true, 1f, VP_AudioSetup.Dialog.ANSWER_CLIP, true);
                else
                    m_source.PlayOneShot(answerclip);

                if (m_answers[index].m_waitForVoiceClip)
                {
                    StartCoroutine(WaitForAnswerAudioEnd(answerclip.length+m_delayAfterAnswer, () =>
                    {
                        if (m_dialogData.m_onAnswerCallback != null)
                            m_dialogData.m_onAnswerCallback.Invoke(index);

	                    OnDialogContinue?.Invoke();
                        VP_DialogManager.Instance.Answer(index, m_answers[index].m_callback);
                    }));
                }
            }
            else
            {
                if (m_dialogData.m_onAnswerCallback != null)
                    m_dialogData.m_onAnswerCallback.Invoke(index);

	            OnDialogContinue?.Invoke();
                VP_DialogManager.Instance.Answer(index, m_answers[index].m_callback);
            }
        }

        public virtual void PlayContinueAudio()
        {
            AudioClip m_continueAudio = m_characterAudios.GetContinueSound();

            if (m_useAudioManager)
                VP_AudioManager.Instance.PlayOneShot(m_continueAudio, VP_AudioSetup.AUDIO_TYPE.SFX, 1f);
            else
                m_source.PlayOneShot(m_continueAudio);
        }

        public virtual void ChooseNumber(int _number)
        {
            m_playSoundOnContinue = m_dialogData.m_soundOnContinue;

            if (m_playSoundOnContinue && m_canPlaySound)
            {
                PlayContinueAudio();
            }

            m_iconForNext.gameObject.SetActive(false);
            m_mainText.maxVisibleCharacters = 0;

            StartFadeOut(false);

            if (m_dialogData.m_chooseNumberCallback != null)
                m_dialogData.m_chooseNumberCallback.Invoke(_number);

	        OnDialogContinue?.Invoke();
            VP_DialogManager.Instance.ChoseNumberFromMessage(_number);
        }

        public virtual void CancelNumber()
        {
            m_playSoundOnContinue = m_dialogData.m_soundOnContinue;

            if (m_playSoundOnContinue && m_canPlaySound)
            {
                PlayContinueAudio();
            }

            m_iconForNext.gameObject.SetActive(false);
            m_mainText.maxVisibleCharacters = 0;

            StartFadeOut(false);

            if (m_dialogData.m_cancelNumberCallback != null)
                m_dialogData.m_cancelNumberCallback.Invoke();

	        OnDialogContinue?.Invoke();
            VP_DialogManager.Instance.CancelNumberFromMessage();
        }

        /// <summary>
        /// Answer the current selection
        /// </summary>
        public virtual void AnswerCurrent()
        {
            Answer(m_answerButtons[m_selectedAnswerIndex].Index);
        }

        /// <summary>
        /// We choose the animation in the animator based on your selection
        /// </summary>
        protected virtual void ChooseInitAnimation()
        {
            switch (m_dialogAnimation)
            {
                case ANIMATION_TYPE.FADE_IN_OUT:
                    StartFadeIn();
                    break;
                case ANIMATION_TYPE.CUSTOM:
                    // your code goes here
                    break;
                case ANIMATION_TYPE.TWEEN:
#if DOTWEEN
                    if (m_doTweenAnimationIn)
                        m_doTweenAnimationIn.onComplete.AddListener(() => StartShowingCharacters());
                    else
                        StartFadeIn();
#else
                      StartFadeIn();
#endif
                    break;
            }
        }
        /// <summary>
        /// Choose the end of the animation
        /// </summary>
        protected virtual void ChooseEndAnimation()
        {
            switch (m_dialogAnimation)
            {
                case ANIMATION_TYPE.FADE_IN_OUT:
                    StartFadeOut();
                    break;
                case ANIMATION_TYPE.CUSTOM:
                    // your code goes here
                    break;
                case ANIMATION_TYPE.TWEEN:
#if DOTWEEN
                    if (m_doTweenAnimationOut)
                        m_doTweenAnimationOut.onComplete.AddListener(() => NextText());
                    else
                        StartFadeOut();
#else
                   StartFadeOut();
#endif
                    break;
            }
        }
        /// <summary>
        /// Set the next text
        /// </summary>
        public virtual void NextText()
        {
#if DOTWEEN
            if (m_dialogAnimation == ANIMATION_TYPE.TWEEN)
            {
                m_doTweenAnimationOut.onComplete.RemoveListener(() => NextText());
            }
#endif

            //Debug.Log("Next text");
            m_state = TEXT_STATE.HIDDEN;
            m_optionGroup.SetActive(false);
	        m_iconForNext.gameObject.SetActive(false);
	        OnDialogContinue?.Invoke();
	        if (m_dialogData != null)
	        {
		        if (m_dialogData.m_onDialogCompleteCallback != null)
			        m_dialogData.m_onDialogCompleteCallback.Invoke();			 
	        }
	        
	        VP_DialogManager.Instance.ContinueText(m_dialogData.m_onDialogEndCallback);
        }

        public virtual void DirectMessage(string _msg)
        {
            m_mainText.text = _msg;
            StartShowingCharacters(0.1f);
        }

        protected virtual void SetGraphicsAlpha(float _alpha)
        {
            if (m_dialogCanvasGroup)
                m_dialogCanvasGroup.alpha = _alpha;

            foreach (int ids in m_graphicsToFadeManually.Keys)
            {
                Graphic graphic = m_graphicsToFadeManually[ids]?.m_graphic;
                if (!graphic)
                    continue;

                Color cGraphicc = graphic.color;
                cGraphicc.a = _alpha;
                graphic.color = cGraphicc;
            }
        }

        protected virtual void ChangeEventSystemHandlingObject()
        {
            m_currentES.SetSelectedGameObject(m_previousSelection);
            VP_DialogManager.StopListeningToOnDialogComplete(ChangeEventSystemHandlingObject);
        }

        /// <summary>
        /// Method called for displaying new data 
        /// </summary>
        /// <param name="_newData"></param>
        public virtual void ShowDialog(VP_DialogMessageData _newData)
        {
            StopAllCoroutines();

            if (string.IsNullOrEmpty(_newData.m_text))
            {
	            _newData.m_text = "NO TEXT";
            }

            if (m_canPlaySound && m_characterAudios == null)
            {
                m_characterAudios = Resources.Load<VP_CharacterAudioDatabase>(m_letterSoundPath);
            }

            if (m_changeBg && m_backgrounds == null)
            {
                m_backgrounds = Resources.Load<VP_DialogBackgrounds>(m_backgroundsdPath);
            }

            m_currentES = EventSystem.current;
            if (m_eventSystemHandling && _newData.m_numberOfAnswers == 0 && !_newData.m_chooseNumber)
            {
                m_previousSelection = m_currentES.currentSelectedGameObject;
                m_currentES.SetSelectedGameObject(m_mainButton);
                VP_DialogManager.StopListeningToOnDialogComplete(ChangeEventSystemHandlingObject);
            }

            if (m_fadeAnimationType == FADE_ANIMATION_TYPE.ANIMATOR)
            {
                m_animator.SetTrigger(VP_DialogSetup.Animations.IDLE_SHOWN);
            }
            else if (m_fadeAnimationType == FADE_ANIMATION_TYPE.DOTWEEN || m_fadeAnimationType == FADE_ANIMATION_TYPE.MANUAL_FADING)
            {
                SetGraphicsAlpha(0f);
            }
	        
            this.m_dialogData = _newData;
            
            m_playSoundOnContinue = _newData.m_soundOnContinue;
            m_noInputDialogWaitTime = _newData.m_automaticScreenTime;
            m_textSpeed = m_setSpeed ? _newData.m_textSpeed : m_defaultSpeed;
            m_showingText = "";
            m_fadingGraphics.Clear();
            m_fadingCanvasGroups.Clear();
            m_autoTimeToAnswer = _newData.m_timeForAnswer;
            m_waitingChars = 0;
            m_timesPerformedContinueDialog = 0;
            m_state = TEXT_STATE.HIDDEN;
	        m_chooseNumber = _newData.m_chooseNumber;
	        if (m_chooseNumberGO != null)
            	m_chooseNumberGO?.SetActive(false);
            m_canChooseNumber = false;
            m_canCancel = _newData.m_canCancel;

#if USE_TEXT_ANIMATOR
     
            if (m_useTextAnimator)
            {
                if (m_textAnimatorPlayer != null)
                {
                    m_textAnimatorPlayer?.gameObject.SetActive(m_useTextAnimator);
                    m_textAnimatorPlayer?.ShowText(m_showingText);
                }
                else
                {
                    m_useTextAnimator = false;
                }
            }

                   m_mainText.gameObject.SetActive(!m_useTextAnimator);
#else
            m_mainText.gameObject.SetActive(true);
#endif
            if (m_chooseNumber)
            {
                Vector3 p = _newData.m_numberParameters;
                m_parameters = new Vector2(p.x, p.y);
                m_currentNumber = (int)p.z;

                if (m_currentNumber > m_parameters.y)
                    m_currentNumber = (int)m_parameters.y;
                else if (m_currentNumber < m_parameters.x)
                    m_currentNumber = (int)m_parameters.x;

                SetChooseNumberString();
            }
            else
            {
                m_currentNumber = 0;
                m_parameters = Vector2.zero;
            }

            m_currPrintedChars = 0;
            m_waitingForInputToContinueSameDialog = false;
            m_characterData = _newData.m_characterData;

            if (_newData.m_overrideColor)
                m_mainText.color = _newData.m_color;

            if (_newData.m_font != null)
                m_mainText.font = _newData.m_font;
            
           
            m_optionGroup.SetActive(false);

            m_mainButton.SetActive(true);

            foreach (Transform trs in m_buttonTrs)
            {
                Destroy(trs.gameObject);
            }

            if (m_answerButtons == null)
                m_answerButtons = new List<VP_DialogMessageButton>();

            if (m_answers == null)
                m_answers = new List<VP_Dialog.Answer>();

            m_emotionList = new List<KeyValuePair<int, KeyValuePair<EMOTION, string>>>();

            m_autoAnswer = _newData.m_autoAnswer;
            m_answerButtons.Clear();
            m_answers.Clear();
            m_numberOfAnswers = 0;
            m_textAndAnswers = _newData.m_answersAtSameTime;
            m_needToShowAnswers = false;
            m_waitingForAnswerAudio = false;

            m_iconForNext.gameObject.SetActive(m_useContinueIcon);

            m_timeToFadeWithDoTween = _newData.m_fadeDuration;

            if (_newData.m_answers != null && _newData.m_answers.Count > 0)
            {
                int ansidx = 0;
                m_selectedAnswerIndex = 0;
                foreach (VP_Dialog.Answer _answer in _newData.m_answers)
                {
                    m_answers.Add(_answer);

                    if (_answer.m_selectIfCancel)
                    {
                        if (!m_canCancel)
                            m_canCancel = true;

                        m_cancelIndex = ansidx;
                    }

                    if (_answer.m_visible)
                    {
                        GameObject go = Instantiate(m_buttonPrefab, m_buttonTrs);
                        VP_DialogMessageButton btn = go.GetComponent<VP_DialogMessageButton>();
                        bool translate = _answer.m_translate;
                        string txt = translate ? VP_LocalizationManager.GetText(_answer.m_answerKey) : _answer.m_answerKey;
                        btn.SetData(this, ansidx, txt);
                        m_answerButtons.Add(btn);
                        m_numberOfAnswers++;
                        ansidx++;
                    }
                }
            }

            if (m_characterData != null)
            {
            	
#if !USE_ANIMANCER            	
	            m_hasAnimations = m_useMugshotAnimation &&(m_characterData.animator != null && !string.IsNullOrEmpty(m_characterData.speakingAnim));
                m_mugshotAnimator.runtimeAnimatorController = (m_hasAnimations) ? m_characterData.animator : null;
#else
	            if (m_useAnimancer)
	            {
	            	m_hasAnimations = m_useMugshotAnimation && (m_characterData.m_animationResources != null && !string.IsNullOrEmpty(m_characterData.speakingAnim));
		            m_animationResources = (m_hasAnimations) ? m_characterData.m_animationResources : null;
	            }
	            else
	            {
		            m_hasAnimations = m_useMugshotAnimation && (m_characterData.animator != null && !string.IsNullOrEmpty(m_characterData.speakingAnim));
		            m_mugshotAnimator.runtimeAnimatorController = (m_hasAnimations) ? m_characterData.animator : null;
	            }
#endif
                if (m_characterData.m_overrideTextColor)
                    m_mainText.color = m_characterData.m_characterDefaultTextColor;

                if (m_characterData.m_overrideFontSize && m_changeFontSize)
                    m_mainText.fontSize = m_characterData.m_characterDefaultFontSize;

                if (m_characterData.m_overrideTextNameColor)
                    m_characterNameText.color = m_characterData.m_characterDefaultNameTextColor;

                string charaName = m_discoverName && VP_DialogManager.Instance.GlobalVariables.GetBoolVariableValue(m_discoverVariableName) ? m_characterData.characterName : " ???";
                m_characterNameText.text = VP_LocalizationManager.CanTranslate(charaName) ? VP_LocalizationManager.GetText(charaName) : charaName;
                m_sprite.sprite = m_characterData.image;
                m_regularDelay = m_characterData.m_timeBetweenCharacters;
            }
            else
            {
                m_characterNameText.text = "???";
                m_sprite.sprite = null;
                m_hasAnimations = false;
#if !USE_ANIMANCER
                if (m_mugshotAnimator && m_mugshotAnimator.isInitialized)
	            m_mugshotAnimator.runtimeAnimatorController = null;
#else
	            if (!m_useAnimancer)
	            {
		            if (m_mugshotAnimator && m_mugshotAnimator.isInitialized)
			            m_mugshotAnimator.runtimeAnimatorController = null;
	            }
	            else
	            {
	            	m_animationResources = null;
	            }
#endif
                
                m_regularDelay = 0.1f;
            }

            m_iconForNext.gameObject.SetActive(false);
            VP_DialogPositionData positionData = _newData.m_positionData;
            if (positionData != null)
            {
                if (m_setPositionData)
                {
                    positionData.Trigger();

                    if (positionData.m_setPosition)
                    {

                        float x_pos = 0f;
                        float y_pos = 0f;

                        if (positionData.m_useLocalPosition)
                        {
                            x_pos = positionData.m_position.x;
                            y_pos = positionData.m_position.y;
                        }
                        else
                        {
                            x_pos = positionData.m_dialogLeftRight.x;
                            y_pos = positionData.m_dialogtopBottom.y;
                        }

                        if (m_setInsideScreen)
                        {
                            if (x_pos < 10f)
                                x_pos = 10f;
                            else if (x_pos + (m_bg.sprite.rect.width / 2) > Screen.width)
                                x_pos = 10f;

                            if (y_pos < 0f)
                                y_pos = 0f;
                        }

                        this.transform.localPosition = new Vector3(x_pos, y_pos);
                    }

                    if (positionData.m_setBGPosition)
                    {
                        VP_Utils.Math.SetRectValues(m_bg.rectTransform, positionData.m_bgLeftRight, positionData.m_bgTopBottom);
                    }

                    if (positionData.m_changeRotation)
                    {
                        this.transform.localRotation = Quaternion.Euler(positionData.m_rotation.x, positionData.m_rotation.y, positionData.m_rotation.z);
                    }

                    if (positionData.m_changeScale)
                    {
                        this.transform.localScale = positionData.m_scale;
                    }

                    if (positionData.m_changeZ)
                    {
                        this.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y, positionData.m_posZ);
                    }
                }

                if (m_useIconArrow)
                {
                    m_iconArrow.gameObject.SetActive(true);
                    if (m_setIconArrowPosition)
                        m_iconArrow.transform.position = new Vector2(_newData.m_positionData.m_arrowIconPosition.x, m_iconArrow.transform.position.y);
                }
                else
                {
                    m_iconArrow.gameObject.SetActive(false);
                }

                switch (_newData.m_dialogType)
                {
                    case DIALOG_TYPE.MUGSHOT:
                        m_characterNameText.gameObject.SetActive(false);
                        m_sprite.gameObject.SetActive(true);
                        m_bg.enabled = true;
                        m_bg.gameObject.SetActive(true);
                        if (m_setPositionData && positionData && positionData.m_setTextPosition)
                            VP_Utils.Math.SetRectValues(m_mainText.rectTransform, positionData.m_textLeftRightMugshot, positionData.m_textTopBottomMugshot);
                        break;
                    case DIALOG_TYPE.MUGSHOT_AND_NAME:
                        m_characterNameText.gameObject.SetActive(true);
                        m_sprite.gameObject.SetActive(true);
                        m_bg.enabled = true;
                        m_bg.gameObject.SetActive(true);
                        if (m_setPositionData && positionData && positionData.m_setTextPosition)
                            VP_Utils.Math.SetRectValues(m_mainText.rectTransform, positionData.m_textLeftRightMugshot, positionData.m_textTopBottomMugshot);
                        break;
                    case DIALOG_TYPE.REGULAR:
                        m_characterNameText.gameObject.SetActive(false);
                        m_sprite.gameObject.SetActive(false);
                        m_bg.enabled = true;
                        m_bg.gameObject.SetActive(true);
                        if (m_setPositionData && positionData && positionData.m_setTextPosition)
                            VP_Utils.Math.SetRectValues(m_mainText.rectTransform, positionData.m_textLeftRightRegular, positionData.m_textTopBottomRegular);
                        break;
                    case DIALOG_TYPE.REGULAR_NAME:
                        m_characterNameText.gameObject.SetActive(true);
                        m_sprite.gameObject.SetActive(false);
                        m_bg.enabled = true;
                        m_bg.gameObject.SetActive(true);
                        if (m_setPositionData && positionData && positionData.m_setTextPosition)
                            VP_Utils.Math.SetRectValues(m_mainText.rectTransform, positionData.m_textLeftRightRegular, positionData.m_textTopBottomRegular);
                        break;
                    case DIALOG_TYPE.NONE:
                        m_bg.enabled = false;
                        m_characterNameText.gameObject.SetActive(false);
                        m_sprite.gameObject.SetActive(false);
                        break;
                }
            }
            else
            {
                if (m_setPositionData)
                {
                    if (m_dialogRect)
                        VP_Utils.Math.SetRectValues(m_dialogRect, Vector2.zero, Vector2.zero);
                    else
                        transform.localPosition = Vector2.zero;
                }

                switch (_newData.m_dialogType)
                {
                    case DIALOG_TYPE.MUGSHOT:
                        m_characterNameText.gameObject.SetActive(false);
                        m_sprite.gameObject.SetActive(true);
                        m_bg.gameObject.SetActive(true);
                        break;
                    case DIALOG_TYPE.MUGSHOT_AND_NAME:
                        m_characterNameText.gameObject.SetActive(true);
                        m_sprite.gameObject.SetActive(true);
                        m_bg.gameObject.SetActive(true);
                        break;
                    case DIALOG_TYPE.REGULAR:
                        m_characterNameText.gameObject.SetActive(false);
                        m_sprite.gameObject.SetActive(false);
                        m_bg.gameObject.SetActive(true);
                        break;
                    case DIALOG_TYPE.REGULAR_NAME:
                        m_characterNameText.gameObject.SetActive(true);
                        m_sprite.gameObject.SetActive(false);
                        m_bg.gameObject.SetActive(true);
                        break;
                    case DIALOG_TYPE.NONE:
                        m_characterNameText.gameObject.SetActive(false);
                        m_sprite.gameObject.SetActive(false);
                        break;
                }
            }

            if (m_changeBg)
            {
                if (_newData.m_dialogType != DIALOG_TYPE.NONE)
                    m_bg.sprite = m_backgrounds.GetBackground(_newData.m_dialogType);
            }

            m_textToShow = _newData.m_text;

            m_showingText = m_textToShow;
            m_showingText = VP_Utils.DialogUtils.RemoveAllTags(m_textToShow);
            m_mainText.text = VP_Utils.DialogUtils.RemoveCustomTags(m_textToShow);
            m_mainText.maxVisibleCharacters = _newData.m_showDirectly ? m_showingText.Length : 0;

            ProcessCustomTags(m_textToShow);
            m_textCharacters = m_showingText.Length;

            if (m_dialogData.m_fadeInOut)
            {
                StartFadeIn();
            }
            else
            {
                if (_newData.m_dialogType != DIALOG_TYPE.NONE)
                {
                    if (m_fadeAnimationType == FADE_ANIMATION_TYPE.ANIMATOR)
                    {
                        m_animator.SetTrigger(VP_DialogSetup.Animations.SHOW_DIRECTLY);
                    }
                }

                StartShowingCharacters(0.1f);
            }
        }
        
	    public virtual void DoFade(Graphic _graphicToFade, float _origin, float _end, float _duration = 1f, System.Action _callback = null)
	    {
	    	if (!m_fadingGraphics.Contains(_graphicToFade))
	    	{
	    		m_fadingGraphics.Add(_graphicToFade);
		    	StartCoroutine(DoFadeCoroutine(_graphicToFade, _origin, _end, _duration, _callback));
	    	}
	  
	    }

        /// <summary>
        /// If you want fading without using DoTween Pro
        /// </summary>
        /// <param name="_graphicToFade"></param>
        /// <returns></returns>
        protected virtual IEnumerator DoFadeCoroutine(Graphic _graphicToFade, float _origin, float _end, float _duration = 1f, System.Action _callback = null)
	    {
	    	float alpha = _origin;
	    	float normalizedTime = _origin;
            Color c = _graphicToFade.color;

            for (float t=0f;t<_duration;t+=Time.deltaTime) 
	    	{
		    	normalizedTime = t / _duration;
		    		
		    	alpha = Mathf.Lerp(_origin, _end, normalizedTime);
		    		
		    	c.a = alpha;
                _graphicToFade.color = c;

                yield return null;
	    	}
	    	    	
            if (m_fadingGraphics.Contains(_graphicToFade))
		        m_fadingGraphics.Remove(_graphicToFade);

            c.a = alpha;
            _graphicToFade.color = c;

            if (_callback != null)
			    _callback.Invoke();
	    }

        public virtual void DoFade(CanvasGroup _canvasGroup, float _origin, float _end, float _duration = 1f, System.Action _callback = null)
        {
            if (!m_fadingCanvasGroups.Contains(_canvasGroup))
            {
                m_fadingCanvasGroups.Add(_canvasGroup);
                StartCoroutine(DoFadeCoroutine(_canvasGroup, _origin, _end, _duration, _callback));
            }

        }

        /// <summary>
        /// If you want fading without using DoTween Pro
        /// </summary>
        /// <param name="_graphicToFade"></param>
        /// <returns></returns>
        protected virtual IEnumerator DoFadeCoroutine(CanvasGroup _canvasGroup, float _origin, float _end, float _duration = 1f, System.Action _callback = null)
        {
            float alpha = _origin;
            float normalizedTime = _origin;

            for (float t = 0f; t < _duration; t += Time.deltaTime)
            {
                normalizedTime = t / _duration;

                alpha = Mathf.Lerp(_origin, _end, normalizedTime);

                _canvasGroup.alpha = alpha;
                yield return null;
            }

            // In case you manually clear this
            if (m_fadingCanvasGroups.Contains(_canvasGroup))
                m_fadingCanvasGroups.Remove(_canvasGroup);

            _canvasGroup.alpha = _end;

            if (_callback != null)
                _callback.Invoke();
        }

        
        /// <summary>
        /// End directly and show the text without character by character
        /// </summary>
        protected virtual void EndDirectly()
        {
#if !USE_MORE_EFFECTIVE_COROUTINES
            if (m_displayCoroutine != null)
                StopCoroutine(m_displayCoroutine);
#else
            Timing.KillCoroutines(m_displayCoroutine);
#endif
            if (m_needToContinueSameDialog && m_maxCharactersOrWordsNewLine == 0)
                CalculateMaxCharactersInLine();

            if (!m_needToContinueSameDialog)
                m_mainText.maxVisibleCharacters = m_showingText.Length;
            else
                m_mainText.maxVisibleCharacters = m_maxCharactersOrWordsNewLine;

            m_state = TEXT_STATE.SHOWN;

            if (m_useContinueIcon)
                m_iconForNext.gameObject.SetActive(true);

            this.UpdateMeshAndAnims();

            AudioClip m_characterAudio = m_characterAudios.GetDefaultCharacter();

            bool playerNeedsAudio = (m_characterData == null || (m_characterData.m_dialogVoiceClip == null && m_characterData.m_dialogVoiceType != DIALOG_VOICE_TYPE.NO_SOUND));
            bool useDefaultCharacterAudio = (m_playDefaultSoundOnNoCharacter && m_characterAudio != null && playerNeedsAudio);
            AudioClip m_clip;

            if (useDefaultCharacterAudio)
            {
                m_clip = m_characterAudio;
            }
            else if ((m_characterData != null && (m_characterData.m_dialogVoiceClip != null && m_characterData.m_dialogVoiceType != DIALOG_VOICE_TYPE.NO_SOUND)))
            {
                m_clip = m_characterData.m_dialogVoiceClip;
            }
            else
            {
                m_clip = null;
            }

            if (m_clip)
            {
                if (m_useAudioManager)
                    VP_AudioManager.Instance.PlayOneShot(m_clip, VP_AudioSetup.AUDIO_TYPE.SFX, 1f);
                else
                    m_source.PlayOneShot(m_clip);
            }
            
            if (m_dialogData.m_clipDialogue != null)
            {
                if (m_useAudioManager)
                {
                    VP_AudioManager.Instance.PlayAudio(m_dialogData.m_clipDialogue, VP_AudioSetup.AUDIO_TYPE.VOICE, true, 1f, VP_AudioSetup.Dialog.DIALOGUE_CLIP, true);
                }
                else
                {
                    m_source.PlayOneShot(m_dialogData.m_clipDialogue);
                }
            }

            CheckEndDialog();
        }
        /// <summary>
        /// Start fade in(default anim)
        /// </summary>
        public virtual void StartFadeIn()
        {
            m_state = TEXT_STATE.FADING_ON;
            m_iconForNext.gameObject.SetActive(false);
            if (m_fadeAnimationType == FADE_ANIMATION_TYPE.ANIMATOR)
            {
                m_animator.SetTrigger(VP_DialogSetup.Animations.FADE_IN);
                m_animator.speed = m_animatorOriginalSpeed * m_dialogData.m_fadeDuration;
            }
            else if (m_fadeAnimationType == FADE_ANIMATION_TYPE.CUSTOM)
            {
                // TODO: Your custom fade in
            }
            else if (m_fadeAnimationType == FADE_ANIMATION_TYPE.DOTWEEN)
            {
#if DOTWEEN
                // todo adjust
                if (m_dialogCanvasGroup)
                {
                    m_dialogCanvasGroup.DOFade(1f, m_timeToFadeWithDoTween).OnComplete(() => { StartShowingCharacters(0.1f); });

                    foreach (int ids in m_graphicsToFadeManually.Keys)
                    {
                        Graphic graphic = m_graphicsToFadeManually[ids].m_graphic;

                        if (!graphic)
                            continue;

                        var time = m_useSameFadeTimeToAllGraphics ? m_timeToFadeWithDoTween : m_graphicsToFadeManually[ids].m_time;
                        graphic.DOFade(1f, time);
                    }

                }
                else
                {
                    
                    foreach (int ids in m_graphicsToFadeManually.Keys)
                    {
                        Graphic graphic = m_graphicsToFadeManually[ids].m_graphic;

                        if (!graphic)
                            continue;

                        var time = m_useSameFadeTimeToAllGraphics ? m_timeToFadeWithDoTween : m_graphicsToFadeManually[ids].m_time;
                        graphic.DOFade(1f, time);
                    }
                    StartCoroutine(GenericWait(m_timeToFadeWithDoTween, () => { StartShowingCharacters(); }));
                    //Invoke("StartShowingCharacters", m_timeToFadeWithDoTween);
                }


#endif
            }
            else if (m_fadeAnimationType == FADE_ANIMATION_TYPE.MANUAL_FADING)
            {
            	// todo adjust
	            if (m_dialogCanvasGroup)
	            {
	            	DoFade(m_dialogCanvasGroup,0f, 1f, m_timeToFadeWithDoTween, ()=>
	            	{
	            		StartShowingCharacters(0.1f);
	            	});

		            foreach (int ids in m_graphicsToFadeManually.Keys)
		            {
			            Graphic graphic = m_graphicsToFadeManually[ids].m_graphic;

			            if (!graphic)
				            continue;

			            var time = m_useSameFadeTimeToAllGraphics ? m_timeToFadeWithDoTween : m_graphicsToFadeManually[ids].m_time;
			            DoFade(graphic, 0f, 1f, time);
		            }

	            }
	            else
	            {
                    
		            foreach (int ids in m_graphicsToFadeManually.Keys)
		            {
			            Graphic graphic = m_graphicsToFadeManually[ids].m_graphic;

			            if (!graphic)
				            continue;

			            var time = m_useSameFadeTimeToAllGraphics ? m_timeToFadeWithDoTween : m_graphicsToFadeManually[ids].m_time;
			            DoFade(graphic, 0f, 1f, time);
		            }

                    StartCoroutine(GenericWait(m_timeToFadeWithDoTween, ()=> { StartShowingCharacters(); }));
		            //Invoke("StartShowingCharacters", m_timeToFadeWithDoTween);
	            }

            }
            m_optionGroup.SetActive(false);
        }

        protected virtual IEnumerator GenericWait(float _time = 2f, System.Action _callback = null)
        {
            float timer = 0f;
            while(timer < _time)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            if (_callback != null)
            {
                _callback.Invoke();
            }
        }

        /// <summary>
        /// Start fadeout (default anim)
        /// </summary>
        public virtual void StartFadeOut(bool _nextText = true)
        {
            m_state = TEXT_STATE.FADING_OUT;
            m_iconForNext.gameObject.SetActive(false);

            m_optionGroup.SetActive(false);

            if (m_fadeAnimationType == FADE_ANIMATION_TYPE.ANIMATOR)
            {
                m_animator.SetTrigger(VP_DialogSetup.Animations.FADE_OUT);
                m_animator.speed = m_animatorOriginalSpeed * m_dialogData.m_fadeDuration;
            }
            else if (m_fadeAnimationType == FADE_ANIMATION_TYPE.CUSTOM)
            {
                // TODO: Your custom fade out
            }
            else if (m_fadeAnimationType == FADE_ANIMATION_TYPE.DOTWEEN)
            {
#if DOTWEEN
                if (m_dialogCanvasGroup)
                {
                    if (_nextText)
                        m_dialogCanvasGroup.DOFade(0f, m_timeToFadeWithDoTween).OnComplete(NextText);
                    else
                        m_dialogCanvasGroup.DOFade(0f, m_timeToFadeWithDoTween);

                    foreach (int ids in m_graphicsToFadeManually.Keys)
                    {
                        Graphic graphic = m_graphicsToFadeManually[ids].m_graphic;

                        if (!graphic)
                            continue;

                        var time = m_useSameFadeTimeToAllGraphics ? m_timeToFadeWithDoTween : m_graphicsToFadeManually[ids].m_time;
                        graphic.DOFade(0f, time);
                    }
                }
                else
                {
                    
                    foreach (int ids in m_graphicsToFadeManually.Keys)
                    {
                        Graphic graphic = m_graphicsToFadeManually[ids].m_graphic;

                        if (!graphic)
                            continue;

                        var time = m_useSameFadeTimeToAllGraphics ? m_timeToFadeWithDoTween : m_graphicsToFadeManually[ids].m_time;
                        graphic.DOFade(0f, time);
                    }
                    if (_nextText)
                        StartCoroutine(GenericWait(m_timeToFadeWithDoTween, () => { NextText(); }));//Invoke("NextText", m_timeToFadeWithDoTween);
                }
#endif
            }
            else if (m_fadeAnimationType == FADE_ANIMATION_TYPE.MANUAL_FADING)
            {
            	if (m_dialogCanvasGroup)
            	{
	            	if (_nextText)
		            	DoFade(m_dialogCanvasGroup, m_dialogCanvasGroup.alpha, 0f, m_timeToFadeWithDoTween,NextText);
	            	else
		            	DoFade(m_dialogCanvasGroup, m_dialogCanvasGroup.alpha, 0f, m_timeToFadeWithDoTween);

	            	foreach (int ids in m_graphicsToFadeManually.Keys)
	            	{
		            	Graphic graphic = m_graphicsToFadeManually[ids].m_graphic;

		            	if (!graphic)
			            	continue;

		            	var time = m_useSameFadeTimeToAllGraphics ? m_timeToFadeWithDoTween : m_graphicsToFadeManually[ids].m_time;
		            	DoFade(graphic, graphic.color.a, 0f, time);
	            	}
            	}
            	else
            	{                    
	            	foreach (int ids in m_graphicsToFadeManually.Keys)
	            	{
		            	Graphic graphic = m_graphicsToFadeManually[ids].m_graphic;

		            	if (!graphic)
			            	continue;

		            	var time = m_useSameFadeTimeToAllGraphics ? m_timeToFadeWithDoTween : m_graphicsToFadeManually[ids].m_time;
		            	DoFade(graphic, graphic.color.a, 0f, time);
	            	}
	            	if (_nextText)
                        StartCoroutine(GenericWait(m_timeToFadeWithDoTween, () => { NextText(); }));//Invoke("NextText", m_timeToFadeWithDoTween);
                }
            }
        }

        /// <summary>
        /// We start displaying character by character :D
        /// </summary>
        /// <param name="_timeBetweenChars"></param>
        public virtual void StartShowingCharacters(float _timeBetweenChars = 0.1f)
        {
            if (m_dialogData.m_onDialogStartCallback != null)
                m_dialogData.m_onDialogStartCallback.Invoke();

#if DOTWEEN
            if (m_dialogAnimation == ANIMATION_TYPE.TWEEN)
            {
                m_doTweenAnimationIn.onComplete.RemoveListener(() => StartShowingCharacters());
            }
#endif
                foreach (var anim in m_mainText.GetComponents<VP_DialogTextAnimation>())
            {
                anim.StopPlaying();
            }

            if (m_queueIfMaxCharactersOrWords)
            {
                m_waitingForInputToContinueSameDialog = false;

                if (m_maxCharactersOrWordsNewLine == 0)
                    CalculateMaxCharactersInLine();

                if (m_textCharacters > m_maxCharactersOrWordsNewLine)
                    m_needToContinueSameDialog = true;
            }

#if !USE_MORE_EFFECTIVE_COROUTINES
            if (m_displayCoroutine != null)
                StopCoroutine(m_displayCoroutine);
#else
            Timing.KillCoroutines(m_displayCoroutine);
#endif

            if (m_dialogData.m_showDirectly)
            {
                EndDirectly();
            }
            else
            {
#if USE_TEXT_ANIMATOR
                if (m_useTextAnimator && m_textAnimatorPlayer != null)
                {
                    m_textAnimatorPlayer.textAnimator.tmproText.font = m_mainText.font;
                    if (m_changeFontSize)
                        m_textAnimatorPlayer.textAnimator.tmproText.fontSize = m_mainText.fontSize;

                    m_textAnimatorPlayer.ShowText(VP_Utils.DialogUtils.RemoveCustomTags(m_mainText.text));
                    
                    m_textAnimatorPlayer.onTextShowed.AddListener(CheckEndDialog);
                    m_textAnimatorPlayer.onCharacterVisible.AddListener(CheckTextAnimatorCharacter);

   

                }
                else
                {
#if USE_MORE_EFFECTIVE_COROUTINES
                    m_displayCoroutine = Timing.RunCoroutine(DisplayTextByChars(_timeBetweenChars, VP_Utils.DialogUtils.RemoveAllTags(m_mainText.text)));
#else
                    m_displayCoroutine = StartCoroutine(DisplayTextByChars(_timeBetweenChars, VP_Utils.DialogUtils.RemoveAllTags(m_mainText.text)));
#endif
                }
#else
#if USE_MORE_EFFECTIVE_COROUTINES
                m_displayCoroutine = Timing.RunCoroutine(DisplayTextByChars(_timeBetweenChars, VP_Utils.DialogUtils.RemoveAllTags(m_mainText.text)));
#else
                m_displayCoroutine = StartCoroutine(DisplayTextByChars(_timeBetweenChars, VP_Utils.DialogUtils.RemoveAllTags(m_mainText.text)));
#endif
#endif
            }
        }

#if USE_TEXT_ANIMATOR
        protected virtual void CheckTextAnimatorCharacter(char _char)
        {
            if (m_playDefaultSoundOnNoCharacter)
                PlayCharacterAudio(_char);

            if (m_emotionList.Count > 0)
            {
                m_currPrintedChars++;
                CheckEmotions();
            }
        }
#endif

        protected virtual void PlayCharacterAudio(char _char)
        {
            if (m_canPlaySound)
            {
                AudioClip m_clip = null;

                int alphabetLength = System.Enum.GetNames(typeof(ALPHABET)).Length;
                bool playerNeedsAudio = (m_characterData == null || (m_characterData.m_dialogVoiceClip == null && m_characterData.m_dialogVoiceType != DIALOG_VOICE_TYPE.NO_SOUND));
                bool useDefaultCharacterAudio = (m_playDefaultSoundOnNoCharacter && playerNeedsAudio);
                if (useDefaultCharacterAudio)
                {
                    m_clip = m_characterAudios.GetDefaultCharacter();
                }
                else if ((m_characterData != null && (m_characterData.m_dialogVoiceClip != null && m_characterData.m_dialogVoiceType != DIALOG_VOICE_TYPE.NO_SOUND)))
                {
                    m_clip = m_characterData.m_dialogVoiceClip;
                }
                else
                {
                    m_clip = null;
                }

                m_counterTextAnimator++;

                if (m_counterTextAnimator >= m_thresholdTextAnimator)
                {
                    m_counterTextAnimator = 0;
                    if (m_voiceType == DIALOG_VOICE_TYPE.SOUND_EVERY_CHARACTER && m_clip != null)
                    {
                        if (_char != ' ')
                        {
                            if (!m_useAudioManager)
                                m_letterSource.PlayOneShot(m_clip);
                            else
                                VP_AudioManager.PlayOneShot(m_clip);
                        }
                    }
                    else if (m_voiceType == DIALOG_VOICE_TYPE.SOUND_EVERY_CHARACTER_DIFFERENT_SOUND || m_voiceType == DIALOG_VOICE_TYPE.SOUND_VOCALS)
                    {
                        if (_char != ' ')
                        {
                            if (!m_useAudioManager)
                                m_letterSource.PlayOneShot(useDefaultCharacterAudio || m_characterData == null ? m_clip : m_characterAudios.GetLetterSound(_char, m_characterData.gender));
                            else
                                VP_AudioManager.PlayOneShot(useDefaultCharacterAudio || m_characterData == null ? m_clip : m_characterAudios.GetLetterSound(_char, m_characterData.gender));
                        }


                    }                    
                    else if (m_voiceType == DIALOG_VOICE_TYPE.SOUND_EVERY_WORD && m_clip != null)
                    {
                        if (_char == ' ')
                        {
                            if (!m_useAudioManager)
                                m_letterSource.PlayOneShot(useDefaultCharacterAudio || m_characterData == null ? m_clip : m_characterAudios.SpecialCharacter(m_characterData.gender));
                            else
                                VP_AudioManager.PlayOneShot(useDefaultCharacterAudio || m_characterData == null ? m_clip : m_characterAudios.SpecialCharacter(m_characterData.gender));
                        }
                    }
                    else
                    {
                        if (_char != ' ')
                        {
                            if (!m_useAudioManager)
                                m_letterSource.PlayOneShot(m_clip);
                            else
                                VP_AudioManager.PlayOneShot(m_clip);
                        }
                    }
                }

            }
        }

        /// <summary>
        /// Calculate max characters in line (in case is 0)
        /// </summary>
        public void CalculateMaxCharactersInLine()
        {
            m_maxCharactersOrWordsNewLine = (int)(m_mainText.fontSize * 0.5f + m_mainText.lineSpacing);
        }

        /// <summary>
        /// Text coroutine
        /// </summary>
        /// <param name="timeBetweenChars"></param>
        /// <returns></returns>
#if !USE_MORE_EFFECTIVE_COROUTINES
        protected virtual IEnumerator DisplayTextByChars(float timeBetweenChars, string taglessText)
#else
        protected virtual IEnumerator<float> DisplayTextByChars(float timeBetweenChars, string taglessText)
#endif
        {
            m_state = TEXT_STATE.SHOWING;
           
            //Debug.Log("Line Spacing: "+m_mainText.lineSpacing + " and font size:"+m_mainText.fontSize );
            AudioClip m_clip = null;
            bool playerNeedsAudio = (m_characterData == null || (m_characterData.m_dialogVoiceClip == null && m_characterData.m_dialogVoiceType != DIALOG_VOICE_TYPE.NO_SOUND));
            bool useDefaultCharacterAudio = (m_playDefaultSoundOnNoCharacter && playerNeedsAudio);
            int alphabetLength = System.Enum.GetNames(typeof(ALPHABET)).Length;

            if (useDefaultCharacterAudio)
            {
                m_clip = m_characterAudios.GetDefaultCharacter();
            }
            else if ((m_characterData != null && (m_characterData.m_dialogVoiceClip != null && m_characterData.m_dialogVoiceType != DIALOG_VOICE_TYPE.NO_SOUND)))
            {
                m_clip = m_characterData.m_dialogVoiceClip;
            }
            else
            {
                m_clip = null;
            }
            
            if (m_dialogData.m_clipDialogue != null)
            {
                if (m_useAudioManager)
                {
                    VP_AudioManager.Instance.PlayAudio(m_dialogData.m_clipDialogue, VP_AudioSetup.AUDIO_TYPE.VOICE, true, 1f, VP_AudioSetup.Dialog.DIALOGUE_CLIP, true);
                }
                else
                {
                    m_source.PlayOneShot(m_dialogData.m_clipDialogue);
                }
            }

            char[] characters = taglessText.ToCharArray();
           
            m_mainText.maxVisibleCharacters = 0;

            if (m_characterData != null)
            {
                m_voiceType = m_characterData.m_dialogVoiceType;
            }
            else
            {
                m_voiceType = m_clip != null ? DIALOG_VOICE_TYPE.SOUND_EVERY_CHARACTER : DIALOG_VOICE_TYPE.NO_SOUND;
            }

            bool m_hasEmotions = m_emotionList.Count > 0;
            int blankSpaces = 0;
            
            while ((m_currPrintedChars < m_textCharacters && m_currPrintedChars < characters.Length) || m_needLastDisplay)
            {
                if (m_refreshAnimations)
                    this.UpdateMeshAndAnims();

                if (!m_waitingForInputToContinueSameDialog)
                {
                    if (m_needLastDisplay)
                    {
                        m_waitingChars++;
                        if (m_waitingChars >= m_maxCharactersOrWordsNewLine)
                        {
                            m_mainText.maxVisibleCharacters = m_mainText.text.Length;
                            if (m_useContinueIcon)
                                m_iconForNext.gameObject.SetActive(true);

                            break;
                        }

                        continue;
                    }

                    char _char = characters[m_currPrintedChars];

                    if (m_canPlaySound)
                    {
                        if (m_voiceType == DIALOG_VOICE_TYPE.SOUND_EVERY_CHARACTER && m_clip != null)
                        {
                            if (_char != ' ')
                            {
                                if (!m_useAudioManager)
                                    m_letterSource.PlayOneShot(m_clip);
                                else
                                    VP_AudioManager.PlayOneShot(m_clip);
                            }
                        }
                        else if (m_voiceType == DIALOG_VOICE_TYPE.SOUND_EVERY_CHARACTER_DIFFERENT_SOUND || m_voiceType == DIALOG_VOICE_TYPE.SOUND_VOCALS)
                        {
                            if (_char != ' ')
                            {
                                if (!m_useAudioManager)
                                    m_letterSource.PlayOneShot(useDefaultCharacterAudio || m_characterData == null ? m_clip : m_characterAudios.GetLetterSound(_char, m_characterData.gender));
                                else
                                    VP_AudioManager.PlayOneShot(useDefaultCharacterAudio || m_characterData == null ? m_clip : m_characterAudios.GetLetterSound(_char, m_characterData.gender));
                            }


                        }
                        else if (m_voiceType == DIALOG_VOICE_TYPE.SOUND_EVERY_WORD && m_clip != null)
                        {
                            if (_char == ' ')
                            {
                                if (!m_useAudioManager)
                                    m_letterSource.PlayOneShot(useDefaultCharacterAudio || m_characterData == null ? m_clip : m_characterAudios.SpecialCharacter(m_characterData.gender));
                                else
                                    VP_AudioManager.PlayOneShot(useDefaultCharacterAudio || m_characterData == null ? m_clip : m_characterAudios.SpecialCharacter(m_characterData.gender));
                            }
                        }
                        else
                        {
                            if (_char != ' ' && m_clip)
                            {
                                if (!m_useAudioManager)
                                    m_letterSource.PlayOneShot(m_clip);
                                else
                                    VP_AudioManager.PlayOneShot(m_clip);
                            }
                        }
                    }

                    if (m_characterData && m_hasAnimations)
                    {
#if !USE_ANIMANCER                    	
	                    m_mugshotAnimator.SetBool(m_characterData.speakingAnim, true);
#else
	                    if (!m_useAnimancer)
	                    {
		                    m_mugshotAnimator.SetBool(m_characterData.speakingAnim, true);
	                    }
	                    else
	                    {
	                    	if (m_animationResources.TryGetClip(m_characterData.speakingAnim, out Animancer.ClipState.Transition _transition))
	                    	{
		         
	                    		if (m_animationResources.TryGetClip(m_characterData.startSpeakingAnim, out Animancer.ClipState.Transition _transition2))
	                    		{
		                    	    m_animancerState = m_animancerComponent.Play(_transition2, _transition2.FadeDuration);
		                    		m_animancerState.Events.OnEnd=()=>{ m_animancerState = m_animancerComponent.Play(_transition, _transition.FadeDuration); };
	                    		}
	                    		else
	                    		{
		                    		m_animancerState = m_animancerComponent.Play(_transition, _transition.FadeDuration);
	                    		}
	                    	}
	                    }
#endif
                    }

#if !USE_MORE_EFFECTIVE_COROUTINES
                    if (m_characterPrintDelays.Count > m_currPrintedChars)
                        yield return new WaitForSeconds(this.m_characterPrintDelays[m_currPrintedChars]);
                    else
                        yield return new WaitForSeconds(m_regularDelay);
#else
                    if (m_characterPrintDelays.Count > m_currPrintedChars)
                        yield return Timing.WaitForSeconds(this.m_characterPrintDelays[m_currPrintedChars]);
                    else
                        yield return Timing.WaitForSeconds(m_regularDelay);
#endif
                    if (m_queueIfMaxCharactersOrWords && !m_waitingForInputToContinueSameDialog)
                    {
                        m_waitingChars++;
                        m_currPrintedChars++;
                        VP_DialogManager.OnTextCharacterDisplayAction();

                        if (_char == ' ')
                            blankSpaces++;

                        if ((m_trimType == TEXT_TRIM_TYPE.BY_CHARACTER && (m_waitingChars >= m_maxCharactersOrWordsNewLine)) || (m_trimType == TEXT_TRIM_TYPE.BY_WORD && (blankSpaces >= m_maxCharactersOrWordsNewLine)))
                        {
                            m_timesPerformedContinueDialog++;
                            m_waitingForInputToContinueSameDialog = true;
                            if (m_useContinueIcon)
                                m_iconForNext.gameObject.SetActive(true);

                            if (!m_dialogData.m_waitForInput)
                            {
#if !USE_MORE_EFFECTIVE_COROUTINES
                                if (m_automaticSubDialog != null)
                                    StopCoroutine(m_automaticSubDialog);

                                m_automaticSubDialog = StartCoroutine(AutomaticSubDialog());
#else
                                Timing.KillCoroutines(m_automaticSubDialog);
                                m_automaticSubDialog = Timing.RunCoroutine(AutomaticSubDialog());
#endif
                            }
#if !USE_MORE_EFFECTIVE_COROUTINES
                            yield return null;
#else
                            yield return Timing.WaitForOneFrame;
#endif
                        }

                    }
                    else
                    {
                        m_currPrintedChars++;

                        VP_DialogManager.OnTextCharacterDisplayAction();

                        if (m_hasEmotions)
                        {
                            CheckEmotions();
                        }
                    }

                    if (!m_waitingForInputToContinueSameDialog)
                        m_mainText.maxVisibleCharacters = (m_queueIfMaxCharactersOrWords) ? ((!m_trimPassedDialog) ? m_timesPerformedContinueDialog * m_maxCharactersOrWordsNewLine + m_waitingChars : m_waitingChars) : m_currPrintedChars;

#if !USE_MORE_EFFECTIVE_COROUTINES
                    yield return null;
#else
                   yield return Timing.WaitForOneFrame;
#endif
                }
                else
                {

                    m_mainText.maxVisibleCharacters = ((!m_trimPassedDialog) ? (m_timesPerformedContinueDialog - 1) * m_maxCharactersOrWordsNewLine + m_waitingChars : m_waitingChars);
#if !USE_MORE_EFFECTIVE_COROUTINES
                    yield return null;
#else
                    yield return Timing.WaitForOneFrame;
#endif
                }
            }
            
            m_needLastDisplay = false;

            CheckEndDialog();

        }

        protected virtual void CheckEmotions()
        {
            foreach (KeyValuePair<int, KeyValuePair<EMOTION, string>> emotion in m_emotionList)
            {
                if (m_currPrintedChars == emotion.Key)
                {
                    VP_EventManager.TriggerEvent(VP_EventSetup.Dialog.TRIGGER_EMOTION, emotion.Value);
                    break;
                }
            }
        }

        protected virtual void ShowAnswers()
        {
            m_needToShowAnswers = false;
            m_optionGroup.SetActive(true);
            VP_DialogManager.OnAnswerShowAction();
            m_selectedAnswerIndex = 0;
            EventSystem.current.SetSelectedGameObject(m_answerButtons[m_selectedAnswerIndex].gameObject);

#if !USE_MORE_EFFECTIVE_COROUTINES
            if (m_displayCoroutine != null)
                StopCoroutine(m_displayCoroutine);
#else
            Timing.KillCoroutines(m_displayCoroutine);
#endif

            if (m_autoAnswer > 0)
            {
                VP_DialogManager.OnAutoAnswerTimeStartAction();
                StartAutoAnswerTimer();
            }
        }

        /// <summary>
        /// After showing the text method
        /// </summary>
        protected virtual void CheckEndDialog()
        {
#if USE_TEXT_ANIMATOR
            if (m_useTextAnimator)
            {
                m_textAnimatorPlayer?.onTextShowed.RemoveListener(CheckEndDialog);
                m_textAnimatorPlayer?.onCharacterVisible.RemoveListener(CheckTextAnimatorCharacter);
            }
#endif

            m_mainText.maxVisibleCharacters = m_textToShow.Length;

            if (m_dialogData.m_onTextShownCallback != null)
                m_dialogData.m_onTextShownCallback.Invoke();

            VP_DialogManager.OnTextShownAction();

	        if (m_hasAnimations)
	        {
#if USE_ANIMANCER
		        if (!m_useAnimancer)
		        {
			        m_mugshotAnimator.SetBool(m_characterData.speakingAnim, false);
		        }
		        else
		        {
			        if (m_animationResources.TryGetClip(m_characterData.idleAnim, out Animancer.ClipState.Transition _transition))
			        {
				        if (m_animationResources.TryGetClip(m_characterData.endSpeakingAnim, out Animancer.ClipState.Transition _transition2))
				        {
					        m_animancerState = m_animancerComponent.Play(_transition2, _transition2.FadeDuration);
					        m_animancerState.Events.OnEnd=()=>{ m_animancerState = m_animancerComponent.Play(_transition, _transition.FadeDuration); };
				        }
				        else
				        {
				        	m_animancerState = m_animancerComponent.Play(_transition, _transition.FadeDuration);
				        }
			        }
		        }
	        
#else
	        	m_mugshotAnimator.SetBool(m_characterData.speakingAnim, false);
#endif
	        	
	        }
                

            m_mainButton.SetActive(m_numberOfAnswers == 0 || (m_numberOfAnswers > 0 && !m_textAndAnswers));

            m_state = TEXT_STATE.SHOWN;
            
            if (m_useContinueIcon)
                m_iconForNext.gameObject.SetActive(true);

            if (m_fadeAnimationType == FADE_ANIMATION_TYPE.ANIMATOR && m_animator)
                m_animator.speed = m_animatorOriginalSpeed;

            m_needToShowAnswers = (m_numberOfAnswers > 0);

            if (m_setPositionData && m_dialogData.m_positionData)
            {
                Vector2 optionPos = m_dialogData.m_positionData.m_optionGroupPosition;
                m_buttonTrs.anchoredPosition = new Vector2(optionPos.x, optionPos.y + m_numberOfAnswers * 25);
            }

            if (m_needToShowAnswers)
            {
                if (m_textAndAnswers)
                {
                    ShowAnswers();
                }
                else
                {
                    if (m_fadeAnimationType == FADE_ANIMATION_TYPE.DOTWEEN)
                    {
#if DOTWEEN
                    	
                        m_mainText.DOFade(0f, m_timeToFadeWithDoTween).OnComplete(() =>
                        {
                            ShowAnswers();
                            m_mainText.alpha = 1f;
                            m_mainText.maxVisibleCharacters = 0;
                        });
#endif
                    }
                    else if (m_fadeAnimationType == FADE_ANIMATION_TYPE.MANUAL_FADING)
                    {
                    	DoFade(m_mainText, m_mainText.alpha, 0f, m_timeToFadeWithDoTween, ()=>
                    	{
                    		ShowAnswers();
	                    	m_mainText.alpha = 1f;
	                    	m_mainText.maxVisibleCharacters = 0;
                    	});
                    }
                    else if (m_fadeAnimationType == FADE_ANIMATION_TYPE.ANIMATOR)
                    {
                        // Insert your ONLY TEXT + needed elements fade
                    }
                    else // custom
                    {

                    }
                }

                return;
            }
            else if (m_chooseNumber)
            {
	            if (m_chooseNumberGO != null)
		            m_chooseNumberGO?.SetActive(true);

                VP_DialogManager.OnNumberShowAction();

                m_canChooseNumber = true;
            }

            if (!m_dialogData.m_waitForInput)
            {
                if ((m_numberOfAnswers > 0))
                {
                    if (m_answerButtons.Count > 0)
                        EventSystem.current.SetSelectedGameObject(m_answerButtons[0].gameObject);

                    m_dialogData.m_waitForInput = true;
                    return;
                }

#if !USE_MORE_EFFECTIVE_COROUTINES
                if (m_automaticDialog != null)
                    StopCoroutine(m_automaticDialog);

                m_automaticDialog = StartCoroutine(WaitForNextTextAutomatic(m_noInputDialogWaitTime));
#else
                Timing.KillCoroutines(m_displayCoroutine);
                
                m_automaticDialog = Timing.RunCoroutine(WaitForNextTextAutomatic(m_noInputDialogWaitTime));
#endif
            }
        }

#if !USE_MORE_EFFECTIVE_COROUTINES
        /// <summary>
        /// if we don't wait for the user's input, we continue directly
        /// </summary>
        /// <param name="_time"></param>
        /// <returns></returns>
        protected virtual IEnumerator WaitForNextTextAutomatic(float _time = 2f)
        {
            float timer = 0f;

            while (timer < _time)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            CheckNextDialog();
        }

        protected virtual IEnumerator AutomaticSubDialog(float _time = 2f)
        {
            float timer = 0f;

            while (timer < _time)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            ContinueSameDialog();
        }
#else
        /// <summary>
        /// if we don't wait for the user's input, we continue directly
        /// </summary>
        /// <param name="_time"></param>
        /// <returns></returns>
        protected virtual IEnumerator<float> WaitForNextTextAutomatic(float _time = 2f)
        {
            float timer = 0f;

            while (timer < _time)
            {
                timer += Time.deltaTime;
                yield return Timing.WaitForOneFrame;
            }

            CheckNextDialog();
        }

        protected virtual IEnumerator<float> AutomaticSubDialog(float _time = 2f)
        {
            float timer = 0f;

            while (timer < _time)
            {
                timer += Time.deltaTime;
                yield return Timing.WaitForOneFrame;
            }

            ContinueSameDialog();
        }
#endif
        /// <summary>
        /// Update the mesh for displaying animations in-line
        /// </summary>
        protected virtual void UpdateMeshAndAnims()
        {
            // This must be done here rather than in each TextAnimation's OnTMProChanged
            // b/c we must cache mesh data for all animations before animating any of them

            // Update the text mesh data (which also causes all attached TextAnimations to cache the mesh data)
            m_mainText.ForceMeshUpdate();

            // Force animate calls on all TextAnimations because TMPro has reset the mesh to its base state
            // NOTE: This must happen immediately. Cannot wait until end of frame, or the base mesh will be rendered
            for (int i = 0; i < this.m_animations.Count; i++)
            {
                this.m_animations[i].AnimateAllChars();
            }
        }
        /// <summary>
        /// Proccess tags
        /// </summary>
        /// <param name="text"></param>
        protected virtual void ProcessCustomTags(string _noTagText)
        {
            var text = VP_Utils.DialogUtils.RemoveUnityTags(_noTagText);
            this.m_characterPrintDelays = new List<float>(text.Length);
            this.m_animations = new List<VP_DialogTextAnimation>();

            var textAsSymbolList = VP_Utils.DialogUtils.CreateSymbolListFromText(text);

            int printedCharCount = 0;
            int customTagOpenIndex = 0;
            string customTagParam = "";

            float nextDelay = this.m_regularDelay * (1 / m_textSpeed);
            int skipCounter = 0;
            foreach (var symbol in textAsSymbolList)
            {
                if (skipCounter > 0)
                {
                    skipCounter--;
                    continue;
                }

                if (symbol.IsTag)
                {
                    // TODO - Verification that custom tags are not nested, b/c that will not be handled gracefully
                    if (symbol.Tag.TagType == VP_DialogSetup.Tags.DELAY)
                    {
#if USE_TEXT_ANIMATOR
                        if (m_useTextAnimator)
                            continue;
#endif

                        if (symbol.Tag.IsClosingTag)
                        {
                            nextDelay = this.m_regularDelay * (1 / m_textSpeed);
                        }
                        else
                        {
                            nextDelay = symbol.GetFloatParameter(this.m_regularDelay) * (1 / m_textSpeed);
                        }
                    }

                    else if (symbol.Tag.TagType == VP_DialogSetup.Tags.ANIM || symbol.Tag.TagType == VP_DialogSetup.Tags.ANIMATION)
                    {
                        if (symbol.Tag.IsOpeningTag)
                        {

                            skipCounter += symbol.Tag.m_fullText.Length;
                            var mid = symbol.Tag.m_middleText.Length;
                            customTagOpenIndex = printedCharCount;
                            printedCharCount += mid;
                          
                            customTagParam = symbol.Tag.Parameter;


#if USE_TEXT_ANIMATOR
                            if (m_useTextAnimator)
                                continue;
#endif

                            // Add a TextAnimation component to process this animation
                            VP_DialogTextAnimation anim = null;
                            if (this.IsAnimationShake(customTagParam))
                            {
                                anim = m_mainText.gameObject.GetComponent<VP_DialogShakeAnimation>();
                                ((VP_DialogShakeAnimation)anim).LoadPreset(this.m_shakeLibrary, customTagParam);
                            }
                            else if (this.IsAnimationCurve(customTagParam))
                            {
                                anim = m_mainText.gameObject.GetComponent<VP_DialogCurveAnimation>();
                                ((VP_DialogCurveAnimation)anim).LoadPreset(this.m_curveLibrary, customTagParam);
                            }
                            else
                            {
                                // Could not find animation. Should we error here?
                            }

                            anim.SetCharsToAnimate(customTagOpenIndex, printedCharCount - 1);
                            anim.Play();
                            this.m_animations.Add(anim);
                        }
                    }
                    else if (symbol.Tag.TagType == VP_DialogSetup.Tags.EMOTION)
                    {
                        if (symbol.Tag.IsOpeningTag)
                        {
                            var midtxt = symbol.Tag.m_middleText;
                            customTagParam = symbol.Tag.Parameter;

                            EMOTION emotion = EMOTION.NONE;
                            System.Enum.TryParse(customTagParam.ToUpper(), out emotion);

                            if (emotion != EMOTION.NONE)
                                m_emotionList.Add(new KeyValuePair<int, KeyValuePair<EMOTION, string>>(printedCharCount, new KeyValuePair<EMOTION, string>(emotion, !string.IsNullOrEmpty(midtxt) ? midtxt : "Play")));

                            skipCounter += symbol.Tag.m_fullText.Length;
                            printedCharCount += midtxt.Length;
                        }
                    }
                    else if (symbol.Tag.TagType == VP_DialogSetup.Tags.VARIABLE)
                    {
                        if (symbol.Tag.IsOpeningTag)
                        {
                            var midtxt = symbol.Tag.m_middleText;
                            skipCounter += symbol.Tag.m_fullText.Length;
                            printedCharCount += midtxt.Length;
                        }
                    }
                    else if (symbol.Tag.TagType == VP_DialogSetup.Tags.GRAPH_VARIABLE)
                    {
                        if (symbol.Tag.IsOpeningTag)
                        {
                            var midtxt = symbol.Tag.m_middleText;
                            skipCounter += symbol.Tag.m_fullText.Length;
                            printedCharCount += midtxt.Length;
                        }
                    }
                    else
                    {
                        // Unrecognized CustomTag Type. Should we error here?
                    }
                }
                else
                {
                    printedCharCount++;

                    this.m_characterPrintDelays.Add(nextDelay);
                }
            }

            m_refreshAnimations = (m_animations.Count > 0);
        }



        /// <summary>
        /// Check if the string is in the preset
        /// </summary>
        /// <param name="animName"></param>
        /// <returns></returns>
        protected virtual bool IsAnimationShake(string animName)
        {
            return this.m_shakeLibrary.ContainsKey(animName);
        }
        /// <summary>
        /// Check if the string is in the preset
        /// </summary>
        /// <param name="animName"></param>
        /// <returns></returns>
        protected virtual bool IsAnimationCurve(string animName)
        {
            return this.m_curveLibrary.ContainsKey(animName);
        }

    }

    [System.Serializable]
    public class FadeUITime
    {
        public float m_time;
        public UnityEngine.UI.Graphic m_graphic;
    }

}
