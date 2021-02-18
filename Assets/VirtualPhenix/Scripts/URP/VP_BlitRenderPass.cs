#if PHOENIX_URP_BLIT_PASS  && USE_FADE
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace VirtualPhenix.URP
{

    public class VP_BlitRenderPass : ScriptableRenderPass
    {
        static readonly string k_RenderTag = "VP_BlitURP";
        static readonly string m_TemporaryColorTexTag = "_TemporaryColorTexture"; 

        private RenderTargetIdentifier m_source;
        private RenderTargetHandle m_destination;
        private RenderTargetHandle m_temporaryColorTexture;

        private VP_CustomRPSettings m_customRPSettings;
        private bool m_customPass;

        public VP_BlitRenderPass(VP_CustomRPSettings _settings, bool _customPass)
        {
            m_customRPSettings = _settings;
            m_customPass = _customPass;

            renderPassEvent = m_customRPSettings.m_passEvent;

            if (m_customRPSettings.m_material == null)
            {
                VP_Debug.Log("No material. Trying to create material from shader");

                if (m_customRPSettings.m_shader == null)
                {
                    VP_Debug.Log("No Shader");
                    return;
                }

                m_customRPSettings.m_material = CoreUtils.CreateEngineMaterial(m_customRPSettings.m_shader);
            }


        }

        public void SetSettings(VP_CustomRPSettings _settings)
        {
            m_customRPSettings = _settings;

            renderPassEvent = m_customRPSettings.m_passEvent;

            if (m_customRPSettings.m_material == null)
            {
                VP_Debug.Log("No material. Trying to create material from shader");

                if (m_customRPSettings.m_shader == null)
                {
                    VP_Debug.Log("No Shader");
                    return;
                }

                m_customRPSettings.m_material = CoreUtils.CreateEngineMaterial(m_customRPSettings.m_shader);
            }
        }

        // This method is called before executing the render pass.
        // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
        // When empty this render pass will render to the active camera render target.
        // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
        // The render pipeline will ensure target setup and clearing happens in an performance manner.
        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            if (!m_customPass)
                return;

            m_temporaryColorTexture.Init(m_TemporaryColorTexTag);
        }

   

        /// Cleanup any allocated resources that were created during the execution of this render pass.
        public override void FrameCleanup(CommandBuffer cmd)
        {
            if (!m_customPass)
                return;

            if (m_destination == RenderTargetHandle.CameraTarget)
            {
                cmd.ReleaseTemporaryRT(m_temporaryColorTexture.id);
            }
        }

        // Here you can implement the rendering logic.
        // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
        // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
        // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (!m_customPass)
                return;

            CommandBuffer cmd = CommandBufferPool.Get(k_RenderTag);

            if (m_destination == RenderTargetHandle.CameraTarget)
            {
                cmd.GetTemporaryRT(m_temporaryColorTexture.id, renderingData.cameraData.cameraTargetDescriptor, FilterMode.Point);
                cmd.Blit(m_source, m_temporaryColorTexture.Identifier());
                cmd.Blit(m_temporaryColorTexture.Identifier(), m_source, m_customRPSettings.m_material);
            }
            else
            {
                cmd.Blit(m_source, m_destination.Identifier(), m_customRPSettings.m_material, 0);
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public void Setup(RenderTargetIdentifier source, RenderTargetHandle destination)
        {
            m_source = source;
            m_destination = destination;
        }       
    }
}
#endif