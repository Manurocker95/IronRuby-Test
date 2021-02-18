#if USE_MORE_EFFECTIVE_COROUTINES
using MEC;
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if DOTWEEN
using DG.Tweening;
#endif

namespace VirtualPhenix
{

    [System.Serializable]
    public class VP_AudioItem
    {
        public string m_key;
        public VP_AudioSetup.AUDIO_TYPE m_type;
        public AudioSource m_source;
        public AudioClip m_clip;
        public bool m_override = true;
        [Range(0, 1)] public float m_originalVolume = 1f;
        [Range(0, 1)] public float m_currentVolume = 1f;
        public bool m_inLoop = false;
        public bool m_playOnAwake = false;
        public float m_spatialBlend = 0f;
        public int m_maxTimes = 0;
        public bool m_fadeInOnInit = false;
        public float m_fadingTime = 1f;

#if !USE_MORE_EFFECTIVE_COROUTINES
        private Coroutine m_fadeCoroutine;
#else
        private CoroutineHandle m_fadeCoroutine;
#endif

        public VP_AudioItem()
        {

        }

	    public VP_AudioItem(VP_AudioItem _copy)
	    {
		    m_key = _copy.m_key;
		    m_type = _copy.m_type;
		    m_source = _copy.m_source;
		    m_originalVolume = _copy.m_originalVolume;
		    m_currentVolume = _copy.m_currentVolume;
		    m_override = _copy.m_override;
		    m_clip = _copy.m_clip;
		    m_originalVolume = _copy.m_originalVolume;
		    m_spatialBlend = _copy.m_spatialBlend;
		    m_maxTimes = _copy.m_maxTimes;
		    m_playOnAwake = _copy.m_playOnAwake;
		    m_inLoop = _copy.m_inLoop;
		    m_fadingTime = _copy.m_fadingTime;
		    m_fadeInOnInit = _copy.m_fadeInOnInit;
	    }

	    public VP_AudioItem(string _key, AudioClip _clip, AudioSource _source, VP_AudioSetup.AUDIO_TYPE _type, float _originalVolume, bool _overrideClip, float multiplier)
        {
            m_key = _key;
            m_type = _type;
            m_source = _source;
            m_originalVolume = _originalVolume;
            m_currentVolume = m_originalVolume * multiplier;
            m_override = _overrideClip;
            m_clip = _clip;
	        m_originalVolume = m_source.volume;
            m_source.spatialBlend = m_spatialBlend;
        }

        public virtual void Create(string _key, AudioClip _clip, AudioSource _source, VP_AudioSetup.AUDIO_TYPE _type, float _originalVolume, bool _overrideClip, float multiplier)
        {
            m_key = _key;
            m_type = _type;
            m_source = _source;
            m_originalVolume = _originalVolume;
            m_currentVolume = m_originalVolume * multiplier;
            m_override = _overrideClip;
            m_clip = _clip;
            m_originalVolume = m_source.volume;
            m_source.spatialBlend = m_spatialBlend;
        }

        public virtual void InitSource(bool _initVolume = true)
        {
            m_source.loop = m_inLoop;
            m_source.playOnAwake = m_playOnAwake;
            m_source.spatialBlend = m_spatialBlend;

            float multiplier = VP_AudioManager.Instance.GetVolumeMultiplier(m_type);

            if (_initVolume)
            {            
                m_currentVolume = m_originalVolume * multiplier;
                m_source.volume = m_currentVolume;
            }

            if (m_playOnAwake)
            {
                if (m_override)
                {
                    m_source.clip = m_clip;
                    Play();
                    return;
                }

                PlayOneShot();
            }
        }

        public virtual void UpdateVolume(float _modifier)
        {
            m_currentVolume = m_originalVolume * _modifier;
            m_source.volume = m_currentVolume;
        }

        public virtual void SetVolume(float _volume, bool _toSource = false)
        {
            _volume = Mathf.Clamp(_volume, 0f, 1f);

            m_currentVolume = _volume;

            if (_toSource && m_source)
            {
                m_source.volume = _volume;
            }
        }

        public virtual void SetItem2D()
        {
            m_spatialBlend = 0f;
            m_source.spatialBlend = m_spatialBlend;
        }

        public virtual void SetItem3D()
        {
            m_spatialBlend = 1f;
            m_source.spatialBlend = m_spatialBlend;
        }

        public virtual void Play(float _volume = -1, float volumeMultiplier = 1f)
        {

            if (m_override && m_clip && m_clip != m_source.clip)
                m_source.clip = m_clip;

            m_source.volume = (_volume >= 0 ? _volume : m_currentVolume) * volumeMultiplier;

            m_source.Play();
        }

        public virtual void PlayIfNotPlaying(float _volume = -1, float volumeMultiplier = 1f)
        {
            if (m_source && !m_source.isPlaying)
            {
                if (m_override && m_clip && m_clip != m_source.clip)
                    m_source.clip = m_clip;

                m_source.volume = (_volume >= 0 ? _volume : m_currentVolume) * volumeMultiplier;

                m_source.Play();
            }
        }   
       

        public virtual void PlayOneShot(float _volume = -1, float volumeMultiplier = 1f)
        {
            if (m_clip && m_source)
            {
                m_source.PlayOneShot(m_clip, (_volume >= 0 ? _volume : m_currentVolume) * volumeMultiplier);
            }
        }

        public virtual void CheckInit()
        {
            if (m_playOnAwake)
            {
                if (m_fadeInOnInit)
                {
                   
                    FadeIn();
                }
                else
                {
                    Play();
                }
            }
        }

        public virtual void FadeIn(float _time = -1f, float _endVolume = -1)
        {
            if (m_override && m_clip && m_clip != m_source.clip)
                m_source.clip = m_clip;

            if (_time < 0)
                _time = m_fadingTime;

            if (_endVolume < 0)
                _endVolume = m_source.volume;

            m_source.Play();
#if DOTWEEN
            m_source.DOFade(_endVolume, _time).From(0f);
#else
            DoFade(0f, _time, 0f);
#endif
        }

        public virtual void FadeOut(float _time = -1f)
        {
            if (_time < 0)
                _time = m_fadingTime;

#if DOTWEEN
            m_source.DOFade(0f, _time);
#else
            DoFade(0f, _time);
#endif
        }

        public virtual void DoFade(float _endValue, float _time, float _from = -1, System.Action _callback = null)
        {
            VP_AudioManager.Instance.DoFade(m_source, ref m_fadeCoroutine, _endValue, _time, _from, _callback);
        }

        public virtual void Stop()
        {
            m_source.Stop();
        }

        public virtual void Pause()
        {
            m_source.Pause();
        }

        public virtual void Resume()
        {
            m_source.Play();
        }

        public virtual bool IsPlaying()
        {
            return m_source.isPlaying;
        }

        public virtual VP_AudioSetup.AUDIO_TYPE GetTYPE()
        {
            return m_type;
        }

        public virtual void Mute()
        {
            m_source.mute = true;
        }

        public virtual void Unmute()
        {
            m_source.mute = false;
        }

        public virtual void SetLoop(bool value)
        {
            m_source.loop = value;
        }

        public virtual float GetAudioLength()
        {
            return m_source.clip ? m_source.clip.length : 0f;
        }

        public virtual void OverrideSourceClip()
        {
            m_override = true;

            if (m_source != null && m_clip != null)
                m_source.clip = m_clip;
        }
    }
}