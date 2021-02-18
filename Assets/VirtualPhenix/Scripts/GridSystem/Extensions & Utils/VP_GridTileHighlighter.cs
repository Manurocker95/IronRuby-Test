#if USE_GRID_SYSTEM
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.GridEngine
{
    public class VP_GridTileHighlighter : VP_MonoBehaviour
    {
        [Header("Main GameObject")]
        [SerializeField] protected Transform m_mainGameObject;

        [Header("Offset Position")]
        [SerializeField] protected Vector3 m_offset = Vector3.zero;
        [Header("Unhighlighted Position")]
        [SerializeField] protected Vector3 m_unHighlightedPosition = new Vector3(999, 0, 999);

        protected VP_GridTile m_currentTargetTile;
        protected VP_GridTile m_previousTargetTile;

        protected virtual void Reset()
        {
            m_mainGameObject = transform;
        }

        protected override void Initialize()
        {
            base.Initialize();

            if (!m_mainGameObject)
                m_mainGameObject = transform;
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            VP_GridManager m_gridManager = VP_GridManager.Instance;

            if (m_gridManager != null)
            {
                if (m_gridManager.m_HoveredGridTile != null)
                {
                    // Update the currently and previous hovered tiles
                    if (m_gridManager.m_HoveredGridTile != m_currentTargetTile)
                    {
                        m_previousTargetTile = m_currentTargetTile;
                        m_currentTargetTile = m_gridManager.m_HoveredGridTile;
                    }
                }
                else
                {
                    // Reset the variable
                    if (m_currentTargetTile != null)
                    {
                        m_previousTargetTile = m_currentTargetTile;
                        m_currentTargetTile = null;
                    }
                }

                // Set the highlighter's position
                m_mainGameObject.position = (m_currentTargetTile != null) ? m_currentTargetTile.WorldPosition + m_offset : m_unHighlightedPosition;
            }
            
        }
    }

}
#endif