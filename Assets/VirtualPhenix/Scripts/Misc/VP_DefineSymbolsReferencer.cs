using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix.ResourceReference;

namespace VirtualPhenix
{
	[System.Serializable]
	[CreateAssetMenu(fileName = "Define Symbols Referencer", menuName = "Virtual Phenix/Resource Dictionary/Define Symbols/Project Define Symbols", order = 1)]
   
	public class VP_DefineSymbolsReferencer : VP_ResourceReferencer<string, bool>
	{
		public string InternalName = "VirtualPhenix";
		
		public virtual void SetDefaultSymbols(Dictionary<string, bool> _default)
		{
			foreach (string d in _default.Keys)
			{
				if (!m_resources.Contains(d))
					Add(d, _default[d]);
				else
					m_resources[d] = _default[d];
			}
		}
		
		public virtual void SetDefaultSymbols(IList<string> _default)
		{
			foreach (string d in _default)
			{
				if (!m_resources.Contains(d))
					Add(d, true);
				else
					m_resources[d] = true;
			}
		}
		
		public virtual void SetAvoidSymbols(IList<string> _default)
		{
			foreach (string d in _default)
			{
				if (!m_resources.Contains(d))
					Add(d, false);
				else
					m_resources[d] = false;
			}
		}
		
		public virtual List<string> GetAvoidSymbols()
		{
			List<string> avoid = new List<string>();
			foreach (string s in m_resources.Keys)
			{
				if (!m_resources[s])
				{
					avoid.Add(s);
				}
			}
			
			return avoid;
		}
	}
}
