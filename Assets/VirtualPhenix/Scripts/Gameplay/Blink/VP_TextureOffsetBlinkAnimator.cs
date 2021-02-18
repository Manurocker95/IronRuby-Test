using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Gameplay
{
    public class VP_TextureOffsetBlinkAnimator : VP_MaterialBlinkAnimator<Vector2>
    {
        protected override void Reset()
        {
            base.Reset();
			m_openEyeOffset = Vector2.zero;
			m_closeEyeOffset = new Vector2(0, 0.5f);
		}

	    protected override void TextureSetup(Vector2 eyeOffset)
	    {
		    // All Renderers
		    if (m_renderIndex.Length <= 0 || m_renderIndex[0] < 0)
		    {
			    // All materials per renderer
			    if (m_materialIndex.Length <= 0 || m_materialIndex[0] < 0)
			    {
				    // All IDs in material-> Be sure
				    if (m_idIndex.Length <= 0 || m_idIndex[0] < 0)
				    {
					    for (int i = 0; i < m_renderers.Length; i++)
					    {
						    var renderer = m_renderers[i];

						    for (int j = 0; j < renderer.materials.Length; j++)
						    {
							    for (int k = 0; k < m_eyeTextureIDs.Length; k++)
							    {
								    var eyeID = m_eyeTextureIDs[k];
								    renderer.materials[j].SetTextureOffset(eyeID, eyeOffset);
							    }
						    }
					    }
				    }
				    else
				    {
					    for (int i = 0; i < m_renderers.Length; i++)
					    {
						    var renderer = m_renderers[i];

						    for (int j = 0; j < renderer.materials.Length; j++)
						    {
							    for (int k = 0; k < m_idIndex.Length; k++)
							    {
								    var eyeID = m_eyeTextureIDs[m_idIndex[k]];
								    renderer.materials[j].SetTextureOffset(eyeID, eyeOffset);
							    }
		
						    }
					    }
				    }
			    }
			    else
			    {
				    // All IDs in material-> Be sure
				    if (m_idIndex.Length <= 0 || m_idIndex[0] < 0)
				    {
					    for (int i = 0; i < m_renderers.Length; i++)
					    {
						    var renderer = m_renderers[i];
							
						    for (int l = 0; l < m_materialIndex.Length; l++)
						    {
							    var material = renderer.materials[m_materialIndex[l]];
							
							    for (int k = 0; k < m_eyeTextureIDs.Length; k++)
							    {
								    var eyeID = m_eyeTextureIDs[k];
								    material.SetTextureOffset(eyeID, eyeOffset);
							    }
						    }
							
						
					    }
				    }
				    else
				    {
					    for (int i = 0; i < m_renderers.Length; i++)
					    {
						    var renderer = m_renderers[i];
							
						    for (int l = 0; l < m_materialIndex.Length; l++)
						    {
							    var material = renderer.materials[m_materialIndex[l]];
							    for (int k = 0; k < m_idIndex.Length; k++)
							    {
								    var eyeID = m_eyeTextureIDs[m_idIndex[k]];
								    material.SetTextureOffset(eyeID, eyeOffset);
							    }
						    }
							
					    }
				    }
			    }
		    }
		    else
		    {
			    // All IDs in material-> Be sure
			    if (m_idIndex.Length <= 0 || m_idIndex[0] < 0)
			    {
				    for (int m = 0; m < m_renderIndex.Length; m++)
				    {
					    var renderer = m_renderers[m_renderIndex[m]];
					    for (int l = 0; l < m_materialIndex.Length; l++)
					    {
						    var material = renderer.materials[m_materialIndex[l]];
						    for (int k = 0; k < m_eyeTextureIDs.Length; k++)
						    {
							    var eyeID = m_eyeTextureIDs[k];
							    material.SetTextureOffset(eyeID, eyeOffset);
						    }
					    }
				    }
					
					
			    }
			    else
			    {
				    for (int m = 0; m < m_renderIndex.Length; m++)
				    {
					    var renderer = m_renderers[m_renderIndex[m]];
					    for (int l = 0; l < m_materialIndex.Length; l++)
					    {
						    var material = renderer.materials[m_materialIndex[l]];
						    for (int k = 0; k < m_idIndex.Length; k++)
						    {
							    var eyeID = m_eyeTextureIDs[m_idIndex[k]];
							    material.SetTextureOffset(eyeID, eyeOffset);
						    }
					    }
				    }
			    }
		    }
	    }
	}

}