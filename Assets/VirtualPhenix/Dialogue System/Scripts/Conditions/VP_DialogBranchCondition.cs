using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Dialog
{
    [CreateAssetMenu(menuName = "Virtual Phenix/Dialogue System/Data/Branch Condition Data")]
    public class VP_DialogBranchCondition : VP_ScriptableObject
    {
        private Dictionary<string, List<VP_DialogCondition>> conditions;

        private void Awake()
        {
            if (conditions == null)
                conditions = new Dictionary<string, List<VP_DialogCondition>>();

            /*
              foreach (GameObject obj in Object.FindObjectsOfType(typeof(GameObject)))
              {
                  GUIContent content = new GUIContent(obj.name);
                  menu.AddItem(content, obj == currentObj, () => SelectGOInfo(property, obj));
              }
              */
        }

        public void AddToDictionary(string _key, VP_DialogCondition condition)
        {
            if (conditions.ContainsKey(_key))
            {
                if (conditions[_key] == null)
                {
                    conditions[_key] = new List<VP_DialogCondition>();
                }

                if (!conditions[_key].Contains(condition))
                {
                    conditions[_key].Add(condition);
                }
            }
        }

        public bool Invoke(string _key, int index)
        {
            if (!conditions.ContainsKey(_key) || index >= conditions[_key].Count)
            {
                return false;
            }
            return conditions[_key][index].Invoke();
        }
    }
}
