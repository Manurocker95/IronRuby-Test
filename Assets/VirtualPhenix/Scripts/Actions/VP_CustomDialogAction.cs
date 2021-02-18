using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix.Dialog;

namespace VirtualPhenix.Actions
{
    public enum DIALOG_BG_TYPE
    {
        REGULAR,
        SPECIAL,
        SIGN
    }

    [System.Serializable]
	public class VP_CustomDialogAction : VP_CustomActions
    {
        [Header("-- Text --"), Space(10)]
        [SerializeField] protected DIALOG_BG_TYPE m_dialogBGType;
        [SerializeField] protected bool m_useDialgueSystemGraph = false;
        [SerializeField, TextArea] public string m_regularMessage;
        [SerializeField] protected bool m_useTranslation = false;
        [SerializeField] protected bool m_showDirectly = false;
        [SerializeField] protected bool m_fadeAnimation = true;
        [SerializeField] protected VP_DialogMessage m_customMessage;
        public VP_DialogMessage CustomMessage { get { return m_customMessage; } set { m_customMessage = value; } }
        public DIALOG_BG_TYPE CustomMessageBG { get { return m_dialogBGType; } set { m_dialogBGType = value; } }

        public override void InitActions(System.Action _initCallback = null, System.Action _invokeCallback = null, System.Action<int> _indexCallback = null)
        {
            m_action = CUSTOM_GAME_ACTIONS.DIALOG;
            base.InitActions(_initCallback, _invokeCallback, _indexCallback);

        }

        public override void InvokeAction()
        {
            base.InvokeAction();

            if (m_useDialgueSystemGraph)
            {

                if (!string.IsNullOrEmpty(m_regularMessage))
                    VP_DialogManager.SendDialogMessage(m_regularMessage);
            }
            else
            {
                if (m_customMessage == null)
                {
                    if (m_dialogBGType == DIALOG_BG_TYPE.SPECIAL)
                    {
                        //m_customMessage = PLGU_GameManager.SpecialDialog; // TODO ADD SPECIAL DIALOG TO GAMEMANAGER
                    }

                    if (m_dialogBGType == DIALOG_BG_TYPE.SIGN)
                    {
                        //m_customMessage = PLGU_GameManager.SignDialog;
                    }
                }


                VP_DialogManager.ShowDirectMessage(m_regularMessage, null, DIALOG_TYPE.REGULAR, false, false, true, null, null, null, () => 
                { 
                    if (m_onEndCallback != null) 
                        m_onEndCallback.Invoke(); 
                    
                    VP_ActionManager.Instance.DoGameplayAction(); 
                });
            }
        }
    }

}
