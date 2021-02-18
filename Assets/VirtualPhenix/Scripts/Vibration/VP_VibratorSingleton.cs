using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if USE_NICEVIBRATIONS
using MoreMountains.NiceVibrations;
#endif

namespace VirtualPhenix.Vibration
{
    public class VP_VibratorSingleton<T> : VP_SingletonMonobehaviour<T> where T : VP_Monobehaviour
    {
        [Header("Vibration"), Space]
        [SerializeField, Range(0, 1)] protected float m_vibrationIntensity = 0.5f;
        [SerializeField, Range(0, 1)] protected float m_vibrationSharpness = 0.75f;

        public bool m_vibration = false;
        public bool m_canVibrate = false;

        public virtual void InitFromSave()
        {
            m_vibration = PlayerPrefs.GetInt("VibrationActive", 1) == 0 ? true : false;
        }

        public virtual void InitializeVibrator()
        {
#if USE_NICEVIBRATIONS
            m_canVibrate = MMVibrationManager.HapticsSupported();
#endif
        }

        public virtual void Vibrate()
        {
#if USE_NICEVIBRATIONS
            if (m_canVibrate && m_vibration)
                MMVibrationManager.TransientHaptic(m_vibrationIntensity, m_vibrationSharpness, true, this);
#endif
        }

        public virtual void Vibrate(int _intensity)
        {
#if USE_NICEVIBRATIONS
            if (m_canVibrate && m_vibration)
                MMVibrationManager.TransientHaptic(_intensity, m_vibrationSharpness, true, this);
#endif
        }

        public virtual void Vibrate(int _leftMotor, int _rightMotor)
        {
#if USE_NICEVIBRATIONS
            if (m_canVibrate && m_vibration)
                MMVibrationManager.TransientHaptic(_leftMotor, _rightMotor);
#endif
        }

        public void Vibrate(float _intensity, float _sharpness)
        {
#if USE_NICEVIBRATIONS
            if (m_canVibrate && m_vibration)
                MMVibrationManager.TransientHaptic(_intensity, _sharpness, true, this);
#endif
        }

        public virtual void StopVibration()
        {
#if USE_NICEVIBRATIONS
            if (m_canVibrate)
                MMVibrationManager.StopAllHaptics(true);
#endif
        }
    }
}