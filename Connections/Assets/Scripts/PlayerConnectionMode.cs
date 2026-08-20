using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerConnectionMode : MonoBehaviour
{
    Player player;
    [SerializeField] ConnectionIndicator connectionIndicatorPrefab;

    public ConnectionIndicator IndicatorPrefab => connectionIndicatorPrefab;


    NodeConnection nodeConnection;
    Vector2Int lastGridPosition = new Vector2Int(-1, -1);

    public NodeConnection NodeConnection => nodeConnection;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (player.PlayerMode != Player.EPlayerMode.Connection || nodeConnection == null) return;

        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            CancelConnectionMode();
        }

        else
        {
            Vector2Int gridPosition = player.GridPositionUnderMouse;
            if (lastGridPosition != gridPosition && Game.ValidGridPosition(gridPosition))
            {
                GridCell cell = Game.Grid.Cell[gridPosition.x, gridPosition.y];
                if (cell == null) return;
                lastGridPosition = gridPosition;
                if (!nodeConnection.TryConnectRoad(cell) && 
                    nodeConnection.TrySetOutNode(cell.Node))
                {
                    CancelConnectionMode();
                }
            }
        }

        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            CancelConnectionMode();
        }
    }

    private static void SetAllNodesToConnectionMode()
    {
        foreach (Node node in Game.Nodes)
        {
            node.NodeObject.SetConnection(true);
        }
    }

    private void CancelConnectionMode()
    {
        Game.Grid.ClearAllConnections();
        if (nodeConnection != null)
        {
            if (nodeConnection.RecievingNode == null && nodeConnection.ConnectionIndicator != null)
            {
                Destroy(nodeConnection.ConnectionIndicator.gameObject);
                UI.Sound.PlayCancelMode();
            }
            nodeConnection = null;
        }

        player.ExitMode();
    }

    internal void EnterConnectionMode(NodeConnection nodeConnection)
    {
        Game.HideConnectionIndictors();
        this.nodeConnection = nodeConnection;

        /*foreach (Node node in Game.Nodes)
        {
            node.NodeObject.SetConnection(true);
        }*/
    }

    private void SpawnIndicator()
    {
        if (nodeConnection != null && nodeConnection.ConnectionIndicator == null)
        {
            nodeConnection.SetConnectionIndicator(Instantiate(connectionIndicatorPrefab, nodeConnection.SendingNode.WorldPosition, Quaternion.identity));
        }
    }
}