using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Animations
{
	public class VP_AnimationIntParameter : VP_AnimationParameter<int>
	{
		public override void SetParameter(Animator _animator)
		{
			_animator.SetInteger(m_parameterName, m_value);
		}
		
		public override int GetParameterValue(Animator _animator)
		{
			return _animator.GetInteger(m_parameterName);
		}
	}
}
