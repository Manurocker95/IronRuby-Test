using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix
{
    [System.Serializable]
    public class VP_StateMachineBehaviourAnimation
    {
        public string clipName;
        public float clipLength;
        public float clipSpeed;
        public Animation animation;

        public VP_StateMachineBehaviourAnimation(string _clipName, float _clipLength, float _clipSpeed)
        {
            clipName = _clipName;
            clipLength = _clipLength;
            clipSpeed = _clipSpeed;
        }
    }

}
