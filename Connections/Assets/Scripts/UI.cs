using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public static UI Instance { get; private set; }

    [SerializeField] UI_CellInfo cellInfo;
    [SerializeField] UI_ResetButton resetButton;
    [SerializeField] UI_NodeInfo nodeInfo;
    [SerializeField] UI_Objectives objectives;
    [SerializeField] UI_PauseMenu pauseMenu;
    [SerializeField] UI_LevelComplete levelComplete;
    [SerializeField] UI_BuildMenu buildMenu;
    [SerializeField] bool showCellInfo = true;
    [SerializeField] Texture2D defaultBackground;
    [SerializeField] Texture2D selectedBackground;
    [SerializeField] RawImage buildButtonBackground;
    [SerializeField] RawImage deleteButtonBackground;
    [SerializeField] UI_Sound sound;
    [SerializeField] GameObject HUDobj;
    [SerializeField] UI_HowToPlay howToPlay;

    public static UI_CellInfo CellInfo => Instance.cellInfo;
    public static UI_ResetButton ResetButton => Instance.resetButton;
    public static UI_NodeInfo NodeInfo => Instance.nodeInfo;
    public static UI_Objectives Objectives => Instance.objectives;
    public static UI_PauseMenu PauseMenu => Instance.pauseMenu;
    public static UI_LevelComplete LevelComplete => Instance.levelComplete;
    public static UI_BuildMenu BuildMenu => Instance.buildMenu;
    public static UI_Sound Sound => Instance.sound;
    public static UI_HowToPlay HowToPlay => Instance.howToPlay;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this.gameObject);
    }

    public static void UpdatePositionUnderMouse(Vector2Int gridPosition)
    {
        if (Instance == null) return;

        bool validPosition = gridPosition != new Vector2Int(-1, -1);

        if (!validPosition) return;
        GridCell gridCell = Game.Grid.Cell[gridPosition.x, gridPosition.y];


        if (Instance.showCellInfo)
        {
            Instance.cellInfo.gameObject.SetActive(validPosition);
            if (Instance.cellInfo.GridCell != gridCell)
            {
                Instance.cellInfo.Set(gridCell);
            }
        }

        Instance.nodeInfo.gameObject.SetActive(gridCell.Node != null);

        if (gridCell.Node != null)
        {
            if (Instance.nodeInfo.Node != gridCell.Node)
            {
                Instance.nodeInfo.Set(gridCell.Node);
            }
        }
    }

    public static void UpdateObjectives()
    {
        if (Instance == null) return;

        Instance.objectives.gameObject.SetActive(true);

    }

    public static void UpdateButtonBackground()
    {
        switch(Game.Player.PlayerMode)
        {
            case Player.EPlayerMode.Build:
                Instance.buildButtonBackground.texture = Instance.selectedBackground;
                Instance.deleteButtonBackground.texture = Instance.defaultBackground;
                break;
            case Player.EPlayerMode.Delete:
                Instance.buildButtonBackground.texture = Instance.defaultBackground;
                Instance.deleteButtonBackground.texture = Instance.selectedBackground;
                break;
            default:
                Instance.buildButtonBackground.texture = Instance.defaultBackground;
                Instance.deleteButtonBackground.texture = Instance.defaultBackground;
                break;
        }
    }

    public static bool MenuOpen => Instance != null && (Instance.pauseMenu.gameObject.activeSelf || Instance.levelComplete.gameObject.activeSelf);

    public void ShowHUD(bool showingHUD)
    {
        HUDobj.SetActive(showingHUD);
    }
}
