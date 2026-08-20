using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ResourceItem : MonoBehaviour
{
    [SerializeField] RawImage resourceIcon;
    [SerializeField] TMP_Text resourceName;
    [SerializeField] TMP_Text resourceAmount;

    public void Set(NodeResource nodeResource)
    {
        resourceIcon.texture = nodeResource.Config.Icon;
        resourceName.text = nodeResource.Config.Name;
        resourceAmount.text = nodeResource.Amount.ToString();
    }
    public float Height
    {
        get
        {
            return GetComponent<RectTransform>().rect.height;
        }
    }
}