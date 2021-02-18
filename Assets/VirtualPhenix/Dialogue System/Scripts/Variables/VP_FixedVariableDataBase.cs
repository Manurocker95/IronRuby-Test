using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix.Dialog;

namespace VirtualPhenix.Variables
{
    [CreateAssetMenu(fileName = "Variable Database", menuName = "Virtual Phenix/Variable Database", order = 1)]
    public class VP_FixedVariableDataBase : VP_ScriptableObject
    {
        [SerializeField] private VP_VariableDataBase m_variables = new VP_VariableDataBase();
        public VP_VariableDataBase Variables { get { return m_variables; } }

        protected override void OnValidate()
        {
            if (m_variables == null)
                m_variables = new VP_VariableDataBase();
        }
    }
}
