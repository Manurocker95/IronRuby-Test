using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix.ResourceReference;

namespace VirtualPhenix
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "Database Dictionary Referencer", menuName = "Virtual Phenix/Resource Dictionary/Full Resources", order = 1)]
	public class VP_ResourceReferencerDictionary : VP_ResourceReferencer<string, VP_ResourceReferencerBase>
    {
		
    }
}