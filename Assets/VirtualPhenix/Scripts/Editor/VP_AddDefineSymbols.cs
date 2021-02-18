using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace VirtualPhenix
{
    /// <summary>
    /// Adds the given define symbols to PlayerSettings define symbols.
    /// Just add your own define symbols to the Symbols property at the below.
    /// </summary>
    [InitializeOnLoad]
    public partial class VP_AddDefineSymbols : Editor
    {


        /// <summary>
        /// Symbols that will be added to the editor
        /// </summary>
        public static string[] Symbols = new string[] {
         "USE_VIRTUAL_PHENIX_DIALOGUE_SYSTEM",
         "PHOENIX_ACTIONS",
         "PHOENIX_URP_BLIT_PASS",
         "USE_ADS", // 3
         "USE_INAPP_PURCHASES", // 4
         "USE_GAME_SERVICES", // 5
         "USE_NOTIFICATIONS", // 6
         "USE_APPLE_WATCH",
         "USE_ADDRESSABLES",
         "USE_MICROPHONE",
         "USE_TILEMAP",
         "USE_POSTPROCESSING_STACK",
         "USE_GRID_SYSTEM",
         "DOTWEEN",
         "USE_TEXT_ANIMATOR",
         "USE_CINEMACHINE",
         "USE_MORE_EFFECTIVE_COROUTINES",
         "ODIN_INSPECTOR",
         "USE_FADE",
         "USE_SCRIPTABLE_OBJECT_DATABASE",
         "USE_SERIALIZABLE_DICTIONARIES_LITE",
         "USE_INCONTROL",
         "USE_NICEVIBRATIONS",
         "USE_ULTIMATE_MOBILE_PRO",
         "USE_MONETIZATION",
         "USE_ANIMANCER",
         "USE_VRIF",
         "USE_FLOOK_ANIMATOR",
         "USE_BEHAVIOR_DESIGNER",
         "USE_ASTAR_PROJECT_PRO",
         "USE_VOLUMETRIC_BLOOD",
         "USE_STANDALONE_FILE_BROWSER",
         "USE_NEW_INPUT_SYSTEM",
         "USE_CUSTOM_PAINTER",
         "USE_CUSTOM_ASSET_BUNDLES_LOADER",
         "USE_PERMISSIONS"
        };

        // Default inactive-> need to activate them manually after reset
        public static List<string> DefaultAvoidSymbols = new List<string>()
        {
             "USE_ADS",
             "USE_INAPP_PURCHASES",
             "USE_GAME_SERVICES",
             "USE_NOTIFICATIONS",
             "USE_APPLE_WATCH",
             "USE_ADDRESSABLES",
             "DOTWEEN",
             "USE_TEXT_ANIMATOR",
             "USE_CINEMACHINE",
             "USE_MORE_EFFECTIVE_COROUTINES",
             "ODIN_INSPECTOR",
             "USE_SCRIPTABLE_OBJECT_DATABASE",
             "USE_SERIALIZABLE_DICTIONARIES_LITE",
             "USE_INCONTROL",
             "USE_NICEVIBRATIONS",
             "USE_ULTIMATE_MOBILE_PRO",
             "USE_MONETIZATION",
             "USE_ANIMANCER",
             "USE_VRIF",
             "USE_FLOOK_ANIMATOR",
             "USE_BEHAVIOR_DESIGNER",
             "USE_ASTAR_PROJECT_PRO",
             "USE_VOLUMETRIC_BLOOD",
             "USE_STANDALONE_FILE_BROWSER",
             "USE_MICROPHONE",
             "USE_TILEMAP",
             "USE_POSTPROCESSING_STACK",
             "USE_GRID_SYSTEM",
             "USE_NEW_INPUT_SYSTEM",
             "USE_CUSTOM_PAINTER",
             "USE_CUSTOM_ASSET_BUNDLES_LOADER",
             "USE_PERMISSIONS"
        };

        /// <summary>
        /// Add define symbols as soon as Unity gets done compiling.
        /// </summary>
        static VP_AddDefineSymbols()
        {

        }
#if USE_DEFINE_SYMBOL_SHORTCUT
		[MenuItem("Virtual Phenix/Define Symbols/Refresh Define Symbols")]
#endif
        public static void RefreshDefine()
        {
            RemoveDefine();
            AddDefine();
        }

        public static void RefreshDefine(VP_DefineSymbolsReferencer refe)
        {
            RemoveDefine();
            AddDefine(refe);
        }

#if USE_DEFINE_SYMBOL_SHORTCUT
		[MenuItem("Virtual Phenix/Define Symbols/Setup Define Symbols")]
#endif
        public static void AddDefine()
        {
            string path = GetSaveFilePath();

            if (!System.IO.File.Exists(path))
            {
                Debug.LogError("Please, configure the define symbol dictionary");
                EditorApplication.ExecuteMenuItem("Virtual Phenix/Window/Define Symbol Editor");
                return;
            }
            AddDefine(LoadMainDictionary(path));
        }


        protected static VP_DefineSymbolsReferencer LoadMainDictionary(string path)
        {
            VP_DefineSymbolsReferencer m_symbolDictionary = null;

            VP_Utils.GetObjectOfTypeInProject(out m_symbolDictionary, "VP_DefineSymbolsReferencer", (VP_DefineSymbolsReferencer refe) =>
            {
                return VP_Utils.CheckPathOfDefineSymbol(refe, path);
            });

            return m_symbolDictionary;
        }


        public static string GetSaveFilePath()
        {
            string path = System.IO.Directory.GetCurrentDirectory() + "\\Assets\\VirtualPhenix\\Resources\\Editor\\Define Symbols\\defineSymbolConfig.txt";
#if UNITY_EDITOR_OSX
			path = path.Replace("\\", "/");
#endif
            return path;
        }

        public static void AddDefine(VP_DefineSymbolsReferencer _referencer)
        {
            var AvoidSymbols = _referencer == null ? DefaultAvoidSymbols : _referencer.GetAvoidSymbols();
            string manifest = System.IO.Directory.GetCurrentDirectory() + "\\Packages\\manifest.json";
#if UNITY_EDITOR_OSX
            manifest = manifest.Replace("\\", "/");
#endif

            if (!System.IO.File.Exists(manifest))
                Debug.Log("Cant Find manifest at: " + manifest);

            string definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            List<string> allDefines = definesString.Split(';').ToList();

            foreach (string d in AvoidSymbols)
            {
                if (allDefines.Contains(d))
                {
                    allDefines.Remove(d);
                }
            }

            if (!AvoidSymbols.Contains(Symbols[0]))
            {
                Debug.Log("Trying to add #USE_VIRTUAL_PHENIX_DIALOGUE_SYSTEM symbol.");

                if (!allDefines.Contains(Symbols[0]))
                    allDefines.Add(Symbols[0]);
            }

            if (!AvoidSymbols.Contains(Symbols[1]))
            {
                // Phoenix Actions
                string actnPath = System.IO.Directory.GetCurrentDirectory() + "\\Assets\\VirtualPhenix\\Actions";

#if UNITY_EDITOR_OSX
            actnPath = actnPath.Replace("\\", "/");
#endif

                if (System.IO.Directory.Exists(actnPath))
                {
                    Debug.Log("Virtual Phenix Actions is in the project. Adding #PHOENIX_ACTIONS symbol.");
                    if (!allDefines.Contains(Symbols[1]))
                        allDefines.Add(Symbols[1]);
                }
                else
                {
                    Debug.Log($"Virtual Phenix Actions could not be found in {actnPath}. if you have it in other path, please, change the path here or add the dependency.");
                }
            }

            if (!AvoidSymbols.Contains(Symbols[2]))
            {
                if (VP_Utils.RenderPipelineUtils.IsUniversalRenderPipeline())
                {
                    Debug.Log("URP is in the project. Adding #PHOENIX_URP_BLIT_PASS symbol.");
                    if (!allDefines.Contains(Symbols[2]))
                        allDefines.Add(Symbols[2]);
                }
                else
                {
                    Debug.Log($"URP is not in the project.");
                }
            }

        
            if (!AvoidSymbols.Contains(Symbols[3]))
            {
                Debug.Log("Trying to add #USE_ADS symbol.");
                if (!allDefines.Contains(Symbols[3]))
                    allDefines.Add(Symbols[3]);
            }

            if (!AvoidSymbols.Contains(Symbols[4]))
            {
                Debug.Log("Trying to add #USE_INAPP_PURCHASES symbol.");
                if (!allDefines.Contains(Symbols[4]))
                    allDefines.Add(Symbols[4]);
            }

            // Add Game Services
            if (!AvoidSymbols.Contains(Symbols[5]))
            {
                Debug.Log("Trying to add  #USE_GAME_SERVICES symbol.");
                if (!allDefines.Contains(Symbols[5]))
                    allDefines.Add(Symbols[5]);
            }
            // Notifications
            if (!AvoidSymbols.Contains(Symbols[6]))
            {
                Debug.Log("Trying to add #USE_NOTIFICATIONS symbol.");
                if (!allDefines.Contains(Symbols[6]))
                    allDefines.Add(Symbols[6]);
            }
#if UNITY_IOS
            //Apple Watch
            if (!AvoidSymbols.Contains(Symbols[7]))
            {
                Debug.Log("Trying to add #USE_APPLE_WATCH symbol.");
                if (!allDefines.Contains(Symbols[7]))
                    allDefines.Add(Symbols[7]);
            }
#endif
            
            string manifestJSONData = System.IO.File.Exists(manifest) ? System.IO.File.ReadAllText(manifest) : "";

            // Addressables
            if (!AvoidSymbols.Contains(Symbols[8]))
            {
#if UNITY_EDITOR_OSX
            manifest = manifest.Replace("\\", "/");
#endif
                if (System.IO.File.Exists(manifest))
                {
      
                    if (manifestJSONData.Contains(VP_PackageInstallSetup.ADDRESSABLES_PACKAGE))
                    {
                        if (!allDefines.Contains(Symbols[8]))
                            allDefines.Add(Symbols[8]);
                    }
                }
            }

            // Microphone
            if (!AvoidSymbols.Contains(Symbols[9]))
            {
                Debug.Log("Trying to add #USE_MICROPHONE symbol.");
                if (!allDefines.Contains(Symbols[9]))
                    allDefines.Add(Symbols[9]);
            }

            // Tilemap
            if (!AvoidSymbols.Contains(Symbols[10]) && manifestJSONData.Contains(VP_PackageInstallSetup.TILE_MAP_EDITOR_PACKAGE))
            {
                Debug.Log("Trying to add #USE_TILEMAP symbol.");
                if (!allDefines.Contains(Symbols[10]))
                    allDefines.Add(Symbols[10]);
            }

            // Legacy PostProcessing Stack
            if (!AvoidSymbols.Contains(Symbols[11]) && manifestJSONData.Contains(VP_PackageInstallSetup.POST_PROCESSING_STACK_PACKAGE))
            {
                Debug.Log("Trying to add #USE_POSTPROCESSING_STACK symbol.");
                if (!allDefines.Contains(Symbols[11]))
                    allDefines.Add(Symbols[11]);
            }

            // Virtual Phenix Grid System
            if (!AvoidSymbols.Contains(Symbols[12]))
            {
                Debug.Log("Trying to add #USE_GRID_SYSTEM symbol.");

                if (!allDefines.Contains(Symbols[12]))
                    allDefines.Add(Symbols[12]);
            }

            // EXTERNAL
            if (!AvoidSymbols.Contains(Symbols[13]))
            {
                // Do tween pro
                string dotweenPath = System.IO.Directory.GetCurrentDirectory() + "\\Assets\\Demigiant\\DOTweenPro";
#if UNITY_EDITOR_OSX
            dotweenPath = dotweenPath.Replace("\\", "/");
#endif
                if (System.IO.Directory.Exists(dotweenPath))
                {
                    Debug.Log("Do Tween Pro is in the project. Adding #DOTWEEN symbol.");
                    if (!allDefines.Contains(Symbols[13]))
                        allDefines.Add(Symbols[13]);
                }
                else
                {
                    Debug.Log($"Do Tween Pro could not be found in {dotweenPath}. Trying in secondary Path.");
                    dotweenPath = System.IO.Directory.GetCurrentDirectory() + "\\Assets\\Plugins\\Demigiant\\DOTweenPro";
#if UNITY_EDITOR_OSX
                dotweenPath = dotweenPath.Replace("\\", "/");
#endif
                    if (System.IO.Directory.Exists(dotweenPath))
                    {
                        Debug.Log("Do Tween Pro is in the project. Adding #DOTWEEN symbol.");
                        if (!allDefines.Contains(Symbols[13]) && !AvoidSymbols.Contains(Symbols[13]))
                            allDefines.Add(Symbols[13]);
                    }
                    else
                    {
                        Debug.Log($"Do Tween Pro could not be found in {dotweenPath}. if you have it in other path, please, change the path here or add the dependency.");
                    }

                }
            }

            if (!AvoidSymbols.Contains(Symbols[14]))
            {
                // Text Animator
                string textanimatorpath = System.IO.Directory.GetCurrentDirectory() + "\\Assets\\Plugins\\Febucci\\Text Animator";
#if UNITY_EDITOR_OSX
            textanimatorpath = textanimatorpath.Replace("\\", "/");
#endif
                if (System.IO.Directory.Exists(textanimatorpath))
                {
                    Debug.Log("Text Animator is in the project. Adding #USE_TEXT_ANIMATOR symbol.");
                    if (!allDefines.Contains(Symbols[14]))
                        allDefines.Add(Symbols[14]);
                }
                else
                {
                    Debug.Log($"Text Animator could not be found in {textanimatorpath}. if you have it in other path, please, change the path here or add the dependency manually.");
                    textanimatorpath = System.IO.Directory.GetCurrentDirectory() + "\\Assets\\Febucci\\Text Animator";
#if UNITY_EDITOR_OSX
                textanimatorpath = textanimatorpath.Replace("\\", "/");
#endif
                    if (System.IO.Directory.Exists(textanimatorpath))
                    {
                        Debug.Log("Text Animator is in the project. Adding #USE_TEXT_ANIMATOR symbol.");
                        if (!allDefines.Contains(Symbols[14]) && !AvoidSymbols.Contains(Symbols[14]))
                            allDefines.Add(Symbols[14]);
                    }
                    else
                    {
                        Debug.Log($"Text Animator could not be found in {textanimatorpath}. if you have it in other path, please, change the path here.");
                    }
                }
            }


            if (!AvoidSymbols.Contains(Symbols[15]))
            {
                // Cinemachine

#if UNITY_EDITOR_OSX
            manifest = manifest.Replace("\\", "/");
#endif
                if (System.IO.File.Exists(manifest))
                {
                    if (manifestJSONData.Contains(VP_PackageInstallSetup.CINEMACHINE_PACKAGE))
                    {
                        if (!allDefines.Contains(Symbols[15]))
                            allDefines.Add(Symbols[15]);
                    }
                }
            }

            if (!AvoidSymbols.Contains(Symbols[16]))
            {
                // More effective coroutines
                string mecPath = System.IO.Directory.GetCurrentDirectory() + "\\Assets\\Plugins\\Trinary Software";
#if UNITY_EDITOR_OSX
            mecPath = mecPath.Replace("\\", "/");
#endif
                if (System.IO.Directory.Exists(mecPath))
                {
                    Debug.Log("More effective coroutines is in the project. Adding #USE_MORE_EFFECTIVE_COROUTINES symbol.");
                    if (!allDefines.Contains(Symbols[16]))
                        allDefines.Add(Symbols[16]);
                }
                else
                {
                    Debug.Log($"More effective coroutines could not be found in {mecPath}. if you have it in other path, please, change the path here or add the dependency.");
                }

            }

            if (!AvoidSymbols.Contains(Symbols[17]))
            {
                // Odin Inspector
                string odinPath = System.IO.Directory.GetCurrentDirectory() + "\\Assets\\Plugins\\Sirenix\\Odin Inspector";
#if UNITY_EDITOR_OSX
            odinPath = odinPath.Replace("\\", "/");
#endif
                if (System.IO.Directory.Exists(odinPath))
                {
                    Debug.Log("Odin Inspector is in the project. Adding #ODIN_INSPECTOR symbol.");
                    if (!allDefines.Contains(Symbols[17]))
                        allDefines.Add(Symbols[17]);
                }
                else
                {
                    Debug.Log($"Odin Inspector could not be found in {odinPath}. if you have it in other path, please, change the path here or add the dependency.");
                }

            }

            if (!AvoidSymbols.Contains(Symbols[18]))
            {
                // Fade Effect
                string fadePath = System.IO.Directory.GetCurrentDirectory() + "\\Assets\\VirtualPhenix\\Scripts\\Fade";
#if UNITY_EDITOR_OSX
            fadePath = fadePath.Replace("\\", "/");
#endif
                if (System.IO.Directory.Exists(fadePath))
                {
                    Debug.Log("Virtual Phenix native Fade is in the project. Adding #USE_FADE symbol. If you have a custom Fade with this folder, please delete this pragma.");
                    if (!allDefines.Contains(Symbols[18]))
                        allDefines.Add(Symbols[18]);
                }
                else
                {
                    // Fade Effect
                    fadePath = System.IO.Directory.GetCurrentDirectory() + "\\Assets\\Fade";
#if UNITY_EDITOR_OSX
                fadePath = fadePath.Replace("\\", "/");
#endif
                    if (System.IO.Directory.Exists(fadePath))
                    {
                        Debug.Log("Fade is in the project. Adding #USE_FADE symbol. If you have a custom Fade with this folder, please delete this pragma.");
                        if (!allDefines.Contains(Symbols[18]) && !AvoidSymbols.Contains(Symbols[18]))
                            allDefines.Add(Symbols[18]);
                    }
                    else
                    {
                        Debug.Log($"Fade Effect could not be found in {fadePath}. if you have it in other path, please, change the path here or add the dependency.");
                    }
                }
            }

            if (!AvoidSymbols.Contains(Symbols[19]))
            {
                Debug.Log("Checking " + Symbols[19]);
                // Scriptable Object Database
                string sodatabasePath = System.IO.Directory.GetCurrentDirectory() + "\\Assets\\Plugins\\ScriptableObjectDatabase";
#if UNITY_EDITOR_OSX
            sodatabasePath = sodatabasePath.Replace("\\", "/");
#endif
                if (System.IO.Directory.Exists(sodatabasePath))
                {
                    Debug.Log("Scriptable object database  is in the project. Adding #USE_SCRIPTABLE_OBJECT_DATABASE symbol. If you have a custom ScriptableObjectDatabase with this folder, please delete this pragma.");
                    if (!allDefines.Contains(Symbols[19]))
                        allDefines.Add(Symbols[19]);
                }
                else
                {
                    Debug.Log($"Scriptable object database could not be found in {sodatabasePath}. if you have it in other path, please, change the path here or add the dependency.");
                }

            }

            if (!AvoidSymbols.Contains(Symbols[20]))
            {
                // Scriptable Object Database
                string serdicePath = System.IO.Directory.GetCurrentDirectory() + "\\Assets\\VirtualPhenix\\Third Party\\Rotary Heart\\SerializableDictionaryLite";
#if UNITY_EDITOR_OSX
            serdicePath = serdicePath.Replace("\\", "/");
#endif
                if (System.IO.Directory.Exists(serdicePath))
                {
                    Debug.Log("Serializable Dictionaries lite is in the project. Adding #USE_SERIALIZABLE_DICTIONARIES_LITE symbol. If you have a custom Serializable Dictionaries lite with this folder, please delete this pragma.");
                    if (!allDefines.Contains(Symbols[20]))
                        allDefines.Add(Symbols[20]);
                }
                else
                {
                    Debug.Log($"Serializable Dictionaries lite could not be found in {serdicePath}. Will try second path.");

                    serdicePath = System.IO.Directory.GetCurrentDirectory() + "\\Assets\\Rotary Heart\\SerializableDictionaryLite";
#if UNITY_EDITOR_OSX
                serdicePath = serdicePath.Replace("\\", "/");
#endif
                    if (System.IO.Directory.Exists(serdicePath))
                    {
                        Debug.Log("Serializable Dictionaries lite is in the project. Adding #USE_SERIALIZABLE_DICTIONARIES_LITE symbol. If you have a custom Serializable Dictionaries lite with this folder, please delete this pragma.");
                        if (!allDefines.Contains(Symbols[20]) && !AvoidSymbols.Contains(Symbols[20]))
                            allDefines.Add(Symbols[20]);
                    }
                    else
                    {
                        Debug.Log($"Serializable Dictionaries lite could not be found in {serdicePath}. if you have it in other path, please, change the path here or add the dependency.");
                    }

                }
            }

            if (!AvoidSymbols.Contains(Symbols[21]))
            {
                // InControl
                string inControlPath = System.IO.Directory.GetCurrentDirectory() + "\\Assets\\InControl";
#if UNITY_EDITOR_OSX
            inControlPath = inControlPath.Replace("\\", "/");
#endif
                if (System.IO.Directory.Exists(inControlPath))
                {
                    Debug.Log("InControl is in the project. Adding #USE_INCONTROL symbol. If you have a custom InControl with this folder, please delete this pragma.");
                    if (!allDefines.Contains(Symbols[21]))
                        allDefines.Add(Symbols[21]);
                }
                else
                {
                    Debug.Log($"InControl could not be found in {inControlPath}. if you have it in other path, please, change the path here or add the dependency.");
                }
            }

            if (!AvoidSymbols.Contains(Symbols[22]))
            {
                // Nice Vibrations
                string niceVibPath = System.IO.Directory.GetCurrentDirectory() + "\\Assets\\NiceVibrations";
#if UNITY_EDITOR_OSX
            niceVibPath = niceVibPath.Replace("\\", "/");
#endif
                if (System.IO.Directory.Exists(niceVibPath))
                {
                    Debug.Log("Nice Vibrations is in the project. Adding #USE_NICEVIBRATIONS symbol. If you have a custom Nice Vibrations with this folder, please delete this pragma.");
                    if (!allDefines.Contains(Symbols[22]))
                        allDefines.Add(Symbols[22]);
                }
                else
                {
                    Debug.Log($"Nice Vibrations could not be found in {niceVibPath}. if you have it in other path, please, change the path here or add the dependency.");
                }
            }

            if (!AvoidSymbols.Contains(Symbols[23]))
            {
                // Ultimate Mobile Pro
                string ultimateMobileProPath = System.IO.Directory.GetCurrentDirectory() + "\\Assets\\Plugins\\StansAssets";
#if UNITY_EDITOR_OSX
                ultimateMobileProPath = ultimateMobileProPath.Replace("\\", "/");
#endif
                if (System.IO.Directory.Exists(ultimateMobileProPath))
                {
                    Debug.Log("Ultimate Mobile Pro is in the project. Adding #USE_ULTIMATE_MOBILE_PRO symbol. If you have a custom Ultimate Mobile Pro with this folder, please delete this pragma.");
                    if (!allDefines.Contains(Symbols[23]))
                        allDefines.Add(Symbols[23]);
                }
                else
                {
                    Debug.Log($"Ultimate Mobile Pro could not be found in {ultimateMobileProPath}. if you have it in other path, please, change the path here or add the dependency.");
                }
            }

            if (!AvoidSymbols.Contains(Symbols[24]))
            {
                // Monetization (Analytics)
                if (System.IO.File.Exists(manifest))
                {
                    if (manifestJSONData.Contains(VP_PackageInstallSetup.ADS_PACKAGE))
                    {
                        string adsPath = System.IO.Directory.GetCurrentDirectory() + "\\Assets\\Plugins\\UnityPurchasing";
#if UNITY_EDITOR_OSX
                    adsPath = adsPath.Replace("\\", "/");
#endif
                        if (System.IO.Directory.Exists(adsPath))
                        {
                            Debug.Log("Analytics is in the project. Adding #USE_MONETIZATION symbol. If you don't want to use Unity Ads with this folder, please delete this pragma.");

                            if (!allDefines.Contains(Symbols[24]))
                                allDefines.Add(Symbols[24]);
                        }
                        else
                        {
                            Debug.Log($"Analytics could not be found in the project. if you have it in other path, please, change the path here or add the dependency.");
                        }
                    }
                    else
                    {
                        Debug.Log($"Unity Ads could not be found in the project. if you have it in other path, please, change the path here or add the dependency.");
                    }
                }
            }

            if (!AvoidSymbols.Contains(Symbols[25]))
            {
                // Animancer
                string animancerPath = System.IO.Directory.GetCurrentDirectory() + "\\Assets\\Plugins\\Animancer";
#if UNITY_EDITOR_OSX
            animancerPath = animancerPath.Replace("\\", "/");
#endif
                if (System.IO.Directory.Exists(animancerPath))
                {
                    Debug.Log("Animancer is in the project. Adding #USE_ANIMANCER symbol. If you don't want to use Animancer with this folder, please delete this pragma.");
                    if (!allDefines.Contains(Symbols[25]))
                        allDefines.Add(Symbols[25]);
                }
                else
                {
                    Debug.Log($"Animancer could not be found in {animancerPath}. if you have it in other path, please, change the path here or add the dependency.");
                }
            }

            if (!AvoidSymbols.Contains(Symbols[26]))
            {
                // VR Interaction Framework
                string vrifInstancerPath = System.IO.Directory.GetCurrentDirectory() + "\\Assets\\BNG Framework";
#if UNITY_EDITOR_OSX
            vrifInstancerPath = vrifInstancerPath.Replace("\\", "/");
#endif
                if (System.IO.Directory.Exists(vrifInstancerPath))
                {
                    Debug.Log("VR Interaction Framework is in the project. Adding #USE_VRIF symbol. If you don't want to use custom GPU Instancer with this folder, please delete this pragma.");
                    if (!allDefines.Contains(Symbols[26]))
                        allDefines.Add(Symbols[26]);
                }
                else
                {
                    Debug.Log($"VR Interaction Framework could not be found in {vrifInstancerPath}. if you have it in other path, please, change the path here or add the dependency.");
                }

            }

            if (!AvoidSymbols.Contains(Symbols[27]))
            {
                // FLook Animator
                string flookPath = System.IO.Directory.GetCurrentDirectory() + "\\Assets\\FImpossible Creations\\Look Animator";
#if UNITY_EDITOR_OSX
            flookPath = flookPath.Replace("\\", "/");
#endif
                if (System.IO.Directory.Exists(flookPath))
                {
                    Debug.Log("FLook Animator is in the project. Adding #USE_FLOOK_ANIMATOR symbol. If you don't want to use customFLook Animator with this folder, please delete this pragma.");
                    if (!allDefines.Contains(Symbols[27]))
                        allDefines.Add(Symbols[27]);
                }
                else
                {
                    flookPath = System.IO.Directory.GetCurrentDirectory() + "\\Assets\\FImpossible Creations\\Plugins\\Look Animator";
#if UNITY_EDITOR_OSX
            flookPath = flookPath.Replace("\\", "/");
#endif
                    if (System.IO.Directory.Exists(flookPath))
                    {
                        Debug.Log("FLook Animator is in the project. Adding #USE_FLOOK_ANIMATOR symbol. If you don't want to use customFLook Animator with this folder, please delete this pragma.");
                        if (!allDefines.Contains(Symbols[27]) && !AvoidSymbols.Contains(Symbols[27]))
                            allDefines.Add(Symbols[27]);
                    }
                    else
                    {
                        Debug.Log($"FLook Animator could not be found in {flookPath}. if you have it in other path, please, change the path here or add the dependency.");
                    }
                }
            }

            if (!AvoidSymbols.Contains(Symbols[28]))
            {
                // Behavior Designer
                string bdPath = System.IO.Directory.GetCurrentDirectory() + "\\Assets\\Behavior Designer";
#if UNITY_EDITOR_OSX
            bdPath = bdPath.Replace("\\", "/");
#endif
                if (System.IO.Directory.Exists(bdPath))
                {
                    Debug.Log("Behavior Designer is in the project. Adding #USE_BEHAVIOR_DESIGNER symbol. If you don't want to use Behavior Designer with this folder, please delete this pragma.");
                    if (!allDefines.Contains(Symbols[28]))
                        allDefines.Add(Symbols[28]);
                }
                else
                {
                    Debug.Log($"Behavior Designer could not be found in {bdPath}. if you have it in other path, please, change the path here or add the dependency.");
                }
            }

            if (!AvoidSymbols.Contains(Symbols[29]))
            {
                // AStar Pathfinding Project Pro
                string astarPath = System.IO.Directory.GetCurrentDirectory() + "\\Assets\\AstarPathfindingProject";
#if UNITY_EDITOR_OSX
            astarPath = astarPath.Replace("\\", "/");
#endif
                if (System.IO.Directory.Exists(astarPath))
                {
                    Debug.Log("AStar Pathfinding Project Pro is in the project. Adding #USE_ASTAR_PROJECT_PRO symbol. If you don't want to use AStar Pathfinding Project Pro with this folder, please delete this pragma.");
                    if (!allDefines.Contains(Symbols[29]))
                        allDefines.Add(Symbols[29]);
                }
                else
                {
                    Debug.Log($"AStar Pathfinding Project Pro could not be found in {astarPath}. if you have it in other path, please, change the path here or add the dependency.");
                }
            }

            if (!AvoidSymbols.Contains(Symbols[30]))
            {
                // Volumetric Blood
                string volumetricBloodPath = System.IO.Directory.GetCurrentDirectory() + "\\Assets\\KriptoFX\\VolumetricBloodFX";
#if UNITY_EDITOR_OSX
            volumetricBloodPath = volumetricBloodPath.Replace("\\", "/");
#endif
                if (System.IO.Directory.Exists(volumetricBloodPath))
                {
                    Debug.Log("Volumetric Blood is in the project. Adding #USE_VOLUMETRIC_BLOOD symbol. If you don't want to use Volumetric Blood FX with this folder, please delete this pragma.");
                    if (!allDefines.Contains(Symbols[30]))
                        allDefines.Add(Symbols[30]);
                }
                else
                {
                    Debug.Log($"Volumetric Blood FX could not be found in {volumetricBloodPath}. if you have it in other path, please, change the path here or add the dependency.");
                }
            }

            if (!AvoidSymbols.Contains(Symbols[31]))
            {
                // Standalone File Browser
                string sfbPath = System.IO.Directory.GetCurrentDirectory() + "\\Assets\\StandaloneFileBrowser";
#if UNITY_EDITOR_OSX
	        sfbPath = sfbPath.Replace("\\", "/");
#endif
                if (System.IO.Directory.Exists(sfbPath))
                {
                    Debug.Log("Use Standalone File Browser is in the project. Adding #USE_STANDALONE_FILE_BROWSER symbol. If you don't want to use Volumetric Blood FX with this folder, please delete this pragma.");
                    if (!allDefines.Contains(Symbols[31]))
                        allDefines.Add(Symbols[31]);
                }
                else
                {
                    sfbPath = System.IO.Directory.GetCurrentDirectory() + "\\Assets\\VirtualPhenix\\Third Party\\StandaloneFileBrowser";
#if UNITY_EDITOR_OSX
		        sfbPath = sfbPath.Replace("\\", "/");
#endif
                    if (System.IO.Directory.Exists(sfbPath))
                    {
                        Debug.Log("Use Standalone File Browser is in the project. Adding #USE_STANDALONE_FILE_BROWSER symbol. If you don't want to use Volumetric Blood FX with this folder, please delete this pragma.");
                        if (!allDefines.Contains(Symbols[31]) && !AvoidSymbols.Contains(Symbols[30]))
                            allDefines.Add(Symbols[31]);
                    }
                    else
                    {
                        Debug.Log($"Use Standalone File Browser could not be found in {sfbPath}. if you have it in other path, please, change the path here or add the dependency.");
                    }

                }
            }

            // New Input System
            if (!AvoidSymbols.Contains(Symbols[32]) && manifestJSONData.Contains(VP_PackageInstallSetup.NEW_INPUT_SYSTEM_PACKAGE))
            {
                Debug.Log("Tryint to add #USE_NEW_INPUT_SYSTEM symbol.");

                if (!allDefines.Contains(Symbols[32]))
                    allDefines.Add(Symbols[32]);
            }
             
            // VP Canvas Painter
            if (!AvoidSymbols.Contains(Symbols[33]))
            {
                Debug.Log("Tryint to add #USE_CUSTOM_PAINTER symbol.");

                if (!allDefines.Contains(Symbols[33]))
                    allDefines.Add(Symbols[33]);
            }
               
            // VP Asset Bundle Loader
            if (!AvoidSymbols.Contains(Symbols[34]))
            {
                Debug.Log("Tryint to add #USE_CUSTOM_ASSET_BUNDLE_LOADER symbol.");

                if (!allDefines.Contains(Symbols[34]))
                    allDefines.Add(Symbols[34]);
            }  
            
            // If you need permissions requests for Microphpne
            if (!AvoidSymbols.Contains(Symbols[35]))
            {
                Debug.Log("Tryint to add #USE_PERMISSIONS symbol.");

                if (!allDefines.Contains(Symbols[35]))
                    allDefines.Add(Symbols[35]);
            }


            PlayerSettings.SetScriptingDefineSymbolsForGroup(
                EditorUserBuildSettings.selectedBuildTargetGroup,
                string.Join(";", allDefines.ToArray()));
        }
#if USE_DEFINE_SYMBOL_SHORTCUT
		[MenuItem("Virtual Phenix/Define Symbols/Remove Define Symbols")]
#endif 
        public static void RemoveDefine()
        {
            string definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            List<string> allDefines = definesString.Split(';').ToList();

            foreach (string str in Symbols)
                allDefines.Remove(str);

            PlayerSettings.SetScriptingDefineSymbolsForGroup(
            EditorUserBuildSettings.selectedBuildTargetGroup,
            string.Join(";", allDefines.ToArray()));
        }
    }
}

