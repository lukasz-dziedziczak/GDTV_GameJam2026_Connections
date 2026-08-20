using TMPro;
using UnityEngine;

public class UI_CellInfo : MonoBehaviour
{
    [SerializeField] TMP_Text CellCoords;
    [SerializeField] TMP_Text NodeInfo;
    [SerializeField] TMP_Text ConnectionInfo;

    GridCell gridCell;

    public void Set(GridCell gridCell)
    {
        this.gridCell = gridCell;
        CellCoords.text = $"{gridCell.GridPosition}";
        NodeInfo.text = $"{(gridCell.Node != null ? "Node" : "")}";
        ConnectionInfo.text = $"{(gridCell.Road != null ? "Connection" : "")}";
    }

    public GridCell GridCell => gridCell;
}
