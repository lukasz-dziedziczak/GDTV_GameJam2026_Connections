using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_LevelComplete : MonoBehaviour
{
    [SerializeField] Button continueButton;
    [SerializeField] Button quitButton; 

    private void OnEnable()
    {
        continueButton.interactable = LevelManager.CanGoToNextLevel;
    }

    public void OnContinuePress()
    {
        UI.Sound.PlayLevelComplete();
        // go to next level
        Time.timeScale = 1;
        LevelManager.GoToNextLevel();
    }

    public void OnQuitPress()
    {
        UI.Sound.PlayLevelComplete();
        Time.timeScale = 1;
        LevelManager.GoToStart();
    }
}
