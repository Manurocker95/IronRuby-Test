using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VirtualPhenix.Level;

namespace VirtualPhenix
{
	[DefaultExecutionOrder(VP_ExecutingOrderSetup.LEVEL_MANAGER), AddComponentMenu(VP_PrefixSetup.MAIN_PREFIX + "/Level/Level Manager")]
    public class VP_LevelManager : VP_GenericLevelManager<VP_Level<VP_LevelData>, VP_LevelData>
    {

    }
}
