using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Localization
{
	[System.Serializable]
	public class VP_LocalizationConfig
	{
		public SystemLanguage CurrentLanguage;
		public SystemLanguage DefaultLanguage = SystemLanguage.English;
		public bool CanChangeLanguage = true;
		public bool UseGoogleTranslation = false;
		
	}
}