#if USE_GRID_SYSTEM && USE_TILEMAP
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Object = UnityEngine.Object;
using System.Linq;
using VirtualPhenix.GridEngine;

namespace UnityEditor.Tilemaps
{
    [CustomGridBrush(false, true, false, VirtualPhenix.VP_GridTileMapperSetup.GRID_OBJECT_BRUSH_NAME)]
    [CreateAssetMenu(fileName = "Virtual Phenix: GridObject Brush", menuName = "Virtual Phenix/Grid System/Brushes/Create GridObject Brush")]
    public class VP_TilePaletteGridObjectBrush : VP_GridBrushBase
    {
        private VP_GridObjectBrushCell[] m_cells;
        private Vector3Int m_size;
        private Vector3Int m_pivot;
        private Vector3Int m_moveStartPos;
        private VP_GridObjectBrushCell m_editorCell;
        private bool m_pickedObject = false;

        public VP_GridObjectBrushCell[] Cells { get { return m_cells; } }
        public Vector3Int Pivot { get { return m_pivot; } }
        public Vector3Int Size { get { return m_size; } }
        public VP_GridObjectBrushCell EditorCell { get { return m_editorCell; } set { m_editorCell = value; } }
        public bool pickedObject { get { return (!CellsAreNull() && m_pickedObject); } set { m_pickedObject = value; } }
        public int CurrentLayer { get { return m_editorCell.Layer; } }

        public bool CellsAreNull()
        {
            foreach (VP_GridObjectBrushCell cell in m_cells)
            {
                if (cell.GridObject != null)
                    return false;
            }
            return true;
        }
        public VP_TilePaletteGridObjectBrush()
        {
            Init(Vector3Int.one, Vector3Int.zero);
            SizeUpdated();
        }

        public void Init(Vector3Int size)
        {
            Init(size, Vector3Int.zero);
            SizeUpdated();
        }

        public void Init(Vector3Int size, Vector3Int pivot)
        {
            m_size = size;
            m_pivot = pivot;
            SizeUpdated();
        }
        /*
        public override void MoveStart(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
        {
            if (brushTarget == null || gridLayout == null)
                return;
            // Do not allow editing palettes
            if (brushTarget.layer == 31)
                return;

            Reset();
            UpdateSizeAndPivot(new Vector3Int(position.size.x, position.size.y, 1), Vector3Int.zero);
            m_MoveStartPos = position.min;

            foreach (Vector3Int pos in position.allPositionsWithin)
            {
                Vector3Int brushPosition = new Vector3Int(pos.x - position.x, pos.y - position.y, 0);
                PickCellSceneReference(pos, brushPosition, gridLayout, brushTarget.transform);
            }
        }

        public override void MoveEnd(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
        {
            if (brushTarget == null || gridLayout == null)
                return;
            // Do not allow editing palettes
            if (brushTarget.layer == 31)
                return;

            // Settings for the loop
            var relativePos = position.min - m_MoveStartPos;
            var startX = relativePos.x > 0 ? position.size.x - 1 : 0;
            var endX = relativePos.x > 0 ? 0 : position.size.x - 1;
            var actionX = relativePos.x > 0 ? -1 : 1;
            var startY = relativePos.y > 0 ? position.size.y - 1 : 0;
            var endY = relativePos.y > 0 ? 0 : position.size.y - 1;
            var actionY = relativePos.y > 0 ? -1 : 1;
            var x = startX;
            var y = startY;

            //Debug.Log("startX: " + startX.ToString() + " endX: " + endX.ToString() + " actionX: " + actionX.ToString());
            //Debug.Log("startY: " + startY.ToString() + " endY: " + endY.ToString() + " actionY: " + actionY.ToString());

            // Loop through positions within the bounds based on the direction
            while (true)
            {
                while (true)
                {
                    var local = new Vector3Int(x, y, 0);
                    var location = local + position.position;
                    BrushCell cell = m_Cells[GetCellIndexWrapAround(local.x, local.y, local.z)];
                    if (cell != null && cell.gridObject != null)
                    {
                        GridManager.Instance.MoveTileToPosition(cell.gridObject, location.ToVector2IntXY(), cell.offset);
                    }

                    if (x == endX)
                        break;

                    x += actionX;
                }
                if (y == endY)
                {
                    break;
                }

                y += actionY;
                x = startX;
            }
            ResetPick();
        }
        */

        public override void Paint(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
        {
            if (brushTarget == null || gridLayout == null)
                return;
            // Do not allow editing palettes
            if (brushTarget.layer == 31)
                return;

            if (!pickedObject)
            {
                ResetPick();
            }

            Vector3Int min = position - m_pivot;
            BoundsInt bounds = new BoundsInt(min, m_size);
            BoxFill(gridLayout, brushTarget, bounds);
        }

        public override void BoxFill(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
        {

            if (brushTarget == null || gridLayout == null)
                return;
            // Do not allow editing palettes
            if (brushTarget.layer == 31)
                return;

            foreach (Vector3Int location in position.allPositionsWithin)
            {
                Vector3Int local = location - position.min;
                VP_GridObjectBrushCell cell = m_cells[GetCellIndexWrapAround(local.x, local.y, local.z)];
                if (cell != null)
                {
                    PaintCell(gridLayout, location, cell);
                }
            }
        }

        private void PaintCell(GridLayout grid, Vector3Int position, VP_GridObjectBrushCell cell)
        {
            if (cell.GridObject != null)
            {
                VP_GridManager.Instance.InstantiateGridObject(cell.GridObject, position.ToVector2IntXY(), cell.Orientation);
            }
        }

        public override void Pick(GridLayout gridLayout, GameObject brushTarget, BoundsInt position, Vector3Int pickStart)
        {
            // Do not allow editing palettes
            if (brushTarget.layer == 31)
                return;

            Reset();
            UpdateSizeAndPivot(new Vector3Int(position.size.x, position.size.y, 1), new Vector3Int(pickStart.x, pickStart.y, 0));

            bool pickedAObject = false;
            foreach (Vector3Int pos in position.allPositionsWithin)
            {
                Vector3Int brushPosition = new Vector3Int(pos.x - position.x, pos.y - position.y, 0);
                var objectAtPos = PickCellPrefab(pos, brushPosition, gridLayout, brushTarget.transform);
                if (objectAtPos)
                    pickedAObject = true;
            }
            // Wether or not we picked a tile
            if (pickedAObject)
            {
                pickedObject = true;
            }
            else
            {
                ResetPick();
            }
        }

        private bool PickCellPrefab(Vector3Int position, Vector3Int brushPosition, GridLayout grid, Transform parent)
        {
            if (parent != null)
            {
                var gridComp = grid as Grid;
                Vector3 cellCenter = gridComp.GetCellCenterWorld(position);
                VP_GridObject objectAtPos = VP_GridManager.Instance.GetGridObjectAtPosition(position.ToVector2IntXY());

                if (objectAtPos != null)
                {
                    Object prefab = PrefabUtility.GetCorrespondingObjectFromSource(objectAtPos);

                    if (prefab)
                    {
                        SetGridObject(brushPosition, (VP_GridObject)prefab);
                        SetOrientation(brushPosition, objectAtPos.InitialOrientation);
                        return true;
                    }
                }
            }

            return false;
        }

        private void PickCellSceneReference(Vector3Int position, Vector3Int brushPosition, GridLayout grid, Transform parent)
        {
            if (parent != null)
            {
                var gridComp = grid as Grid;
                Vector3 cellCenter = gridComp.GetCellCenterWorld(position);
                VP_GridObject objectAtPos = VP_GridManager.Instance.GetGridObjectAtPosition(position.ToVector2IntXY());

                if (objectAtPos != null)
                {
                    SetGridObject(brushPosition, objectAtPos);
                    SetOrientation(brushPosition, objectAtPos.InitialOrientation);
                }
            }
        }

        public void SetCellFromEditor(Vector3Int brushPosition, VP_GridObject gridObject, Orientations orientation)
        {
            Reset();
            SetGridObject(brushPosition, gridObject);
            SetOrientation(brushPosition, orientation);

            m_editorCell = new VP_GridObjectBrushCell();
            m_editorCell.GridObject = gridObject;
            m_editorCell.Orientation = orientation;
        }

        public void ClearCellFromEditor()
        {
            ResetPick();
            m_editorCell = null;
        }

        public override void Erase(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
        {
            ResetPick();

            if (brushTarget == null || gridLayout == null)
                return;

            // Do not allow editing palettes
            if (brushTarget.layer == 31)
                return;

            Vector3Int min = position - m_pivot;
            BoundsInt bounds = new BoundsInt(min, m_size);
            BoxErase(gridLayout, brushTarget, bounds);
        }

        public override void BoxErase(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
        {
            if (brushTarget == null || gridLayout == null)
                return;

            // Do not allow editing palettes
            if (brushTarget.layer == 31)
                return;

            foreach (Vector3Int location in position.allPositionsWithin)
            {
                EraseCell(gridLayout, location, brushTarget.transform);
            }

            ResetPick();
        }

        private void EraseCell(GridLayout grid, Vector3Int position, Transform parent)
        {
            VP_GridManager.Instance.EraseGridObjectAtPosition(position.ToVector2IntXY(), CurrentLayer);
        }

        public override void FloodFill(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
        {
            Debug.LogWarning("FloodFill not supported");
        }

        public override void Rotate(RotationDirection direction, Grid.CellLayout layout)
        {
            Debug.Log("Rotate not supported for GridObjects, set their initial orientation on the editor instead");
        }

        public override void Flip(FlipAxis flip, Grid.CellLayout layout)
        {
            Debug.Log("Flip not supported");
        }

        public void Reset()
        {
            UpdateSizeAndPivot(Vector3Int.one, Vector3Int.zero);
            m_moveStartPos = Vector3Int.zero;
        }

        public void ResetPick()
        {
            Reset();
            pickedObject = false;

            if (m_editorCell != null && m_editorCell.GridObject != null)
            {
                m_cells[0] = m_editorCell;
            }
        }

        public void UpdateSizeAndPivot(Vector3Int size, Vector3Int pivot)
        {
            m_size = size;
            m_pivot = pivot;
            SizeUpdated();
        }

        private void SizeUpdated()
        {
            m_cells = new VP_GridObjectBrushCell[m_size.x * m_size.y * m_size.z];
            BoundsInt bounds = new BoundsInt(Vector3Int.zero, m_size);
            foreach (Vector3Int pos in bounds.allPositionsWithin)
            {
                m_cells[GetCellIndex(pos)] = new VP_GridObjectBrushCell();
            }
        }

        public void SetGridObject(Vector3Int position, VP_GridObject go)
        {
            if (ValidateCellPosition(position))
                m_cells[GetCellIndex(position)].GridObject = go;
        }

        public void SetOrientation(Vector3Int position, Orientations orientation)
        {
            if (ValidateCellPosition(position))
                m_cells[GetCellIndex(position)].Orientation = orientation;
        }

        public int GetCellIndex(Vector3Int brushPosition)
        {
            return GetCellIndex(brushPosition.x, brushPosition.y, brushPosition.z);
        }

        public int GetCellIndex(int x, int y, int z)
        {
            return x + m_size.x * y + m_size.x * m_size.y * z;
        }
        public int GetCellIndex(int x, int y, int z, int sizex, int sizey, int sizez)
        {
            return x + sizex * y + sizex * sizey * z;
        }
        public int GetCellIndexWrapAround(int x, int y, int z)
        {
            return (x % m_size.x) + m_size.x * (y % m_size.y) + m_size.x * m_size.y * (z % m_size.z);
        }

        private bool ValidateCellPosition(Vector3Int position)
        {
            var valid =
                position.x >= 0 && position.x < m_size.x &&
                position.y >= 0 && position.y < m_size.y &&
                position.z >= 0 && position.z < m_size.z;
            if (!valid)
                throw new ArgumentException(string.Format("Position {0} is an invalid cell position. Valid range is between [{1}, {2}).", position, Vector3Int.zero, m_size));
            return valid;
        }
    }
}
#endif