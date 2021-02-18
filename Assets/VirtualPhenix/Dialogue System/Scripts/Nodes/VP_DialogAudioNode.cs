using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace VirtualPhenix.Dialog
{
    public enum DIALOG_AUDIO_ACTION
    {
        PLAY,
        PLAY_ONE_SHOT,
        STOP

    }

    [NodeTint(VP_DialogSetup.NodeColors.AUDIO_NODE), CreateNodeMenuAttribute("Audio")]
    public class VP_DialogAudioNode : VP_DialogBaseNode
    {
        [SerializeField] public DIALOG_AUDIO_ACTION m_audioAction = DIALOG_AUDIO_ACTION.PLAY;
        [SerializeField] public VP_AudioSetup.AUDIO_TYPE m_audioType = VP_AudioSetup.AUDIO_TYPE.BGM;
        [SerializeField] public AudioClip m_audioClip;
        [SerializeField] public VP_DialogAudioKey m_AudiokeyData;
        [SerializeField] public string m_audioKey;
        [SerializeField] public bool m_addAudio;
        [SerializeField] public bool m_removeAudio;
        [SerializeField] public bool m_audioInLoop;
        [SerializeField] public float m_audioVolume = 1.0f;
        [SerializeField] public bool m_stopAllPrevious;

        protected override void Awake()
        {
            base.Awake();

            if (m_AudiokeyData == null)
            {
                m_AudiokeyData = Resources.Load<VP_DialogAudioKey>(VP_DialogSetup.DialogNodes.AUDIO_NODE_KEY_PATH);
                m_AudiokeyData.key = "";
            }

            if (string.IsNullOrEmpty(m_ID))
                m_ID = VP_Utils.CreateID();

            if (m_AudiokeyData.list != null && !m_AudiokeyData.list.ContainsKey(m_ID))
                m_AudiokeyData.list.Add(m_ID, "");
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (m_AudiokeyData != null && m_AudiokeyData.list != null && m_AudiokeyData.list.ContainsKey(m_ID))
                m_AudiokeyData.list.Remove(m_ID);
        }

        public override void Trigger()
        {
            if (m_needToSkip)
            {
                m_needToSkip = false;
            }
            // VP_Debug.Log("Triggering audio");
            if (!string.IsNullOrEmpty(m_audioKey))
            {
                if (m_audioClip != null)
                {
                    if (m_audioAction == DIALOG_AUDIO_ACTION.PLAY)
                    {
                        if (!m_stopAllPrevious)
                            VP_AudioManager.Instance.PlayAudio(m_audioClip, m_audioType, true, m_audioVolume, m_audioKey, m_addAudio);
                        else
                            VP_AudioManager.Instance.PlayAndStopPrevious(m_audioClip, m_audioType, true, m_audioVolume, m_audioKey, m_addAudio);
                    }
                    else if (m_audioAction == DIALOG_AUDIO_ACTION.PLAY_ONE_SHOT)
                    {
                        VP_AudioManager.Instance.PlayOneShot(m_audioClip, m_audioType, m_audioVolume);
                    }
                    else
                    {
                        VP_AudioManager.Instance.StopAudioItem(m_audioKey, m_removeAudio);
                    }
                }
                else
                {
                    if (m_audioAction == DIALOG_AUDIO_ACTION.PLAY)
                    {
                        if (!m_stopAllPrevious)
                            VP_AudioManager.Instance.PlayAudioItem(m_audioKey);
                        else
                            VP_AudioManager.Instance.PlayAndStopPrevious(m_audioKey, m_audioType, m_audioVolume);
                    }
                    else if (m_audioAction == DIALOG_AUDIO_ACTION.PLAY_ONE_SHOT)
                    {
                        VP_AudioManager.Instance.PlayOneShot(m_audioClip, m_audioType, m_audioVolume);
                    }
                    else
                    {
                        VP_AudioManager.Instance.StopAudioItem(m_audioKey, m_removeAudio);
                    }
                }
            }
            else
            {
                if (m_stopAllPrevious)
                    VP_AudioManager.Instance.StopAllItems(m_audioType);
            }
            VP_DialogManager.OnDialogCompleteAction();
            NodePort port = null;
            if (port == null)
            {
                port = GetOutputPort(VP_DialogSetup.Fields.DIALOG_NODE_OUTPUT);
            }

            if (port.ConnectionCount > 0)
            {
                for (int i = 0; i < port.ConnectionCount; i++)
                {
                    NodePort connection = port.GetConnection(i);
                    (connection.node as VP_DialogBaseNode).Trigger();
                }
            }
            else
            {
                CheckEndSequenceNode();
            }
        }
    }
}
