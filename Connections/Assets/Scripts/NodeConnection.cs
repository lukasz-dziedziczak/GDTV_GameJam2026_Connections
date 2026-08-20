using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

[System.Serializable]
public class NodeConnection
{
    public Node SendingNode { get; private set; }
    public Node RecievingNode { get; private set; }
    
    public List<GridCell> Path = new List<GridCell>();

    public GridCell ConnectingIn
    {
        get
        {
            foreach (GridCell cell in SendingNode.GridCells)
            {
                if (Path[0].NeighbourCells.Contains(cell))
                {
                    return cell;
                }
            }
            return null;
        }
    }

    public GridCell ConnectingOut
    {
        get
        {
            foreach (GridCell cell in RecievingNode.GridCells)
            {
                if (Path[Path.Count-1].NeighbourCells.Contains(cell))
                {
                    return cell;
                }
            }
            return null;
        }
    }

    [field: SerializeField] public ConnectionIndicator ConnectionIndicator { get; private set; }

    public NodeConnection(Node inNode)
    {
        SendingNode = inNode;
        SendingNode.ShowConnectionOnPermiter();
        UI.Sound.PlayConnectionStart();
    }

    public bool TrySetOutNode(Node outNode)
    {
        if (!ConnectableNode(outNode)) return false;
        SetOutNode(outNode);
        UI.Sound.PlayConnectionComplete();
        return true;
    }

    public bool TryConnectRoad(GridCell cell)
    {
        if (!ConnectableRoad(cell)) return false;
        AddToPath(cell);
        UI.Sound.PlayConnectionAdd();
        return true;
    }

    public void SetOutNode(Node outNode)
    {
        RecievingNode = outNode;

        bool sendingHasOutReceivingHasIn = false;
        foreach (NodeResource outResource in SendingNode.OutResources)
        {
            foreach (NodeResource inResource in RecievingNode.InResources)
            {
                if (outResource.Config == inResource.Config)
                {
                    sendingHasOutReceivingHasIn = true;
                    break;
                }
            }
            if (sendingHasOutReceivingHasIn) break;
        }

        if (!sendingHasOutReceivingHasIn)
        {
            bool receivingHasOutSendingHasIn = false;
            foreach (NodeResource outResource in RecievingNode.OutResources)
            {
                foreach (NodeResource inResource in SendingNode.InResources)
                {
                    if (outResource.Config == inResource.Config)
                    {
                        receivingHasOutSendingHasIn = true;
                        break;
                    }
                }
                if (receivingHasOutSendingHasIn) break;
            }

            if (receivingHasOutSendingHasIn)
            {
                Node temp = SendingNode;
                SendingNode = RecievingNode;
                RecievingNode = temp;
            }
            else
            {
                Debug.LogError("No compatible resource direction between sending and receiving nodes.");
            }
        }

        UpdatePathIndicator();
        if (ConnectionIndicator != null) ConnectionIndicator.SetTransparent();
        ConnectedNodes();
    }

    public GridCell LastCell
    {
        get
        {
            if (Path.Count == 0) return null;
            return Path[Path.Count - 1];
        }
    }

    private void ConnectedNodes()
    {
        if (SendingNode == null)
        {
            Debug.LogError("InNode is null when trying to connect nodes.");
            return;
        }

        if (RecievingNode == null)
        {
            Debug.LogError("OutNode is null when trying to connect nodes.");
            return;
        }

        SendingNode.ConnectedNodes.Add(RecievingNode);
        RecievingNode.ConnectedNodes.Add(SendingNode);
        Game.AddConnection(this);
    }

    public void SetConnectionIndicator(ConnectionIndicator connectionIndicator)
    {
        ConnectionIndicator = connectionIndicator;
        ConnectionIndicator.Initialize(this);
    }

    public void AddToPath(GridCell gridCell)
    {
        Path.Add(gridCell);
        UpdatePathIndicator();

        Game.Grid.ClearAllConnections();
        foreach (GridCell nCell in gridCell.NeighbourCells)
        {
            if (nCell.Node != null && SendingNode != nCell.Node)
            {
                if (nCell.Node.NodeObject == null)
                {
                    Debug.LogError($"NodeConnection.AddToPath: NodeObject is null on node '{nCell.GridPosition}' while connecting from '{(SendingNode != null ? SendingNode.Name : "null")}'.")
;
                }
                else
                {
                    nCell.Node.NodeObject.SetConnection(true);
                }
                continue;
            }

            if (nCell.Road == null) continue;
            if (Path.Contains(nCell)) continue;
            Game.Grid.ShowConnection(nCell);
        }
    }

    public bool ConnectableNode(Node node)
    {
        if (node == null) return false;
        if (SendingNode == node) return false;
        if (Path.Count == 0) return false;
        bool lastCellNeighbourToNode = false;

        if (node.GridCells == null)
        {
            Debug.LogError($"NodeConnection.ConnectableNode: GridCells is null on node '{node.Name}' while trying to connect from '{(SendingNode != null ? SendingNode.Name : "null")}'.");
            return false;
        }

        foreach (GridCell cell in node.GridCells)
        {
            if (LastCell.NeighbourCells.Contains(cell))
            {
                lastCellNeighbourToNode = true;
                break;
            }
        }
        if (!lastCellNeighbourToNode) return false;
        bool shareResourceConfig = false;
        foreach (NodeResource inResource in SendingNode.InResources)
        {
            foreach (NodeResource outResource in node.OutResources)
            {
                if (inResource.Config == outResource.Config)
                {
                    shareResourceConfig = true;
                    break;
                }
            }
            if (shareResourceConfig) break;
        }
        if (!shareResourceConfig)
        {
            foreach (NodeResource outResource in SendingNode.OutResources)
            {
                foreach (NodeResource inResource in node.InResources)
                {
                    if (outResource.Config == inResource.Config)
                    {
                        shareResourceConfig = true;
                        break;
                    }
                }
                if (shareResourceConfig) break;
            }
        }
        if (!shareResourceConfig) return false;
        return true;
    }

    private bool ConnectableRoad(GridCell cell)
    {
        if (cell.Road == null) return false;
        if (!cell.GridObject.IsConnection) return false;
        if (Path.Contains(cell)) return false;
        if (Path.Count == 0 && !SendingNode.PerimiterCells.Contains(cell)) return false;
        if (Path.Count > 0 && !LastCell.NeighbourCells.Contains(cell)) return false;
        return true;
    }

    ResourceConfig ResourceConfig
    {
        get
        {
            if (SendingNode == null || RecievingNode == null) return null;

            foreach (NodeResource inResource in SendingNode.InResources)
            {
                foreach (NodeResource outResource in RecievingNode.OutResources)
                {
                    if (inResource.Config == outResource.Config)
                    {
                        return inResource.Config;
                    }
                }
            }
            foreach (NodeResource outResource in SendingNode.OutResources)
            {
                foreach (NodeResource inResource in RecievingNode.InResources)
                {
                    if (outResource.Config == inResource.Config)
                    {
                        return outResource.Config;
                    }
                   
                }
            }
            return null;
        }
    }

    public void UpdatePathIndicator()
    {
        if (ConnectionIndicator == null && SendingNode != null && Path.Count > 0)
        {
            SpawnIndicator();
        }

        if (ConnectionIndicator != null) ConnectionIndicator.UpdatePath();
    }

    public void RemoveFromPath(GridCell gridCell)
    {
        Path.Remove(gridCell);
        UpdatePathIndicator();
    }

    public void Show()
    {
        if (ConnectionIndicator != null) ConnectionIndicator.gameObject.SetActive(true);
    }

    public void Hide()
    {        
        if (ConnectionIndicator != null) ConnectionIndicator.gameObject.SetActive(false);
    }

    public void SpawnIndicator()
    {
        if (Game.ConnectionIndicatorPrefab != null)
        {
            ConnectionIndicator = GameObject.Instantiate(Game.ConnectionIndicatorPrefab);
            ConnectionIndicator.Initialize(this);
        }
    }
}
