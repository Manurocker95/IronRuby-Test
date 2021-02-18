using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix.Privacy;

namespace VirtualPhenix
{
	[DefaultExecutionOrder(VP_ExecutingOrderSetup.PRIVACY_MANAGER), AddComponentMenu(VP_PrefixSetup.MAIN_PREFIX + "/Privacy/Privacy Manager")]
    public class VP_PrivacyManager : VP_GenericPrivacyManager<VP_AppConsent>
    {
        public static new VP_PrivacyManager Instance
        {
            get
            {
                return (VP_PrivacyManager)m_instance;
            }
        }
    }
}