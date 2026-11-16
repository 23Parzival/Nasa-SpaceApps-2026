using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;

    [SerializeField] private GameObject settingsPanel;

    public static bool IsPaused { get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //start every scene unpaused with the menu hidden
        UnpauseGame();
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            //Escape closes a minigame instead of pausing
            if (GameState.minigameOpen)
            {
                return;
            }

            if (IsPaused)
            {
                UnpauseGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        IsPaused = true;

        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }
    }

    public void UnpauseGame()
    {
        Time.timeScale = 1f;
        IsPaused = false;

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }

    //called by the Main Menu button
    public void GoToMainMenu()
    {
        GameSaveManager.Save();
        SceneManager.LoadScene(SceneFlow.StartSceneName);
    }

    void OnDestroy()
    {
        //leaving the scene while paused (e.g. loading another scene) shouldn't leave the game frozen
        if (IsPaused)
        {
            Time.timeScale = 1f;
            IsPaused = false;
        }
    }
}
