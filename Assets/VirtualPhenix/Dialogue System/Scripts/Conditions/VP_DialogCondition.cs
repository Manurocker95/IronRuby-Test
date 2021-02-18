using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VirtualPhenix.Dialog
{
    [System.Serializable]
    public class VP_DialogCondition : SerializableCallback<bool>
    {
        public enum CONDITION_TYPE
        {
            FLOAT,
            INT,
            STRING,
            OBJECT
        }

        [SerializeField] public CONDITION_TYPE m_conditionType;
        [SerializeField] public object m_conditionData;
        [SerializeField] public string m_varName;
        [SerializeField] public Object m_object;
        [SerializeField] public IField m_varCondition;
    }
}

