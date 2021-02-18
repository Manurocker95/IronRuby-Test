using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Animations
{
	public class VP_AnimationFloatParameter : VP_AnimationParameter<float>
	{
		public override void SetParameter(Animator _animator)
		{
			_animator.SetFloat(m_parameterName, m_value);
		}
		
		public override float GetParameterValue(Animator _animator)
		{
			return _animator.GetFloat(m_parameterName);
		}
	}
}
