using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix;
using VirtualPhenix.Dialog;

namespace VirtualPhenix.Dialog.Demo
{
    [AddComponentMenu(VP_PrefixSetup.MAIN_PREFIX+"/Examples/Demo Interaction NPC")]
    public class VP_DemoNPC : VP_DemoInteraction
    {
        /// <summary>
        /// Key in the graph
        /// </summary>
        [SerializeField, TextArea] protected string m_dialogKey;      
        [SerializeField] protected bool m_isDirectMessage = false;      
        [SerializeField] protected DIALOG_TYPE m_dialogType = DIALOG_TYPE.REGULAR;
        [SerializeField] protected VP_DialogMessage m_customDialogMessage = null;  
        [SerializeField] protected VP_DialogPositionData m_customPositionData = null;  
        [SerializeField] protected bool m_setMessageOrientation = false;  

        [SerializeField] protected string m_characterNameVariable = "characterName";
        [SerializeField] protected string m_npcName = "NPC";

        [SerializeField] protected bool m_discovered = false;
        [SerializeField] protected string m_discoveredNameVariable = "discoveredName";

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            if (string.IsNullOrEmpty(m_dialogKey))
                m_dialogKey = VP_DialogSetup.InitEvents.INIT_DEMO_8_NPC_DIALOG;

            if (string.IsNullOrEmpty(m_characterNameVariable))
                m_characterNameVariable = "characterName";            
            
            if (string.IsNullOrEmpty(m_discoveredNameVariable))
                m_characterNameVariable = "characterName";
        }

        public virtual void CheckEndDialog()
        {
            if (m_interacting)
                OnEndInteraction();
        }

        public override void OnInteraction ()
        {
            var dialogManager = VP_DialogManager.Instance;
            m_interacting = true;
            m_speakRefresh = true;

            if (m_character != null)
            {
                m_npcName = m_character.characterName;
            }
            dialogManager.SetVariable(m_characterNameVariable, m_npcName);
            dialogManager.SetVariable(m_discoveredNameVariable, m_discovered);

            if (m_setMessageOrientation && m_customDialogMessage)
            {
                m_customDialogMessage.transform.position = new Vector3(m_customDialogMessage.transform.position.x, this.transform.position.y, m_customDialogMessage.transform.position.z);
            }

            if (!m_isDirectMessage)
                VP_DialogManager.SendDialogMessage(m_dialogKey);
            else
                VP_DialogManager.ShowDirectMessage(m_dialogKey, null, m_dialogType, false, false, true, m_customDialogMessage);

            m_canBeInteracted = false;
        }

        public override void OnInteraction(VP_DemoCharacterController _controller)
        {
            if (m_character != null)
            {
                m_npcName = m_character.characterName;
            }
            VP_DialogManager.Instance.SetVariable(m_characterNameVariable, m_npcName);
            VP_DialogManager.Instance.SetVariable(m_discoveredNameVariable, m_discovered);

            base.OnInteraction(_controller);

            m_interacting = true;
            m_speakRefresh = true;

            if (m_setMessageOrientation && m_customDialogMessage)
            {
                Vector3 eu = transform.rotation.eulerAngles;
                m_customDialogMessage.transform.rotation = Quaternion.Euler(eu.x, eu.y + 180, eu.z);
                //new Quaternion(m_customDialogMessage.transform.rotation.x, this.transform.rotation.y, m_customDialogMessage.transform.rotation.z, m_customDialogMessage.transform.rotation.w);
            }


            if (!m_isDirectMessage)
                VP_DialogManager.SendDialogMessage(m_dialogKey, null, null, m_customDialogMessage);
            else
                VP_DialogManager.ShowDirectMessageWithCharacter(m_dialogKey, null, m_dialogType, m_character, m_customPositionData, false, false, true, m_customDialogMessage);

            m_canBeInteracted = false;
        }

        public override void OnEndInteraction()
        {
            m_interacting = false;
            m_canBeInteracted = true;
        }
    }

}
