using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using VirtualPhenix.ResourceReference;

namespace VirtualPhenix
{
    [CreateAssetMenu(fileName = "SceneDatabase", menuName = "Virtual Phenix/Resource Dictionary/Scenes/Scene Resources", order = 1)]
    public class VP_SceneDatabase : VP_ResourceReferencer<string, VP_SceneRefData>
    {
        public virtual string GetSceneName(string _key)
        {
	        if (ContainsKey(_key))
            {
                return GetResource(_key).SceneName;
            }

            return "";
        }

        public virtual void SetIsLoaded(string _key, bool _value)
        {
            if (ContainsKey(_key))
            {
                GetResource(_key).IsLoaded = _value;
            }
        }

        public virtual bool IsResourceLoaded(string _key, bool _value)
        {
            if (ContainsKey(_key))
            {
                return GetResource(_key).IsLoaded;
            }

            return false;
        }


        public virtual bool IsSceneLoaded(string _key, bool _value)
        {
            if (ContainsKey(_key))
            {
                return SceneManager.GetSceneByName(GetResource(_key).SceneName).isLoaded;
            }

            return false;
        }
    }

    [System.Serializable]
    public class VP_SceneRefData
    {
        public VP_SceneReference Scene;
        public bool IsLoaded;

        public string SceneName
        {
            get
            {
                return Scene.SceneName;
            }
        }
    }
}
