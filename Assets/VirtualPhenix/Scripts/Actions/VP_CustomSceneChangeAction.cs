using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Actions
{
    public class VP_CustomSceneChangeAction : VP_CustomFadeAction
    {
        [Header("--Scene to change-"), Space(10)]
        [SerializeField] protected string m_sceneToChange = "";
        [SerializeField] protected string m_onloadEvent= "";
        [SerializeField] protected Vector3 m_positionInNewScene;
        [SerializeField] protected Quaternion m_rotationInNewScene;
        [SerializeField] protected bool m_playChangeSceneAudio;
        [SerializeField] protected AudioClip m_transitionAudio;
        [SerializeField] protected int m_initPointInNextScene;
        [SerializeField] protected bool m_useInitPosition;
        [SerializeField] protected bool m_useChunk;
        [SerializeField] protected string m_chunkID = "Chunk ID";


        public override void InitActions(Action _initCallback = null, Action _invokeCallback = null, Action<int> _indexCallback = null)
        {
            base.InitActions(_initCallback, _invokeCallback, _indexCallback);
        }

        public override void InvokeAction()
        {
            base.InvokeAction();
        }

    }

}
