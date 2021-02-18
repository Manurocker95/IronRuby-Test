#if ODIN_INSPECTOR
using Sirenix.Serialization;
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace VirtualPhenix.Formatter
{
#if ODIN_INSPECTOR
    public class VP_ScriptableObjectDatabaseReferenceResolver : IExternalStringReferenceResolver
    {
        // Multiple string reference resolvers can be chained together.
        public IExternalStringReferenceResolver NextResolver { get; set; }


        public bool CanReference(object value, out string id)
        {
            VP_ScriptableObjectDataBaseManager som = VP_ScriptableObjectDataBaseManager.Instance;

            if (value is ScriptableObject && som)
            {
                short so = som.GetScriptableObjectIDFromFullDatabase(value as ScriptableObject);
                id = so.ToString();

                return true;
            }
            id = "";
            return false;
        }

        public bool TryResolveReference(string id, out object value)
        {
            value = VP_ScriptableObjectDataBaseManager.Instance.GetScriptableObjectFromFullDatabase(id);

            return value != null;
        }

    }
#endif
}
