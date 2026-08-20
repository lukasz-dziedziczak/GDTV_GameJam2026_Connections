using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_StartMenuButtons : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip buttonPress;
    [SerializeField] UI_HowToPlay howToPlay;

    public UI_HowToPlay HowToPlay => howToPlay;

    public void OnStartButtonPress()
    {
        if (buttonPress != null) 
        {
            audioSource.PlayOneShot(buttonPress);
        }

        LevelManager.GoToNextLevel();
    }

    public void OnExitButtonPress()
    {
        if (buttonPress != null)
        {
            audioSource.PlayOneShot(buttonPress);
        }

        Application.Quit();
    }

    public void OnHowToPlayPress()
    {
        if (howToPlay != null)
        {
            audioSource.PlayOneShot(buttonPress);
            howToPlay.gameObject.SetActive(true);
        }
    }
}
