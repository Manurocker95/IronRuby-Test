using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if ODIN_INSPECTOR

using Sirenix.Serialization;
using Sirenix.OdinInspector;

#endif

namespace VirtualPhenix
{
   // [CreateAssetMenu(fileName = "Blank_VP_SO", menuName = "Virtual Phenix/Scriptable Objects/", order = 1)]
#if ODIN_INSPECTOR
	[System.Serializable]
    public class VP_ScriptableObject : Sirenix.OdinInspector.SerializedScriptableObject
#else
	[System.Serializable]
    public class VP_ScriptableObject : ScriptableObject
#endif
    {
        // Your data
        protected virtual void OnValidate()
        {
            
        }
    }
}
