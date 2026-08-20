using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Objective : MonoBehaviour
{
    [SerializeField] RawImage icon;
    [SerializeField] TMP_Text amounts;

    NodeResource nodeResource;

    public ResourceConfig Resource => nodeResource.Config;

    public void Set(NodeResource node)
    {
        nodeResource = node;
        icon.texture = node.Config.Icon;
        amounts.text = "0/" + node.Amount.ToString();
    }

    public void UpdateAmount(int amount)
    {
        amounts.text = amount.ToString() + "/" + nodeResource.Amount.ToString();
    }

    public float Width
    {
        get
        {
            return GetComponent<RectTransform>().rect.width;
        }
    }
}
