using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDeleteMode : MonoBehaviour
{
    Player player;
    Vector2Int lastGridPosition = new Vector2Int(-1, -1);
    GridCell lastCell;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    void Update()
    {
        if (player == null || player.PlayerMode != Player.EPlayerMode.Delete) return;

        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            UI.Sound.PlayCancelMode();
            ExitDeleteMode();
            return;
        }

        if (Mouse.current.leftButton.isPressed)
        {
            DeleteAtCell(player.GridPositionUnderMouse);
        }
    }

    void DeleteAtCell(Vector2Int gridPosition)
    {
        if (!Game.ValidGridPosition(gridPosition))
        {
            Debug.LogWarning($"DeleteAtCell: invalid grid position {gridPosition}.");
            return;
        }

        GridCell cell = Game.Grid.Cell[gridPosition.x, gridPosition.y];
        if (cell == null)
        {
            Debug.LogWarning($"DeleteAtCell: Grid.Cell at {gridPosition} is null.");
            return;
        }

        if (lastCell == cell) return; // prevent multiple deletes on the same cell while holding mouse button

        if (cell.Node != null && cell.Node.Type != ENodeType.Gate)
        {
            Node nodeToDelete = cell.Node;

            // clear node reference from each grid cell that belonged to the node
            if (nodeToDelete.GridCells != null)
            {
                foreach (GridCell c in nodeToDelete.GridCells)
                {
                    if (c == null) continue;
                    if (c.Node == nodeToDelete) c.Node = null;
                }
            }

            if (nodeToDelete.NodeObject != null)
            {
                Destroy(nodeToDelete.NodeObject.gameObject);
                nodeToDelete.NodeObject = null;
            }

            if (Game.Nodes.Contains(nodeToDelete)) Game.Nodes.Remove(nodeToDelete);
            UI.Sound.PlayDeleteNode();
            lastCell = cell;
        }
        else if (cell.Road != null)
        {
            Component roadComponent = cell.Road as Component;
            if (roadComponent != null) Destroy(roadComponent.gameObject);
            cell.Road = null;
            UI.Sound.PlayDeleteNode();
            lastCell= cell;
        }
    }

    internal void EnterDeleteMode()
    {
        lastGridPosition = new Vector2Int(-1, -1);
        lastCell = null;
        Game.Grid.ShowDeleteGrid(true);
    }

    void ExitDeleteMode()
    {
        Game.TurnOffDeleteMode();
        player.ExitMode();
    }
}
