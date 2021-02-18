using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if DOTWEEN
using DG.Tweening;
#endif

#if USE_MORE_EFFECTIVE_COROUTINES
using MEC;
#endif


namespace VirtualPhenix.Audio
{
    [DefaultExecutionOrder(VP_ExecutingOrderSetup.AUDIO_MANAGER)]
    public class VP_AudioManagerBase : VP_SingletonMonobehaviour<VP_AudioManagerBase>
    {
        [Header("One Shot Source")]
        [SerializeField] protected AudioSource m_oneShotSource;
        [SerializeField] protected AudioSource m_bgmSource;
        [SerializeField] protected AudioSource m_sfxSource;
        [SerializeField] protected AudioSource m_voiceSource;

        [Header("Volumes")]
        [SerializeField] protected float m_masterVolume = 1.0f;
        [SerializeField] protected float m_bgmVolume = 1.0f;
        [SerializeField] protected float m_sfxVolume = 1.0f;
        [SerializeField] protected float m_voiceVolume = 1.0f;

        public virtual float MasterVolume { get { return m_masterVolume; } }
        public virtual float BGMVolume { get { return m_bgmVolume; } }
        public virtual float SfxVolume { get { return m_sfxVolume; } }
        public virtual float VoiceVolume { get { return m_voiceVolume; } }
        public virtual float OneShotVolume { get { return OneShotSource != null ? OneShotSource.volume : 1f; } }
        public virtual AudioSource OneShotSource { get { return m_oneShotSource; } }
        public virtual AudioSource BGMSource { get { return m_bgmSource; } }
        public virtual AudioSource SFXSource { get { return m_sfxSource; } }
        public virtual AudioSource VoiceSource { get { return m_voiceSource; } }

        public virtual float GetVolumeMultiplier(VP_AudioSetup.AUDIO_TYPE _type)
        {
            switch (_type)
            {
                case VP_AudioSetup.AUDIO_TYPE.BGM:
                    return m_bgmVolume * m_masterVolume;
                case VP_AudioSetup.AUDIO_TYPE.SFX:
                    return m_sfxVolume * m_masterVolume;
                case VP_AudioSetup.AUDIO_TYPE.VOICE:
                    return m_voiceVolume * m_masterVolume;
            }

            return 1f;
        }

        public virtual void CreateAudioAndDestroyItAfterPlay(AudioClip _clip, float _volume, float _spatialBlend = 0)
        {
            GameObject gameObject = new GameObject();
            AudioSource _as = gameObject.AddComponent<AudioSource>();
            _as.clip = _clip;
            _as.spatialBlend = _spatialBlend;

            _as.Play();

#if USE_MORE_EFFECTIVE_COROUTINES
            
            VP_Utils.RunWaitTimeCoroutine(_clip.length, () =>
            {
                Destroy(gameObject);
            }, gameObject);

#else
            VP_Utils.RunWaitTimeCoroutine(_clip.length, () =>
            {
                Destroy(gameObject);
            }, false, gameObject);
#endif
        }

        protected virtual void CheckDefaultSources()
        {
            if (!m_bgmSource)
            {
                GameObject newSource;
                if (!VP_Utils.GetGameObjectInChildrenWithName(VP_AudioSetup.DefaultNames.BGM_SOURCE_NAME, gameObject, out newSource))
                {
                    newSource = new GameObject(VP_AudioSetup.DefaultNames.BGM_SOURCE_NAME);
                    newSource.transform.parent = this.transform;
                    m_bgmSource = newSource.AddComponent<AudioSource>();
                }
                else
                {
                    m_bgmSource = newSource.GetOrAddComponent<AudioSource>();
                }
            }

            if (!m_sfxSource)
            {
                GameObject newSource;
                if (!VP_Utils.GetGameObjectInChildrenWithName(VP_AudioSetup.DefaultNames.SFX_SOURCE_NAME, gameObject, out newSource))
                {
                    newSource = new GameObject(VP_AudioSetup.DefaultNames.SFX_SOURCE_NAME);
                    newSource.transform.parent = this.transform;
                    m_sfxSource = newSource.AddComponent<AudioSource>();
                }
                else
                {
                    m_sfxSource = newSource.GetOrAddComponent<AudioSource>();
                }
            }

            if (!m_voiceSource)
            {
                GameObject newSource;
                if (!VP_Utils.GetGameObjectInChildrenWithName(VP_AudioSetup.DefaultNames.VOICE_SOURCE_NAME, gameObject, out newSource))
                {
                    newSource = new GameObject(VP_AudioSetup.DefaultNames.VOICE_SOURCE_NAME);
                    newSource.transform.parent = this.transform;
                    m_voiceSource = newSource.AddComponent<AudioSource>();
                }
                else
                {
                    m_voiceSource = newSource.GetOrAddComponent<AudioSource>();
                }
            }

            if (!m_oneShotSource)
            {
                GameObject newSource;
                if (!VP_Utils.GetGameObjectInChildrenWithName(VP_AudioSetup.DefaultNames.ONE_SHOT_SOURCE_NAME, gameObject, out newSource))
                {
                    newSource = new GameObject(VP_AudioSetup.DefaultNames.ONE_SHOT_SOURCE_NAME);
                    newSource.transform.parent = this.transform;
                    m_oneShotSource = newSource.AddComponent<AudioSource>();
                }
                else
                {
                    m_oneShotSource = newSource.GetOrAddComponent<AudioSource>();
                }
            }
        }
        public virtual void StopAudioAndPlayInSource(VP_AudioSetup.AUDIO_TYPE _stopType, AudioClip _clip, VP_AudioSetup.AUDIO_TYPE _type)
        {
            StopAudioInSource(_stopType);
            PlayAudioInSource(_clip, _type);
        }

        public virtual void PlayAudioInSource(AudioClip _clip, VP_AudioSetup.AUDIO_TYPE _type)
        {
            switch (_type)
            {
                case VP_AudioSetup.AUDIO_TYPE.BGM:
                    m_bgmSource.Stop();
                    m_bgmSource.clip = _clip;
                    m_bgmSource.Play();
                    break;
                case VP_AudioSetup.AUDIO_TYPE.SFX:
                    m_sfxSource.Stop();
                    m_sfxSource.clip = _clip;
                    m_sfxSource.Play();
                    break;
                case VP_AudioSetup.AUDIO_TYPE.VOICE:
                    m_voiceSource.Stop();
                    m_voiceSource.clip = _clip;
                    m_voiceSource.Play();
                    break;
            }
        }

        public virtual void PlayOneShotAudioInSource(AudioClip _clip, VP_AudioSetup.AUDIO_TYPE _type, float _volume)
        {
              
            switch (_type)
            {
                case VP_AudioSetup.AUDIO_TYPE.BGM:
                    m_bgmSource.PlayOneShot(_clip, _volume*GetVolumeMultiplier(_type));
                    break;
                case VP_AudioSetup.AUDIO_TYPE.SFX:
                    m_sfxSource.PlayOneShot(_clip, _volume * GetVolumeMultiplier(_type));
                    break;
                case VP_AudioSetup.AUDIO_TYPE.VOICE:
                    m_voiceSource.PlayOneShot(_clip, _volume * GetVolumeMultiplier(_type));
                    break;
            }
        }

        public virtual void PlayOneShot(AudioClip _clip, VP_AudioSetup.AUDIO_TYPE _type, float _volume)
        {
            if (m_oneShotSource != null)
            {
                m_oneShotSource.PlayOneShot(_clip, _volume*GetVolumeMultiplier(_type));
            }
            else
            {
                GameObject newSource = new GameObject("Audio:" + _clip.name);
                newSource.transform.parent = this.transform;
                m_oneShotSource = newSource.AddComponent<AudioSource>();
                m_oneShotSource.PlayOneShot(_clip, _volume * GetVolumeMultiplier(_type));
            }
        }

        public virtual void StopAllSources()
        {
            m_bgmSource.Stop();
            m_voiceSource.Stop();
            m_sfxSource.Stop();
            m_oneShotSource.Stop();
        }

        public virtual void SetMaster(float _value)
        {
            m_masterVolume = _value;
        }

        public virtual void StopAudioInSource(VP_AudioSetup.AUDIO_TYPE _type)
        {
            switch (_type)
            {
                case VP_AudioSetup.AUDIO_TYPE.BGM:
                    m_bgmSource.Stop();
                    break;
                case VP_AudioSetup.AUDIO_TYPE.SFX:
                    m_sfxSource.Stop();
                    break;
                case VP_AudioSetup.AUDIO_TYPE.VOICE:
                    m_voiceSource.Stop();
                    break;
            }
        }


        public virtual void SetAllTypesVolume(float _value)
        {
            m_bgmVolume = _value;
            m_voiceVolume = _value;
            m_sfxVolume = _value;
        }

        public virtual void SetSourceVolume(VP_AudioSetup.AUDIO_TYPE _type, float _volume)
        {
            switch (_type)
            {
                case VP_AudioSetup.AUDIO_TYPE.BGM:
                    m_bgmSource.volume = _volume;
                    break;
                case VP_AudioSetup.AUDIO_TYPE.SFX:
                    m_sfxSource.volume = _volume;
                    break;
                case VP_AudioSetup.AUDIO_TYPE.VOICE:
                    m_voiceSource.volume = _volume;
                    break;
            }
        }

        public virtual AudioSource GetAudioSourceForType(VP_AudioSetup.AUDIO_TYPE _type)
        {
            switch (_type)
            {
                case VP_AudioSetup.AUDIO_TYPE.BGM:
                    return m_bgmSource;
                case VP_AudioSetup.AUDIO_TYPE.SFX:
                    return m_sfxSource;
                case VP_AudioSetup.AUDIO_TYPE.VOICE:
                    return m_voiceSource;
                default:
                    return m_oneShotSource;
            }
        }


#if !USE_MORE_EFFECTIVE_COROUTINES
        public virtual void DoFade(AudioSource _source, ref Coroutine _coroutine, float _endValue, float _time, float _from = -1, System.Action _callback = null)
        {
            _coroutine = StartCoroutine(DoFadeCoroutine(_source, _endValue, _time, _from, _callback));
        }
#else
	    public virtual void DoFade(AudioSource _source, ref MEC.CoroutineHandle _coroutine, float _endValue, float _time, float _from = -1, System.Action _callback = null)
        {
            _coroutine = MEC.Timing.RunCoroutine(DoFadeCoroutineMEC(_source, 0f, _time, _from, null));
        }
#endif

        public virtual void DoFade(AudioSource _source, float _endValue, float _time, float _from = -1, System.Action _callback = null)
        {
#if USE_MORE_EFFECTIVE_COROUTINES
            MEC.Timing.RunCoroutine(DoFadeCoroutineMEC(_source, 0f, _time, _from, null));
#else
            StartCoroutine(DoFadeCoroutine(_source, _endValue, _time, _from, _callback));
#endif
        }

        protected virtual IEnumerator DoFadeCoroutine(AudioSource _source, float _endValue, float _time, float _from = -1, System.Action _callback = null, bool _useSteps = false)
        {


            if (_from >= 0)
            {
                _source.volume = _from;
            }

            float currentValue = _source.volume;
            float stepValue = _useSteps ? Mathf.Abs(_endValue - currentValue) / _time : Time.deltaTime;

            if (_endValue > currentValue)
            {
                while (currentValue < _endValue)
                {
                    currentValue += stepValue;
                    currentValue = Mathf.Clamp(currentValue, 0f, 1f);
                    _source.volume = currentValue;
                    yield return null;
                }
            }
            else
            {
                while (currentValue > _endValue)
                {
                    currentValue -= stepValue;
                    currentValue = Mathf.Clamp(currentValue, 0f, 1f);
                    _source.volume = currentValue;

                    yield return null;
                }
            }

            _source.volume = _endValue;

            if (_callback != null)
                _callback.Invoke();
        }


#if USE_MORE_EFFECTIVE_COROUTINES
        protected virtual IEnumerator<float> DoFadeCoroutineMEC(AudioSource _source, float _endValue, float _time, float _from = -1, System.Action _callback = null, bool _useSteps = false)
        {
            if (_from >= 0)
            {
                _source.volume = _from;
            }

            float currentValue = _source.volume;
            float stepValue = _useSteps ? Mathf.Abs(_endValue - currentValue) / _time : Time.deltaTime;

            if (_endValue > currentValue)
            {
                while(currentValue < _endValue)
                {
                    currentValue += stepValue;
                    currentValue = Mathf.Clamp(currentValue, 0f, 1f);
                    _source.volume = currentValue;
                    yield return Timing.WaitForOneFrame;
                }
            }
            else
            {
                while (currentValue > _endValue)
                {
                    currentValue -= stepValue;
                    currentValue = Mathf.Clamp(currentValue, 0f, 1f);
                    _source.volume = currentValue;

                    yield return Timing.WaitForOneFrame;
                }
            }

            _source.volume = _endValue;

            if (_callback != null)
                _callback.Invoke();
        }
#endif
    }
}