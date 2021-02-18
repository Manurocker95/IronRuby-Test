#if USE_GRID_SYSTEM
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.GridEngine
{
    public static class VP_GridUtilities
    {
#if UNITY_EDITOR

        [UnityEditor.MenuItem("Virtual Phenix/Grid System/Tilemap/Tilemap Next &t")] // alt+t
        public static void OpenNextTilemap() => NextTilemap(move: 1);

        [UnityEditor.MenuItem("Virtual Phenix/Grid System/Tilemap/Tilemap Prev #&t")] // shift+alt+t
        public static void OpenPrevTilemap() => NextTilemap(move: -1);

        public static void NextTilemap(int move)
        {
            UnityEditor.EditorApplication.ExecuteMenuItem("Window/2D/Tile Palette");
            var targets = UnityEditor.Tilemaps.GridPaintingState.validTargets
                .OrderBy(go => go.transform.GetSiblingIndex()).ToArray();

            int now = System.Array.IndexOf(targets, UnityEditor.Tilemaps.GridPaintingState.scenePaintTarget);
            if (now != -1)
            {
                int next = now + move;
                if (next >= targets.Length) next = 0;
                else if (next < 0) next = targets.Length - 1;

                UnityEditor.Tilemaps.GridPaintingState.scenePaintTarget = targets[next];
                UnityEditor.EditorGUIUtility.PingObject(UnityEditor.Tilemaps.GridPaintingState.scenePaintTarget);
            }
            else Debug.LogWarning($"No valid Tilemap Targets");
        }

     /*
        [UnityEditor.MenuItem("Virtual Phenix/Grid System/Paint State/Valid Target Count")]
        public static void CheckTargetCount()
        {
            Debug.Log(UnityEditor.Tilemaps.GridPaintingState.validTargets != null ? UnityEditor.Tilemaps.GridPaintingState.validTargets.Length : 0);
        }    


        [UnityEditor.MenuItem("Virtual Phenix/Grid System/Troubleshooting/Delete Default UGE Brush")]
        public static void DeleteUGEBrush()
        {
            var brushes = new List<GridBrushBase>(UnityEditor.Tilemaps.GridPaintingState.brushes);

            foreach (var brush in brushes)
            {
                if (brush.name.Contains("UGE"))
                    UnityEditor.Tilemaps.GridPaintingState.brushes.Remove(brush);
            }

            Debug.Log(brushes.Count);
            Debug.Log(UnityEditor.Tilemaps.GridPaintingState.gridBrush);
            Debug.Log(UnityEditor.Tilemaps.GridPaintingState.activeBrushEditor);
            VP_Utils.PingObject(UnityEditor.Tilemaps.GridPaintingState.gridBrush);
        }
    */

        [UnityEditor.MenuItem("Virtual Phenix/Grid System/Utils/Set GridObj Pos based on GridTile")]
        public static void SetGridObjPositionBasedOnTilePosition()
        {
            foreach (UnityEngine.Object obj in UnityEditor.Selection.objects)
            {
                if (obj is GameObject && ((GameObject)obj).TryGetComponent(out VP_GridObject go))
                {
                    if (go.CurrentGridTile != null)
                    {
                        go.GridPosition = go.CurrentGridTile.GridPosition;
                    }
                }
            }
        }
        [UnityEditor.MenuItem("Virtual Phenix/Grid System/Utils/Set Tile GridPos based on GridObj")]
        public static void SetGridTilePositionBasedOnGridObjectPosition()
        {
            foreach (UnityEngine.Object obj in UnityEditor.Selection.objects)
            {
                if (obj is GameObject && ((GameObject)obj).TryGetComponent(out VP_GridObject go))
                {
                    if (go.CurrentGridTile != null)
                    {
                        go.CurrentGridTile.GridPosition = go.GridPosition;
                    }
                }
            }
        }

        [UnityEditor.MenuItem("Virtual Phenix/Grid System/Utils/Set Current Tile To Grid Object")]
        public static void SetSelectedGridTilesToGridObject()
        {
            VP_GridManager gridManager = VP_GridManager.Instance;

            if (gridManager != null)
            {
                foreach (UnityEngine.Object obj in UnityEditor.Selection.objects)
                {
                    if (obj is GameObject && ((GameObject)obj).TryGetComponent(out VP_GridObject go))
                    {
                        if (gridManager.ExistsTileAtPosition(go.GridPosition, out VP_GridTile _otherTile))
                        {
                            go.CurrentGridTile = gridManager.GetGridTileAtPosition(go.GridPosition);
                        }
                    }
                }
            }
        }

         [UnityEditor.MenuItem("Virtual Phenix/Grid System/Utils/Set Selected Grid Tiles To GridManager")]
        public static void SetSelectedGridTilesToGridManager()
        {
            VP_GridManager gridManager = VP_GridManager.Instance;

            if (gridManager != null)
            {
                foreach (UnityEngine.Object obj in UnityEditor.Selection.objects)
                {
                    if (obj is GameObject && ((GameObject)obj).TryGetComponent(out VP_GridTile tile))
                    {
                        gridManager.SetGridTileAtItsPosition(tile);
                    }
                }
            }
        }
#endif
        // Manhattan distance used for rectangle grids
        public static float Heuristic(VP_GridTile a, VP_GridTile b)
        {
            return Mathf.Abs(a.GridPosition.x - b.GridPosition.x) + Mathf.Abs(a.GridPosition.y - b.GridPosition.y);
        }

        // Distance calculation based on cube coordinates used in hexagon grids
        public static float HexCubeDistance(Vector3Int a, Vector3Int b)
        {
            return Mathf.Max(Mathf.Abs(a.x - b.x), Mathf.Abs(a.y - b.y), Mathf.Abs(a.z - b.z));
        }

        // Turn R offset coordinates into cube coordinates used in hexagon grids
        public static Vector3Int ROffsetToCubeCoordinates(Vector2Int gridPosition)
        {
            int x = gridPosition.x - ((gridPosition.y - (gridPosition.y & 1)) / 2);
            int y = gridPosition.y;
            int z = -x - y;
            return new Vector3Int(x, y, z);
        }

        // Takes two offset coordinates converts them to cube and then calculates the distance used for hexagong rids.
        public static float HexDistance(VP_GridTile a, VP_GridTile b)
        {
            var aCube = ROffsetToCubeCoordinates(a.GridPosition);
            var bCube = ROffsetToCubeCoordinates(b.GridPosition);
            return HexCubeDistance(aCube, bCube);
        }

        public static Color ColorFromRGB(int r, int g, int b)
        {
            return new Color((float)r / 256, (float)g / 256, (float)b / 256);
        }

        public static bool RollRandom(float percentage)
        {
            return (new System.Random().Next(100)) < percentage;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array">the original array to be rotated</param>
        /// <param name="size">the size from a 2d perspective of the grid</param>
        /// <param name="iterations">the number of 90 degrees rotations</param>
        /// <returns></returns>
        public static T[] RotateArray<T>(T[] array, Vector2Int size, int iterations)
        {
            var n = size.x;
            var m = size.y;
            var ret = new T[m * n];

            //90 degrees
            if (iterations == 1)
            {
                for (int i = 0; i < m; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        ret[j + i * n] = array[i + (n - 1 - j) * m];
                    }
                }
            }
            //180
            else if (iterations == 2)
            {
                for (int i = 0; i < m; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        ret[i + j * m] = array[(m - 1 - i) + (n - 1 - j) * m];
                    }
                }
            }
            //-90
            else if (iterations == 3)
            {
                for (int i = 0; i < m; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        ret[j + i * n] = array[(m - 1 - i) + j * m];
                    }
                }
            }
            return ret;
        }

        public static T[] FlipArrayX<T>(T[] array, Vector2Int size)
        {
            var columns = size.x;
            var lines = size.y;
            var ret = new T[lines * columns];
            for (int i = 0; i < lines; i++)
            {
                for (int j = 0; j < columns / 2; j++)
                {
                    ret[j + i * columns] = array[(columns - 1 - j) + i * columns];
                    ret[(columns - 1 - j) + i * columns] = array[j + i * columns];
                }
            }
            return ret;
        }

        public static T[] FlipArrayY<T>(T[] array, Vector2Int size)
        {
            var columns = size.x;
            var lines = size.y;
            var ret = new T[lines * columns];
            for (int i = 0; i < lines / 2; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    ret[j + i * columns] = array[j + (lines - 1 - i) * columns];
                    ret[j + (lines - 1 - i) * columns] = array[j + i * columns];
                }
            }
            return ret;
        }
    }

}
#endif