#if ODIN_INSPECTOR
using Sirenix.Serialization;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace VirtualPhenix.Formatter
{
    public class VP_ScriptableObjectGuidReferenceResolver : IExternalGuidReferenceResolver
    {
        // Multiple string reference resolvers can be chained together.
        public IExternalGuidReferenceResolver NextResolver { get; set; }

        public bool CanReference(object value, out Guid guid)
        {
#if UNITY_EDITOR
            if (value is ScriptableObject)
            {

                guid = new Guid(AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(value as ScriptableObject)));
                return true;
            }
#endif
            guid = default(Guid);
            return false;
        }

        public bool TryResolveReference(Guid guid, out object value)
        {
#if UNITY_EDITOR
            value = AssetDatabase.LoadAssetAtPath<ScriptableObject>(AssetDatabase.GUIDToAssetPath(guid.ToString()));
#else
            value = null;
#endif

            return value != null;
        }
    }
}

#endif