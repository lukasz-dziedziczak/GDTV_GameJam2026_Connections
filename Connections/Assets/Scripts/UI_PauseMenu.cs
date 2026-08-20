using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_PauseMenu : MonoBehaviour
{
    public void OnResumePress()
    {
        UI.Sound.PlayButtonPress();
        Game.TogglePause();
    }

    public void OnResetPress()
    {
        UI.Sound.PlayButtonPress();
        Time.timeScale = 1;
        SceneManager.LoadScene(1);
    }

    public void OnQuitPress()
    {
        UI.Sound.PlayButtonPress();
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void OnHowToPlayPress()
    {
        UI.Sound.PlayButtonPress();
        UI.HowToPlay.gameObject.SetActive(true);
    }
}
