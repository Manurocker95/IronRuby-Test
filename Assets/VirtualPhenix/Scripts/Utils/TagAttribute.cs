using System;
using UnityEngine;

namespace VirtualPhenix
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class TagAttribute : NaughtyAttributes.DrawerAttribute
	{
		public bool UseDefaultTagFieldDrawer = false;

		public TagAttribute()
        {

        }

		public TagAttribute(bool _useDefaultTag = true)
		{
			UseDefaultTagFieldDrawer = _useDefaultTag;
		}
	}

}
