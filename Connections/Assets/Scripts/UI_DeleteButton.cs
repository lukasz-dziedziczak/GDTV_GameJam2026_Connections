using UnityEngine;

public class UI_DeleteButton : MonoBehaviour
{
    public void OnPress()
    {
        Game.Player.EnterDeleteMode();
        UI.Sound.PlayDeleteModePress();
    }
}
