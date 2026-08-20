using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_BuildButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] RawImage icon;
    [SerializeField] RawImage iconBackground;
    [SerializeField] Button button;

    public BuildMenuItem BuildMenuItem { get; private set; }
    UI_BuildMenu buildMenu;
    public int SubmenuIndex { get; private set; } = -1;

    private void OnEnable()
    {
        if (button != null) button.onClick.AddListener(OnBuildButtonPress);
    }

    private void OnDisable()
    {
        if (button != null) button.onClick.RemoveListener(OnBuildButtonPress);
    }

    public void Set(UI_BuildMenu buildMenu, BuildMenuItem buildMenuItem)
    {
        this.buildMenu = buildMenu;
        BuildMenuItem = buildMenuItem;
        if (buildMenuItem.Icon != null) icon.texture = buildMenuItem.Icon;
        if (buildMenuItem.IconBackground != null) iconBackground.texture = buildMenuItem.IconBackground;
    }

    public void SetSubmenu(UI_BuildMenu buildMenu, int submenuIndex, Texture menuIcon, Texture menuIconBackground)
    {
        this.buildMenu = buildMenu;
        SubmenuIndex = submenuIndex;
        if (menuIcon != null) icon.texture = menuIcon;
        if (menuIconBackground != null) iconBackground.texture = menuIconBackground;
    }

    public void OnBuildButtonPress()
    {
        if (SubmenuIndex != -1)
        {
            buildMenu.OpenSubmenu(SubmenuIndex);
            UI.Sound.PlayBuildButtonPress();
        }
        else
        {
            buildMenu.CloseBuildMenu();
            BuildMenuItem.OnBuildButtonPress();
            UI.Sound.PlayStartBuildingNode();
        }
    }

    public void OnEnterHover()
    {
        if (BuildMenuItem == null || BuildMenuItem.NodeConfig == null) return;
        UI.NodeInfo.gameObject.SetActive(true);
        UI.NodeInfo.Set(BuildMenuItem.NodeConfig);
    }

    public void OnExitHover()
    {
        if (BuildMenuItem == null || BuildMenuItem.NodeConfig == null) return;
        UI.NodeInfo.gameObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Game.MouseOverUI = true;
        OnEnterHover(); 
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnExitHover();
        Game.MouseOverUI = false;
    }
}
