using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Game : MonoBehaviour
{
    public static Game Instance;

    [SerializeField] float cellSize = 1f;
    [SerializeField] Vector2Int gridSize = new Vector2Int(10, 10);
    [SerializeField] GridObject gridObjectPrefab;
    [SerializeField] Grid grid;
    [SerializeField] NodeObject nodeObjectPrefab;
    [SerializeField] List<Node> nodes = new List<Node>();
    [SerializeField] Vector2Int gatePosition;
    [SerializeField] NodeConfig gateConfig;
    [SerializeField] Vector2 playerPositionOffset;
    Vector2Int currentlySelected = new Vector2Int(-1, -1);
    [SerializeField] bool bSpawnGridObjects;
    [SerializeField] Player player;
    [SerializeField] Pesant pesantPrefab;
    [SerializeField] LevelConfig overrideLevelConfig;
    [SerializeField] LevelManager levelManagerPrefab;
    [SerializeField] List<NodeConnection> connections = new List<NodeConnection>();

    Node gateNode;
    LevelConfig levelConfig;

    public static Grid Grid => Instance != null ? Instance.grid : null;
    public static Player Player => Instance != null ? Instance.player : null;
    public static List<Node> Nodes => Instance != null ? Instance.nodes : null;
    public static List<NodeConnection> Connections => Instance != null ? Instance.connections : null;
    public static Node GateNode => Instance != null ? Instance.gateNode : null;
    public static LevelConfig LevelConfig => Instance != null ? Instance.levelConfig : null;
    public static bool MouseOverUI;

    private void Start()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (overrideLevelConfig != null) levelConfig = overrideLevelConfig;
        else
        {
            if (LevelManager.Instance == null && levelManagerPrefab != null)
            {
                Instantiate(levelManagerPrefab);
                LevelManager.SetLevelIndex(0);
            }

            levelConfig = LevelManager.CurrentLevelConfig;
        }

        if (levelConfig == null)
        {
            Debug.LogError("LevelConfig is not set.");
            return;
        }

        if (levelConfig.RequiredResources == null || levelConfig.RequiredResources.Count == 0) {
            Debug.LogError("LevelConfig.RequiredResources is not set or empty.");
            return;
        }

        CreateGrid();
        CenterPlayerPosition();
        AddGateNode();

        UI.UpdateObjectives();
        UI.BuildMenu.InitializeForLevel();
    }

    private void Update()
    {
        foreach (Node node in nodes)
        {
            node.NodeTick(Time.deltaTime);
        }

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TogglePause();
        }
    }

    private void CenterPlayerPosition()
    {
        if (player != null)
        {
            player.transform.position = new Vector3(
                ((gridSize.x - 1) * cellSize / 2f) + playerPositionOffset.x,
                player.transform.position.y,
                ((gridSize.y - 1) * cellSize / 2f) + playerPositionOffset.y
            );
        }
    }

    private void CreateGrid()
    {
        grid = new Grid(gridSize);

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                grid.Cell[x, y] = new GridCell(x, y, cellSize);
                if (bSpawnGridObjects) grid.Cell[x, y].SetGridObject(Instantiate(gridObjectPrefab, grid.Cell[x, y].WorldPosition, Quaternion.identity));
            }
        }
    }

    public static void SelectCell(Vector2Int newSelection)
    {
        if (Instance == null) return;
        Instance.grid.SelectCell(newSelection);
    }

    public static Vector2Int GetGridPositionFromWorldPosition(Vector3 worldPosition)
    {
        if (Instance == null) return new Vector2Int(-1, -1);
        int x = Mathf.FloorToInt(worldPosition.x / Instance.cellSize);
        int y = Mathf.FloorToInt(worldPosition.z / Instance.cellSize);
        if (x >= 0 && x < Instance.gridSize.x && y >= 0 && y < Instance.gridSize.y)
        {
            return new Vector2Int(x, y);
        }

        if (Instance.gateNode != null && Instance.gateNode.GridCells != null)
        {
            float half = Instance.cellSize / 2f;
            foreach (GridCell c in Instance.gateNode.GridCells)
            {
                if (c == null) continue;
                Vector3 wp = c.WorldPosition;
                if (worldPosition.x >= wp.x - half && worldPosition.x < wp.x + half &&
                    worldPosition.z >= wp.z - half && worldPosition.z < wp.z + half)
                {
                    return c.GridPosition;
                }
            }
        }

        return new Vector2Int(-1, -1);
    }

    public static void SelectCellFromWorldPosition(Vector3 worldPosition)
    {
        Vector2Int cellPosition = GetGridPositionFromWorldPosition(worldPosition);
        SelectCell(cellPosition);
    }

    public static void AddConnection(NodeConnection connection)
    {
        if (Instance == null)
        {
            Debug.LogWarning("Game.Instance is null. Cannot add connection.");
            return;
        }

        Node sendingNode = connection.SendingNode;
        if (sendingNode != null)
        {
            for (int i = Instance.connections.Count - 1; i >= 0; i--)
            {
                NodeConnection existingConnection = Instance.connections[i];
                if (existingConnection.SendingNode == sendingNode && existingConnection.RecievingNode != connection.RecievingNode)
                {
                    Node oldReceiver = existingConnection.RecievingNode;
                    if (sendingNode != null && sendingNode.ConnectedNodes.Contains(oldReceiver))
                    {
                        sendingNode.ConnectedNodes.Remove(oldReceiver);
                    }
                    if (oldReceiver != null && oldReceiver.ConnectedNodes.Contains(sendingNode))
                    {
                        oldReceiver.ConnectedNodes.Remove(sendingNode);
                    }
                    Instance.connections.RemoveAt(i);
                }
            }
        }

        Instance.connections.Add(connection);
    }

    public static bool ValidGridPosition(Vector2Int gridPosition)
    {
        if (Instance == null) return false;

        if (Instance.grid != null && Instance.grid.Cell != null)
        {
            int width = Instance.grid.Cell.GetLength(0);
            int height = Instance.grid.Cell.GetLength(1);
            return gridPosition.x >= 0 && gridPosition.x < width && gridPosition.y >= 0 && gridPosition.y < height;
        }

        return gridPosition.x >= 0 && gridPosition.x < Instance.gridSize.x && gridPosition.y >= 0 && gridPosition.y < Instance.gridSize.y;
    }

    public static bool GameComplete
    {
        get
        {
            return GateNode != null && GateNode.Satisfied;
        }
    }

    public static void SpawnNode(NodeConfig nodeConfig, Vector2Int gridPosition)
    {
        if (Instance == null) return;
        if (nodeConfig == null)
        {
            Debug.LogWarning("SpawnNode: nodeConfig is null.");
            return;
        }

        if (!ValidGridPosition(gridPosition))
        {
            Debug.LogWarning($"SpawnNode: origin {gridPosition} is out of bounds.");
            return;
        }

        // Validate footprint fits and cells are free
        Vector2Int size = nodeConfig.Size;
        for (int dx = 0; dx < Mathf.Max(1, size.x); dx++)
        {
            for (int dy = 0; dy < Mathf.Max(1, size.y); dy++)
            {
                Vector2Int pos = new Vector2Int(gridPosition.x + dx, gridPosition.y + dy);
                if (!ValidGridPosition(pos))
                {
                    Debug.LogWarning($"SpawnNode: footprint cell {pos} out of bounds. Aborting spawn.");
                    return;
                }

                if (Instance.grid.Cell[pos.x, pos.y].Node != null)
                {
                    Debug.LogWarning($"SpawnNode: footprint cell {pos} is already occupied. Aborting spawn.");
                    return;
                }
            }
        }

        // Create node (Node constructor assumes provided cell is bottom-left origin)
        Node node = new Node(Instance.grid.Cell[gridPosition.x, gridPosition.y], nodeConfig);

        // Compute center world position of occupied cells
        Vector3 center = Vector3.zero;
        int count = 0;
        foreach (var c in node.GridCells)
        {
            if (c == null) continue;
            center += c.WorldPosition;
            count++;
        }
        if (count > 0) center /= count;
        else center = Instance.grid.Cell[gridPosition.x, gridPosition.y].WorldPosition;

        // Instantiate visual object
        if (nodeConfig.ObjectPrefab != null)
        {
            node.NodeObject = Instantiate(nodeConfig.ObjectPrefab, center, Quaternion.identity);
        }

        Instance.nodes.Add(node);
    }

    public static Pesant SpawnPesant(Node startingNode, Node destinationNode, NodeResource nodeResource)
    {
        if (Instance == null) return null;
        if (startingNode == null || destinationNode == null) return null;
        if (Instance.pesantPrefab == null) return null;
        NodeConnection found = null;
        bool reversed = false;
        for (int i = 0; i < Instance.connections.Count; i++)
        {
            var c = Instance.connections[i];
            if (c == null) continue;
            if (c.SendingNode == startingNode && c.RecievingNode == destinationNode)
            {
                found = c;
                reversed = false;
                break;
            }
            if (c.SendingNode == destinationNode && c.RecievingNode == startingNode)
            {
                found = c;
                reversed = true;
                Debug.Log("Found reversed connection.");
                break;
            }
        }

        if (found == null)
        {
            Debug.LogWarning("No connection found between the specified nodes.");
            return null;
        }

        if (found.Path == null || found.Path.Count == 0)
        {
            Debug.LogWarning("The found connection has no path.");
            return null;
        }

        List<GridCell> path = new List<GridCell>(found.Path);
        if (reversed) path.Reverse();

        Vector3 spawnPos = startingNode.WorldPosition;
        foreach(GridCell cell in startingNode.GridCells)
        {
            if (path[0].NeighbourCells.Contains(cell))
            {
                spawnPos = cell.WorldPosition;
                break;
            }
        }
        
        Pesant pesant = Instantiate(Instance.pesantPrefab, spawnPos, Quaternion.identity);
        if (pesant == null) return null;

        // make the peasant face the first path position
        if (path != null && path.Count > 0)
        {
            Vector3 firstTarget = path[0].WorldPosition;
            Vector3 dir = firstTarget - spawnPos;
            dir.y = 0f;
            if (dir.sqrMagnitude > 0.0001f)
            {
                pesant.transform.rotation = Quaternion.LookRotation(dir);
            }
        }

        if (nodeResource.Config != null && nodeResource.Amount > 0)
        {
            pesant.Inventory.Add(new NodeResource(nodeResource.Config, nodeResource.Amount));
        }

        pesant.Path = path;
        pesant.DestinationNode = destinationNode;
        pesant.OriginNode = startingNode;
        return pesant;
    }

    public static List<Node> AllConsumerNodes
    {
        get
        {
            List<Node> nodes = new List<Node>();
            foreach (Node node in Instance.nodes)
            {
                if (node.Type == ENodeType.Consumer) nodes.Add(node);
            }
            return nodes;
        }
    }

    public static void TogglePause()
    {
        if (!UI.PauseMenu.gameObject.activeSelf)
        {
            UI.Sound.PlayPauseGame();
            UI.PauseMenu.gameObject.SetActive(true);
            Time.timeScale = 0f;
        }
        else
        {
            UI.PauseMenu.gameObject.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    void AddGateNode()
    {
        if (gateConfig == null) return;

        Vector2Int size = gateConfig.Size;
        List<GridCell> cellsToAdd = new List<GridCell>();

        // collect missing footprint cells
        for (int dx = 0; dx < Mathf.Max(1, size.x); dx++)
        {
            for (int dy = 0; dy < Mathf.Max(1, size.y); dy++)
            {
                int x = gatePosition.x + dx;
                int y = gatePosition.y + dy;
                if (x < 0 || y < 0) continue;

                GridCell existing = null;
                if (grid != null && grid.Cell != null)
                {
                    int w = grid.Cell.GetLength(0);
                    int h = grid.Cell.GetLength(1);
                    if (x >= 0 && x < w && y >= 0 && y < h)
                    {
                        existing = grid.Cell[x, y];
                    }
                }

                if (existing == null)
                {
                    cellsToAdd.Add(new GridCell(x, y, cellSize));
                }
            }
        }

        // add new cells into the grid array (will resize if needed)
        if (cellsToAdd.Count > 0)
        {
            grid.AddCells(cellsToAdd);
        }

        // ensure grid objects exist for the whole footprint and position them from the cell's WorldPosition
        for (int dx = 0; dx < Mathf.Max(1, size.x); dx++)
        {
            for (int dy = 0; dy < Mathf.Max(1, size.y); dy++)
            {
                int x = gatePosition.x + dx;
                int y = gatePosition.y + dy;
                if (x < 0 || y < 0) continue;

                GridCell cell = null;
                if (grid != null && grid.Cell != null)
                {
                    int w = grid.Cell.GetLength(0);
                    int h = grid.Cell.GetLength(1);
                    if (x >= 0 && x < w && y >= 0 && y < h) cell = grid.Cell[x, y];
                }

                if (cell != null && bSpawnGridObjects && gridObjectPrefab != null && cell.GridObject == null)
                {
                    GridObject go = Instantiate(gridObjectPrefab, cell.WorldPosition, Quaternion.identity, Instance.transform);
                    cell.SetGridObject(go);
                }
                else if (cell != null && cell.GridObject != null)
                {
                    cell.GridObject.transform.SetParent(Instance.transform, true);
                    cell.GridObject.transform.position = cell.WorldPosition;
                    cell.GridObject.SetGridPosition(cell.GridPosition);
                }
            }
        }

        // pick an origin cell inside the (possibly resized) grid if available
        GridCell originCell = null;
        if (grid != null && grid.Cell != null)
        {
            int w = grid.Cell.GetLength(0);
            int h = grid.Cell.GetLength(1);
            if (gatePosition.x >= 0 && gatePosition.x < w && gatePosition.y >= 0 && gatePosition.y < h)
            {
                originCell = grid.Cell[gatePosition.x, gatePosition.y];
            }
        }

        // if origin not available, try first added cell
        if (originCell == null && cellsToAdd.Count > 0)
        {
            var p = cellsToAdd[0].GridPosition;
            originCell = grid.Cell[p.x, p.y];
        }

        // create node using a valid origin cell when possible
        Node node = new Node(originCell, gateConfig);

        // fallback: if constructor didn't attach footprint (e.g., originCell was null), attach manually
        if (node.GridCells == null || node.GridCells.Count == 0)
        {
            for (int dx = 0; dx < Mathf.Max(1, size.x); dx++)
            {
                for (int dy = 0; dy < Mathf.Max(1, size.y); dy++)
                {
                    int x = gatePosition.x + dx;
                    int y = gatePosition.y + dy;
                    if (x < 0 || y < 0) continue;

                    if (grid != null && grid.Cell != null)
                    {
                        int w = grid.Cell.GetLength(0);
                        int h = grid.Cell.GetLength(1);
                        if (x >= 0 && x < w && y >= 0 && y < h)
                        {
                            var c = grid.Cell[x, y];
                            if (c != null && !node.GridCells.Contains(c))
                            {
                                node.GridCells.Add(c);
                                c.Node = node;
                            }
                        }
                    }
                }
            }
        }

        // compute spawn position using node.WorldPosition (don't mutate grid indices)
        Vector3 spawnPos = node.WorldPosition;
        if (spawnPos == Vector3.zero)
        {
            spawnPos = new Vector3(gatePosition.x * cellSize + cellSize / 2f, 0f, gatePosition.y * cellSize + cellSize / 2f);
        }

        if (gateConfig.ObjectPrefab != null)
        {
            node.NodeObject = Instantiate(gateConfig.ObjectPrefab, spawnPos, Quaternion.identity);
        }

        nodes.Add(node);
        gateNode = node;
    }
    
    public static ConnectionIndicator ConnectionIndicatorPrefab
    {
        get
        {
            return Instance != null && Instance.player != null && Instance.player.ConnectionMode.IndicatorPrefab != null ? 
                Instance.player.ConnectionMode.IndicatorPrefab : null;
        }
    }

    public static void HideConnectionIndictors()
    {
        foreach(NodeConnection nodeConnection in Connections)
        {
            nodeConnection.Hide();
        }
    }

    public static void TurnOffDeleteMode()
    {
        foreach (Node node in Nodes)
        {
            if (node.GridCells == null) continue;
            foreach (GridCell cell in node.GridCells)
            {
                if (cell == null || cell.GridObject == null) continue;
                cell.GridObject.SetDeleteing(false);
                node.NodeObject.SetDelete(false);
                node.NodeObject.SetMouseOver(false);
            }
        }
        Instance.grid.TurnOffDeleteMode();
    }
}