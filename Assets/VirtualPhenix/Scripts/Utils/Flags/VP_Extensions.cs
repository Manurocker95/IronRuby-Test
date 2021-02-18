using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix
{
    public static partial class VP_Extensions
    {
        public static Dictionary<int, string> EnumNamedValues<T>(this T _enum) where T : System.Enum
        {
            var result = new Dictionary<int, string>();
            var values = System.Enum.GetValues(typeof(T));

            foreach (int item in values)
                result.Add(item, System.Enum.GetName(typeof(T), item));

            return result;
        }

        public static T SetFlag<T>(this T _enum, T _b) where T : System.Enum
        {
            System.Int32 b = (System.Int32)(object)_b;

            System.Int32 value = (_enum.SetFlag(b));
            return (T)(object)value;
        }

        public static System.Int32 SetFlag<T>(this T _enum, System.Int32 b) where T : System.Enum
        {
            System.Int32 a = (System.Int32)(object)_enum;
            return a | b;
        }

        public static T UnsetFlag<T>(this T _enum, T _b) where T : System.Enum
        {
            System.Int32 b = (System.Int32)(object)_b;
            System.Int32 value = (_enum.UnsetFlag(b));
            return (T)(object)value;
        }

        public static System.Int32 UnsetFlag<T>(this T _enum, System.Int32 b)
        {
            System.Int32 a = (System.Int32)(object)_enum;
            return a & (~b);
        }

        public static bool HasFlag<T>(this T _enum, T _b) where T : System.Enum
        {
            System.Int32 b = (System.Int32)(object)_b;
            return (_enum.HasFlag(b));
        }

        // Works with "None" as well
        public static bool HasFlag<T>(this T _enum, System.Int32 b)
        {
            System.Int32 a = (System.Int32)(object)_enum;
            return (a & b) == b;
        }

        public static T ToogleFlag<T>(this T _enum, T _b) where T : System.Enum
        {
            System.Int32 b = (System.Int32)(object)_b;
            System.Int32 value = (_enum.ToogleFlag(b));
            return (T)(object)value;
        }

        public static System.Int32 ToogleFlag<T>(this T _enum, System.Int32 b)
        {
            System.Int32 a = (System.Int32)(object)_enum;
            return a ^ b;
        }
    }
}