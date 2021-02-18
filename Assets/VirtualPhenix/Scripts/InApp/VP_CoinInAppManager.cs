using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VirtualPhenix.Formatter;
using VirtualPhenix.Localization;

#if DOTWEEN
using DG.Tweening;
#endif

namespace VirtualPhenix.InApp
{
	[DefaultExecutionOrder(VP_ExecutingOrderSetup.INAPP_MANAGER),AddComponentMenu(VP_PrefixSetup.MAIN_PREFIX+"/In App/In App Manager")]
    public class VP_CoinInAppManager : VP_InAppManager<VP_CoinPack, int, VP_CoinInAppManager>
    {

    }
}
