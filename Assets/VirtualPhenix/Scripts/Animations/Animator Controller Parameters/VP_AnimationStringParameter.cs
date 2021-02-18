using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Animations
{
	public class VP_AnimationStringParameter : VP_AnimationParameter<string>
	{
		public override void SetParameter(Animator _animator)
		{
			_animator.SetTrigger(m_parameterName);
		}
		
		public override string GetParameterValue(Animator _animator)
		{
			return "";
		}
	}
}
