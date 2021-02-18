using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Animations
{
	public class VP_AnimationParameter<T> : VP_BasicAnimatorParameter
	{
		[SerializeField] protected string m_parameterName;
		[SerializeField] protected T m_value;
		
		public virtual T GetParameterValue(Animator _animator)
		{
			return default(T);
		}
	}
}
