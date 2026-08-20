using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GridObject : MonoBehaviour
{
    [SerializeField] Canvas canvas;
    [SerializeField] TMP_Text positionText;
    [SerializeField] RawImage border;
    [SerializeField] RawImage gridImage;
    [SerializeField] Image background;
    [SerializeField] Color defaultColor = Color.white;
    [SerializeField] Color selectedColor = Color.yellow;
    [SerializeField] Color mouseOverColor = Color.yellow;
    [SerializeField] Color connectionColor = Color.blue;
    [SerializeField] Color deleteColor = Color.red;
    [SerializeField] bool showDebugPosition = false;

    bool selected = false;
    bool mouseOver = false;
    bool connection = false;
    bool delete = false;

    public bool IsSelected => selected;
    public bool IsMouseOver => mouseOver;
    public bool IsConnection => connection;
    public bool IsDelete => delete;
    public void SetGridPosition(Vector2Int newPosition)
    {
        //GridPosition = newPosition;
        positionText.text = $"{newPosition.x} , {newPosition.y}";
        positionText.gameObject.SetActive(showDebugPosition);
        UpdateColors();
        transform.parent = Game.Instance.transform;
    }

    public void SetSelected(bool bSelected)
    {
        selected = bSelected;
        UpdateColors(); 
    }

    public void SetMouseOver(bool bMouseOver)
    {
        mouseOver = bMouseOver;
        UpdateColors();
    }

    public void SetConnection(bool bConnection)
    {
        connection = bConnection;
        UpdateColors();
    }

    public void SetDeleteing(bool bDeleteing)
    {
        delete = bDeleteing;
        UpdateColors();
    }

    private void UpdateColors()
    {
        if (mouseOver && !delete)
        {
            background.color = mouseOverColor;
            border.color = mouseOverColor;

        }
        else if (selected)
        {
            background.color = selectedColor;
            border.color = selectedColor;
        }
        else if (connection)
        {
            background.color = connectionColor;
            border.color = connectionColor;
        }
        else if (delete)
        {
            background.color = mouseOver?  deleteColor : defaultColor;
            border.color = deleteColor;
        }
        else
        {
            background.color = defaultColor;
            border.color = defaultColor;
        }
    }

    public void ShowGridImage(bool show)
    {
        gridImage.gameObject.SetActive(show);
    }
}
