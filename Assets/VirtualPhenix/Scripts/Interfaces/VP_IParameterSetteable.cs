using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix
{
	public interface VP_IParameterSetteable
	{
		void SetParameter();
		void SetParameterInIndex(int _index);
	}
}
