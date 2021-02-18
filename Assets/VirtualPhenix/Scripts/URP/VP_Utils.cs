using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VirtualPhenix.Fade;

namespace VirtualPhenix
{
    public static partial class VP_Utils
    {
        public static class RenderPipelineUtils
        {
            public enum PipelineType
            {
                Unsupported,
                BuiltInPipeline,
                LightweightPipeline,
                UniversalPipeline,
                HDPipeline
            }

            public static bool IsUnsupportedRenderPipeline()
            {
                return DetectPipeline() == PipelineType.Unsupported;
            }

            public static bool IsLegacyRenderPipeline()
            {
                return DetectPipeline() == PipelineType.BuiltInPipeline;
            }

            public static bool IsHighDefinitionRenderPipeline()
            {
                return DetectPipeline() == PipelineType.HDPipeline;
            }

            public static bool IsUniversalRenderPipeline()
            {
                return DetectPipeline() == PipelineType.UniversalPipeline;
            }

            public static bool IsLightweightlRenderPipeline()
            {
                return DetectPipeline() == PipelineType.LightweightPipeline;
            }

            /// <summary>
            /// Returns the type of renderpipeline that is currently running
            /// </summary>
            /// <returns></returns>
            public static PipelineType DetectPipeline()
            {
                if (UnityEngine.Rendering.GraphicsSettings.renderPipelineAsset != null)
                {
                    // SRP
                    var srpType = UnityEngine.Rendering.GraphicsSettings.renderPipelineAsset.GetType().ToString();
                    if (srpType.Contains("HDRenderPipelineAsset"))
                    {
                        return PipelineType.HDPipeline;
                    }
                    else if (srpType.Contains("UniversalRenderPipelineAsset"))
                    {
                        return PipelineType.UniversalPipeline;
                    }
                    else if (srpType.Contains("LightweightRenderPipelineAsset"))
                    {
                        return PipelineType.LightweightPipeline;
                    }
                    else
                    {
                        return PipelineType.Unsupported;
                    }
                }

                // no SRP
                return PipelineType.BuiltInPipeline;
            }

#if PHOENIX_URP_BLIT_PASS && USE_FADE

            public static UnityEngine.Rendering.Universal.ScriptableRendererData ExtractScriptableRendererData()
            {
                var pipeline = ((UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset)UnityEngine.Rendering.GraphicsSettings.renderPipelineAsset);
                System.Reflection.FieldInfo propertyInfo = pipeline.GetType().GetField("m_RendererDataList", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                return ((UnityEngine.Rendering.Universal.ScriptableRendererData[])propertyInfo?.GetValue(pipeline))?[0];
            }


            public static void SetFadeEffectToURPPass(VP_FadeEffect _currentEffect)
            {
                UnityEngine.Rendering.Universal.ScriptableRendererData _scriptableRendererData = ExtractScriptableRendererData();

                if (!_scriptableRendererData)
                {
                    ExtractScriptableRendererData();
                }

                foreach (var renderObjSetting in _scriptableRendererData.rendererFeatures.OfType<URP.VP_BlitRenderPassFeature>())
                {
                    renderObjSetting.ChangedEffect(_currentEffect);
                }
            }

            public static void SetFadeEffectToURPPass(VP_FadeEffect _currentEffect, float _alpha)
            {
                UnityEngine.Rendering.Universal.ScriptableRendererData _scriptableRendererData = ExtractScriptableRendererData();

                if (!_scriptableRendererData)
                {
                    ExtractScriptableRendererData();
                }

                foreach (var renderObjSetting in _scriptableRendererData.rendererFeatures.OfType<URP.VP_BlitRenderPassFeature>())
                {
                    renderObjSetting.ChangedEffect(_currentEffect);
                }
            }
#endif
        }
    }
}
