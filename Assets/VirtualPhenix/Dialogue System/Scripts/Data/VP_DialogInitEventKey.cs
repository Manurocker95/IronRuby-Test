using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Dialog
{
    [System.Serializable, CreateAssetMenu(menuName = "Virtual Phenix/Dialogue System/Data/Init Event Keys")]
    public class VP_DialogInitEventKey : VP_ScriptableObject
    {
        public string m_keySetup;
        [SerializeField] public string key;
	    [SerializeField] public VP_SerializableDictionary<string, string> list;

        public System.Action OnKeySet;

        public void Awake()
        {
            if (string.IsNullOrEmpty(m_keySetup))
            {
                m_keySetup = Application.dataPath + "/VirtualPhenix/Dialogue System/Scripts/Setup/VP_DialogSetup.cs";
            }
        }

        public void StartListeningOnKeySet(System.Action _callback)
        {
            if (OnKeySet == null)
                OnKeySet = _callback;
            else
                OnKeySet += _callback;
        }

        public void StopListeningOnKeySet(System.Action _callback)
        {
            if (OnKeySet != null)
                OnKeySet -= _callback;
        }

        public void OnKeySetInvoke()
        {
            OnKeySet.Invoke();
        }
    }
}
