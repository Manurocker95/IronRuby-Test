#if USE_GRID_SYSTEM
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.GridEngine
{
    [DefaultExecutionOrder(VP_ExecutingOrderSetup.GRID_HIGHLIGHT_MANAGER)]
    public class VP_GridHighlightManager : VP_SingletonMonobehaviour<VP_GridHighlightManager>
    {

        public List<VP_GridTile> m_CurrentlyHighlightedTiles = new List<VP_GridTile>();

        [Header("Visual Node Settings")]
        public GameObject[] m_NodePrefabs;
        public Vector3 m_NodeWorldPositionOffSet;

        protected Transform _highlightNodePrefabsHolder;
        protected string _holderName = "Highlight Holder";

        public virtual void HighlighTiles(List<VP_GridTile> tilesToHighlight, int prefabIndex = 0, bool UnhighlightPreviousTiles = true)
        {
            if (UnhighlightPreviousTiles)
                UnHighlightTiles();

            if (tilesToHighlight != null && tilesToHighlight.Count > 0)
            {
                if (_highlightNodePrefabsHolder == null)
                {
                    _highlightNodePrefabsHolder = new GameObject(_holderName).transform;
                }

                for (int i = 0; i < tilesToHighlight.Count; i++)
                {
                    var newNode = Instantiate(m_NodePrefabs[prefabIndex], tilesToHighlight[i].WorldPosition + m_NodeWorldPositionOffSet, Quaternion.identity).transform;
                    newNode.parent = _highlightNodePrefabsHolder;
                }
            }
        }

        public virtual void UnHighlightTiles()
        {
            if (_highlightNodePrefabsHolder != null)
            {
                m_CurrentlyHighlightedTiles.Clear();
                DestroyImmediate(_highlightNodePrefabsHolder.gameObject);
            }
        }
    }

}
#endif