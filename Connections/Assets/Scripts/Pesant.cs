using System.Collections.Generic;
using UnityEngine;

public class Pesant : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] float moveSpeed = 1f;
    [SerializeField] float rotationSpeed = 1f;
    public List<NodeResource> Inventory = new List<NodeResource>();
    public List<GridCell> Path = new List<GridCell>();
    public Node OriginNode = null;
    public Node DestinationNode = null;

    const float arrivalThreshold = 0.05f;
    Vector3 destPos = Vector3.zero;
    Vector3 originPos = Vector3.zero;
    [SerializeField] GameObject carryObject;

    int pathIndex = 0;
    bool bWalkingBack = false;

    void Update()
    {
        if (Path == null || Path.Count == 0) return;

        if (destPos == Vector3.zero && DestinationNode != null)
        {
            GridCell lastCell = Path[Path.Count - 1];
            for (int i = 0; i < DestinationNode.GridCells.Count; i++)
            {
                GridCell cell = DestinationNode.GridCells[i];
                if (cell == null) continue;
                if (lastCell.NeighbourCells.Contains(cell))
                {
                    destPos = cell.WorldPosition;
                    break;
                }
            }
        }

        if (originPos == Vector3.zero && OriginNode != null)
        {
            GridCell firstCell = Path[0];
            for (int i = 0; i < OriginNode.GridCells.Count; i++)
            {
                GridCell cell = OriginNode.GridCells[i];
                if (cell == null) continue;
                if (firstCell.NeighbourCells.Contains(cell))
                {
                    originPos = cell.WorldPosition;
                    break;
                }
            }
        }

        Vector3 target;
        bool hasTarget = false;

        if (!bWalkingBack)
        {
            if (pathIndex < Path.Count)
            {
                target = Path[pathIndex].WorldPosition;
                hasTarget = true;
            }
            else if (destPos != Vector3.zero)
            {
                target = destPos;
                hasTarget = true;
            }
            else
            {
                return;
            }
        }
        else
        {
            if (pathIndex >= 0)
            {
                target = Path[pathIndex].WorldPosition;
                hasTarget = true;
            }
            else
            {
                target = originPos;
                hasTarget = true;
            }
        }

        if (!hasTarget) return;

        RotateTowards(target);
        MoveTowards(target);

        if (Vector3.Distance(transform.position, target) <= arrivalThreshold)
        {
            transform.position = target;

            if (!bWalkingBack)
            {
                if (pathIndex < Path.Count)
                {
                    pathIndex++;
                }
                else
                {
                    TransferInventoryToDestination();
                    StopCarrying();
                    bWalkingBack = true;
                    pathIndex = Path.Count - 1;
                }
            }
            else
            {
                if (pathIndex >= 0)
                {
                    pathIndex--;
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }
    }

    void MoveTowards(Vector3 target)
    {
        float step = moveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target, step);
    }

    void RotateTowards(Vector3 target)
    {
        Vector3 dir = target - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude <= 0.0001f) return;
        Quaternion targetRot = Quaternion.LookRotation(dir);
        float angle = Quaternion.Angle(transform.rotation, targetRot);
        if (angle <= 0.5f)
        {
            transform.rotation = targetRot;
            return;
        }
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
    }

    void TransferInventoryToDestination()
    {
        if (DestinationNode == null || Inventory == null || Inventory.Count == 0) return;
        if (DestinationNode.Inventory == null) DestinationNode.Inventory = new List<NodeResource>();

        foreach (var item in Inventory)
        {
            if (item.Config == null || item.Amount <= 0) continue;
            int idx = DestinationNode.Inventory.FindIndex(r => r.Config == item.Config);
            if (idx >= 0)
            {
                var updated = DestinationNode.Inventory[idx];
                updated.Amount += item.Amount;
                DestinationNode.Inventory[idx] = updated;
            }
            else
            {
                DestinationNode.Inventory.Add(new NodeResource(item.Config, item.Amount));
            }
        }

        Inventory.Clear();

        UI.Objectives.UpdateObjectives();

        if (DestinationNode.Type == ENodeType.Gate)
        {
            UI.Sound.PlayGateResourceDelivered();
        }

        if (Game.GameComplete)
        {
            Debug.Log("Game Complete!");
            UI.LevelComplete.gameObject.SetActive(true);
            UI.Sound.PlayLevelComplete();
            Time.timeScale = 0f;
        }
    }

    private void OnDestroy()
    {
        if (OriginNode != null)
        {
            OriginNode.Pesants.Remove(this);
        }
    }

    private void StopCarrying()
    {
        if (carryObject != null) carryObject.SetActive(false);
        if (animator != null) animator.SetBool("carrying", false);
        else Debug.LogWarning("No animator found on pesant to stop carrying animation.");
    }
}
