#if USE_GRID_SYSTEM
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.GridEngine
{
    public class VP_GridPathfindingDemoVisualizer : VP_SingletonMonobehaviour<VP_GridPathfindingDemoVisualizer>
    {
        [Header("Path Settings")]
        public Color SearchToColor;
        public Color SearchFromColor;
        public Color PathColor;

        public VP_GridPathfindingDemoGridTile FromTile, ToTile;

        // The current path
        public List<VP_GridTile> Path = new List<VP_GridTile>();

        [Header("Editor Settings")]
        public Color WalkableTileColor;
        public Color NonWalkableTileColor;

        void Update()
        {
            /*
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButtonDown(0)) {
                // Check if this is the highlighted tile
                if (GridTile.HighlightedGridTile != null) {

                    // Unset the tsf first
                    if (FromTile != null) {
                        FromTile.DisableFromTile();
                    }

                    var comp = GridTile.HighlightedGridTile as PathfindingDemoGridTile;
                    comp.EnableFromTile();
                }
            } 
            */
            /*
            if (FromTile != null & ToTile != null && FromTile != ToTile) {
                var tempPath = AStar.Search(FromTile, ToTile);

                if (Path != tempPath) {
                    if (Path != null) {
                        UnHighlightPath(Path);
                        Path.Clear();
                    }

                    Path = tempPath;
                    if (Path != null && Path.Count > 0 && Path.Contains(ToTile)) {
                        HighlightPath(Path);
                    }
                }
            }
            */
        }

        // Change the color of the tiles in the path and update their cost/distance text
        public virtual void HighlightPath(List<VP_GridTile> path)
        {
            int CostSoFar = 0;

            for (int i = 0; i < path.Count; i++)
            {
                CostSoFar += path[i].Cost();
                var tile = path[i] as VP_GridPathfindingDemoGridTile;
                // Distance Label
                tile.Distance = CostSoFar;
                tile.UpdateDistanceLabel();
                tile.EnableDistanceText();
                // Color
                if (tile != ToTile)
                {
                    tile.ChangeColor(PathColor);
                }
            }
        }

        public virtual void UnHighlightPath()
        {
            if (Path != null & Path.Count > 0)
            {
                UnHighlightPath(Path);
            }
        }

        // Reset back the color of the tiles in the path and hide their cost/distance text
        public virtual void UnHighlightPath(List<VP_GridTile> path)
        {
            for (int i = 0; i < path.Count; i++)
            {
                var tile = path[i] as VP_GridPathfindingDemoGridTile;
                // Text Label
                if (tile != FromTile)
                    tile.DisableDistanceText();
                // Color
                if (i != path.Count - 1 && tile != ToTile)
                {
                    tile.ChangeColor(WalkableTileColor);
                }
            }
        }

        public virtual void SetTargetTile(VP_GridPathfindingDemoGridTile targetTile)
        {
            if (ToTile != targetTile)
            {
                ToTile = targetTile;
                UnHighlightPath();
            }

            if (FromTile != null && FromTile != ToTile)
            {
                Path = VP_GridAStar.Search(FromTile, ToTile);
                if (Path != null && Path.Count > 0 && Path.Contains(ToTile))
                {
                    HighlightPath(Path);
                }
            }
        }

        public virtual void SetFromTile(VP_GridPathfindingDemoGridTile fromTile)
        {
            if (FromTile != fromTile)
            {
                UnHighlightPath();
                FromTile = fromTile;
            }
        }
    }

}
#endif