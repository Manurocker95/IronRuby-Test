using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Vibration
{
    public interface VP_IVibration
    {
        void InitializeVibrator();
        void Vibrate();
        void Vibrate(int _intensity);
        void Vibrate(int _leftMotor, int _rightMotor);
        void StopVibration();
        void Vibrate(float _intensity, float _sharpness);
    }
}
