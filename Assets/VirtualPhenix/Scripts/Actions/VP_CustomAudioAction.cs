using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix;

namespace VirtualPhenix.Actions
{
    public class VP_CustomAudioAction : VP_CustomActions
    {
        [Header("--Play Audio--"), Space(10)]
        [SerializeField] protected bool m_previousAudio;
        [SerializeField] protected AudioClip m_audioToPlay;
        [SerializeField] protected VP_AudioSetup.AUDIO_TYPE m_audioType;
        [SerializeField] protected bool m_audioLoop;
        [SerializeField] protected bool m_oneShot;
        [SerializeField] protected bool m_addItForLater;
        [SerializeField] protected string m_audioKey;
        [SerializeField] protected float m_audioVolume;
        [SerializeField] protected bool m_waitForAudio;

        public override void InvokeAction()
        {
            base.InvokeAction();
        }
    }

}
