using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Animations
{
	public class VP_AnimationBoolParameter : VP_AnimationParameter<bool>
	{
		public override void SetParameter(Animator _animator)
		{
			_animator.SetBool(m_parameterName, m_value);
		}
		
		public override bool GetParameterValue(Animator _animator)
		{
			return _animator.GetBool(m_parameterName);
		}
	}
}
