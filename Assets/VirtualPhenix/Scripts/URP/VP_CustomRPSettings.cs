#if PHOENIX_URP_BLIT_PASS 
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace VirtualPhenix.URP
{
    [System.Serializable]
    public class VP_CustomRPSettings
    {
        public RenderPassEvent m_passEvent = RenderPassEvent.AfterRendering;
        public Material m_material;
        public Shader m_shader = null;

        public VP_CustomRPSettings()
        {

        }

        public VP_CustomRPSettings(RenderPassEvent _pass, Material _material, Shader _shader)
        {
            m_passEvent = _pass;
            m_material = _material;
            m_shader = _shader;
        }
    }
}
#endif