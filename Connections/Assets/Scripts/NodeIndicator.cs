using UnityEngine;
using UnityEngine.UI;

public class NodeIndicator : MonoBehaviour
{
    [SerializeField] NodeObject nodeObject;

    [SerializeField] RawImage nodeImage;
    [SerializeField] Image progress;


    private void OnEnable()
    {
        //transform.LookAt(Camera.main.transform);
        if (nodeObject == null) nodeObject = GetComponentInParent<NodeObject>();
        nodeImage.texture = nodeObject.Node.OutResources[0].Config.Icon;
        progress.fillAmount = 0;
    }

    private void Update()
    {
        if (nodeObject == null || nodeObject.Node == null) return;

        if (nodeObject.Node.IsProducing) progress.fillAmount = 1 - nodeObject.Node.ProductionProgress;
        else progress.fillAmount = 0;
    }
}
