#if ODIN_INSPECTOR
using Sirenix.Serialization;

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if USE_ADDRESSABLES 
using UnityEngine.AddressableAssets;

namespace VirtualPhenix.Formatter
{
    public class MIS_AddressableResolver : IExternalGuidReferenceResolver
    {
        // Multiple string reference resolvers can be chained together.
        public IExternalGuidReferenceResolver NextResolver { get; set; }

        public async Task InstantiateAddressableAsset<T>(AssetReference reference, T _var) where T : Object
        {
            _var = (await reference.InstantiateAsync().Task as T);
        }

        public bool CanReference(object value, out System.Guid guid)
        {
#if UNITY_EDITOR
            if (value is ScriptableObject)
            {

                guid = new System.Guid(AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(value as ScriptableObject)));
                return true;
            }
#endif
            guid = default(System.Guid);
            return false;
        }

        public bool TryResolveReference(System.Guid guid, out object value)
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
#endif