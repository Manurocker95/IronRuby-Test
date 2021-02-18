
namespace VirtualPhenix
{
#if UNITY_EDITOR
    using UnityEngine;
    using UnityEditor;
    using System.Collections;

    public class VP_PhoenixToolsInfo : VP_ScriptableObject
    {
        [MenuItem("Virtual Phenix/Tools/Show Phoenix Tools Version")]
        public static void ShowVersion()
        {
            EditorUtility.DisplayDialog("Current Version: ", VP_PhoenixToolsSetup.CURRENT_VERSION, "Ok");
        }
    }
#endif

}