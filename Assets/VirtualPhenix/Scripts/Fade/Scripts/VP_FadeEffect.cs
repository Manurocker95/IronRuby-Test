using UnityEngine;

namespace VirtualPhenix.Fade 
{
	[CreateAssetMenu(fileName = "DefaultAudioDatabase", menuName = "Virtual Phenix/Scriptable Objects/Effect/Fade", order = 1)]
	public class VP_FadeEffect : VP_ScriptableObject 
	{
		//-----------------------------------------------------------------------------------------
		// Inspector Variables:
		//-----------------------------------------------------------------------------------------
		[SerializeField] public Color m_defaultColor = Color.black;

		[SerializeField] protected string m_effectName;

		[SerializeField] protected Shader m_shader;

		[SerializeField] protected Material m_baseMaterial;

		//-----------------------------------------------------------------------------------------
		// Public Properties:
		//-----------------------------------------------------------------------------------------
		
		public string Name { get { return m_effectName; } }

		public Material BaseMaterial {
			get {
				return m_baseMaterial;
			}
		}

		public Shader BaseShader {get { return m_shader; }}

		public Material GenerateMaterial() { return new Material(BaseMaterial); }

		public void EditorAssignMaterial(Material material) { m_baseMaterial = material; }
	}
}