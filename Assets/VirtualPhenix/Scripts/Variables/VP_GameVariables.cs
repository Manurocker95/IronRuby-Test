
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix.Dialog;

namespace VirtualPhenix.Variables
{
    
    [CreateAssetMenu(fileName = "GameVariablesDB", menuName = "Virtual Phenix/Scriptable Objects/Variables", order = 1)]
    public class VP_GameVariables : VP_ScriptableObject
    {
        [SerializeField] protected VP_GameVariablesData m_data;

        public VP_GameVariablesData Data { get { return m_data; } }
    }
}
