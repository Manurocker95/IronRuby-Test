using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix
{
    public static partial class VP_Extensions
    {
        public static float HandleFov(this Camera _camera, float _currentFov, float _normalFov, float _velocity, float _d, float _maxVelocity, float _maxFov, float _multiplier = 8f)
        {
            float FAmt = _normalFov;
            //lerp to the velocity
            float LerpAmt = _velocity / _maxVelocity;
            FAmt = Mathf.Lerp(_normalFov, _maxFov, LerpAmt);

            float fovLerp = Mathf.Lerp(_currentFov, FAmt, _d * _multiplier);
            _camera.fieldOfView = fovLerp;

            return fovLerp;
        }
    }
}