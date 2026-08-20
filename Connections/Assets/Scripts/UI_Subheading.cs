using TMPro;
using UnityEngine;

public class UI_Subheading : MonoBehaviour
{
    [SerializeField] TMP_Text subheadingText;

    public void Set(string subheading)
    {
        if (subheadingText == null) return;
        subheadingText.text = subheading;
    }

    public float Height
    {
        get
        {
            return GetComponent<RectTransform>().rect.height;
        }
    }
}
