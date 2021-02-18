#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

using UnityEngine;
using VirtualPhenix;

namespace Helpers
{
#if ODIN_INSPECTOR
    public class HierarchyIcon : SerializedMonoBehaviour
#else
    public class HierarchyIcon : MonoBehaviour
#endif
    {
#if UNITY_EDITOR
        [HideInInspector] public Texture2D icon;
        [HideInInspector] public string tooltip;
        [HideInInspector] public int m_iconPosition = 0;
        public bool showIcon = false;
#endif
    }
}
