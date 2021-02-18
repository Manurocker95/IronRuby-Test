using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix.Controllers.Components;

namespace VirtualPhenix.AI
{
	[AddComponentMenu("")]
    public class VP_BehaviorTree : VP_CharacterComponent
	{
        public virtual void SetState(string _stateName)
        {

        }
      
		public virtual void SetInitialVariables()
        {

        }
        
		public virtual bool HasVariable(string _name)
		{
			return false;
		}
		
		public virtual void SetVariableToBehaviorTree<T>(string _name, T _value)
		{
	
		}
    }
}