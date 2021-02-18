using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Animations
{	
	public class VP_AnimatorParameterSetter : VP_Monobehaviour, VP_IParameterSetteable
	{
	
		[SerializeField] private VP_AnimatorControllerParameters m_parameters = new VP_AnimatorControllerParameters(); 
		
		public virtual void SetParameter()
		{
			foreach (Animator anm in m_parameters.Keys)
			{
				if (anm == null || m_parameters[anm] == null)
					continue;
					
				foreach (VP_BasicAnimatorParameter par in m_parameters[anm])
				{
					par.SetParameter(anm);
				}
			}
		}
		
		public virtual void SetParameterInIndex(int _index)
		{
			foreach (Animator anm in m_parameters.Keys)
			{
				if (anm == null || m_parameters[anm] == null || m_parameters[anm].Count == 0)
					continue;

				m_parameters[anm][_index].SetParameter(anm);
			}
		}
	}
}

