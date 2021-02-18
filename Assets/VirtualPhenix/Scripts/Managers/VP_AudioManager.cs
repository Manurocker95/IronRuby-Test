using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if DOTWEEN
using DG.Tweening;
#endif

#if USE_MORE_EFFECTIVE_COROUTINES
using MEC;
#endif

using VirtualPhenix.Audio;

namespace VirtualPhenix
{
    [
         DefaultExecutionOrder(VP_ExecutingOrderSetup.AUDIO_MANAGER),
         AddComponentMenu(VP_PrefixSetup.MAIN_PREFIX + "/Audio/Audio Manager")
    ]
    public class VP_AudioManager : VP_GenericAudioManager<VP_AudioItem>
    {
        public static new VP_AudioManager Instance
        {
            get
            {
                return (VP_AudioManager)m_instance;
            }
        }

	    public static void PlaySFXOneShot(string _clip, float _volume = 1f)
	    {
		    if (_clip.IsNotNullNorEmpty())
			    PlayAudioItemOneShotbyKey(_clip, _volume);
	    }

        public static void PlaySFXOneShot(AudioClip _clip, float _volume = 1f)
	    {
		    if (_clip != null)
            	PlayOneShot(_clip, VP_AudioSetup.AUDIO_TYPE.SFX, false, _volume);
        }

        public static void PlayBGMOneShot(AudioClip _clip, float _volume = 1f)
	    {
		    if (_clip != null)
            	PlayOneShot(_clip, VP_AudioSetup.AUDIO_TYPE.BGM, false, _volume);
        }

        public override VP_AudioItem Create(string _key, AudioClip _clip, AudioSource _source, VP_AudioSetup.AUDIO_TYPE _type, float _originalVolume, bool _overrideClip, float multiplier)
        {
            VP_AudioItem audioItem = new VP_AudioItem();
            audioItem.Create(_key, _clip, _source, _type, _originalVolume, _overrideClip, GetVolumeMultiplier(_type));
            return audioItem;
        }

        protected override void InitializeDictionary()
        {
            if (m_audioDictionary == null)
            {
                m_audioDictionary = new VP_AudioDictionary<VP_AudioItem>();
            }

            foreach (VP_AudioItem item in m_predefinedAudios)
            {
                m_audioDictionary.Add(item.m_key, item);

	            // VP_Debug.Log("Item: " + item.m_key);
            }
        }
        
	    public override VP_AudioItem CreateItemFromCopy(VP_AudioItem _copy)
	    {
	    	return new VP_AudioItem(_copy);
	    }
    }
}
