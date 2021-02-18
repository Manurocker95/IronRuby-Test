using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace VirtualPhenix.Dialog
{
    [System.Serializable]
    public class Field<T> : FieldData, IField
    {
        Type IField.Type => typeof(T);
        public T Value { get; set; }

        public Field(string _name, int _length, T _value)
        {
            this.Name = _name;
            this.Length = _length;
            this.Value = _value;
            this.m_type = (typeof(T));
        }

        public Field(string _name, T _value)
        {
            this.Name = _name;
            this.Length = 1;
            this.Value = _value;
            this.m_type = (typeof(T));
        }


        public override string ToString()
        {
            return Value.ToString();
        }

        public virtual bool IsTheSameAs (T _value)
        {
            return (object)this.Value == (object)_value;
        }
    }
}
