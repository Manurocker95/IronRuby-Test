using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace VirtualPhenix.Dialog.Demo
{
    public class VP_DemoButton : VP_DemoInteraction
    {
        [Header("Button")]
        public UnityEvent m_InteractionEvent;
        public VP_DialogMessage m_message;
        public Material[] m_material;
        public Renderer  m_renderer;
        public bool m_activated = false;

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            VP_DialogManager.Instance.SetGlobalVariable("buttonPressed", m_activated);
        }

        public override void OnInteraction(VP_DemoCharacterController _characterController)
        {
            base.OnInteraction(_characterController);

            m_activated = !m_activated;

            m_InteractionEvent.Invoke();
        }

        public override void OnEndInteraction()
        {
            base.OnEndInteraction();
        }

        public void BtnInvoke()
        {
            VP_DialogManager.Instance.SetGlobalVariable("buttonPressed", m_activated);
            VP_DialogManager.ShowDirectMessage($"Click! The button press is  <var=buttonPressed>bool</var>, and this is a direct message instead of graph!",null, DIALOG_TYPE.REGULAR, false, false, true, m_message, null, null, () =>
            {       
                m_renderer.material = m_activated ? m_material[1] : m_material[0];
            });
        }
    }

}
