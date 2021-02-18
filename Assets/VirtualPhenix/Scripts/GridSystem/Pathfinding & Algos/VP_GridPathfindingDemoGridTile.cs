#if USE_GRID_SYSTEM
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace VirtualPhenix.GridEngine
{
    public class VP_GridPathfindingDemoGridTile : VP_GridTile
    {
        protected int _distance;
        protected Renderer _renderer;

        public int Distance
        {
            get
            {
                return _distance;
            }
            set
            {
                _distance = value;
                UpdateDistanceLabel();
            }
        }

        protected override void Start()
        {
            base.Start();
            DisableDistanceText();
            _renderer = GetComponent<Renderer>();
            if (_renderer == null)
            {
                _renderer = GetComponentInChildren<Renderer>();
            }
            if (!IsTileWalkable)
            {
                SetUnWalkable();
            }
        }

        protected virtual void Update()
        {
            // Tile to search from
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButtonDown(0))
            {
                // Check if this is the highlighted tile
                if (VP_GridManager.Instance.m_HoveredGridTile != this)
                    return;

                // Unset the tsf first
                if (VP_GridPathfindingDemoVisualizer.Instance.FromTile != null)
                {
                    VP_GridPathfindingDemoVisualizer.Instance.FromTile.DisableFromTile();
                }

                EnableFromTile();
            }
        }

        protected override void OnMouseEnter()
        {
            base.OnMouseEnter();
            EnableHighlight();
        }

        protected override void OnMouseExit()
        {
            base.OnMouseExit();
            DisableHighlight();
        }

        protected virtual void OnMouseDown()
        {

            if (Input.GetKey(KeyCode.LeftShift))
            {
                // Unset the FromTile first
                if (VP_GridPathfindingDemoVisualizer.Instance.FromTile != null)
                {
                    VP_GridPathfindingDemoVisualizer.Instance.FromTile.DisableFromTile();
                }

                EnableFromTile();
            }
            else if (IsTileWalkable)
            {
                SetUnWalkable();
            }
            else
            {
                SetWalkable();
            }
        }

        public virtual void DisableHighlight()
        {
            if (!IsTileWalkable)
                return;

            var inst = VP_GridPathfindingDemoVisualizer.Instance;
            if (inst.FromTile == this)
            {
                return;
            }

            if (inst.ToTile == this)
            {
                inst.ToTile = null;

                ChangeColor(inst.WalkableTileColor);
            }
        }

        public virtual void EnableHighlight()
        {
            // Check if the tile is walkable
            if (!IsTileWalkable)
                return;
            var inst = VP_GridPathfindingDemoVisualizer.Instance;
            // Don't set the current target tile as highlighted if its the current search from tile
            if (inst.FromTile != null)
            {
                if ((inst.FromTile == VP_GridManager.Instance.m_HoveredGridTile))
                    return;
            }

            ChangeColor(inst.SearchToColor);

            inst.SetTargetTile(this);
        }

        public virtual void EnableDistanceText()
        {
            TMPro.TMP_Text text = GetComponentInChildren<TMPro.TMP_Text>();
            text.enabled = true;
        }
        public virtual void DisableDistanceText()
        {
            TMPro.TMP_Text text = GetComponentInChildren<TMPro.TMP_Text>();
            text.enabled = false;
        }

        public virtual void UpdateDistanceLabel()
        {
            TMPro.TMP_Text label = GetComponentInChildren<TMPro.TMP_Text>();
            label.text = Distance.ToString();
        }

        public virtual void EnableFromTile()
        {
            if (!IsTileWalkable)
                return;

            if (VP_GridPathfindingDemoVisualizer.Instance.ToTile == this)
            {
                DisableHighlight();
            }
            // Set the from tile
            VP_GridPathfindingDemoVisualizer.Instance.SetFromTile(this);
            // Distance text
            Distance = 0;
            UpdateDistanceLabel();
            EnableDistanceText();
            // Set the color
            ChangeColor(VP_GridPathfindingDemoVisualizer.Instance.SearchFromColor);

        }

        public virtual void DisableFromTile()
        {
            if (VP_GridPathfindingDemoVisualizer.Instance.FromTile == this)
            {
                VP_GridPathfindingDemoVisualizer.Instance.FromTile = null;
            }

            DisableDistanceText();
            ChangeColor(VP_GridPathfindingDemoVisualizer.Instance.WalkableTileColor);
        }

        public virtual void SetWalkable()
        {
            m_isTileWalkable = true;

            DisableDistanceText();
            ChangeColor(VP_GridPathfindingDemoVisualizer.Instance.WalkableTileColor);
        }

        public virtual void SetUnWalkable()
        {
            m_isTileWalkable = false;
            var inst = VP_GridPathfindingDemoVisualizer.Instance;
            if (inst.FromTile == this)
            {
                inst.FromTile = null;
                inst.UnHighlightPath();
            }

            if (inst.ToTile == this)
            {
                inst.ToTile = null;
                inst.UnHighlightPath();
            }

            DisableDistanceText();
            ChangeColor(inst.NonWalkableTileColor);
        }

        public virtual void ChangeColor(Color color)
        {
            if (_renderer)
            {
                _renderer.material.color = color;
            }
        }
    }

}
#endif