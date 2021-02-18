using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix
{
    [System.Serializable]
    public class VP_AudioClipSerializableData
    {
        public float[] m_sampleData;
        public string m_name;
        public float m_length;
        public int m_frequency;
        public int m_channels;
        public int m_samples;
        public bool m_ambisonic;
        public AudioClipLoadType m_loadType;
        public bool m_preloadAudioData;
        public bool m_loadInBackground;
        public bool m_is3D;
        public AudioDataLoadState m_loadState;

        public void SetFromAudioClip(AudioClip _clip)
        {
            m_ambisonic = _clip.ambisonic;
            m_channels = _clip.channels;
            m_frequency = _clip.frequency;
            m_length = _clip.length;
            m_loadInBackground = _clip.loadInBackground;
            m_loadState = _clip.loadState;
            m_loadType = _clip.loadType;
            m_preloadAudioData = _clip.preloadAudioData;
            m_samples = _clip.samples;
            m_name = _clip.name;

            float[] samples = new float[_clip.samples * _clip.channels];
            _clip.GetData(samples, 0);

            m_sampleData = samples;
        }

        public AudioClip GetAudioClipFromData()
        {
            AudioClip m_clip = AudioClip.Create(m_name, m_samples, m_channels, m_frequency, m_ambisonic);
            m_clip.SetData(m_sampleData, 0);
            return m_clip;
        }
    }
}