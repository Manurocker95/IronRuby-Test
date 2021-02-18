using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Dialog
{
    [System.Serializable]
    public class VP_DialogEmotionData : VP_MonoBehaviour
    {
        public enum AFTER_EMOTION
        {
            DESTROY,
            DEACTIVATE,
            STOP
        }

        [SerializeField] protected bool m_isPrefab;
        [SerializeField] protected GameObject m_particleGO;
        [SerializeField] protected Transform m_particleParent;
        [SerializeField] protected VP_DialogMessage.EMOTION m_emotion;
        [SerializeField] protected AFTER_EMOTION m_actionAfterEnd;
        [SerializeField] protected ParticleSystem m_particleSystem;
        [SerializeField] protected VP_Audio m_audio;

        protected GameObject m_emotionParticleGO;

        protected override void Start()
        {
            base.Start();
            if (!m_particleGO)
            {
                m_particleGO = this.gameObject;

                if (!m_particleSystem)
                    m_particleSystem = GetComponent<ParticleSystem>();
            }
        }

        public virtual void SetEmotionAction(string action)
        {
            if (action == "PLAY" || action == "Play" || action == "play")
            {
                Play();
            }
            else if (action == "STOP" || action == "Stop" || action == "stop")
            {
                Stop();
            }
        }

        public virtual void Stop()
        {
            switch (m_actionAfterEnd)
            {
                case AFTER_EMOTION.DESTROY:
                    Destroy(m_emotionParticleGO);
                    break;
                case AFTER_EMOTION.DEACTIVATE:
                    if (m_audio)
                    {
                        m_audio.Stop();
                    }
                    m_emotionParticleGO.SetActive(false);
                    break;
                case AFTER_EMOTION.STOP:
                    if (m_audio)
                    {
                        m_audio.Stop();
                    }
                    m_particleSystem.Stop();
                    break;
            }
        }

        public virtual void Play()
        {
            if (m_isPrefab)
            {
                m_emotionParticleGO = Instantiate(m_particleGO, m_particleParent);
                m_particleSystem = m_particleGO.GetComponent<ParticleSystem>();
            }
            else
            {
                m_emotionParticleGO = m_particleGO;
                if (!m_emotionParticleGO.activeInHierarchy)
                {
                    m_emotionParticleGO.SetActive(true);
                }
            }

            if (m_particleSystem)
            {
                float duration = m_particleSystem.main.duration;
                m_particleSystem.Play();
                StartCoroutine(WaitForEnd(duration, Stop));
            }

            if (m_audio)
            {
                m_audio.Play();
            }
        }

        protected IEnumerator WaitForEnd(float _time, System.Action _callback)
        {
            float time = 0f;

            while (time < _time)
            {
                time += Time.deltaTime;
                yield return null;
            }

            if (_callback != null)
                _callback.Invoke();
        }
    }
}
