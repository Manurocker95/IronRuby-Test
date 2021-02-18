// Original: 
namespace VirtualPhenix.Dialog
{
    using System.Collections;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    [RequireComponent(typeof(TextMeshProUGUI))]
    public abstract class VP_DialogTextAnimation : VP_MonoBehaviour
    {
        [SerializeField] private int m_firstCharToAnimate;

        [SerializeField] private int m_lastCharToAnimate;

        /// <summary>
        /// Determines how often Animate() will be called
        /// </summary>
        private const float m_frameRate = 15f;
        private static readonly float m_timeBetweenAnimates = 1f / m_frameRate;

        private float m_lastAnimateTime;
        private TextMeshProUGUI m_textComponent;
        private TMP_TextInfo m_textInfo;
        private TMP_MeshInfo[] m_cachedMeshInfo;
        private bool m_playing = false;

        protected int FirstCharToAnimate
        {
            get
            {
                return this.m_firstCharToAnimate;
            }
        }
        protected int LastCharToAnimate
        {
            get
            {
                return this.m_lastCharToAnimate;
            }
        }

        private TextMeshProUGUI TextComponent
        {
            get
            {
                if (this.m_textComponent == null)
                {
                    this.m_textComponent = this.GetComponent<TextMeshProUGUI>();
                }

                return this.m_textComponent;
            }
        }

        /// <summary>
        /// Set the range of characters that should be animated by this Component
        /// </summary>
        /// <param name="firstChar">0-based index of the first printable character that should be animated</param>
        /// <param name="lastChar">0-based index of the last printable character that should be animated</param>
        public void SetCharsToAnimate(int firstChar, int lastChar)
        {
            this.m_firstCharToAnimate = firstChar;
            this.m_lastCharToAnimate = lastChar;
        }

        public void StopPlaying()
        {
            m_playing = false;
        }

        public void Play()
        {
            m_playing = true;
        }

        /// <summary>
        /// Cache the vertex data of the text object b/c the animation transform is applied to the original position of the characters.
        /// </summary>
        public void CacheTextMeshInfo()
        {
            this.m_textInfo = this.TextComponent.textInfo;
            this.m_cachedMeshInfo = this.m_textInfo.CopyMeshInfoVertexData();
        }

        protected override void Start()
        {
            base.Start();
            this.TextComponent.ForceMeshUpdate();
            this.m_lastAnimateTime = float.MinValue;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            TMPro_EventManager.TEXT_CHANGED_EVENT.Add(OnTMProChanged);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(OnTMProChanged);

            // Reset to baseline/standard text mesh
            this.TextComponent.ForceMeshUpdate();
        }

        protected virtual void Update()
        {
            if (m_playing)
            {
                if (Time.time > this.m_lastAnimateTime + m_timeBetweenAnimates)
                {
                    this.AnimateAllChars();
                }
            }
        }

        /// <summary>
        /// Derived classes must implement how individual characters are animated
        /// </summary>
        /// <param name="characterIndex">Index of character being animated</param>
        /// <param name="translation">X/Y translation vector</param>
        /// <param name="rotation">2D rotation angle</param>
        /// <param name="scale">Uniform scale</param>
        protected abstract void Animate(int characterIndex, out Vector2 translation, out float rotation, out float scale);

        /// <summary>
        /// Get the vertices of the TMPro mesh, request translation/rotation/scale info from Animate(), 
        /// then, transform the vertices and apply them back to the mesh
        /// </summary>
        public void AnimateAllChars()
        {
            this.m_lastAnimateTime = Time.time;

            int characterCount = this.m_textInfo.characterCount;

            // If no characters do nothing
            if (characterCount == 0)
            {
                return;
            }

            for (int i = 0; i < characterCount; i++)
            {
                // Skip characters that aren't specified to animate
                if (i < this.m_firstCharToAnimate || i > this.m_lastCharToAnimate)
                {
                    continue;
                }

                TMP_CharacterInfo charInfo = m_textInfo.characterInfo[i];

                // Skip characters that are not visible and thus have no geometry to manipulate.
                if (!charInfo.isVisible)
                {
                    continue;
                }

                // Get the index of the material used by the current character.
                int materialIndex = charInfo.materialReferenceIndex;

                // Get the index of the first vertex used by this text element.
                int vertexIndex = charInfo.vertexIndex;

                // Get the cached vertices of the mesh used by this text element (character or sprite).
                Vector3[] sourceVertices = m_cachedMeshInfo[materialIndex].vertices;

                // NOTE: Alternate calculation - Determine the center point of each character at the baseline.
                //Vector2 charMidBasline = new Vector2((sourceVertices[vertexIndex + 0].x + sourceVertices[vertexIndex + 2].x) / 2, charInfo.baseLine);
                // Determine the center point of each character.
                Vector2 charMidBasline = (sourceVertices[vertexIndex + 0] + sourceVertices[vertexIndex + 2]) / 2;

                // Need to translate all 4 vertices of each quad to align with middle of character / baseline.
                // This is needed so the matrix TRS is applied at the origin for each character.
                Vector3 offset = charMidBasline;

                Vector3[] destinationVertices = m_textInfo.meshInfo[materialIndex].vertices;

                // Apply offset from center
                destinationVertices[vertexIndex + 0] = sourceVertices[vertexIndex + 0] - offset;
                destinationVertices[vertexIndex + 1] = sourceVertices[vertexIndex + 1] - offset;
                destinationVertices[vertexIndex + 2] = sourceVertices[vertexIndex + 2] - offset;
                destinationVertices[vertexIndex + 3] = sourceVertices[vertexIndex + 3] - offset;

                Vector2 translation;
                float rotation, scale;

                // This is where the derived class sets translation/rotation/scale
                this.Animate(i, out translation, out rotation, out scale);
                Matrix4x4 matrix = Matrix4x4.TRS(translation, Quaternion.Euler(0f, 0f, rotation), scale * Vector3.one);

                // Apply the derived class transformation
                destinationVertices[vertexIndex + 0] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 0]);
                destinationVertices[vertexIndex + 1] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 1]);
                destinationVertices[vertexIndex + 2] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 2]);
                destinationVertices[vertexIndex + 3] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 3]);

                // Remove offset from center
                destinationVertices[vertexIndex + 0] += offset;
                destinationVertices[vertexIndex + 1] += offset;
                destinationVertices[vertexIndex + 2] += offset;
                destinationVertices[vertexIndex + 3] += offset;
            }

            this.ApplyChangesToMesh();
        }

        /// <summary>
        /// Apply the modified vertices (calculated by Animate) to the mesh
        /// </summary>
        private void ApplyChangesToMesh()
        {
            for (int i = 0; i < this.m_textInfo.meshInfo.Length; i++)
            {
                this.m_textInfo.meshInfo[i].mesh.vertices = this.m_textInfo.meshInfo[i].vertices;
                this.TextComponent.UpdateGeometry(this.m_textInfo.meshInfo[i].mesh, i);
            }
        }

        /// <summary>
        /// This event is fired whenever the TMPro mesh is updated
        /// For example, if the text string changes or MaxVisibleCharacters changes
        /// </summary>
        private void OnTMProChanged(Object obj)
        {
            if (obj == this.TextComponent)
            {
                this.CacheTextMeshInfo();
            }
        }
    }
}