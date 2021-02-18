using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;
using System.Linq;
using System.IO;
using UnityEditor.PackageManager;
using UnityEditor.SceneManagement;

namespace VirtualPhenix
{
    public class VP_DependencyInstaller : Editor
    {
        public class AfterImport : AssetPostprocessor
        {
            static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
            {
                foreach (string importedAsset in importedAssets)
                {
                    if (importedAsset.Contains("VP_DependencyInstaller.cs"))
                    {
                        FullInstall();

                        VP_WelcomeWindow.ShowWindow();
                    }
                }
            }
        }

        public static void SetDefineSymbols()
        {
            string m_defineSymbolsPath = GetSymbolPath();
            VP_DefineSymbolsReferencer m_symbolDictionary = null;

            VP_Utils.GetObjectOfTypeInProject(out m_symbolDictionary, "VP_DefineSymbolsReferencer", CheckPath);

            if (m_symbolDictionary == null)
            {
                m_symbolDictionary = Resources.Load<VP_DefineSymbolsReferencer>("Editor/" + VP_DefineSymbolEditor.DEFINE_SYMBOL_ASSET_NAME);
            }

            if (m_symbolDictionary != null)
            {
                VP_AddDefineSymbols.AddDefine(m_symbolDictionary);
            }
        }

        protected static string GetSymbolPath()
        {
            string path = GetSaveFilePath();
            return (System.IO.File.Exists(path)) ? System.IO.File.ReadAllText(path) : "";
        }

        protected static string GetCurrentDirectory()
        {
            return System.IO.Directory.GetCurrentDirectory();
        }

        protected static bool CheckPath(VP_DefineSymbolsReferencer _symbol)
        {
            return VP_Utils.CheckPathOfDefineSymbol(_symbol, GetSymbolPath());
        }


        protected static string GetSaveFilePath()
        {
            string p = GetCurrentDirectory() + VP_DefineSymbolEditor.DEFINE_SYMBOL_TXT_DEFAULT_PATH;

#if UNITY_EDITOR_OSX
			p = p.Replace("\\", "/");
#endif

            if (!System.IO.Directory.Exists(p))
            {
                Debug.Log("Creating " + p);
                System.IO.Directory.CreateDirectory(p);
            }

            string path = p + VP_DefineSymbolEditor.DEFINE_SYMBOL_TXT_NAME;
#if UNITY_EDITOR_OSX
			path = path.Replace("\\", "/");
#endif
            return path;
        }

        [MenuItem("Virtual Phenix/Packages/Install Dependencies")]
        public static void FullInstall()
        {
            SetDefineSymbols();
            InstallDependencies();
        }

        public static void InstallDependencies()
        {
            bool installHappened = false;
#if USE_TILEMAP
            if (!VP_PackageInstallation.IsInstalled(VP_PackageInstallSetup.TILE_MAP_EDITOR_PACKAGE))
            {
                InstallTilemapEditor();
                installHappened = true;
            }
#endif

#if USE_POSTPROCESSING_STACK
            if (!VP_PackageInstallation.IsInstalled(VP_PackageInstallSetup.POST_PROCESSING_STACK_PACKAGE))
            {
                InstallPostProcessing();
                installHappened = true;
            }
#endif

#if USE_CINEMACHINE
            if (!VP_PackageInstallation.IsInstalled(VP_PackageInstallSetup.CINEMACHINE_PACKAGE))
            {
                InstallCinemachine();
                installHappened = true;
            }
#endif

            if (!VP_PackageInstallation.IsInstalled(VP_PackageInstallSetup.TEXT_MESH_PRO_PACKAGE))
            {
                InstallTextMeshPro();
                installHappened = true;
            }

#if USE_ADDRESSABLES

            if (!VP_PackageInstallation.IsInstalled(VP_PackageInstallSetup.ADDRESSABLES_PACKAGE))
            {
                InstallAddressables();
                installHappened = true;
            }
#endif

            if (installHappened)
            {
                AssetDatabase.Refresh();
                ReloadCurrentScene();
            }
        }
        protected static void ReloadCurrentScene()
        {
            string currentScenePath = EditorSceneManager.GetActiveScene().path;
            EditorSceneManager.OpenScene(currentScenePath);
        }

        protected static void InstallTilemapEditor()
        {
            if (!VP_PackageInstallation.IsInstalled(VP_PackageInstallSetup.TILE_MAP_EDITOR_PACKAGE))
            {
                VP_PackageInstallation.Install(VP_PackageInstallSetup.TILE_MAP_EDITOR_PACKAGE+VP_PackageInstallSetup.TILE_MAP_EDITOR_PACKAGE_VERSION);
            }
        }

        protected static void InstallPostProcessing()
        {
            if (!VP_PackageInstallation.IsInstalled(VP_PackageInstallSetup.POST_PROCESSING_STACK_PACKAGE))
            {
                VP_PackageInstallation.Install(VP_PackageInstallSetup.POST_PROCESSING_STACK_PACKAGE + VP_PackageInstallSetup.POST_PROCESSING_STACK_PACKAGE_VERSION);
            }
        }

        protected static void InstallCinemachine()
        {
            if (!VP_PackageInstallation.IsInstalled(VP_PackageInstallSetup.CINEMACHINE_PACKAGE))
            {
                VP_PackageInstallation.Install(VP_PackageInstallSetup.CINEMACHINE_PACKAGE+ VP_PackageInstallSetup.CINEMACHINE_PACKAGE_VERSION);
            }
        }

        protected static void InstallTextMeshPro()
        {
            if (!VP_PackageInstallation.IsInstalled(VP_PackageInstallSetup.TEXT_MESH_PRO_PACKAGE))
            {
                VP_PackageInstallation.Install(VP_PackageInstallSetup.TEXT_MESH_PRO_PACKAGE+VP_PackageInstallSetup.TEXT_MESH_PRO_PACKAGE_VERSION);
            }
        }

        protected static void InstallAddressables()
        {
            if (!VP_PackageInstallation.IsInstalled(VP_PackageInstallSetup.ADDRESSABLES_PACKAGE))
            {
                VP_PackageInstallation.Install(VP_PackageInstallSetup.ADDRESSABLES_PACKAGE+VP_PackageInstallSetup.ADDRESSABLES_PACKAGE_VERSION);
            }
        }
    }
    public class VP_PackageInstallation
    {
        public static bool IsInstalled(string packageID)
        {
            string packagesFolder = Application.dataPath + "/../Packages/";
            string manifestFile = packagesFolder + "manifest.json";
            string manifest = File.ReadAllText(manifestFile);
            return manifest.Contains(packageID);
        }
        public static void Install(string packageVersionID)
        {
            Client.Add(packageVersionID);
        }
    }
}