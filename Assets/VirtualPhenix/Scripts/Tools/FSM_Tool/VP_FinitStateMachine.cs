#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

namespace VirtualPhenix
{
    [System.Serializable]
    public class VP_FinitStateMachine
    {
        public Dictionary<int,List<VP_StateMachineBehaviour>> m_states;
        public Dictionary<string, AnimatorControllerParameterType> m_paramAndTypes;

        public int LayerCount;
        public int StateCount;

        public VP_FinitStateMachine(UnityEditor.Animations.AnimatorController animator)
        {
            m_states = new Dictionary<int, List<VP_StateMachineBehaviour>>();
            UnityEngine.Debug.Log("Trying to parse Animator " + animator.name);
            LayerCount = animator.layers.Length;

            UnityEditor.Animations.AnimatorStateMachine rootStateMachine;
          
            List<VP_StateMachineBehaviour> stateMachineStates = new List<VP_StateMachineBehaviour>();
            List<string> _parameters = new List<string>();

            for (int i = 0; i < LayerCount; i++)
            {
                // For each layer, we get the state machine
                rootStateMachine = animator.layers[i].stateMachine;
                stateMachineStates.Clear();

                foreach (ChildAnimatorState state in rootStateMachine.states)
                {
                    VP_StateMachineBehaviour smb = new VP_StateMachineBehaviour(state.state.name);
                 
                    for (int j = 0; j < state.state.transitions.Length; j++)
                    {
                        _parameters = new List<string>();
                        _parameters.Add(""); // default - blank condition

                        if (state.state.transitions[j].conditions.Length == 0)
                        {
                            smb.SetAnimationValues(state.state.motion.name, state.state.motion.averageDuration, state.state.speed);
                            smb.noCondition = true;
                        }

                        foreach (AnimatorCondition ac in state.state.transitions[j].conditions)
                        {
                           // Animator.StringToHash();
                            //state.state.nameHash
                            _parameters.Add(ac.parameter);

                        }
                        smb.SetParameters(_parameters, state.state.transitions[j].destinationState.name);
                    }

                    //For Micah's transition array approach for each state
                    smb.SetParameters(state.state.name, state.state.transitions);

                    stateMachineStates.Add(smb);
                }

                m_states.Add(i, stateMachineStates);
            }

            StateCount = m_states.Count;


            ///Create a dictionary with the params of the animator, dictionary<name and type>
            /// param names without spaces
            m_paramAndTypes = new Dictionary<string, AnimatorControllerParameterType>();
            AnimatorControllerParameter param;
            for (int i = 0; i < animator.parameters.Length; i++)
            {
                
                param = animator.parameters[i];
                m_paramAndTypes.Add(param.name.Replace(" ", string.Empty), param.type);
            }   

        }

        public string GetStateName(int _layer, int _index)
        {
            return m_states[_layer][_index].stateName;
        }

        public bool HasCondition(int _layer, int _index)
        {
            return !m_states[_layer][_index].noCondition;
        }

        public bool HasConditionByName(string _name)
        {
            foreach (List<VP_StateMachineBehaviour> list in m_states.Values)
            {
                foreach (VP_StateMachineBehaviour s in list)
                {
                    if (_name == s.stateName)
                        return !s.noCondition;
                }
            }

            return false;
        }

        public VP_StateMachineBehaviour GetStateByName(string _name)
        {
            foreach (List<VP_StateMachineBehaviour> list in m_states.Values)
            {
                foreach (VP_StateMachineBehaviour s in list)
                {
                    if (_name == s.stateName)
                        return s;
                }
            }

            return null;
        }

        public bool HasClipByName(string _name)
        {
            foreach (List<VP_StateMachineBehaviour> list in m_states.Values)
            {
                foreach (VP_StateMachineBehaviour s in list)
                {
                    if (_name == s.stateName)
                        return s.HasClip();
                }
            }

            return false;
        }
    }

}
#endif