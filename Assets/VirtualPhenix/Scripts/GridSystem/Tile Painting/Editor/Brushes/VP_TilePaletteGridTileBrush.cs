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
    [CustomGridBrush(false, true, false, VirtualPhenix.VP_GridTileMapperSetup.GRID_TILE_BRUSH_NAME)]
    [CreateAssetMenu(fileName = "Virtual Phenix: GridTile Brush", menuName = "Virtual Phenix/Grid System/Brushes/Create GridTile Brush")]
    public class VP_TilePaletteGridTileBrush : VP_GridBrushBase
    {
        protected VP_GridTileBrushCell[] m_cells;
        protected Vector3Int m_size;
        protected Vector3Int m_pivot;
        protected Vector3Int m_moveStartPos;
        protected VP_GridTileBrushCell m_editorCell;
        protected bool m_pickedTile = false;
        protected bool m_lockZPosition = true;


        public VP_GridTileBrushCell[] Cells { get { return m_cells; } }
        public Vector3Int Pivot { get { return m_pivot; } }
        public Vector3Int Size { get { return m_size; } }
        public VP_GridTileBrushCell EditorCell { get { return m_editorCell; } set { m_editorCell = value; } }
        public UnityEngine.Object CellTileObject { get { return m_editorCell != null && m_editorCell.GridTile != null ? m_editorCell.GridTile.gameObject : null; } }
        public bool PickedTile { get { return (!CellsAreNull() && m_pickedTile); } set { m_pickedTile = value; } }
        public bool LockedZPosition { get { return (!CellsAreNull() && m_lockZPosition); } set { m_lockZPosition = value; } }
        public int CurrentLayer { get { return m_editorCell != null ? m_editorCell.Layer : 0; } }

        public virtual bool CellsAreNull()
        {
            foreach (VP_GridTileBrushCell cell in m_cells)
            {
                if (cell.GridTile != null)
                    return false;
            }
            return true;
        }
        public VP_TilePaletteGridTileBrush()
        {
            Init(Vector3Int.one, Vector3Int.zero);
            SizeUpdated();
        }

        public virtual void Init(Vector3Int size)
        {
            Init(size, Vector3Int.zero);
            SizeUpdated();
        }

        public virtual void Init(Vector3Int size, Vector3Int pivot)
        {
            m_size = size;
            m_pivot = pivot;
            SizeUpdated();
        }

        [AddComponentMenu("VirtualPhenix/GridSystem/Brush/Create Tile Brush")]
        public static void CreateBrush()
        {

        }

        public override void MoveStart(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
        {
            if (brushTarget == null || gridLayout == null)
                return;
            // Do not allow editing palettes
            if (brushTarget.layer == 31)
                return;

            Reset();
            UpdateSizeAndPivot(new Vector3Int(position.size.x, position.size.y, 1), Vector3Int.zero);
            m_moveStartPos = position.min;

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
            var relativePos = position.min - m_moveStartPos;
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
                    VP_GridTileBrushCell cell = m_cells[GetCellIndexWrapAround(local.x, local.y, local.z)];
                    if (cell != null && cell.GridTile != null)
                    {
                        VP_GridManager.Instance.MoveTileToPosition(cell.GridTile, location.ToVector2IntXY(), cell.Offset, cell.Layer);
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

        public override void Paint(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
        {
            if (brushTarget == null || gridLayout == null)
                return;
            // Do not allow editing palettes
            if (brushTarget.layer == 31)
                return;

            if (!PickedTile)
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
                VP_GridTileBrushCell cell = m_cells[GetCellIndexWrapAround(local.x, local.y, local.z)];
                if (cell != null)
                {
                    PaintCell(gridLayout, location, cell);
                }
            }
        }

        protected virtual void PaintCell(GridLayout grid, Vector3Int position, VP_GridTileBrushCell cell)
        {
            if (cell.GridTile != null)
            {
                VP_GridManager.Instance.InstantiateGridTile(cell.GridTile, position.ToVector2IntXY(), cell.Height, cell.Layer, cell.TileType, cell.Offset, cell.Orientation);
            }
        }

        public override void Pick(GridLayout gridLayout, GameObject brushTarget, BoundsInt position, Vector3Int pickStart)
        {
            // Do not allow editing palettes
            if (brushTarget.layer == 31)
                return;

            Reset();
            UpdateSizeAndPivot(new Vector3Int(position.size.x, position.size.y, 1), new Vector3Int(pickStart.x, pickStart.y, 0));

            bool pickedATile = false;
            foreach (Vector3Int pos in position.allPositionsWithin)
            {
                Vector3Int brushPosition = new Vector3Int(pos.x - position.x, pos.y - position.y, 0);
                var tileAtPos = PickCellPrefab(pos, brushPosition, gridLayout, brushTarget.transform);
                if (tileAtPos)
                    pickedATile = true;
            }
            // Wether or not we picked a tile
            if (pickedATile)
            {
                PickedTile = true;
            }
            else
            {
                ResetPick();
            }
        }

        protected virtual bool PickCellPrefab(Vector3Int position, Vector3Int brushPosition, GridLayout grid, Transform parent)
        {
            if (parent != null)
            {
                var gridComp = grid as Grid;
                Vector3 cellCenter = gridComp.GetCellCenterWorld(position);
                VP_GridTile tileAtPos = VP_GridManager.Instance.GetGridTileAtPosition(position.ToVector2IntXY());

                if (tileAtPos != null)
                {
                    Object prefab = PrefabUtility.GetCorrespondingObjectFromSource(tileAtPos);

                    if (prefab)
                    {
                        SetGridTile(brushPosition, (VP_GridTile)prefab);
                        SetHeight(brushPosition, tileAtPos.TileHeight);
                        SetGridTileType(brushPosition, tileAtPos.TileType);
                        SetGridTileLayer(brushPosition, tileAtPos.TileLayer);
                        SetOffset(brushPosition, tileAtPos.transform.position - cellCenter);
                        SetOrientation(brushPosition, tileAtPos.transform.localRotation);
                        return true;
                    }
                }
            }

            return false;
        }

        protected virtual void PickCellSceneReference(Vector3Int position, Vector3Int brushPosition, GridLayout grid, Transform parent)
        {
            if (parent != null)
            {
                var gridComp = grid as Grid;
                Vector3 cellCenter = gridComp.GetCellCenterWorld(position);
                VP_GridTile tileAtPos = VP_GridManager.Instance.GetGridTileAtPosition(position.ToVector2IntXY());

                if (tileAtPos != null)
                {
                    SetGridTile(brushPosition, tileAtPos);
                    SetHeight(brushPosition, tileAtPos.TileHeight);
                    SetGridTileType(brushPosition, tileAtPos.TileType);
                    SetGridTileLayer(brushPosition, tileAtPos.TileLayer);
                    SetOffset(brushPosition, tileAtPos.transform.position - cellCenter);
                    SetOrientation(brushPosition, tileAtPos.transform.localRotation);
                }
            }
        }

        public virtual void SetCellFromEditor(Vector3Int brushPosition, VP_GridTile gridTile, int height, Vector3 offSet, Quaternion orientation, int _layer, GRID_TILE_TYPES _type)
        {
            Reset();
            SetGridTile(brushPosition, gridTile);
            SetGridTileType(brushPosition, _type);
            SetGridTileLayer(brushPosition, _layer);
            SetHeight(brushPosition, height);
            SetOffset(brushPosition, offSet);
            SetOrientation(brushPosition, orientation);

            m_editorCell = new VP_GridTileBrushCell();
            m_editorCell.GridTile = gridTile;
            m_editorCell.Height = height;
            m_editorCell.Layer = _layer;
            m_editorCell.TileType = _type;
            m_editorCell.Offset = offSet;
            m_editorCell.Orientation = orientation;
        }

        public virtual void ClearCellFromEditor()
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

        protected virtual void EraseCell(GridLayout grid, Vector3Int position, Transform parent)
        {
            VP_GridManager.Instance.EraseGridTileAtPosition(position.ToVector2IntXY(), CurrentLayer);
        }

        public override void FloodFill(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
        {
            Debug.LogWarning("FloodFill not supported");
        }

        public override void Rotate(RotationDirection direction, Grid.CellLayout layout)
        {
            switch (layout)
            {
                case GridLayout.CellLayout.Hexagon:
                    {
                        var rotationDirection = direction == RotationDirection.Clockwise ? 60 : -60;
                        var rotationAxis = VP_GridManager.Instance.GetRotationAxisVector();
                        Quaternion orientation = Quaternion.Euler(rotationAxis * rotationDirection);
                        foreach (VP_GridTileBrushCell cell in m_cells)
                        {
                            cell.Orientation = cell.Orientation * orientation;
                        }
                    }
                    break;
                case Grid.CellLayout.Isometric:
                case Grid.CellLayout.IsometricZAsY:
                case GridLayout.CellLayout.Rectangle:
                    {
                        var rotationDirection = direction == RotationDirection.Clockwise ? 90 : -90;
                        var rotationAxis = VP_GridManager.Instance.GetRotationAxisVector();
                        Quaternion orientation = Quaternion.Euler(rotationAxis * rotationDirection);
                        foreach (VP_GridTileBrushCell cell in m_cells)
                        {
                            cell.Orientation = cell.Orientation * orientation;
                        }
#if UNITY_EDITOR
                        // Rotate the selected tile in the collection (if any)
                        VP_TilePaletteGridTileBrushEditor.Instance.RotateSelectedTile(orientation);
#endif
                    }
                    break;
            }
        }

        public override void Flip(FlipAxis flip, Grid.CellLayout layout)
        {
            Debug.Log("Flip not supported");
        }

        public virtual void Reset()
        {
            UpdateSizeAndPivot(Vector3Int.one, Vector3Int.zero);
            m_moveStartPos = Vector3Int.zero;
        }

        public virtual void ResetPick()
        {
            Reset();
            PickedTile = false;

            if (m_editorCell != null && m_editorCell.GridTile != null)
            {
                m_cells[0] = m_editorCell;
            }
        }

        public virtual void UpdateSizeAndPivot(Vector3Int size, Vector3Int pivot)
        {
            m_size = size;
            m_pivot = pivot;
            SizeUpdated();
        }

        protected virtual void SizeUpdated()
        {
            m_cells = new VP_GridTileBrushCell[m_size.x * m_size.y * m_size.z];
            BoundsInt bounds = new BoundsInt(Vector3Int.zero, m_size);
            foreach (Vector3Int pos in bounds.allPositionsWithin)
            {
                m_cells[GetCellIndex(pos)] = new VP_GridTileBrushCell();
            }
        }

        public virtual void SetGridTile(Vector3Int position, VP_GridTile gt)
        {
            if (ValidateCellPosition(position))
                m_cells[GetCellIndex(position)].GridTile = gt;
        }

        public virtual void SetHeight(Vector3Int position, int height)
        {
            if (ValidateCellPosition(position))
                m_cells[GetCellIndex(position)].Height = height;
        }
          
        public virtual void SetGridTileType(Vector3Int position, GRID_TILE_TYPES _type)
        {
            if (ValidateCellPosition(position))
                m_cells[GetCellIndex(position)].TileType = _type;
        }
        public virtual void SetGridTileLayer(Vector3Int position, int _layer)
        {
            if (ValidateCellPosition(position))
                m_cells[GetCellIndex(position)].Layer = _layer;
        }

        public virtual void SetOffset(Vector3Int position, Vector3 offset)
        {
            if (ValidateCellPosition(position))
                m_cells[GetCellIndex(position)].Offset = offset;
        }

        public virtual void SetOrientation(Vector3Int position, Quaternion orientation)
        {
            if (ValidateCellPosition(position))
                m_cells[GetCellIndex(position)].Orientation = orientation;
        }

        public virtual int GetCellIndex(Vector3Int brushPosition)
        {
            return GetCellIndex(brushPosition.x, brushPosition.y, brushPosition.z);
        }

        public virtual int GetCellIndex(int x, int y, int z)
        {
            return x + m_size.x * y + m_size.x * m_size.y * z;
        }
        public virtual int GetCellIndex(int x, int y, int z, int sizex, int sizey, int sizez)
        {
            return x + sizex * y + sizex * sizey * z;
        }
        public virtual int GetCellIndexWrapAround(int x, int y, int z)
        {
            return (x % m_size.x) + m_size.x * (y % m_size.y) + m_size.x * m_size.y * (z % m_size.z);
        }

        protected virtual bool ValidateCellPosition(Vector3Int position)
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