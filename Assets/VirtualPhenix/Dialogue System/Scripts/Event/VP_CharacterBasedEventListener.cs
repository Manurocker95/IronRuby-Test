using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Dialog
{
    public class VP_CharacterBasedEventListener : VP_SimpleEventListener
    {
        [SerializeField] private VP_DialogCharacterData m_character = null;
        private bool m_isCharacter = false;

        protected override void StartAllListeners()
        {
            if (m_character)
            {
                VP_DialogManager.StartListeningToOnCharacterSpeak((_character) => { m_isCharacter = (_character == m_character); });

                for (int i = 0; i < m_CustomEvents.Length; i++)
                {
                    if (i < m_CustomEventCallback.Length)
                        VP_EventManager.StartListening(m_CustomEvents[i], () => { if (m_isCharacter) m_CustomEventCallback[i].Invoke(); });
                }

                VP_DialogManager.StartListeningToOnDialogStart(() => { if (m_isCharacter) m_OnStartDialog.Invoke(); });
                VP_DialogManager.StartListeningToOnDialogEnd(() => { if (m_isCharacter) m_OnEndDialog.Invoke(); });
                VP_DialogManager.StartListeningToOnTextShown(() => { if (m_isCharacter) m_OnTextShown.Invoke(); });
                VP_DialogManager.StartListeningToOnAnswerShow(() => { if (m_isCharacter) m_OnAnswersShown.Invoke(); });
                VP_DialogManager.StartListeningOnChoiceSelection((int _selection) => { if (m_isCharacter) m_OnAnswer.Invoke(_selection); });
                VP_DialogManager.StartListeningToOnSkip(() => { if (m_isCharacter) m_OnSkip.Invoke(); });
            }
        }

        protected override void StopAllListeners()
        {
            if (m_character)
            {
                VP_DialogManager.StopListeningToOnCharacterSpeak((_character) => { m_isCharacter = (_character == m_character); });

                for (int i = 0; i < m_CustomEvents.Length; i++)
                {
                    if (i < m_CustomEventCallback.Length)
                        VP_EventManager.StopListening(m_CustomEvents[i], () => { if (m_isCharacter) m_CustomEventCallback[i].Invoke(); });
                }

                VP_DialogManager.StopListeningToOnDialogStart(() => { if (m_isCharacter) m_OnStartDialog.Invoke(); });
                VP_DialogManager.StopListeningToOnDialogEnd(() => { if (m_isCharacter) m_OnEndDialog.Invoke(); });
                VP_DialogManager.StopListeningToOnTextShown(() => { if (m_isCharacter) m_OnTextShown.Invoke(); });
                VP_DialogManager.StopListeningToOnAnswerShow(() => { if (m_isCharacter) m_OnAnswersShown.Invoke(); });
                VP_DialogManager.StopListeningOnChoiceSelection((int _selection) => { if (m_isCharacter) m_OnAnswer.Invoke(_selection); });
            }
        }
    }

}
