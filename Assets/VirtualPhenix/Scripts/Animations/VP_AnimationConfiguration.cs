using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix.ResourceReference;

namespace VirtualPhenix
{
	[System.Serializable]
	public class VP_AnimationConfigurationParameter
	{
		public string m_parameterName;
		public System.Type m_type;
	}
	
	[System.Serializable]
	[CreateAssetMenu(fileName = "Animation Configuration", menuName = "Virtual Phenix/Resource Dictionary/Animation/Animation Config", order = 1)]
   
	public class VP_AnimationConfiguration : VP_ResourceReferencer<string, string>
	{
		public VP_AnimationConfiguration()
		{
			SetDefaultValues();
		}
		
		public void SetDefaultValues()
		{
			m_resources = new VP_SerializableDictionary<string, string>()
			{
				{ VP_AnimationSetup.IDs.IDLE, VP_AnimationSetup.NPC.IDLE },
				{ VP_AnimationSetup.IDs.WALK, VP_AnimationSetup.NPC.WALK },
				{ VP_AnimationSetup.IDs.RUN, VP_AnimationSetup.NPC.RUN },
				{ VP_AnimationSetup.IDs.PREPARE_JUMP, VP_AnimationSetup.NPC.PREPARE_JUMP },
				{ VP_AnimationSetup.IDs.JUMP, VP_AnimationSetup.NPC.JUMP },
				{ VP_AnimationSetup.IDs.FALL, VP_AnimationSetup.NPC.FALL },
				{ VP_AnimationSetup.IDs.LAND, VP_AnimationSetup.NPC.LAND },
				{ VP_AnimationSetup.IDs.BUTT_SMASH, VP_AnimationSetup.NPC.BUTT_SMASH },
				{ VP_AnimationSetup.IDs.INTERACT, VP_AnimationSetup.NPC.INTERACT },
				{ VP_AnimationSetup.IDs.START_TALK, VP_AnimationSetup.NPC.START_TALK },
				{ VP_AnimationSetup.IDs.TALK, VP_AnimationSetup.NPC.TALK },
				{ VP_AnimationSetup.IDs.END_TALK, VP_AnimationSetup.NPC.END_TALK },
				{ VP_AnimationSetup.IDs.START_TALK_SIT, VP_AnimationSetup.NPC.START_TALK_SIT },
				{ VP_AnimationSetup.IDs.TALK_SIT, VP_AnimationSetup.NPC.TALK_SIT },
				{ VP_AnimationSetup.IDs.END_TALK_SIT, VP_AnimationSetup.NPC.END_TALK_SIT },
				{ VP_AnimationSetup.IDs.MOUTH_IDLE, VP_AnimationSetup.NPC.MOUTH_IDLE },
				{ VP_AnimationSetup.IDs.MOUTH_TALK, VP_AnimationSetup.NPC.MOUTH_TALK },
				{ VP_AnimationSetup.IDs.GIVE_ITEM, VP_AnimationSetup.NPC.GIVE_ITEM },
			};
		}
		
	#if UNITY_EDITOR	
		[UnityEditor.MenuItem("Virtual Phenix/Resource Dictionary/Animation/Fill Animation Config with default Values")]
		public static void FillAnimationConfigWithDefaultValues()
		{
			var gos = UnityEditor.Selection.objects;
			
			foreach (Object go in gos)
			{
				if (go is VP_AnimationConfiguration)
				{
					(go as VP_AnimationConfiguration).SetDefaultValues();
				}
			}
		}
	#endif
	}
}