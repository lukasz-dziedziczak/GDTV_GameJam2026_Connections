using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GridCell
{
    [field: SerializeField] public Vector2Int GridPosition { get; private set; }

    [field: SerializeField] public GridObject GridObject { get; private set; }

    float cellSize;

    public Node Node;
    public Road Road;

    [field: SerializeField] public bool IsSelected { get; private set; }

    public GridCell(int x, int  y, float newCellSize)
    {
        GridPosition = new Vector2Int(x, y);
        cellSize = newCellSize;
    }

    public void SetGridObject(GridObject gridObject)
    {
        GridObject = gridObject;
        GridObject.SetGridPosition(GridPosition);
    }

    public Vector3 WorldPosition
    {
        get
        {
            return new Vector3(
                GridPosition.x * cellSize + cellSize / 2,
                0,
                GridPosition.y * cellSize + cellSize / 2);
        }
    }

    public void Select()
    {
        IsSelected = true;

        if (GridObject != null)
        {
            GridObject.SetSelected(true);
        }

        if (Node != null && Node.NodeObject != null)
        {
            Node.NodeObject.SetSelected(true);
        }
    }

    public void Deselect()
    {
        IsSelected = false;

        if (GridObject != null)
        {
            GridObject.SetSelected(false);
        }

        if (Node != null && Node.NodeObject != null)
        {
            Node.NodeObject.SetSelected(false);
        }
    }

    public List<GridCell> NeighbourCells
    {
        get
        {
            return Game.Grid.NeighbourCells(this);
        }
    }

    public void SetMouseOver(bool isMouseOver)
    {
        if (GridObject != null)
        {
            GridObject.SetMouseOver(isMouseOver);
        }
    }
}