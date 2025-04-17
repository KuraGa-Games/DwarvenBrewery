using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject saveLoadPanel;

    private bool isPaused = false;

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused) Resume();
            else Pause();
        }
    }

    public void Resume()
    {
        BackToPauseMenu();
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    private void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
        saveLoadPanel.SetActive(false);
    }

    public void OpenSaveLoadMenu()
    {
        saveLoadPanel.SetActive(true);
        settingsPanel.SetActive(false);
    }

    public void BackToPauseMenu()
    {
        saveLoadPanel.SetActive(false);
        settingsPanel.SetActive(false);
    }

    public void QuitToMainMenu()
    {
       /* Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");*/
    }
}
