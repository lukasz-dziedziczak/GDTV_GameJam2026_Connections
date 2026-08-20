using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBuildMode : MonoBehaviour
{
    Player player;

    [field: SerializeField] public BuildMenuItem BuildMenuItem { get; private set; }

    Road connectionObject;
    NodeObject nodeObject;
    bool hasPlacedFirstRoad = false;

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
        if (player.PlayerMode != Player.EPlayerMode.Build) return;

        if (connectionObject != null)
        {
            Vector2Int mouseGridPos = player.GridPositionUnderMouse;
            if (Game.ValidGridPosition(mouseGridPos))
            {
                Vector3 mouseWorldPos = Game.Grid.Cell[mouseGridPos.x, mouseGridPos.y].WorldPosition;
                connectionObject.transform.position = mouseWorldPos;

                if (Mouse.current.leftButton.isPressed)
                {
                    CompleteConnectionObjectPlacement(mouseGridPos);
                }
            }
            else connectionObject.transform.position = new Vector3(0, 200, 0);

            if (Mouse.current.leftButton.wasReleasedThisFrame & hasPlacedFirstRoad && !Keyboard.current.shiftKey.isPressed)
            {
                UI.Sound.PlayNodePlace();
                CancelBuildMode();
            }
        }

        if (nodeObject != null)
        {
            Vector2Int mouseGridPos = player.GridPositionUnderMouse;
            if (Game.ValidGridPosition(mouseGridPos) && BuildMenuItem != null && BuildMenuItem.NodeConfig != null)
            {
                Vector2Int size = BuildMenuItem.NodeConfig.Size;
                int sx = Mathf.Max(1, size.x);
                int sy = Mathf.Max(1, size.y);

                bool valid = true;
                Vector3 center = Vector3.zero;
                int count = 0;

                for (int dx = 0; dx < sx; dx++)
                {
                    for (int dy = 0; dy < sy; dy++)
                    {
                        Vector2Int pos = new Vector2Int(mouseGridPos.x + dx, mouseGridPos.y + dy);
                        if (!Game.ValidGridPosition(pos))
                        {
                            valid = false;
                            break;
                        }
                        GridCell cell = Game.Grid.Cell[pos.x, pos.y];
                        if (cell == null)
                        {
                            valid = false;
                            break;
                        }
                        center += cell.WorldPosition;
                        count++;
                    }
                    if (!valid) break;
                }

                if (valid && count > 0)
                {
                    center /= count;
                    nodeObject.transform.position = center;
                    if (Mouse.current.leftButton.isPressed)
                    {
                        CompleteNodeObjectPlacement(mouseGridPos);
                    }
                }
                else
                {
                    nodeObject.transform.position = new Vector3(0, 200, 0);
                }
            }
            else
            {
                nodeObject.transform.position = new Vector3(0, 200, 0);
            }
        }

        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            UI.Sound.PlayCancelMode();
            CancelBuildMode();
        }
    }

    private void CompleteNodeObjectPlacement(Vector2Int mouseGridPos)
    {
        if (BuildMenuItem == null || BuildMenuItem.NodeConfig == null) return;

        Vector2Int size = BuildMenuItem.NodeConfig.Size;
        int sx = Mathf.Max(1, size.x);
        int sy = Mathf.Max(1, size.y);

        for (int dx = 0; dx < sx; dx++)
        {
            for (int dy = 0; dy < sy; dy++)
            {
                Vector2Int pos = new Vector2Int(mouseGridPos.x + dx, mouseGridPos.y + dy);
                if (!Game.ValidGridPosition(pos)) return;

                GridCell cell = Game.Grid.Cell[pos.x, pos.y];
                if (cell == null || cell.Road != null || cell.Node != null) return;
            }
        }

        GridCell originCell = Game.Grid.Cell[mouseGridPos.x, mouseGridPos.y];
        nodeObject.SetSelected(false);
        Node node = new Node(originCell, BuildMenuItem.NodeConfig);
        node.NodeObject = nodeObject;
        nodeObject.SetNode(node);
        Game.Nodes.Add(node);
        if (!Keyboard.current.shiftKey.isPressed)
        {
            nodeObject = null;
            BuildMenuItem = null;
            player.ExitMode();
        }
        else
        {
            nodeObject = Instantiate(BuildMenuItem.NodeConfig.ObjectPrefab, new Vector3(0, 200, 0), Quaternion.identity);
            nodeObject.SetSelected(true);
        }

        UI.Sound.PlayNodePlace();
    }

    private void CancelBuildMode()
    {
        if (connectionObject != null)
        {
            Destroy(connectionObject.gameObject);
            connectionObject = null;
        }

        if (nodeObject != null)
        {
            Destroy(nodeObject.gameObject);
            nodeObject = null;
        }

        hasPlacedFirstRoad = false;
        BuildMenuItem = null;
        player.ExitMode();
    }

    public void EnterBuildMode(BuildMenuItem buildMenuItem)
    {
        if (player.PlayerMode == Player.EPlayerMode.Build)
        {
            CancelBuildMode();
        }

        BuildMenuItem = buildMenuItem;

        if (BuildMenuItem.ConnectionObjectPrefab != null)
        {
            Vector3 pos = new Vector3(0, 200, 0);
            connectionObject = Instantiate(BuildMenuItem.ConnectionObjectPrefab, pos, Quaternion.identity);
            connectionObject.SetGhost(true);
        }
        else if (BuildMenuItem.NodeConfig != null)
        {
            nodeObject = Instantiate(BuildMenuItem.NodeConfig.ObjectPrefab, new Vector3(0, 200, 0), Quaternion.identity);
            nodeObject.SetSelected(true);
        }


    }

    void CompleteConnectionObjectPlacement(Vector2Int mouseGridPos)
    {
        GridCell cell = Game.Grid.Cell[mouseGridPos.x, mouseGridPos.y];

        if (cell.Road != null || cell.Node != null) return;

        connectionObject.SetGhost(false);
        cell.Road = connectionObject;
        hasPlacedFirstRoad = true;

        Vector3 pos = new Vector3(0, 200, 0);
        connectionObject = Instantiate(BuildMenuItem.ConnectionObjectPrefab, cell.WorldPosition, Quaternion.identity);

        UI.Sound.PlayRoadPlace();
    }
}
