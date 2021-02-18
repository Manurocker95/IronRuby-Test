using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Dialog
{
    [CreateAssetMenu(fileName = "EmotionDB", menuName = "Virtual Phenix/Dialogue System/Emotion Database", order = 1)]
    public class VP_EmotionDatabase : VP_ScriptableObject
    {
        [Header("Emotions")]
        [SerializeField] private VP_SerializedEmotions m_emotions = new VP_SerializedEmotions();

        public VP_SerializedEmotions Emotions { get { return m_emotions; } }

        protected override void OnValidate()
        {
            base.OnValidate();

            if (m_emotions == null)
            {
                m_emotions = new VP_SerializedEmotions();
            }
        }

        public virtual VP_DialogEmotionData GetEmotion(VP_DialogMessage.EMOTION _emotion)
        {
            if (m_emotions.Contains(_emotion))
                return m_emotions[_emotion];

            return null;
        }
    }

}
