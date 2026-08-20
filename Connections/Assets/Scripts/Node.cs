using System;
using System.Collections.Generic;
using UnityEngine;


public enum ENodeType
{
    Producer,
    Consumer,
    Converter,
    Gate
}

[System.Serializable]
public class Node
{
    [field: SerializeField] public List<GridCell> GridCells { get; private set; } = new List<GridCell>();
    [field: SerializeField] public string Name { get; private set; }

    public NodeObject NodeObject;
    public ENodeType Type { get; private set; }

    public List<NodeResource> InResources { get; private set; } = new List<NodeResource>();
    public List<NodeResource> OutResources { get; private set; } = new List<NodeResource>();
    public List<Node> ConnectedNodes = new List<Node>();
    public List<NodeResource> Inventory = new List<NodeResource>();

    float productionTime;
    float timer = -1;
    ResourceConfig currentProduction = null;
    public bool IsProducing => timer >= 0;

    public List<Pesant> Pesants = new List<Pesant>();

    public ResourceConfig CurrentProduction => currentProduction;
    public float ProductionProgress => Mathf.Clamp01(timer / productionTime);

    public Node(GridCell gridCell, NodeConfig nodeConfig)
    {
        if (nodeConfig == null)
        {
            Debug.LogError("NodeConfig is null when trying to create a Node. Node creation aborted.");
            return;
        }

        Name = nodeConfig.Name;
        productionTime = nodeConfig.ProductionTime;

        Vector2Int origin = gridCell != null ? gridCell.GridPosition : new Vector2Int(-1, -1);
        Vector2Int size = nodeConfig != null ? nodeConfig.Size : new Vector2Int(1, 1);

        // Assume provided gridCell is the bottom-left (origin) of the footprint
        for (int dx = 0; dx < Mathf.Max(1, size.x); dx++)
        {
            for (int dy = 0; dy < Mathf.Max(1, size.y); dy++)
            {
                int x = origin.x + dx;
                int y = origin.y + dy;
                Vector2Int pos = new Vector2Int(x, y);

                if (!Game.ValidGridPosition(pos))
                {
                    Debug.LogWarning($"Node footprint cell out of bounds: {pos}. Skipping.");
                    continue;
                }

                var cell = Game.Grid.Cell[x, y];
                if (cell == null)
                {
                    Debug.LogWarning($"Grid.Cell[{x},{y}] is null. Skipping.");
                    continue;
                }

                if (cell.Node != null)
                {
                    Debug.LogWarning($"Grid cell {pos} already has a Node. Skipping assigning this node to that cell.");
                    continue;
                }

                GridCells.Add(cell);
                cell.Node = this;
            }
        }

        // Fallback: if no cells were added (e.g., out of bounds), attach to provided cell
        if (GridCells.Count == 0 && gridCell != null)
        {
            GridCells.Add(gridCell);
            gridCell.Node = this;
        }

        Type = nodeConfig != null ? nodeConfig.Type : ENodeType.Producer;
        InResources = Type == ENodeType.Gate ? Game.LevelConfig.RequiredResources : nodeConfig.InResources;
        OutResources = nodeConfig.OutResources;
    }

    public void NodeTick(float DeltaTime)
    {
        ProduceInventory(DeltaTime);
        SendInventoryToConnectedNodes();
    }

    private void ProduceInventory(float DeltaTime)
    {
        if (timer < 0 && HasRequiredResources && !ResourcesProduced)
        {
            RemoveRequiredResourcesFromInventory();
            timer = 0;
        }

        if (timer >= 0)
        {
            timer += DeltaTime;
            if (timer >= productionTime)
            {
                timer = -1;
                AddOutResourcesToInventory();
            }
        }
    }

    private void AddOutResourcesToInventory()
    {
        if (OutResources == null || OutResources.Count == 0) return;
        if (Inventory == null) Inventory = new List<NodeResource>();

        foreach (var outRes in OutResources)
        {
            if (outRes.Config == null) continue;
            int idx = Inventory.FindIndex(r => r.Config == outRes.Config);
            if (idx >= 0)
            {
                var updated = Inventory[idx];
                updated.Amount += outRes.Amount;
                Inventory[idx] = updated;
            }
            else
            {
                Inventory.Add(new NodeResource(outRes.Config, outRes.Amount));
            }
        }
    }

    public bool Satisfied
    {
        get
        {
            if (InResources == null || InResources.Count == 0) return true;
            if (Inventory == null || Inventory.Count == 0) return false;

            List<NodeResource> invCopy = new List<NodeResource>();
            foreach (var r in Inventory)
            {
                invCopy.Add(new NodeResource(r.Config, r.Amount));
            }

            foreach (NodeResource required in InResources)
            {
                if (required.Config == null) return false;
                int idx = invCopy.FindIndex(r => r.Config == required.Config);
                if (idx < 0) return false;
                if (invCopy[idx].Amount < required.Amount) return false;
                var updated = invCopy[idx];
                updated.Amount -= required.Amount;
                invCopy[idx] = updated;
            }

            return true;
        }
    }

    bool HasRequiredResources
    {
        get
        {
            if (Type != ENodeType.Converter) return true;
            if (InResources == null || InResources.Count == 0) return true;
            if (Inventory == null || Inventory.Count == 0) return false;

            List<NodeResource> invCopy = new List<NodeResource>();
            foreach (var r in Inventory)
            {
                invCopy.Add(new NodeResource(r.Config, r.Amount));
            }

            foreach (var required in InResources)
            {
                if (required.Config == null) return false;
                int idx = invCopy.FindIndex(r => r.Config == required.Config);
                if (idx < 0) return false;
                if (invCopy[idx].Amount < required.Amount) return false;
                var updated = invCopy[idx];
                updated.Amount -= required.Amount;
                invCopy[idx] = updated;
            }

            return true;
        }
    }

    bool ResourcesProduced
    {
        get
        {
            if (Type == ENodeType.Consumer) return true;
            if (OutResources == null || OutResources.Count == 0) return true;
            if (Inventory == null || Inventory.Count == 0) return false;

            List<NodeResource> invCopy = new List<NodeResource>();
            foreach (var r in Inventory)
            {
                invCopy.Add(new NodeResource(r.Config, r.Amount));
            }

            foreach (var produced in OutResources)
            {
                if (produced.Config == null) return false;
                int idx = invCopy.FindIndex(r => r.Config == produced.Config);
                if (idx < 0) return false;
                if (invCopy[idx].Amount < produced.Amount) return false;
                var updated = invCopy[idx];
                updated.Amount -= produced.Amount;
                invCopy[idx] = updated;
            }

            return true;
        }
    }

    void RemoveRequiredResourcesFromInventory()
    {
        if (Type != ENodeType.Converter) return;
        if (InResources == null || InResources.Count == 0) return;
        if (Inventory == null || Inventory.Count == 0) return;
        foreach (var required in InResources)
        {
            if (required.Config == null) continue;
            int idx = Inventory.FindIndex(r => r.Config == required.Config);
            if (idx < 0) continue;
            var updated = Inventory[idx];
            updated.Amount -= required.Amount;
            if (updated.Amount <= 0)
                Inventory.RemoveAt(idx);
            else
                Inventory[idx] = updated;
        }
    }

    void SendInventoryToConnectedNodes()
    {
        if (ConnectedNodes == null || ConnectedNodes.Count == 0) return;
        if (Inventory == null || Inventory.Count == 0) return;

        foreach (var node in ConnectedNodes)
        {
            if (node == null || node.InResources == null || node.InResources.Count == 0) continue;

            bool pesantEnRoute = false;
            if (Pesants != null)
            {
                foreach (var p in Pesants)
                {
                    if (p != null && p.DestinationNode == node)
                    {
                        pesantEnRoute = true;
                        break;
                    }
                }
            }
            if (pesantEnRoute) continue;

            foreach (var required in node.InResources)
            {
                if (required.Config == null) continue;

                int nodeHave = 0;
                if (node.Inventory != null)
                {
                    int nIdx = node.Inventory.FindIndex(r => r.Config == required.Config);
                    if (nIdx >= 0) nodeHave = node.Inventory[nIdx].Amount;
                }

                int need = required.Amount - nodeHave;
                if (need <= 0) continue;

                int myIdx = Inventory.FindIndex(r => r.Config == required.Config);
                if (myIdx < 0) continue;
                int avail = Inventory[myIdx].Amount;
                if (avail <= 0) continue;

                int transfer = Math.Min(avail, need);
                if (transfer <= 0) continue;

                var spawned = Game.SpawnPesant(this, node, new NodeResource(required.Config, transfer));
                if (spawned != null)
                {
                    Pesants.Add(spawned);
                }

                var updated = Inventory[myIdx];
                updated.Amount -= transfer;
                if (updated.Amount <= 0)
                    Inventory.RemoveAt(myIdx);
                else
                    Inventory[myIdx] = updated;
            }
        }
    }

    public List<GridCell> PerimiterCells
    {
        get
        {
            List<GridCell> perimeter = new List<GridCell>();
            if (GridCells == null || GridCells.Count == 0) return perimeter;
            foreach (GridCell cell in GridCells)
            {
                if (cell == null) continue;
                foreach (GridCell neighbourCell in cell.NeighbourCells)
                {
                    if (neighbourCell == null) continue;
                    if (!GridCells.Contains(neighbourCell) && !perimeter.Contains(neighbourCell))
                    {
                        perimeter.Add(neighbourCell);
                    }
                }
            }
            foreach (GridCell cell in perimeter.ToArray())
            {
                if (GridCells.Contains(cell))
                {
                    perimeter.Remove(cell);
                }
            }
            return perimeter;
        }
    }

    public List<GridCell> PerimiterRoads
    {
        get
        {
            List<GridCell> roads = new List<GridCell>();
            foreach (GridCell cell in PerimiterCells)
            {
                if (cell.Road != null)
                {
                    roads.Add(cell);
                }
            }
            return roads;
        }
    }


    public List<GridCell> ConnectableCells
    {
        get
        {
            List<GridCell> cells = new List<GridCell>();

            foreach (GridCell cell in PerimiterCells)
            {
                if (cell.GridObject.IsConnection)
                {
                    cells.Add(cell);
                }
            }

            return cells;
        }
    }

    public void ShowConnectionOnPermiter()
    {
        foreach (GridCell cell in PerimiterCells)
        {
            if (cell.Road != null) Game.Grid.ShowConnection(cell);
        }
    }

    public Vector3 WorldPosition
    {
        get
        {
            if (GridCells == null || GridCells.Count == 0) return Vector3.zero;
            Vector3 sum = Vector3.zero;
            int count = 0;
            foreach (GridCell cell in GridCells)
            {
                if (cell == null) continue;
                sum += cell.WorldPosition;
                count++;
            }
            if (count == 0) return Vector3.zero;
            return sum / count;
        }
    }

    public void ShowConnectionIndicators(bool show = true)
    {
        foreach(NodeConnection connection in Game.Connections)
        {
            if (connection.SendingNode == this || connection.RecievingNode == this)
            {
                if (show) connection.Show();
                else connection.Hide();
            }
        }
    }
}
