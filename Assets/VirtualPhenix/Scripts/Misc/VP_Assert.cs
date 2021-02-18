using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix
{
    public static partial class VP_Assert
    {
        public static void NotNull(object obj, string failureMessage = "")
        {
            if (obj == null)
            {
                throw new System.NullReferenceException("Not Null Assertion failed: " + failureMessage);
            }
        }

        public static void Null(object obj, string failureMessage = "")
        {
            if (obj != null)
            {
                throw new System.Exception("Null Assertion failed: " + failureMessage);
            }
        }

        public static void AssertTrue(bool expression, string failureMessage)
        {
            if (!expression)
            {
                throw new System.Exception("Assertion failed: " + failureMessage);
            }
        }

        public static void WeakAssertTrue(bool expression, string failureMessage)
        {
            if (!expression)
            {
                Debug.LogError("Weak Assertion failed: " + failureMessage);
            }
        }
    }
}