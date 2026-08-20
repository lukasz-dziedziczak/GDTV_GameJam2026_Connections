using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_ResetButton : MonoBehaviour
{
    public void OnButtonPress()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
