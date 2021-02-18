#if PHOENIX_URP_BLIT_PASS && USE_FADE
using VirtualPhenix.Fade;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace VirtualPhenix.URP
{
    public class VP_BlitRenderPassFeature : ScriptableRendererFeature
    {
        VP_BlitRenderPass m_ScriptablePass;

        public VP_CustomRPSettings m_settings;
        
        public bool m_customPass = true;

        public override void Create()
        {
            if (m_settings.m_material == null)
            {
                VP_FadePostProcess m_pp = FindObjectOfType<VP_FadePostProcess>();
                if (m_pp != null)
                {
                    m_settings.m_material = m_pp.CurrentEffect.BaseMaterial;
                }
            }

            m_ScriptablePass = new VP_BlitRenderPass(m_settings, m_customPass);

        }
        public void ChangedEffect(VP_FadeEffect effect)
        {
            m_settings.m_material = effect.BaseMaterial;
            m_ScriptablePass.SetSettings(m_settings);
        }

        // Here you can inject one or multiple render passes in the renderer.
        // This method is called when setting up the renderer once per-camera.
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            m_ScriptablePass.Setup(renderer.cameraColorTarget, RenderTargetHandle.CameraTarget);
            renderer.EnqueuePass(m_ScriptablePass);
        }

    }
}
#endif