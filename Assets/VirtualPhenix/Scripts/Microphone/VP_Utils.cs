#if USE_MICROPHONE
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix
{
    public static partial class VP_Utils
    {
      
        public static AudioClip TrimSamplesOfAudioClip(AudioClip _clip, int _length, string _name = "New Clip")
        {
            if (_length > _clip.length)
            {
                _length = (int)_clip.length;
            }

            float[] samples = new float[_clip.samples * _clip.channels];
            _clip.GetData(samples, 0);
            float[] _resamp = new float[_length];

            for (int i = 0; i < _length; i++)
            {
                _resamp[i] = samples[i];
            }

            AudioClip newClip = AudioClip.Create(_name, _resamp.Length, _clip.channels, _clip.frequency, true);
            newClip.SetData(_resamp, 0);
            return newClip;
        }

        public static AudioClip StartMicrophoneRecord(bool _inLoop = true, int lengthInSeconds = 10, int frequency = 44100, string deviceName = "", int idx = 0)
        {
            if (string.IsNullOrEmpty(deviceName))
            {
                deviceName = GetCurrentMicrophoneDeviceName(idx);
            }

            return HasMicrophoneAvailable() ? Microphone.Start(deviceName, _inLoop, lengthInSeconds, frequency) : null;
        }

        public static int GetMicrophonePosition(string deviceName = "", int idx = 0)
        {
            if (string.IsNullOrEmpty(deviceName))
            {
                deviceName = GetCurrentMicrophoneDeviceName(idx);
            }

            return Microphone.GetPosition(deviceName);
        }

        public static void EndMicrophoneRecord(string deviceName = "", int idx = 0)
        {
            if (string.IsNullOrEmpty(deviceName))
            {
                deviceName = GetCurrentMicrophoneDeviceName(idx);
            }

            Microphone.End(deviceName);
        }

        public static bool IsMicrophoneRecording(string deviceName = "", int idx = 0)
        {
            if (string.IsNullOrEmpty(deviceName))
            {
                deviceName = GetCurrentMicrophoneDeviceName(idx);
            }

            return Microphone.IsRecording(deviceName);
        }

        public static string GetBuiltInMicrophoneDeviceName()
        {
            return "Built-in Microphone";
        }

        public static bool HasMicrophoneAvailable()
        {
            return Microphone.devices.Length > 0;
        }


        public static string GetCurrentMicrophoneDeviceName(int idx = 0)
        {
            return Microphone.devices.Length > 0 ? Microphone.devices[idx] : "No Microphone Available";
        }

        public static int GetAllMicrophoneDeviceCunt()
        {
            return Microphone.devices.Length;
        }

        public static string[] GetAllMicrophoneDeviceNames()
        {
            return Microphone.devices;
        }

    }
}
#endif