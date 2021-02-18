using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if USE_INCONTROL

using InControl;
namespace VirtualPhenix.Inputs
{
	[AddComponentMenu(VP_PrefixSetup.MAIN_PREFIX+"/UI/Custom In Control Input Module")]
	public class VP_InControlInputModule : VP_InControlInputModuleBase<VP_InControlInputModuleKeyData, VP_InControlInputModuleActions>
	{
		protected override VP_InControlInputModuleKeyData DefaultData()
		{
			return new VP_InControlInputModuleKeyData();
		}
		
		protected override VP_InControlInputModuleActions DefaultAction()
		{
			return new VP_InControlInputModuleActions();
		}
	}
}
#endif