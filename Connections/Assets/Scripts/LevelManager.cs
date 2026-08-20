using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [SerializeField] LevelConfig[] levels;
    [SerializeField] int currentLevelIndex = -1;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public static void SetLevelIndex(int level)
    {
        if (Instance == null)
        {
            Debug.LogError("LevelManager instance not found.");
            return;
        }

        if (level < -1 || level >= Instance.levels.Length)
        {
            Debug.LogError("Invalid level index: " + level);
            return;
        }
        Instance.currentLevelIndex = level;
    }

    public static LevelConfig CurrentLevelConfig
    {
        get
        {
            if (Instance != null && Instance.currentLevelIndex >= 0 && Instance.currentLevelIndex < Instance.levels.Length)
            {
                return Instance.levels[Instance.currentLevelIndex];
            }
            return null;
        }
    }

    public static bool CanGoToNextLevel
    {
        get
        {
            if (Instance == null) return false;
            return Instance.currentLevelIndex + 1 < Instance.levels.Length;
        }
    }

    public static void GoToNextLevel()
    {
        Instance.currentLevelIndex++;
        if (Instance.currentLevelIndex < Instance.levels.Length)
        {
            LoadScene();
        }
        else GoToStart();

    }

    public static void GoToStart()
    {
        Instance.currentLevelIndex = -1;
        LoadScene();
    }

    public static void LoadScene()
    {
        if (Instance.currentLevelIndex == -1) SceneManager.LoadScene(0);
        else if (Instance.currentLevelIndex < Instance.levels.Length) SceneManager.LoadScene(1);
        else Debug.LogError("No level to load");
    }
}
