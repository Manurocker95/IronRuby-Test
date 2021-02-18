using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Animations
{
	public class VP_BasicAnimatorParameter
	{
		[SerializeField] protected bool m_randomPoint;
		
		public virtual void SetParameter(Animator _animator)
		{
			
		}
		
		public virtual void UpdateRandomPoint(Animator _animator)
		{
			if (m_randomPoint)
			{
				_animator.Update(Random.value);
			}
		}
	}
}
