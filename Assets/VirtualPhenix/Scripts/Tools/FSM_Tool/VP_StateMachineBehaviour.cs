#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;

namespace VirtualPhenix
{
    public class VP_StateMachineBehaviour
    {
        public string stateName;
        public bool noCondition;
        public VP_StateMachineBehaviourAnimation animation;

        public Dictionary<List<string>, string> stateMachineTransitionsDictionary;
        public List<string> transitioNNames;

        public Dictionary<string, AnimatorStateTransition[]> stateMachineTransitionsDictionary_Miki;


        public VP_StateMachineBehaviour(string _name)
        {
            stateName = _name;
            noCondition = false;
            transitioNNames = new List<string>();
            stateMachineTransitionsDictionary = new Dictionary<List<string>, string>();
            stateMachineTransitionsDictionary_Miki = new Dictionary<string, AnimatorStateTransition[]>();
        }

        public void SetParameters(List<string> _parameters, string _state)
        {
            transitioNNames.Add(_state);
            stateMachineTransitionsDictionary.Add(_parameters, _state);
        }

        public void SetParameters(string stateName, AnimatorStateTransition[] transitions)
        {
            stateMachineTransitionsDictionary_Miki.Add(stateName, transitions);
        }

        public void SetAnimationValues(string _clipName, float _clipLength, float _clipSpeed)
        {
            animation = new VP_StateMachineBehaviourAnimation(_clipName, _clipLength, _clipSpeed);
        }

        public bool HasClip()
        {
            return !string.IsNullOrEmpty(animation.clipName);
        }
    }
}
#endif