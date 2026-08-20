using UnityEngine;

public class Road : MonoBehaviour
{
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] Material defaultMaterial;
    [SerializeField] Material ghostMaterial;

    public void SetGhost(bool bGhost)
    {
        meshRenderer.material = bGhost ? ghostMaterial : defaultMaterial;
    }
}
