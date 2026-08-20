using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public enum EPlayerMode
    {
        None,
        Build,
        Connection,
        Delete
    }

    [SerializeField] Camera cam;
    [field: SerializeField] public PlayerBuildMode BuildMode {  get; private set; }
    [field: SerializeField] public PlayerConnectionMode ConnectionMode {  get; private set; }
    [field: SerializeField] public PlayerDeleteMode DeleteMode {  get; private set; }


    bool bSelecting = false;
    [field: SerializeField] public EPlayerMode PlayerMode {  get; private set; }

    private void Update()
    {
        if (Game.Instance == null || Game.Grid == null) return;
        if (Game.MouseOverUI || UI.MenuOpen) return;
        
        Game.Grid.SetMouseOverCell(GridPositionUnderMouse);

        if (PlayerMode != EPlayerMode.None) return;

        if (Mouse.current.leftButton.wasPressedThisFrame) OnMouseClick();
        else if (Mouse.current.leftButton.wasReleasedThisFrame) OnMouseRelease();

    }

    public Vector2Int GridPositionUnderMouse
    {
        get
        {
            return Game.GetGridPositionFromWorldPosition(GetMouseWorldPosition());
        }
    }

    public Vector3 GetMouseWorldPosition()
    {
        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
        Ray ray = cam.ScreenPointToRay(mouseScreenPosition);
        Plane plane = new Plane(Vector3.up, Vector3.zero);
        float distance;
        if (plane.Raycast(ray, out distance))
        {
            return ray.GetPoint(distance);
        }
        return Vector3.zero;
    }

    void OnMouseClick()
    {
        Node clickedNode = NodeUnderMouse;
        if (clickedNode != null)
        {
            EnterConnectionMode(clickedNode);
        }
    }

    void OnMouseRelease()
    {
        Game.HideConnectionIndictors();
    }

    Node NodeUnderMouse
    {
        get
        {
            Vector2Int gridPosition = Game.GetGridPositionFromWorldPosition(GetMouseWorldPosition());
            if (!Game.ValidGridPosition(gridPosition)) return null;
            return Game.Grid.Cell[gridPosition.x, gridPosition.y].Node;
        }
    }

    public void EnterBuildMode(BuildMenuItem buildMenuItem)
    {
        BuildMode.EnterBuildMode(buildMenuItem);
        PlayerMode = EPlayerMode.Build;
        Game.Grid.ShowGrid(true);
        UI.UpdateButtonBackground();
    }

    public void ExitMode()
    {
        PlayerMode = EPlayerMode.None;
        Game.Grid.ShowGrid(false);
        UI.UpdateButtonBackground();
    }

    public void EnterConnectionMode(Node startNode)
    {
        PlayerMode = EPlayerMode.Connection;
        ConnectionMode.EnterConnectionMode(new NodeConnection(startNode));
        UI.UpdateButtonBackground();
    }

    public void EnterDeleteMode()
    {
        PlayerMode = EPlayerMode.Delete;
        DeleteMode.EnterDeleteMode();
        UI.UpdateButtonBackground();
    }
    
}
