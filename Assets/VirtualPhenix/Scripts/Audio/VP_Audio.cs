using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix
{
    [
        DefaultExecutionOrder(VP_ExecutingOrderSetup.AUDIO_ITEM),
        AddComponentMenu(VP_PrefixSetup.MAIN_PREFIX + "/Audio/Audio Item")
    ]
    public class VP_Audio : VP_Monobehaviour
    {
        [Header("Audio Item")]
        public bool m_useAudioManager = true;
        public bool m_useAudioKey = true;
        public bool m_reportIfNull = true;
	    public bool m_stopOnDestroy = true;
        [SerializeField] protected VP_AudioItem m_audio;

        public VP_AudioItem Audio { get { return m_audio; } }
        public float Length { get { return m_audio.GetAudioLength(); } }

        public virtual void SetClip(AudioClip _newClip)
        {
            m_audio.m_clip = _newClip;
        }

        protected virtual void Reset()
        {
            m_startListeningTime = StartListenTime.None;
            m_stopListeningTime = StopListenTime.None;
            m_initializationTime = InitializationTime.OnStart;
        }

        // Start is called before the first frame update
        protected override void Initialize()
        {
            base.Initialize();

            if (m_useAudioManager && m_useAudioKey)
            {
                string m_audioKey = m_audio.m_key;
                if (string.IsNullOrEmpty(m_audioKey) && m_reportIfNull)
                {
                    Debug.LogError("NO AUDIO ITEM KEY in " + gameObject.name);
                    this.gameObject.SetActive(false);
                    return;
                }

                if (!VP_AudioManager.nullableInstance)
                {
                    Debug.LogError("NO AUDIO MANAGER FOR KEY: " + m_audioKey);
                    this.gameObject.SetActive(false);
                    return;
                }

                if (VP_AudioManager.Instance.AudioAlreadyExists(m_audioKey) && m_reportIfNull)
                {
                    Debug.LogError("AUDIO KEY ALREADY IN DICTIONARY. KEY: " + m_audioKey);
                    this.gameObject.SetActive(false);
                    return;
                }
            }

            AudioSource m_source = m_audio.m_source;

            if (m_source == null)
            {
                GameObject _childSource = new GameObject();
                _childSource.transform.SetParent(this.transform);
                m_source = _childSource.AddComponent<AudioSource>();
                m_audio.m_source = m_source;
            }

            m_audio.InitSource();

            if (m_useAudioManager && m_useAudioKey)
            {
                VP_AudioManager.Instance.AddAudioItem(m_audio, m_audio.m_playOnAwake, m_audio.m_fadeInOnInit);
            }
            else
            {
                m_audio.CheckInit();
            }
        }


        public virtual void CreateAudio()
        {
            m_audio = new VP_AudioItem();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

	        if (m_stopOnDestroy)
		        Stop();

            if (m_useAudioManager && m_useAudioKey)
            {
                var am = VP_AudioManager.Instance;
                if (am && !VP_MonobehaviourSettings.Quiting)
                    am.RemoveItem(m_audio);
            }
        }

        public virtual void Play()
        {
            m_audio.Play();
        }

        public virtual void PlayWithMultiplier()
        {
            m_audio.Play(-1f, VP_AudioManager.Instance.GetVolumeMultiplier(m_audio.m_type));
        }

        public virtual void FadeIn(float _time = 1f)
        {
            m_audio.FadeIn(_time);
        }

        public virtual void FadeOut(float _time = 1f)
        {
            m_audio.FadeOut(_time);
        }

        public virtual void Stop()
        {
            m_audio.Stop();
        }
    }

}
