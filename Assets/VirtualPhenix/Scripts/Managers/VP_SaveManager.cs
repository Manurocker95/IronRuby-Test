using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix.Save;
using VirtualPhenix.Formatter;
using VirtualPhenix.Settings;
using System.IO;

namespace VirtualPhenix
{
	[
	DefaultExecutionOrder(VP_ExecutingOrderSetup.SAVE_MANAGER),
	AddComponentMenu(VP_PrefixSetup.MAIN_PREFIX + "/Save/Save Manager")
	]
	public class VP_SaveManager : VP_GenericSaveManager<VP_Save>
	{

	}
}
