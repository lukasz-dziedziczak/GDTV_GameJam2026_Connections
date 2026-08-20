using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UI_BuildMenu;

public class UI_BuildMenu : MonoBehaviour
{
    [SerializeField] UI_BuildButton buildButtonPrefab;
    [SerializeField] GameObject buildButtonMenu;
    [SerializeField] GameObject buildButtonSubmenu;
    [SerializeField] List<BuildSubmenu> buildSubmenus;
    [SerializeField] BuildMenuItem[] baseBuildMenuItems;
    float submenuHeightBase;
    [SerializeField] float submenuHeightOffset;

    [SerializeField] Texture2D defaultBackground;
    [SerializeField] Texture2D selectedBackground;

    List<UI_BuildButton> buildButtons = new List<UI_BuildButton>();
    List<UI_BuildButton> subbuildButtons = new List<UI_BuildButton>();

    HashSet<NodeConfig> allowedNodeConfigs = new HashSet<NodeConfig>();
    HashSet<int> submenusWithAllowedItems = new HashSet<int>();

    [System.Serializable]
    public class BuildSubmenu
    {
        [field: SerializeField] public BuildMenuItem[] BuildMenuItems { get; private set; }
        [field: SerializeField] public Texture icon;
        [field: SerializeField] public Texture iconBackground;

    }

    private void Start()
    {
        submenuHeightBase = buildButtonSubmenu.GetComponent<RectTransform>().anchoredPosition.y;
        //InitializeForLevel();
    }

    public void InitializeForLevel()
    {
        allowedNodeConfigs.Clear();
        submenusWithAllowedItems.Clear();

        LevelConfig currentLevel = Game.LevelConfig;
        if (currentLevel == null)
        {
            Debug.LogError("CurrentLevelConfig is null");
            return;
        }

        if (currentLevel.RequiredResources == null)
        {
            Debug.LogError("RequiredResources list is null");
            return;
        }

        foreach (NodeResource requiredResource in currentLevel.RequiredResources)
        {
            if (requiredResource.Config == null)
            {
                Debug.LogError("RequiredResource.Config is null");
                continue;
            }
            AddNodeConfigAndDependencies(requiredResource.Config);
        }

        for (int i = 0; i < buildSubmenus.Count; i++)
        {
            foreach (BuildMenuItem buildMenuItem in buildSubmenus[i].BuildMenuItems)
            {
                if (buildMenuItem == null)
                {
                    Debug.LogError($"BuildMenuItem is null in submenu {i}");
                    continue;
                }
                if (buildMenuItem.NodeConfig != null && allowedNodeConfigs.Contains(buildMenuItem.NodeConfig))
                {
                    submenusWithAllowedItems.Add(i);
                    break;
                }
            }
        }
    }

    void AddNodeConfigAndDependencies(ResourceConfig resourceConfig)
    {
        if (resourceConfig == null)
        {
            Debug.LogError("resourceConfig is null in AddNodeConfigAndDependencies");
            return;
        }

        if (buildSubmenus == null)
        {
            Debug.LogError("buildSubmenus is null");
            return;
        }

        foreach (BuildSubmenu submenu in buildSubmenus)
        {
            if (submenu == null)
            {
                Debug.LogError("submenu is null");
                continue;
            }
            if (submenu.BuildMenuItems == null)
            {
                Debug.LogError("submenu.BuildMenuItems is null");
                continue;
            }
            foreach (BuildMenuItem buildMenuItem in submenu.BuildMenuItems)
            {
                if (buildMenuItem == null)
                {
                    Debug.LogError("buildMenuItem is null in submenu");
                    continue;
                }
                if (buildMenuItem.NodeConfig == null)
                {
                    Debug.LogError("buildMenuItem.NodeConfig is null in submenu");
                    continue;
                }
                if (buildMenuItem.NodeConfig.OutResources == null)
                {
                    Debug.LogError("buildMenuItem.NodeConfig.OutResources is null in submenu");
                    continue;
                }
                if (buildMenuItem.NodeConfig.OutResources.Exists(r => r.Config == resourceConfig))
                {
                    if (allowedNodeConfigs.Add(buildMenuItem.NodeConfig))
                    {
                        if (buildMenuItem.NodeConfig.InResources != null)
                        {
                            foreach (NodeResource inResource in buildMenuItem.NodeConfig.InResources)
                            {
                                AddNodeConfigAndDependencies(inResource.Config);
                            }
                        }
                    }
                }
            }
        }
    }

    private void Update()
    {
        if (buildButtonMenu.activeSelf && Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (!RectTransformUtility.RectangleContainsScreenPoint(buildButtonMenu.GetComponent<RectTransform>(), Mouse.current.position.ReadValue(), null) && !RectTransformUtility.RectangleContainsScreenPoint(buildButtonSubmenu.GetComponent<RectTransform>(), Mouse.current.position.ReadValue(), null))
            {
                CloseBuildMenu();
            }
        }
    }

    public void OnBuildButtonPress()
    {
        if (!buildButtonMenu.activeSelf)
        {
            OpenBuildMenu();
        }
        else
        {
            CloseBuildMenu();
        }
        UI.Sound.PlayBuildButtonPress();
    }

    public void OpenBuildMenu()
    {
        buildButtonMenu.SetActive(true);
        foreach(BuildMenuItem buildMenuItem in baseBuildMenuItems)
        {
            UI_BuildButton buildButton = Instantiate(buildButtonPrefab, buildButtonMenu.transform);
            buildButton.Set(this, buildMenuItem);
            buildButtons.Add(buildButton);
        }
        for (int bS = buildSubmenus.Count - 1; bS >= 0; bS--)
        {
            if (submenusWithAllowedItems.Contains(bS))
            {
                UI_BuildButton buildButton = Instantiate(buildButtonPrefab, buildButtonMenu.transform);
                buildButton.SetSubmenu(this, bS, buildSubmenus[bS].icon, buildSubmenus[bS].iconBackground);
                buildButtons.Add(buildButton);
            }
        }
    }

    public void CloseBuildMenu()
    {
        foreach (UI_BuildButton buildButton in buildButtons)
        {
            Destroy(buildButton.gameObject);
        }
        buildButtons.Clear();
        CloseSubmenu();
        buildButtonMenu.SetActive(false);
        Game.MouseOverUI = false;
    }

    public void OpenSubmenu(int submenuIndex)
    {
        SetSubmenuHeight(submenuIndex);

        if (buildButtonSubmenu.activeSelf && subbuildButtons.Count > 0)
        {
            foreach (UI_BuildButton buildButton in subbuildButtons)
            {
                Destroy(buildButton.gameObject);
            }
            subbuildButtons.Clear();
        }

        buildButtonSubmenu.SetActive(true);
        foreach (BuildMenuItem buildMenuItem in buildSubmenus[submenuIndex].BuildMenuItems)
        {
            if (allowedNodeConfigs.Contains(buildMenuItem.NodeConfig))
            {
                UI_BuildButton buildButton = Instantiate(buildButtonPrefab, buildButtonSubmenu.transform);
                buildButton.Set(this, buildMenuItem);
                subbuildButtons.Add(buildButton);
            }
        }
    }

    public void CloseSubmenu()
    {
        foreach (UI_BuildButton buildButton in subbuildButtons)
        {
            Destroy(buildButton.gameObject);
        }
        subbuildButtons.Clear();

        buildButtonSubmenu.SetActive(false);
    }

    void SetSubmenuHeight(int submenuIndex)
    {
        float height = submenuHeightBase + (submenuHeightOffset * submenuIndex);
        buildButtonSubmenu.GetComponent<RectTransform>().anchoredPosition = new Vector2(buildButtonSubmenu.GetComponent<RectTransform>().anchoredPosition.x, height);
    }
}