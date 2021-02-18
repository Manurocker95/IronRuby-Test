using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix.Dialog;

namespace VirtualPhenix
{
	[System.Serializable]
	public class VP_AnimatorParameter
	{
		[Header("Animation Name")]
		public string AnimationName = "Idle";
		
		[Header("Parameter Type")]
		public VariableTypes ParameterType = VariableTypes.Bool;
		
		[Header("Possible Values")]
		public int IntValue;
		public float FloatValue;
		public bool BoolValue;
		
		[Header("Animancer Set Data")]
		public int MaskIndex = -1;
		public int GroupIndex = 0;
		public string AnimationSet = "Default";		
		
		public object GetParameter()
		{
			switch (ParameterType)
			{
			case VariableTypes.Bool:
				return BoolValue;
			case VariableTypes.Float:
				return FloatValue;
			case VariableTypes.Int:
				return IntValue;
			default:
				return "";
			}
		}
		
		public void SetParameterValue(object value)
		{
			if (value is int)
			{
				IntValue = (int)value;
			}
			else if (value is float)
			{
				FloatValue = (float)value;
			}
			else if (value is bool)
			{
				BoolValue = (bool)value;
			}
		}
	}
}