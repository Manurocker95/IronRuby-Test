#if USE_GRID_SYSTEM
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace VirtualPhenix.GridEngine
{
    [RequireComponent(typeof(VP_GridObject))]
    public class VP_GridPathVisualizer3D : VP_MonoBehaviour
    {

        public bool m_Activated = false;

        public GameObject m_NodePrefab;
        public Vector3 m_NodeWorldPositionOffSet;

        protected VP_GridObject _gridObject;
        protected List<VP_GridTile> _currentPath;
        protected Transform _visualPathHolder;
        protected string _holderName = "Visual Path Holder";

        protected override void Awake()
        {
            base.Awake();
            _gridObject = GetComponent<VP_GridObject>();
        }

        private void Update()
        {

            // Enable or Disable on keypress
            if (Input.GetKeyDown(KeyCode.C))
            {
                m_Activated = !m_Activated;

                if (!m_Activated)
                {
                    ClearVisualPath();
                }
            }

            // Update only every 6 frames
            if (Time.frameCount % 6 != 0)
                return;
            // If it isn't activated just return
            if (!m_Activated)
            {
                return;
            }

            var targetTile = VP_GridManager.Instance.m_HoveredGridTile;
            var path = targetTile == null ? null : VP_GridAStar.Search(_gridObject.CurrentGridTile, targetTile);

            if (!VP_GridHelpers.CompareLists(_currentPath, path))
            {
                _currentPath = path;
                ClearVisualPath();

                if (_currentPath != null && _currentPath.Count > 0 && _currentPath.Contains(targetTile))
                {
                    PopulateVisualPath();
                }
            }
        }

        private void PopulateVisualPath()
        {
            if (_visualPathHolder == null)
            {
                _visualPathHolder = new GameObject(_holderName).transform;
            }

            var lastDirection = _currentPath[0].GridPosition - _gridObject.GridPosition;
            for (int i = 0; i < _currentPath.Count; i++)
            {
                var newNode = Instantiate(m_NodePrefab, _currentPath[i].WorldPosition + m_NodeWorldPositionOffSet, Quaternion.identity).transform;
                newNode.parent = _visualPathHolder;

                if (i + 1 < _currentPath.Count)
                {
                    lastDirection = _currentPath[i + 1].GridPosition - _currentPath[i].GridPosition;
                }
                // Set the rotation of the node so it faces the next position on the path
                newNode.rotation = Quaternion.LookRotation(lastDirection.ToVector3X0Z());

            }

            foreach (VP_GridTile tile in _currentPath)
            {
                //var newNode = Instantiate(m_NodePrefab, tile.m_WorldPosition + m_NodeWorldPositionOffSet, Quaternion.identity).transform;
                //newNode.parent = _visualPathHolder;
            }
        }

        private void ClearVisualPath()
        {
            if (_visualPathHolder != null)
            {
                DestroyImmediate(_visualPathHolder.gameObject);
            }
        }

    }

}
#endif