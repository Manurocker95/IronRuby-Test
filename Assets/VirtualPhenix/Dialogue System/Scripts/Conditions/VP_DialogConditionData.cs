using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix
{
    public class VP_DialogConditionData<T, T2> : VP_ScriptableObject
    {
        [SerializeField] public string varName;

        [SerializeField] public T var1;
        [SerializeField] public T var2;

        public virtual bool IsEqual()
        {
            return false;
        }

        public virtual bool IsDifferent()
        {
            return false;
        }

        public virtual bool IsGreater()
        {
            return false;
        }

        public virtual bool IsLowerOrEqual()
        {
            return false;
        }

        public virtual bool IsGreaterOrEqual()
        {
            return false;
        }

        public virtual bool IsLower()
        {
            return false;
        }


        public virtual bool CallMethod()
        {
            return false;
        }

        public virtual bool IsActive(int index)
        {
            return false;
        }

        public virtual bool IsTag(string tag, int index)
        {
            return false;
        }
    }

    public class VP_DialogConditionData<T> : ScriptableObject
    {
        [SerializeField] public T var1;

        public virtual bool IsEqualTo(T var2)
        {
            return false;
        }

        public virtual bool IsDifferentTo(T var2)
        {
            return false;
        }

        public virtual bool IsGreaterTo(T var2)
        {
            return false;
        }

        public virtual bool IsLowerOrEqualTo(T var2)
        {
            return false;
        }

        public virtual bool IsGreaterOrEqualTo(T var2)
        {
            return false;
        }

        public virtual bool IsLowerTo(T var2)
        {
            return false;
        }

        public virtual bool CallMethod<T2> (string methodName, T2 parameter)
        {
            return false;
        }

        public virtual bool CallMethod(string methodName)
        {
            return false;
        }


        public virtual bool IsActive()
        {
            return false;
        }

        public virtual bool IsTag(string tag)
        {
            return false;
        }
    }
}