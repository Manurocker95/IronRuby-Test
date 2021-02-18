using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix
{
    [System.Serializable, CreateAssetMenu(menuName = "Virtual Phenix/Dialogue System/Data/audiokeyData")]
    public class VP_DialogAudioKey : VP_ScriptableObject
    {
	    [SerializeField] public string key;
        #if ODIN_INSPECTOR
	    [Sirenix.Serialization.OdinSerialize] public VP_DialogAudioKeyDictionary list = new VP_DialogAudioKeyDictionary();
        #else
	    [SerializeField] public VP_DialogAudioKeyDictionary list = new VP_DialogAudioKeyDictionary();
        #endif
    }
}
