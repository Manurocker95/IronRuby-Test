using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix
{
    public static class VP_CustomParser
    {
        public static bool TryParse<TEnum>(string value, out TEnum result) where TEnum : struct, IConvertible
        {
            var retValue = value == null ?
                        false :
                        Enum.IsDefined(typeof(TEnum), value);
            result = retValue ?
                        (TEnum)Enum.Parse(typeof(TEnum), value) :
                        default(TEnum);
            return retValue;
        }

    }

}
