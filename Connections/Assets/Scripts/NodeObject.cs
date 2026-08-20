using UnityEngine;

public class NodeObject : MonoBehaviour
{
    public Node Node { get; private set; }
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] Material mouseOverMaterial;
    [SerializeField] Material selectedMaterial;
    [SerializeField] Material connectionMaterial;
    [SerializeField] Material deleteMaterial;
    [SerializeField] NodeIndicator indicator;

    bool mouseOver = false;
    bool selected = false;
    bool connection = false;
    bool delete = false;

    private void Start()
    {
        UpdateMaterial();
    }

    public void SetNode(Node node)
    {
        Node = node;
        indicator.gameObject.SetActive(true);
    }

    public void SetSelected(bool bSelected)
    {
        selected = bSelected;
        UpdateMaterial();
    }

    public void SetMouseOver(bool bMouseOver)
    {
        if (Game.Player.PlayerMode == Player.EPlayerMode.Delete)
        {
            delete = bMouseOver;
        }
        else mouseOver = bMouseOver;
        UpdateMaterial();
    }

    public void SetConnection(bool bConnection)
    {
        connection = bConnection;
        UpdateMaterial();
    }

    public void SetDelete(bool bDelete)
    {
        delete = bDelete;
        UpdateMaterial();
    }

    private void UpdateMaterial()
    {
        meshRenderer.gameObject.SetActive(mouseOver || selected || connection || delete);
        if (mouseOver)
        {
            meshRenderer.material = mouseOverMaterial;
        }
        else if (selected)
        {
            meshRenderer.material = selectedMaterial;
        }
        else if (connection)
        {
            meshRenderer.material = connectionMaterial;
        }
        else if (delete)
        {
            meshRenderer.material = deleteMaterial;
        }
    }
}
