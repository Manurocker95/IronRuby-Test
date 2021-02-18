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
    public class VP_GenericAudioManager<T> : VP_AudioManagerBase where T : VP_AudioItem
    {
        public static new VP_GenericAudioManager<T> Instance
        {
            get
            {
                return (VP_GenericAudioManager<T>)m_instance;
            }
        }

        [Header("Runtime Dictionary")]
        /// <summary>
        /// Dictionary of audio items
        /// </summary>
        [SerializeField] protected VP_AudioDictionary<T> m_audioDictionary;

        [Header("Add audios to the dictionary if they don't exist there")]
        [SerializeField] protected bool m_addNonExistingItems = false;

        [Header("Common-used audios")]
        [SerializeField] protected List<T> m_predefinedAudios;

        public virtual bool AddNonExistingItems { get { return m_addNonExistingItems; } }

        public virtual VP_AudioDictionary<T> AudioDictionary { get { return m_audioDictionary; } }
        public virtual List<T> PredefinedAudios { get { return m_predefinedAudios; } }

        protected override void Initialize()
        {
            base.Initialize();
            CheckDefaultSources();
            InitializeDictionary();
        }

        protected virtual void InitializeDictionary()
        {
            if (m_audioDictionary == null)
            {
                m_audioDictionary = new VP_AudioDictionary<T>();
            }

            if (m_predefinedAudios == null)
            {
                m_predefinedAudios = new List<T>();
            }

            foreach (T item in m_predefinedAudios)
                m_audioDictionary.Add(item.m_key, item);

        }

        protected override void Reset()
        {
            base.Reset();


            m_audioDictionary = new VP_AudioDictionary<T>();

            if (TryGetComponent<VP_AudioManager>(out VP_AudioManager _manager))
            {
                m_audioDictionary = new VP_AudioDictionary<T>();
                var manDic = _manager.AudioDictionary;
                if (manDic != null)
                {
                    foreach (string keys in manDic.Keys)
                    {
                        m_audioDictionary.Add(keys, CreateItemFromCopy(manDic[keys]));
                    }
                }

                m_predefinedAudios = new List<T>();

                foreach (VP_AudioItem it in _manager.PredefinedAudios)
                {
                    m_predefinedAudios.Add(CreateItemFromCopy(it));
                }

                m_addNonExistingItems = _manager.AddNonExistingItems;
                m_bgmVolume = _manager.BGMVolume;
                m_sfxVolume = _manager.SfxVolume;
                m_voiceVolume = _manager.VoiceVolume;
                m_masterVolume = _manager.MasterVolume;

                m_oneShotSource = _manager.OneShotSource;
                m_bgmSource = _manager.BGMSource;
                m_sfxSource = _manager.SFXSource;
                m_voiceSource = _manager.VoiceSource;
            }

            CheckDefaultSources();
        }



        public virtual T CreateItemFromCopy(VP_AudioItem _copy)
        {
            return (T)_copy;
        }

        public override void SetMaster(float _value)
        {
            base.SetMaster(_value);

            foreach (T item in m_audioDictionary.Values)
            {
                item.UpdateVolume(GetVolumeMultiplier(item.m_type));
            }
        }

        public override void SetAllTypesVolume(float _value)
        {
            base.SetAllTypesVolume(_value);

            foreach (T item in m_audioDictionary.Values)
            {
                item.UpdateVolume(GetVolumeMultiplier(item.m_type));
            }
        }

        public virtual void SetAllItemsVolume(float _value)
        {
            foreach (T item in m_audioDictionary.Values)
            {
                item.m_source.volume = _value * GetVolumeMultiplier(item.m_type);
            }
        }


        public virtual void SetAllItemsToHalfVolume()
        {
            foreach (T item in m_audioDictionary.Values)
            {
                item.m_source.volume *= 0.25f;
            }
        }

        public virtual void SetAllItemsToRegularVolume()
        {
            foreach (T item in m_audioDictionary.Values)
            {
                item.m_source.volume *= 4f;
            }
        }

        public virtual void OverrideItem(T _item)
        {
            m_audioDictionary[_item.m_key] = _item;
        }

        public static void FadeAudio(AudioClip _clip, float _duration, float from = 0f, VP_AudioSetup.AUDIO_TYPE _type = VP_AudioSetup.AUDIO_TYPE.SFX, bool _overrideClip = false, float _volume = 1f, string _keyAdd = "", bool _addItForLater = false, bool _inLoop = false)
        {
            if (m_instance)
                Instance.FadeAudioOut(_clip, _duration, from, _type, _overrideClip, _volume, _keyAdd, _addItForLater, _inLoop);
        }

        public virtual void FadeAudioOut(AudioClip _clip, float time, float from = 0f, VP_AudioSetup.AUDIO_TYPE _type = VP_AudioSetup.AUDIO_TYPE.SFX, bool _overrideClip = false, float _volume = 1f, string _keyAdd = "", bool _addItForLater = false, bool _inLoop = false)
        {
            if (_clip == null)
                return;

            if (_addItForLater)
            {
                string _key = string.IsNullOrEmpty(_keyAdd) ? _clip.name : _keyAdd;
                if (m_audioDictionary.ContainsKey(_key))
                {
                    m_audioDictionary[_key].Play();

                }
                else
                {
                    GameObject newSource = new GameObject();
                    newSource.transform.parent = this.transform;
                    AudioSource source = newSource.AddComponent<AudioSource>();
                    source.clip = _clip;

                    T audioItem = default(T);
                    audioItem.Create(_key, _clip, source, _type, _volume, _overrideClip, GetVolumeMultiplier(_type));

                    audioItem.Play();

                    if (_addItForLater || m_addNonExistingItems)
                        m_audioDictionary.Add(_key, audioItem);
                }
            }


            switch (_type)
            {
                case VP_AudioSetup.AUDIO_TYPE.BGM:
                    m_bgmSource.clip = _clip;
                    m_bgmSource.loop = _inLoop;
                    m_bgmSource.Play();
#if DOTWEEN
                    m_bgmSource.DOFade(_volume, time).From(from);
#else
                    DoFade(m_bgmSource, _volume, time, from);
#endif
                    break;
                case VP_AudioSetup.AUDIO_TYPE.SFX:
                    m_sfxSource.clip = _clip;
                    m_sfxSource.loop = _inLoop;
                    m_sfxSource.Play();
#if DOTWEEN
                    m_sfxSource.DOFade(_volume, time).From(from);
#else
                    DoFade(m_sfxSource, _volume, time, from);
#endif
                    break;
                case VP_AudioSetup.AUDIO_TYPE.VOICE:
                    m_voiceSource.clip = _clip;
                    m_voiceSource.loop = _inLoop;
                    m_voiceSource.Play();
#if DOTWEEN
                    m_voiceSource.DOFade(_volume, time).From(from);
#else
                    DoFade(m_voiceSource, _volume, time, from);
#endif
                    break;
            }

        }

        public static void FadeTypeOut(VP_AudioSetup.AUDIO_TYPE _item, float time, float _desiredVolume = 0f)
        {
            if (m_instance)
                Instance._FadeTypeOut(_item, time, _desiredVolume);
        }

        public virtual void _FadeTypeOut(VP_AudioSetup.AUDIO_TYPE _item, float time, float _desiredVolume = 0f)
        {
            switch (_item)
            {
                case VP_AudioSetup.AUDIO_TYPE.BGM:
#if DOTWEEN
                    m_bgmSource.DOFade(_desiredVolume, time);
#else
                    DoFade(m_bgmSource, _desiredVolume, time);
#endif
                    break;
                case VP_AudioSetup.AUDIO_TYPE.SFX:
#if DOTWEEN
                    m_sfxSource.DOFade(_desiredVolume, time);
#else
                    DoFade(m_sfxSource, _desiredVolume, time);
#endif
                    break;
                case VP_AudioSetup.AUDIO_TYPE.VOICE:
#if DOTWEEN
                    m_voiceSource.DOFade(_desiredVolume, time);
#else
                    DoFade(m_voiceSource, _desiredVolume, time);
#endif
                    break;
            }

            foreach (VP_AudioItem item in m_audioDictionary.Values)
            {
                if (item.m_type == _item)
                {
#if DOTWEEN
                    item.m_source.DOFade(_desiredVolume, time);
#else
                    DoFade(item.m_source, _desiredVolume, time);
#endif
                }
            }

        }

        public static void FadeItemOut(string _item, float time, float _desiredVolume = 0f)
        {
            if (m_instance)
                Instance.FadeOutItem(_item, time, _desiredVolume);
        }

        public virtual void FadeOutItem(string _item, float time, float _desiredVolume = 0f)
        {
            if (m_audioDictionary.ContainsKey(_item))
            {
                StopCoroutine(FadeOutCor(m_audioDictionary[_item].m_source, time));
                StartCoroutine(FadeOutCor(m_audioDictionary[_item].m_source, time));
            }
        }

        protected virtual IEnumerator FadeOutCor(AudioSource source, float time, float _desiredVolume = 0f)
        {
            float amount = source.volume / time;

            while (amount > _desiredVolume)
            {
                if (source == null)
                    break;


                source.volume -= amount;
                yield return null;
            }


        }


        public virtual void SetItemsOfTypeVolume(VP_AudioSetup.AUDIO_TYPE _type, float _newBGMValue)
        {
            switch (_type)
            {
                case VP_AudioSetup.AUDIO_TYPE.BGM:
                    m_bgmVolume = _newBGMValue;
                    break;
                case VP_AudioSetup.AUDIO_TYPE.SFX:
                    m_sfxVolume = _newBGMValue;
                    break;
                case VP_AudioSetup.AUDIO_TYPE.VOICE:
                    m_voiceVolume = _newBGMValue;
                    break;
            }

            if (m_audioDictionary == null)
            {
                m_audioDictionary = new VP_AudioDictionary<T>();
            }

            foreach (VP_AudioItem item in m_audioDictionary.Values)
            {
                if (item == null)
                    continue;

                if (_type == item.m_type)
                {
                    item.m_source.volume = item.m_originalVolume * _newBGMValue * m_masterVolume;
                }
            }
        }

        public virtual void SetVolumesSettings(VP_AudioSettings settings, bool _save)
        {
            if (settings == null)
                return;

            m_bgmVolume = settings.m_bgmVolume;
            m_sfxVolume = settings.m_sfxVolume;
            m_voiceVolume = settings.m_voiceVolume;

            if (m_audioDictionary == null)
            {
                m_audioDictionary = new VP_AudioDictionary<T>();
            }

            foreach (VP_AudioItem item in m_audioDictionary.Values)
            {
                if (item == null)
                    continue;

                switch (item.m_type)
                {
                    case VP_AudioSetup.AUDIO_TYPE.BGM:

                        item.m_source.volume = item.m_originalVolume * m_bgmVolume * m_masterVolume;
                        break;
                    case VP_AudioSetup.AUDIO_TYPE.SFX:

                        item.m_source.volume = item.m_originalVolume * m_sfxVolume * m_masterVolume;
                        break;
                    case VP_AudioSetup.AUDIO_TYPE.VOICE:
                        item.m_source.volume = item.m_originalVolume * m_voiceVolume * m_masterVolume;
                        break;
                }
            }
        }

        public virtual void StopAllItems(VP_AudioSetup.AUDIO_TYPE _type)
        {
            if (m_audioDictionary == null || m_audioDictionary.Count == 0)
                return;

            StopAllSources();

            foreach (VP_AudioItem item in m_audioDictionary.Values)
            {
                if (item == null || item.m_type != _type)
                    continue;

                item.Stop();
            }
        }

        public virtual float GetAudioLengthFromItem(string _item)
        {
            if (m_audioDictionary.ContainsKey(_item))
            {
                return m_audioDictionary[_item].GetAudioLength();
            }

            return 0f;
        }

        public virtual void ClearNullAudios()
        {
            foreach (string key in m_audioDictionary.Keys)
            {
                if (m_audioDictionary[key] == null)
                {
                    m_audioDictionary.Remove(key);
                }
            }
        }

        public virtual void RemoveItem(T item)
        {
            if (m_predefinedAudios == null)
                return;

            if (m_predefinedAudios.Contains(item))
            {
                m_predefinedAudios.Remove(item);
            }

            if (m_audioDictionary.ContainsKey(item.m_key))
            {
                m_audioDictionary.Remove(item.m_key);
            }
        }

        public virtual void RemoveItem(string item)
        {
            if (m_predefinedAudios == null)
                return;

            if (m_audioDictionary.ContainsKey(item))
            {
                m_audioDictionary.Remove(item);
            }
        }

        public virtual void AddToPredefined(T item)
        {
            if (m_predefinedAudios == null)
                return;

            if (!m_predefinedAudios.Contains(item))
            {
                m_predefinedAudios.Add(item);
            }
            else
            {
                Debug.LogError("AudioManager already contains that item.");
            }
        }

        public virtual void StopAudioItem(string _key, bool _removeIt = false)
        {
            if (m_audioDictionary.ContainsKey(_key))
            {
                m_audioDictionary[_key].m_source.Stop();

                if (_removeIt)
                    m_audioDictionary.Remove(_key);
            }
        }

        public virtual void PlayAudioItem(string _key, float _volume = -1f)
        {
            if (m_audioDictionary.ContainsKey(_key))
            {
                VP_AudioItem item = m_audioDictionary[_key];

                float itemMultiplier = 1f;
                switch (item.m_type)
                {
                    case VP_AudioSetup.AUDIO_TYPE.BGM:
                        itemMultiplier = m_masterVolume * m_bgmVolume;
                        break;
                    case VP_AudioSetup.AUDIO_TYPE.SFX:
                        itemMultiplier = m_masterVolume * m_sfxVolume;
                        break;
                    case VP_AudioSetup.AUDIO_TYPE.VOICE:
                        itemMultiplier = m_masterVolume * m_voiceVolume;
                        break;
                }

                item.Play(_volume, itemMultiplier);
            }
        }

        public virtual void PlayItemOneShot(string _key, float _volume = -1f)
        {
            if (m_audioDictionary.ContainsKey(_key))
            {
                VP_AudioItem item = m_audioDictionary[_key];

                float itemMultiplier = 1f;
                switch (item.m_type)
                {
                    case VP_AudioSetup.AUDIO_TYPE.BGM:
                        itemMultiplier = m_masterVolume * m_bgmVolume;
                        break;
                    case VP_AudioSetup.AUDIO_TYPE.SFX:
                        itemMultiplier = m_masterVolume * m_sfxVolume;
                        break;
                    case VP_AudioSetup.AUDIO_TYPE.VOICE:
                        itemMultiplier = m_masterVolume * m_voiceVolume;
                        break;
                }

                item.PlayOneShot(_volume, itemMultiplier);
            }
        }
        public virtual void PlayAndStopPrevious(AudioClip _clip, VP_AudioSetup.AUDIO_TYPE _type = VP_AudioSetup.AUDIO_TYPE.SFX, bool _overrideClip = false, float _volume = 1f, string _keyAdd = "", bool _addItForLater = false, bool _inLoop = false)
        {
            foreach (VP_AudioItem item in m_audioDictionary.Values)
            {
                if (item == null || item.m_type != _type)
                    continue;

                switch (item.m_type)
                {
                    case VP_AudioSetup.AUDIO_TYPE.BGM:

                        item.Stop();
                        break;
                    case VP_AudioSetup.AUDIO_TYPE.SFX:

                        item.Stop();
                        break;
                    case VP_AudioSetup.AUDIO_TYPE.VOICE:
                        item.Stop();
                        break;
                }
            }

            PlayAudio(_clip, _type, _overrideClip, _volume, _keyAdd, _addItForLater, _inLoop);
        }

        public virtual void PlayAndStopPrevious(string _clipKey, VP_AudioSetup.AUDIO_TYPE _type = VP_AudioSetup.AUDIO_TYPE.SFX, float _volume = 1.0f)
        {
            foreach (VP_AudioItem item in m_audioDictionary.Values)
            {
                if (item == null || item.m_type != _type)
                    continue;

                switch (item.m_type)
                {
                    case VP_AudioSetup.AUDIO_TYPE.BGM:

                        item.Stop();
                        break;
                    case VP_AudioSetup.AUDIO_TYPE.SFX:

                        item.Stop();
                        break;
                    case VP_AudioSetup.AUDIO_TYPE.VOICE:
                        item.Stop();
                        break;
                }
            }

            if (m_audioDictionary.ContainsKey(_clipKey))
                m_audioDictionary[_clipKey].SetVolume(_volume);

            PlayAudioItem(_clipKey);
        }

        public virtual T Create(string _key, AudioClip _clip, AudioSource _source, VP_AudioSetup.AUDIO_TYPE _type, float _originalVolume, bool _overrideClip, float multiplier)
        {
            T audioItem = default(T);
            audioItem.Create(_key, _clip, _source, _type, _originalVolume, _overrideClip, GetVolumeMultiplier(_type));
            return audioItem;
        }

        public virtual void PlayAudio(AudioClip _clip, VP_AudioSetup.AUDIO_TYPE _type = VP_AudioSetup.AUDIO_TYPE.SFX, bool _overrideClip = false, float _volume = 1f, string _keyAdd = "", bool _addItForLater = false, bool _inLoop = false)
        {
            if (_clip == null)
                return;

            if (_addItForLater)
            {
                string _key = string.IsNullOrEmpty(_keyAdd) ? _clip.name : _keyAdd;
                if (m_audioDictionary.ContainsKey(_key))
                {
                    m_audioDictionary[_key].Play();
                }
                else
                {
                    GameObject newSource = new GameObject();
                    newSource.transform.parent = this.transform;
                    AudioSource source = newSource.AddComponent<AudioSource>();
                    source.clip = _clip;

                    T audioItem = Create(_key, _clip, source, _type, _volume, _overrideClip, GetVolumeMultiplier(_type));


                    switch (_type)
                    {
                        case VP_AudioSetup.AUDIO_TYPE.BGM:
                            audioItem.m_source.volume = m_bgmVolume;
                            break;
                        case VP_AudioSetup.AUDIO_TYPE.SFX:
                            audioItem.m_source.volume = m_sfxVolume;
                            break;
                        case VP_AudioSetup.AUDIO_TYPE.VOICE:
                            audioItem.m_source.volume = m_voiceVolume;
                            break;

                    }

                    audioItem.Play();

                    if (_addItForLater || m_addNonExistingItems)
                        m_audioDictionary.Add(_key, audioItem);
                }
            }
            else
            {
                switch (_type)
                {
                    case VP_AudioSetup.AUDIO_TYPE.BGM:
                        m_bgmSource.Stop();
                        m_bgmSource.clip = _clip;
                        m_bgmSource.loop = _inLoop;
                        m_bgmSource.Play();
                        break;
                    case VP_AudioSetup.AUDIO_TYPE.SFX:
                        m_sfxSource.Stop();
                        m_sfxSource.clip = _clip;
                        m_sfxSource.loop = _inLoop;
                        m_sfxSource.Play();
                        break;
                    case VP_AudioSetup.AUDIO_TYPE.VOICE:
                        m_voiceSource.Stop();
                        m_voiceSource.clip = _clip;
                        m_voiceSource.loop = _inLoop;
                        m_voiceSource.Play();
                        break;
                }
            }

        }

        public virtual void PlayAudioOneShot(AudioClip _clip, AudioSource _source, VP_AudioSetup.AUDIO_TYPE _type = VP_AudioSetup.AUDIO_TYPE.SFX, float _volume = 1f)
        {
            if (_clip == null || _source == null)
                return;

            _source.PlayOneShot(_clip, GetVolumeMultiplier(_type)*_volume);
        }

        public virtual void PlayAudio(AudioClip _clip, AudioSource _source, VP_AudioSetup.AUDIO_TYPE _type = VP_AudioSetup.AUDIO_TYPE.SFX, bool _overrideClip = false, float _volume = 1f, string _keyAdd = "", bool _addItForLater = false, bool _inLoop = false)
        {
            if (_clip == null || _source == null)
                return;

            if (_addItForLater)
            {
                string _key = string.IsNullOrEmpty(_keyAdd) ? _clip.name : _keyAdd;
                if (m_audioDictionary.ContainsKey(_key))
                {
                    m_audioDictionary[_key].Play();
                }
                else
                {

                    AudioSource source = _source;
                    source.clip = _clip;

                    T audioItem = Create(_key, _clip, source, _type, _volume, _overrideClip, GetVolumeMultiplier(_type));


                    switch (_type)
                    {
                        case VP_AudioSetup.AUDIO_TYPE.BGM:
                            audioItem.m_source.volume = GetVolumeMultiplier(_type) * _volume;
                            break;
                        case VP_AudioSetup.AUDIO_TYPE.SFX:
                            audioItem.m_source.volume = GetVolumeMultiplier(_type) * _volume;
                            break;
                        case VP_AudioSetup.AUDIO_TYPE.VOICE:
                            audioItem.m_source.volume = GetVolumeMultiplier(_type) * _volume;
                            break;

                    }

                    audioItem.Play();

                    if (_addItForLater || m_addNonExistingItems)
                        m_audioDictionary.Add(_key, audioItem);
                }
            }
            else
            {
                _source.Stop();
                _source.clip = _clip;
                _source.volume = GetVolumeMultiplier(_type) * _volume;
                _source.loop = _inLoop;
                _source.Play();
            }

        }

        public virtual void PlayOneShot(AudioClip _clip, VP_AudioSetup.AUDIO_TYPE _type, float _volume, string _keyAdd, bool _addItForLater)
        {
            base.PlayOneShot(_clip, _type, _volume);
            if (_addItForLater && !string.IsNullOrEmpty(_keyAdd))
            {
                if (!m_audioDictionary.ContainsKey(_keyAdd))
                {
                    T audioItem = Create(_keyAdd, _clip, m_oneShotSource, _type, _volume, false, GetVolumeMultiplier(_type));
                    m_audioDictionary.Add(_keyAdd, audioItem);
                }

            }
        }

        public virtual void AddAudioItem(T _item, bool _play = false, bool _fadeIn = false, bool _needInitialization = false)
        {
            if (_needInitialization)
            {
                switch (_item.m_type)
                {
                    case VP_AudioSetup.AUDIO_TYPE.BGM:
                        _item.m_source.volume = m_bgmVolume * _item.m_originalVolume * m_masterVolume;
                        break;
                    case VP_AudioSetup.AUDIO_TYPE.SFX:
                        _item.m_source.volume = m_sfxVolume * _item.m_originalVolume * m_masterVolume;
                        break;
                    case VP_AudioSetup.AUDIO_TYPE.VOICE:
                        _item.m_source.volume = m_voiceVolume * _item.m_originalVolume * m_masterVolume;
                        break;
                }
            }

            if (m_audioDictionary.ContainsKey(_item.m_key))
            {
                if (_play)
                {
                    if (!_fadeIn)
                        _item.Play();
                    else
                        _item.FadeIn();
                }
                else
                {
                    _item.Stop();
                }
            }
            else
            {
                m_audioDictionary.Add(_item.m_key, _item);

                if (_play)
                    _item.Play();
                else
                    _item.Stop();
            }
        }

        public virtual void SetDimensionOfAudioItem(string _key, float _dimValue)
        {
            if (AudioAlreadyExists(_key))
            {
                m_audioDictionary[_key].m_source.panStereo = _dimValue;
            }
        }

        public virtual void SetPitchOfAudioItem(string _key, float _newPitch)
        {
            if (AudioAlreadyExists(_key))
            {
                m_audioDictionary[_key].m_source.pitch = _newPitch;
            }
        }

        public virtual bool AudioAlreadyExists(string _key)
        {
            return m_audioDictionary.ContainsKey(_key);
        }

        /// <summary>
        /// Play an audio based on the audioName/key. That Audio is stored in the audioManager
        /// </summary>
        /// <param name="_audioName"></param>
        public virtual void PlayAudio(string _audioName, VP_AudioSetup.AUDIO_TYPE _type = VP_AudioSetup.AUDIO_TYPE.SFX, bool _overrideClip = false, float _volume = 1f)
        {
            if (m_audioDictionary.ContainsKey(_audioName))
            {
                m_audioDictionary[_audioName].Play();
            }
            else
            {
                AudioClip audio = Resources.Load<AudioClip>(VP_AudioSetup.AUDIO_PATH + _audioName);

                if (audio)
                {
                    GameObject newSource = new GameObject();
                    newSource.transform.parent = this.transform;
                    AudioSource source = newSource.AddComponent<AudioSource>();
                    source.clip = audio;

                    T audioItem = Create(_audioName, audio, source, _type, _volume, _overrideClip, GetVolumeMultiplier(_type));

                    switch (_type)
                    {
                        case VP_AudioSetup.AUDIO_TYPE.BGM:
                            audioItem.m_source.volume = m_bgmVolume;
                            break;
                        case VP_AudioSetup.AUDIO_TYPE.SFX:
                            audioItem.m_source.volume = m_sfxVolume;
                            break;
                    }

                    audioItem.Play();

                    if (m_addNonExistingItems)
                        m_audioDictionary.Add(_audioName, audioItem);
                }

            }
        }

        public virtual void StopAndPlayAudioItems(string _stopKey, string _playKey, bool _oneShot = false)
        {
            StopAudioItem(_stopKey);

            if (!_oneShot)
                PlayAudioItem(_playKey);
            else
                PlayItemOneShot(_playKey);
        }

        public virtual void StopAndPlayAudios(string _stopKey, AudioClip _playClip, VP_AudioSetup.AUDIO_TYPE _type, bool _oneShot = false, bool _override = true, float _volume = 1f, string key = "", bool _add = true)
        {
            StopAudioItem(_stopKey);

            if (!_oneShot)
                PlayAudio(_playClip, _type, _override, _volume, key, _add);
            else
                PlayOneShot(_playClip, _type, _volume, key, _add);
        }

        protected virtual IEnumerator DestroyAfterPlay(float _time, string _key, GameObject _sourceObj)
        {
            float timer = 0;

            while (timer < _time)
            {
                yield return null;
            }

            if (m_audioDictionary.ContainsKey(_key))
                m_audioDictionary.Remove(_key);

            Destroy(_sourceObj);
        }

        public virtual bool IsItemPlaying(string _key)
        {
            return m_audioDictionary.ContainsKey(_key) && m_audioDictionary[_key].IsPlaying();
        }

        public static void PlayAudioItemByKey(string _itemKey)
        {
            if (nullableInstance)
                Instance.PlayAudioItem(_itemKey);
        }
        public static void StopAudioItembyKey(string _itemKey)
        {
            if (nullableInstance)
                Instance.StopAudioItem(_itemKey);
        }
        public static void PlayAudioItemOneShotbyKey(string _itemKey, float _volume = -1f)
        {
            if (nullableInstance)
                Instance.PlayItemOneShot(_itemKey, _volume);
        }

        public static bool IsAudioItemPlaying(string _itemKey)
        {
            return (nullableInstance) ? Instance.IsItemPlaying(_itemKey) : false;
        }

        public static void StopAndPlayByKey(string _stopKey, string _playKey)
        {
            if (nullableInstance)
            {
                Instance.StopAndPlayAudioItems(_stopKey, _playKey);
            }
        }

        public static void StopAndReplay(string _stopKey)
        {
            if (nullableInstance)
            {
                Instance.StopAndPlayAudioItems(_stopKey, _stopKey);
            }
        }

        public static void StopAudioItembyKey(string _itemKey, bool _deleteIt = false)
        {
            if (nullableInstance)
                Instance.StopAudioItem(_itemKey, _deleteIt);
        }

        public static void StopAllAudiosByType(VP_AudioSetup.AUDIO_TYPE _type)
        {
            if (nullableInstance)
                Instance.StopAllItems(_type);
        }

        public static void PlayOneShot(AudioClip _clip, VP_AudioSetup.AUDIO_TYPE _type = VP_AudioSetup.AUDIO_TYPE.SFX, bool _overrideClip = false, float _volume = 1f, string _keyAdd = "", bool _addItForLater = false)
        {
            if (nullableInstance)
            {
                Instance.PlayOneShot(_clip, _type, _volume, _keyAdd, _addItForLater);
            }
        }

        public static void PlayNewAudio(AudioClip _clip, AudioSource _source, VP_AudioSetup.AUDIO_TYPE _type = VP_AudioSetup.AUDIO_TYPE.SFX, bool _overrideClip = false, float _volume = 1f, string _keyAdd = "", bool _addItForLater = false, bool _inLoop = false)
        {
            if (nullableInstance)
            {
                Instance.PlayAudio(_clip, _source, _type, _overrideClip, _volume, _keyAdd, _addItForLater, _inLoop);
            }
        }

        public static void PlayNewAudio(AudioClip _clip, VP_AudioSetup.AUDIO_TYPE _type = VP_AudioSetup.AUDIO_TYPE.SFX, bool _overrideClip = false, float _volume = 1f, string _keyAdd = "", bool _addItForLater = false, bool _inLoop = false)
        {
            if (nullableInstance)
            {
                Instance.PlayAudio(_clip, _type, _overrideClip, _volume, _keyAdd, _addItForLater, _inLoop);
            }
        }

    }
}