using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Dialog
{
    [System.Serializable]
    public class VP_DialogMessageAxisInputData
    {
        public enum DATA_TYPE
        {
            NORMAL,
            RAW
        }

        public DATA_TYPE m_dataType = DATA_TYPE.NORMAL;
        public VariableComparison m_compare = VariableComparison.Equal;
        public float m_value = 0f;
    }

    public class VP_DialogMessageInput : MonoBehaviour
    {
        public enum DIALOG_INPUT_TYPE
        {
            AXIS,
            BUTTON,
            KEYCODE,
            CUSTOM
        }

        public enum PRESS_TYPE
        {
            DOWN,
            HELD,
            UP
        }

        public enum EVENT_TYPE
        {
            DELEGATE,
            EVENTMANAGER
        }

        [SerializeField] private EVENT_TYPE m_handleType = EVENT_TYPE.DELEGATE;
        [SerializeField] private List<DIALOG_INPUT_TYPE> m_inputTypes = new List<DIALOG_INPUT_TYPE>();

        [Header("Input.Button")]
        [SerializeField] private VP_DialogMessageInputButtonDic m_interactButtons = new VP_DialogMessageInputButtonDic();
        [SerializeField] private VP_DialogMessageInputButtonDic m_cancelButtons = new VP_DialogMessageInputButtonDic();
        [SerializeField] private VP_DialogMessageInputButtonDic m_upButtons = new VP_DialogMessageInputButtonDic();
        [SerializeField] private VP_DialogMessageInputButtonDic m_downButtons = new VP_DialogMessageInputButtonDic();
        [SerializeField] private VP_DialogMessageInputButtonDic m_rightButtons = new VP_DialogMessageInputButtonDic();
        [SerializeField] private VP_DialogMessageInputButtonDic m_leftButtons = new VP_DialogMessageInputButtonDic();

        [Header("Input.KeyCode")]
        [SerializeField] private VP_DialogMessageInputKeyCodeDic m_interactKeys = new VP_DialogMessageInputKeyCodeDic();
        [SerializeField] private VP_DialogMessageInputKeyCodeDic m_cancelKeys = new VP_DialogMessageInputKeyCodeDic();
        [SerializeField] private VP_DialogMessageInputKeyCodeDic m_upKeys = new VP_DialogMessageInputKeyCodeDic();
        [SerializeField] private VP_DialogMessageInputKeyCodeDic m_downKeys = new VP_DialogMessageInputKeyCodeDic();
        [SerializeField] private VP_DialogMessageInputKeyCodeDic m_rightKeys = new VP_DialogMessageInputKeyCodeDic();
        [SerializeField] private VP_DialogMessageInputKeyCodeDic m_leftKeys = new VP_DialogMessageInputKeyCodeDic();

        [Header("Input.Axis")]
        [SerializeField] private VP_DialogMessageInputAxisDic m_interactAxis = new VP_DialogMessageInputAxisDic();
        [SerializeField] private VP_DialogMessageInputAxisDic m_cancelAxis = new VP_DialogMessageInputAxisDic();
        [SerializeField] private VP_DialogMessageInputAxisDic m_upAxis = new VP_DialogMessageInputAxisDic();
        [SerializeField] private VP_DialogMessageInputAxisDic m_downAxis = new VP_DialogMessageInputAxisDic();
        [SerializeField] private VP_DialogMessageInputAxisDic m_rightAxis = new VP_DialogMessageInputAxisDic();
        [SerializeField] private VP_DialogMessageInputAxisDic m_leftAxis = new VP_DialogMessageInputAxisDic();

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (m_inputTypes.Contains(DIALOG_INPUT_TYPE.BUTTON))
            {
                foreach (string str in m_interactButtons.Keys)
                {
                    switch (m_interactButtons[str])
                    {
                        case PRESS_TYPE.DOWN:
                            if (Input.GetButtonDown(str))
                            {
                                Interact();
                            }
                            break;
                        case PRESS_TYPE.UP:
                            if (Input.GetButtonUp(str))
                            {
                                Interact();
                            }
                            break;
                        case PRESS_TYPE.HELD:
                            if (Input.GetButton(str))
                            {
                                Interact();
                            }
                            break;
                    }
                }

                foreach (string str in m_cancelButtons.Keys)
                {
                    switch (m_cancelButtons[str])
                    {
                        case PRESS_TYPE.DOWN:
                            if (Input.GetButtonDown(str))
                            {
                                Cancel();
                            }
                            break;
                        case PRESS_TYPE.UP:
                            if (Input.GetButtonUp(str))
                            {
                                Cancel();
                            }
                            break;
                        case PRESS_TYPE.HELD:
                            if (Input.GetButton(str))
                            {
                                Cancel();
                            }
                            break;
                    }
                }

                foreach (string str in m_downButtons.Keys)
                {
                    switch (m_downButtons[str])
                    {
                        case PRESS_TYPE.DOWN:
                            if (Input.GetButtonDown(str))
                            {
                                DownAnswer();
                            }
                            break;
                        case PRESS_TYPE.UP:
                            if (Input.GetButtonUp(str))
                            {
                                DownAnswer();
                            }
                            break;
                        case PRESS_TYPE.HELD:
                            if (Input.GetButton(str))
                            {
                                DownAnswer();
                            }
                            break;
                    }
                }

                foreach (string str in m_upButtons.Keys)
                {
                    switch (m_upButtons[str])
                    {
                        case PRESS_TYPE.DOWN:
                            if (Input.GetButtonDown(str))
                            {
                                UpAnswer();
                            }
                            break;
                        case PRESS_TYPE.UP:
                            if (Input.GetButtonUp(str))
                            {
                                UpAnswer();
                            }
                            break;
                        case PRESS_TYPE.HELD:
                            if (Input.GetButton(str))
                            {
                                UpAnswer();
                            }
                            break;
                    }
                }

                foreach (string str in m_rightButtons.Keys)
                {
                    switch (m_rightButtons[str])
                    {
                        case PRESS_TYPE.DOWN:
                            if (Input.GetButtonDown(str))
                            {
                                RightAnswer();
                            }
                            break;
                        case PRESS_TYPE.UP:
                            if (Input.GetButtonUp(str))
                            {
                                RightAnswer();
                            }
                            break;
                        case PRESS_TYPE.HELD:
                            if (Input.GetButton(str))
                            {
                                RightAnswer();
                            }
                            break;
                    }
                }

                foreach (string str in m_leftButtons.Keys)
                {
                    switch (m_rightButtons[str])
                    {
                        case PRESS_TYPE.DOWN:
                            if (Input.GetButtonDown(str))
                            {
                                LeftAnswer();
                            }
                            break;
                        case PRESS_TYPE.UP:
                            if (Input.GetButtonUp(str))
                            {
                                LeftAnswer();
                            }
                            break;
                        case PRESS_TYPE.HELD:
                            if (Input.GetButton(str))
                            {
                                LeftAnswer();
                            }
                            break;
                    }
                }
            }
            
            if (m_inputTypes.Contains(DIALOG_INPUT_TYPE.AXIS))
            {
                foreach (string str in m_interactAxis.Keys)
                {
                    VP_DialogMessageAxisInputData data = m_interactAxis[str];
                    switch (data.m_dataType)
                    {
                        case VP_DialogMessageAxisInputData.DATA_TYPE.NORMAL:
                            switch (data.m_compare)
                            {
                                case VariableComparison.Equal:
                                    if (Input.GetAxis(str) == data.m_value)
                                    {
                                        Interact();
                                    }
                                    break;
                                case VariableComparison.Mayor:
                                    if (Input.GetAxis(str) > data.m_value)
                                    {
                                        Interact();
                                    }
                                    break;
                                case VariableComparison.Minor:
                                    if (Input.GetAxis(str) < data.m_value)
                                    {
                                        Interact();
                                    }
                                    break;
                                case VariableComparison.MinorEqual:
                                    if (Input.GetAxis(str) <= data.m_value)
                                    {
                                        Interact();
                                    }
                                    break;
                                case VariableComparison.MayorEqual:
                                    if (Input.GetAxis(str) >= data.m_value)
                                    {
                                        Interact();
                                    }
                                    break;
                            }
                            break;
                        case VP_DialogMessageAxisInputData.DATA_TYPE.RAW:
                            switch (data.m_compare)
                            {
                                case VariableComparison.Equal:
                                    if (Input.GetAxisRaw(str) == data.m_value)
                                    {
                                        Interact();
                                    }
                                    break;
                                case VariableComparison.Mayor:
                                    if (Input.GetAxisRaw(str) > data.m_value)
                                    {
                                        Interact();
                                    }
                                    break;
                                case VariableComparison.Minor:
                                    if (Input.GetAxisRaw(str) < data.m_value)
                                    {
                                        Interact();
                                    }
                                    break;
                                case VariableComparison.MinorEqual:
                                    if (Input.GetAxisRaw(str) <= data.m_value)
                                    {
                                        Interact();
                                    }
                                    break;
                                case VariableComparison.MayorEqual:
                                    if (Input.GetAxisRaw(str) >= data.m_value)
                                    {
                                        Interact();
                                    }
                                    break;
                            }
                            break;
                    }
                }

                foreach (string str in m_cancelAxis.Keys)
                {
                    VP_DialogMessageAxisInputData data = m_cancelAxis[str];
                    switch (data.m_dataType)
                    {
                        case VP_DialogMessageAxisInputData.DATA_TYPE.NORMAL:
                            switch (data.m_compare)
                            {
                                case VariableComparison.Equal:
                                    if (Input.GetAxis(str) == data.m_value)
                                    {
                                        Cancel();
                                    }
                                    break;
                                case VariableComparison.Mayor:
                                    if (Input.GetAxis(str) > data.m_value)
                                    {
                                        Cancel();
                                    }
                                    break;
                                case VariableComparison.Minor:
                                    if (Input.GetAxis(str) < data.m_value)
                                    {
                                        Cancel();
                                    }
                                    break;
                                case VariableComparison.MinorEqual:
                                    if (Input.GetAxis(str) <= data.m_value)
                                    {
                                        Cancel();
                                    }
                                    break;
                                case VariableComparison.MayorEqual:
                                    if (Input.GetAxis(str) >= data.m_value)
                                    {
                                        Cancel();
                                    }
                                    break;
                            }
                            break;
                        case VP_DialogMessageAxisInputData.DATA_TYPE.RAW:
                            switch (data.m_compare)
                            {
                                case VariableComparison.Equal:
                                    if (Input.GetAxisRaw(str) == data.m_value)
                                    {
                                        Cancel();
                                    }
                                    break;
                                case VariableComparison.Mayor:
                                    if (Input.GetAxisRaw(str) > data.m_value)
                                    {
                                        Cancel();
                                    }
                                    break;
                                case VariableComparison.Minor:
                                    if (Input.GetAxisRaw(str) < data.m_value)
                                    {
                                        Cancel();
                                    }
                                    break;
                                case VariableComparison.MinorEqual:
                                    if (Input.GetAxisRaw(str) <= data.m_value)
                                    {
                                        Cancel();
                                    }
                                    break;
                                case VariableComparison.MayorEqual:
                                    if (Input.GetAxisRaw(str) >= data.m_value)
                                    {
                                        Cancel();
                                    }
                                    break;
                            }
                            break;
                    }
                }

                // Up
                foreach (string str in m_upAxis.Keys)
                {
                    VP_DialogMessageAxisInputData data = m_upAxis[str];
                    switch (data.m_dataType)
                    {
                        case VP_DialogMessageAxisInputData.DATA_TYPE.NORMAL:
                            switch (data.m_compare)
                            {
                                case VariableComparison.Equal:
                                    if (Input.GetAxis(str) == data.m_value)
                                    {
                                        UpAnswer();
                                    }
                                    break;
                                case VariableComparison.Mayor:
                                    if (Input.GetAxis(str) > data.m_value)
                                    {
                                        UpAnswer();
                                    }
                                    break;
                                case VariableComparison.Minor:
                                    if (Input.GetAxis(str) < data.m_value)
                                    {
                                        UpAnswer();
                                    }
                                    break;
                                case VariableComparison.MinorEqual:
                                    if (Input.GetAxis(str) <= data.m_value)
                                    {
                                        UpAnswer();
                                    }
                                    break;
                                case VariableComparison.MayorEqual:
                                    if (Input.GetAxis(str) >= data.m_value)
                                    {
                                        UpAnswer();
                                    }
                                    break;
                            }
                            break;
                        case VP_DialogMessageAxisInputData.DATA_TYPE.RAW:
                            switch (data.m_compare)
                            {
                                case VariableComparison.Equal:
                                    if (Input.GetAxisRaw(str) == data.m_value)
                                    {
                                        UpAnswer();
                                    }
                                    break;
                                case VariableComparison.Mayor:
                                    if (Input.GetAxisRaw(str) > data.m_value)
                                    {
                                        UpAnswer();
                                    }
                                    break;
                                case VariableComparison.Minor:
                                    if (Input.GetAxisRaw(str) < data.m_value)
                                    {
                                        UpAnswer();
                                    }
                                    break;
                                case VariableComparison.MinorEqual:
                                    if (Input.GetAxisRaw(str) <= data.m_value)
                                    {
                                        UpAnswer();
                                    }
                                    break;
                                case VariableComparison.MayorEqual:
                                    if (Input.GetAxisRaw(str) >= data.m_value)
                                    {
                                        UpAnswer();
                                    }
                                    break;
                            }
                            break;
                    }
                }

                // down
                foreach (string str in m_downAxis.Keys)
                {
                    VP_DialogMessageAxisInputData data = m_downAxis[str];
                    switch (data.m_dataType)
                    {
                        case VP_DialogMessageAxisInputData.DATA_TYPE.NORMAL:
                            switch (data.m_compare)
                            {
                                case VariableComparison.Equal:
                                    if (Input.GetAxis(str) == data.m_value)
                                    {
                                        DownAnswer();
                                    }
                                    break;
                                case VariableComparison.Mayor:
                                    if (Input.GetAxis(str) > data.m_value)
                                    {
                                        DownAnswer();
                                    }
                                    break;
                                case VariableComparison.Minor:
                                    if (Input.GetAxis(str) < data.m_value)
                                    {
                                        DownAnswer();
                                    }
                                    break;
                                case VariableComparison.MinorEqual:
                                    if (Input.GetAxis(str) <= data.m_value)
                                    {
                                        DownAnswer();
                                    }
                                    break;
                                case VariableComparison.MayorEqual:
                                    if (Input.GetAxis(str) >= data.m_value)
                                    {
                                        DownAnswer();
                                    }
                                    break;
                            }
                            break;
                        case VP_DialogMessageAxisInputData.DATA_TYPE.RAW:
                            switch (data.m_compare)
                            {
                                case VariableComparison.Equal:
                                    if (Input.GetAxisRaw(str) == data.m_value)
                                    {
                                        DownAnswer();
                                    }
                                    break;
                                case VariableComparison.Mayor:
                                    if (Input.GetAxisRaw(str) > data.m_value)
                                    {
                                        DownAnswer();
                                    }
                                    break;
                                case VariableComparison.Minor:
                                    if (Input.GetAxisRaw(str) < data.m_value)
                                    {
                                        DownAnswer();
                                    }
                                    break;
                                case VariableComparison.MinorEqual:
                                    if (Input.GetAxisRaw(str) <= data.m_value)
                                    {
                                        DownAnswer();
                                    }
                                    break;
                                case VariableComparison.MayorEqual:
                                    if (Input.GetAxisRaw(str) >= data.m_value)
                                    {
                                        DownAnswer();
                                    }
                                    break;
                            }
                            break;
                    }
                }

                // Right
                foreach (string str in m_rightAxis.Keys)
                {
                    VP_DialogMessageAxisInputData data = m_rightAxis[str];
                    switch (data.m_dataType)
                    {
                        case VP_DialogMessageAxisInputData.DATA_TYPE.NORMAL:
                            switch (data.m_compare)
                            {
                                case VariableComparison.Equal:
                                    if (Input.GetAxis(str) == data.m_value)
                                    {
                                        RightAnswer();
                                    }
                                    break;
                                case VariableComparison.Mayor:
                                    if (Input.GetAxis(str) > data.m_value)
                                    {
                                        RightAnswer();
                                    }
                                    break;
                                case VariableComparison.Minor:
                                    if (Input.GetAxis(str) < data.m_value)
                                    {
                                        RightAnswer();
                                    }
                                    break;
                                case VariableComparison.MinorEqual:
                                    if (Input.GetAxis(str) <= data.m_value)
                                    {
                                        RightAnswer();
                                    }
                                    break;
                                case VariableComparison.MayorEqual:
                                    if (Input.GetAxis(str) >= data.m_value)
                                    {
                                        DownAnswer();
                                    }
                                    break;
                            }
                            break;
                        case VP_DialogMessageAxisInputData.DATA_TYPE.RAW:
                            switch (data.m_compare)
                            {
                                case VariableComparison.Equal:
                                    if (Input.GetAxisRaw(str) == data.m_value)
                                    {
                                        RightAnswer();
                                    }
                                    break;
                                case VariableComparison.Mayor:
                                    if (Input.GetAxisRaw(str) > data.m_value)
                                    {
                                        RightAnswer();
                                    }
                                    break;
                                case VariableComparison.Minor:
                                    if (Input.GetAxisRaw(str) < data.m_value)
                                    {
                                        RightAnswer();
                                    }
                                    break;
                                case VariableComparison.MinorEqual:
                                    if (Input.GetAxisRaw(str) <= data.m_value)
                                    {
                                        RightAnswer();
                                    }
                                    break;
                                case VariableComparison.MayorEqual:
                                    if (Input.GetAxisRaw(str) >= data.m_value)
                                    {
                                        RightAnswer();
                                    }
                                    break;
                            }
                            break;
                    }
                }

                // Left
                foreach (string str in m_leftAxis.Keys)
                {
                    VP_DialogMessageAxisInputData data = m_leftAxis[str];
                    switch (data.m_dataType)
                    {
                        case VP_DialogMessageAxisInputData.DATA_TYPE.NORMAL:
                            switch (data.m_compare)
                            {
                                case VariableComparison.Equal:
                                    if (Input.GetAxis(str) == data.m_value)
                                    {
                                        LeftAnswer();
                                    }
                                    break;
                                case VariableComparison.Mayor:
                                    if (Input.GetAxis(str) > data.m_value)
                                    {
                                        LeftAnswer();
                                    }
                                    break;
                                case VariableComparison.Minor:
                                    if (Input.GetAxis(str) < data.m_value)
                                    {
                                        LeftAnswer();
                                    }
                                    break;
                                case VariableComparison.MinorEqual:
                                    if (Input.GetAxis(str) <= data.m_value)
                                    {
                                        LeftAnswer();
                                    }
                                    break;
                                case VariableComparison.MayorEqual:
                                    if (Input.GetAxis(str) >= data.m_value)
                                    {
                                        LeftAnswer();
                                    }
                                    break;
                            }
                            break;
                        case VP_DialogMessageAxisInputData.DATA_TYPE.RAW:
                            switch (data.m_compare)
                            {
                                case VariableComparison.Equal:
                                    if (Input.GetAxisRaw(str) == data.m_value)
                                    {
                                        LeftAnswer();
                                    }
                                    break;
                                case VariableComparison.Mayor:
                                    if (Input.GetAxisRaw(str) > data.m_value)
                                    {
                                        LeftAnswer();
                                    }
                                    break;
                                case VariableComparison.Minor:
                                    if (Input.GetAxisRaw(str) < data.m_value)
                                    {
                                        LeftAnswer();
                                    }
                                    break;
                                case VariableComparison.MinorEqual:
                                    if (Input.GetAxisRaw(str) <= data.m_value)
                                    {
                                        LeftAnswer();
                                    }
                                    break;
                                case VariableComparison.MayorEqual:
                                    if (Input.GetAxisRaw(str) >= data.m_value)
                                    {
                                        LeftAnswer();
                                    }
                                    break;
                            }
                            break;
                    }
                }
            }
            
            if (m_inputTypes.Contains(DIALOG_INPUT_TYPE.KEYCODE))
            {
                foreach (KeyCode str in m_interactKeys.Keys)
                {
                    switch (m_interactKeys[str])
                    {
                        case PRESS_TYPE.DOWN:
                            if (Input.GetKeyDown(str))
                            {
                                Interact();
                                
                            }
                            break;
                        case PRESS_TYPE.UP:
                            if (Input.GetKeyUp(str))
                            {
                                Interact();
                            }
                            break;
                        case PRESS_TYPE.HELD:
                            if (Input.GetKey(str))
                            {
                                Interact();
                            }
                            break;
                    }
                }

                foreach (KeyCode str in m_cancelKeys.Keys)
                {
                    switch (m_cancelKeys[str])
                    {
                        case PRESS_TYPE.DOWN:
                            if (Input.GetKeyDown(str))
                            {
                                Cancel();

                            }
                            break;
                        case PRESS_TYPE.UP:
                            if (Input.GetKeyUp(str))
                            {
                                Cancel();
                            }
                            break;
                        case PRESS_TYPE.HELD:
                            if (Input.GetKey(str))
                            {
                                Cancel();
                            }
                            break;
                    }
                }

                foreach (KeyCode str in m_downKeys.Keys)
                {
                    switch (m_downKeys[str])
                    {
                        case PRESS_TYPE.DOWN:
                            if (Input.GetKeyDown(str))
                            {
                                DownAnswer();
                            }
                            break;
                        case PRESS_TYPE.UP:
                            if (Input.GetKeyUp(str))
                            {
                                DownAnswer();
                            }
                            break;
                        case PRESS_TYPE.HELD:
                            if (Input.GetKey(str))
                            {
                                DownAnswer();
                            }
                            break;
                    }
                }

                foreach (KeyCode str in m_upKeys.Keys)
                {
                    switch (m_upKeys[str])
                    {
                        case PRESS_TYPE.DOWN:
                            if (Input.GetKeyDown(str))
                            {
                                UpAnswer();
                            }
                            break;
                        case PRESS_TYPE.UP:
                            if (Input.GetKeyUp(str))
                            {
                                UpAnswer();
                            }
                            break;
                        case PRESS_TYPE.HELD:
                            if (Input.GetKey(str))
                            {
                                UpAnswer();
                            }
                            break;
                    }
                }

                foreach (KeyCode str in m_rightKeys.Keys)
                {
                    switch (m_rightKeys[str])
                    {
                        case PRESS_TYPE.DOWN:
                            if (Input.GetKeyDown(str))
                            {
                                RightAnswer();
                            }
                            break;
                        case PRESS_TYPE.UP:
                            if (Input.GetKeyUp(str))
                            {
                                RightAnswer();
                            }
                            break;
                        case PRESS_TYPE.HELD:
                            if (Input.GetKey(str))
                            {
                                RightAnswer();
                            }
                            break;
                    }
                }

                foreach (KeyCode str in m_leftKeys.Keys)
                {
                    switch (m_leftKeys[str])
                    {
                        case PRESS_TYPE.DOWN:
                            if (Input.GetKeyDown(str))
                            {
                                LeftAnswer();
                            }
                            break;
                        case PRESS_TYPE.UP:
                            if (Input.GetKeyUp(str))
                            {
                                LeftAnswer();
                            }
                            break;
                        case PRESS_TYPE.HELD:
                            if (Input.GetKey(str))
                            {
                                LeftAnswer();
                            }
                            break;
                    }
                }
            }
        }

        void CustomInput()
        {
            // Fill with your input own calls
        }

        void Cancel()
        {
            if (m_handleType == EVENT_TYPE.DELEGATE)
                VP_DialogManager.OnDialogCancelAction();
            else
                VP_EventManager.TriggerEvent(VP_EventSetup.Input.CANCEL_BUTTON);
        }

        void Interact()
        {
            if (m_handleType == EVENT_TYPE.DELEGATE)
                VP_DialogManager.OnDialogInteractAction();
            else
             VP_EventManager.TriggerEvent(VP_EventSetup.Input.INTERACT_BUTTON);

        }

        void UpAnswer()
        {
            if (m_handleType == EVENT_TYPE.DELEGATE)
                VP_DialogManager.OnDialogUpAction();
            else
                VP_EventManager.TriggerEvent(VP_EventSetup.Input.UP_ANSWER);

        }

        void DownAnswer()
        {
            if (m_handleType == EVENT_TYPE.DELEGATE)
                VP_DialogManager.OnDialogDownAction();
            else
             VP_EventManager.TriggerEvent(VP_EventSetup.Input.DOWN_ANSWER);
        }

        void RightAnswer()
        {
            if (m_handleType == EVENT_TYPE.DELEGATE)
                VP_DialogManager.OnDialogRightAction();
            else
                VP_EventManager.TriggerEvent(VP_EventSetup.Input.RIGHT_ANSWER);

        }

        void LeftAnswer()
        {
            if (m_handleType == EVENT_TYPE.DELEGATE)
                VP_DialogManager.OnDialogLeftAction();
            else
                VP_EventManager.TriggerEvent(VP_EventSetup.Input.LEFT_ANSWER);
        }
    }

}
