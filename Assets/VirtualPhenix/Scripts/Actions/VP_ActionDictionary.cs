using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Actions
{
    [System.Serializable]
    public class VP_ActionContainer
    {
        [SerializeField] private List<VP_CustomActions> m_actionList = new List<VP_CustomActions>();

        public List<VP_CustomActions> ActionList { get { return m_actionList; } }
	    public int Count { get { return m_actionList.Count; } }
        
	    
    }
    
	[System.Serializable]
	public class VP_ActionDictionary : VP_SerializableDictionary<int, VP_ActionContainer>
	{

	}
	
}
