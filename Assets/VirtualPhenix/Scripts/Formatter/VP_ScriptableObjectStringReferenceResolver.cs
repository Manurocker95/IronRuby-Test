#if ODIN_INSPECTOR
using Sirenix.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace VirtualPhenix.Formatter
{
    public class VP_ScriptableObjectStringReferenceResolver : IExternalStringReferenceResolver
    {
        // Multiple string reference resolvers can be chained together.
        public IExternalStringReferenceResolver NextResolver { get; set; }


        public bool CanReference(object value, out string id)
        {
#if UNITY_EDITOR
            if (value is ScriptableObject)
            {
                id = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(value as ScriptableObject));
                return true;
            }
#endif
            id = "";
            return false;
        }

        public bool TryResolveReference(string id, out object value)
        {
            value = null;

#if UNITY_EDITOR
            value = AssetDatabase.LoadAssetAtPath<ScriptableObject>(AssetDatabase.GUIDToAssetPath(id));
#endif
            return value != null;
        }

    }
}
#endif