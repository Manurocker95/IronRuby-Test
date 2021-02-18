using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Dialog
{

    public interface IField
    {
        Type Type { get; }
    }

    [System.Serializable]
    public class FieldData : IField
    {

        [SerializeField] protected string m_name;
        protected int m_length;
        protected System.Type m_type;

        public string Name { get { return m_name; } set { m_name = value; } }

        public int Length { get { return m_length; } set { m_length = value; } }

        public Type Type => typeof(FieldData);
    }
}
