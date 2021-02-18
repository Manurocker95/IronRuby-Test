using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix.Settings;

namespace VirtualPhenix
{
	[DefaultExecutionOrder(VP_ExecutingOrderSetup.SETTINGS_MANAGER), AddComponentMenu(VP_PrefixSetup.MAIN_PREFIX + "/Settings/Settings Manager")]
    public class VP_SettingsManager<T> : VP_SingletonMonobehaviour<VP_SettingsManager<T>> where T : VP_Settings
    {
        
        public virtual void SetBGMValue()
        {

        }
    }
}
