using UnityEngine;

public class UI_HowToPlay : MonoBehaviour
{
    [SerializeField] UI ui;
    [SerializeField] UI_StartMenuButtons startMenuButtons;

    public void OnPressClose()
    {  
        if (ui != null)
        {
            UI.HowToPlay.gameObject.SetActive(false);
            UI.Sound.PlayButtonPress();
        }
        else if (startMenuButtons != null)
        {
            startMenuButtons.HowToPlay.gameObject.SetActive(false);
        }
    }
}
