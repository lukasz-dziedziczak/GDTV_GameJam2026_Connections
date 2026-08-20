using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[System.Serializable]
public class Grid
{
    public Grid(Vector2Int gridSize)
    {
        Cell = new GridCell[gridSize.x, gridSize.y];
    }

    public GridCell[,] Cell;
    float cellSize;
    List<GridCell> selectedCells = new List<GridCell>();
    List<Node> selectedNodes = new List<Node>();
    List<GridCell> showingConnection = new List<GridCell>();
    GridCell mouseOverCell;
    Node mouseOverNode;

    public bool ShowMouseOverCell;

    public void SelectCell(Vector2Int gridPosition)
    {
        if (gridPosition.x == -1 && gridPosition.y == -1) return;
        var target = Cell[gridPosition.x, gridPosition.y];
        if (target == null) return;

        if (target.Node != null)
        {
            var node = target.Node;
            if (selectedNodes.Contains(node)) return;
            if (node.NodeObject != null)
            {
                node.NodeObject.SetSelected(true);
                selectedNodes.Add(node);
            }
            return;
        }

        if (selectedCells.Contains(target)) return;
        target.Select();
        selectedCells.Add(target);
    }

    public void DeselectCell(Vector2Int gridPosition)
    {
        if (gridPosition.x == -1 && gridPosition.y == -1) return;
        var target = Cell[gridPosition.x, gridPosition.y];
        if (target == null) return;

        if (target.Node != null && selectedNodes.Contains(target.Node))
        {
            var node = target.Node;
            if (node.NodeObject != null) node.NodeObject.SetSelected(false);
            selectedNodes.Remove(node);
            return;
        }

        if (selectedCells.Contains(target))
        {
            target.Deselect();
            selectedCells.Remove(target);
        }
    }

    public void DeselectAll()
    {
        for (int i = selectedNodes.Count - 1; i >= 0; i--)
        {
            var node = selectedNodes[i];
            if (node != null && node.NodeObject != null) node.NodeObject.SetSelected(false);
        }
        selectedNodes.Clear();

        for (int i = selectedCells.Count - 1; i >= 0; i--)
        {
            var cell = selectedCells[i];
            if (cell != null) cell.Deselect();
        }
        selectedCells.Clear();
    }

    public List<GridCell> NeighbourCells(GridCell gridCell)
    {
        List<GridCell> neighbours = new List<GridCell>();
        Vector2Int pos = gridCell.GridPosition;
        int width = Cell.GetLength(0);
        int height = Cell.GetLength(1);

        if (pos.y + 1 < height)
            neighbours.Add(Cell[pos.x, pos.y + 1]);
        if (pos.y - 1 >= 0)
            neighbours.Add(Cell[pos.x, pos.y - 1]);
        if (pos.x - 1 >= 0)
            neighbours.Add(Cell[pos.x - 1, pos.y]);
        if (pos.x + 1 < width)
            neighbours.Add(Cell[pos.x + 1, pos.y]);

        return neighbours;
    }

    public void SetMouseOverCell(Vector2Int gridPosition)
    {
        //if (Game.Player.PlayerMode == Player.EPlayerMode.Delete) return;

        GridCell newCell = null;
        Node newNode = null;

        if (gridPosition.x != -1 && gridPosition.y != -1)
        {
            if (gridPosition.x >= 0 && gridPosition.x < Cell.GetLength(0) && gridPosition.y >= 0 && gridPosition.y < Cell.GetLength(1))
            {
                newCell = Cell[gridPosition.x, gridPosition.y];
                if (newCell != null) newNode = newCell.Node;
            }
        }

        if (mouseOverNode != null && mouseOverNode != newNode)
        {
            mouseOverNode.NodeObject?.SetMouseOver(false);
            mouseOverNode.ShowConnectionIndicators(false);
            mouseOverNode = null;
        }

        if (mouseOverCell != null && mouseOverCell != newCell)
        {
            mouseOverCell.SetMouseOver(false);
            mouseOverCell = null;
        }

        if (newNode != null)
        {
            if (mouseOverNode != newNode)
            {
                newNode.NodeObject?.SetMouseOver(true);
                newNode.ShowConnectionIndicators();
                mouseOverNode = newNode;
                UI.Sound.PlayMouseOverBuilding();
            }
        }
        else if (newCell != null && (ShowMouseOverCell || Game.Player.PlayerMode == Player.EPlayerMode.Delete))
        {
            if (mouseOverCell != newCell)
            {
                newCell.SetMouseOver(true);
                mouseOverCell = newCell;
            }
        }

        UI.UpdatePositionUnderMouse(gridPosition);
    }

    public void ShowGrid(bool show)
    {
        for (int x = 0; x < Cell.GetLength(0); x++)
        {
            for (int y = 0; y < Cell.GetLength(1); y++)
            {
                if (Cell[x, y] != null && Cell[x, y].GridObject != null)
                {
                    Cell[x, y].GridObject.ShowGridImage(show);
                }
            }
        }
    }

    public void ShowDeleteGrid(bool show)
    {
        for (int x = 0; x < Cell.GetLength(0); x++)
        {
            for (int y = 0; y < Cell.GetLength(1); y++)
            {
                if (Cell[x, y] != null && Cell[x, y].GridObject != null)
                {
                    Cell[x, y].GridObject.SetDeleteing(show);
                }
            }
        }
    }

    public void ShowConnection(GridCell cell)
    {
        if (cell == null || cell.GridObject == null || cell.Road == null) return;

        cell.GridObject.SetConnection(true);
        showingConnection.Add(cell);
    }

    public void HideConnection(GridCell cell)
    {
        if (cell == null || cell.GridObject == null || cell.Road == null || !showingConnection.Contains(cell)) return;
        cell.GridObject.SetConnection(false);
        showingConnection.Remove(cell);
    }

    public void ClearAllConnections()
    {
        foreach (GridCell cell in showingConnection)
        {
            cell.GridObject.SetConnection(false);
        }
        showingConnection.Clear();

        foreach (Node node in Game.Nodes)
        {
            node.NodeObject.SetConnection(false);
        }
    }

    public bool IsShowingConnection(GridCell cell)
    {
        return showingConnection.Contains(cell);
    }

    public bool IsShowingConnection(Vector2Int gridPosition)
    {
        if (!Game.ValidGridPosition(gridPosition)) return false;
        var target = Cell[gridPosition.x, gridPosition.y];
        if (target == null) return false;
        return showingConnection.Contains(target);
    }

    public void AddCells(List<GridCell> cells)
    {
        if (cells == null || cells.Count == 0) return;

        int width = Cell != null ? Cell.GetLength(0) : 0;
        int height = Cell != null ? Cell.GetLength(1) : 0;
        int maxX = width - 1;
        int maxY = height - 1;
        bool needsResize = false;

        foreach (GridCell c in cells)
        {
            if (c == null) continue;
            Vector2Int p = c.GridPosition;
            if (p.x < 0 || p.y < 0)
            {
                Debug.LogWarning($"AddCells: cell position {p} has negative coordinate; skipping.");
                continue;
            }
            if (p.x > maxX)
            {
                maxX = p.x;
                needsResize = true;
            }
            if (p.y > maxY)
            {
                maxY = p.y;
                needsResize = true;
            }
        }

        if (!needsResize)
        {
            foreach (GridCell c in cells)
            {
                if (c == null) continue;
                Vector2Int p = c.GridPosition;
                if (p.x >= 0 && p.x < width && p.y >= 0 && p.y < height)
                {
                    Cell[p.x, p.y] = c;
                    if (c.GridObject != null) c.GridObject.SetGridPosition(c.GridPosition);
                }
            }
            return;
        }

        int newWidth = maxX + 1;
        int newHeight = maxY + 1;
        GridCell[,] newCells = new GridCell[newWidth, newHeight];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                newCells[x, y] = Cell[x, y];
            }
        }

        foreach (GridCell c in cells)
        {
            if (c == null) continue;
            Vector2Int p = c.GridPosition;
            if (p.x < 0 || p.y < 0) continue;
            newCells[p.x, p.y] = c;
            if (c.GridObject != null) c.GridObject.SetGridPosition(c.GridPosition);
        }

        Cell = newCells;
    }

    public void TurnOffDeleteMode()
    {
        foreach(GridCell cell in Cell)
        {
            if (cell != null && cell.GridObject != null)
            {
                cell.GridObject.SetMouseOver(false);
                cell.GridObject.SetDeleteing(false);
            }
        }
    }
}

