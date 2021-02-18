using Helpers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VirtualPhenix.Dialog;
using VirtualPhenix.Localization;
using VirtualPhenix.Variables;

namespace VirtualPhenix
{
    public enum DIALOG_INPUT_CALLBACK
    {
        INTERACT,
        CANCEL,
        UP,
        DOWN,
        RIGHT,
        LEFT
    }

    [System.Serializable]
    public class VP_DialogDirectMessage
    {
        [SerializeField] protected VP_DialogMessageData m_data;
        [SerializeField] protected VP_DialogMessage m_customMessage;

        public virtual VP_DialogMessageData Data { get { return m_data; } }
        public virtual VP_DialogMessage CustomMessage { get { return m_customMessage; } }

        public VP_DialogDirectMessage(VP_DialogMessage _customMessage, VP_DialogMessageData _messageData)
        {
            m_data = _messageData;
            m_customMessage = _customMessage;
        }
    }
    [System.Serializable]
    public class VP_IntUnityEvent : UnityEvent<int>
    {

    }
    [System.Serializable]
    public class VP_CharacterDataUnityEvent : UnityEvent<VP_DialogCharacterData>
    {

    }
    [System.Serializable]
    public class VP_TransformUnityEvent : UnityEvent<Transform>
    {

    }

	[DefaultExecutionOrder(VP_ExecutingOrderSetup.DIALOGUE_MANAGER)]
    public class VP_DialogManager : VP_SingletonMonobehaviour<VP_DialogManager>
	{
		[Header("Config")]
		[SerializeField]protected bool m_alwaysTranslate = false;
		
		[Header("Default UI")]
    	[SerializeField]protected VP_DialogMessage m_defaultDialogMessage;
    	[SerializeField]protected Transform m_defaultCanvas;

        [Header("Current Chart")]
        /// <summary>
        /// Current Chart with a referenced graph
        /// </summary>
        [SerializeField, HideInInspector] protected VP_DialogChart m_currentChart;

        [Header("Dialog Prefab")]
        /// <summary>
        /// Prefab instantiated if it doesn't already exists in scene when ShowMessage method is called
        /// </summary>
        [SerializeField] protected GameObject m_dialogPrefab = null;

        [Header("Talking Dialog")]
        [SerializeField] protected bool m_usingDirectMessage;

        [Header("Current Dialog and Canvas")]


        /// <summary>
        /// Current dialog message 
        /// </summary>
        [SerializeField] protected VP_DialogMessage m_talkingDialog;
        /// <summary>
        /// Current canvas where the dialog prefab is instantiated if needed
        /// </summary>
        [SerializeField, HideInInspector] protected Transform m_dialogCanvas;


        [Header("Properties")]
        /// <summary>
        /// Checker that any dialogue graph is being played
        /// </summary>
        public bool m_speaking = false;
        /// <summary>
        /// Deactivate the dialogCanvas after End? -> Public in case you need to set it on runtime
        /// </summary>
        public bool m_deactivateCanvasOnFadeOut = true;
        /// <summary>
        /// Always parent on canvas?
        /// </summary>
        public bool m_alwaysParentDialogWithCanvas = true;

        [Header("Global Dialogue Variables")]
        [SerializeField] protected VP_FixedVariableDataBase m_copyDatabaseFrom;
        [SerializeField] protected VP_VariableDataBase m_globalDatabase = new VP_VariableDataBase();
        /// <summary>
        /// Queue of direct messages
        /// </summary>
        protected Queue<VP_DialogDirectMessage> m_directMessages = new Queue<VP_DialogDirectMessage>();
        public virtual bool m_queueBusy { get { return m_directMessages.Count > 0 || m_speaking; } }
        public virtual bool m_queueActive { get { return m_directMessages.Count > 0; } }
	    public virtual VP_DialogMessage TalkingDialog { get { return m_talkingDialog; } }
        /// <summary>
        /// Callback for sequence
        /// </summary>
        public static System.Action OnDialogCompleteForOutput;

        [Header("Callbacks")]
        /// <summary>
        /// Callback when a node is completed. it is not just dialog node.
        /// </summary>
        [SerializeField] protected UnityEvent OnDialogComplete;
        /// <summary>
        /// Callback called when the full text is displayed in the dialog bubble
        /// </summary>
        [SerializeField] protected UnityEvent OnTextShown;
        /// <summary>
        /// Callback called when the current bubble is starting to be displayed
        /// </summary>
        [SerializeField] protected UnityEvent OnDialogStart;
        /// <summary>
        /// Callback called when the answers are displayed
        /// </summary>
        [SerializeField] protected UnityEvent OnAnswerShow;
        /// <summary>
        /// Callback called when choose number is displayed
        /// </summary>
        [SerializeField] protected UnityEvent OnNumberShow;
        /// <summary>
        /// Callback called when the dialogue is ended. Fully ended.
        /// </summary>
        [SerializeField] protected UnityEvent OnDialogEnd;
        /// <summary>
        /// Callback called when the user selects specific choice
        /// </summary>
        [SerializeField] protected VP_IntUnityEvent OnChoiceSelection;
        /// <summary>
        /// Callback called when the user selects number
        /// </summary>
        [SerializeField] protected VP_IntUnityEvent OnNumberChosen;
        /// <summary>
        /// Pressed B/Cancel Input when number is chosen 
        /// </summary>
        [SerializeField] protected UnityEvent OnNumberCancel;
        /// <summary>
        /// Callback called when the log is starting to be recorded. 
        /// </summary>
        [SerializeField] protected UnityEvent m_onRegisteredAble;
        /// <summary>
        /// Callback called on end or start so the user can hide the log button
        /// </summary>
        [SerializeField] protected UnityEvent m_onRegisteredDisable;
        /// <summary>
        /// Callback called when character is speaking. This is used for VP_DialogAnimator so the user can see when a specific character is speaking.
        /// </summary>
        [SerializeField] protected VP_CharacterDataUnityEvent OnCharacterSpeak;
        /// <summary>
        /// Callback called from DialogAnimator so you can use that transform for IK look at
        /// </summary>
        [SerializeField] protected VP_TransformUnityEvent OnAnimationTarget;
        /// <summary>
        /// Callback called when pressing "Skip"
        /// </summary>
        [SerializeField] protected UnityEvent OnSkip;
        /// <summary>
        /// Interact Button -> If you don't want to use VP_EventManager
        /// </summary>
        [SerializeField] protected UnityEvent OnDialogInteract;
        /// <summary>
        /// Up Button -> If you don't want to use VP_EventManager
        /// </summary>
        [SerializeField] protected UnityEvent OnDialogUp;
        /// <summary>
        /// Down Button -> If you don't want to use VP_EventManager
        /// </summary>
        [SerializeField] protected UnityEvent OnDialogDown;
        /// <summary>
        /// Right Button -> If you don't want to use VP_EventManager
        /// </summary>
        [SerializeField] protected UnityEvent OnDialogRight;
        /// <summary>
        /// Left Button -> If you don't want to use VP_EventManager
        /// </summary>
        [SerializeField] protected UnityEvent OnDialogLeft;
        /// <summary>
        /// Cancel button -> same as interact but if any of the options have select if cancelled 
        /// or number selection can be cancelled, this can be used.
        /// </summary>
        [SerializeField] protected UnityEvent OnDialogCancel;
        /// <summary>
        /// Use this callback to select specific answer without calling VP_DialogMessage.Answer
        /// </summary>
        [SerializeField] protected VP_IntUnityEvent OnExternalAnswerSelected;
        /// <summary>
        /// Callback called when the auto-time for answers starts
        /// </summary>
        [SerializeField] protected UnityEvent OnAutoAnswerTimeStart;
        /// <summary>
        /// Callback called when the auto-time for answers ends
        /// </summary>
        [SerializeField] protected UnityEvent OnAutoAnswerTimeEnd;
        /// <summary>
        /// Callback called when a character is displayed
        /// </summary>
        [SerializeField] protected UnityEvent OnTextCharacterDisplay;
        /// <summary>
        /// Properties
        /// </summary>
        public virtual VP_DialogMessage DialogMessage { get { return m_talkingDialog; } }
        public virtual VP_DialogChart CurrentChart { get { return m_currentChart; } set { m_currentChart = value; } }
        public virtual Transform CurrentCanvas { get { return m_dialogCanvas; } set { m_dialogCanvas = value; } }
        public static bool IsSpeaking { get { return m_instance && Instance.m_speaking; } }
        public virtual VP_VariableDataBase GlobalVariables { get { return m_globalDatabase; } }
        public virtual VP_VariableDataBase GraphVariables { get { return Instance.GraphVariables; } }
        public static bool IsBusy { get { return Instance.m_queueBusy; } }
        public static bool IsQueueActive { get { return Instance.m_queueActive; } }
        public virtual int OnChoiceSelectionCount { get { return OnChoiceSelection != null ? OnChoiceSelection.GetPersistentEventCount() : 0; } }

        protected override void Initialize()
        {
            base.Initialize();

            m_directMessages = new Queue<VP_DialogDirectMessage>();
            if (m_defaultCanvas == null)
            {
                if (m_currentChart && m_currentChart.ChartCanvas)
                {
                    m_defaultCanvas = m_currentChart.ChartCanvas;
                }
                else if (m_dialogCanvas != null)
                {
                    m_defaultCanvas = m_dialogCanvas;
                }
                else
                {
                    m_defaultCanvas = transform.GetChild(0);
                }
            }

            if (m_defaultDialogMessage == null)
            {
                m_defaultDialogMessage = (VP_DialogMessage)FindObjectOfType(typeof(VP_DialogMessage));
            }

            if (m_dialogPrefab == null)
                m_dialogPrefab = Resources.Load<GameObject>("Dialogue/Prefabs/Dialog/Dialog");

            if (m_copyDatabaseFrom != null)
            {
                m_globalDatabase.CopyFrom(m_copyDatabaseFrom.Variables);
            }
        }

        public virtual void SetInitialData(VP_DialogMessage _chartmsg, Transform _canvas, VP_DialogChart _chart)
        {
            if (!m_currentChart)
                m_currentChart = _chart;

            if (!m_talkingDialog)
                m_talkingDialog = _chartmsg;

            if (!m_defaultCanvas)
                m_defaultCanvas = _canvas;

            if (!m_dialogCanvas)
                m_dialogCanvas = _canvas;

            if (!m_defaultDialogMessage)
                m_defaultDialogMessage = _chartmsg;
        }

        public virtual void SetDefaultData()
        {
            if (m_defaultCanvas == null)
            {
                if (m_currentChart && m_currentChart.ChartCanvas)
                {
                    m_defaultCanvas = m_currentChart.ChartCanvas;
                }
                else if (m_dialogCanvas != null)
                {
                    m_defaultCanvas = m_dialogCanvas;
                }
                else
                {
                    m_defaultCanvas = transform.GetChild(0);
                }
            }

            // Assigned again to avoid warning
            if (m_dialogPrefab == null)
                m_dialogPrefab = Resources.Load<GameObject>("Dialogue/Prefabs/Dialog/Dialog");

            if (m_defaultDialogMessage == null)
            {
                m_defaultDialogMessage = (VP_DialogMessage)FindObjectOfType(typeof(VP_DialogMessage));
            }
        }

	    public virtual void SetDefaultDialogMessage(VP_DialogMessage _newDefault)
	    {
	    	m_talkingDialog = _newDefault;
	    	m_defaultDialogMessage = _newDefault;
	    }

	    public virtual void PressedInteractInTalkingDialog()
        {
	        m_talkingDialog?.PressedInteract();
        }
        
	    public virtual void PressedCancelInTalkingDialog()
        {
	        m_talkingDialog.CheckCancel();
        }
        
	    
	    public virtual void PressedUpInTalkingDialog()
        {
            m_talkingDialog?.CheckUpAnswerIndex();
        }

        public virtual void PressedDownInTalkingDialog()
        {
            m_talkingDialog?.CheckDownAnswerIndex();
        }

        public virtual void PressedRightInTalkingDialog()
        {
            m_talkingDialog?.CheckRightInput();
        }

        public virtual void PressedLeftInTalkingDialog()
        {
            m_talkingDialog?.CheckLeftInput();
        }

        #region Listen To Callbacks



        public static void StartListeningToOnTextCharacterDisplay(UnityAction _actionCalled)
        {
            Instance?.OnTextCharacterDisplay.AddListener(_actionCalled);
        }


        public static void StopListeningToOnTextCharacterDisplay(UnityAction _actionCalled)
        {
            Instance?.OnTextCharacterDisplay.RemoveListener(_actionCalled);
        }
             
        
        public static void StartListeningToOnNumberShow(UnityAction _actionCalled)
        {
            Instance?.OnNumberShow.AddListener(_actionCalled);
        }


        public static void StopListeningToOnNumberShow(UnityAction _actionCalled)
        {
            Instance?.OnNumberShow.RemoveListener(_actionCalled);
        }


        public static void StartListeningToOnDialogInteract(UnityAction _actionCalled)
        {
            Instance?.OnDialogInteract.AddListener(_actionCalled);
        }


        public static void StopListeningToOnDialogInteract(UnityAction _actionCalled)
        {
            Instance?.OnDialogInteract.RemoveListener(_actionCalled);
        }

        public static void StartListeningToOnNumberCancel(UnityAction _actionCalled)
        {
            Instance?.OnNumberCancel.AddListener(_actionCalled);
        }


        public static void StopListeningToOnNumberCancel(UnityAction _actionCalled)
        {
            Instance?.OnNumberCancel.RemoveListener(_actionCalled);
        }

        public static void StartListeningToOnDialogUp(UnityAction _actionCalled)
        {
            Instance?.OnDialogUp.AddListener(_actionCalled);
        }


        public static void StopListeningToOnDialogUp(UnityAction _actionCalled)
        {
            Instance?.OnDialogUp.RemoveListener(_actionCalled);
        }

        public static void StartListeningToOnDialogRight(UnityAction _actionCalled)
        {
            Instance?.OnDialogRight.AddListener(_actionCalled);
        }

        public static void StopListeningToOnDialogRight(UnityAction _actionCalled)
        {
            Instance?.OnDialogRight.RemoveListener(_actionCalled);
        }

        public static void StartListeningToOnDialogLeft(UnityAction _actionCalled)
        {
            Instance?.OnDialogLeft.AddListener(_actionCalled);
        }

        public static void StopListeningToOnDialogLeft(UnityAction _actionCalled)
        {
            Instance?.OnDialogLeft.RemoveListener(_actionCalled);
        }

        public static void StartListeningToOnDialogCancel(UnityAction _actionCalled)
        {
            Instance?.OnDialogCancel.AddListener(_actionCalled);
        }

        public static void StopListeningToOnDialogCancel(UnityAction _actionCalled)
        {
            Instance?.OnDialogCancel.RemoveListener(_actionCalled);
        }

        public static void StartListeningToOnDialogDown(UnityAction _actionCalled)
        {
            Instance?.OnDialogDown.AddListener(_actionCalled);
        }


        public static void StopListeningToOnDialogDown(UnityAction _actionCalled)
        {
            Instance?.OnDialogDown.RemoveListener(_actionCalled);
        }

        public static void StartListeningToOnAutoAnswerTimeStart(UnityAction _actionCalled)
        {
            Instance?.OnAutoAnswerTimeStart.AddListener(_actionCalled);
        }


        public static void StopListeningToOnAutoAnswerTimeStart(UnityAction _actionCalled)
        {
            Instance?.OnAutoAnswerTimeStart.RemoveListener(_actionCalled);
        }

        public static void StartListeningToOnAutoAnswerTimeEnd(UnityAction _actionCalled)
        {
            Instance?.OnAutoAnswerTimeEnd.AddListener(_actionCalled);
        }


        public static void StopListeningToOnAutoAnswerTimeEnd(UnityAction _actionCalled)
        {
            Instance?.OnAutoAnswerTimeEnd.RemoveListener(_actionCalled);
        }

        public static void StartListeningToOnExternalAnswerSelected(UnityAction<int> _actionCalled)
        {
            Instance?.OnExternalAnswerSelected.AddListener(_actionCalled);
        }


        public static void StopListeningToOnExternalAnswerSelected(UnityAction<int> _actionCalled)
        {
            Instance?.OnExternalAnswerSelected.RemoveListener(_actionCalled);
        }

        public static void StartListeningToOnSkip(UnityAction _actionCalled)
        {
            Instance?.OnSkip.AddListener(_actionCalled);
        }


        public static void StopListeningToOnSkip(UnityAction _actionCalled)
        {
            Instance?.OnSkip.RemoveListener(_actionCalled);
        }

        public static void StartListeningToOnSpeakingTargetTransform(UnityAction<Transform> _actionCalled)
        {
            Instance?.OnAnimationTarget.AddListener(_actionCalled);
        }


        public static void StopListeningToOnSpeakingTargetTransform(UnityAction<Transform> _actionCalled)
        {
            Instance?.OnAnimationTarget.RemoveListener(_actionCalled);
        }

        public static void StartListeningToOnCharacterSpeak(UnityAction<VP_DialogCharacterData> _actionCalled)
        {
            Instance?.OnCharacterSpeak.AddListener(_actionCalled);
        }


        public static void StopListeningToOnCharacterSpeak(UnityAction<VP_DialogCharacterData> _actionCalled)
        {
            Instance?.OnCharacterSpeak.RemoveListener(_actionCalled);
        }

        public static void StartListeningToOnRegisterDialogAble(UnityAction _actionCalled)
        {
            Instance?.m_onRegisteredAble.AddListener(_actionCalled);
        }


        public static void StopListeningToOnRegisterDialogAble(UnityAction _actionCalled)
        {
            Instance?.m_onRegisteredAble.RemoveListener(_actionCalled);
        }

        public static void StartListeningToOnRegisterDialogDisable(UnityAction _actionCalled)
        {
            Instance?.m_onRegisteredDisable.AddListener(_actionCalled);
        }


        public static void StopListeningToOnRegisterDialogDisable(UnityAction _actionCalled)
        {
            Instance?.m_onRegisteredDisable.RemoveListener(_actionCalled);
        }

        public static void StartListeningToOnDialogCompleteForOutput(System.Action _actionCalled)
        {
            if (OnDialogCompleteForOutput != null)
                OnDialogCompleteForOutput += _actionCalled;
            else
                OnDialogCompleteForOutput = _actionCalled;
        }


        public static void StopListeningToOnDialogCompleteForOutput(System.Action _actionCalled)
        {
            if (OnDialogCompleteForOutput != null)
                OnDialogCompleteForOutput -= _actionCalled;
        }

        public static void StartListeningToOnDialogComplete(UnityAction _actionCalled)
        {
            Instance?.OnDialogComplete.AddListener(_actionCalled);
        }

        public static void StopListeningToOnDialogComplete(UnityAction _actionCalled)
        {
            Instance?.OnDialogComplete.RemoveListener(_actionCalled);
        }

        public static void StartListeningToOnAnswerShow(UnityAction _actionCalled)
        {
            Instance?.OnAnswerShow.AddListener(_actionCalled);
        }

        public static void StopListeningToOnAnswerShow(UnityAction _actionCalled)
        {
            Instance?.OnAnswerShow.RemoveListener(_actionCalled);
        }

        public static void StartListeningOnChoiceSelection(UnityAction<int> _actionCalled)
        {
            Instance?.OnChoiceSelection.AddListener(_actionCalled);
        }

        public static void StopListeningOnChoiceSelection(UnityAction<int> _actionCalled)
        {
            Instance?.OnChoiceSelection.RemoveListener(_actionCalled);
        }



        public static void StartListeningToOnTextShown(UnityAction _actionCalled)
        {
            Instance?.OnTextShown.AddListener(_actionCalled);
        }

        public static void StopListeningToOnTextShown(UnityAction _actionCalled)
        {
            Instance?.OnTextShown.RemoveListener(_actionCalled);

        }

        public static void StartListeningToOnChooseNumber(UnityAction<int> _actionCalled)
        {
            Instance?.OnNumberChosen.AddListener(_actionCalled);
        }


        public static void StopListeningToOnChooseNumber(UnityAction<int> _actionCalled)
        {
            Instance?.OnNumberChosen.RemoveListener(_actionCalled);
        }

        public static void StartListeningToOnDialogStart(UnityAction _actionCalled)
        {
            Instance?.OnDialogStart.AddListener(_actionCalled);
        }


        public static void StopListeningToOnDialogStart(UnityAction _actionCalled)
        {
            Instance?.OnDialogStart.RemoveListener(_actionCalled);
        }

        public static void StartListeningToOnDialogEnd(UnityAction _actionCalled)
        {
            
            Instance?.OnDialogEnd.AddListener(_actionCalled);
        }


        public static void StopListeningToOnDialogEnd(UnityAction _actionCalled)
        {
            Instance?.OnDialogEnd.RemoveListener(_actionCalled);
        }

        #endregion

        #region Callback Clear
        public static void ClearOnAutoAnswerTimeStartAction()
        {
            Instance?.OnAutoAnswerTimeStart.RemoveAllListeners();
        }
        public static void ClearOnTextCharacterDisplayAction()
        {
            Instance?.OnTextCharacterDisplay.RemoveAllListeners();
        }

        public static void ClearOnNumberShowAction()
        {
            Instance?.OnNumberShow.RemoveAllListeners();
        }

        public static void ClearOnAutoAnswerTimeEndAction()
        {
            Instance?.OnAutoAnswerTimeEnd.RemoveAllListeners();
        }

        public static void ClearOnExternalAnswerSelectedCallback(int _selection)
        {
            Instance?.OnExternalAnswerSelected.RemoveAllListeners();
        }

        public static void ClearOnCharacterSpeakAction(VP_DialogCharacterData _character)
        {
            Instance?.OnCharacterSpeak.RemoveAllListeners();
        }

        public static void ClearOnAnimationTargetAction(Transform _target)
        {
            Instance?.OnAnimationTarget.RemoveAllListeners();
        }

        public static void ClearOnDialogRegisterDisableAction()
        {
            Instance?.m_onRegisteredDisable.RemoveAllListeners();
        }

        public static void ClearOnDialogRegisterAbleAction()
        {
            Instance?.m_onRegisteredAble.RemoveAllListeners();
        }

        public static void ClearOnDialogCompleteOutputAction()
        {
            OnDialogCompleteForOutput = null;
        }
        public static void ClearOnDialogCompleteAction()
        {
            Instance?.OnDialogComplete.RemoveAllListeners();
        }

        public static void ClearOnAnswerShowAction()
        {
            Instance?.OnAnswerShow.RemoveAllListeners();
        }

        public static void ClearOnDialogStartAction()
        {
            Instance?.OnDialogStart.RemoveAllListeners();
        }

        public static void ClearOnDialogEndAction()
        {
            Instance?.OnDialogEnd.RemoveAllListeners();
        }

        public static void ClearOnTextShownAction()
        {
            Instance?.OnTextShown.RemoveAllListeners();
        }

        public static void ClearOnSkipAction()
        {
            Instance?.OnSkip.RemoveAllListeners();
        }

        public static void ClearOnDialogInteractAction()
        {
            Instance?.OnDialogInteract.RemoveAllListeners();
        }

        public static void ClearOnDialogUpAction()
        {
            Instance?.OnDialogUp.RemoveAllListeners();
        }

        public static void ClearOnDialogDownAction()
        {
            Instance?.OnDialogDown.RemoveAllListeners();
        }

        public static void ClearOnDialogRightAction()
        {
            Instance?.OnDialogRight.RemoveAllListeners();
        }

        public static void ClearOnDialogLeftAction()
        {
            Instance?.OnDialogLeft.RemoveAllListeners();
        }

        public static void ClearOnDialogCancelAction()
        {
            Instance?.OnDialogCancel.RemoveAllListeners();
        }

        public static void ClearOnNumberChosenAction(int _chosenNumber)
        {
            Instance?.OnNumberChosen.RemoveAllListeners();
        }

        public static void ClearOnNumberCancelAction()
        {
            Instance?.OnNumberCancel.RemoveAllListeners();
        }
        #endregion

        #region Callback Invoke

        public static void OnTextCharacterDisplayAction()
        {
            Instance?.OnTextCharacterDisplay.Invoke();
        }       
        public static void OnAutoAnswerTimeStartAction()
        {
            Instance?.OnAutoAnswerTimeStart.Invoke();
        }
        public static void OnNumberShowAction()
        {
            Instance?.OnNumberShow.Invoke();
        }

        public static void OnAutoAnswerTimeEndAction()
        {
            Instance?.OnAutoAnswerTimeEnd.Invoke();
        }

        public static void OnExternalAnswerSelectedCallback(int _selection)
        {
            Instance?.OnExternalAnswerSelected.Invoke(_selection);
        }

        public static void OnCharacterSpeakAction(VP_DialogCharacterData _character)
        {
            Instance?.OnCharacterSpeak.Invoke(_character);
        }

        public static void OnAnimationTargetAction(Transform _target)
        {
            Instance?.OnAnimationTarget.Invoke(_target);
        }

        public static void OnDialogRegisterDisableAction()
        {
            Instance?.m_onRegisteredDisable.Invoke();
        }

        public static void OnDialogRegisterAbleAction()
        {
            Instance?.m_onRegisteredAble.Invoke();
        }

        public static void OnDialogCompleteOutputAction()
        {
            if (OnDialogCompleteForOutput != null)
                OnDialogCompleteForOutput.Invoke();
        }
        /// <summary>
        /// When the dialog branch ends, it calls a callback so the sequence can be performanced
        /// </summary>
        public static void OnDialogCompleteAction()
        {
            Instance?.OnDialogComplete.Invoke();
        }

        public static void OnAnswerShowAction()
        {
            Instance?.OnAnswerShow.Invoke();
        }

        public static void OnDialogStartAction()
        {
            Instance?.OnDialogStart.Invoke();
        }

        public static void OnDialogEndAction()
        {
            Instance?.OnDialogEnd.Invoke();
        }

        public static void OnTextShownAction()
        {
            Instance?.OnTextShown.Invoke();
        }

        public virtual void SelectCurrentAnswer()
        {
            DialogMessage.AnswerCurrent();
        }

        public static void SelectCurrentOption()
        {
            Instance?.SelectCurrentAnswer();
        }

        public static void OnSkipAction()
        {
            Instance?.OnSkip.Invoke();
        }

        public static void OnDialogInteractAction()
        {
            Instance?.OnDialogInteract.Invoke();
        }

        public static void OnDialogUpAction()
        {
            Instance?.OnDialogUp.Invoke();
        }

        public static void OnDialogDownAction()
        {
            Instance?.OnDialogDown.Invoke();
        }

        public static void OnDialogRightAction()
        {
            Instance?.OnDialogRight.Invoke();
        }

        public static void OnDialogLeftAction()
        {
            Instance?.OnDialogLeft.Invoke();
        }

        public static void OnDialogCancelAction()
        {
            Instance?.OnDialogCancel.Invoke();
        }

        public static void OnNumberChosenAction(int _chosenNumber)
        {
            Instance?.OnNumberChosen.Invoke(_chosenNumber);
        }

        public static void OnNumberCancelAction()
        {
            Instance?.OnNumberCancel.Invoke();
        }

        #endregion

        #region Variables
        /// <summary>
        /// Get variable if exits in the dictionary
        /// </summary>
        /// <param name="_varName"></param>
        /// <returns></returns>
        public virtual FieldData GetVariable<T>(string _varName, T _type)
        {
            return VP_Utils.DialogUtils.GetVariableValueFromDatabase(_varName, _type, m_globalDatabase);
        }
        /// <summary>
        /// Get variable if exits in the dictionary
        /// </summary>
        /// <param name="_varName"></param>
        /// <returns></returns>
        public virtual FieldData GetVariableFromStringType(string _varName, string _type)
        {
            return VP_Utils.DialogUtils.GetVariableValueStrTypeFromDatabase(_varName, _type, m_globalDatabase);
        }
        public virtual string GetVariableStringValue<T>(string _varName, T _type)
        {
            return VP_Utils.DialogUtils.GetVariableValueStringFromDatabase(_varName, _type, m_globalDatabase);
        }
        public virtual string GetVariableStringValueFromStringType(string _varName, string _type)
        {
            return VP_Utils.DialogUtils.GetVariableValueStringFromStrTypeFromDatabase(_varName, _type, m_globalDatabase);
        }
        /// <summary>
        /// Set a custom generic variable, a value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_name"></param>
        /// <param name="value"></param>
        public virtual void SetVariable<T>(string _name, T value)
        {
            VP_Utils.DialogUtils.SetVariableToDatabase(_name, value, m_globalDatabase);
        }

        public virtual void SetGlobalVariable<T>(string _name, T value)
        {
            VP_Utils.DialogUtils.SetVariableToDatabase(_name, value, m_globalDatabase);
        }
        public virtual void SetGraphVariable<T>(string _name, T value)
        {
            VP_Utils.DialogUtils.SetVariableToDatabase(_name, value, m_currentChart?.Graph?.GraphVariables);
        }
        #endregion

        public virtual FieldData GetGraphVariable<T>(string _varName, T _type)
        {
            return m_currentChart?.Graph?.GetVariableValue(_varName, _type);
        }
        public virtual FieldData GetGraphVariableFromStringType(string _varName, string _type)
        {
            return m_currentChart?.Graph?.GetVariableFromStringType(_varName, _type);
        }
        public virtual string GetGraphVariableStringValue<T>(string _varName, T _type)
        {
            return m_currentChart?.Graph?.GetVariableStringValue(_varName, _type);
        }
        public virtual string GetGraphVariableStringValueFromStringType(string _varName, string _type)
        {
            return m_currentChart?.Graph?.GetVariableStringValueFromStringType(_varName, _type);
        }

        public virtual List<VP_DialogLog> GetRegisteredLogs()
        {
            return m_currentChart ? m_currentChart.GetRegisteredLogs() : null;
        }

        protected virtual void NextDirectMessageAfterGraph()
        {
            NextDirectMessage(false);
            StopListeningToOnDialogEnd(NextDirectMessageAfterGraph);
        }

        public virtual void DirectMessage(string _message, AudioClip _clip = null, DIALOG_TYPE _type = DIALOG_TYPE.REGULAR, bool _translate = false, bool showDirectly = false, bool fadeInOut = true, VP_DialogMessage _customDialogMessage = null,
            UnityAction _onCompleteCallback = null, UnityAction _onStartCallback = null, UnityAction _onEndCallback = null, UnityAction _onTextShown = null, VP_DialogPositionData _pos = null,
            bool _skippable = true, bool waitForInput = true, float _duration = 0.5f, bool _soundOnContinue = true, VP_DialogCharacterData _character = null, float _textSpeed = 1f, bool _waitForAudioEnd = true,
            List<VP_Dialog.Answer> _answers = null, UnityAction<int> _onAnswerChosen = null, bool _showAnswersSameTime = true, int _autoAnswer = -1, bool _overrideTextColor = false, Color textColor = default(Color), TMPro.TMP_FontAsset _font = null, float _fontSize = 45f,
             float _timeForAnswer = 5f, bool _chooseNumber = false, Vector3 _parameters = default(Vector3), bool _canCancel = true, float _automaticScreenTime = 1f, UnityAction<int> _onNumberChosen = null, UnityAction _onCancelCallback = null)
        {
            string translatedText = !_translate ? _message : VP_LocalizationManager.GetText(_message);

	        if (translatedText.IsNullOrEmpty())
		        translatedText = _message;

            VP_DialogDirectMessage directmsg = new VP_DialogDirectMessage(_customDialogMessage, new VP_DialogMessageData(_clip, _type, translatedText, _character, IsQueueActive, fadeInOut, _skippable, showDirectly, _textSpeed,
                _duration, _waitForAudioEnd, _answers, _showAnswersSameTime, _autoAnswer, _pos, textColor, _font, waitForInput, _fontSize, _timeForAnswer, _chooseNumber, _parameters, _canCancel, _soundOnContinue, _overrideTextColor, _automaticScreenTime, _onCompleteCallback, _onEndCallback, _onStartCallback,
                _onTextShown, _onAnswerChosen, _onNumberChosen, _onCancelCallback));

            if (m_queueBusy)
            {
            
                // if we are using graph, we need to trigger after it shows up
                if (!m_usingDirectMessage)
                {
                    StartListeningToOnDialogEnd(NextDirectMessageAfterGraph);
                }

                m_directMessages.Enqueue(directmsg);
            }
            else
            {
                InitDirectMessage(directmsg);
            }
        }

        public virtual void DirectMessageWithOptions(string _message, AudioClip _clip = null, DIALOG_TYPE _type = DIALOG_TYPE.REGULAR, List<VP_Dialog.Answer> _answers = null, UnityAction<int> _onAnswerChosen = null, UnityAction _onCancelCallback = null,
            bool _showAnswersSameTime = true, int _autoAnswer = -1, bool _translate = false, VP_DialogMessage _customDialogMessage = null, UnityAction _onCompleteCallback = null, UnityAction _onStartCallback = null, UnityAction _onEndCallback = null,
           UnityAction _onTextShown = null, VP_DialogPositionData _pos = null, bool _skippable = true, bool waitForInput = true, float _duration = 0.5f, bool showDirectly = false, bool fadeInOut = true, bool _soundOnContinue = true,
            VP_DialogCharacterData _character = null, float _textSpeed = 1f, bool _waitForAudioEnd = true, bool _overrideTextColor = false, Color textColor = default(Color), TMPro.TMP_FontAsset _font = null, float _fontSize = 45f,
             float _timeForAnswer = 5f, bool _chooseNumber = false, Vector3 _parameters = default(Vector3), bool _canCancel = true, float _automaticScreenTime = 1f, UnityAction<int> _onNumberChosen = null)
        {
            string translatedText = !_translate ? _message : VP_LocalizationManager.GetText(_message);
	        
	        if (translatedText.IsNullOrEmpty())
		        translatedText = _message;
		        
            VP_DialogDirectMessage directmsg = new VP_DialogDirectMessage(_customDialogMessage, new VP_DialogMessageData(_clip, _type, translatedText, _character, IsQueueActive, fadeInOut, _skippable, showDirectly, _textSpeed,
                _duration, _waitForAudioEnd, _answers, _showAnswersSameTime, _autoAnswer, _pos, textColor, _font, waitForInput, _fontSize, _timeForAnswer, _chooseNumber, _parameters, _canCancel, _soundOnContinue, _overrideTextColor, _automaticScreenTime, _onCompleteCallback, _onEndCallback, _onStartCallback,
                _onTextShown, _onAnswerChosen, _onNumberChosen, _onCancelCallback));

            if (m_queueBusy)
            {
                // if we are using graph, we need to trigger after it shows up
                if (!m_usingDirectMessage)
                {
                    StartListeningToOnDialogEnd(NextDirectMessageAfterGraph);
                }

                m_directMessages.Enqueue(directmsg);
            }
            else
            {
                InitDirectMessage(directmsg);
            }
        }

        public virtual GameObject InstantiatePrefab(GameObject _prefab, Transform _parent)
        {
            return Instantiate(_prefab, _parent);
        }

        public virtual GameObject InstantiatePrefab(GameObject _prefab, Vector3 _position, Quaternion _rotation)
        {
            return Instantiate(_prefab, _position, _rotation);
        }

        public virtual void InitDirectMessageAndGraph(VP_DialogDirectMessage directmsg)
        {
            if (m_talkingDialog != null)
                m_talkingDialog.gameObject.SetActive(false);

            VP_DialogMessage customMsg = directmsg.CustomMessage;

            m_talkingDialog = (customMsg != null) ? customMsg : (m_talkingDialog == null ? m_defaultDialogMessage : m_talkingDialog);

            if (m_dialogCanvas == null)
                m_dialogCanvas = m_defaultCanvas;

            if (m_alwaysParentDialogWithCanvas && m_talkingDialog.transform.parent != m_dialogCanvas)
                m_dialogCanvas = m_talkingDialog.transform.parent;

            if (m_deactivateCanvasOnFadeOut)
                m_dialogCanvas.gameObject.SetActive(true);

            m_usingDirectMessage = true;

            ShowMessage(m_currentChart, directmsg.Data);
        }

        public virtual void InitDirectMessage(VP_DialogDirectMessage directmsg)
        {
            if (m_talkingDialog != null)
                m_talkingDialog.gameObject.SetActive(false);

            VP_DialogMessage customMsg = directmsg.CustomMessage;

            m_talkingDialog = (customMsg != null) ? customMsg : (m_talkingDialog == null ? m_defaultDialogMessage : m_talkingDialog);

            if (m_dialogCanvas == null)
                m_dialogCanvas = m_defaultCanvas;

            if (!m_talkingDialog)
            {
                VP_Debug.LogError("No talking dialog");
                
                if (directmsg != null && directmsg.Data != null && directmsg.Data.m_answers != null && directmsg.Data.m_answers.Count > 0)
                    Answer(0);
                else
                    ContinueText();

                return;
            }

            if (m_alwaysParentDialogWithCanvas && m_talkingDialog.transform.parent != m_dialogCanvas)
                m_dialogCanvas = m_talkingDialog.transform.parent;

            if (m_deactivateCanvasOnFadeOut)
                m_dialogCanvas.gameObject.SetActive(true);

            m_usingDirectMessage = true;

            ShowMessage(null, directmsg.Data);
        }
        /// <summary>
        /// Manual choose numbers
        /// </summary>
        /// <param name="parameters">
        /// x = Min
        /// y = Max
        /// z = default
        /// </param>
        public virtual void ChooseNumber(string _message, Vector3 _parameters = default(Vector3), UnityAction<int> _onNumberChosen = null, UnityAction _onNumberCancel = null, AudioClip _clip = null, DIALOG_TYPE _type = DIALOG_TYPE.REGULAR, bool _translate = false,
           VP_DialogMessage _customDialogMessage = null, UnityAction _onCompleteCallback = null, UnityAction _onStartCallback = null, UnityAction _onEndCallback = null, UnityAction _onTextShown = null,
            VP_DialogPositionData _pos = null, bool _skippable = true, bool waitForInput = true, float _duration = 0.5f, bool showDirectly = false, bool fadeInOut = true, bool _soundOnContinue = true,
            VP_DialogCharacterData _character = null, float _textSpeed = 1f, bool _waitForAudioEnd = true, bool _overrideTextColor = false, Color textColor = default(Color), TMPro.TMP_FontAsset _font = null, float _fontSize = 45f,
             float _timeForAnswer = 5f, bool _canCancel = true, List<VP_Dialog.Answer> _answers = null, bool _showAnswersSameTime = true, int _autoAnswer = -1, float _automaticScreenTime = 1f)
        {
            string translatedText = !_translate ? _message : VP_LocalizationManager.GetText(_message);
	        
	        if (translatedText.IsNullOrEmpty())
		        translatedText = _message;
		        
            VP_DialogDirectMessage directmsg = new VP_DialogDirectMessage(_customDialogMessage, new VP_DialogMessageData(_clip, _type, translatedText, _character, IsQueueActive, fadeInOut, _skippable, showDirectly, _textSpeed,
                _duration, _waitForAudioEnd, _answers, _showAnswersSameTime, _autoAnswer, _pos, textColor, _font, waitForInput, _fontSize, _timeForAnswer, true, _parameters, _canCancel, _soundOnContinue, _overrideTextColor, _automaticScreenTime, _onCompleteCallback, _onEndCallback, _onStartCallback,
                _onTextShown, null, _onNumberChosen, _onNumberCancel));

            if (m_queueBusy)
            {
                // if we are using graph, we need to trigger after it shows up
                if (!m_usingDirectMessage)
                {
                    StartListeningToOnDialogEnd(NextDirectMessageAfterGraph);
                }

                m_directMessages.Enqueue(directmsg);
            }
            else
            {
                InitDirectMessage(directmsg);
            }
        }


        public virtual void ChooseNumberInGraph(string _message, Vector3 _parameters = default(Vector3), UnityAction<int> _onNumberChosen = null, UnityAction _onNumberCancel = null, AudioClip _clip = null, DIALOG_TYPE _type = DIALOG_TYPE.REGULAR,
            bool _translate = false, VP_DialogMessage _customDialogMessage = null, UnityAction _onCompleteCallback = null, UnityAction _onStartCallback = null, UnityAction _onEndCallback = null, UnityAction _onTextShown = null,
            VP_DialogPositionData _pos = null, bool _skippable = true, bool waitForInput = true, float _duration = 0.5f, bool showDirectly = false, bool fadeInOut = true, bool _soundOnContinue = true,
            VP_DialogCharacterData _character = null, float _textSpeed = 1f, bool _waitForAudioEnd = true, bool _overrideTextColor = false, Color textColor = default(Color), TMPro.TMP_FontAsset _font = null, float _fontSize = 45f,
             float _timeForAnswer = 5f, bool _canCancel = true, List<VP_Dialog.Answer> _answers = null, bool _showAnswersSameTime = true, int _autoAnswer = -1, float _automaticScreenTime = 1f)
        {
            string translatedText = !_translate ? _message : VP_LocalizationManager.GetText(_message);

	        if (translatedText.IsNullOrEmpty())
		        translatedText = _message;

            VP_DialogDirectMessage directmsg = new VP_DialogDirectMessage(_customDialogMessage, new VP_DialogMessageData(_clip, _type, translatedText, _character, IsQueueActive, fadeInOut, _skippable, showDirectly, _textSpeed,
                _duration, _waitForAudioEnd, _answers, _showAnswersSameTime, _autoAnswer, _pos, textColor, _font, waitForInput, _fontSize, _timeForAnswer, true, _parameters, _canCancel, _soundOnContinue, _overrideTextColor, _automaticScreenTime, _onCompleteCallback, _onEndCallback, _onStartCallback,
                _onTextShown, null, _onNumberChosen, _onNumberCancel));

            InitDirectMessageAndGraph(directmsg);
        }

        public virtual void ChoseNumberFromMessage(int _chosenNumber)
        {
            HideMessage();

            VP_AudioManager.Instance.RemoveItem(VP_AudioSetup.Dialog.ANSWER_CLIP);
            VP_AudioManager.Instance.RemoveItem(VP_AudioSetup.Dialog.DIALOGUE_CLIP);

            if (!m_usingDirectMessage)
            {
                if (!m_currentChart)
                {
                    OnNumberChosenAction(_chosenNumber);
                    OnDialogEndAction();
                }
                else
                {
                    OnNumberChosenAction(_chosenNumber);
                }
            }
            else
            {
                //m_queueBusy = false;
                m_usingDirectMessage = false;
                if (m_directMessages.Count > 0)
                {
                    NextDirectMessage();
                }
                else
                {
                    OnNumberChosenAction(_chosenNumber);
                    OnDialogEndAction();
                }
            }
        }

        protected virtual void NextDirectMessage(bool _onComplete = true)
        {
            if (_onComplete)
                OnDialogCompleteAction();

            VP_DialogDirectMessage newMsg = m_directMessages.Dequeue();
            InitDirectMessage(newMsg);

        }

        public virtual void CancelNumberFromMessage()
        {
            HideMessage();

            VP_AudioManager.Instance.RemoveItem(VP_AudioSetup.Dialog.ANSWER_CLIP);
            VP_AudioManager.Instance.RemoveItem(VP_AudioSetup.Dialog.DIALOGUE_CLIP);

            if (!m_usingDirectMessage)
            {
                if (!m_currentChart)
                {
                    OnNumberCancelAction();
                    OnDialogEndAction();
                }
                else
                {
                    OnNumberCancelAction();
                }
            }
            else
            {
                //m_queueBusy = false;
                m_usingDirectMessage = false;
                if (m_directMessages.Count > 0)
                {
                    NextDirectMessage();
                }
                else
                {
                    OnNumberCancelAction();
                    OnDialogEndAction();
                }
            }
        }

        public virtual float GetDialogAutoTimeProgress()
        {
            return m_talkingDialog.AutoAnswerTimeProgress();
        }

        public static float GetAutoTimeProgress()
        {
            return (m_instance && IsSpeaking) ? Instance.GetDialogAutoTimeProgress() : 0.0f;
        }

        /// <summary>
        /// Set a current dialog game object as current 
        /// </summary>
        /// <param name="_value"></param>
        public virtual void SetThisDialog(VP_DialogMessage value, Transform chartCanvas)
        {
            if (value != null)
                m_talkingDialog = value;

            if (chartCanvas != null)
            {
                m_dialogCanvas = chartCanvas;
                if (m_deactivateCanvasOnFadeOut)
                    m_dialogCanvas.gameObject.SetActive(true);
            }

        }
        /// <summary>
        /// Select a new chart but not start a dialog til a message is sent
        /// </summary>
        /// <param name="_newChart"></param>
        public virtual void SelectCurrentChart(VP_DialogChart _newChart)
        {
            if (m_currentChart != _newChart)
                m_currentChart = _newChart;
        }
        /// <summary>
        /// Send the string starting message
        /// </summary>
        /// <param name="_msg"></param>
        public virtual void _SendMessage(string _msg, UnityAction _onEndCallback = null)
        {
            if (m_currentChart)
            {
                if (_onEndCallback != null)
                {
                    StartListeningToOnDialogEnd(_onEndCallback);
                }

                m_usingDirectMessage = false;
                m_currentChart.SendDialogMessage(_msg);
            }
            else
            {
                Debug.LogError("No chart is added to Dialog manager but key is trying to be used.");
            }
        }
        /// <summary>
        /// Hide the canvas
        /// </summary>
        public virtual void _HideCanvas()
        {
            if (m_deactivateCanvasOnFadeOut)
                m_dialogCanvas.gameObject.SetActive(false);
        }
        /// <summary>
        /// The called method changes the chart and starts a dialog based on the data
        /// </summary>
        /// <param name="chart"></param>
        /// <param name="_data"></param>
        public virtual void _ShowMessage(VP_DialogChart chart, VP_DialogMessageData _data)
		{
			if (m_alwaysTranslate)
			{
				_data.m_text = VP_LocalizationManager.GetText(_data.m_text);
			}
        	
            if (m_deactivateCanvasOnFadeOut)
                m_dialogCanvas.gameObject.SetActive(true);

            m_speaking = true;
            if (chart && chart != m_currentChart)
                m_currentChart = chart;

            if (m_talkingDialog == null)
            {
                m_talkingDialog = FindObjectOfType<VP_DialogMessage>();

                if (m_talkingDialog == null)
                {
                    GameObject go = Instantiate(m_dialogPrefab, m_dialogCanvas);
                    m_talkingDialog = go.GetComponent<VP_DialogMessage>();
                }
            }
            else
            {
                m_talkingDialog.gameObject.SetActive(true);
            }
            OnCharacterSpeakAction(_data.m_characterData);
            OnDialogStartAction();
            m_talkingDialog.ShowDialog(_data);
        }


        /// <summary>
        /// Continue to next text. If the dialog doesn't continue, the dialog GO, just disappears.
        /// </summary>
        public virtual void ContinueText(UnityAction _onEndCallback = null)
        {
            HideMessage();

            VP_AudioManager.Instance.RemoveItem(VP_AudioSetup.Dialog.ANSWER_CLIP);
            VP_AudioManager.Instance.RemoveItem(VP_AudioSetup.Dialog.DIALOGUE_CLIP);

            if (!m_usingDirectMessage)
            {
                if (m_currentChart)
                {
                    m_currentChart.ContinueText();
                }
                else
                {
                    _onEndCallback?.Invoke();

                    OnDialogEndAction();
                }
            }
            else
            {
                //m_queueBusy = false;
                m_usingDirectMessage = false;
                if (m_directMessages.Count > 0)
                {
                    NextDirectMessage();
                }
                else
                {
                    _onEndCallback?.Invoke();
                    OnDialogEndAction();
                }
            }
        }

        public virtual void Skipping()
        {
            OnSkipAction();
            HideMessage();

            if (!m_usingDirectMessage)
            {
                if (m_currentChart)
                    m_currentChart.ContinueText();
                else
                    OnDialogEndAction();
            }
            else
            {
                //m_queueBusy = false;
                m_usingDirectMessage = false;
                m_directMessages.Clear();
                OnDialogEndAction();
            }
        }

        public virtual void HideMessage()
        {
            m_speaking = false;

            if (m_deactivateCanvasOnFadeOut && m_dialogCanvas)
                m_dialogCanvas.gameObject.SetActive(false);

	        if (m_talkingDialog)
            	m_talkingDialog.gameObject.SetActive(false);
        }

        /// <summary>
        /// Continues the dialog from the index of the answer
        /// </summary>
        /// <param name="index"></param>
        public virtual void Answer(int index, UnityAction _answerCallback = null, UnityAction _onCompleteCallback = null, UnityAction _onEndCallback = null)
        {
            HideMessage();

            VP_AudioManager.Instance.RemoveItem(VP_AudioSetup.Dialog.ANSWER_CLIP);
            VP_AudioManager.Instance.RemoveItem(VP_AudioSetup.Dialog.DIALOGUE_CLIP);

            _onCompleteCallback?.Invoke();

            if (!m_usingDirectMessage)
            {
                if (m_currentChart != null)
                {
                    m_currentChart.AnswerText(index);
                }
                else
                {
                    if (OnChoiceSelection != null)
                    {
                        OnChoiceSelection.Invoke(index);
                    }

                    if (_answerCallback != null)
                    {
                        _answerCallback.Invoke();
                    }

                    if (!m_queueBusy)
                    {
                        _onEndCallback?.Invoke();
                        OnDialogEndAction();
                    }
                }
            }
            else
            {
                if (OnChoiceSelection != null)
                {
                    OnChoiceSelection.Invoke(index);
                }

                if (_answerCallback != null)
                {
                    _answerCallback.Invoke();
                }

                if (!m_queueBusy)
                {
                    _onEndCallback?.Invoke();
                    OnDialogEndAction();
                }
            }
        }

        public virtual void ConfirmMessage(string _message, AudioClip _clip = null, DIALOG_TYPE _type = DIALOG_TYPE.REGULAR, string _yesText = "Yes", UnityAction _yesCallback = null, string _noText = "No", UnityAction _noCallback = null, AudioClip _yesAudio = null, AudioClip _noAudio = null,
           UnityAction<int> _onselection = null, bool _showAnswersSameTime = true, int _autoAnswer = -1, bool _translate = false, VP_DialogMessage _customDialogMessage = null, float _textSpeed = 1f, float _fontSize = 45f, float _yesFontSize = 45f, float _noFontSize = 45f, bool _canCancel = true, UnityAction _onCancelCallback = null,
           UnityAction _onCompleteCallback = null, UnityAction _onStartCallback = null, UnityAction _onEndCallback = null, UnityAction _onTextShown = null, VP_DialogPositionData _pos = null, bool _skippable = true, bool waitForInput = true,
            float _duration = 0.5f, bool showDirectly = false, bool fadeInOut = true, bool _soundOnContinue = true, VP_DialogCharacterData _character = null, bool _waitForAudioEnd = true,
            bool _overrideTextColor = false, Color textColor = default(Color), TMPro.TMP_FontAsset _font = null, float _timeForAnswer = 5f, bool _chooseNumber = false, Vector3 _parameters = default(Vector3), float
            _automaticScreenTime = 1f, UnityAction<int> _onNumberChosen = null)
        {
            var lm = VP_LocalizationManager.Instance;

            List<VP_Dialog.Answer> answers = new List<VP_Dialog.Answer>()
            {
                new VP_Dialog.Answer(_translate, _yesText, true, false, false, _yesAudio, _yesCallback),
                new VP_Dialog.Answer(_translate, _noText, true, true, false, _noAudio, _noCallback)
            };

            DirectMessageWithOptions(_message, _clip, _type, answers, _onselection, _onCancelCallback, _showAnswersSameTime, _autoAnswer, _translate, _customDialogMessage, _onCompleteCallback, _onStartCallback, _onEndCallback, _onTextShown,
                _pos, _skippable, waitForInput, _duration, showDirectly, fadeInOut, _soundOnContinue, _character, _textSpeed, _waitForAudioEnd, _overrideTextColor, textColor, _font, _fontSize, _timeForAnswer, _chooseNumber, _parameters, _canCancel,
                _automaticScreenTime, _onNumberChosen);
        }

        public virtual void NoConfirmMessage(string _message, AudioClip _clip = null, DIALOG_TYPE _type = DIALOG_TYPE.REGULAR, string _yesText = "Yes", UnityAction _yesCallback = null, string _noText = "No", UnityAction _noCallback = null, AudioClip _yesAudio = null, AudioClip _noAudio = null,
           UnityAction<int> _onselection = null, bool _showAnswersSameTime = true, int _autoAnswer = -1, bool _translate = false, VP_DialogMessage _customDialogMessage = null, float _textSpeed = 1f, float _fontSize = 45f, float _yesFontSize = 45f, float _noFontSize = 45f, bool _canCancel = true, UnityAction _onCancelCallback = null,
           UnityAction _onCompleteCallback = null, UnityAction _onStartCallback = null, UnityAction _onEndCallback = null, UnityAction _onTextShown = null, VP_DialogPositionData _pos = null, bool _skippable = true, bool waitForInput = true,
            float _duration = 0.5f, bool showDirectly = false, bool fadeInOut = true, bool _soundOnContinue = true, VP_DialogCharacterData _character = null, bool _waitForAudioEnd = true,
            bool _overrideTextColor = false, Color textColor = default(Color), TMPro.TMP_FontAsset _font = null, float _timeForAnswer = 5f, bool _chooseNumber = false, Vector3 _parameters = default(Vector3), float
            _automaticScreenTime = 1f, UnityAction<int> _onNumberChosen = null)
        {
            List<VP_Dialog.Answer> answers = new List<VP_Dialog.Answer>()
            {
                new VP_Dialog.Answer(_translate, _noText, true, false, false, _noAudio, _noCallback),
                new VP_Dialog.Answer(_translate, _yesText, true, true, false, _yesAudio, _yesCallback),
            };

            DirectMessageWithOptions(_message, _clip, _type, answers, _onselection, _onCancelCallback, _showAnswersSameTime, _autoAnswer, _translate, _customDialogMessage, _onCompleteCallback, _onStartCallback, _onEndCallback, _onTextShown,
                _pos, _skippable, waitForInput, _duration, showDirectly, fadeInOut, _soundOnContinue, _character, _textSpeed, _waitForAudioEnd, _overrideTextColor, textColor, _font, _fontSize, _timeForAnswer, _chooseNumber, _parameters, _canCancel,
                _automaticScreenTime, _onNumberChosen);
        }



        public static void SetCurrentChart(VP_DialogChart _chart)
        {
            if (m_instance)
                Instance.SelectCurrentChart(_chart);
        }

        public static void ShowConfirmMessage(string _message, AudioClip _clip = null, DIALOG_TYPE _type = DIALOG_TYPE.REGULAR, string _yesText = "Yes", UnityAction _yesCallback = null, string _noText = "No", UnityAction _noCallback = null, AudioClip _yesAudio = null, AudioClip _noAudio = null,
           UnityAction<int> _onselection = null, bool _showAnswersSameTime = true, int _autoAnswer = -1, bool _translate = false, VP_DialogMessage _customDialogMessage = null, float _textSpeed = 1f, float _fontSize = 45f, float _yesFontSize = 45f, float _noFontSize = 45f, bool _canCancel = true, UnityAction _onCancelCallback = null,
           UnityAction _onCompleteCallback = null, UnityAction _onStartCallback = null, UnityAction _onEndCallback = null, UnityAction _onTextShown = null, VP_DialogPositionData _pos = null, bool _skippable = true, bool waitForInput = true,
            float _duration = 0.5f, bool showDirectly = false, bool fadeInOut = true, bool _soundOnContinue = true, VP_DialogCharacterData _character = null, bool _waitForAudioEnd = true,
            bool _overrideTextColor = false, Color textColor = default(Color), TMPro.TMP_FontAsset _font = null, float _timeForAnswer = 5f, bool _chooseNumber = false, Vector3 _parameters = default(Vector3), float
            _automaticScreenTime = 1f, UnityAction<int> _onNumberChosen = null)
        {
            if (Instance)
            {
                Instance.ConfirmMessage(_message, _clip, _type, _yesText, _yesCallback, _noText, _noCallback, _yesAudio, _noAudio, _onselection, _showAnswersSameTime, _autoAnswer, _translate, _customDialogMessage, _textSpeed, _fontSize, _yesFontSize, _noFontSize,
                    _canCancel, _onCancelCallback, _onCompleteCallback,_onStartCallback, _onEndCallback, _onTextShown, _pos, _skippable, waitForInput, _duration, showDirectly, fadeInOut, _soundOnContinue, _character, _waitForAudioEnd, _overrideTextColor, textColor,
                    _font, _timeForAnswer, _chooseNumber, _parameters, _automaticScreenTime, _onNumberChosen);
            }
        }

        public static void ShowNoConfirmMessage(string _message, AudioClip _clip = null, DIALOG_TYPE _type = DIALOG_TYPE.REGULAR, string _yesText = "Yes", UnityAction _yesCallback = null, string _noText = "No", UnityAction _noCallback = null, AudioClip _yesAudio = null, AudioClip _noAudio = null,
           UnityAction<int> _onselection = null, bool _showAnswersSameTime = true, int _autoAnswer = -1, bool _translate = false, VP_DialogMessage _customDialogMessage = null, float _textSpeed = 1f, float _fontSize = 45f, float _yesFontSize = 45f, float _noFontSize = 45f, bool _canCancel = true, UnityAction _onCancelCallback = null,
           UnityAction _onCompleteCallback = null, UnityAction _onStartCallback = null, UnityAction _onEndCallback = null, UnityAction _onTextShown = null, VP_DialogPositionData _pos = null, bool _skippable = true, bool waitForInput = true,
            float _duration = 0.5f, bool showDirectly = false, bool fadeInOut = true, bool _soundOnContinue = true, VP_DialogCharacterData _character = null, bool _waitForAudioEnd = true,
            bool _overrideTextColor = false, Color textColor = default(Color), TMPro.TMP_FontAsset _font = null, float _timeForAnswer = 5f, bool _chooseNumber = false, Vector3 _parameters = default(Vector3), float
            _automaticScreenTime = 1f, UnityAction<int> _onNumberChosen = null)
        {
            if (Instance)
            {
                Instance.NoConfirmMessage(_message, _clip, _type, _yesText, _yesCallback, _noText, _noCallback, _yesAudio, _noAudio, _onselection, _showAnswersSameTime, _autoAnswer, _translate, _customDialogMessage, _textSpeed, _fontSize, _yesFontSize, _noFontSize,
                    _canCancel, _onCancelCallback, _onCompleteCallback, _onStartCallback, _onEndCallback, _onTextShown, _pos, _skippable, waitForInput, _duration, showDirectly, fadeInOut, _soundOnContinue, _character, _waitForAudioEnd, _overrideTextColor, textColor,
                    _font, _timeForAnswer, _chooseNumber, _parameters, _automaticScreenTime, _onNumberChosen);
            }
        }

        /// <summary>
        /// Static methods that only calls a method if the instance is in the scene to avoid nulls
        /// 
        /// The called method changes the chart and starts a dialog
        /// </summary>
        /// <param name="chart"></param>
        /// <param name="_data"></param>
        public static void ShowMessage(VP_DialogChart chart, VP_DialogMessageData _data)
        {
            if (Instance)
                Instance._ShowMessage(chart, _data);
        }
        /// <summary>
        /// Static methods that only calls a method if the instance is in the scene to avoid nulls
        /// 
        /// The called method tries to start the dialog
        /// </summary>
        /// <param name="chart"></param>
        /// <param name="_data"></param>
        public static void SendDialogMessage(string _key, VP_DialogChart _chart = null, UnityAction _onEndCallback = null, VP_DialogMessage _customMessage = null)
        {
            if (m_instance)
            {
                var instance = Instance;

                if (_chart != null)
                    instance.CurrentChart = _chart;

                if (_customMessage != null)
                {
                    instance.CurrentChart.CurrentDialogMessage = _customMessage;
                }

                instance._SendMessage(_key, _onEndCallback);
            }

        }

        public static void SendMessage(string _key, VP_DialogChart _chart = null, UnityAction _onEndCallback = null, VP_DialogMessage _customMessage = null)
        {
            if (m_instance)
            {
                var instance = Instance;

                if (_chart != null)
                    instance.CurrentChart = _chart;

                if (_customMessage != null)
                {
                    instance.CurrentChart.CurrentDialogMessage = _customMessage;
                }

                instance._SendMessage(_key, _onEndCallback);
            }

        }

        /// <summary>
        /// Static methods that only calls a method if the instance is in the scene to avoid nulls
        /// 
        /// The called method tries to start the dialog
        /// </summary>
        /// <param name="chart"></param>
        /// <param name="_data"></param>
        public static void SendDialogMessageOnGraph(string _key, VP_DialogGraph _graph = null)
        {
            if (m_instance)
            {
                var instance = Instance;

                if (_graph != null)
                {

                    if (instance.CurrentChart != null)
                    {
                        instance.m_currentChart.Graph = (_graph);
                    }
                    else
                    {
                        Debug.LogError("No chart is added to dialgo manager, so graph can't be assigned.");

                        return;
                    }
                }

                instance._SendMessage(_key);
            }

        }

        public static void ShowDirectMessageInPosition(string _message, VP_DialogPositionData _pos = null, AudioClip _clip = null, DIALOG_TYPE _type = DIALOG_TYPE.REGULAR, bool _translate = false, VP_DialogMessage _customDialogMessage = null,
            UnityAction _onCompleteCallback = null, UnityAction _onStartCallback = null, UnityAction _onEndCallback = null, UnityAction _onTextShown = null, bool showDirectly = false, bool fadeInOut = true,
            bool _skippable = true, bool waitForInput = true, float _duration = 0.5f, bool _soundOnContinue = true, VP_DialogCharacterData _character = null, float _textSpeed = 1f, bool _waitForAudioEnd = true,
            List<VP_Dialog.Answer> _answers = null, UnityAction<int> _onAnswerChosen = null, bool _showAnswersSameTime = true, int _autoAnswer = -1, bool _overrideTextColor = false, Color textColor = default(Color), TMPro.TMP_FontAsset _font = null, float _fontSize = 45f,
             float _timeForAnswer = 5f, bool _chooseNumber = false, Vector3 _parameters = default(Vector3), bool _canCancel = true, float _automaticScreenTime = 1f, UnityAction<int> _onNumberChosen = null, UnityAction _onCancelCallback = null)
        {
            if (m_instance)
                Instance.DirectMessage(_message, _clip, _type, _translate, showDirectly, fadeInOut, _customDialogMessage, _onCompleteCallback, _onStartCallback, _onEndCallback, _onTextShown,
                _pos, _skippable, waitForInput, _duration, _soundOnContinue, _character, _textSpeed, _waitForAudioEnd, _answers, _onAnswerChosen, _showAnswersSameTime, _autoAnswer, _overrideTextColor, textColor, _font, _fontSize,
                _timeForAnswer, _chooseNumber, _parameters, _canCancel, _automaticScreenTime, _onNumberChosen, _onCancelCallback);
        }

        public static void ChooseNumberWithParams(string _message, Vector3 _parameters = default(Vector3), bool _canCancel = true, UnityAction<int> _onNumberChosen = null, UnityAction _onNumberCancel = null, AudioClip _clip = null, DIALOG_TYPE _type = DIALOG_TYPE.REGULAR, bool _translate = false, bool showDirectly = false, bool fadeInOut = true, VP_DialogMessage _customDialogMessage = null,
           UnityAction _onCompleteCallback = null, UnityAction _onStartCallback = null, UnityAction _onEndCallback = null, UnityAction _onTextShown = null, VP_DialogPositionData _pos = null,
            bool _skippable = true, bool waitForInput = true, float _duration = 0.5f, bool _soundOnContinue = true, VP_DialogCharacterData _character = null, float _textSpeed = 1f, bool _waitForAudioEnd = true,
            bool _overrideTextColor = false, Color textColor = default(Color), TMPro.TMP_FontAsset _font = null, float _fontSize = 45f, float _automaticScreenTime = 1f)
        {
            if (m_instance)
                Instance.ChooseNumber(_message, _parameters, _onNumberChosen, _onNumberCancel, _clip, _type, _translate, _customDialogMessage, _onCompleteCallback, _onStartCallback, _onEndCallback, _onTextShown, _pos, _skippable, waitForInput, _duration, showDirectly, fadeInOut,
                    _soundOnContinue, _character, _textSpeed, _waitForAudioEnd, _overrideTextColor, textColor, _font, _fontSize, -1f, _canCancel, null, true, -1, _automaticScreenTime);
        }

        public static void ChooseNumberWithParamsInGraph(string _message, Vector3 _parameters = default(Vector3), UnityAction<int> _onNumberChosen = null, bool _canCancel = true, UnityAction _onNumberCancel = null, AudioClip _clip = null, DIALOG_TYPE _type = DIALOG_TYPE.REGULAR,
            bool _translate = false, VP_DialogMessage _customDialogMessage = null, VP_DialogPositionData _pos = null, UnityAction _onCompleteCallback = null, UnityAction _onStartCallback = null, UnityAction _onEndCallback = null, UnityAction _onTextShown = null,
            bool _skippable = true, bool waitForInput = true, float _duration = 0.5f, bool showDirectly = false, bool fadeInOut = true, bool _soundOnContinue = true,
            VP_DialogCharacterData _character = null, float _textSpeed = 1f, bool _waitForAudioEnd = true, bool _overrideTextColor = false, Color textColor = default(Color), TMPro.TMP_FontAsset _font = null, float _fontSize = 45f,
             float _timeForAnswer = 5f, List<VP_Dialog.Answer> _answers = null, bool _showAnswersSameTime = true, int _autoAnswer = -1, float _automaticScreenTime = 1f)
        {
            if (m_instance)
                Instance.ChooseNumberInGraph(_message, _parameters, _onNumberChosen, _onNumberCancel, _clip, _type, _translate, _customDialogMessage, _onCompleteCallback, _onStartCallback, _onEndCallback, _onTextShown,
                _pos, _skippable, waitForInput, _duration, showDirectly, fadeInOut, _soundOnContinue, _character, _textSpeed, _waitForAudioEnd, _overrideTextColor, textColor, _font, _fontSize, _timeForAnswer, _canCancel, _answers,
                _showAnswersSameTime, _autoAnswer, _automaticScreenTime);
        }

        public static void ShowDirectMessage(string _message, AudioClip _clip = null, DIALOG_TYPE _type = DIALOG_TYPE.REGULAR, bool _translate = false, bool showDirectly = false, bool fadeInOut = true, VP_DialogMessage _customDialogMessage = null,
            UnityAction _onCompleteCallback = null, UnityAction _onStartCallback = null, UnityAction _onEndCallback = null, UnityAction _onTextShown = null, VP_DialogPositionData _pos = null,
            bool _skippable = true, bool waitForInput = true, float _duration = 0.5f, bool _soundOnContinue = true, VP_DialogCharacterData _character = null, float _textSpeed = 1f, bool _waitForAudioEnd = true,
            List<VP_Dialog.Answer> _answers = null, UnityAction<int> _onAnswerChosen = null, bool _showAnswersSameTime = true, int _autoAnswer = -1, bool _overrideTextColor = false, Color textColor = default(Color), TMPro.TMP_FontAsset _font = null, float _fontSize = 45f,
             float _timeForAnswer = 5f, bool _chooseNumber = false, Vector3 _parameters = default(Vector3), bool _canCancel = true, float _automaticScreenTime = 1f, UnityAction<int> _onNumberChosen = null, UnityAction _onCancelCallback = null)
		{
            if (m_instance)
                Instance.DirectMessage(_message, _clip, _type, _translate, showDirectly, fadeInOut, _customDialogMessage, _onCompleteCallback, _onStartCallback, _onEndCallback, _onTextShown,
                _pos, _skippable, waitForInput, _duration, _soundOnContinue, _character, _textSpeed, _waitForAudioEnd, _answers, _onAnswerChosen, _showAnswersSameTime, _autoAnswer, _overrideTextColor, textColor, _font, _fontSize,
                _timeForAnswer, _chooseNumber, _parameters, _canCancel, _automaticScreenTime, _onNumberChosen, _onCancelCallback);
            else
	            Debug.LogError("No instance for VP_DialogManager");
        
        }

        public static void ShowDirectMessage(string _message, bool _translate, VP_DialogMessage _customDialogMessage = null, DIALOG_TYPE _type = DIALOG_TYPE.REGULAR, float _textSpeed = 1f,
            float _duration = 0.5f, bool _skippable = true, bool waitForInput = true, float _fontSize = 45f, UnityAction _onCompleteCallback = null, UnityAction _onStartCallback = null, UnityAction _onEndCallback = null, UnityAction _onTextShown = null, 
            VP_DialogPositionData _pos = null, AudioClip _clip = null, bool _soundOnContinue = true, VP_DialogCharacterData _character = null,  bool _waitForAudioEnd = true, bool showDirectly = false, bool fadeInOut = true, List<VP_Dialog.Answer> _answers = null, 
            UnityAction<int> _onAnswerChosen = null, bool _showAnswersSameTime = true, int _autoAnswer = -1, bool _overrideTextColor = false, Color textColor = default(Color), TMPro.TMP_FontAsset _font = null,
            float _timeForAnswer = 5f, bool _chooseNumber = false, Vector3 _parameters = default(Vector3), bool _canCancel = true, float _automaticScreenTime = 1f, UnityAction<int> _onNumberChosen = null, UnityAction _onCancelCallback = null)
        {
            if (m_instance)
                Instance.DirectMessage(_message, _clip, _type, _translate, showDirectly, fadeInOut, _customDialogMessage, _onCompleteCallback, _onStartCallback, _onEndCallback, _onTextShown,
                _pos, _skippable, waitForInput, _duration, _soundOnContinue, _character, _textSpeed, _waitForAudioEnd, _answers, _onAnswerChosen, _showAnswersSameTime, _autoAnswer, _overrideTextColor, textColor, _font, _fontSize,
                _timeForAnswer, _chooseNumber, _parameters, _canCancel, _automaticScreenTime, _onNumberChosen, _onCancelCallback);
        }


        public static void ShowDirectMessageWithCharacter(string _message, AudioClip _clip = null, DIALOG_TYPE _type = DIALOG_TYPE.REGULAR, VP_DialogCharacterData _character = null, VP_DialogPositionData _pos = null, bool _translate = false,
            bool showDirectly = false, bool fadeInOut = true, VP_DialogMessage _customDialogMessage = null, UnityAction _onCompleteCallback = null, UnityAction _onStartCallback = null, UnityAction _onEndCallback = null, UnityAction _onTextShown = null,
            bool _skippable = true, bool waitForInput = true, float _duration = 0.5f, bool _soundOnContinue = true, float _textSpeed = 1f, bool _waitForAudioEnd = true,
            List<VP_Dialog.Answer> _answers = null, bool _showAnswersSameTime = true, int _autoAnswer = -1, bool _overrideTextColor = false, Color textColor = default(Color), TMPro.TMP_FontAsset _font = null, float _fontSize = 45f,
             float _timeForAnswer = 5f, bool _chooseNumber = false, Vector3 _parameters = default(Vector3), bool _canCancel = true, float _automaticScreenTime = 1f, UnityAction<int> _onNumberChosen = null, UnityAction _onCancelCallback = null,
             UnityAction<int> _onAnswerChosen = null)
        {
            if (m_instance)
                Instance.DirectMessage(_message, _clip, _type, _translate, showDirectly, fadeInOut, _customDialogMessage, _onCompleteCallback, _onStartCallback, _onEndCallback, _onTextShown,
                _pos, _skippable, waitForInput, _duration, _soundOnContinue, _character, _textSpeed, _waitForAudioEnd, _answers, _onAnswerChosen, _showAnswersSameTime, _autoAnswer, _overrideTextColor, textColor, _font, _fontSize,
                _timeForAnswer, _chooseNumber, _parameters, _canCancel, _automaticScreenTime, _onNumberChosen, _onCancelCallback);
        }


        public static void ShowDirectMessageAndOptions(string _message, AudioClip _clip = null, DIALOG_TYPE _type = DIALOG_TYPE.REGULAR, List<VP_Dialog.Answer> _answers = null, UnityAction<int> _onAnswerChosen = null, bool _showAnswersSameTime = true,
            int _autoAnswer = -1, bool _translate = false, bool showDirectly = false, bool fadeInOut = true, VP_DialogMessage _customDialogMessage = null, UnityAction _onCompleteCallback = null, UnityAction _onStartCallback = null, UnityAction _onEndCallback = null,
           UnityAction _onTextShown = null, VP_DialogPositionData _pos = null, bool _skippable = true, bool waitForInput = true, float _duration = 0.5f, bool _soundOnContinue = true, VP_DialogCharacterData _character = null, float _textSpeed = 1f,
            bool _waitForAudioEnd = true, bool _overrideTextColor = false, Color textColor = default(Color), TMPro.TMP_FontAsset _font = null, float _fontSize = 45f,
             float _timeForAnswer = 5f, bool _chooseNumber = false, Vector3 _parameters = default(Vector3), bool _canCancel = true, float _automaticScreenTime = 1f, UnityAction<int> _onNumberChosen = null, UnityAction _onCancelCallback = null)
        {
            if (m_instance)
                Instance.DirectMessageWithOptions(_message, _clip, _type, _answers, _onAnswerChosen, _onCancelCallback, _showAnswersSameTime, _autoAnswer, _translate, _customDialogMessage, _onCompleteCallback, _onStartCallback, _onEndCallback, _onTextShown,
                    _pos, _skippable, waitForInput, _duration, showDirectly, fadeInOut, _soundOnContinue, _character, _textSpeed, _waitForAudioEnd, _overrideTextColor, textColor, _font, _fontSize, _timeForAnswer, _chooseNumber, _parameters, _canCancel,
                    _automaticScreenTime, _onAnswerChosen);
        }

        public static void EndCurrentDialog()
        {
            if (m_instance)
                Instance.HideMessage();
        }
        
	    public void SetVariablesFromDatabase(VP_VariableDataBase database)
	    {
		    VP_BoolVariables loadedBools = database.GetBoolVariableList;
		    VP_IntVariables loadedInts = database.GetIntVariableList;
		    VP_FloatVariables loadedFloats = database.GetFloatVariableList;
		    VP_DoubleVariables loadedDoubles = database.GetDoubleVariableList;
		    VP_StringVariables loadedStrings = database.GetStringVariableList;
		    VP_GameObjectVariables loadedGos = database.GetGameObjectVariableList;
		    VP_UnityObjectVariables loadedUOs = database.GetUnityObjectVariableList;

		    VP_BoolVariables gBools = new VP_BoolVariables();
		    VP_IntVariables gInts = new VP_IntVariables();
		    VP_FloatVariables gFloats = new VP_FloatVariables();
		    VP_DoubleVariables gDoubles = new VP_DoubleVariables();
		    VP_StringVariables gStrings = new VP_StringVariables();
		    VP_GameObjectVariables gGos = new VP_GameObjectVariables();
		    VP_UnityObjectVariables gUOs = new VP_UnityObjectVariables();

		    // We copy for not modifying original values in runtime

		    gBools.CopyFrom(loadedBools);
		    gInts.CopyFrom(loadedInts);
		    gFloats.CopyFrom(loadedFloats);
		    gDoubles.CopyFrom(loadedDoubles);
		    gStrings.CopyFrom(loadedStrings);
		    gGos.CopyFrom(loadedGos);
		    gUOs.CopyFrom(loadedUOs);

		    m_globalDatabase.SetBoolVariableList = gBools;
		    m_globalDatabase.SetIntVariableList = gInts;
		    m_globalDatabase.SetFloatVariableList = gFloats;
		    m_globalDatabase.SetDoubleVariableList = gDoubles;
		    m_globalDatabase.SetStringVariableList = gStrings;
		    m_globalDatabase.SetGameObjectVariableList = gGos;
		    m_globalDatabase.SetUnityObjectVariableList = gUOs;
	    }
    }
}