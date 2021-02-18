using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace VirtualPhenix
{

    public static partial class VP_Utils
    {
#if UNITY_EDITOR
        public static List<T> FindAssetsByType<T>(string _assembly = "UnityEngine.") where T : UnityEngine.Object
        {
            List<T> assets = new List<T>();
            string[] guids = UnityEditor.AssetDatabase.FindAssets(string.Format("t:{0}", typeof(T).ToString().Replace(_assembly, "")));
            for (int i = 0; i < guids.Length; i++)
            {
                string assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[i]);
                T asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (asset != null)
                {
                    assets.Add(asset);
                }
            }
            return assets;
        }

        public static class ScriptableObjectUtility
        {
            /// <summary>
            //	This makes it easy to create, name and place unique new ScriptableObject asset files.
            /// </summary>
            public static void CreateAsset<T>() where T : ScriptableObject
            {
                T asset = ScriptableObject.CreateInstance<T>();

                string path = UnityEditor.AssetDatabase.GetAssetPath(UnityEditor.Selection.activeObject);
                if (path == "")
                {
                    path = "Assets";
                }
                else if (Path.GetExtension(path) != "")
                {
                    path = path.Replace(Path.GetFileName(UnityEditor.AssetDatabase.GetAssetPath(UnityEditor.Selection.activeObject)), "");
                }

                string assetPathAndName = UnityEditor.AssetDatabase.GenerateUniqueAssetPath(path + "/New " + typeof(T).ToString() + ".asset");

                UnityEditor.AssetDatabase.CreateAsset(asset, assetPathAndName);

                UnityEditor.AssetDatabase.SaveAssets();
                UnityEditor.AssetDatabase.Refresh();
                UnityEditor.EditorUtility.FocusProjectWindow();
                UnityEditor.Selection.activeObject = asset;
            }

        }
#endif
    }
}
