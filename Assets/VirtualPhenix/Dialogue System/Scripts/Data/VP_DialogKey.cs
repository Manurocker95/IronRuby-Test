using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix
{
    [System.Serializable, CreateAssetMenu(fileName="New Key DB", menuName = "Virtual Phenix/Dialogue System/Data/Dialog Keys")]
    public class VP_DialogKey : VP_ScriptableObject
    {
       [SerializeField] public string key;
       [SerializeField] public Dictionary<string, string> list;  
    }
}
