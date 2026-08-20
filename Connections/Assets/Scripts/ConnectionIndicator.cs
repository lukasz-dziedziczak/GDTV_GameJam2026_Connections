using UnityEngine;

public class ConnectionIndicator : MonoBehaviour
{
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] Material defaultMaterial;
    [SerializeField] Material transparentMaterial;
    [SerializeField] float lineHeight = 0.5f;
    NodeConnection connection;

    public void Initialize(NodeConnection nodeConnection)
    {
        connection = nodeConnection;
    }

    public void UpdatePath()
    {
        if (lineRenderer == null || connection == null) return;

        lineRenderer.positionCount = 0;
        int idx = 0;

        if (connection.SendingNode != null && connection.Path != null && connection.Path.Count > 0)
        {
            GridCell inCell = connection.ConnectingIn;
            if (inCell != null)
            {
                Vector3 p = inCell.WorldPosition;
                lineRenderer.positionCount++;
                lineRenderer.SetPosition(idx++, new Vector3(p.x, lineHeight, p.z));
            }
        }

        if (connection.Path == null || connection.Path.Count == 0)
        {
            return;
        }

        foreach (GridCell cell in connection.Path)
        {
            if (cell == null) continue;
            Vector3 p = cell.WorldPosition;
            lineRenderer.positionCount++;
            lineRenderer.SetPosition(idx++, new Vector3(p.x, lineHeight, p.z));
        }

        if (connection.RecievingNode != null)
        {
            GridCell outCell = connection.ConnectingOut;
            if (outCell != null)
            {
                Vector3 p = outCell.WorldPosition;
                lineRenderer.positionCount++;
                lineRenderer.SetPosition(idx++, new Vector3(p.x, lineHeight, p.z));
            }
        }
    }

    public void SetTransparent(bool transparent = true)
    {
        if (lineRenderer == null) return;

        lineRenderer.material = transparent ? transparentMaterial : defaultMaterial;
    }
}


